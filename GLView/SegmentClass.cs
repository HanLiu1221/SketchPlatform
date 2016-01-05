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
        // apple green: 49,163,84
        // orange red: 255, 153, 102

        public static StrokeStyle strokeStyle = StrokeStyle.Pencil;
        public static GuideLineType GuideLineStyle = GuideLineType.SimpleCross;
        public static int StrokeSize = 6;
        public static Color StrokeColor = Color.FromArgb(37,37,37);//(54, 69, 79);
        public static Color VanLineColor = Color.LightGray;
        public static Color HiddenColor = Color.LightGray;
        public static Color HighlightColor = Color.FromArgb(178, 24, 43);
        public static Color FaceColor = Color.FromArgb(253, 205, 172);
        public static Color AnimColor = Color.FromArgb(251, 128, 114);
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

        public Matrix4d DeserializeJSON(string filename, out Vector3d center)
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
            List<SequenceJson> boxSequences = jsonFile.sequence;
            // repeat boxSequences
            List<string> boxNames = new List<string>();
            for (int i = 0; i < boxSequences.Count; ++i)
            {
                Vector3d[] bbox = null;
                Mesh mesh = null;
                if (boxSequences[i].box != null)
                {
                    string file = path + boxSequences[i].box;
                    bbox = this.loadBoudingbox(file);
                }
                if (boxSequences[i].segment != null)
                {
                    string file = path + boxSequences[i].segment;
                    mesh = new Mesh(file, false);
                }
                if (bbox == null && mesh == null)
                {
                    continue;
                }
                int boxIndex = boxNames.IndexOf(boxSequences[i].box);
                Box box;
                Segment seg;
                if (boxIndex == -1)
                {
                    boxIndex = this.segments.Count;
                    boxNames.Add(boxSequences[i].box);
                    box = new Box(bbox);
                    seg = new Segment(mesh, box);
                    seg.idx = idx++;
                    box.targetLines = new List<List<GuideLine>>();
                    this.segments.Add(seg);
                }
                else
                {
                    seg = this.segments[boxIndex];
                    box = seg.boundingbox;

                }                
                
                // guides
                if (boxSequences[i].guides != null)
                {
                    Vector3d maxcoord = Vector3d.MinCoord();
                    Vector3d mincoord = Vector3d.MaxCoord();
                    List<GuideLine> lines = new List<GuideLine>();
                    foreach (GuideJson guide in boxSequences[i].guides)
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
                        lines.Add(line);
                    }
                    Vector3d v1 = lines[0].v - lines[0].u;
                    Vector3d v2 = lines[1].v - lines[1].u;
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
                    foreach (GuideLine line in lines)
                    {
                        line.setHostPlane(plane);
                    }
                    seg.boundingbox.guideLines.Add(lines);
                }

                // sequence
                string boxIndexString = "box " + boxIndex.ToString();
                string cur = boxIndexString + " ";
                List<GuideLine> targets = new List<GuideLine>();
                if (boxSequences[i].guide_sequence != null && boxSequences[i].guide_sequence.Count > 0)
                {
                    cur += "guideGroup " + (box.guideLines.Count - 1).ToString();
                    cur += " guide ";
                    foreach (GuideSequenceJson seq in boxSequences[i].guide_sequence)
                    {
                        if (seq.guide_indexes != null)
                        {
                            foreach (string s in seq.guide_indexes)
                            {
                                cur += s + " ";
                            }
                        }
                        int last = Int32.Parse(seq.guide_indexes[seq.guide_indexes.Count-1]);
                        targets.Add(box.guideLines[box.guideLines.Count - 1][last]);
                    }
                    box.targetLines.Add(targets);
                }

                if (boxSequences[i].face_to_highlight != null)
                {
                    int nps = boxSequences[i].face_to_highlight.Count;
                    Vector3d[] points = new Vector3d[nps];
                    for (int k = 0; k < nps; ++k)
                    {
                        points[k] = new Vector3d(double.Parse(boxSequences[i].face_to_highlight[k].x),
                            double.Parse(boxSequences[i].face_to_highlight[k].y),
                            double.Parse(boxSequences[i].face_to_highlight[k].z));
                    }
                    Plane face = new Plane(points);
                    cur += " highlightFace " + box.facesToHighlight.Count.ToString();
                    box.facesToHighlight.Add(face);
                }
                // face to draw
                if (boxSequences[i].face_to_draw != null)
                {
                    int nps = boxSequences[i].face_to_draw.Count;
                    Vector3d[] points = new Vector3d[nps];
                    for (int k = 0; k < nps; ++k)
                    {
                        points[k] = new Vector3d(double.Parse(boxSequences[i].face_to_draw[k].x),
                            double.Parse(boxSequences[i].face_to_draw[k].y),
                            double.Parse(boxSequences[i].face_to_draw[k].z));
                    }
                    cur += " faceToDraw " + box.facesToDraw.Count.ToString();
                    Plane face = new Plane(points);
                    box.facesToDraw.Add(face);
                }
                render_sequence.Add(cur);
            }
            render_sequence.Insert(0, "box 0");
            this.sequences = render_sequence.ToArray();
            center = this.NormalizeSegments();
            return modelView;
        }//DeserializeJSON


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

        public void parseASequence(int idx, out int segIdx, out int guidelineGroupIndex, out List<int> guideLineIndexs, 
            out int nextBox, out int highlightFaceIndex, out int drawFaceIndex)
        {
            string seq = this.sequences[idx];
            char[] separator = { '\n', ' ', ':', ';'};
            string[] tokens = seq.Split(separator);
            int i = -1;
            int boxIdx = -1;
            guideLineIndexs = new List<int>();
            nextBox = -1;
            guidelineGroupIndex = -1;
            drawFaceIndex = -1;
            highlightFaceIndex = -1;
            while (++i < tokens.Length)
            {
                if (tokens[i] == "box")
                {
                    boxIdx = Int32.Parse(tokens[++i]);
                    ++i;
                }
                if (i >= tokens.Length) break;
                if (i < tokens.Length && tokens[i] == "guideGroup")
                {
                    guidelineGroupIndex = Int32.Parse(tokens[++i]);
                }
                if (tokens[i] == "guide")
                {
                    while (++i < tokens.Length && tokens[i] != "" && tokens[i] != "highlightFace")
                    {
                        guideLineIndexs.Add(Int32.Parse(tokens[i]));
                    }
                }
                if(i < tokens.Length && tokens[i] == "faceToDraw")
                {
                    drawFaceIndex = Int32.Parse(tokens[++i]);
                }
                if (i < tokens.Length && tokens[i] == "highlightFace")
                {
                    highlightFaceIndex = Int32.Parse(tokens[++i]);
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
