using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;
using Tao.OpenGl;
using Tao.Platform.Windows;
using System.Drawing;

using Geometry;
using Component;

namespace SketchPlatform
{
    public class GLViewer : SimpleOpenGlControl
    {
        /******************** Initialization ********************/
        public GLViewer() 
        {
            this.InitializeComponent();
            this.InitializeContexts();
        }

        public void Init()
        {
            this.initializeVariables();
            // glsl shaders
            this.shader = new Shader(
                @"shaders\vertexshader.glsl",
                @"shaders\fragmentshader.glsl");
            this.shader.Link();

            this.LoadTextures();
        }

        private void InitializeComponent() 
        {
            this.SuspendLayout();

            this.Name = "GlViewer";

            this.ResumeLayout(false);
        }

        private void initializeVariables()
        {
            this.meshClasses = new List<MeshClass>();
            this.segmentClasses = new List<SegmentClass>();
            this.currModelTransformMatrix = Matrix4d.IdentityMatrix();
            this.arcBall = new ArcBall(this.Width, this.Height);
            this.initializeColors();
            this.camera = new Camera();

            axes = new Vector3d[18] { new Vector3d(-1.2, 0, 0), new Vector3d(1.2, 0, 0),
                                      new Vector3d(1, 0.2, 0), new Vector3d(1.2, 0, 0), 
                                      new Vector3d(1, -0.2, 0), new Vector3d(1.2, 0, 0), 
                                      new Vector3d(0, -1.2, 0), new Vector3d(0, 1.2, 0), 
                                      new Vector3d(-0.2, 1, 0), new Vector3d(0, 1.2, 0),
                                      new Vector3d(0.2, 1, 0), new Vector3d(0, 1.2, 0),
                                      new Vector3d(0, 0, -1.2), new Vector3d(0, 0, 1.2),
                                      new Vector3d(-0.2, 0, 1), new Vector3d(0, 0, 1.2),
                                      new Vector3d(0.2, 0, 1), new Vector3d(0, 0, 1.2)};
        }

        private void initializeColors()
        {
            ColorSet = new Color[20];

            ColorSet[0] = Color.Red;// Color.FromArgb(203, 213, 232);
            ColorSet[1] = Color.Green;// Color.FromArgb(252, 141, 98);
            ColorSet[2] = Color.Blue; // Color.FromArgb(102, 194, 165);
            ColorSet[3] = Color.Black;// Color.FromArgb(231, 138, 195);
            ColorSet[4] = Color.Cyan;// Color.FromArgb(166, 216, 84);
            ColorSet[5] = Color.Magenta;// Color.FromArgb(251, 180, 174);
            ColorSet[6] = Color.Yellow;// Color.FromArgb(204, 235, 197);
            ColorSet[7] = Color.FromArgb(222, 203, 228);
            ColorSet[8] = Color.FromArgb(31, 120, 180);
            ColorSet[9] = Color.FromArgb(251, 154, 153);
            ColorSet[10] = Color.FromArgb(227, 26, 28);
            ColorSet[11] = Color.FromArgb(255, 127, 0);
            ColorSet[12] = Color.FromArgb(51, 160, 44);
            ColorSet[13] = Color.FromArgb(202, 178, 214);
            ColorSet[14] = Color.FromArgb(141, 211, 199);
            ColorSet[15] = Color.FromArgb(255, 255, 179);
            ColorSet[16] = Color.FromArgb(251, 128, 114);
            ColorSet[17] = Color.FromArgb(179, 222, 105);
            ColorSet[18] = Color.FromArgb(188, 128, 189);
            ColorSet[19] = Color.FromArgb(217, 217, 217);

            ModelColor = ColorSet[0];
            GuideLineColor = Color.RoyalBlue;
        }
        
        //private void initializeCamera()
        //{
        //    // initialize vpCamera
        //    int w = this.Width, h = this.Height;
        //    this.vpCamera = new VPCamera();
        //    this.vpCamera.Init(
        //        new Vector2d(-200, h / 2 - 200),
        //        new Vector2d(w + 200, h / 2 - 200),
        //        w, h
        //    );
        //    int[] viewport = { 0, 0, w, h };

        //    this.vpCamera.CubeCalibrate(viewport, this.Height, out this.cameraBox);
        //}

        // modes
        public enum UIMode 
        {
            // !Do not change the order of the modes --- used in the current program to retrieve the index (Integer)
            Viewing, VertexSelection, EdgeSelection, FaceSelection, ComponentSelection, Guide, NONE
        }

        private bool drawVertex = false;
        private bool drawEdge = false;
        private bool drawFace = true;
        private bool isDrawAxes = false;
        private bool isDrawQuad = false;

        public bool showBoundingbox = false;
        public bool showMesh = false;
        public bool showSketchyEdges = true;
        public bool showGuideLines = false;
        public bool showOcclusion = false;
        public bool enableDepthTest = false;
        public bool showVanishingLines = true;

        private bool inGuideMode = false;
        Vector3d eye = new Vector3d(0, 0, 1);
        private float[] material = { 0.62f, 0.74f, 0.85f, 1.0f };
        private float[] ambient = { 0.2f, 0.2f, 0.2f, 1.0f };
        private float[] diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] specular = { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] position = { 1.0f, 1.0f, 1.0f, 0.0f };

        /******************** Variables ********************/
        private UIMode currUIMode = UIMode.Viewing;
        private Matrix4d currModelTransformMatrix = Matrix4d.IdentityMatrix();
        private Matrix4d modelTransformMatrix = Matrix4d.IdentityMatrix();
        private Matrix4d fixedModelView = Matrix4d.IdentityMatrix();
        private Matrix4d scaleMat = Matrix4d.IdentityMatrix();
        private Matrix4d transMat = Matrix4d.IdentityMatrix();
        private Matrix4d rotMat = Matrix4d.IdentityMatrix();
        private ArcBall arcBall = new ArcBall();
        private Vector2d mouseDownPos;
        private Vector2d prevMousePos;
        private Vector2d currMousePos;
        private bool isMouseDown = false;
        private List<MeshClass> meshClasses;
        private List<SegmentClass> segmentClasses;
        private MeshClass currMeshClass;
        private SegmentClass currSegmentClass;
        private Vector3d[] axes;
        private Quad2d highlightQuad;
        private int[] meshStats;
        private int[] segStats;
        private Box cameraBox;
        private Camera camera;
        private Shader shader;
        public static uint pencilTextureId, crayonTextureId, inkTextureId, waterColorTextureId, charcoalTextureId,
            brushTextureId;
        
        private bool drawShadedOrTexturedStroke = true;
        private StrokePoint[] vanishingPoints;

        public static Random rand = new Random();
        private bool readyForGuideArrow = false;
        private Vector3d objectCenter = new Vector3d();
        private int depthType = 1;
        private int vanishinglineDrawType = 0;

        //########## sequence vars ##########//
        private int sequenceIdx = -1;
        private int nSequence = 0;
        private int lineToDraw = -1;
        public int currBoxIdx = -1;
        public int nextBoxIdx = -1;

        //########## static vars ##########//
        public static Color[] ColorSet;
        static public Color ModelColor;
        public static Color GuideLineColor;

        /******************** Functions ********************/

        public UIMode CurrentUIMode
        {
            get
            {
                return this.currUIMode;
            }
            set
            {
                this.currUIMode = value;
            }
        }

        private void LoadTextures()					// load textures for canvas and brush
        {
            this.CreateTexture(@"data\pencil.png", out GLViewer.pencilTextureId);
            this.CreateTexture(@"data\crayon.png", out GLViewer.crayonTextureId);
            this.CreateTexture(@"data\ink.jpg", out GLViewer.inkTextureId);
            this.CreateTexture(@"data\watercolor.png", out GLViewer.waterColorTextureId);
            this.CreateTexture(@"data\charcoal.jpg", out GLViewer.charcoalTextureId);
            this.CreateGaussianTexture(32);
        }

