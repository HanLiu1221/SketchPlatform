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
    public unsafe class SegmentClass
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
        public static double StrokeSize = 2;
        public static double GuideLineSize = 2;
        public static Color StrokeColor = Color.FromArgb(110, 110, 110);//(54, 69, 79);
        public static Color sideStrokeColor = Color.FromArgb(120, 120, 120);//(54, 69, 79);
        public static Color VanLineColor = Color.FromArgb(210, 210, 210);
        public static Color HiddenColor = Color.FromArgb(170, 170, 170);
        public static Color HighlightColor = Color.FromArgb(222, 45, 38);
        public static Color FaceColor = Color.FromArgb(253, 205, 172);
        public static Color AnimColor = Color.FromArgb(251, 128, 114);
        public static Color HiddenLineColor = Color.FromArgb(120, 120, 120);
        public static Color HiddenGuideLinecolor = Color.FromArgb(116, 169, 207);
        public static Color HiddenHighlightcolor = Color.FromArgb(251, 106, 74);
        
        // guide line colors
        public static Color OneHafColor = Color.FromArgb(77, 175, 74);
        public static Color OneThirdColor = Color.FromArgb(231, 41, 138);
        public static Color OnequarterColor = Color.FromArgb(56, 108, 176);
        public static Color OneSixthColor = Color.FromArgb(166, 216, 84);
        public static Color ReflectionColor = Color.FromArgb(228, 26, 28);

        public static Color GuideLineWithTypeColor = Color.FromArgb(252, 141, 89);
        public static Color ArrowColor = Color.FromArgb(116, 196, 118);

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
            //this.perturbStrokeColor();
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
            //this.perturbStrokeColor();
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

        public Matrix4d DeserializeJSON(string filename, out Vector3d center, out List<string> pageNumbers)
        {
            StreamReader sr = new StreamReader(filename);

            string str = sr.ReadToEnd();

            JsonFile jsonFile = new JavaScriptSerializer().Deserialize<JsonFile>(str);

            this.segments = new List<Segment>();
            string path = filename.Substring(0, filename.LastIndexOf('\\') + 1);
            Matrix4d modelView = null;
            int idx = 0;
            pageNumbers = new List<string>();

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
            int page = 1;
            char subpage = 'a';
            for (int i = 0; i < boxSequences.Count; ++i)
            {
                Vector3d[] bbox = null;
                Mesh mesh = null;
                string meshfileName = "";
                if (boxSequences[i].box != null)
                {
                    string file = path + boxSequences[i].box;
                    bbox = this.loadBoudingbox(file);
                }
                if (boxSequences[i].segment != null)
                {
                    string file = path + boxSequences[i].segment;
                    mesh = new Mesh(file, false);
                    meshfileName  = file;
                }
                if (bbox == null && mesh == null)
                {
                    continue;
                }
                int boxIndex = boxNames.IndexOf(boxSequences[i].box);
                
                Segment seg = null;
                if (boxIndex == -1)
                {
                    boxIndex = this.segments.Count;
                    boxNames.Add(boxSequences[i].box);
                    Box newBox = new Box(bbox);
                    Segment newSeg = new Segment(mesh, newBox);
                    newSeg.idx = idx++;
                    this.segments.Add(newSeg);
                    seg = newSeg;
                }
                else
                {
                    seg = this.segments[boxIndex];
                    if (mesh != null && seg.mesh == null)
                    {
                        seg.mesh = mesh;
                    }
                }
                if (meshfileName != "")
                {
                    seg.loadTrieMesh(meshfileName);
                }

                Box box = seg.boundingbox;
                if (boxSequences[i].hasGuides != null && boxSequences[i].hasGuides.Count > 0)
                {
                    box.guideBoxSeqenceIdx = new List<int>();
                    char[] ss = { ',' };
                    box.guideBoxIdx = Int32.Parse(boxSequences[i].hasGuides[0]);
                    box.guideBoxSeqGroupIdx = Int32.Parse(boxSequences[i].hasGuides[1]);
                    for (int t = 2; t < boxSequences[i].hasGuides.Count; ++t)
                    {
                        int id = Int32.Parse(boxSequences[i].hasGuides[t]);
                        box.guideBoxSeqenceIdx.Add(id);
                    }
                    // add one sequence to just show the guide face
                    string s = "box " + boxIndex.ToString();
                    s += " drawOnlyguides";
                    render_sequence.Add(s);
                    ++page;
                    pageNumbers.Add(page.ToString());
                }

                // arrows
                if (boxSequences[i].arrows != null)
                {
                    List<Vector3d> arrowVecs = new List<Vector3d>();
                    foreach (GuideJson arrow in boxSequences[i].arrows)
                    {
                        Vector3d pfrom = null, pto = null;
                        if (arrow.from != null)
                        {
                            pfrom = new Vector3d(
                                double.Parse(arrow.from.x),
                                double.Parse(arrow.from.y),
                                double.Parse(arrow.from.z));
                        }
                        if (arrow.to != null)
                        {
                            pto = new Vector3d(
                                double.Parse(arrow.to.x),
                                double.Parse(arrow.to.y),
                                double.Parse(arrow.to.z));
                        }
                        arrowVecs.Add(pfrom);
                        arrowVecs.Add(pto);
                    }
                    box.arrows = new List<Arrow3D>();
                    if (arrowVecs.Count > 2)
                    {
                        Vector3d vf = arrowVecs[2] - arrowVecs[0];
                        Vector3d va = arrowVecs[1] - arrowVecs[0];
                        Vector3d vn = va.Cross(vf).normalize();
                        for (int j = 0; j < arrowVecs.Count; j += 2)
                        {
                            Arrow3D arrow = new Arrow3D(arrowVecs[j], arrowVecs[j + 1], vn);
                            box.arrows.Add(arrow);
                        }
                    }
                    // draw arrows before guides start if there is any
                    string astr = "box " + boxIndex.ToString();
                    astr += " arrows";
                    render_sequence.Add(astr);
                    ++page;
                    pageNumbers.Add(page.ToString());
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
                List<string> curGuides = new List<string>();
                
                if (boxSequences[i].guide_sequence != null && boxSequences[i].guide_sequence.Count > 0)
                {
                    cur += "guideGroup " + (box.guideLines.Count - 1).ToString();
                    cur += " guide ";
                    string cur_backup = new string(cur.ToArray());
                    curGuides.Add(boxIndexString + " ");
                    ++page;
                    pageNumbers.Add(page.ToString());
                    foreach (GuideSequenceJson seq in boxSequences[i].guide_sequence)
                    {                        
                        string separateGuideLineIndx = "";
                        subpage = 'a';
                        if (seq.guide_indexes != null)
                        {
                            foreach (string s in seq.guide_indexes)
                            {
                                //cur += s + " ";
                                separateGuideLineIndx += s + " ";
                                cur = new string(cur_backup.ToArray());
                                cur += separateGuideLineIndx;
                                curGuides.Add(cur);
                                string pstr = page.ToString() + new string(subpage, 1);
                                subpage = (char)(subpage + 1);
                                pageNumbers.Add(pstr);
                            }
                        }
                        int last = Int32.Parse(seq.guide_indexes[seq.guide_indexes.Count-1]);
                        box.guideLines[box.guideLines.Count - 1][last].isGuide = true;
                        if (seq.type != null)
                        {
                            box.guideLines[box.guideLines.Count - 1][last].type = Int32.Parse(seq.type);
                        }
                        ++page;
                        //curGuides.Add(cur);
                        //cur = new string(cur_backup.ToArray());
                    }
                    --page;
                }
                if (curGuides.Count == 0)
                {
                    curGuides.Add(cur);
                    ++page;
                    pageNumbers.Add(page.ToString());
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
                    for (int k = 0; k < curGuides.Count; ++k)
                    {
                        curGuides[k] += " highlightFace " + box.facesToHighlight.Count.ToString();
                    }
                    curGuides[0] += " blinking ";
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
                    for (int k = 0; k < curGuides.Count; ++k)
                    {
                        curGuides[k] += " faceToDraw " + box.facesToDraw.Count.ToString();
                    }
                    Plane face = new Plane(points);
                    box.facesToDraw.Add(face);
                }

                if (boxSequences[i].previous_guides != null && boxSequences[i].previous_guides.Count > 0)
                {
                    for (int k = 0; k < curGuides.Count; ++k)
                    {
                        curGuides[k] += " previous_guide_group_id " + boxSequences[i].previous_guides[0];
                        for (int j = 1; j < boxSequences[i].previous_guides.Count; ++j)
                        {
                            curGuides[k] += " " + boxSequences[i].previous_guides[j];
                        }
                    }
                }
                foreach (string s in curGuides)
                {
                    render_sequence.Add(s);
                }

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
            pageNumbers.Insert(0, "1");
            ++page;
            pageNumbers.Add(page.ToString());
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

        public List<int> parseBoxSeqIndex()
        {
            char[] separator = { '\n', ' ', ':', ';' };
            List<int> seqIdx = new List<int>();
            for (int i = 0; i < this.sequences.Length; ++i)
            {
                string[] tokens = this.sequences[i].Split(separator);
                int j = 0;  
                while (j < tokens.Length)
                {
                    if (tokens[j] == "box")
                    {
                        int boxIdx = Int32.Parse(tokens[j+1]);
                        seqIdx.Add(boxIdx);
                        break;
                    }
                    ++j;
                }
            }
            return seqIdx;
        }

        public void parseASequence(int idx, out int segIdx, out int guidelineGroupIndex, out List<int> guideLineIndexs, 
            out int nextBox, out int highlightFaceIndex, out int drawFaceIndex, out bool showBlinking, 
            out bool showOnlyGuides, out bool drawArrow, out List<int> previousGuideGroupIds)
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
            showBlinking = false;
            showOnlyGuides = false;
            drawArrow = false;
            previousGuideGroupIds = new List<int>();
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
                if (i < tokens.Length && tokens[i] == "blinking")
                {
                    showBlinking = true;
                }
                if (i < tokens.Length && tokens[i] == "drawOnlyguides")
                {
                    showOnlyGuides = true;
                    break;
                }
                if (i < tokens.Length && tokens[i] == "arrows")
                {
                    drawArrow = true;
                    break;
                }
                if (i < tokens.Length && tokens[i] == "previous_guide_group_id")
                {
                    while (++i < tokens.Length && tokens[i] != "")
                    {
                        previousGuideGroupIds.Add(Int32.Parse(tokens[i]));
                    }
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
