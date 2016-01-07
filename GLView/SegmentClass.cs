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
using TrimeshWrapper;

namespace Component
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
        // apple green: 49,163,84
        // orange red: 255, 153, 102

        public static StrokeStyle strokeStyle = StrokeStyle.Pencil;
        public static GuideLineType GuideLineStyle = GuideLineType.Random;
        public static int StrokeSize = 4;
        public static int GuideLineSize = 4;
        public static Color StrokeColor = Color.FromArgb(60, 60, 60);//(54, 69, 79);
        public static Color VanLineColor = Color.LightGray;
        public static Color HiddenColor = Color.LightGray;
        public static Color HighlightColor = Color.FromArgb(222, 45, 38);
        public static Color FaceColor = Color.FromArgb(253, 205, 172);
        public static Color AnimColor = Color.FromArgb(251, 128, 114);
        public static Color HiddenLineColor = Color.FromArgb(120, 120, 120);
        public static Color HiddenGuideLinecolor = Color.FromArgb(158, 202, 225);
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
                        case 0:
                            GuideLineStyle = GuideLineType.Random;
                            line.DefineRandomStrokes();
                            break;
                        case 1:
                        default:
                            GuideLineStyle = GuideLineType.SimpleCross;
                            line.DefineCrossStrokes();
                            break;
                    }                    
                }
            }
            this.perturbStrokeColor();
        } // ChangeGuidelineStyle

        public void ChangeStrokeStyle(Matrix4d T, Camera camera)
		{
            foreach(Segment seg in this.segments)
            {
                foreach (List<GuideLine> lines in seg.boundingbox.guideLines)
                {
                    foreach (GuideLine line in lines)
                    {
                        foreach (Stroke stroke in line.strokes)
                        {
                            stroke.setStrokeSize(GuideLineSize);
                        }
                    }
                }
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine edge in allLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.changeStyle((int)SegmentClass.strokeStyle);
                    }
                }
            }
            this.perturbStrokeColor();
        }// ChangeStrokeStyle

        public void perturbStrokeColor()
        {
            foreach (Segment seg in this.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    for (int i = 1; i < edge.strokes.Count; ++i)
                    {
                        Stroke stroke = edge.strokes[i];
                        stroke.strokeColor = SegmentClass.HiddenLineColor;
                    }
                }
            }
        }//perturbStrokeColor

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
                        box.guideLines[box.guideLines.Count - 1][last].isGuide = true;
                    }
                }
                Plane faceToHighlight = null;
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
                    faceToHighlight = face;
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

                if (faceToHighlight != null)
                {
                    foreach (GuideLine line in seg.boundingbox.guideLines[seg.boundingbox.guideLines.Count - 1])
                    {
                        line.setHostPlane(faceToHighlight);
                    }
                }
            }
            render_sequence.Insert(0, "box 0");
            this.sequences = render_sequence.ToArray();
            //center = this.NormalizeSegmentsToBox();
            return modelView;
        }//DeserializeJSON


        public Vector3d NormalizeSegmentsToBox()
        {
            Vector3d maxCoord = Vector3d.MinCoord();
            Vector3d minCoord = Vector3d.MaxCoord();
            Vector3d m_maxCoord = Vector3d.MinCoord();
            Vector3d m_minCoord = Vector3d.MaxCoord();
            foreach (Segment seg in this.segments)
            {
                if (seg.mesh == null) continue;
                m_maxCoord = Vector3d.Max(m_maxCoord, seg.mesh.MaxCoord);
                m_minCoord = Vector3d.Min(m_minCoord, seg.mesh.MinCoord);
                maxCoord = Vector3d.Max(maxCoord, seg.boundingbox.points[6]);
                minCoord = Vector3d.Min(minCoord, seg.boundingbox.points[0]);
            }
            Vector3d center = (maxCoord + minCoord) / 2;
            Vector3d m_d = m_maxCoord - m_minCoord;
            Vector3d b_d = maxCoord - minCoord;
            double scale = m_d.x > m_d.y ? m_d.x : m_d.y;
            scale = m_d.z > scale ? m_d.z : scale;
            //scale /= 2; // [-1, 1]
            double b_scale = b_d.x > b_d.y ? b_d.x : b_d.y;
            b_scale = b_d.z > b_scale ? b_d.z : b_scale;

            scale = scale / b_scale;
            foreach (Segment seg in this.segments)
            {
                if (seg.mesh == null) continue;
                seg.mesh.normalize(center, scale);
                //seg.boundingbox.normalize(center, scale);
            }
            return center;
        }// NormalizeSegmentsToBox

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

        public List<Mesh> currMeshes;
        public List<Vector3d> contourPoints;

        public void calculateContourPoint(Matrix4d trans, Vector3d eye)
        {
            this.currMeshes = new List<Mesh>();
            this.contourPoints = new List<Vector3d>();
            double thresh = 0.1;
            // current pos, normal
            foreach (Segment seg in this.segments)
            {
                if (seg.mesh == null) continue;
                double[] vertexPos = new double[seg.mesh.VertexPos.Length];
                for (int i = 0, j = 0; i < seg.mesh.VertexCount; ++i, j += 3)
                {
                    Vector3d v0 = new Vector3d(seg.mesh.VertexPos[j],
                        seg.mesh.VertexPos[j + 1],
                        seg.mesh.VertexPos[j + 2]);
                    Vector3d v1 = (trans * new Vector4d(v0, 1)).ToVector3D();
                    vertexPos[j] = v1.x;
                    vertexPos[j + 1] = v1.y;
                    vertexPos[j + 2] = v1.z;
                }
                // transformed mesh
                Mesh m = new Mesh(seg.mesh, vertexPos);
                currMeshes.Add(m);
                for (int i = 0, j = 0; i < m.VertexCount; ++i, j += 3)
                {
                    Vector3d v0 = new Vector3d(m.VertexPos[j],
                        m.VertexPos[j + 1],
                        m.VertexPos[j + 2]);
                    Vector3d vn = new Vector3d(m.VertexNormal[j],
                        m.VertexNormal[j + 1],
                        m.VertexNormal[j + 2]).normalize();
                    Vector3d v = (eye - v0).normalize();
                    double cosv = v.Dot(vn);
                    if (Math.Abs(cosv) < thresh && cosv > 0)
                    {
                        this.contourPoints.Add(new Vector3d(seg.mesh.VertexPos[j],
                        seg.mesh.VertexPos[j + 1],
                        seg.mesh.VertexPos[j + 2]));
                    }
                }
            }// fore each segment
        }//calculateContourPoint

        public void computeContourEdges(Matrix4d trans, Vector3d eye)
        {
            this.currMeshes = new List<Mesh>();
            this.contourPoints = new List<Vector3d>();
            double thresh = 0.1;

            // current pos, normal
            foreach (Segment seg in this.segments)
            {
                if (seg.mesh == null) continue;
                double[] vertexPos = new double[seg.mesh.VertexPos.Length];
                for (int i = 0, j = 0; i < seg.mesh.VertexCount; ++i, j += 3)
                {
                    Vector3d v0 = new Vector3d(seg.mesh.VertexPos[j],
                        seg.mesh.VertexPos[j + 1],
                        seg.mesh.VertexPos[j + 2]);
                    Vector3d v1 = (trans * new Vector4d(v0, 1)).ToVector3D();
                    vertexPos[j] = v1.x;
                    vertexPos[j + 1] = v1.y;
                    vertexPos[j + 2] = v1.z;
                }
                
                // transformed mesh
                Mesh m = new Mesh(seg.mesh, vertexPos);
                currMeshes.Add(m);

                // check the sign change of each edge
                foreach (HalfEdge edge in m.HalfEdges)
                {
                    if (edge.invHalfEdge == null) // boudnary
                    {
                        this.contourPoints.Add(seg.mesh.getVertexPos(edge.FromIndex));
                        this.contourPoints.Add(seg.mesh.getVertexPos(edge.ToIndex));
                        continue;
                    }
                    if (edge.invHalfEdge.index < edge.index)
                    {
                        continue; // checked
                    }
                    int fidx = edge.FaceIndex;
                    int invfidx = edge.invHalfEdge.FaceIndex;
                    Vector3d v1 = m.getFaceCenter(fidx);
                    Vector3d v2 = m.getFaceCenter(invfidx);
                    Vector3d e1 = (eye - v1).normalize();
                    Vector3d e2 = (eye - v2).normalize();
                    Vector3d n1 = m.getFaceNormal(fidx);
                    Vector3d n2 = m.getFaceNormal(invfidx);
                    double c1 = e1.Dot(n1);
                    double c2 = e2.Dot(n2);
                    if (c1 * c2 <= 0)
                    {
                        this.contourPoints.Add(seg.mesh.getVertexPos(edge.FromIndex));
                        this.contourPoints.Add(seg.mesh.getVertexPos(edge.ToIndex));
                    }
                }
            }// fore each segment
        }//computeContourEdges
    }
}
