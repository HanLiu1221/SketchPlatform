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
        public static StrokeStyle strokeStyle = StrokeStyle.Ink1;
        public GuideLineType guideLineStyle = GuideLineType.SimpleCross;
        public static int strokeSize = 2;

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
                    SegmentClass.strokeStyle = StrokeStyle.Pencil;
                    break;
                case 1:
                    SegmentClass.strokeStyle = StrokeStyle.Pen1;
                    break;
                case 2:
                    SegmentClass.strokeStyle = StrokeStyle.Pen2;
                    break;
                case 3:
                    SegmentClass.strokeStyle = StrokeStyle.Crayon;
                    break;
                case 4:
                    SegmentClass.strokeStyle = StrokeStyle.Ink1;
                    break;
                case 5:
                default:
                    SegmentClass.strokeStyle = StrokeStyle.Ink2;
                    break;
            }
        }// setStrokeStyle

        public void ChangeGuidelineStyle(int idx)
        {
            foreach (Segment seg in this.segments)
            {
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine edge in allLines)
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
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine edge in allLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.changeStyle((int)SegmentClass.strokeStyle, T, camera);
                    }
                }
            }
        }// ChangeStrokeStyle

        public bool shadedOrTexture()
        {
            if ((int)SegmentClass.strokeStyle == 3)// || (int)this.strokeStyle == 4)
            {
                return false;
            }
            else
            {
                return true;
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
                if (bbox == null && mesh == null)
                {
                    continue;
                }

                Cube c = new Cube(bbox);
                Segment seg = new Segment(mesh, c);
                seg.idx = idx++;
                this.segments.Add(seg);
                if (boxes[i].guides != null)
                {
                    Vector3d maxcoord = Vector3d.MinCoord();
                    Vector3d mincoord = Vector3d.MaxCoord();
                    foreach (Guide guide in boxes[i].guides)
                    {
                        Vector3d pfrom = null, pto = null;
                        if (guide.from != null)
                        {
                            pfrom = new Vector3d(
                                double.Parse(guide.from.x),
                                double.Parse(guide.from.y),
                                double.Parse(guide.from.z));
                        }
                        if (guide.to != null)
                        {
                            pto = new Vector3d(
                                double.Parse(guide.to.x),
                                double.Parse(guide.to.y),
                                double.Parse(guide.to.z));
                        }
                        maxcoord = Vector3d.Max(maxcoord, pfrom);
                        maxcoord = Vector3d.Max(maxcoord, pto);
                        mincoord = Vector3d.Min(mincoord, pfrom);
                        mincoord = Vector3d.Min(mincoord, pto);
                        GuideLine line = new GuideLine(pfrom, pto, null);
                        seg.boundingbox.guideLines.Add(line);
                    }
                    Vector3d v1 = seg.boundingbox.guideLines[0].v - seg.boundingbox.guideLines[0].u;
                    Vector3d v2 = seg.boundingbox.guideLines[1].v - seg.boundingbox.guideLines[1].u;
                    Vector3d normal = v1.Cross(v2).normalize();
                    Vector3d ends = (maxcoord - mincoord).normalize();
                    double len = (maxcoord - mincoord).Length() / 2;
                    Vector3d dir = normal.Cross(ends);
                    Vector3d center = (maxcoord + mincoord) / 2;
                    Vector3d[] points = new Vector3d[4];
                    points[0] = mincoord;
                    points[1] = center - len * dir;
                    points[2] = maxcoord;
                    points[3] = center + len * dir;
                    Plane plane = new Plane(points);
                    foreach (GuideLine line in seg.boundingbox.guideLines)
                    {
                        line.setHostPlane(plane);
                    }
                }
            }
            return modelView;            
        }//DeserializeJSON
        
    }
}