        private void CreateTexture(string imagefile, out uint textureid)
        {
            Bitmap image = new Bitmap(imagefile);

            // to gl texture
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            //	image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            System.Drawing.Imaging.BitmapData bitmapdata = image.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Gl.glGenTextures(1, out textureid);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureid);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_R, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 4, image.Width, image.Height, 0, Gl.GL_BGRA,
                Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0);
        }

        public byte[] CreateGaussianTexture(int size)
        {
            int w = size * 2, size2 = size * size;
            Bitmap bitmap = new Bitmap(w, w);
            byte[] alphas = new byte[w * w * 4];
            for (int i = 0; i < w; ++i)
            {
                int dx = i - size;
                for (int j = 0; j < w; ++j)
                {
                    int J = j * w + i;

                    int dy = j - size;
                    double dist2 = (dx * dx + dy * dy);

                    byte alpha = 0;
                    if (dist2 <= size2)	// -- not necessary actually, similar effects
                    {
                        // set gaussian values for the alphas
                        // modify the denominator to get different over-paiting effects
                        double gau_val = Math.Exp(-dist2 / (2 * size2 / 2)) / Math.E / 2;
                        alpha = Math.Min((byte)255, (byte)((gau_val) * 255));
                        //	alpha = 100; // Math.Min((byte)255, (byte)((gau_val) * 255));
                    }

                    byte beta = (byte)(255 - alpha);
                    alphas[J * 4] = (byte)(beta);
                    alphas[J * 4 + 1] = (byte)(beta);
                    alphas[J * 4 + 2] = (byte)(beta);
                    alphas[J * 4 + 3] = (byte)(alpha);

                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(alpha, beta, beta, beta));
                }
            }
            bitmap.Save(@"data\output.png");

            // create gl texture
            uint[] txtid = new uint[1];
            // -- create texture --
            Gl.glGenTextures(1, txtid);				// Create The Texture
            GLViewer.brushTextureId = txtid[0];

            // to gl texture
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            //	image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            System.Drawing.Imaging.BitmapData bitmapdata = bitmap.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, txtid[0]);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_R, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 4, bitmap.Width, bitmap.Height, 0, Gl.GL_RGBA,
                Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

            return alphas;
        }

        public void loadMesh(string filename)
        {
            Mesh m = new Mesh(filename, true);
            MeshClass mc = new MeshClass(m);
            this.meshClasses.Add(mc);
            this.currMeshClass = mc;

            meshStats = new int[3];
            meshStats[0] = m.VertexCount;
            meshStats[1] = m.Edges.Length;
            meshStats[2] = m.FaceCount;
        }

        public void loadSegments(string segfolder)
        {
            this.segmentClasses = new List<SegmentClass>();
            int idx = segfolder.LastIndexOf('\\');
            string bbofolder = segfolder.Substring(0, idx + 1);
            bbofolder += "bounding_boxes\\";
            SegmentClass sc = new SegmentClass();
            sc.ReadSegments(segfolder, bbofolder);
            this.setRandomSegmentColor(sc);
            this.segmentClasses.Add(sc);
            this.currSegmentClass = sc;
            this.calculateSketchMesh();
            segStats = new int[2];
            segStats[0] = sc.segments.Count;
        }// loadSegments

        public void loadJSONFile(string jsonFile)
        {
            SegmentClass sc = new SegmentClass();
            //Matrix4d m = sc.DeserializeJSON(jsonFile, out this.objectCenter);
            Matrix4d m = sc.DeserializeJSON_new(jsonFile, out this.objectCenter);

            if (m != null)
            {
                this.fixedModelView = new Matrix4d(m);
                this.currModelTransformMatrix = m;
                this.eye = new Vector3d(0, 0, 1);
            }
            this.updateCamera();
            this.segmentClasses.Add(sc);
            this.setRandomSegmentColor(sc);
            this.currSegmentClass = sc;
            this.setGuideLineStyle((int)SegmentClass.GuideLineStyle);
            this.calculateSketchMesh();

            segStats = new int[2];
            segStats[0] = sc.segments.Count;
            this.setGuideLineColor(GLViewer.GuideLineColor);

            this.resetHighlightVars();

            // write sequence file
            this.foldername = jsonFile.Substring(0, jsonFile.LastIndexOf('\\'));
            string filename = this.foldername + "\\sequence.txt";
            StreamWriter sw = new StreamWriter(filename);
            foreach (string str in this.currSegmentClass.Sequence)
            {
                sw.WriteLine(str);
            }
            sw.Close();

            // parse sequence
            string seqfile = jsonFile.Substring(0, jsonFile.LastIndexOf('\\') + 1);
            seqfile += "sequence.txt";
            if (File.Exists(seqfile))
            {
                this.nSequence = this.currSegmentClass.LoadGuideSequence(seqfile);
            }

            this.Refresh();
            this.calculatePoint2DInfo();
            this.calculateVanishingPoints();
            this.calculateVanishingLine();

        }// loadJSONFile

        private void calculatePoint2DInfo()
        {
            this.updateCamera();
            //Vector3d z = new Vector3d(0, 0, 1);
            //Matrix4d mvp = this.camera.GetProjMat().Inverse();
            //mvp = this.modelTransformMatrix.Inverse() * mvp;
            //mvp = this.camera.GetProjMat().Transpose() * mvp;
            //this.eye = (mvp * new Vector4d(z, 1)).ToHomogeneousVector();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                Box box = seg.boundingbox;
                box.points2 = new Vector2d[box.points.Length];
                for (int i = 0; i < box.points.Length; ++i)
                {
                    box.points2[i] = this.camera.Project(box.points[i]).ToVector2d();
                }
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        foreach (StrokePoint sp in stroke.strokePoints)
                        {
                            Vector3d v3 = this.camera.Project(sp.pos3);
                            sp.pos2 = v3.ToVector2d();
                        }
                    }
                }
                foreach (Plane plane in seg.boundingbox.planes)
                {
                    plane.points2 = new Vector2d[plane.points.Length];
                    for (int i = 0; i < plane.points.Length; ++i)
                    {
                        Vector3d v3 = this.camera.Project(plane.points[i]);
                        plane.points2[i] = v3.ToVector2d();
                    }
                }
                if (box.vanLines != null)
                {
                    for (int i = 0; i < box.vanLines.Length; ++i)
                    {
                        foreach (Line3d line in box.vanLines[i])
                        {
                            line.u2 = this.camera.Project(line.u3).ToVector2d();
                            line.v2 = this.camera.Project(line.v3).ToVector2d();
                        }
                    }
                }
                foreach (GuideLine line in box.guideLines)
                {
                    if (line.vanLines != null)
                    {
                        for (int i = 0; i < line.vanLines.Length; ++i)
                        {
                            foreach (Line3d l in line.vanLines[i])
                            {
                                l.u2 = this.camera.Project(l.u3).ToVector2d();
                                l.v2 = this.camera.Project(l.v3).ToVector2d();
                            }
                        }
                    }
                }
            }
        }//calculatePoint2DInfo

        private void calculateVanishingPoints()
        {
            if (this.currSegmentClass == null) return;
            this.vanishingPoints = new StrokePoint[2];
            Box b = this.currSegmentClass.segments[0].boundingbox;
            Line2d l1 = new Line2d(b.points2[2], b.points2[1]);
            Line2d l2 = new Line2d(b.points2[6], b.points2[5]);
            Vector2d v = Polygon.LineIntersectionPoint2d(l1, l2);

            Vector3d v3 = this.camera.ProjectPointToPlane(v, new Vector3d(), new Vector3d(0, 1, 0));
            this.vanishingPoints[0] = new StrokePoint(v3);
            this.vanishingPoints[0].pos2 = v;

            l1 = new Line2d(b.points2[6], b.points2[2]);
            l2 = new Line2d(b.points2[5], b.points2[1]);
            v = Polygon.LineIntersectionPoint2d(l1, l2);
            v3 = this.camera.ProjectPointToPlane(v, new Vector3d(), new Vector3d(0, 1, 0));
            this.vanishingPoints[1] = new StrokePoint(v3);
            this.vanishingPoints[1].pos2 = v;

            this.vanishingPoints[0].pos3 = new Vector3d(0, 0, -2);
            this.vanishingPoints[1].pos3 = new Vector3d(-2, 0, 0);

            
        }

        private void calculateVanishingLine()
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                Box box = seg.boundingbox;
                box.buildVanishingLines(this.vanishingPoints[0].pos3, 0);
                box.buildVanishingLines(this.vanishingPoints[1].pos3, 1);
                for (int i = 0; i < box.vanLines.Length; ++i)
                {
                    foreach (Line3d line in box.vanLines[i])
                    {
                        line.u2 = this.camera.Project(line.u3).ToVector2d();
                        line.v2 = this.camera.Project(line.v3).ToVector2d();
                    }
                }
                foreach (GuideLine line in box.guideLines)
                {
                    line.buildVanishingLines(this.vanishingPoints[0].pos3, 0);
                    line.buildVanishingLines(this.vanishingPoints[1].pos3, 1);
                    for (int i = 0; i < line.vanLines.Length; ++i)
                    {
                        foreach (Line3d l in line.vanLines[i])
                        {
                            l.u2 = this.camera.Project(l.u3).ToVector2d();
                            l.v2 = this.camera.Project(l.v3).ToVector2d();
                        }
                    }
                }
            }
        }// calculateVanishingLine

        public int[] getMeshStatistics()
        {
            return this.meshStats;
        }

        public int[] getSegmentStatistics()
        {
            return this.segStats;
        }

        public void setRandomSegmentColor(SegmentClass sc)
        {
            foreach (Segment seg in sc.segments)
            {
                int idx = GLViewer.rand.Next(0, GLViewer.ColorSet.Length - 1);
                Color c = GLViewer.ColorSet[idx];
                seg.color = Color.FromArgb(80, c);
            }
        }

        public void setStrokeStylePerSeg(int size)
        {
            SegmentClass.StrokeSize = size;
            if (this.currSegmentClass == null) return;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                this.setStrokeStylePerSeg(size, seg, SegmentClass.StrokeColor, GuideLineColor);
            }
        }//setStrokeStylePerSeg

        private void setStrokeStylePerSeg(int size, Segment seg, Color c, Color gc)
        {
            foreach (GuideLine edge in seg.boundingbox.edges)
            {
                foreach (Stroke stroke in edge.strokes)
                {
                    stroke.strokeColor = c;
                    stroke.setStrokeSize(size);
                    stroke.changeStyle((int)SegmentClass.strokeStyle);
                }
            }
            foreach (GuideLine edge in seg.boundingbox.guideLines)
            {
                foreach (Stroke stroke in edge.strokes)
                {
                    stroke.strokeColor = gc;
                    stroke.setStrokeSize(size * 0.8);
                    stroke.changeStyle((int)SegmentClass.strokeStyle);
                }
            }
        }// setStrokeStylePerSeg - per seg

        public void setGuideLineColor(Color c)
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (GuideLine edge in seg.boundingbox.guideLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.strokeColor = c;
                    }
                }
            }
        }

        public void setSektchyEdgesColor(Color c)
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.strokeColor = c;
                    }
                }
            }
        }//setStrokeStylePerSeg

        public void setGuideLineStyle(int idx)
        {
            switch (idx)
            {
                case 1:
                    SegmentClass.GuideLineStyle = SegmentClass.GuideLineType.Random;
                    break;
                case 0:
                default:
                    SegmentClass.GuideLineStyle = SegmentClass.GuideLineType.SimpleCross;
                    break;
            }    
            if (this.currSegmentClass != null)
            {
                this.currSegmentClass.ChangeGuidelineStyle(idx);
                this.calculateSketchMesh();
                this.setGuideLineColor(GLViewer.GuideLineColor);
            }
        }//setGuideLineStyle

        public void setStrokeStyle(int idx)
        {
            if (this.currSegmentClass != null)
            {
                this.currSegmentClass.setStrokeStyle(idx);
                this.updateStrokeMesh();
                this.drawShadedOrTexturedStroke = this.currSegmentClass.shadedOrTexture();
            }
        }//setStrokeStyle

        public void setDepthType(int idx)
        {
            this.depthType = idx;
            this.updateDepthVal();
        }// setDepthType

        public void setVanishingLineDrawType(int idx)
        {
            this.vanishinglineDrawType = idx;
        }// setDepthType

        public void nextBox()
        {
            this.inGuideMode = true;
            this.currBoxIdx = (this.currBoxIdx + 1) % this.currSegmentClass.segments.Count;
            this.clearBoxes();
            this.activateBoxAndGuideLines();
        }

        public void prevBox()
        {
            this.inGuideMode = true;
            this.currBoxIdx = (this.currBoxIdx - 1 + this.currSegmentClass.segments.Count)
                % this.currSegmentClass.segments.Count;
            this.clearBoxes();
            this.activateBoxAndGuideLines();
        }

        private void activateBoxAndGuideLines()
        {
            Segment activeSeg = this.currSegmentClass.segments[this.currBoxIdx];
            activeSeg.active = true;
            List<GuideLine> currLines = activeSeg.boundingbox.getAllLines();
            foreach (GuideLine line in currLines)
            {
                line.active = true;
                foreach (Stroke stroke in line.strokes)
                {
                    stroke.strokeColor = SegmentClass.StrokeColor;
                }
            }
        }// activateBoxAndGuideLines

        public void AutoSequence()
        {
            for (int i = 0; i < this.nSequence; ++i)
            {
                this.nextSequence();
                this.Refresh();
                System.Threading.Thread.Sleep(100);
                this.captureScreen(i);
            }
        }
        public void nextSequence()
        {
            this.inGuideMode = true;
            if (this.nSequence <= 0)
            {
                return;
            }
            this.lineToDraw = -1;
            if (this.sequenceIdx == this.nSequence - 1)
            {
                MessageBox.Show("Finish!");
                return;
            }
            this.sequenceIdx++;
            if (this.sequenceIdx == 0)
            {
                this.clearBoxes();
            }
            this.activateGuideSequence();
        }

        public void prevSequence()
        {
            this.inGuideMode = true;
            if (this.nSequence <= 0)
            {
                return;
            }
            this.lineToDraw = -1;
            if (this.sequenceIdx <= 0)
            {
                MessageBox.Show("This is the first box!");
                return;
            }
            
            Segment activeSeg = this.currSegmentClass.segments[this.currBoxIdx];
            this.deActivateBoxAndGuideLines(activeSeg);
            this.sequenceIdx--;
            this.activateGuideSequence();
        }

        private void clearBoxes()
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                seg.active = false;
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    line.active = false;
                }
            }
        }//clearBoxes

        private void resumeBoxes()
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                seg.active = true;
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    line.active = true;
                }
                this.setStrokeStylePerSeg(SegmentClass.StrokeSize, seg, SegmentClass.StrokeColor, GuideLineColor);
            }
            this.currBoxIdx = -1;
            this.sequenceIdx = -1;
        }

        private void resetHighlightVars()
        {
            this.sequenceIdx = -1;
            this.currBoxIdx = -1;
            this.lineToDraw = -1;
            this.nSequence = 0;
        }
        
        private void activateGuideSequence()
        {
            this.activateDrawnBoxes();

            // parse sequence
            List<int> guideLinesIndex;
            this.currSegmentClass.parseASequence(this.sequenceIdx, out this.currBoxIdx, out guideLinesIndex, out this.nextBoxIdx, out this.lineToDraw);
            this.readyForGuideArrow = false;
            Segment activeSeg = this.currSegmentClass.segments[this.currBoxIdx];
            // active box
            this.activateBox(activeSeg);
            // active guide lines
            this.showAnimatedGuideLines(activeSeg, guideLinesIndex);
            this.readyForGuideArrow = true;
            // actuve next box (guided)
            if (this.nextBoxIdx != -1 && this.lineToDraw != -1)
            {
                GuideLine edge = this.currSegmentClass.segments[this.nextBoxIdx].boundingbox.edges[this.lineToDraw];
                foreach (Stroke s in edge.strokes)
                {
                    s.setStrokeSize(2);
                    s.changeStyle((int)SegmentClass.strokeStyle);
                }
            }

        }// activateGuideSequence

        private void activateDrawnBoxes()
        {
            // fade out the boxes that have already been drawn
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                this.setStrokeStylePerSeg(SegmentClass.StrokeSize, seg, SegmentClass.HiddenColor, GuideLineColor);
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    line.active = true;
                }
                foreach (GuideLine line in seg.boundingbox.guideLines)
                {
                    line.active = false;
                }
            }
        }// activateDrawnBox

        private void activateBox(Segment activeSeg)
        {
            activeSeg.active = true;
            this.setStrokeStylePerSeg(4, activeSeg, SegmentClass.StrokeColor, GuideLineColor);
        }// activateBox


        private void deActivateBoxAndGuideLines(Segment activeSeg)
        {
            activeSeg.active = false;
            List<GuideLine> currLines = activeSeg.boundingbox.getAllLines();
            foreach (GuideLine line in currLines)
            {
                line.active = false;
                foreach (Stroke stroke in line.strokes)
                {
                    stroke.strokeColor = SegmentClass.StrokeColor;
                    stroke.setStrokeSize(SegmentClass.StrokeSize);
                    stroke.changeStyle((int)SegmentClass.strokeStyle);
                }
            }
        }// deActivateBoxAndGuideLines


        private void showAnimatedGuideLines(Segment seg, List<int> guideLinesIndex)
        {
            // draw arrows
            if (this.enableDepthTest)
                Gl.glDisable(Gl.GL_DEPTH_TEST);
            for (int i = 0; i < guideLinesIndex.Count; ++i)
            {
                GuideLine line = seg.boundingbox.guideLines[guideLinesIndex[i]];
                line.active = true;
                this.Refresh();
                System.Threading.Thread.Sleep(100);
            }
            if (guideLinesIndex.Count > 0)
            {
                GuideLine last = seg.boundingbox.guideLines[guideLinesIndex[guideLinesIndex.Count - 1]];
                last.isGuide = true;
                foreach (Stroke stroke in last.strokes)
                {
                    stroke.strokeColor = Color.Red;
                    stroke.setStrokeSize(4);
                    stroke.changeStyle((int)SegmentClass.strokeStyle);
                }
            }
            if (this.enableDepthTest)
                Gl.glEnable(Gl.GL_DEPTH_TEST);
        }// showAnimatedGuideLines

        private void projectStrokePointsTo2d(Stroke stroke)
        {
            if (stroke.strokePoints == null || stroke.strokePoints.Count == 0)
                return;
            foreach(StrokePoint p in stroke.strokePoints)
            {
                Vector3d p3 = p.pos3;
                p.pos2 = this.camera.Project(p3.x, p3.y, p3.z).ToVector2d();
            }
        }

        public void calculateSketchMesh()
        {
            this.updateCamera();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine edge in allLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        Vector3d u3 = stroke.u3;
                        Vector3d v3 = stroke.v3;
                        stroke.u2 = this.camera.Project(u3.x, u3.y, u3.z).ToVector2d();
                        stroke.v2 = this.camera.Project(v3.x, v3.y, v3.z).ToVector2d();
                        Vector2d dir = (stroke.v2 - stroke.u2).normalize();
                        Vector2d normal = new Vector2d(-dir.y, dir.x);
                        normal.normalize();
                        this.projectStrokePointsTo2d(stroke);
                        stroke.setStrokeMeshPoints(normal);
                        stroke.hostPlane = edge.hostPlane;                        
                    }
                }
            }

            Matrix4d m = this.camera.GetProjMat() * this.camera.GetModelviewMat() * this.camera.GetBallMat();
            m = m.Transpose();
            m = this.modelTransformMatrix;
            this.currSegmentClass.ChangeStrokeStyle(m, camera);

        }// calculateSketchMesh

        private void updateStrokeMesh()
        {
            foreach (Segment seg in this.currSegmentClass.segments)
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
        }// updateStrokeMesh

        public void createEllipse(Segment seg)
        {
            if (seg == null) return;
            Box box = seg.boundingbox;
            int[] ids = { 1, 2, 6, 5 };
            Vector3d u = (box.points[ids[2]] + box.points[ids[3]]) / 2 - (box.points[ids[0]] + box.points[ids[1]]) / 2;
            Vector3d v = (box.points[ids[0]] + box.points[ids[3]]) / 2 - (box.points[ids[1]] + box.points[ids[2]]) / 2;
            double a = (box.points[ids[0]] - box.points[ids[3]]).Length();
            double b = (box.points[ids[0]] - box.points[ids[1]]).Length();
            Vector3d c = new Vector3d();
            for (int i = 0; i < ids.Length; ++i)
            {
                c += box.points[ids[i]];
            }
            c /= ids.Length;
            Ellipse3D e = new Ellipse3D(c, u, v, a, b);
            box.ellipses.Add(e);
        }

        public string foldername;
        public void outputBoxSequence()
        {
            if (this.currSegmentClass == null) return;
            string filename = foldername + "\\sequence.txt";
            StreamWriter sw = new StreamWriter(filename);
            for (int i = 0; i < this.currSegmentClass.segments.Count; ++i)
            {
                sw.WriteLine("box:" + i.ToString());
            }
            sw.Close();
        }

        private void captureScreen(int idx)
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var gfx = Graphics.FromImage(bmp);
            Size newSize = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Size.Width, Screen.PrimaryScreen.Bounds.Size.Height - 40);
            gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, newSize, CopyPixelOperation.SourceCopy);
            string imageFolder = foldername + "\\screenCapture";
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            string name = imageFolder + "\\seq_" + idx.ToString() + ".png";
            bmp.Save(name, System.Drawing.Imaging.ImageFormat.Png);
        }

        //########## depth ##########//
        private List<Plane> allBoxPlanes;
        /*****TEST*****/
        private Line3d testLine;
        private StrokePoint testPoint;
        private Vector2d testPos2;
        private Plane testPlane;
        private bool getPoint = false;
        /*****TEST*****/

        private void getAllHostPlanes()
        {
            this.allBoxPlanes = new List<Plane>();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                Box box = seg.boundingbox;
                foreach (Plane p in box.planes)
                {
                    this.allBoxPlanes.Add(p);
                }
            }
        }

        private Vector3d getCurPos(Vector3d v)
        {
            return (this.modelTransformMatrix * new Vector4d(v, 1)).ToVector3D();
        }

        private Vector3d getOriPos(Vector3d v)
        {
            return (this.modelTransformMatrix.Inverse() * new Vector4d(v, 1)).ToVector3D();
        }

        private void calculateStrokePointDepthByCameraPos()
        {
            this.updateCamera();
            if (this.currSegmentClass == null)
            {
                return;
            }

            double minD = double.MaxValue, maxD = double.MinValue;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        foreach (StrokePoint sp in stroke.strokePoints)
                        {
                            Vector3d pos = this.getCurPos(sp.pos3);
                            sp.depth = (pos - eye).Length();
                            maxD = maxD > sp.depth ? maxD : sp.depth;
                            minD = minD < sp.depth ? minD : sp.depth;
                        }
                    }
                }
            }
            double scale = maxD - minD;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        foreach (StrokePoint sp in stroke.strokePoints)
                        {
                            sp.depth = 1 - (sp.depth - minD) / scale;
                        }
                    }
                }
            }
        }// calculateStrokePointDepthByCameraPos

        private void goThroughDepthTest()
        {
            // 
            Gl.glBeginQuery(Gl.GL_SAMPLES_PASSED, 1);
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            if (this.showSketchyEdges)
            {
                this.drawSketchyEdges3D();
            }
            if (this.showGuideLines)
            {
                this.drawGuideLines3D();
            }
            this.DrawHighlight3D();

            Gl.glEndQuery(Gl.GL_SAMPLES_PASSED);

            int queryReady = Gl.GL_FALSE;
            int count = 1000;
            while (queryReady != Gl.GL_TRUE && count-- > 0)
            {
                Gl.glGetQueryObjectiv(1, Gl.GL_QUERY_RESULT_AVAILABLE,  out queryReady);
            }
            int samples;
            Gl.glGetQueryObjectiv(1, Gl.GL_QUERY_RESULT, out samples);
        }

        private void calculateStrokePointDepth()
        {
            this.updateCamera();
            if (this.currSegmentClass == null)
            {
                return;
            }
            this.goThroughDepthTest();
            this.getPoint = false;
            double minD = double.MaxValue, maxD = double.MinValue;

            //this.eye = (this.currModelTransformMatrix.Inverse() * new Vector4d(0, 0, 1, 1)).ToHomogeneousVector();
            this.calculatePoint2DInfo();
            List<double> depths = new List<double>();

            //Gl.glReadBuffer(Gl.GL_FRONT);
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        foreach (StrokePoint sp in stroke.strokePoints)
                        {
                            float[] depth = new float[1];
                            Gl.glReadPixels((int)sp.pos2.x, (int)sp.pos2.y, 1, 1, Gl.GL_DEPTH_COMPONENT, Gl.GL_FLOAT, depth);

                            sp.depth = depth[0];
                            depths.Add(sp.depth);
                            maxD = maxD > sp.depth ? maxD : sp.depth;
                            minD = minD < sp.depth ? minD : sp.depth;
                        }
                    }
                }
            }

            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        foreach (StrokePoint sp in stroke.strokePoints)
                        {
                            if (this.isIntersectAPlaneByDepth(sp, stroke.hostPlane))
                            {
                                sp.depth = 0;
                            }
                            else
                            {
                                sp.depth = 1.0;
                            }
                        }
                    }
                }
            }
            //Gl.glDisable(Gl.GL_DEPTH_TEST);
        }// calculateStrokePointDepth

        private void calculateHiddenStrokePointDepthByLinePlaneIntersection()
        {
            this.updateCamera();
            if (this.currSegmentClass == null)
            {
                return;
            }
            this.getPoint = false;
            this.allBoxPlanes = new List<Plane>();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (Plane p in seg.boundingbox.planes)
                {
                    this.allBoxPlanes.Add(p);
                }
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        foreach (StrokePoint sp in stroke.strokePoints)
                        {
                            if (this.isIntersectAPlaneByDistance(sp, stroke.hostPlane))
                            {
                                sp.depth = 0;
                            }
                            else
                            {
                                sp.depth = 1.0;
                            }
                        }
                    }
                }
            }
        }// calculateHiddenStrokePointDepthByLinePlaneIntersection

        private bool isIntersectAPlaneByDistance(StrokePoint sp, Plane p)
        {
            if (this.allBoxPlanes == null || this.allBoxPlanes.Count == 0)
            {
                return false;
            }
            Vector3d sp_pos = this.getCurPos(sp.pos3);
            Line3d line = new Line3d(sp_pos, this.eye);    
            
            foreach (Plane plane in this.allBoxPlanes)
            {
                double dis = Polygon.PointDistToPlane(sp.pos3, plane.center, plane.normal);
                if (dis < Polygon.thresh) continue;
                Vector3d point;
                Vector3d center = this.getCurPos(plane.center);
                Vector3d normal = this.getCurPos(plane.normal);
                if (!Polygon.LinePlaneIntersection(line, center, normal, out point))
                {
                    continue;
                }
                double sp_dis = (sp_pos - this.eye).Length();
                double p_dis = (point - this.eye).Length();
                if (sp_dis < p_dis) continue;
                Vector3d point_o = this.getOriPos(point);
                Vector2d p2 = this.camera.Project(point_o).ToVector2d();
                if (Polygon.PointInPoly(p2, plane.points2))
                {
                    //if (!this.getPoint)
                    //{
                    //    this.getPoint = true;
                    //    this.testPoint = new StrokePoint(point_o);
                    //    this.testLine = new Line3d(this.testPoint.pos3, this.eye);
                    //    this.testPos2 = p2;
                    //    this.testPlane = plane;
                    //}
                    return true;
                }
            }
            if (!this.getPoint)
            {
                this.getPoint = true;
                this.testPoint = sp;
                this.testLine = new Line3d(this.testPoint.pos3, this.eye);
                this.testPos2 = sp.pos2;
                this.testPlane = p;
            }
            return false;
        }// isIntersectAPlaneByDistance

        private bool isIntersectAPlaneByDepth(StrokePoint sp, Plane p)
        {
            if (this.allBoxPlanes == null || this.allBoxPlanes.Count == 0)
            {
                return false;
            }
            Vector3d sp_pos = this.getCurPos(sp.pos3);
            Line3d line = new Line3d(sp_pos, this.eye);
            foreach (Plane plane in this.allBoxPlanes)
            {
                if (plane == p) continue;
                Vector3d point;
                Vector3d center = this.getCurPos(plane.center);
                Vector3d normal = this.getCurPos(plane.normal);
                if (!Polygon.LinePlaneIntersection(line, center, normal, out point))
                {
                    continue;
                }
                double sp_dis = (sp_pos - this.eye).Length();
                double p_dis = (point - this.eye).Length();
                if (sp_dis < p_dis) continue;
                Vector3d point_o = this.getOriPos(point);
                Vector2d p2 = this.camera.Project(point_o).ToVector2d();
                if (Polygon.PointInPoly(p2, plane.points2))
                {
                    if (!this.getPoint)
                    {
                        this.getPoint = true;
                        this.testPoint = sp;
                        this.testPoint.pos2 = this.camera.Project(this.testPoint.pos3).ToVector2d();
                        this.testLine = line;// new Line3d(this.testPoint.pos3, this.eye);
                        this.testPos2 = p2;
                        this.testPlane = plane;
                    }
                    return true;
                }
            }
            return false;
        }// isIntersectAPlaneByDistance

        //########## set modes ##########//
        public void setTabIndex(int i)
        {
            this.currMeshClass.tabIndex = i;
        }

        public void setUIMode(int i)
        {
            switch (i)
            {
                case 2:
                    this.currUIMode = UIMode.VertexSelection;
                    break;
                case 3:
                    this.currUIMode = UIMode.EdgeSelection;
                    break;
                case 4:
                    this.currUIMode = UIMode.FaceSelection;
                    break;
                case 1:
                default:
                    this.currUIMode = UIMode.Viewing;
                    break;
            }
        }

        public void setRenderOption(int i)
        {
            switch (i)
            {
                case 1:
                    this.drawVertex = !this.drawVertex;
                    break;
                case 2:
                    this.drawEdge = !this.drawEdge;
                    break;
                case 3:
                default:
                    this.drawFace = !this.drawFace;
                    break;
            }
            this.Refresh();
        }//setRenderOption


        public void displayAxes()
        {
            this.isDrawAxes = !this.isDrawAxes;
        }

        public void resetView()
        {
            this.arcBall.reset();
            this.currModelTransformMatrix = Matrix4d.IdentityMatrix();
            this.modelTransformMatrix = Matrix4d.IdentityMatrix();
            this.rotMat = Matrix4d.IdentityMatrix();
            this.scaleMat = Matrix4d.IdentityMatrix();
            this.transMat = Matrix4d.IdentityMatrix();
            this.updateDepthVal();
            this.calculatePoint2DInfo();
            this.Refresh();
        }
        
        public void reloadView()
        {
            this.arcBall.reset();
            this.currModelTransformMatrix = new Matrix4d(this.fixedModelView);
            this.modelTransformMatrix = Matrix4d.IdentityMatrix();
            this.updateDepthVal();
            this.calculatePoint2DInfo();
            this.Refresh();
        }
        private void updateCamera()
        {
            Matrix4d m = this.currModelTransformMatrix;
            double[] ballmat =  m.Transpose().ToArray();	// matrix applied with arcball
            this.camera.SetBallMatrix(ballmat);
            this.camera.Update();
        }

        private void updateDepthVal()
        {
            // tag == 1: tune opacity by the distance to the eye postion
            // tag == 2:
            if (depthType == 1)
            {
                this.calculateStrokePointDepthByCameraPos();
            }
            else if (depthType == 2)
            {
                this.calculateHiddenStrokePointDepthByLinePlaneIntersection();
            } else if (depthType == 3)
            {
                this.calculateStrokePointDepth();
            }

        }
        //########## Mouse ##########//

        private void viewMouseDown(MouseEventArgs e)
        {
            //if (this.currMeshClass == null) return;
            this.arcBall = new ArcBall(this.Width, this.Height);
            //this.currModelTransformMatrix = Matrix4d.IdentityMatrix();
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Middle:
                    {
                        this.arcBall.mouseDown(e.X, e.Y, ArcBall.MotionType.Pan);
                        break;
                    }
                case System.Windows.Forms.MouseButtons.Right:
                    {
                        this.arcBall.mouseDown(e.X, e.Y, ArcBall.MotionType.Scale);
                        break;
                    }
                case System.Windows.Forms.MouseButtons.Left:
                default:
                    {
                        this.arcBall.mouseDown(e.X, e.Y, ArcBall.MotionType.Rotate);
                        break;
                    }
            }
        }// viewMouseDown

        private void viewMouseMove(int x, int y)
        {
            if (!this.isMouseDown) return;
            this.arcBall.mouseMove(x, y);
        }// viewMouseMove
        
        private void viewMouseUp()
        {
            this.currModelTransformMatrix = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
            if (this.arcBall.motion == ArcBall.MotionType.Pan)
            {
                this.transMat = this.arcBall.getTransformMatrix() * this.transMat;
            }else if  (this.arcBall.motion == ArcBall.MotionType.Rotate)
            {
                this.rotMat = this.arcBall.getTransformMatrix() * this.rotMat;
            }
            else
            {
                this.scaleMat = this.arcBall.getTransformMatrix() * this.scaleMat;
            }
            this.arcBall.mouseUp();
            //this.modelTransformMatrix = this.transMat * this.rotMat * this.scaleMat;

            this.modelTransformMatrix = this.currModelTransformMatrix.Transpose();

            this.calculatePoint2DInfo();
            this.updateDepthVal();
        }// viewMouseUp

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.mouseDownPos = new Vector2d(e.X, e.Y);
            this.currMousePos = new Vector2d(e.X, e.Y);
            this.isMouseDown = true;

            switch (this.currUIMode)
            {
                case UIMode.VertexSelection:
                case UIMode.EdgeSelection:
                case UIMode.FaceSelection:
                    {
                        if (this.currMeshClass != null)
                        {
                            Matrix4d m = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
                            Gl.glMatrixMode(Gl.GL_MODELVIEW);
                            Gl.glPushMatrix();
                            Gl.glMultMatrixd(m.Transpose().ToArray());

                            this.currMeshClass.selectMouseDown((int)this.currUIMode, 
                                Control.ModifierKeys == Keys.Shift,
                                Control.ModifierKeys == Keys.Control);

                            Gl.glMatrixMode(Gl.GL_MODELVIEW);
                            Gl.glPopMatrix();

                            this.isDrawQuad = true;
                        }
                        break;
                    }
                case UIMode.ComponentSelection:
                    {
                        break;
                    }
                case UIMode.Viewing:
                default:
                    {
                        this.viewMouseDown(e);
                        this.Refresh();
                        break;
                    }
            }            
        }// OnMouseDown

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.prevMousePos = this.currMousePos;
            this.currMousePos = new Vector2d(e.X, e.Y);

            switch (this.currUIMode)
            {
                case UIMode.VertexSelection:
                case UIMode.EdgeSelection:
                case UIMode.FaceSelection:
                    {
                        if (this.currMeshClass != null && this.isMouseDown)
                        {
                            this.highlightQuad = new Quad2d(this.mouseDownPos, this.currMousePos);
                            this.currMeshClass.selectMouseMove((int)this.currUIMode, this.highlightQuad);
                            this.Refresh();
                        }
                        break;
                    }
                case UIMode.ComponentSelection:
                    {
                        break;
                    }
                case UIMode.Viewing:
                //default:
                    {
                        this.viewMouseMove(e.X, e.Y);
                        this.Refresh();
                        break;
                    }
            }            
        }// OnMouseMove

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.prevMousePos = this.currMousePos;
            this.currMousePos = new Vector2d(e.X, e.Y);
            this.isMouseDown = false;

            switch (this.currUIMode)
            {
                case UIMode.VertexSelection:
                case UIMode.EdgeSelection:
                case UIMode.FaceSelection:
                    {
                        this.isDrawQuad = false;
                        if (this.currMeshClass != null)
                        {
                            this.currMeshClass.selectMouseUp();
                        }
                        this.Refresh();
                        break;
                    }
                case UIMode.ComponentSelection:
                    {
                        break;
                    }
                case UIMode.Viewing:
                default:
                    {
                        this.viewMouseUp();
                        break;
                    }
            }
        }// OnMouseUp

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyData)
            {
                case System.Windows.Forms.Keys.V:
                    {
                        this.currUIMode = UIMode.Viewing;
                        break;
                    }
                case Keys.R:
                    {
                        this.reloadView();
                        break;
                    }
                case Keys.I:
                    {
                        this.resetView(); // Identity
                        break;
                    }
                case Keys.D:
                    {
                        this.inGuideMode = !this.inGuideMode;
                        if (this.inGuideMode)
                        {
                            this.showBoundingbox = false;
                            this.showMesh = false;
                            this.nextBox();
                        }
                        else
                        {
                            this.resumeBoxes();
                        }
                        this.Refresh();
                        break;
                    }
                case Keys.PageDown:
                    {
                        this.nextSequence();
                        break;
                    }
                case Keys.PageUp:
                    {
                        this.prevSequence();
                        break;
                    }
                default: 
                    break;
            }
            this.Refresh();
        }// OnKeyDown        

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaint(e);
            this.MakeCurrent();
            this.clearScene();
            this.Draw2D();
            this.Draw3D();
        }// onPaint

        private void setViewMatrix()
        {
            int w = this.Width;
            int h = this.Height;
            if (h == 0)
            {
                h = 1;
            }
            //this.MakeCurrent();

            Gl.glViewport(0, 0, w, h);

            double aspect = (double)w / h;

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            //if (w >= h)
            //{
            //    Glu.gluOrtho2D(-1.0 * aspect, 1.0 * aspect, -1.0, 1.0);
            //}
            //else
            //{
            //    Glu.gluOrtho2D(-1.0, 1.0, -1.0 * aspect, 1.0 * aspect);
            //}
            Glu.gluPerspective(90, aspect, 0.1, 1000);
            
            //Gl.glLoadMatrixd(this.camera.GetProjMat().ToArray());
            Glu.gluLookAt(0, 0, 1, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            //Gl.glLoadMatrixd(this.camera.GetModelviewMat().ToArray());

            this.drawPoints(new Vector3d[1] { this.eye }, Color.DarkOrange, 10.0f);
            Matrix4d m = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
            //m = Matrix4d.TranslationMatrix(this.objectCenter) * m * Matrix4d.TranslationMatrix(
            //    new Vector3d() - this.objectCenter);


            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glMultMatrixd(m.Transpose().ToArray());

            //Glu.gluLookAt(0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
        }

        private void Draw2D()
        {
            int w = this.Width, h = this.Height;

            Gl.glViewport(0, 0, w, h);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(0, w, 0, h);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glPushMatrix();

            //this.drawSketchyLines2D();   
         
            /*****TEST*****/
            //this.drawTest2D();

            Gl.glPopMatrix();
            Gl.glPopMatrix();
        }

        private void drawTest3D()
        {
            if (this.testLine != null)
            {
                this.drawLines3D(this.testLine.u3, this.testLine.v3, Color.Magenta, 4.0f);
            }
            if (this.testPlane != null)
            {
                this.drawQuadEdge3d(this.testPlane, Color.Red);
            }
            if (this.testPoint != null)
            {
                this.drawPoints(new Vector3d[1] { this.testPoint.pos3 }, Color.DarkSalmon, 10.0f);
            }
        }

        private void drawTest2D()
        {
            if (this.testPoint != null)
            {
                Gl.glColor3ub(0, 0, 255);
                Gl.glPointSize(10.0f);
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glVertex2dv(this.testPos2.ToArray());
                Gl.glEnd();
            }
        }

        //private float[] vertices;
        //private byte[] indices;
        //private float[] texture;

        //private int[] mVertexBuffer;
        //private int[] mIndexBuffer;
        //private int[] mTextureBuffer;

        //private void initBuffer()
        //{
        //    //uint quad_array;
        //    //Gl.glGenVertexArraysAPPLE(1, out quad_array);
        //    //Gl.glBindVertexArrayAPPLE(quad_array);

        //    //float[] verticesArray = {
        //    //    -1.0f, -1.0f, 0.0f,
        //    //    1.0f, -1.0f, 0.0f,
        //    //    -1.0f,  1.0f, 0.0f,
        //    //    -1.0f,  1.0f, 0.0f,
        //    //    1.0f, -1.0f, 0.0f,
        //    //    1.0f,  1.0f, 0.0f
        //    //};

        //    //uint quad_vertexbuffer;
        //    //Gl.glGenBuffers(1, out quad_vertexbuffer);

        //    //Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, quad_vertexbuffer);
        //    //Gl.glBufferData(Gl.GL_ARRAY_BUFFER, (IntPtr)(g_quad_vertex_buffer_data.Length * sizeof(float)), g_quad_vertex_buffer_data, Gl.GL_STATIC_DRAW);

        //    vertices = new float[] {
        //                                    -1.0f, -1.0f, 1.0f,
        //                                    1.0f, -1.0f, 1.0f,
        //                                    -1.0f, 1.0f, 1.0f,
        //                                    1.0f, 1.0f, 1.0f,

        //                                    1.0f, -1.0f, 1.0f,
        //                                    1.0f, -1.0f, -1.0f, 
        //                                    1.0f, 1.0f, 1.0f, 
        //                                    1.0f, 1.0f, -1.0f,

        //                                    1.0f, -1.0f, -1.0f, 
        //                                    -1.0f, -1.0f, -1.0f, 
        //                                    1.0f, 1.0f, -1.0f, 
        //                                    -1.0f, 1.0f, -1.0f,

        //                                    -1.0f, -1.0f, -1.0f, 
        //                                    -1.0f, -1.0f, 1.0f, 
        //                                    -1.0f, 1.0f, -1.0f, 
        //                                    -1.0f, 1.0f, 1.0f,

        //                                    -1.0f, -1.0f, -1.0f, 
        //                                    1.0f, -1.0f, -1.0f, 
        //                                    -1.0f, -1.0f, 1.0f, 
        //                                    1.0f, -1.0f, 1.0f,

        //                                    -1.0f, 1.0f, 1.0f, 
        //                                    1.0f, 1.0f, 1.0f, 
        //                                    -1.0f, 1.0f, -1.0f, 
        //                                    1.0f, 1.0f, -1.0f, 
        //                                    };

        //    texture = new float[] {
        //                                                                                   0.0f, 1.0f,                                                1.0f, 1.0f,                                                0.0f, 0.0f,                                                 1.0f, 0.0f,                                                 
        //                                    0.0f, 1.0f, 
        //                                    1.0f, 1.0f, 
        //                                    0.0f, 0.0f, 
        //                                    1.0f, 0.0f, 

        //                                    0.0f, 1.0f, 
        //                                    1.0f, 1.0f, 
        //                                    0.0f, 0.0f, 
        //                                    1.0f, 0.0f, 

        //                                    0.0f, 1.0f, 
        //                                    1.0f, 1.0f, 
        //                                    0.0f, 0.0f, 
        //                                    1.0f, 0.0f, 

        //                                    0.0f, 1.0f, 
        //                                    1.0f, 1.0f, 
        //                                    0.0f, 0.0f, 
        //                                    1.0f, 0.0f, 

        //                                    0.0f, 1.0f, 
        //                                    1.0f, 1.0f, 
        //                                    0.0f, 0.0f, 
        //                                    1.0f, 0.0f,  
        //                                    };

        //    indices = new byte[] {
        //                                    0, 1, 3, 0, 3, 2,                                                4, 5, 7, 4, 7, 6,
        //                                    8, 9, 11, 8, 11, 10,
        //                                    12, 13, 15, 12, 15, 14, 
        //                                    16, 17, 19, 16, 19, 18, 
        //                                     20, 21, 23, 20, 23, 22, 
        //                                    };

        //    mVertexBuffer = new int[1];
        //    mTextureBuffer = new int[1];
        //    mIndexBuffer = new int[1];

        //    Gl.glGenBuffers(1, mVertexBuffer);
        //    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mVertexBuffer[0]);
        //    Gl.glBufferData(Gl.GL_ARRAY_BUFFER,
        //         (IntPtr)(verticesArray.Length * sizeof(float)),
        //                      verticesArray, Gl.GL_STATIC_DRAW);

        //    Gl.glGenBuffers(1, mTextureBuffer);
        //    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mTextureBuffer[0]);
        //    Gl.glBufferData(Gl.GL_ARRAY_BUFFER,
        //         (IntPtr)(textureArray.Length * sizeof(float)),
        //                      textureArray, Gl.GL_STATIC_DRAW);

        //    Gl.glGenBuffers(1, mIndexBuffer);
        //    Gl.glBindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, mIndexBuffer[0]);
        //    Gl.glBufferData(Gl.GL_ELEMENT_ARRAY_BUFFER,
        //         (IntPtr)(indicesArray.Length * sizeof(short)),
        //                      indicesArray, Gl.GL_STATIC_DRAW);

        //}

        //private void drawTexture()
        //{

        //    Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
        //    Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

        //    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mVertexBuffer[0]);

        //    //Notice: IntPtr.Zero is used...
        //    Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, IntPtr.Zero);

        //    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mTextureBuffer[0]);

        //    //Notice: IntPtr.Zero is used...
        //    Gl.glTexCoordPointer(2, Gl.GL_FLOAT, 0, IntPtr.Zero);
        //    Gl.glBindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, mIndexBuffer[0]);

        //    //Notice: IntPtr.Zero is used. Last parameter is
        //    //an offset using Vertex Buffer Objects and in Vertex Arrays
        //    //it is a pointer
        //    Gl.glDrawElements(Gl.GL_TRIANGLES, indices.Length,
        //             Gl.GL_UNSIGNED_SHORT, IntPtr.Zero);

        //    //Remember to unbind your buffer to prevent it to destroy
        //    //other draw calls or objects
        //    Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
        //    Gl.glBindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, 0);

        //    Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
        //    Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);  

        //}

        
        private void Draw3D()
        {
            this.setViewMatrix();

            /***** Draw *****/
            //clearScene();

            if (this.isDrawAxes)
            {
                this.drawAxes();
            }

            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL);

            if (this.enableDepthTest)
            {
                Gl.glEnable(Gl.GL_DEPTH_TEST);
            }
            if (this.currMeshClass != null)
            {
                if (this.drawFace)
                {
                    //this.currMeshClass.renderShaded();
                    this.drawMesh(this.currMeshClass.Mesh, Color.Blue, true);
                }
                if (this.drawEdge)
                {
                    this.currMeshClass.renderWireFrame();
                }
                if (this.drawVertex)
                {
                    this.currMeshClass.renderVertices();
                }
                this.currMeshClass.drawSelectedVertex();
                this.currMeshClass.drawSelectedEdges();
                this.currMeshClass.drawSelectedFaces();
            }
            
            this.drawSegments();
            if (this.showSketchyEdges)
            {
                this.drawSketchyEdges3D();
            }
            if (this.showGuideLines)
            {
                this.drawGuideLines3D();
            }
            this.DrawHighlight3D();

            if (this.vanishingPoints != null)
            {
                this.drawSphere();
                //this.drawPoints(this.vanishingPoints, Color.Red, 4.0f);
            }

            //this.drawTest3D();

            if (this.enableDepthTest)
            {
                Gl.glDisable(Gl.GL_DEPTH_TEST);
            }


            Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();

            if (this.isDrawQuad)
            {
                this.drawQuad2d(this.highlightQuad, ColorSet[3]);
            }

            this.SwapBuffers();
        }// Draw3D        

        private void drawSegments()
        {
            if (this.currSegmentClass == null)
            {
                return;
            }
            
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (this.showMesh)
                {
                    this.drawMesh(seg.mesh, seg.color, false);
                }
                if (this.showBoundingbox)
                {
                    this.drawBoundingboxWithEdges(seg.boundingbox, seg.color);
                }
            }
        }//drawSegments

        private void drawSketchyLines2D()
        {
            if (this.currSegmentClass == null)
            {
                return;
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (Plane plane in seg.boundingbox.planes)
                {
                    this.drawPlane2D(plane);
                }
                foreach(GuideLine edge in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        //this.drawStrokeMesh2d(stroke);
                    }
                }
            }
        }// drawSketchyLines2D

        private void drawSketchyEdges3D()
        {
            if (this.currSegmentClass == null || !this.showSketchyEdges)
            {
                return;
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                if (this.enableDepthTest)
                {
                    this.drawBoundingbox(seg.boundingbox, Color.White);
                }
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    //this.drawGuideLineEndpoints(edge, Color.Gray, 2.0f);
                    int nstrokes = edge.strokes.Count;
                    if (this.inGuideMode)
                    {
                        nstrokes = nstrokes > 2 ? 2 : nstrokes;
                    }
                    for (int i = 0; i < nstrokes; ++i )
                    {
                        Stroke stroke = edge.strokes[i];
                        if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                        {
                            this.drawLines3D(stroke.u3, stroke.v3, Color.Gray, 2.0f);
                        }
                        if (this.drawShadedOrTexturedStroke)
                        {
                            this.drawTriMeshShaded3D(stroke, false, this.showOcclusion);
                        }
                        else
                        {
                            this.drawTriMeshTextured3D(stroke, this.showOcclusion);
                        }
                    }
                }
            }
        }// drawSketchyEdges3D

        private void drawGuideLines3D()
        {
            if (this.currSegmentClass == null || !this.showGuideLines)
            {
                return;
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;

                foreach (GuideLine line in seg.boundingbox.guideLines)
                {
                    if (!line.active) continue;

                    //this.drawGuideLineEndpoints(line, GLViewer.GuideLineColor, 2.0f);
                    foreach (Stroke stroke in line.strokes)
                    {
                        if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                        {
                            this.drawLines3D(stroke.u3, stroke.v3, Color.Pink, 2.0f);
                        }
                        if (this.drawShadedOrTexturedStroke)
                        {
                            this.drawTriMeshShaded3D(stroke, false, this.showOcclusion);
                        }
                        else
                        {
                            this.drawTriMeshTextured3D(stroke, this.showOcclusion);
                        }
                    }
                }
            }
            
        }// drawGuideLines3D

        private void drawGuideArrow(Arrow3D a, Color c)
        {
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glLineWidth(4.0f);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3dv(a.u.ToArray());
            Gl.glVertex3dv(a.v.ToArray());
            this.drawTriangle(a.cap);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glLineWidth(1.0f);
        }// drawGuideArrow

        private void drawTriangle(Triangle t)
        {
            Gl.glVertex3dv(t.u.ToArray());
            Gl.glVertex3dv(t.v.ToArray());
            Gl.glVertex3dv(t.v.ToArray());
            Gl.glVertex3dv(t.w.ToArray());
            Gl.glVertex3dv(t.w.ToArray());
            Gl.glVertex3dv(t.u.ToArray());
        }

        private void drawEllipseCurve3D(Ellipse3D e)
        {
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glBegin(Gl.GL_LINES);
            for (int i = 0; i < e.points.Length; ++i)
            {
                Gl.glVertex3dv(e.points[i].ToArray());
                Gl.glVertex3dv(e.points[(i + 1) % e.points.Length].ToArray());
            }
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
        }// drawEllipseCurve3D
        
        private void DrawHighlight3D()
        {
            if (this.currBoxIdx == -1 || this.currBoxIdx > this.currSegmentClass.segments.Count)
                return;

            Segment seg = this.currSegmentClass.segments[this.currBoxIdx];

            if (this.enableDepthTest)
            {
                Gl.glDisable(Gl.GL_DEPTH_TEST);
            }
            this.drawActiveBox(seg);
            this.drawActiveGuideLines(seg);
            this.drawLineToDraw();
            if (this.enableDepthTest)
            {
                Gl.glEnable(Gl.GL_DEPTH_TEST);
            }
        }// DrawHighlight3D

        private void drawActiveBox(Segment seg)
        {
            if (!this.inGuideMode) return;
            if (this.showOcclusion)
            {
                //this.drawBoundingbox(seg.boundingbox, Color.White);
            }
            // draw bounding edges
            foreach (GuideLine edge in seg.boundingbox.edges)
            {
                //this.drawGuideLineEndpoints(edge, Color.Gray, 1.0f);
                foreach (Stroke stroke in edge.strokes)
                {
                    if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                    {
                        this.drawLines3D(stroke.u3, stroke.v3, Color.Pink, 2.0f);
                    }
                    if (this.drawShadedOrTexturedStroke)
                    {
                        this.drawTriMeshShaded3D(stroke, false, this.showOcclusion);
                    }
                    else
                    {
                        this.drawTriMeshTextured3D(stroke, this.showOcclusion);
                    }
                }
                foreach (Ellipse3D e in seg.boundingbox.ellipses)
                {
                    this.drawEllipseCurve3D(e);
                }
                if (this.showVanishingLines)
                {
                    this.drawVanishingLines3d(seg.boundingbox);
                }
            }
        }// drawActiveBox


        private void drawActiveGuideLines(Segment seg)
        {
            // draw guidelines
            foreach (GuideLine line in seg.boundingbox.guideLines)
            {
                if (!line.active) continue;
                //this.drawGuideLineEndpoints(line, GLViewer.GuideLineColor, 2.0f);
                foreach (Stroke stroke in line.strokes)
                {
                    if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                    {
                        this.drawLines3D(stroke.u3, stroke.v3, Color.Pink, 2.0f);
                    }
                    this.drawTriMeshShaded3D(stroke, false, false);
                }
                if (line.isGuide)
                {
                    this.drawVanishingLines3d(line);
                }
            }
        }// drawActiveGuideLines

        
        private void drawLineToDraw()
        {
            if (this.readyForGuideArrow && this.nextBoxIdx != -1 && this.lineToDraw != -1)
            {
                this.drawGuideArrow(this.currSegmentClass.segments[this.nextBoxIdx].boundingbox.edges[this.lineToDraw].guideArrow, Color.Red);
            }
        }

        private void drawGuideLineEndpoints(GuideLine gline, Color c, float pointSize)
        {
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glPointSize(pointSize);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glVertex3dv(gline.u.ToArray());
            Gl.glVertex3dv(gline.v.ToArray());
            Gl.glEnd();
        }

        private void drawPoints(Vector3d[] points, Color c, float pointSize)
        {
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glPointSize(pointSize);
            Gl.glBegin(Gl.GL_POINTS);
            foreach (Vector3d v in points)
            {
                Gl.glVertex3dv(v.ToArray());
            }
            Gl.glEnd();
        }

        public void drawStrokeMesh3d(Stroke stroke)
        {
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
            //Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glDepthFunc(Gl.GL_LESS);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_NORMALIZE);

            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_POLYGON_SMOOTH);

            Gl.glColor4ub(stroke.strokeColor.R, stroke.strokeColor.G, stroke.strokeColor.B, stroke.strokeColor.A);

            Gl.glBegin(Gl.GL_TRIANGLES);
            for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            {
                int vidx1 = stroke.faceIndex[j];
                int vidx2 = stroke.faceIndex[j + 1];
                int vidx3 = stroke.faceIndex[j + 2];
                Gl.glVertex3dv(stroke.meshVertices3d[vidx1].ToArray());
                Gl.glVertex3dv(stroke.meshVertices3d[vidx2].ToArray());
                Gl.glVertex3dv(stroke.meshVertices3d[vidx3].ToArray());
            }
            Gl.glEnd();

            //Gl.glLineWidth(1.0f);
            //Gl.glBegin(Gl.GL_LINES);
            //for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            //{
            //    int vidx1 = stroke.faceIndex[j];
            //    int vidx2 = stroke.faceIndex[j + 1];
            //    int vidx3 = stroke.faceIndex[j + 2];
            //    Gl.glVertex3dv(stroke.meshVertices3d[vidx1].ToArray());
            //    Gl.glVertex3dv(stroke.meshVertices3d[vidx2].ToArray());
            //    Gl.glVertex3dv(stroke.meshVertices3d[vidx2].ToArray());
            //    Gl.glVertex3dv(stroke.meshVertices3d[vidx3].ToArray());
            //    Gl.glVertex3dv(stroke.meshVertices3d[vidx3].ToArray());
            //    Gl.glVertex3dv(stroke.meshVertices3d[vidx1].ToArray());
            //}
            //Gl.glEnd();

            //Gl.glColor3ub(0, 255, 0);
            //Gl.glPointSize(4.0f);
            //Gl.glBegin(Gl.GL_POINTS);
            //foreach (StrokePoint p in stroke.strokePoints)
            //{
            //    Gl.glVertex3dv(p.pos3.ToArray());
            //}
            //Gl.glEnd();

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_LIGHT0);
            //Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glDisable(Gl.GL_COLOR_MATERIAL);
            Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
        }

        private void drawPlane2D(Plane plane)
        {
            if (plane.points2 == null) return;
            Gl.glColor3ub(0, 0, 255);
            Gl.glPointSize(4.0f);
            Gl.glBegin(Gl.GL_POINTS);
            foreach (Vector2d p in plane.points2)
            {
                Gl.glVertex2dv(p.ToArray());
            }
            Gl.glEnd();
        }

        private void drawStrokeMesh2d(Stroke stroke)
        {
            if(stroke.meshVertices2d == null || stroke.faceIndex == null)
            {
                return;
            }

            Gl.glColor3ub(255, 0, 0);
            Gl.glPointSize(4.0f);
            Gl.glBegin(Gl.GL_POINTS);
            foreach (StrokePoint p in stroke.strokePoints)
            {
                Gl.glVertex2dv(p.pos2.ToArray());
            }
            Gl.glEnd();

            //Gl.glEnable(Gl.GL_CULL_FACE);
            //Gl.glEnable(Gl.GL_LIGHTING);

            //Gl.glColor4ub(stroke.strokeColor.R, stroke.strokeColor.G, stroke.strokeColor.B, stroke.strokeColor.A);

            //Gl.glBegin(Gl.GL_TRIANGLES);
            //for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            //{
            //    int vidx1 = stroke.faceIndex[j];
            //    int vidx2 = stroke.faceIndex[j + 1];
            //    int vidx3 = stroke.faceIndex[j + 2];
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx1].ToArray());
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx2].ToArray());
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx3].ToArray());
            //}
            //Gl.glEnd();

            //Gl.glLineWidth(1.0f);
            //Gl.glBegin(Gl.GL_LINES);
            //for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            //{
            //    int vidx1 = stroke.faceIndex[j];
            //    int vidx2 = stroke.faceIndex[j + 1];
            //    int vidx3 = stroke.faceIndex[j + 2];
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx1].ToArray());
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx2].ToArray());
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx2].ToArray());
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx3].ToArray());
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx3].ToArray());
            //    Gl.glVertex2dv(stroke.meshVertices2d[vidx1].ToArray());
            //}
            //Gl.glEnd();

            //Gl.glDisable(Gl.GL_LIGHTING);
            //Gl.glDisable(Gl.GL_CULL_FACE);

            //Gl.glLineWidth(2.0f);
        }

        int[] vp1 = { 1, 5, 4, 0 };
        int[] vp2 = { 1, 2, 3, 0 };
        private void drawVanishingLines3d(Box box)
        {            
            for (int i = 0; i < vp1.Length; ++i)
            {
                Line3d line = box.vanLines[0][vp1[i]];
                switch(this.vanishinglineDrawType){
                    case 1:
                        this.drawLines3D(line.u3, line.v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    case 2:
                        this.drawDashedLines3D(line.u3, line.v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    default:
                        break;
                }                
            }
            for (int i = 0; i < vp2.Length; ++i)
            {
                Line3d line = box.vanLines[1][vp2[i]];
                switch (this.vanishinglineDrawType)
                {
                    case 1:
                        this.drawLines3D(line.u3, line.v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    case 2:
                        this.drawDashedLines3D(line.u3, line.v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    default:
                        break;
                }  
            }
        }

        private void drawVanishingLines3d(GuideLine line)
        {
            switch (this.vanishinglineDrawType)
            {
                case 1:
                    {
                        this.drawLines3D(line.vanLines[0][0].u3, line.vanLines[0][0].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines3D(line.vanLines[0][1].u3, line.vanLines[0][1].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines3D(line.vanLines[1][0].u3, line.vanLines[1][0].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines3D(line.vanLines[1][1].u3, line.vanLines[1][1].v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    }
                case 2:
                    {
                        this.drawDashedLines3D(line.vanLines[0][0].u3, line.vanLines[0][0].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawDashedLines3D(line.vanLines[0][1].u3, line.vanLines[0][1].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawDashedLines3D(line.vanLines[1][0].u3, line.vanLines[1][0].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawDashedLines3D(line.vanLines[1][1].u3, line.vanLines[1][1].v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    }
                default:
                    break;
            }                         
        }

        private void drawVanishingLines2d(Box box)
        {
            for (int i = 0; i < vp1.Length; ++i)
            {
                Line3d line = box.vanLines[0][vp1[i]];
                switch (this.vanishinglineDrawType)
                {
                    case 1:
                        this.drawLines2D(line.u2, line.v2, SegmentClass.VanLineColor, 1.0f);
                        break;
                    case 2:
                        this.drawDashedLines2D(line.u2, line.v2, SegmentClass.VanLineColor, 1.0f);
                        break;
                    default:
                        break;
                }
            }
            for (int i = 0; i < vp2.Length; ++i)
            {
                Line3d line = box.vanLines[1][vp2[i]];
                switch (this.vanishinglineDrawType)
                {
                    case 1:
                        this.drawLines2D(line.u2, line.v2, SegmentClass.VanLineColor, 1.0f);
                        break;
                    case 2:
                        this.drawDashedLines2D(line.u2, line.v2, SegmentClass.VanLineColor, 1.0f);
                        break;
                    default:
                        break;
                }
            }
        }

        private void drawVanishingLines2d(GuideLine line)
        {
            switch (this.vanishinglineDrawType)
            {
                case 1:
                    {
                        this.drawLines2D(line.vanLines[0][0].u2, line.vanLines[0][0].v2, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines2D(line.vanLines[0][1].u2, line.vanLines[0][1].v2, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines2D(line.vanLines[1][0].u2, line.vanLines[1][0].v2, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines2D(line.vanLines[1][1].u2, line.vanLines[1][1].v2, SegmentClass.VanLineColor, 1.0f);
                        break;
                    }
                case 2:
                    {
                        this.drawDashedLines2D(line.vanLines[0][0].u2, line.vanLines[0][0].v2, SegmentClass.VanLineColor, 1.0f);
                        this.drawDashedLines2D(line.vanLines[0][1].u2, line.vanLines[0][1].v2, SegmentClass.VanLineColor, 1.0f);
                        this.drawDashedLines2D(line.vanLines[1][0].u2, line.vanLines[1][0].v2, SegmentClass.VanLineColor, 1.0f);
                        this.drawDashedLines2D(line.vanLines[1][1].u2, line.vanLines[1][1].v2, SegmentClass.VanLineColor, 1.0f);
                        break;
                    }
                default:
                    break;
            }   
            
        }

        private void drawLines2D(Vector2d v1, Vector2d v2, Color c, float linewidth)
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(linewidth);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glVertex2dv(v1.ToArray());
            Gl.glVertex2dv(v2.ToArray());
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glLineWidth(1.0f);
        }

        private void drawDashedLines2D(Vector2d v1, Vector2d v2, Color c, float linewidth)
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(linewidth);
            Gl.glLineStipple(1, 0x00FF);
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3dv(v1.ToArray());
            Gl.glVertex3dv(v2.ToArray());
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_LINE_STIPPLE);

            Gl.glLineWidth(1.0f);
        }

        private void drawLines3D(Vector3d v1, Vector3d v2, Color c, float linewidth)
        {

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(linewidth);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3dv(v1.ToArray());
            Gl.glVertex3dv(v2.ToArray());
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glLineWidth(1.0f);
        }

        private void drawDashedLines3D(Vector3d v1, Vector3d v2, Color c, float linewidth)
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(linewidth);
            Gl.glLineStipple(1, 0x00FF);
            Gl.glEnable(Gl.GL_LINE_STIPPLE);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3dv(v1.ToArray());
            Gl.glVertex3dv(v2.ToArray());
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_LINE_STIPPLE);

            Gl.glLineWidth(1.0f);
        }

        private void drawAxes()
        {
            // draw axes with arrows
            for (int i = 0; i < 6; i += 2)
            {
                this.drawLines3D(axes[i], axes[i + 1], Color.Red, 2.0f);
            }

            for (int i = 6; i < 12; i += 2)
            {
                this.drawLines3D(axes[i], axes[i + 1], Color.Green, 2.0f);
            }

            for (int i = 12; i < 18; i += 2)
            {
                this.drawLines3D(axes[i], axes[i + 1], Color.Blue, 2.0f);
            }
            Gl.glEnd();
        }

        private void clearScene()
        {
            Gl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
        }

        private void drawQuad2d(Quad2d q, Color c)
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            //Glu.gluOrtho2D(0, this.Width, 0, this.Height);
            Glu.gluOrtho2D(0, this.Width, this.Height, 0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glColor4ub(c.R, c.G, c.B, 100);
            Gl.glBegin(Gl.GL_POLYGON);
            for (int i = 0; i < 4; ++i)
            {
                Gl.glVertex2dv(q.points[i].ToArray());
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glLineWidth(2.0f);
            Gl.glBegin(Gl.GL_LINES);
            for (int i = 0; i < 4; ++i)
            {
                Gl.glVertex2dv(q.points[i].ToArray());
                Gl.glVertex2dv(q.points[(i + 1) % 4].ToArray());
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_SMOOTH);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
        }

        private void drawQuad3d(Plane q, Color c)
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            // face
            Gl.glColor4ub(c.R, c.G, c.B, c.A);
            Gl.glBegin(Gl.GL_POLYGON);
            for (int i = 0; i < 4; ++i)
            {
                Gl.glVertex3dv(q.points[i].ToArray());
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_BLEND);
        }

        private void drawQuadEdge3d(Plane q, Color c)
        {
            for (int i = 0; i < 4; ++i)
            {
                this.drawLines3D(q.points[i], q.points[(i + 1) % q.points.Length], c, 4.0f);
            }
        }

        // draw mesh
        public void drawMesh(Mesh m, Color c, bool useMeshColor)
        {
            if (m == null) return;
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
            //Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glDepthFunc(Gl.GL_LESS);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_NORMALIZE);

            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_POLYGON_SMOOTH);

            if (useMeshColor)
            {
                for (int i = 0, j = 0; i < m.FaceCount; ++i, j += 3)
                {
                    int vidx1 = m.FaceVertex[j];
                    int vidx2 = m.FaceVertex[j + 1];
                    int vidx3 = m.FaceVertex[j + 2];
                    Vector3d v1 = new Vector3d(
                        m.VertexPos[vidx1 * 3], m.VertexPos[vidx1 * 3 + 1], m.VertexPos[vidx1 * 3 + 2]);
                    Vector3d v2 = new Vector3d(
                        m.VertexPos[vidx2 * 3], m.VertexPos[vidx2 * 3 + 1], m.VertexPos[vidx2 * 3 + 2]);
                    Vector3d v3 = new Vector3d(
                        m.VertexPos[vidx3 * 3], m.VertexPos[vidx3 * 3 + 1], m.VertexPos[vidx3 * 3 + 2]);
                    Color fc = Color.FromArgb(m.FaceColor[i * 4 + 3], m.FaceColor[i * 4], m.FaceColor[i * 4 + 1], m.FaceColor[i * 4 + 2]);
                    Gl.glColor4ub(fc.R, fc.G, fc.B, fc.A);
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    Gl.glNormal3d(m.VertexNormal[vidx1 * 3], m.VertexNormal[vidx1 * 3 + 1], m.VertexNormal[vidx1 * 3 + 2]);
                    Gl.glVertex3d(v1.x, v1.y, v1.z);
                    Gl.glNormal3d(m.VertexNormal[vidx2 * 3], m.VertexNormal[vidx2 * 3 + 1], m.VertexNormal[vidx2 * 3 + 2]);
                    Gl.glVertex3d(v2.x, v2.y, v2.z);
                    Gl.glNormal3d(m.VertexNormal[vidx3 * 3], m.VertexNormal[vidx3 * 3 + 1], m.VertexNormal[vidx3 * 3 + 2]);
                    Gl.glVertex3d(v3.x, v3.y, v3.z);
                    Gl.glEnd();
                }
            }
            else
            {
                Gl.glColor3ub(c.R, c.G, c.B);
                Gl.glBegin(Gl.GL_TRIANGLES);
                for (int i = 0, j = 0; i < m.FaceCount; ++i, j += 3)
                {
                    int vidx1 = m.FaceVertex[j];
                    int vidx2 = m.FaceVertex[j + 1];
                    int vidx3 = m.FaceVertex[j + 2];
                    Vector3d v1 = new Vector3d(
                        m.VertexPos[vidx1 * 3], m.VertexPos[vidx1 * 3 + 1], m.VertexPos[vidx1 * 3 + 2]);
                    Vector3d v2 = new Vector3d(
                        m.VertexPos[vidx2 * 3], m.VertexPos[vidx2 * 3 + 1], m.VertexPos[vidx2 * 3 + 2]);
                    Vector3d v3 = new Vector3d(
                        m.VertexPos[vidx3 * 3], m.VertexPos[vidx3 * 3 + 1], m.VertexPos[vidx3 * 3 + 2]);
                    Gl.glNormal3d(m.VertexNormal[vidx1 * 3], m.VertexNormal[vidx1 * 3 + 1], m.VertexNormal[vidx1 * 3 + 2]);
                    Gl.glVertex3d(v1.x, v1.y, v1.z);
                    Gl.glNormal3d(m.VertexNormal[vidx2 * 3], m.VertexNormal[vidx2 * 3 + 1], m.VertexNormal[vidx2 * 3 + 2]);
                    Gl.glVertex3d(v2.x, v2.y, v2.z);
                    Gl.glNormal3d(m.VertexNormal[vidx3 * 3], m.VertexNormal[vidx3 * 3 + 1], m.VertexNormal[vidx3 * 3 + 2]);
                    Gl.glVertex3d(v3.x, v3.y, v3.z);
                }
                Gl.glEnd();
            }

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_LIGHT0);
            //Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glDisable(Gl.GL_COLOR_MATERIAL);
            Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
        }

        public void drawBoundingboxWithEdges(Box box, Color c)
        {
            if (box == null) return;
            for (int i = 0; i < box.planes.Length; ++i)
            {
                this.drawQuad3d(box.planes[i], c);
                // lines
                for (int j = 0; j < 4; ++j)
                {
                    this.drawLines3D(box.planes[i].points[j], box.planes[i].points[(j + 1) % 4], Color.Black, 2.0f);
                }
            }
        }// drawBoundingboxWithEdges

        public void drawBoundingbox(Box box, Color c)
        {
            if (box == null) return;
            for (int i = 0; i < box.planes.Length; ++i)
            {
                this.drawQuad3d(box.planes[i], c);
            }
        }// drawBoundingbox

        // draw

        public void drawTriMeshShaded3D(Stroke stroke, bool highlight, bool useOcclusion)
        {
            Gl.glPushAttrib(Gl.GL_COLOR_BUFFER_BIT);

            int iMultiSample = 0;
            int iNumSamples = 0;
            Gl.glGetIntegerv(Gl.GL_SAMPLE_BUFFERS, out iMultiSample);
            Gl.glGetIntegerv(Gl.GL_SAMPLES, out iNumSamples);
            if (iNumSamples == 0)
            {
                //Gl.glEnable(Gl.GL_DEPTH_TEST);
                //Gl.glDepthMask(Gl.GL_FALSE);
                //Gl.glEnable(Gl.GL_CULL_FACE);

                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

                Gl.glEnable(Gl.GL_POLYGON_SMOOTH);
                Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
                Gl.glEnable(Gl.GL_LINE_SMOOTH);
                Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);


                Gl.glEnable(Gl.GL_BLEND);
                //Gl.glBlendEquation(Gl.GL_ADD);
                //Gl.glBlendFunc(Gl.GL_SRC_ALPHA_SATURATE, Gl.GL_ONE_MINUS_SRC_ALPHA);
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                Gl.glShadeModel(Gl.GL_SMOOTH);          
            }
            else
            {
                Gl.glEnable(Gl.GL_MULTISAMPLE);
                Gl.glHint(Gl.GL_MULTISAMPLE_FILTER_HINT_NV, Gl.GL_NICEST);
                Gl.glEnable(Gl.GL_SAMPLE_ALPHA_TO_ONE);
            }

            for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            {
                int vidx1 = stroke.faceIndex[j];
                int vidx2 = stroke.faceIndex[j + 1];
                int vidx3 = stroke.faceIndex[j + 2];

                int ipt = (i + stroke.ncapoints) / 2;
                if (ipt < 0) ipt = 0;
                if (ipt >= stroke.strokePoints.Count) ipt = stroke.strokePoints.Count - 1;
                StrokePoint pt = stroke.strokePoints[ipt];

                byte opa = (byte)(pt.depth * 255);

                if (useOcclusion)
                {
                    opa = opa >= 0 ? opa : pt.opacity;
                    opa = opa < 255 ? opa : pt.opacity;
                }
                else
                    opa = pt.opacity;
                if (!highlight)
                {
                    Gl.glColor4ub(stroke.strokeColor.R, stroke.strokeColor.G, stroke.strokeColor.B, (byte)opa);
                }
                else
                {
                    Gl.glColor4ub(GuideLineColor.R, GuideLineColor.G, GuideLineColor.B, (byte)opa);
                }
                //Gl.glBegin(Gl.GL_TRIANGLES);
                Gl.glBegin(Gl.GL_POLYGON);
                //Gl.glNormal3dv(stroke.hostPlane.normal.ToArray());
                Gl.glVertex3dv(stroke.meshVertices3d[vidx1].ToArray());
                //Gl.glNormal3dv(stroke.hostPlane.normal.ToArray());
                Gl.glVertex3dv(stroke.meshVertices3d[vidx2].ToArray());
                //Gl.glNormal3dv(stroke.hostPlane.normal.ToArray());
                Gl.glVertex3dv(stroke.meshVertices3d[vidx3].ToArray());
                Gl.glEnd();
            }          


            if (iNumSamples == 0)
            {
                Gl.glDisable(Gl.GL_BLEND);
                Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
                //Gl.glDisable(Gl.GL_CULL_FACE);
                //Gl.glDepthMask(Gl.GL_TRUE);
                //Gl.glDisable(Gl.GL_DEPTH_TEST);
            }
            else
            {
                Gl.glDisable(Gl.GL_MULTISAMPLE);
            }

            Gl.glPopAttrib();
        }

        public void drawTriMeshTextured3D(Stroke stroke, bool useOcclusion)
        {
            uint tex_id = GLViewer.pencilTextureId;
            switch ((int)SegmentClass.strokeStyle)
            {
                case 3:
                    tex_id = GLViewer.crayonTextureId;
                    break;
                case 4:
                    tex_id = GLViewer.inkTextureId;
                    break;
                case 0:
                default:
                    tex_id = GLViewer.pencilTextureId;
                    break;
            }

            Gl.glPushAttrib(Gl.GL_COLOR_BUFFER_BIT);

            int iMultiSample = 0;
            int iNumSamples = 0;
            Gl.glGetIntegerv(Gl.GL_SAMPLE_BUFFERS, out iMultiSample);
            Gl.glGetIntegerv(Gl.GL_SAMPLES, out iNumSamples);
            if (iNumSamples == 0)
            {
                Gl.glEnable(Gl.GL_POINT_SMOOTH);
                Gl.glEnable(Gl.GL_LINE_SMOOTH);
                Gl.glEnable(Gl.GL_POLYGON_SMOOTH);

                Gl.glDisable(Gl.GL_CULL_FACE);
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
                Gl.glShadeModel(Gl.GL_SMOOTH);
                Gl.glDisable(Gl.GL_DEPTH_TEST);

                Gl.glEnable(Gl.GL_BLEND);
                Gl.glBlendEquation(Gl.GL_ADD);
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);

                Gl.glDepthMask(Gl.GL_FALSE);

                Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
                Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);
                Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
            }
            else
            {
                Gl.glEnable(Gl.GL_MULTISAMPLE);
                Gl.glHint(Gl.GL_MULTISAMPLE_FILTER_HINT_NV, Gl.GL_NICEST);
                Gl.glEnable(Gl.GL_SAMPLE_ALPHA_TO_ONE);
            }

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, tex_id);

            for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            {
                int j1 = stroke.faceIndex[j];
                int j2 = stroke.faceIndex[j + 1];
                int j3 = stroke.faceIndex[j + 2];

                Vector3d pos1 = stroke.meshVertices3d[j1];
                Vector3d pos2 = stroke.meshVertices3d[j2];
                Vector3d pos3 = stroke.meshVertices3d[j3];

                int ipt = (i + stroke.ncapoints) / 2;
                if (ipt < 0) ipt = 0;
                if (ipt >= stroke.strokePoints.Count) ipt = stroke.strokePoints.Count - 1;
                StrokePoint pt = stroke.strokePoints[ipt];

                byte opa = (byte)(pt.depth * 255);

                if (useOcclusion)
                {
                    opa = opa >= 0 ? opa : pt.opacity;
                    opa = opa < 255 ? opa : pt.opacity;
                }
                else
                    opa = pt.opacity;

                Gl.glColor4ub(255, 255, 255, (byte)opa);

                Gl.glBegin(Gl.GL_POLYGON);

                if (i % 2 == 1)
                {
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex3d(pos1.x, pos1.y, pos1.z);
                    Gl.glTexCoord2d(0, 1);
                    Gl.glVertex3d(pos2.x, pos2.y, pos2.z);
                    Gl.glTexCoord2d(1, 1);
                    Gl.glVertex3d(pos3.x, pos3.y, pos3.z);
                }
                else
                {
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex3d(pos1.x, pos1.y, pos1.z);
                    Gl.glTexCoord2d(1, 1);
                    Gl.glVertex3d(pos3.x, pos3.y, pos3.z);
                    Gl.glTexCoord2d(1, 0);
                    Gl.glVertex3d(pos2.x, pos2.y, pos2.z);
                }

                Gl.glEnd();
            }

            Gl.glDisable(Gl.GL_TEXTURE_2D);

            if (iNumSamples == 0)
            {
                Gl.glDisable(Gl.GL_BLEND);
                Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
                Gl.glDisable(Gl.GL_POINT_SMOOTH);
                Gl.glDisable(Gl.GL_LINE_SMOOTH);
                Gl.glDepthMask(Gl.GL_TRUE);
            }
            else
            {
                Gl.glDisable(Gl.GL_MULTISAMPLE);
            }
            Gl.glPopAttrib();
        }

        private void drawSphere()
        {
            Gl.glColor3ub(255, 0, 0);
            Gl.glTranslated(this.vanishingPoints[0].pos3.x, this.vanishingPoints[0].pos3.y, this.vanishingPoints[0].pos3.z);
            Glu.GLUquadric quad0 = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quad0, Glu.GLU_FILL);
            Glu.gluSphere(quad0, 0.01, 20, 20);
            Glu.gluDeleteQuadric(quad0);

            Gl.glTranslated(-this.vanishingPoints[0].pos3.x, -this.vanishingPoints[0].pos3.y, -this.vanishingPoints[0].pos3.z);

            Gl.glTranslated(this.vanishingPoints[1].pos3.x, this.vanishingPoints[1].pos3.y, this.vanishingPoints[1].pos3.z);
            Glu.GLUquadric quad1 = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quad1, Glu.GLU_FILL);
            Glu.gluSphere(quad1, 0.01, 20, 20);
            Glu.gluDeleteQuadric(quad1);

            Gl.glTranslated(-this.vanishingPoints[1].pos3.x, -this.vanishingPoints[1].pos3.y, -this.vanishingPoints[1].pos3.z);
        }
    }// GLViewer
}// namespace
