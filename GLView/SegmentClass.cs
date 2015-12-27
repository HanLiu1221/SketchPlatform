using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Geometry;

namespace SketchPlatform
{
    public class SegmentClass
    {
        public List<Segment> segments;
        public enum StrokeStyle
        {
            Pencil, Pen1, Pen2, Crayon, ink1, ink2
        }
        public StrokeStyle strokeStyle = 0;

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
                    this.strokeStyle = StrokeStyle.ink1;
                    break;
                case 5:
                default:
                    this.strokeStyle = StrokeStyle.ink2;
                    break;
            }
            this.ChangeStrokeStyle();
        }

        private void ChangeStrokeStyle()
		{
            foreach(Segment seg in this.segments)
            {
                foreach(GuideLine edge in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.changeStyle((int)this.strokeStyle);
                    }
                }
            }
        }// ChangeStrokeStyle

        public int drawOrTexture()
        {
            if (this.strokeStyle == 0 || (int)this.strokeStyle == 3 || (int)this.strokeStyle == 4)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
