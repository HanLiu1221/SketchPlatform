using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Geometry;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Drawing;

namespace Component
{
    public class SegmentClass
    {
        public List<Segment> segments;
        public enum GuideLineType
        {
            SimpleCross, Random
        }
        public enum StrokeStyle
        {
            Pencil, Pen1, Pen2, Crayon, Ink1, Ink2
        }
        public static StrokeStyle strokeStyle = StrokeStyle.Pencil;
        public static GuideLineType GuideLineStyle = GuideLineType.SimpleCross;
        public static int StrokeSize = 2;
        public static Color StrokeColor = Color.FromArgb(54, 69, 79);
        public static Color VanLineColor = Color.Gray;
        public static Color HiddenColor = Color.LightGray;
        private string[] sequences;

        public SegmentClass()
        { }

        public string[] Sequence
        {
            get
            {
                return this.sequences;
            }
        }
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
                Box c = new Box(bbox);
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
                foreach (GuideLine line in allLines)
                {
                    switch (idx)
                    {
                        case 1:
                            GuideLineStyle = GuideLineType.Random;
                            line.DefineRandomStrokes();
                            break;
                        case 0:
                        default:
                            GuideLineStyle = GuideLineType.SimpleCross;
                            line.DefineCrossStrokes();
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
                        stroke.changeStyle((int)SegmentClass.strokeStyle);
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

        //public Matrix4d DeserializeJSON(string filename, out Vector3d center)
        //{
        //    StreamReader sr = new StreamReader(filename);

        //    string str = sr.ReadToEnd();
        //    //List<string[]> json = new JavaScriptSerializer().Deserialize<List<string[]>>(str);

        //    List<BoxJson> boxes = new JavaScriptSerializer().Deserialize<List<BoxJson>>(str);
        //    this.segments = new List<Segment>();
        //    string path = filename.Substring(0, filename.LastIndexOf('\\') + 1);
        //    Matrix4d modelView = null;
        //    int idx = 0;
        //    for (int i = 0; i < boxes.Count; ++i)
        //    {
        //        if (boxes[i].modelView != null)
        //        {
        //            // read modelview
        //            char[] separator = { ',', ' ', '\n' };
        //            string[] tokens = boxes[i].modelView.Split(separator);
        //            double[] mat = new double[tokens.Length];
        //            for (int j = 0; j < tokens.Length; ++j)
        //            {
        //                mat[j] = double.Parse(tokens[j]);
        //            }
        //            modelView = new Matrix4d(mat);
        //        }
        //        Vector3d[] bbox = null;
        //        Mesh mesh = null;
        //        if (boxes[i].box != null)
        //        {
        //            string file = path + boxes[i].box;
        //            bbox = this.loadBoudingbox(file);
        //        }
        //        if (boxes[i].segment != null)
        //        {
        //            string file = path + boxes[i].segment;
        //            mesh = new Mesh(file, false);
        //        }
        //        if (bbox == null && mesh == null)
        //        {
        //            continue;
        //        }

        //        Box box = new Box(bbox);
        //        Segment seg = new Segment(mesh, box);
        //        seg.idx = idx++;
        //        this.segments.Add(seg);
        //        if (boxes[i].guides != null)
        //        {
        //            Vector3d maxcoord = Vector3d.MinCoord();
        //            Vector3d mincoord = Vector3d.MaxCoord();
        //            foreach (GuideJson guide in boxes[i].guides)
        //            {
        //                Vector3d pfrom = null, pto = null;
        //                if (guide.from != null)
        //                {
        //                    pfrom = new Vector3d(
        //                        double.Parse(guide.from.x),
        //                        double.Parse(guide.from.y),
        //                        double.Parse(guide.from.z));
        //                }
        //                if (guide.to != null)
        //                {
        //                    pto = new Vector3d(
        //                        double.Parse(guide.to.x),
        //                        double.Parse(guide.to.y),
        //                        double.Parse(guide.to.z));
        //                }
        //                maxcoord = Vector3d.Max(maxcoord, pfrom);
        //                maxcoord = Vector3d.Max(maxcoord, pto);
        //                mincoord = Vector3d.Min(mincoord, pfrom);
        //                mincoord = Vector3d.Min(mincoord, pto);
        //                GuideLine line = new GuideLine(pfrom, pto, null, false);
        //                seg.boundingbox.guideLines.Add(line);
        //            }
        //            Vector3d v1 = seg.boundingbox.guideLines[0].v - seg.boundingbox.guideLines[0].u;
        //            Vector3d v2 = seg.boundingbox.guideLines[1].v - seg.boundingbox.guideLines[1].u;
        //            Vector3d normal = v1.Cross(v2).normalize();
        //            Vector3d ends = (maxcoord - mincoord).normalize();
        //            double len = (maxcoord - mincoord).Length() / 2;
        //            Vector3d dir = normal.Cross(ends);
        //            Vector3d c = (maxcoord + mincoord) / 2;
        //            Vector3d[] points = new Vector3d[4];
        //            points[0] = mincoord;
        //            points[1] = c - len * dir;
        //            points[2] = maxcoord;
        //            points[3] = c + len * dir;
        //            Plane plane = new Plane(points);
        //            foreach (GuideLine line in seg.boundingbox.guideLines)
        //            {
        //                line.setHostPlane(plane);
        //            }
        //        }
        //    }
        //    center = this.NormalizeSegments();
        //    return modelView;
        //}//DeserializeJSON

        public Matrix4d DeserializeJSON_new(string filename, out Vector3d center)
        {
            StreamReader sr = new StreamReader(filename);

            string str = sr.ReadToEnd();

            JsonFile jsonFile = new JavaScriptSerializer().Deserialize<JsonFile>(str);

            this.segments = new List<Segment>();
            string path = filename.Substring(0, filename.LastIndexOf('\\') + 1);
            Matrix4d modelView = null;
            int idx = 0;

            List<string> render_sequence = new List<string>();
            char[] separator = { ',', ' ', '\n' };

            if (jsonFile.modelView != null)
            {
                // read modelview
                string[] tokens = jsonFile.modelView.Split(separator);
                double[] mat = new double[tokens.Length];
                for (int j = 0; j < tokens.Length; ++j)
                {
                    mat[j] = double.Parse(tokens[j]);
                }
                modelView = new Matrix4d(mat);
            }
            center = new Vector3d();
            if (jsonFile.sequence == null)
            {
                return modelView;
            }
            List<SequenceJson> boxes = jsonFile.sequence;
            for (int i = 0; i < boxes.Count; ++i)
            {
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

                Box box = new Box(bbox);
                Segment seg = new Segment(mesh, box);
                seg.idx = idx++;
                this.segments.Add(seg);
                // guides
                if (boxes[i].guides != null)
                {
                    Vector3d maxcoord = Vector3d.MinCoord();
                    Vector3d mincoord = Vector3d.MaxCoord();
                    foreach (GuideJson guide in boxes[i].guides)
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
                        GuideLine line = new GuideLine(pfrom, pto, null, false);
                        seg.boundingbox.guideLines.Add(line);
                    }
                    Vector3d v1 = seg.boundingbox.guideLines[0].v - seg.boundingbox.guideLines[0].u;
                    Vector3d v2 = seg.boundingbox.guideLines[1].v - seg.boundingbox.guideLines[1].u;
                    Vector3d normal = v1.Cross(v2).normalize();
                    Vector3d ends = (maxcoord - mincoord).normalize();
                    double len = (maxcoord - mincoord).Length() / 2;
                    Vector3d dir = normal.Cross(ends);
                    Vector3d c = (maxcoord + mincoord) / 2;
                    Vector3d[] points = new Vector3d[4];
                    points[0] = mincoord;
                    points[1] = c - len * dir;
                    points[2] = maxcoord;
                    points[3] = c + len * dir;
                    Plane plane = new Plane(points);
                    foreach (GuideLine line in seg.boundingbox.guideLines)
                    {
                        line.setHostPlane(plane);
                    }
                }

                // sequence
                string cur = "box " + i.ToString();
                if (boxes[i].guide_sequence != null && boxes[i].guide_sequence.Count > 0)
                {
                    foreach (GuideSequenceJson seq in boxes[i].guide_sequence)
                    {
                        cur = "box " + i.ToString();
                        cur += " guide ";
                        if (seq.guide_indexes != null)
                        {
                            foreach (string s in seq.guide_indexes)
                            {
                                cur += s + " ";
                            }
                        }
                        render_sequence.Add(cur);
                    }                    
                }
                else
                {
                    render_sequence.Add(cur);
                }
            }
            this.sequences = render_sequence.ToArray();
            center = this.NormalizeSegments();
            return modelView;
        }//DeserializeJSON_new


        public Vector3d NormalizeSegments()
        {
            Vector3d maxCoord = Vector3d.MinCoord();
            Vector3d minCoord = Vector3d.MaxCoord();
            foreach (Segment seg in this.segments)
            {
                if (seg.mesh == null) continue;
                maxCoord = Vector3d.Max(maxCoord, seg.mesh.MaxCoord);
                minCoord = Vector3d.Min(minCoord, seg.mesh.MinCoord);
            }
            Vector3d center = (maxCoord + minCoord) / 2;
            Vector3d d = maxCoord - minCoord;
            double scale = d.x > d.y ? d.x : d.y;
            scale = d.z > scale ? d.z : scale;
            scale /= 2; // [-1, 1]
            //foreach (Segment seg in this.segments)
            //{
            //    if (seg.mesh == null) continue;
            //    seg.mesh.normalize(center, scale);
            //    seg.boundingbox.normalize(center, scale);
            //}
            return center;
        }// NormalizeSegments

        public void parseASequence(int idx, out int segIdx, out List<int> guideLines, out int nextBox, out int lineToDraw)
        {
            string seq = this.sequences[idx];
            char[] separator = { '\n', ' ', ':', ';'};
            string[] tokens = seq.Split(separator);
            int i = -1;
            int boxIdx = -1;
            guideLines = new List<int>();
            lineToDraw = -1;
            nextBox = -1;
            while (++i < tokens.Length)
            {
                if (tokens[i] == "box")
                {
                    boxIdx = Int32.Parse(tokens[++i]);
                    ++i;
                }
                if (i >= tokens.Length) break;
                if (tokens[i] == "guide")
                {
                    while (++i < tokens.Length && tokens[i] != "" && tokens[i] != "nextBox")
                    {
                        guideLines.Add(Int32.Parse(tokens[i]));
                    }
                }
                if (i < tokens.Length && tokens[i] == "nextBox")
                {
                    nextBox = Int32.Parse(tokens[++i]);
                }
                if (i < tokens.Length && tokens[i] == "line")
                {
                    lineToDraw = Int32.Parse(tokens[++i]);
                }
            }
            segIdx = boxIdx;
        }//parseASequence

        public int LoadGuideSequence(string filename)
        {
            // get sequences
            StreamReader sr = new StreamReader(filename);
            List<string> seqs = new List<string>();
            while (sr.Peek() > -1)
            {
                seqs.Add(sr.ReadLine());
            }
            this.sequences = seqs.ToArray();
            sr.Close();
            return this.sequences.Length;
        }
    }
}
