using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Geometry;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;


namespace SketchPlatform
{
    public class SegmentClass
    {
        public List<Segment> segments;
        public enum GuideLineType
        {
            Random, SimpleCross
        }
        public enum StrokeStyle
        {
            Pencil, Pen1, Pen2, Crayon, Ink1, Ink2
        }
        public StrokeStyle strokeStyle = StrokeStyle.Ink1;
        public GuideLineType guideLineStyle = GuideLineType.SimpleCross;

        public SegmentClass()
        { }

        public void ReadSegments(string segFolder, string bboxfolder)
        {
            this.segments = new List<Segment>();
            string[] meshfiles = Directory.GetFiles(segFolder, "*.ply");
            string[] bboxfiles = Directory.GetFiles(bboxfolder, "*.ply");
            if (meshfiles.Length != bboxfiles.Length)
            {
                Console.WriteLine("segments and bounding boxes are not matching.");
                return;
            }

            for (int i = 0; i < meshfiles.Length; ++i)
            {
                Mesh mesh = new Mesh(meshfiles[i], false);
                Vector3d[] bbox = this.loadBoudingbox(bboxfiles[i]);
                Cube c = new Cube(bbox);
                Segment seg = new Segment(mesh, c);
                seg.idx = i;
                this.segments.Add(seg);
            }
        }//ReadSegments

        public Vector3d[] loadBoudingbox(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string line = "";
            char[] separator = new char[] { ' ', '\t' };
            int n = 0;
            while (sr.Peek() > -1)
            {
                line = sr.ReadLine();
                string[] array = line.Split(separator);
                if (array.Length > 0 && array[0].Equals("end_header"))
                {
                    break;
                }
                if (array.Length > 1 && array[0].Equals("element") && array[1].Equals("vertex"))
                {
                    n = Int32.Parse(array[2]);
                }
            }
            Vector3d[] points = new Vector3d[n];
            int[] ids = { 0, 1, 3, 2, 7, 6, 4, 5 };
            for (int i = 0; i < n; ++i)
            {
                line = sr.ReadLine();
                string[] array = line.Split(separator);
                if (array.Length < 3) break;
                points[ids[i]] = new Vector3d(double.Parse(array[0]),
                    double.Parse(array[1]),
                    double.Parse(array[2]));
            }
            return points;
        }//loadBoudingbox

        public void setStrokeStyle(int idx)
        {
            switch (idx)
            {
                case 0:
                    this.strokeStyle = StrokeStyle.Pencil;
                    break;
                case 1:
                    this.strokeStyle = StrokeStyle.Pen1;
                    break;
                case 2:
                    this.strokeStyle = StrokeStyle.Pen2;
                    break;
                case 3:
                    this.strokeStyle = StrokeStyle.Crayon;
                    break;
                case 4:
                    this.strokeStyle = StrokeStyle.Ink1;
                    break;
                case 5:
                default:
                    this.strokeStyle = StrokeStyle.Ink2;
                    break;
            }
        }// setStrokeStyle

        public void ChangeGuidelineStyle(int idx)
        {
            foreach (Segment seg in this.segments)
            {
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    switch (idx)
                    {
                        case 1:
                            edge.DefineRandomStrokes();
                            break;
                        case 0:
                        default:
                            edge.DefineCrossStrokes();
                            break;
                    }                    
                }
            }
        } // ChangeGuidelineStyle

        public void ChangeStrokeStyle(Matrix4d T, Camera camera)
		{
            foreach(Segment seg in this.segments)
            {
                foreach(GuideLine edge in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.changeStyle((int)this.strokeStyle, T, camera);
                    }
                }
            }
        }// ChangeStrokeStyle

        public int shadedOrTexture()
        {
            if ((int)this.strokeStyle == 3)// || (int)this.strokeStyle == 4)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }//shadedOrTexture

        public Matrix4d DeserializeJSON(string filename)
        {
            StreamReader sr = new StreamReader(filename);

            string str = sr.ReadToEnd();
            //List<string[]> json = new JavaScriptSerializer().Deserialize<List<string[]>>(str);

            List<Box> boxes = new JavaScriptSerializer().Deserialize<List<Box>>(str);
            this.segments = new List<Segment>();
            string path = filename.Substring(0, filename.LastIndexOf('\\') + 1);
            Matrix4d modelView = null;
            int idx = 0;
            for (int i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].modelView != null)
                {
                    // read modelview
                    char[] separator = { ',', ' ', '\n' };
                    string[] tokens = boxes[i].modelView.Split(separator);
                    double[] mat = new double[tokens.Length];
                    for (int j = 0; j < tokens.Length; ++j)
                    {
                        mat[j] = double.Parse(tokens[j]);
                    }
                    modelView = new Matrix4d(mat);
                }
                Vector3d[] bbox = null;
                Mesh mesh = null;
                if (boxes[i].box != null)
                {
                    string file = path + boxes[i].box;
                    bbox = this.loadBoudingbox(file);
                }
                if (boxes[i].segment != null)
                {
                    string file = path + boxes[i].segment;
                    mesh = new Mesh(file, false);
                }
                if (bbox != null || mesh != null)
                {
                    Cube c = new Cube(bbox);
                    Segment seg = new Segment(mesh, c);
                    seg.idx = idx++;
                    this.segments.Add(seg);
                }
            }

            return modelView;
            
        }//DeserializeJSON
    }
}
