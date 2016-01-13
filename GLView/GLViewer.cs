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
            this.startWid = this.Width;
            this.startHeig = this.Height;

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
            ColorSet[11] = Color.FromArgb(252, 141, 98);
            ColorSet[12] = Color.FromArgb(166, 216, 84);
            ColorSet[13] = Color.FromArgb(231, 138, 195);
            ColorSet[14] = Color.FromArgb(141, 211, 199);
            ColorSet[15] = Color.FromArgb(255, 255, 179);
            ColorSet[16] = Color.FromArgb(251, 128, 114);
            ColorSet[17] = Color.FromArgb(179, 222, 105);
            ColorSet[18] = Color.FromArgb(188, 128, 189);
            ColorSet[19] = Color.FromArgb(217, 217, 217);

            ModelColor = Color.FromArgb(254,224,139);//(166, 189, 219);
            GuideLineColor = Color.FromArgb(116, 169, 207); //Color.FromArgb(4, 90, 141);// Color.FromArgb(0, 15, 85); // pen ink blue
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
            Viewing, VertexSelection, EdgeSelection, FaceSelection, ComponentSelection, Guide, 
            Sketch, Eraser, NONE
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
        public bool lockView = false;
        public bool showFaceToDraw = true;

        public bool showSharpEdge = false;
        public bool enableHiddencheck = true;

        public bool showSegSilhouette = false;
        public bool showSegContour = false;
        public bool showSegSuggestiveContour = false;
        public bool showSegApparentRidge = false;
        public bool showSegBoundary = false;
        public bool showLineOrMesh = true;

        public bool showDrawnStroke = true;

        public bool showBlinking = false;

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
        public string foldername;
        public static Random rand = new Random();
        private bool readyForGuideArrow = false;
        private Vector3d objectCenter = new Vector3d();
        private enum Depthtype
        {
            opacity, hidden, OpenGLDepthTest, none, rayTracing // test 
        }
        private Depthtype depthType = Depthtype.opacity;
        private int vanishinglineDrawType = 0;
        private Line3d animatedLine = null;

        //########## sequence vars ##########//
        private int sequenceIdx = -1;
        private int nSequence = 0;
        public int currBoxIdx = -1;
        public int nextBoxIdx = -1;
        public Segment activeSegment = null;
        private bool drawPrevBoxGuideLines = false;

        public bool showVanishingRay1 = true;
        public bool showVanishingRay2 = true;
        public bool showVanishingPoints = true;
        public bool showBoxVanishingLine = true;
        public bool showGuideLineVanishingLine = false;
        private List<int> boxShowSequence = new List<int>();
        private List<string> pageNumber;

        //########## sketch vars ##########//
        private List<Vector2d> currSketchPoints;
        private double strokeLength = 0;
        private Stroke currStroke = null;
        private List<Stroke> sketchStrokes = new List<Stroke>();

        //########## static vars ##########//
        public static Color[] ColorSet;
        static public Color ModelColor;
        public static Color GuideLineColor;
        private StrokePoint[] paperPos = null;
        private Vector2d[] paperPosLines = null;

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

        private void clearContext()
        {
            this.currMeshClass = null;
            this.currSegmentClass = null;
        }

        public void loadMesh(string filename)
        {
            this.clearContext();

            Mesh m = new Mesh(filename, true);
            MeshClass mc = new MeshClass(m);
            this.meshClasses.Add(mc);
            this.currMeshClass = mc;

            meshStats = new int[3];
            meshStats[0] = m.VertexCount;
            meshStats[1] = m.Edges.Length;
            meshStats[2] = m.FaceCount;
        }

        public void loadTriMesh(string filename)
        {
            //MessageBox.Show("Trimesh is not activated in this version.");
            //return;

            this.clearContext();

            this.currSegmentClass = null;
            MeshClass mc = new MeshClass();
            this.contourPoints = mc.loadTriMesh(filename, this.eye);
            this.meshClasses.Add(mc);
            this.currMeshClass = mc;
            this.Refresh();
        }

        public void loadSegments(string segfolder)
        {
            this.clearContext();

            this.segmentClasses = new List<SegmentClass>();
            int idx = segfolder.LastIndexOf('\\');
            string bbofolder = segfolder.Substring(0, idx + 1);
            bbofolder += "bounding_boxes\\";
            SegmentClass sc = new SegmentClass();
            sc.ReadSegments(segfolder, bbofolder);
            this.setRandomSegmentColor(sc);
            this.segmentClasses.Add(sc);
            this.currSegmentClass = sc;
            this.calculateSketchMesh2d();
            segStats = new int[2];
            segStats[0] = sc.segments.Count;
        }// loadSegments
        
        public void loadJSONFile(string jsonFile)
        {
            this.clearContext();

            this.foldername = jsonFile.Substring(0, jsonFile.LastIndexOf('\\'));

            SegmentClass sc = new SegmentClass();
            //Matrix4d m = sc.DeserializeJSON(jsonFile, out this.objectCenter);
            Matrix4d m = sc.DeserializeJSON(jsonFile, out this.objectCenter, out this.pageNumber);

            if (m != null)
            {
                m[2, 3] = 0; // z
                this.currModelTransformMatrix = m;
                this.eye = new Vector3d(0, 0, 1);
            }

            string viewFile = foldername + "\\view.mat"; 
            if (File.Exists(viewFile))
            {
                this.readModelModelViewMatrix(viewFile);
            }
            this.fixedModelView = new Matrix4d(this.currModelTransformMatrix);

            this.updateCamera();
            this.segmentClasses.Add(sc);
            this.setRandomSegmentColor(sc);
            this.currSegmentClass = sc;
            this.setGuideLineStyle((int)SegmentClass.GuideLineStyle);
            this.calculateSketchMesh2d();

            segStats = new int[2];
            segStats[0] = sc.segments.Count;
            this.setGuideLineColor(GLViewer.GuideLineColor);

            this.resetHighlightVars();

            // write sequence file
            
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

            this.testLines = new List<GuideLine>();

            this.Refresh();
            this.boxShowSequence = this.currSegmentClass.parseBoxSeqIndex();
            this.sequenceIdx = -1;
            this.currBoxIdx = -1;
            this.activeSegment = null;
            this.cal2D();
            if (this.pageNumber != null)
            {
                string totalPageStr = "/" + this.pageNumber[this.pageNumber.Count - 1];
                for (int i = 0; i < this.pageNumber.Count; ++i )
                {
                    this.pageNumber[i] += totalPageStr;
                }
            }
            this.loadSketches("");
        }// loadJSONFile
        

        private void cal2D()
        {
            // otherwise when glViewe is initialized, it will run this function from MouseUp()
            if (this.currSegmentClass == null) return;

            // reset the current 3d transformation again to check in the camera info, projection/modelview
             Gl.glViewport(0, 0, this.Width, this.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            double aspect = (double)this.Width / this.Height;
            Glu.gluPerspective(90, aspect, 0.1, 1000);
            Glu.gluLookAt(0, 0, 1.5, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);            

            Matrix4d m = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
            m = Matrix4d.TranslationMatrix(this.objectCenter) * m * Matrix4d.TranslationMatrix(
                new Vector3d() - this.objectCenter);


            //Gl.glMatrixMode(Gl.GL_MODELVIEW);
            //Gl.glPushMatrix();
            //Gl.glMultMatrixd(m.Transpose().ToArray());

            this.calculatePoint2DInfo();
            this.calculateVanishingPoints();

            //Gl.glMatrixMode(Gl.GL_MODELVIEW);
            //Gl.glPopMatrix();


            this.calculatePaperPosition();

            
        }//cal2D

        private void calculatePoint2DInfo()
        {
            this.updateCamera();
            if (this.currSegmentClass == null) return;
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
                    line.u2 = this.camera.Project(line.u).ToVector2d();
                    line.v2 = this.camera.Project(line.v).ToVector2d();
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
                //if (box.vanLines != null)
                //{
                //    for (int i = 0; i < box.vanLines.Length; ++i)
                //    {
                //        foreach (Line3d line in box.vanLines[i])
                //        {
                //            line.u2 = this.camera.Project(line.u3).ToVector2d();
                //            line.v2 = this.camera.Project(line.v3).ToVector2d();
                //        }
                //    }
                //}
                //for (int g = 0; g < box.guideLines.Count; ++g)
                //{
                //    foreach (GuideLine line in box.guideLines[g])
                //    {
                //        if (line.vanLines != null)
                //        {
                //            for (int i = 0; i < line.vanLines.Length; ++i)
                //            {
                //                foreach (Line3d l in line.vanLines[i])
                //                {
                //                    l.u2 = this.camera.Project(l.u3).ToVector2d();
                //                    l.v2 = this.camera.Project(l.v3).ToVector2d();
                //                }
                //            }
                //        }
                //    }
                //}// guidelines
            }
        }//calculatePoint2DInfo

        private void calSegmentsBounds(out Vector2d minCoord, out Vector2d maxCoord)
        {
            minCoord = Vector2d.MaxCoord();
            maxCoord = Vector2d.MinCoord();
            if (this.currSegmentClass == null) return;
            
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                Box box = seg.boundingbox;
                foreach (Plane plane in seg.boundingbox.planes)
                {
                    plane.points2 = new Vector2d[plane.points.Length];
                    for (int i = 0; i < plane.points.Length; ++i)
                    {
                        Vector3d v3 = this.camera.Project(plane.points[i]);
                        plane.points2[i] = v3.ToVector2d();
                        minCoord = Vector2d.Min(minCoord, plane.points2[i]);
                        maxCoord = Vector2d.Max(maxCoord, plane.points2[i]);
                    }
                }
            }
            Vector2d center = (maxCoord + minCoord) / 2;
        }//calSegmentsBounds

        private void calculatePaperPosition()
        {
            // for printed paper tutorial
            if (this.currSegmentClass == null) return;
            this.paperPos = new StrokePoint[4];
           
            Vector2d minCoord = Vector2d.MaxCoord(), maxCoord = Vector2d.MinCoord();
            this.calSegmentsBounds(out minCoord, out maxCoord);

            Vector2d center = (maxCoord + minCoord) / 2;
            double w = 297, h = 210;
            int sx = this.Location.X, sy = this.Location.Y;
            sx = 0;
            sy = 0;
            minCoord = Vector2d.Min(minCoord, this.vanishingPoints[0].pos2);
            maxCoord = Vector2d.Max(maxCoord, this.vanishingPoints[1].pos2);

            int off = 20;
            double heig = this.Height - off * 2;
            double wid = heig * w / h;

            minCoord -= new Vector2d(this.Location.X, this.Location.Y);
            maxCoord -= new Vector2d(this.Location.X, this.Location.Y);

            double y0 = sy + off * 2; // Math.Max(sy + 10, minCoord.y - off * 2);
            double x0 = center.x - wid / 2;// Math.Max(sx + 10, center.x - this.Width / 2 + 10);
            double y1 = sy + this.Height - off;// Math.Min(sy + this.Height - 10, maxCoord.y + off);
            //x0 -= 20;
            this.paperPos[0] = new StrokePoint(new Vector2d(x0, y0));
            this.paperPos[3] = new StrokePoint(new Vector2d(this.paperPos[0].pos2.x, y1));

            double xx = w / h * (this.paperPos[3].pos2.y - this.paperPos[0].pos2.y);
            this.paperPos[1] = new StrokePoint(new Vector2d(this.paperPos[0].pos2.x + xx, this.paperPos[0].pos2.y));
            this.paperPos[2] = new StrokePoint(new Vector2d(this.paperPos[1].pos2.x, this.paperPos[3].pos2.y));

            //Vector3d v0 = this.camera.Project(new Vector3d());
            //for (int i = 0; i < this.paperPos.Length; ++i)
            //{
            //    this.paperPos[i].pos3 = this.camera.ProjectPointToPlane(this.paperPos[i].pos2, new Vector3d(), new Vector3d(0, 0, 1));
            //}

            this.paperPosLines = new Vector2d[8];
            Vector2d xd = new Vector2d(1, 0);
            Vector2d yd = new Vector2d(0, 1);
            off = 30;
            this.paperPosLines[0] = this.paperPos[0].pos2 + xd * off;
            this.paperPosLines[1] = this.paperPos[0].pos2 + yd * off;

            this.paperPosLines[2] = this.paperPos[1].pos2 - xd * off;
            this.paperPosLines[3] = this.paperPos[1].pos2 + yd * off;

            this.paperPosLines[4] = this.paperPos[2].pos2 - xd * off;
            this.paperPosLines[5] = this.paperPos[2].pos2 - yd * off;

            this.paperPosLines[6] = this.paperPos[3].pos2 + xd * off;
            this.paperPosLines[7] = this.paperPos[3].pos2 - yd * off;

            int x = (int)this.paperPos[2].pos2.x;
            int y = this.Location.Y + (int)this.paperPos[0].pos2.y + 20;
            Program.GetFormMain().setPageNumberLocation(x,y);
            Program.GetFormMain().Refresh();
            this.Refresh();
        }// calculatePaperPosition

        private void calculateVanishingPoints()
        {
            if (this.currSegmentClass == null) return;
            this.vanishingPoints = new StrokePoint[2];
            Box b = this.currSegmentClass.segments[0].boundingbox;
            Line2d l1 = new Line2d(b.points2[2], b.points2[1]);
            Line2d l2 = new Line2d(b.points2[6], b.points2[5]);
            Vector2d v = Polygon.LineIntersectionPoint2d(l1, l2);

            
            Vector3d v3 = this.camera.ProjectPointToPlane(v, b.planes[2].center, b.planes[2].normal);
            this.vanishingPoints[0] = new StrokePoint(v3);
            this.vanishingPoints[0].pos2 = v;

            l1 = new Line2d(b.points2[6], b.points2[2]);
            l2 = new Line2d(b.points2[5], b.points2[1]);
            v = Polygon.LineIntersectionPoint2d(l1, l2);
            v3 = this.camera.ProjectPointToPlane(v, b.planes[2].center, b.planes[2].normal);
            this.vanishingPoints[1] = new StrokePoint(v3);
            this.vanishingPoints[1].pos2 = v;

            //this.calculateVanishingLine();
            this.calculateVanishingLine2d();

            //this.vanishingPoints[0].pos3 = new Vector3d(0, 0, -2);
            //this.vanishingPoints[1].pos3 = new Vector3d(-2, 0, 0);

            //Vector3d zline1 = (b.points[0] - b.points[3]).normalize();
            //Vector3d zline2 = (b.points[4] - b.points[7]).normalize();
            //int s = 1;
            //Vector3d vp1 = zline1 * 0.1 * s;
            //while (true)
            //{
            //    Vector2d v2 = this.camera.Project(vp1).ToVector2d();
            //    double dist = (v2 - this.vanishingPoints[0].pos2).Length();
            //    if (dist < 10)
            //    {
            //        break;
            //    }
            //    ++s;
            //    vp1 = b.points[3] + zline1 * 0.2 * s;
            //}
            //this.vanishingPoints[0].pos3 = vp1;

            //Vector3d xline1 = (b.points[3] - b.points[7]).normalize();
            //Vector3d xline2 = (b.points[0] - b.points[4]).normalize();

            //Vector3d vp2 = xline1 * 0.5 * s;
            //while (true)
            //{
            //    Vector2d v2 = this.camera.Project(vp2).ToVector2d();
            //    double dist = (v2 - this.vanishingPoints[1].pos2).Length();
            //    if (dist < 10)
            //    {
            //        break;
            //    }
            //    ++s;
            //    vp2 = zline1 * 0.1 * s;
            //}
            //this.vanishingPoints[1].pos3 = vp2;
        }

        private void calculateVanishingLine2d()
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                Box box = seg.boundingbox;
                box.buildVanishingLines2d(this.vanishingPoints[0].pos2, 0);
                box.buildVanishingLines2d(this.vanishingPoints[1].pos2, 1);
                for (int g = 0; g < box.guideLines.Count; ++g)
                {
                    foreach (GuideLine line in box.guideLines[g])
                    {
                        line.buildVanishingLines2d(this.vanishingPoints[0].pos2, 0);
                        line.buildVanishingLines2d(this.vanishingPoints[1].pos2, 1);
                    }
                }
            }
        }// calculateVanishingLine

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
                for (int g = 0; g < box.guideLines.Count; ++g)
                {
                    foreach (GuideLine line in box.guideLines[g])
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

        public void setStrokeSizePerSeg(double size)
        {
            SegmentClass.StrokeSize = size;
            if (this.currSegmentClass == null) return;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                this.setStrokeSizePerSeg(size, seg, Color.White, Color.White);
            }
        }//setStrokeSizePerSeg

        private void setStrokeSizePerSeg(double size, Segment seg, Color c, Color gc)
        {
            Box box = seg.boundingbox;
            foreach (GuideLine edge in box.edges)
            {
                for (int i = 0; i < edge.strokes.Count; ++i)
                {
                    Stroke stroke = edge.strokes[i];
                    if (c != Color.White)
                    {
                        stroke.strokeColor = c;
                    }
                    if (i == 0)
                    {
                        stroke.setStrokeSize(size);
                    }
                    else
                    {
                        stroke.setStrokeSize((double)size * 0.7);
                    }
                    stroke.changeStyle((int)SegmentClass.strokeStyle);
                }
            }
            for (int g = 0; g < box.guideLines.Count; ++g)
            {
                foreach (GuideLine line in box.guideLines[g])
                {
                    for (int i = 0; i < line.strokes.Count; ++i )
                    {
                        Stroke stroke = line.strokes[i];
                        if (c != Color.White)
                        {
                            stroke.strokeColor = gc;
                        }
                        if (i == 0)
                        {
                            stroke.setStrokeSize(size);
                        }
                        else
                        {
                            stroke.setStrokeSize((double)size * 0.7);
                        }
                        stroke.changeStyle((int)SegmentClass.strokeStyle);
                    }
                }
            }
        }// setStrokeSizePerSeg - per seg

        private void setStrokeStylePerLine(GuideLine line, double size, Color c)
        {
            for (int i = 0; i < line.strokes.Count; ++i)
            {
                Stroke stroke = line.strokes[i];
                if (c != Color.White)
                {
                    stroke.strokeColor = c;
                }
                if (i == 0)
                {
                    stroke.setStrokeSize(size);
                }
                else
                {
                    stroke.setStrokeSize((double)size * 0.7);
                    stroke.strokeColor = SegmentClass.sideStrokeColor;
                }

                stroke.changeStyle((int)SegmentClass.strokeStyle);
            }

        }// setStrokeStylePerLine - per seg

        public void setGuideLineColor(Color c)
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                for (int g = 0; g < seg.boundingbox.guideLines.Count; ++g)
                {
                    foreach (GuideLine line in seg.boundingbox.guideLines[g])
                    {
                        foreach (Stroke stroke in line.strokes)
                        {
                            stroke.strokeColor = c;
                        }
                    }
                }
            }
        }// setGuideLineColor

        public void setStrokeColor(Color c)
        {
            SegmentClass.PenColor = c;
            foreach (Stroke stroke in this.sketchStrokes)
            {
                stroke.strokeColor = c;
            }
        }

        public void setStrokeSketchyRate(double rate)
        {
            if (this.currSegmentClass == null) return;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                Box box = seg.boundingbox;
                foreach (GuideLine edge in box.edges)
                {
                    edge.strokeGap = rate;
                }
            }
            this.currSegmentClass.ChangeGuidelineStyle((int)SegmentClass.GuideLineStyle);
            this.calculateSketchMesh2d();
            this.setHiddenLines();
        }// setStrokeSketchyRate

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
        }//setStrokeSizePerSeg

        public void setGuideLineStyle(int idx)
        {
            switch (idx)
            {
                case 0:
                    SegmentClass.GuideLineStyle = SegmentClass.GuideLineType.Random;
                    break;
                case 1:
                default:
                    SegmentClass.GuideLineStyle = SegmentClass.GuideLineType.SimpleCross;
                    break;
            }    
            if (this.currSegmentClass != null)
            {
                this.currSegmentClass.ChangeGuidelineStyle(idx);
                this.calculateSketchMesh2d();
                this.setGuideLineColor(GLViewer.GuideLineColor);
            }
            this.updateDepthVal();
        }//setGuideLineStyle

        public void setStrokeStyle(int idx)
        {
            if (this.currSegmentClass != null)// && this.showLineOrMesh)
            {
                this.currSegmentClass.setStrokeStyle(idx);
                this.updateStrokeMesh();
                this.drawShadedOrTexturedStroke = this.currSegmentClass.shadedOrTexture();
            }

                foreach (Stroke stroke in this.sketchStrokes)
                {
                    stroke.changeStyle2d(idx);
                }
        }//setStrokeStyle

        public void setDepthType(int idx)
        {
            switch (idx)
            {
                case 0:
                    this.depthType = Depthtype.opacity;
                    break;
                case 1:
                    this.depthType = Depthtype.hidden;
                    break;
                case 2:
                    this.depthType = Depthtype.OpenGLDepthTest;
                    break;
                case 3:
                    this.depthType = Depthtype.none;
                    break;
                case 4:
                    this.depthType = Depthtype.rayTracing;
                    break;
                default:
                    break;
            }
            this.updateDepthVal();
        }// setDepthType

        public void setVanishingLineDrawType(int idx)
        {
            this.vanishinglineDrawType = idx;
        }// setDepthType

        public void setSegmentColor(Color c)
        {
            if (this.currSegmentClass == null) return;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                seg.color = c;
            }
        }

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
            this.activeSegment = this.currSegmentClass.segments[this.currBoxIdx];
            this.activeSegment.active = true;
            List<GuideLine> currLines = this.activeSegment.boundingbox.getAllLines();
            foreach (GuideLine line in currLines)
            {
                line.active = true;
                foreach (Stroke stroke in line.strokes)
                {
                    stroke.strokeColor = SegmentClass.StrokeColor;
                }
            }
        }// activateBoxAndGuideLines

        public bool showAllFaceToDraw = false;
        public void activateAllGuidelines()
        {
            this.activeSegment = this.currSegmentClass.segments[this.currBoxIdx];
            this.activeSegment.active = true;
            Box box = this.activeSegment.boundingbox;
            List<GuideLine> currLines = box.getAllLines();
            foreach (GuideLine line in currLines)
            {
                line.active = true;
            }
            // set random color to 1/2,. 1/3, 1/4 lines
            int cidx = 11;
            foreach (List<GuideLine> guideGroup in box.guideLines)
            {
                GuideLine line = guideGroup[guideGroup.Count - 1];
                foreach (Stroke stroke in line.strokes)
                {
                    stroke.strokeColor = ColorSet[cidx++];
                }
            }
        }//activateAllGuidelines

        public void deActivateAllGuidelines()
        {
            Segment activeSeg = this.currSegmentClass.segments[this.currBoxIdx];
            activeSeg.active = true;
            Box box = activeSeg.boundingbox;
            List<GuideLine> currLines = box.getAllLines();
            foreach (GuideLine line in currLines)
            {
                line.active = false;
            }
            foreach (List<GuideLine> guideGroup in box.guideLines)
            {
                GuideLine line = guideGroup[guideGroup.Count - 1];
                foreach (Stroke stroke in line.strokes)
                {
                    stroke.strokeColor = SegmentClass.HighlightColor;
                }
            }
        }//deActivateAllGuidelines

        public void AutoSequence()
        {
            for (int i = 0; i < this.nSequence; ++i)
            {
                this.nextSequence();
                this.Refresh();
                System.Threading.Thread.Sleep(100);
                if (i == 0)
                {
                    this.captureScreen(i);
                }
                else
                {
                    this.captureScreenCropped(i);
                }
            }
        }

        public string nextSequence()
        {
            this.inGuideMode = true;
            if (this.nSequence <= 0)
            {
                Program.GetFormMain().setPageNumber("0");
                return "0";
            }
            if (this.sequenceIdx == this.nSequence - 1)
            {
                this.resetAllBoxes();
                this.sequenceIdx++;
                Program.GetFormMain().setPageNumber(this.pageNumber[this.pageNumber.Count - 1]);
                return this.pageNumber[this.pageNumber.Count - 1];
            }
            else if (this.sequenceIdx == this.nSequence)
            {
                MessageBox.Show("This is the last page.");
                Program.GetFormMain().setPageNumber(this.pageNumber[this.pageNumber.Count - 1]);
                return this.pageNumber[this.pageNumber.Count - 1];
            }
            this.sequenceIdx++;
            if (this.sequenceIdx == 0)
            {
                this.clearBoxes();
                this.boxStack = new List<int>();
            }
            this.activateGuideSequence(true);
            this.lockView = true;
            Program.GetFormMain().setLockState(this.lockView);
            Program.GetFormMain().setPageNumber(this.pageNumber[this.boxStack.Count - 1]);
            return this.pageNumber[this.boxStack.Count - 1];
        }//nextSequence

        private List<int> boxStack;
        public string prevSequence()
        {
            this.inGuideMode = true;
            if (this.nSequence <= 0)
            {
                Program.GetFormMain().setPageNumber("0");
                return "0";
            }
            if (this.sequenceIdx <= 0)
            {
                MessageBox.Show("This is the first page.");
                Program.GetFormMain().setPageNumber("1");
                return this.pageNumber[0];
            }
            if (this.currBoxIdx >= 0)
            {
                this.activeSegment = this.currSegmentClass.segments[this.currBoxIdx];
                this.deActivateBoxAndGuideLines(this.activeSegment);
                this.boxStack.RemoveAt(this.boxStack.Count - 1);
            }
            this.sequenceIdx--;
            if (this.activeSegment != null)
            {
                this.activeSegment.drawn = false;
                this.activeSegment.drawn = this.checkBoxDrawnOrNot(this.currBoxIdx, 1);
                if (this.activeSegment.drawn)
                {
                    this.activeSegment.active = true;
                }
            }
            if (sequenceIdx < nSequence && this.boxStack.Count > 0 && this.boxStack[this.boxStack.Count - 1] != -1)
            {
                Segment prevSeg = this.currSegmentClass.segments[this.boxStack[this.boxStack.Count - 1]];
                prevSeg.drawn = this.checkBoxDrawnOrNot(prevSeg.idx, 2);
            }
            this.activateGuideSequence(false);
            this.lockView = true;
            Program.GetFormMain().setLockState(this.lockView);
            if (this.sequenceIdx == nSequence)
            {
                Program.GetFormMain().setPageNumber(this.pageNumber[this.pageNumber.Count - 1]);
                return this.pageNumber[this.pageNumber.Count - 1];
            }
            else
            {
                Program.GetFormMain().setPageNumber(this.pageNumber[this.boxStack.Count - 1]);
                return this.pageNumber[this.boxStack.Count - 1];
            }
        }//prevSequence

        public void redoSequence()
        {
            this.inGuideMode = true;
            this.sequenceIdx = -1;
            this.clearBoxes();
            this.boxStack = new List<int>();
            this.nextSequence();
            this.lockView = true;
            Program.GetFormMain().setLockState(this.lockView);
        }

        private bool checkBoxDrawnOrNot(int boxIdx, int back)
        {
            if(this.boxStack == null) return false;
            bool drawn = false;
            for (int i = this.boxStack.Count - back; i >= 0; --i)
            {
                if (this.boxStack[i] == boxIdx)
                {
                    drawn = true;
                    break;
                }
            }
            return drawn;
        }//checkBoxDrawnOrNot

        private void clearBoxes()
        {
            if (this.currSegmentClass == null) return;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                seg.active = false;
                seg.drawn = false;
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    line.active = false;
                }
                seg.boundingbox.activeFaceIndex = -1;
                seg.boundingbox.highlightFaceIndex = -1;
            }
        }//clearBoxes

        private void resetAllBoxes()
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                seg.active = true;
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in allLines)
                {
                    line.active = true;
                }
                this.setStrokeSizePerSeg(SegmentClass.StrokeSize, seg, SegmentClass.StrokeColor, GuideLineColor);
            }
            this.activeSegment = null;
            this.currBoxIdx = -1;
        }

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
                this.setStrokeSizePerSeg(SegmentClass.StrokeSize, seg, SegmentClass.StrokeColor, GuideLineColor);
            }
            this.currBoxIdx = -1;
            this.sequenceIdx = -1;
            this.activeSegment = null;
            this.boxStack = new List<int>();
            if (this.depthType == Depthtype.hidden)
            {
                this.updateDepthVal();
            }
        }

        public void twoBoxExample()
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                seg.active = true;
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    line.active = true;
                    foreach (Stroke stroke in line.strokes)
                    {
                        stroke.strokeColor = SegmentClass.StrokeColor;
                    }
                }
            }
            GuideLine edge = this.currSegmentClass.segments[1].boundingbox.edges[3];
            foreach (Stroke stroke in edge.strokes)
            {
                stroke.strokeColor = SegmentClass.HighlightColor;
            }
            edge = this.currSegmentClass.segments[1].boundingbox.edges[11];
            foreach (Stroke stroke in edge.strokes)
            {
                stroke.strokeColor = SegmentClass.HighlightColor;
            }
        }//twoBoxExample

        public void setSpecificFace()
        {
            int[] eids = { 2, 6, 7, 10 };
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    line.active = false;
                }
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                for (int i = 0; i < eids.Length; ++i)
                {
                    seg.boundingbox.edges[eids[i]].active = true;
                    foreach (Stroke stroke in seg.boundingbox.edges[eids[i]].strokes)
                    {
                        stroke.strokeColor = SegmentClass.StrokeColor;
                    }
                }
            }
        }//setSpecificFace

        // 3 box example
        private List<GuideLine> testLines;
        public int drawBoxIdx = 0;
        public void activateSpecificEdges()
        {
            int[] eids = { 2, 6, 7, 10 };
            
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                seg.active = true;
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    line.active = false;
                }
            }
            if (this.drawBoxIdx == 1)
            {
                this.currSegmentClass.segments[2].active = false;
            }
            
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                for (int i = 0; i < eids.Length; ++i)
                {
                    seg.boundingbox.edges[eids[i]].active = true;
                    foreach (Stroke stroke in seg.boundingbox.edges[eids[i]].strokes)
                    {
                        stroke.strokeColor = SegmentClass.StrokeColor;
                    }
                }
                foreach (List<GuideLine> lines in seg.boundingbox.guideLines)
                {
                    foreach (GuideLine line in lines)
                    {
                        if (!this.inGuideMode && !line.isGuide)
                        {
                            line.active = false;
                        }
                        foreach (Stroke stroke in line.strokes)
                        {
                            if (!line.isGuide)
                            {
                                stroke.strokeColor = SegmentClass.HiddenColor;
                            }
                            else
                            {
                                stroke.strokeColor = SegmentClass.HighlightColor;
                            }
                        }
                    }
                }
            }
            ++this.drawBoxIdx;
            if (this.currBoxIdx != -1 && this.currGroupIdx != -1)
            {
                Box box = this.currSegmentClass.segments[currBoxIdx].boundingbox;
                List<GuideLine> lines = box.guideLines[this.currGroupIdx];
                foreach (GuideLine line in lines)
                {
                    line.active = true;
                }
            }
            //this.testLines = new List<GuideLine>();
            //GuideLine ln = new GuideLine(new Vector3d(-1.4, -0.5, 0.5), new Vector3d(1.4, -0.5, 0.5),
            //    this.currSegmentClass.segments[0].boundingbox.planes[4], false);
            //this.testLines.Add(ln);
            //ln = new GuideLine(new Vector3d(-1.4, 0.5, 0.5), new Vector3d(1.4, 0.5, 0.5),
            //    this.currSegmentClass.segments[0].boundingbox.planes[4], false);
            //this.testLines.Add(ln);
            //ln = new GuideLine(new Vector3d(0, -0.17, 0.5), new Vector3d(1.4, -0.17, 0.5),
            //    this.currSegmentClass.segments[0].boundingbox.planes[4], false);
            //this.testLines.Add(ln);
            //ln = new GuideLine(new Vector3d(0, -0.83, 0.5), new Vector3d(1.4, -0.83, 0.5),
            //    this.currSegmentClass.segments[0].boundingbox.planes[4], false);
            //this.testLines.Add(ln);
            this.showVanishingPoints = false;
        }//activateSpecificEdges

        public void moveBox()
        {
            int[] bids = { 1, 2 };
            double offset = 0.1;
            for (int b = 0; b < bids.Length; ++b)
            {
                double off = offset;
                if (b == 1)
                {
                    off = -0.07;
                };
                Segment seg = this.currSegmentClass.segments[bids[b]];
                for (int i = 0; i < seg.boundingbox.points.Length; ++i)
                {
                    seg.boundingbox.points[i].y += off;
                }
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        stroke.u3.y += off;
                        stroke.v3.y += off;
                        for (int i = 0; i < stroke.meshVertices3d.Count; ++i)
                        {
                            stroke.meshVertices3d[i].y += off;
                        }
                        for (int i = 0; i < stroke.strokePoints.Count; ++i)
                        {
                            stroke.strokePoints[i].pos3.y += off;
                        }
                    }
                    line.u.y += off;
                    line.v.y += off;
                }
            }
            this.cal2D();
        }

        private void resetHighlightVars()
        {
            this.sequenceIdx = -1;
            this.currBoxIdx = -1;
            this.activeSegment = null;
            this.nSequence = 0;
        }

        private int currGroupIdx = -1;
        private bool showOnlyGuides = false;
        private bool showArrows = false;
        private void activateGuideSequence(bool next)
        {        
            //this.showFaceToDraw = false;
            // parse sequence
            List<int> guideLinesIds;
            int guideGroupIndex;
            int drawFaceIndex;
            int highlightFaceIndex = -1;
            this.showBlinking = false;
            this.showOnlyGuides = false;
            bool lastShowArrow = this.showArrows;
            this.showArrows = false;
            List<int> prevGuideGroupIds = new List<int>();
            this.currSegmentClass.parseASequence(this.sequenceIdx, out this.currBoxIdx, out guideGroupIndex, out guideLinesIds,
                out this.nextBoxIdx, out highlightFaceIndex, out drawFaceIndex, out this.showBlinking, out this.showOnlyGuides,
                out this.showArrows, out prevGuideGroupIds);

            this.activateDrawnBoxes();
            this.drawPrevBoxGuideLines = false;
            if (guideLinesIds.Count == 0 && (highlightFaceIndex == -1 && !lastShowArrow) && this.sequenceIdx > 0)
            {
                this.drawPrevBoxGuideLines = true; // draw previou guide lines
            }

            if (this.showOnlyGuides)
            {
                this.drawPrevBoxGuideLines = true;
                this.currSegmentClass.segments[this.currBoxIdx].drawn = false;
                if (next)
                {
                    this.boxStack.Add(-1);
                }
                return;
            }

            this.readyForGuideArrow = false;
            this.activeSegment = this.currSegmentClass.segments[this.currBoxIdx];
            
            // active box
            this.activateBox(this.activeSegment);
            // active guide lines
            if (this.depthType == Depthtype.hidden)
            {
                this.updateDepthVal();
            }

            this.showAnimatedGuideLines(this.activeSegment, guideGroupIndex, guideLinesIds, highlightFaceIndex, drawFaceIndex, prevGuideGroupIds);
            this.readyForGuideArrow = true;
            
            this.currGroupIdx = guideGroupIndex;

            if (!this.showArrows)
            {
                this.activeSegment.drawn = true;
            }
            if (next)
            {
                if (!this.showArrows)
                {
                    this.boxStack.Add(this.currBoxIdx);
                }
                else
                {
                    this.boxStack.Add(-1);
                }
            }
        }// activateGuideSequence

        private void activateDrawnBoxes()
        {
            /////!!!this code dies the program
            //if (this.enableHiddencheck)
            //{
            //    this.setHiddenLines(); // for the current active box
            //}
            // fade out the boxes that have already been drawn
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                this.setStrokeSizePerSeg(SegmentClass.StrokeSize / 2, seg, SegmentClass.HiddenColor, GuideLineColor);
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    line.active = true;
                    line.strokeGap *= 0.6;
                    if (SegmentClass.GuideLineStyle == SegmentClass.GuideLineType.Random)
                    {
                        line.DefineRandomStrokes();
                    }
                    else
                    {
                        line.DefineCrossStrokes();
                    }
                    foreach (Stroke stroke in line.strokes)
                    {
                        stroke.strokeColor = SegmentClass.HiddenColor;
                        stroke.weight = SegmentClass.StrokeSize / 2;
                    }
                }
                for (int g = 0; g < seg.boundingbox.guideLines.Count; ++g)
                {
                    foreach (GuideLine line in seg.boundingbox.guideLines[g])
                    {
                        line.active = false;
                    }
                }
                seg.boundingbox.activeFaceIndex = -1;
                if (seg.boundingbox.arrows != null)
                {
                    foreach (Arrow3D arrow in seg.boundingbox.arrows)
                    {
                        arrow.active = false;
                    }
                }
            }
            this.calculateSketchMesh2d();
        }// activateDrawnBox

        private void activateBox(Segment activeSeg)
        {
            activeSeg.active = true;
            if (!activeSeg.drawn)
            {
                foreach (GuideLine line in activeSeg.boundingbox.edges)
                {
                    line.active = true;
                }
                this.setStrokeSizePerSeg(SegmentClass.StrokeSize, activeSeg, SegmentClass.StrokeColor, GuideLineColor);
            }
            this.setHiddenLines(activeSeg);
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
            if (activeSeg.boundingbox.arrows != null)
            {
                foreach (Arrow3D arrow in activeSeg.boundingbox.arrows)
                {
                    arrow.active = false;
                }
            }
            activeSeg.boundingbox.activeFaceIndex = -1;
            activeSeg.boundingbox.highlightFaceIndex = -1;
        }// deActivateBoxAndGuideLines

        private void setGuideLineTypeColor(GuideLine line){
            switch (line.type)
            {
                case 1:
                    line.color = GuideLineColor;
                    break;
                case 2:
                    line.color = SegmentClass.OneHafColor;
                    break;
                case 3:
                    line.color = SegmentClass.OneThirdColor;
                    break;
                case 4:
                    line.color = SegmentClass.OnequarterColor;
                    break;
                case 6:
                    line.color = SegmentClass.OneSixthColor;
                    break;
                case 0:
                default:
                    line.color = SegmentClass.ReflectionColor;
                    break;

            }
            foreach (Stroke stroke in line.strokes)
            {
                stroke.strokeColor = line.color;
            }
        }

        public int sleepTime = 200;
        private void showAnimatedGuideLines(Segment seg, int guideGroupIndex, List<int> guideLinesIds, int highlightFaceIndex, 
            int drawFaceIndex, List<int> prevGuideGroupIds)
        {
            Box box = seg.boundingbox;
            // draw previous guidelines
            if (guideLinesIds.Count > 0)
            {
                // fixe to the format
                for (int i = 0; i < guideLinesIds[0]; ++i)
                {
                    GuideLine line = box.guideLines[guideGroupIndex][i];
                    //line.active = true;                    
                    if (line.isGuide)
                    {
                        line.active = true;
                        this.setStrokeStylePerLine(line, SegmentClass.GuideLineSize, SegmentClass.StrokeColor);
                        //this.setGuideLineTypeColor(line);
                        //this.setStrokeStylePerLine(line, (int)(SegmentClass.GuideLineSize * 0.8), SegmentClass.HiddenHighlightcolor);
                    }
                    else
                    {
                        line.active = false;
                        //this.setStrokeStylePerLine(line, SegmentClass.GuideLineSize / 2, SegmentClass.HiddenGuideLinecolor);
                    }
                }
            }
                for (int i = 0; i < prevGuideGroupIds.Count; ++i)
                {
                    foreach (GuideLine line in box.guideLines[prevGuideGroupIds[i]])
                    {
                        if (line.isGuide)
                        {
                            line.active = true;
                            this.setStrokeStylePerLine(line, SegmentClass.GuideLineSize, SegmentClass.StrokeColor);
                        }
                    }
                }
            if (this.enableDepthTest)
                Gl.glDisable(Gl.GL_DEPTH_TEST);
            
            // 1. highligh the hosting face 
            //if (this.showBlinking && highlightFaceIndex != -1)
            //{
            //    int ntimes = 10;
            //    int it = 0;
            //    while (it++ < ntimes)
            //    {
            //        if (it % 2 == 0)
            //        {
            //            this.drawQuad3d(box.facesToHighlight[highlightFaceIndex], SegmentClass.FaceColor);
            //            this.drawQuadEdge3d(box.facesToHighlight[highlightFaceIndex], SegmentClass.StrokeColor);
            //            box.highlightFaceIndex = highlightFaceIndex;
            //        }
            //        else
            //        {
            //            box.highlightFaceIndex = -1;
            //        }
            //        this.Refresh();
            //        System.Threading.Thread.Sleep(100);
            //    }
            //}
            this.showBlinking = false;
            box.highlightFaceIndex = highlightFaceIndex;
            
            // 2. draw guidelines
            // 3. show the target guide line (previous guidelines are computed for it)
            for (int i = 0; i < guideLinesIds.Count; ++i)
            {
                int idx = guideLinesIds[i];
                GuideLine line = box.guideLines[guideGroupIndex][idx];
                line.active = true; 
                if (line.isGuide)
                {
                    this.setStrokeStylePerLine(line, SegmentClass.GuideLineSize, SegmentClass.GuideLineWithTypeColor);
                    //this.setGuideLineTypeColor(line);
                }
                else
                {
                    this.setStrokeStylePerLine(line, SegmentClass.GuideLineSize, GuideLineColor);
                }
                //this.Refresh();
                //System.Threading.Thread.Sleep(this.sleepTime);
                if (line.isGuide)
                {
                    // deactivate previous lines
                    for (int j = 0; j <= i; ++j)
                    {
                        GuideLine jline = box.guideLines[guideGroupIndex][guideLinesIds[j]];
                        if (jline.isGuide)
                        {
                            //this.setStrokeStylePerLine(jline, (int)(SegmentClass.GuideLineSize), SegmentClass.StrokeColor);
                            //this.setGuideLineTypeColor(jline);
                            //this.setStrokeStylePerLine(jline, (int)(SegmentClass.GuideLineSize * 0.8), SegmentClass.HiddenColor);
                        }
                        else
                        {
                            //jline.active = false;
                            this.setStrokeStylePerLine(jline, SegmentClass.GuideLineSize * 0.6, SegmentClass.HiddenGuideLinecolor);
                            //this.setGuideLineTypeColor(jline);
                        }
                    }
                    //this.Refresh();
                }
                this.setStrokeStylePerLine(line, SegmentClass.GuideLineSize * 0.6, SegmentClass.HiddenGuideLinecolor);
            }
            // the last one is now the current one to highlight
            if (guideLinesIds.Count > 0)
            {
                GuideLine last = box.guideLines[guideGroupIndex][guideLinesIds[guideLinesIds.Count - 1]];
                if (last.isGuide)
                {
                    this.setStrokeStylePerLine(last, SegmentClass.GuideLineSize, SegmentClass.GuideLineWithTypeColor);
                }
                else
                {
                    this.setStrokeStylePerLine(last, SegmentClass.GuideLineSize, GuideLineColor);
                }
            }
            // draw the arrows
            this.Refresh();
            if (this.showArrows && box.arrows != null)
            {
                foreach (Arrow3D arrow in box.arrows)
                {
                    arrow.active = true;
                    this.Refresh();
                    System.Threading.Thread.Sleep(this.sleepTime);
                }
            }

            this.animatedLine = null;
            box.activeFaceIndex =  drawFaceIndex;
            //box.highlightFaceIndex = -1;
            //box.highlightFaceIndex = highlightFaceIndex;
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

        public void calculateSketchMesh2d()
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
                        stroke.hostPlane = edge.hostPlane;
                    }
                }
            }

            //Matrix4d m = this.camera.GetProjMat() * this.camera.GetModelviewMat() * this.camera.GetBallMat();
            //m = m.Transpose();
            //m = this.modelTransformMatrix;
            this.currSegmentClass.ChangeStrokeStyle(this.modelTransformMatrix, camera);

        }// calculateSketchMesh2d

        private void calculate2D()
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
                        this.projectStrokePointsTo2d(stroke);
                    }
                }
            }
        }

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

        public void writeModelViewMatrix(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    sw.Write(this.currModelTransformMatrix[i, j].ToString() + " ");
                }
            }
            sw.Close();
        }

        public void readModelModelViewMatrix(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            char[] separator = { ' '};
            string s = sr.ReadLine();
            s.Trim();
            string[] strs = s.Split(separator);
            double[] arr = new double[strs.Length];
            for (int i = 0; i < arr.Length; ++i)
            {
                if (strs[i] == "") continue;
                arr[i] = double.Parse(strs[i]);
            }
            this.currModelTransformMatrix = new Matrix4d(arr);
            this.fixedModelView = new Matrix4d(arr);
        }


        private void captureScreen(int idx)
        {
            Size newSize = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Size.Width - 500, Screen.PrimaryScreen.Bounds.Size.Height - 150);
            var bmp = new Bitmap(newSize.Width, newSize.Height);
            var gfx = Graphics.FromImage(bmp);
            gfx.CopyFromScreen((int)(this.paperPos[0].pos2.x + this.Location.X - 50), Screen.PrimaryScreen.Bounds.Y + 100, 0, 0, newSize, CopyPixelOperation.SourceCopy);
            string imageFolder = foldername + "\\screenCapture";
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            string name = imageFolder + "\\seq_" + idx.ToString() + ".png";
            bmp.Save(name, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void captureScreenCropped(int idx)
        {
            double w = 297, h = 210;
            int wid = 800;
            int heig = (int)(h/w * wid);
            Size newSize = new System.Drawing.Size(wid, heig);
            var bmp = new Bitmap(newSize.Width, newSize.Height);
            var gfx = Graphics.FromImage(bmp);
            gfx.CopyFromScreen((int)(this.paperPos[0].pos2.x + 300), Screen.PrimaryScreen.Bounds.Y + 200, 0, 0, newSize, CopyPixelOperation.SourceCopy);
            string imageFolder = foldername + "\\screenCapture";
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            string name = imageFolder + "\\seq_" + idx.ToString() + ".png";
            bmp.Save(name, System.Drawing.Imaging.ImageFormat.Png);
        }

        public void computeContours()
        {
            //return;
            this.silhouettePoints = new List<Vector3d>();
            this.contourPoints = new List<Vector3d>();
            this.contourLines = new List<List<Vector3d>>();
            this.apparentRidgePoints = new List<Vector3d>();
            this.suggestiveContourPoints = new List<Vector3d>();
            this.boundaryPoints = new List<Vector3d>();

            if (this.currSegmentClass != null)
            {
                if (!this.isShowContour()) return;
                Matrix4d T = this.currModelTransformMatrix;
                Matrix4d Tv = T.Inverse();
                foreach (Segment seg in this.currSegmentClass.segments)
                {

                        foreach (GuideLine line in seg.boundingbox.edges)
                        {
                            foreach (Stroke stroke in line.strokes)
                            {
                                stroke.strokeColor = SegmentClass.HiddenColor;
                            }
                        }
                    
                    if (!seg.active) continue;
                    seg.updateVertex(T);
                    if(this.showSegSilhouette)
                    {
                        seg.computeSihouette(Tv, this.eye);
                    }
                    if (this.showSegContour)
                    {
                        seg.computeContour(Tv, this.eye);
                    }
                    if (this.showSegSuggestiveContour)
                    {
                        seg.computeSuggestiveContour(Tv, this.eye);
                    }
                    if (this.showSegApparentRidge)
                    {
                        seg.computeApparentRidge(Tv, this.eye);
                    }
                    if (this.showSegBoundary)
                    {
                        seg.computeBoundary(Tv, this.eye);
                    }
                }
            }
            if (this.currMeshClass != null && this.currMeshClass.TriMesh != null)
            {
                if (!this.isShowContour()) return;
                
                Mesh mesh = this.currMeshClass.Mesh;
                double[] vertexPos = new double[mesh.VertexPos.Length];
                List<int> vidxs = new List<int>();
                for (int i = 0, j = 0; i < mesh.VertexCount; ++i, j += 3)
                {
                    Vector3d v0 = new Vector3d(mesh.VertexPos[j],
                        mesh.VertexPos[j + 1],
                        mesh.VertexPos[j + 2]);
                    Vector3d v1 = (this.currModelTransformMatrix * new Vector4d(v0, 1)).ToVector3D();
                    vertexPos[j] = v1.x;
                    vertexPos[j + 1] = v1.y;
                    vertexPos[j + 2] = v1.z;
                }
                this.silhouettePoints.Clear();
                this.contourPoints.Clear();
                this.suggestiveContourPoints.Clear();
                this.apparentRidgePoints.Clear();
                
                if (this.showSegSilhouette)
                {
                    this.silhouettePoints = this.currMeshClass.computeContour(vertexPos, this.eye, 1);
                    for (int i = 0; i < this.silhouettePoints.Count; ++i)
                    {
                        Vector3d v = this.silhouettePoints[i];
                        Vector3d vt = (this.currModelTransformMatrix.Inverse() * new Vector4d(v, 1)).ToVector3D();
                        this.silhouettePoints[i] = vt;
                    }
                }
                if (this.showSegContour)
                {
                    this.contourPoints = this.currMeshClass.computeContour(vertexPos, this.eye, 2);
                    for (int i = 0; i < this.contourPoints.Count; ++i)
                    {
                        Vector3d v = this.contourPoints[i];
                        Vector3d vt = (this.currModelTransformMatrix.Inverse() * new Vector4d(v, 1)).ToVector3D();
                        this.contourPoints[i] = vt;
                    }
                }
                if (this.showSegSuggestiveContour)
                {
                    this.suggestiveContourPoints = this.currMeshClass.computeContour(vertexPos, this.eye, 3);
                    for (int i = 0; i < this.suggestiveContourPoints.Count; ++i)
                    {
                        Vector3d v = this.suggestiveContourPoints[i];
                        Vector3d vt = (this.currModelTransformMatrix.Inverse() * new Vector4d(v, 1)).ToVector3D();
                        this.suggestiveContourPoints[i] = vt;
                    }
                }
                if (this.showSegApparentRidge)
                {
                    this.apparentRidgePoints = this.currMeshClass.computeContour(vertexPos, this.eye, 4);
                    for (int i = 0; i < this.apparentRidgePoints.Count; ++i)
                    {
                        Vector3d v = this.apparentRidgePoints[i];
                        Vector3d vt = (this.currModelTransformMatrix.Inverse() * new Vector4d(v, 1)).ToVector3D();
                        this.apparentRidgePoints[i] = vt;
                    }
                }
                if (this.showSegBoundary)
                {
                    this.boundaryPoints = this.currMeshClass.computeContour(vertexPos, this.eye, 5);
                    for (int i = 0; i < this.boundaryPoints.Count; ++i)
                    {
                        Vector3d v = this.boundaryPoints[i];
                        Vector3d vt = (this.currModelTransformMatrix.Inverse() * new Vector4d(v, 1)).ToVector3D();
                        this.boundaryPoints[i] = vt;
                    }
                }
              
            }
            return;

            #region
            //// my implementation
            if (this.currSegmentClass == null && this.meshClasses == null) return;
            this.contourPoints = new List<Vector3d>();
            this.sharpEdges = new List<Vector3d>();
            //this.currSegmentClass.calculateContourPoint(this.currModelTransformMatrix, this.eye);
            if (this.currSegmentClass != null)
            {
                this.computeContourEdges();
            }
            else if(this.currMeshClass != null)
            {
                List<int> vidxs = this.computeMeshContour(this.currMeshClass.Mesh);
                //this.regionGrowingContours(this.currMeshClass.Mesh, vidxs);
                //this.computeContourByVisibility(this.currMeshClass.Mesh);
                this.computeMeshSharpEdge(this.currMeshClass.Mesh);
            }
            if (this.contourPoints.Count > 0)
            {
                this.drawEdge = false;
            }
            #endregion
        }//computeContours

        private List<Vector3d> contourPoints = new List<Vector3d>(), 
            silhouettePoints = new List<Vector3d>(), 
            apparentRidgePoints = new List<Vector3d>(),
            suggestiveContourPoints = new List<Vector3d>(),
            boundaryPoints = new List<Vector3d>();
        private List<Vector3d> sharpEdges;
        private List<List<Vector3d>> contourLines;

        public void computeContourEdges()
        {
            // current pos, normal
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (seg.mesh == null) continue;
                List<int> vidxs = this.computeMeshContour(seg.mesh);
                //seg.regionGrowingContours(vidxs);
                this.computeMeshSharpEdge(seg.mesh);
            }// fore each segment
        }//computeContourEdges

        #region
        // contour tests
        public void regionGrowingContours(Mesh m, List<int> labeled)
        {
            int ndist = 10;
            this.contourLines = new List<List<Vector3d>>();
            while (labeled.Count > 0)
            {
                int i = labeled[0];
                labeled.RemoveAt(0);
                if (!m.Flags[i])
                {
                    continue;
                }
                m.Flags[i] = false;
                List<int> vids = new List<int>();
                List<int> queue = new List<int>();
                vids.Add(i);
                queue.Add(i);
                int s = 0;
                int d = 0;
                while (s < queue.Count && d < ndist)
                {
                    int j = queue[s];
                    for (int k = 0; k < m.VertexFaceIndex.GetLength(1); ++k)
                    {
                        int f = m.VertexFaceIndex[j, k];
                        if (f == -1) continue;
                        for (int fi = 0; fi < 3; ++fi)
                        {
                            int kv = m.FaceVertexIndex[f * 3 + fi];
                            if (m.Flags[kv])
                            {
                                vids.Add(kv);
                                m.Flags[kv] = false;
                                d = 0;
                            }
                            if (!queue.Contains(kv))
                            {
                                queue.Add(kv);
                            }
                        }
                        ++s;
                        ++d;
                    }
                }
                if (vids.Count > 4)
                {
                    List<Vector3d> c = new List<Vector3d>();
                    foreach (int v in vids)
                    {
                        c.Add(m.getVertexPos(v));
                    }
                    this.contourLines.Add(c);
                }
            }
        }//regionGrowingContours
       
        private void computeContourByVisibility(Mesh m)
        {
            this.clearScene();
            int n = m.FaceCount;
            int[] queryIDs = new int[n];
            Gl.glGenQueries(n, queryIDs);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            this.setViewMatrix();
            for (int i = 0, j = 0; i < m.FaceCount; ++i, j += 3)
            {
                int vidx1 = m.FaceVertexIndex[j];
                int vidx2 = m.FaceVertexIndex[j + 1];
                int vidx3 = m.FaceVertexIndex[j + 2];
                Vector3d v1 = new Vector3d(
                    m.VertexPos[vidx1 * 3], m.VertexPos[vidx1 * 3 + 1], m.VertexPos[vidx1 * 3 + 2]);
                Vector3d v2 = new Vector3d(
                    m.VertexPos[vidx2 * 3], m.VertexPos[vidx2 * 3 + 1], m.VertexPos[vidx2 * 3 + 2]);
                Vector3d v3 = new Vector3d(
                    m.VertexPos[vidx3 * 3], m.VertexPos[vidx3 * 3 + 1], m.VertexPos[vidx3 * 3 + 2]);
                Gl.glBeginQuery(Gl.GL_SAMPLES_PASSED, queryIDs[i]);
                Gl.glColor3ub(0, 0, 0);
                Gl.glBegin(Gl.GL_TRIANGLES);
                Gl.glVertex3d(v1.x, v1.y, v1.z);
                Gl.glVertex3d(v2.x, v2.y, v2.z);
                Gl.glVertex3d(v3.x, v3.y, v3.z);
                Gl.glEnd();
                Gl.glEndQuery(Gl.GL_SAMPLES_PASSED);
            }
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();

            // get # passed samples
            int[] faceVis = new int[n];
            int sum = 0;
            for (int i = 0; i < n; ++i)
            {
                int queryReady = Gl.GL_FALSE;
                int count = 1000;
                while (queryReady != Gl.GL_TRUE && count-- > 0)
                {
                    Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                }
                if (queryReady == Gl.GL_FALSE)
                {
                    count = 1000;
                    while (queryReady != Gl.GL_TRUE && count-- > 0)
                    {
                        Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                    }
                }
                Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT, out faceVis[i]);
                if (faceVis[i] > 2)
                {
                    sum += 0;
                }
                sum += faceVis[i];
            }
            Gl.glDeleteQueries(n, queryIDs);

            foreach (HalfEdge edge in m.HalfEdges)
            {
                if (edge.invHalfEdge == null) // boudnary
                {
                    //this.sharpEdges.Add(m.getVertexPos(edge.FromIndex));
                    //this.sharpEdges.Add(m.getVertexPos(edge.ToIndex));
                    continue;
                }
                if (edge.invHalfEdge.index < edge.index)
                {
                    continue; // checked
                }
                int fidx = edge.FaceIndex;
                int invfidx = edge.invHalfEdge.FaceIndex;
                if ((faceVis[fidx] < 3 && faceVis[invfidx] >= 3) ||
                    (faceVis[invfidx] < 3 && faceVis[fidx] >= 3))
                {
                    this.contourPoints.Add(m.getVertexPos(edge.FromIndex));
                    this.contourPoints.Add(m.getVertexPos(edge.ToIndex));
                }
            }

        }//computeContourByVisibility
        #endregion

        private List<int> computeMeshContour(Mesh mesh)
        {
            double thresh = 0.1;
            double[] vertexPos = new double[mesh.VertexPos.Length];
            List<int> vidxs = new List<int>();
            for (int i = 0, j = 0; i < mesh.VertexCount; ++i, j += 3)
            {
                Vector3d v0 = new Vector3d(mesh.VertexPos[j],
                    mesh.VertexPos[j + 1],
                    mesh.VertexPos[j + 2]);
                Vector3d v1 = (this.currModelTransformMatrix * new Vector4d(v0, 1)).ToVector3D();
                vertexPos[j] = v1.x;
                vertexPos[j + 1] = v1.y;
                vertexPos[j + 2] = v1.z;
            }

            // transformed mesh
            Mesh m = new Mesh(mesh, vertexPos);
            //for (int i = 0, j = 0; i < m.VertexCount; ++i, j += 3)
            //{
            //    Vector3d v0 = m.getVertexPos(i);
            //    Vector3d vn = new Vector3d(m.VertexNormal[j],
            //        m.VertexNormal[j + 1],
            //        m.VertexNormal[j + 2]).normalize();
            //    Vector3d v = (eye - v0).normalize();
            //    double cosv = v.Dot(vn);
            //    if (Math.Abs(cosv) < thresh)// && cosv > 0)
            //    {
            //        mesh.Flags[i] = true;
            //        vidxs.Add(i);
            //        this.contourPoints.Add(mesh.getVertexPos(i));
            //    }
            //}

            #region
            // check the sign change of each edge
            foreach (HalfEdge edge in m.HalfEdges)
            {
                if (edge.invHalfEdge == null) // boudnary
                {
                    //this.contourPoints.Add(mesh.getVertexPos(edge.FromIndex));
                    //this.contourPoints.Add(mesh.getVertexPos(edge.ToIndex));
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
                //Vector3d v1 = m.getVertexPos(edge.FromIndex);
                //Vector3d v2 = m.getVertexPos(edge.ToIndex);
                Vector3d e1 = (this.eye - v1).normalize();
                Vector3d e2 = (this.eye - v2).normalize();
                Vector3d n1 = m.getFaceNormal(fidx);
                Vector3d n2 = m.getFaceNormal(invfidx);
                double c1 = e1.Dot(n1);
                double c2 = e2.Dot(n2);
                //if (Math.Abs(c1) < thresh || Math.Abs(c2) < thresh)
                //{
                //    this.contourPoints.Add(mesh.getVertexPos(edge.FromIndex));
                //    this.contourPoints.Add(mesh.getVertexPos(edge.ToIndex));
                //}
                if (c1 * c2 <= 0)
                {
                    this.contourPoints.Add(mesh.getVertexPos(edge.FromIndex));
                    this.contourPoints.Add(mesh.getVertexPos(edge.ToIndex));
                    vidxs.Add(edge.FromIndex);
                    vidxs.Add(edge.ToIndex);
                    mesh.Flags[edge.FromIndex] = true;
                    mesh.Flags[edge.ToIndex] = true;
                }
            }

            #endregion

            return vidxs;
        }// computeMeshContour

        private void collectContourLines()
        {
            if (this.contourPoints == null) return;
            this.contourLines = new List<List<Vector3d>>();

        }

        private void computeMeshSharpEdge(Mesh mesh)
        {
            double thresh = 0.1;
            double[] vertexPos = new double[mesh.VertexPos.Length];
            for (int i = 0, j = 0; i < mesh.VertexCount; ++i, j += 3)
            {
                Vector3d v0 = new Vector3d(mesh.VertexPos[j],
                    mesh.VertexPos[j + 1],
                    mesh.VertexPos[j + 2]);
                Vector3d v1 = (this.currModelTransformMatrix * new Vector4d(v0, 1)).ToVector3D();
                vertexPos[j] = v1.x;
                vertexPos[j + 1] = v1.y;
                vertexPos[j + 2] = v1.z;
            }

            // transformed mesh
            Mesh m = new Mesh(mesh, vertexPos);
            // check the sign change of each edge
            foreach (HalfEdge edge in m.HalfEdges)
            {
                if (edge.invHalfEdge == null) // boudnary
                {
                    this.sharpEdges.Add(mesh.getVertexPos(edge.FromIndex));
                    this.sharpEdges.Add(mesh.getVertexPos(edge.ToIndex));
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
                Vector3d n1 = m.getFaceNormal(fidx);
                Vector3d n2 = m.getFaceNormal(invfidx);
                double c = n1.Dot(n2);
                if (Math.Abs(c) < thresh)
                {
                    this.sharpEdges.Add(mesh.getVertexPos(edge.FromIndex));
                    this.sharpEdges.Add(mesh.getVertexPos(edge.ToIndex));
                }
            }
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
            return (this.currModelTransformMatrix * new Vector4d(v, 1)).ToVector3D();
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
                //List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in seg.boundingbox.edges)
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
                //List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine line in seg.boundingbox.edges)
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

        private void setHiddenLines(Segment seg)
        {
            if (this.depthType == Depthtype.hidden) return;

            this.clearScene();
            // draw the whole sceen from "Draw3D()" to get the visibility info depth value
            int n = 0;
            int nsample = 10; // sample 10 points on each line

            foreach (GuideLine edge in seg.boundingbox.edges)
            {
                ++n;
            }
            // draw and get visibility
            int[] queryIDs = new int[n];
            Gl.glGenQueries(n, queryIDs);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            int idx = 0;
            this.setViewMatrix();

            this.drawBoundingbox(seg.boundingbox, Color.White);

            foreach (GuideLine edge in seg.boundingbox.edges)
            {
                Vector3d dir = (edge.v - edge.u).normalize();
                double dist = (edge.v - edge.u).Length() / nsample;
                Gl.glBeginQuery(Gl.GL_SAMPLES_PASSED, queryIDs[idx++]);
                Gl.glColor3ub(0, 0, 0);
                Gl.glBegin(Gl.GL_LINES);
                for (int i = 0; i < nsample - 1; ++i)
                {
                    Vector3d v1 = edge.u + i * dist * dir;
                    Vector3d v2 = edge.u + (i + 1) * dist * dir;
                    Gl.glVertex3dv(v1.ToArray());
                    Gl.glVertex3dv(v2.ToArray());
                }
                Gl.glEnd();
                Gl.glEndQuery(Gl.GL_SAMPLES_PASSED);
            }

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
            //this.SwapBuffers();

            // get # passed samples
            int[] visPnts = new int[n];
            int maxv = 0;
            for (int i = 0; i < n; ++i)
            {
                int queryReady = Gl.GL_FALSE;
                int count = 1000;
                while (queryReady != Gl.GL_TRUE && count-- > 0)
                {
                    Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                }
                if (queryReady == Gl.GL_FALSE)
                {
                    count = 1000;
                    while (queryReady != Gl.GL_TRUE && count-- > 0)
                    {
                        Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                    }
                }
                Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT, out visPnts[i]);
                if (visPnts[i] > maxv)
                {
                    maxv = visPnts[i];
                }
            }
            Gl.glDeleteQueries(n, queryIDs);

            idx = 0;

            foreach (GuideLine edge in seg.boundingbox.edges)
            {
                if (visPnts[idx++] < 10)
                {
                    this.setStrokeStylePerLine(edge, (double)SegmentClass.StrokeSize / 2, SegmentClass.HiddenLineColor);
                    // the hidden color might get modified
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.strokeColor = SegmentClass.HiddenColor;
                    }
                    
                }
                //else
                //{
                //    this.setStrokeStylePerLine(edge, (double)SegmentClass.StrokeSize, SegmentClass.StrokeColor);
                //}
            }

        }// setHiddenLines

        private void setHiddenLines()
        {
            if (this.currSegmentClass == null || this.depthType == Depthtype.hidden || this.inGuideMode) 
                return;

            this.clearScene();
            // draw the whole sceen from "Draw3D()" to get the visibility info depth value
            int n = 0;
            int nsample = 10; // sample 10 points on each line
            // drawAllActiveBoxes() or drawSketchyEdges3D_hiddenLine()
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    n += nsample;
                }
            }
            // draw and get visibility
            int[] queryIDs = new int[n];
            Gl.glGenQueries(n, queryIDs);
            //for (int i = 0; i < n; ++i)
            //{
            //    queryIDs[i] = i;
            //}
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            int idx = 0;
            this.setViewMatrix();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                this.drawBoundingbox(seg.boundingbox, Color.White);
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    Vector3d dir = (edge.v - edge.u).normalize();
                    double dist = (edge.v-edge.u).Length()/nsample;
                    Gl.glBeginQuery(Gl.GL_SAMPLES_PASSED, queryIDs[idx++]);
                    Gl.glColor3ub(0, 0, 0);
                    Gl.glBegin(Gl.GL_POINTS);
                    for (int i = 0; i < nsample; ++i)
                    {
                        Vector3d v = edge.u + i * dist * dir;
                        Gl.glVertex3dv(v.ToArray());
                    }
                    Gl.glEnd();
                    Gl.glEndQuery(Gl.GL_SAMPLES_PASSED);
                }
            }
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
            //this.SwapBuffers();

            // get # passed samples
            int[] visPnts = new int[n];
            int maxv = 0;
            for (int i = 0; i < n; ++i)
            {
                int queryReady = Gl.GL_FALSE;
                int count = 1000;
                while (queryReady != Gl.GL_TRUE && count-- > 0)
                {
                    Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                }
                if (queryReady == Gl.GL_FALSE)
                {
                    count = 1000;
                    while (queryReady != Gl.GL_TRUE && count-- > 0)
                    {
                        Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                    }
                }
                Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT, out visPnts[i]);
                if (visPnts[i] > maxv)
                {
                    maxv = visPnts[i];
                }
            }
            Gl.glDeleteQueries(n, queryIDs);

            idx = 0;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    if (visPnts[idx++] < 10)
                    {
                        this.setStrokeStylePerLine(edge, SegmentClass.StrokeSize / 2, SegmentClass.HiddenLineColor);
                        foreach (Stroke stroke in edge.strokes)
                        {
                            stroke.strokeColor = SegmentClass.HiddenColor;
                        }
                    }
                    else
                    {
                        this.setStrokeStylePerLine(edge, SegmentClass.StrokeSize, SegmentClass.StrokeColor);
                    }
                }
            }
            //this.Refresh();
        }// setHiddenLines

        int[] visibleTriangleVertices;
        byte[] triangleDepth;
        private void goThroughVisibilityTest()
        {
            this.clearScene();
            // draw the whole sceen from "Draw3D()" to get the visibility info depth value
            int n = 0;
            // drawAllActiveBoxes() or drawSketchyEdges3D_hiddenLine()
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    int nstrokes = edge.strokes.Count;
                    if (this.inGuideMode)
                    {
                        nstrokes = nstrokes > 2 ? 2 : nstrokes;
                    }
                    for (int i = 0; i < nstrokes; ++i)
                    {
                        Stroke stroke = edge.strokes[i];
                        n += stroke.FaceCount;
                    }
                }
            }
            // draw and get visibility
            int[] queryIDs = new int[n];
            Gl.glGenQueries(n, queryIDs);
            //for (int i = 0; i < n; ++i)
            //{
            //    queryIDs[i] = i;
            //}
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            int idx = 0;
            this.setViewMatrix();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                this.drawBoundingbox(seg.boundingbox, Color.White);
            }
            List<Stroke> activeStrokes = new List<Stroke>();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    int nstrokes = edge.strokes.Count;
                    if (this.inGuideMode)
                    {
                        nstrokes = nstrokes > 2 ? 2 : nstrokes;
                    }
                    for (int k = 0; k < nstrokes; ++k)
                    {
                        Stroke stroke = edge.strokes[k];

                        for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
                        {
                            int vidx1 = stroke.faceIndex[j];
                            int vidx2 = stroke.faceIndex[j + 1];
                            int vidx3 = stroke.faceIndex[j + 2];
                            Gl.glBeginQuery(Gl.GL_SAMPLES_PASSED, queryIDs[idx++]);
                            Gl.glColor3ub(0, 0, 0);
                            Gl.glBegin(Gl.GL_POLYGON);
                            Gl.glVertex3dv(stroke.meshVertices3d[vidx1].ToArray());
                            Gl.glVertex3dv(stroke.meshVertices3d[vidx2].ToArray());
                            Gl.glVertex3dv(stroke.meshVertices3d[vidx3].ToArray());
                            Gl.glEnd();
                            Gl.glEndQuery(Gl.GL_SAMPLES_PASSED);
                        }
                        activeStrokes.Add(stroke);
                    }
                }
            }
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
            //this.SwapBuffers();

            // get # passed samples
            visibleTriangleVertices = new int[n];
            int maxv = 0;
            for (int i = 0; i < n; ++i)
            {
                int queryReady = Gl.GL_FALSE;
                int count = 1000;
                while (queryReady != Gl.GL_TRUE && count-- > 0)
                {
                    Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                }
                if (queryReady == Gl.GL_FALSE)
                {
                    count = 1000;
                    while (queryReady != Gl.GL_TRUE && count-- > 0)
                    {
                        Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT_AVAILABLE, out queryReady);
                    }
                }
                Gl.glGetQueryObjectiv(queryIDs[i], Gl.GL_QUERY_RESULT, out visibleTriangleVertices[i]);
                if (this.visibleTriangleVertices[i] > maxv)
                {
                    maxv = visibleTriangleVertices[i];
                }                
            }
            Gl.glDeleteQueries(n, queryIDs);
            //this.SwapBuffers();
            // smooth samples
            int nloop = 6;
            int iloop = 0;
            int thr = 2;
            while (iloop < nloop)
            {
                idx = 0;
                foreach (Stroke stroke in activeStrokes)
                {
                    for (int i = 0; i < stroke.FaceCount; ++i)
                    {
                        if (this.visibleTriangleVertices[idx] > 0)
                        {
                            ++idx;
                            continue;
                        }
                        int neig = 0;
                        if (i + 1 < stroke.FaceCount && this.visibleTriangleVertices[idx + 1] > 0)
                        {
                            ++neig;
                        }
                        if (i - 1 >= 0 && this.visibleTriangleVertices[idx - 1] > 0)
                        {
                            ++neig;
                        }
                        if (neig == 2 && this.visibleTriangleVertices[idx] == 0)
                        {
                            this.visibleTriangleVertices[idx] = neig;
                            ++idx;
                            continue;
                        }
                        // if in any direction there is no more samples, cut it
                        int n_end = 0;
                        for (int j = i + 1, k = 1; j < stroke.FaceCount && n_end < thr; ++j, ++k)
                        {
                            if (this.visibleTriangleVertices[idx + k] > 0)
                            {
                                ++n_end;
                            }
                        }
                        if (n_end >= thr)
                        {
                            //end = thr < i ? thr : i;
                            n_end = 0;
                            for (int j = i - 1, k = 1; j >= 0 && n_end < thr; --j, ++k)
                            {
                                if (this.visibleTriangleVertices[idx - k] > 0)
                                {
                                    ++n_end;
                                }
                            }
                        }
                        if (n_end >= thr && this.visibleTriangleVertices[idx] == 0)
                        {
                            this.visibleTriangleVertices[idx] = Math.Min(1, n_end);
                        }
                        ++idx;
                    }
                }
                ++iloop;
            }// loop
            idx = 0;
            this.triangleDepth = new byte[n];
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    int nstrokes = edge.strokes.Count;
                    if (this.inGuideMode)
                    {
                        nstrokes = nstrokes > 2 ? 2 : nstrokes;
                    }
                    for (int i = 0; i < nstrokes; ++i)
                    {
                        Stroke stroke = edge.strokes[i];
                        for (int j = 0; j < stroke.FaceCount; ++j)
                        {
                            this.triangleDepth[idx] = (byte)(((float)this.visibleTriangleVertices[idx] / maxv) * 255);
                            if (this.triangleDepth[idx] == 0)
                            {
                                this.triangleDepth[idx] = 50;
                            }
                            ++idx;
                        }
                    }
                }
            }

        }// setHiddenLines


        private void calculateStrokePointDepth()
        {
            this.updateCamera();
            if (this.currSegmentClass == null)
            {
                return;
            }
            this.goThroughVisibilityTest();

            return;

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

        public void recoverHiddenlines()
        {
            if (this.currSegmentClass == null || this.depthType == Depthtype.hidden) return;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    this.setStrokeStylePerLine(edge, SegmentClass.StrokeSize, SegmentClass.StrokeColor);
                }
            }
        }//recoverHiddenlines


        public void exportSequenceDiv()
        {
            if(this.currSegmentClass == null) return;
            int idx = this.sequenceIdx;
            string folder = this.foldername + "\\pdfs";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string divName = folder + "\\seq_" + idx.ToString() + ".ps";
            this.calculate2D();
            StreamWriter sw = new StreamWriter(divName);
            sw.WriteLine("<</PageSize[" + this.Width.ToString() + " " + this.Height.ToString() + "]>>setpagedevice");
            //sw.WriteLine("0 50 translate");
            foreach(Segment seg in this.currSegmentClass.segments){
                if (!seg.active) continue;
                foreach (GuideLine line in seg.boundingbox.edges)
                {
                    foreach (Stroke stroke in line.strokes)
                    {
                        sw.WriteLine("newpath");
                        sw.Write(stroke.u2.x.ToString() + " ");
                        sw.Write(stroke.u2.y.ToString() + " ");
                        sw.WriteLine("moveto ");
                        sw.Write(stroke.v2.x.ToString() + " ");
                        sw.Write(stroke.v2.y.ToString() + " "); 
                        sw.WriteLine("lineto ");
                        sw.WriteLine("gsave");
                        sw.WriteLine(stroke.weight.ToString() + " " + "setlinewidth");
                        sw.WriteLine(stroke.strokeColor.R.ToString() + " " +
                            stroke.strokeColor.G.ToString() + " " +
                            stroke.strokeColor.B.ToString() + " setrgbcolor");
                        sw.WriteLine("stroke");
                    }
                }
            }
            if (this.activeSegment != null || this.currBoxIdx != -1)
            {
                if (this.showOnlyGuides)
                {

                }
                else
                {

                }

            }
            sw.WriteLine("showpage");
            sw.Close();
        }//exportSequenceDiv
        

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
                case 5:
                    this.currUIMode = UIMode.Sketch;
                    break;
                case 6:
                    this.currUIMode = UIMode.Eraser;
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
            this.cal2D();
            if (this.enableHiddencheck)
            {
                this.setHiddenLines();
            }
            this.computeContours();
            this.Refresh();
        }
        
        public void reloadView()
        {
            this.arcBall.reset();
            this.currModelTransformMatrix = new Matrix4d(this.fixedModelView);
            this.modelTransformMatrix = Matrix4d.IdentityMatrix();
            this.updateDepthVal();
            this.cal2D();
            if (this.enableHiddencheck)
            {
                this.setHiddenLines();
            }
            this.computeContours();
            this.Refresh();
        }
        

        private void updateCamera()
        {
            if (this.camera == null) return;
            Matrix4d m = this.currModelTransformMatrix;
            double[] ballmat =  m.Transpose().ToArray();	// matrix applied with arcball
            this.camera.SetBallMatrix(ballmat);
            this.camera.Update();
        }

        private void updateDepthVal()
        {
            if (this.depthType == Depthtype.opacity)
            {
                this.calculateStrokePointDepthByCameraPos();
                this.showOcclusion = true;
                this.enableDepthTest = false;
            }
            else if (this.depthType == Depthtype.hidden)
            {
                this.calculateStrokePointDepth();
                this.showOcclusion = false;
                this.enableDepthTest = false;
            }
            else if (this.depthType == Depthtype.rayTracing)
            {
                this.showOcclusion = true;
                this.enableDepthTest = false;
                this.calculateHiddenStrokePointDepthByLinePlaneIntersection();
            }
            else if (this.depthType == Depthtype.OpenGLDepthTest)
            {
                this.enableDepthTest = true;
                this.showOcclusion = false;
            } 
            else
            {
                this.showOcclusion = false;
                this.enableDepthTest = false;
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
            int type = 0;
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
                type = 1;
            }
            this.arcBall.mouseUp();
            //this.modelTransformMatrix = this.transMat * this.rotMat * this.scaleMat;

            this.modelTransformMatrix = this.currModelTransformMatrix.Transpose();
            this.cal2D();
            //if (type == 1)
            //{
            //    this.updateSketchStrokePositionToLocalCoord();
            //}

            this.updateDepthVal();
            if (this.enableHiddencheck)
            {
                this.setHiddenLines();
            }
            this.computeContours();
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
                case UIMode.Sketch:
                    {
                        this.SketchMouseDown(this.currMousePos);
                        break;
                    }
                case UIMode.Eraser:
                    {
                        this.EraserMouseDown(this.currMousePos, e);
                        break;
                    }
                case UIMode.Viewing:
                default:
                    {
                        this.viewMouseDown(e);
                        break;
                    }
            }
            this.Refresh();
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
                            //this.Refresh();
                        }
                        break;
                    }
                case UIMode.ComponentSelection:
                    {
                        break;
                    }
                case UIMode.Sketch:
                    {
                        this.SketchMouseMove(this.currMousePos);
                        break;
                    }
                case UIMode.Eraser:
                    {
                        this.EraserMouseMove(this.currMousePos, e);
                        break;
                    }
                case UIMode.Viewing:
                //default:
                    {
                        if (!this.lockView)
                        {
                            this.viewMouseMove(e.X, e.Y);
                            //this.Refresh();
                        }
                        break;
                    }
            }
            this.Refresh();
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
                        //this.Refresh();
                        break;
                    }
                case UIMode.ComponentSelection:
                    {
                        break;
                    }
                case UIMode.Sketch:
                    {
                        this.SketchMouseUp();
                        break;
                    }
                case UIMode.Eraser:
                    {
                        this.EraserMouseUp();
                        break;
                    }
                case UIMode.Viewing:
                default:
                    {
                        this.viewMouseUp();
                        //this.Refresh();
                        break;
                    }
            }
            this.Refresh();
        }// OnMouseUp

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // to get the correct 2d info of the points
            //this.Refresh();
            this.cal2D();
            this.updateSketchStrokePositionToLocalCoord();
            this.Refresh();
        }

        public void acceptKeyData(KeyEventArgs e)
        {
            SendKeys.Send(e.KeyData.ToString());
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Control == true && e.KeyCode == Keys.C)
            {
                this.clearAllStrokes();
                this.Refresh();
                return;
            }
            if (e.Control == true && e.KeyCode == Keys.Z)
            {
                if (this.sketchStrokes.Count > 0)
                {
                    this.sketchStrokes.RemoveAt(this.sketchStrokes.Count - 1);
                    this.Refresh();
                    return;
                }
            }
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
                case Keys.S:
                    {
                        this.currUIMode = UIMode.Sketch;
                        break;
                    }
                case Keys.E:
                    {
                        this.currUIMode = UIMode.Eraser;
                        break;
                    }
                case Keys.D:
                    {
                        this.inGuideMode = !this.inGuideMode;
                        if (this.inGuideMode)
                        {
                            this.showBoundingbox = false;
                            this.showMesh = false;
                            //this.nextBox();
                            this.nextSequence();
                        }
                        else
                        {
                            this.resumeBoxes();
                        }
                        this.Refresh();
                        break;
                    }
                case Keys.Space:
                    {
                        this.lockView = !this.lockView;
                        Program.GetFormMain().setLockState(this.lockView);
                        break;
                    }
                case Keys.PageDown:
                case Keys.Right:
                    {
                        if (!e.Shift)
                        {
                            this.nextSequence();
                        }
                        break;
                    }
                case Keys.PageUp:
                case Keys.Left:
                    {
                        if (!e.Shift)
                        {
                            this.prevSequence();
                        }
                        break;
                    }
                default:
                    break;
            }
            this.Refresh();
        }// OnKeyDown        

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (this.currUIMode == UIMode.Sketch || this.currUIMode == UIMode.Eraser)
            {
                double ratio = e.Delta / 100.0;
                if (ratio < 0)
                {
                    ratio = 1.0 / Math.Abs(ratio);
                }
                this.penRadius *= ratio;
                SegmentClass.PenSize = SegmentClass.PenSize * ratio;
                this.Refresh();
            }
        }//OnMouseWheel

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaint(e);
            this.MakeCurrent();
            this.clearScene();
            //this.Draw2D();
            this.Draw3D();
            this.Draw2D();

            this.SwapBuffers();
        }// onPaint

        //######### Sketch #########//
        
        private void SketchMouseDown(Vector2d p)
        {
            Vector2d pos = new Vector2d(p.x, this.Height - p.y);
            this.currSketchPoints = new List<Vector2d>();
            this.currSketchPoints.Add(pos);
            this.strokeLength = 0;

            this.lockView = true;
            Program.GetFormMain().setLockState(true);
        }

        public void SketchMouseMove(Vector2d p)
        {
            if (!this.isMouseDown) return;
            Vector2d prevpos = this.currSketchPoints[this.currSketchPoints.Count - 1];
            Vector2d pos = new Vector2d(p.x, this.Height - p.y);
            Vector2d off = pos - prevpos;
            double len = off.Length();
            Vector2d offnormal = off.normalize();
            this.strokeLength += len;

            double dis_thres = 6.0;
            if (len > dis_thres) 
            {
                // moving the pen
                int steps = (int)(len / dis_thres) + 1;
                steps = Math.Min(4, steps);
                double delta = len / steps;
                for (int i = 1; i <= steps; ++i)
                {
                    Vector2d v = prevpos + offnormal * (i * delta);
                    this.currSketchPoints.Add(v);
                }
            }
        }

        public void SketchMouseUp()
        {
            if (this.currSketchPoints.Count > 5) {
                bool isLongEnough = strokeLength >= 5;
                if (isLongEnough)
                {
                    CubicSpline2 spline = new CubicSpline2(this.currSketchPoints.ToArray());
                    this.currStroke = new Stroke(this.currSketchPoints, SegmentClass.PenSize);
                    this.smoothStroke(this.currStroke);
                    this.currStroke.strokeColor = SegmentClass.PenColor;
                    this.storeSketchStrokePositionToLocalCoord(this.currStroke);
                    this.sketchStrokes.Add(this.currStroke);
                }
            }
            this.currStroke = null;
            this.currSketchPoints = new List<Vector2d>();
        }//SketchMouseUp

        private void smoothStroke(Stroke stroke)
        {
            int nloop = 5;
            int n = 0;
            int step = 3;
            while (n < nloop)
            {
                for (int i = n; i < stroke.strokePoints.Count - step; i += step)
                {
                    Vector2d v1 = stroke.strokePoints[i].pos2;
                    Vector2d v2 = stroke.strokePoints[i + step].pos2;
                    double len = (v1-v2).Length()/step;
                    Vector2d dir = (v2 - v1).normalize();
                    for (int j = i; j < i + step; ++j)
                    {
                        stroke.strokePoints[j].pos2 = v1 + (j * len) * dir;
                    }

                }
                ++n;
            }
        }

        private void findClosestVertex(List<Vector2d> points)
        {
            if (this.currSegmentClass == null) return;
            List<Vector2d> meshPoints2d = new List<Vector2d>();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (seg.mesh == null) continue;
                for (int i = 0, j = 0; i < seg.mesh.VertexCount; ++i, j += 3)
                {
                    Vector3d v = new Vector3d(seg.mesh.VertexPos[j], seg.mesh.VertexPos[j + 1], seg.mesh.VertexPos[j + 2]);
                    
                }
            }
        }

        private void storeSketchStrokePositionToLocalCoord(Stroke stroke)
        {
            this.cal2D();
            Vector2d minCoord = Vector2d.MaxCoord(), maxCoord = Vector2d.MinCoord();
            this.calSegmentsBounds(out minCoord, out maxCoord);
            double wl = maxCoord.x - minCoord.x;
            double hl = maxCoord.y - minCoord.y;
            double wg = this.Width;
            double hg = this.Height;

            // relative position [0, 1]
            //foreach (Stroke stroke in this.sketchStrokes)
            //{
                foreach (StrokePoint sp in stroke.strokePoints)
                {
                    sp.pos2_local = new Vector2d(
                        (sp.pos2.x - minCoord.x) / wl,
                        (sp.pos2.y - minCoord.y) / hl);
                }
            //}
        }//storeSketchStrokePositionToLocalCoord

        private void updateSketchStrokePositionToLocalCoord()
        {
            Vector2d minCoord = Vector2d.MaxCoord(), maxCoord = Vector2d.MinCoord();
            this.calSegmentsBounds(out minCoord, out maxCoord);
            double wl = maxCoord.x - minCoord.x;
            double hl = maxCoord.y - minCoord.y;
            double wg = this.Width;
            double hg = this.Height;
            // update pos2 origin in loca cooord

            foreach (Stroke stroke in this.sketchStrokes)
            {
                foreach (StrokePoint sp in stroke.strokePoints)
                {
                    sp.pos2 = new Vector2d(
                        sp.pos2_local.x * wl,
                        sp.pos2_local.y * hl);
                    sp.pos2 += minCoord;
                }
                stroke.changeStyle2d((int)SegmentClass.strokeStyle);
            }
        }//updateSketchStrokePositionToLocalCoord


        private List<Stroke> currEraseStrokes;
        private double penRadius = 8.0f;
        private void selectEraseStroke(Vector2d p)
        {
            this.currEraseStrokes = new List<Stroke>();
            Vector2d pos = new Vector2d(p.x, this.Height - p.y);
            foreach (Stroke stroke in this.sketchStrokes)
            {
                foreach (StrokePoint sp in stroke.strokePoints)
                {
                    if ((sp.pos2 - pos).Length() < this.penRadius)
                    {
                        this.currEraseStrokes.Add(stroke);
                        break;
                    }
                }
            }
        }//selectEraseStroke

        public void clearAllStrokes()
        {
            this.currStroke = null;
            this.sketchStrokes = new List<Stroke>();
        }

        private void SplitStrokeByEraser(Stroke stroke, Vector2d pos)
		{
			// split the current stroke under erasing
			double radius = this.penRadius;
			int s = -1, e = -1;
			int n = stroke.strokePoints.Count;
			for (int i = 0; i < n; ++i )
			{
				StrokePoint p = stroke.strokePoints[i];
				if ((pos - p.pos2).Length() < radius)
				{
					if (s == -1)
						s = i;
				}
				else if (s != -1)
				{
					e = i;
					break;
				}
			}
			if (s == -1 || e == -1)
				return;
			List<StrokePoint> points_left = stroke.strokePoints.GetRange(0, s);
			List<StrokePoint> points_right = stroke.strokePoints.GetRange(e, n - e);
            List<Vector2d> left = new List<Vector2d>();
            List<Vector2d> right = new List<Vector2d>();
            double len_left = 0, len_right = 0;
            for(int i = 0; i < points_left.Count; ++i){
                StrokePoint sp = points_left[i];
                if(i + 1 < points_left.Count){
                len_left += (sp.pos2 - points_left[i+1].pos2).Length();
                }
                left.Add(sp.pos2);
            }
            for(int i = 0; i < points_right.Count; ++i){
                StrokePoint sp = points_right[i];
                if(i + 1 < points_right.Count){
                len_right += (sp.pos2 - points_right[i+1].pos2).Length();
                }
                right.Add(sp.pos2);
            }
			if(len_left > this.penRadius * 2){
                this.sketchStrokes.Add(new Stroke(left, SegmentClass.PenSize));
            }
            if(len_right > this.penRadius * 2){
                this.sketchStrokes.Add(new Stroke(right, SegmentClass.PenSize));
            }
			this.sketchStrokes.Remove(stroke);
		}//SplitStrokeByEraser

        private void toneStroke(Vector2d pos)
        {
            if (this.currEraseStrokes.Count == 0) return;
            double radius = this.penRadius;
            pos = new Vector2d(pos.x, this.Height - pos.y);
            for (int s = 0; s < this.currEraseStrokes.Count; ++s)
            {
                Stroke stroke = this.currEraseStrokes[s];
                // get the set of points that is on the radius
                List<int> point_in_radii = new List<int>();
                // endpoint
                double len_to_left = (pos - stroke.strokePoints[0].pos2).Length();
                double len_to_right = (pos - stroke.strokePoints[stroke.strokePoints.Count - 1].pos2).Length();

                bool left_end_in = len_to_left < radius;
                bool right_end_in = len_to_right < radius;

                //if (!left_end_in && !right_end_in)
                //	continue;
                if (!left_end_in && !right_end_in)
                {
                    this.SplitStrokeByEraser(stroke, pos);
                    this.currEraseStrokes.RemoveAt(s);
                    --s;
                    continue;
                }

                for (int i = 0; i < stroke.strokePoints.Count; ++i)
                {
                    double len = (pos - stroke.strokePoints[i].pos2).Length();
                    if (len < radius)
                    {
                        point_in_radii.Add(i);
                    }
                }
                // remove if not at endpoint
                if (left_end_in && right_end_in)
                {
                    // select on endpoint
                    int split = -1;
                    for (int i = 0; i < point_in_radii.Count - 1; ++i)
                    {
                        if (point_in_radii[i + 1] - point_in_radii[i] > 1)
                        {
                            split = i;
                        }
                    }
                    if (split >= 0)
                    {
                        if (split > point_in_radii.Count - split - 1)
                        {
                            int n = point_in_radii.Count - split - 1;
                            for (int i = 0; i < n; ++i)
                            {
                                point_in_radii.RemoveAt(point_in_radii.Count - 1);
                            }
                            right_end_in = false;
                        }
                        else
                        {
                            for (int i = 0; i <= split; ++i)
                            {
                                point_in_radii.RemoveAt(0);
                            }
                            left_end_in = false;
                        }
                    }
                }
                if (point_in_radii.Count == 0)
                    continue;
                //bool left_end_in = point_in_radii[0] == 0;
                //bool right_end_in = point_in_radii[point_in_radii.Count - 1] == stroke.strokePoints.Count - 1;

                int left = point_in_radii[0];
                int right = point_in_radii[point_in_radii.Count - 1];

                if (point_in_radii.Count == stroke.strokePoints.Count)
                    stroke.strokePoints.Clear();
                else
                {
                    StrokePoint I = stroke.strokePoints[0];
                    if (left_end_in)
                    {
                        for (int i = 0; i < stroke.strokePoints.Count; ++i)
                        {
                            if (!point_in_radii.Contains(i))
                            {
                                I = stroke.strokePoints[i - 1];
                                break;
                            }
                        }
                    }
                    StrokePoint J = stroke.strokePoints[stroke.strokePoints.Count - 1];
                    if (right_end_in)
                    {
                        for (int i = stroke.strokePoints.Count - 1; i >= 0; --i)
                        {
                            if (!point_in_radii.Contains(i))
                            {
                                J = stroke.strokePoints[i + 1];
                                break;
                            }
                        }
                    }
                    List<StrokePoint> new_stroke_points = new List<StrokePoint>();
                    int old_count = stroke.strokePoints.Count;
                    int new_count = old_count - point_in_radii.Count + 1;
                    for (int i = 0; i < stroke.strokePoints.Count; ++i)
                    {
                        if (!point_in_radii.Contains(i))
                        {
                            new_stroke_points.Add(stroke.strokePoints[i]);
                        }
                    }
                    if (left_end_in)
                    {
                        StrokePoint u = I, v = new_stroke_points[0];
                        Vector2d q = Polygon.FindLinesegmentCircleIntersection(u.pos2, v.pos2, pos, this.penRadius);
                        new_stroke_points.Insert(0, new StrokePoint(q));	// at head


                    }
                    if (right_end_in)
                    {
                        StrokePoint u = J, v = new_stroke_points[new_stroke_points.Count - 1];
                        Vector2d q = Polygon.FindLinesegmentCircleIntersection(u.pos2, v.pos2, pos, this.penRadius);
                        new_stroke_points.Add(new StrokePoint(q));		// at tail

                    }
                    if (stroke.get2DLength() < this.penRadius * 2)
                    {
                        this.currEraseStrokes.RemoveAt(s);
                        --s;
                    }
                    else
                    {
                        stroke.changeStyle2d((int)SegmentClass.strokeStyle);
                    }
                }
            }
        }

        private void EraserMouseDown(Vector2d p, MouseEventArgs e)
        {
            this.selectEraseStroke(p);
            if(e.Button == System.Windows.Forms.MouseButtons.Left){
                this.toneStroke(p);
            }else if(e.Button == System.Windows.Forms.MouseButtons.Right){
                foreach(Stroke stroke in this.currEraseStrokes){
                    this.sketchStrokes.Remove(stroke);
                }
            }
        }

        private void EraserMouseMove(Vector2d p, MouseEventArgs e)
        {
            this.selectEraseStroke(p);
            if(e.Button == System.Windows.Forms.MouseButtons.Left){
                this.toneStroke(p);
            }else if(e.Button == System.Windows.Forms.MouseButtons.Right){
                foreach(Stroke stroke in this.currEraseStrokes){
                    this.sketchStrokes.Remove(stroke);
                }
            }
            this.currEraseStrokes = new List<Stroke>();
        }//EraserMouseMove

        private void EraserMouseUp()
        {
            this.currEraseStrokes = new List<Stroke>();
        }//EraserMouseUp

        //######### end- Sketch #########//



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
            Glu.gluLookAt(0, 0, 1.5, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
            //Glu.gluLookAt(-2, 1, 0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            //Gl.glLoadMatrixd(this.camera.GetModelviewMat().ToArray());

            //this.drawPoints3d(new Vector3d[1] { this.eye }, Color.DarkOrange, 10.0f);
            Matrix4d m = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
            m = Matrix4d.TranslationMatrix(this.objectCenter) * m * Matrix4d.TranslationMatrix(
                new Vector3d() - this.objectCenter);


            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glMultMatrixd(m.Transpose().ToArray());
        }

        private int startWid = 0, startHeig = 0;

        private void Draw2D()
        {
            int w = this.Width, h = this.Height;

            //Gl.glPushMatrix();

            Gl.glViewport(0, 0, w, h);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            //double aspect = (double)w / h;
            //Glu.gluPerspective(90, aspect, 0.1, 1000);
            //Glu.gluLookAt(0, 0, 1.5, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);

            Glu.gluOrtho2D(0, w, 0, h);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glPushMatrix();

            //Gl.glScaled((double)this.Width / this.startWid, (double)this.Height / this.startHeig, 1.0);

            if (this.paperPos != null)
            {
                this.drawPaperBoundary2d();
            }

            this.DrawHighlight2D();


            this.drawVanishingPoints2d();

            this.drawSketchPoints();
            this.drawSketchStrokes();

            //this.drawSketchyLines2D();   
         
            /*****TEST*****/
            //this.drawTest2D();

            //this.DrawLight();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
            //Gl.glPopMatrix();
        }

        private void drawSketchPoints()
        {
            if (this.currSketchPoints != null)
            {
                this.drawLines2D(this.currSketchPoints, SegmentClass.PenColor, (float)SegmentClass.PenSize * 2);
                //this.drawPoints2d(this.currSketchPoints.ToArray(), SegmentClass.PenColor, (float)SegmentClass.PenSize);
            }
            //foreach (List<Vector2d> points in this.sketchPoints)
            //{
            //    //this.drawLines2D(points, Color.Black, 4.0f);
            //    this.drawPoints2d(points.ToArray(), Color.Black, 4.0f);
            //}
        }

        private void drawSketchStrokes()
        {
            if (!this.showDrawnStroke) return;
            foreach (Stroke stroke in this.sketchStrokes)
            {
                if (this.drawShadedOrTexturedStroke)
                {
                    this.drawTriMeshShaded2D(stroke, false, this.showOcclusion);
                }
                else
                {
                    this.drawTriMeshTextured2D(stroke, this.showOcclusion);
                }
            }

            if (this.currStroke != null)
            {
                if (this.drawShadedOrTexturedStroke)
                {
                    this.drawTriMeshShaded2D(this.currStroke, false, this.showOcclusion);
                }
                else
                {
                    this.drawTriMeshTextured2D(this.currStroke, this.showOcclusion);
                }
            }
        }// drawSketchStrokes

        public void saveSketches(string filename)
        {
            if (filename == "")
            {
                filename = this.foldername + "\\contour.sketch";
            }
            StreamWriter sw = new StreamWriter(filename);
            int i = 0;
            foreach (Stroke stroke in this.sketchStrokes)
            {
                sw.WriteLine("sketch " + i.ToString());
                sw.WriteLine(stroke.strokePoints.Count().ToString());
                foreach (StrokePoint sp in stroke.strokePoints)
                {
                    //sw.WriteLine(sp.pos2.x.ToString() + " " + sp.pos2.y.ToString());
                    sw.WriteLine(sp.pos2_local.x.ToString() + " " + sp.pos2_local.y.ToString());
                }
                ++i;
            }
            sw.Close();
        }//saveSketches

        public void loadSketches(string filename)
        {
            if (filename == "")
            {
                filename = this.foldername + "\\contour.sketch";
            }
            if (!File.Exists(filename)) return;

            StreamReader sr = new StreamReader(filename);
            char[] separator = { ' ' };
            this.sketchStrokes = new List<Stroke>();
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                string[] tokens = line.Split(separator);
                int n = 0;
                if (tokens.Length > 0 && tokens[0] == "sketch")
                {
                    line = sr.ReadLine();
                    tokens = line.Split(separator);
                    n = Int32.Parse(tokens[0]);
                }
                List<Vector2d> points = new List<Vector2d>();
                for (int i = 0; i < n; ++i)
                {
                    line = sr.ReadLine();
                    tokens = line.Split(separator);
                    double x = double.Parse(tokens[0]);
                    double y = double.Parse(tokens[1]);
                    Vector2d p = new Vector2d(x, y);
                    points.Add(p);
                }
                Stroke stroke = new Stroke(points, SegmentClass.PenSize);
                stroke.strokeColor = Color.Black;
                stroke.changeStyle2d((int)SegmentClass.strokeStyle);
                this.sketchStrokes.Add(stroke);
            }
            this.updateSketchStrokePositionToLocalCoord();
        }

        private void drawPaperBoundary2d()
        {
            Color c = Color.FromArgb(120,198,121);
            float size = 8.0f;
            Gl.glColor3ub(c.R, c.G, c.B);

            Gl.glPointSize(size);
            Gl.glBegin(Gl.GL_POINTS);
            for (int i = 0; i < this.paperPos.Length; ++i)
            {
                Gl.glVertex2dv(this.paperPos[i].pos2.ToArray());
            }
            Gl.glEnd();

            for (int i = 0; i < 4; ++i)
            {
                //if (i % 2 == 0)
                //{
                //    this.drawLines2D(this.paperPos[i].pos2 - new Vector2d(size / 4, 0), this.paperPosLines[2 * i], c, size);
                //}
                //else
                //{
                //    this.drawLines2D(this.paperPos[i].pos2 + new Vector2d(size / 4, 0), this.paperPosLines[2 * i], c, size);
                //}
                this.drawLines2D(this.paperPos[i].pos2, this.paperPosLines[2 * i], c, size);
                this.drawLines2D(this.paperPos[i].pos2, this.paperPosLines[2 * i + 1], c, size);
            }

        }//drawPaperBoundary2d

        private void drawVanishingPoints2d()
        {
            if (this.vanishingPoints == null || !this.showVanishingPoints) return;
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

            Color c = Color.DarkOrange;
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glPointSize(10.0f);
            Gl.glBegin(Gl.GL_POINTS);

            if (this.showVanishingRay1)
            {
                Gl.glVertex2dv(this.vanishingPoints[0].pos2.ToArray());
            }
            if (this.showVanishingRay2)
            {
                Gl.glVertex2dv(this.vanishingPoints[1].pos2.ToArray());
            }
            Gl.glEnd();

            Gl.glDisable(Gl.GL_POINT_SMOOTH);
        }//drawVanishingPoints2d

        private void drawPaperBoundary3d()
        {
            Color c = Color.Red;
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glPointSize(10.0f);
            Gl.glBegin(Gl.GL_POINTS);
            for (int i = 0; i < this.paperPos.Length; ++i)
            {
                Gl.glVertex3dv(this.paperPos[i].pos3.ToArray());
            }
            Gl.glEnd();

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
                this.drawPoints3d(new Vector3d[1] { this.testPoint.pos3 }, Color.DarkSalmon, 10.0f);
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

        private int visibilityIdx = 0;
        private void Draw3D()
        {

            this.setViewMatrix();

            SetDefaultLight();

            /***** Draw *****/
            //clearScene();

            if (this.isDrawAxes)
            {
                this.drawAxes();
            }

            // for visibility rendering, the order is computed from
            // setHiddenLines()

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
                    if (this.isShowContour())
                    {
                        this.drawMeshFace(this.currMeshClass.Mesh, Color.WhiteSmoke, false);
                    }
                    else
                    {
                        this.drawMeshFace(this.currMeshClass.Mesh, SegmentClass.MeshColor, true);
                    }
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
            this.drawTestLines();

            if (this.showSketchyEdges)
            {
                if (this.showLineOrMesh)
                {
                    this.drawAllActiveBoxes_line();
                }
                else
                {
                    this.drawAllActiveBoxes();
                }
            }
            if (this.showGuideLines)
            {
                if (this.showLineOrMesh)
                {
                    this.drawGuideLines3D_line();
                }
                else
                {
                    this.drawGuideLines3D();
                }
            }

            this.drawContours();            

            this.DrawHighlight3D();

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

            //this.SwapBuffers();
        }// Draw3D     
   
        private void drawContours()
        {
            // for mesh
            if (this.isShowContour())
            {
                this.drawContourPoints();
            }

            if (this.showSharpEdge)
            {
                this.drawSharpEdges();
            }
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_ADD);

            if (this.currSegmentClass == null) return;
            // for segment
            float width = 6.0f;
            if (this.showSegSilhouette)
            {
                this.drawSegmentSilhouette(Color.FromArgb(252, 141, 98), width);
            }

            if (this.showSegContour)
            {
                this.drawSegmentContour(Color.FromArgb(0, 15, 85), width);
            }

            if (this.showSegSuggestiveContour)
            {
                this.drawSegmentSuggestiveContour(Color.FromArgb(231, 138, 195), width);
            }

            if (this.showSegApparentRidge)
            {
                this.drawSegmentApparentRige(SegmentClass.StrokeColor, width);
            }

            if (this.showSegBoundary)
            {
                this.drawSegmentBoundary(Color.FromArgb(251, 106, 74), width);
            }
            Gl.glDisable(Gl.GL_DEPTH_TEST);
        }//drawContours

        private bool isShowContour()
        {
            return this.showSegSilhouette || this.showSegContour || this.showSegSuggestiveContour 
                || this.showSegApparentRidge || this.showSegBoundary;
        }

        private void drawSegments()
        {
            if (this.currSegmentClass == null)
            {
                return;
            }
            
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                if (this.showMesh)
                {
                    if (this.drawFace)
                    {
                        if (this.isShowContour() || this.showDrawnStroke)
                        {
                            //this.drawMeshFace(seg.mesh, Color.Blue, false);
                            this.drawMeshFace(seg.mesh, SegmentClass.MeshColor, false);
                        }
                        else
                        {
                            this.drawMeshFace(seg.mesh, seg.color, false);
                            //this.drawMeshFace(seg.mesh, Color.WhiteSmoke, false);
                        }                        
                    }
                    if (this.drawEdge)
                    {
                        this.drawMeshEdge(seg.mesh);
                    }
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

        private void drawSketchyEdges3D_hiddenLine()
        {
            if (this.currSegmentClass == null || !this.showSketchyEdges)
            {
                return;
            }
            int idx = 0;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    foreach(Stroke stroke in edge.strokes)
                    {
                        if (seg.active)
                        {
                            if (this.drawShadedOrTexturedStroke)
                            {
                                this.drawTriMeshShaded3D_hiddenLine(stroke, false);
                            }
                            else
                            {
                                this.drawTriMeshTextured3D(stroke, this.showOcclusion);
                            }
                        }
                        idx += stroke.FaceCount;
                    }
                }

            }
        }// drawSketchyEdges3D_hiddenLine

        private void drawAllActiveBoxes()
        {
            if (this.currSegmentClass == null || !this.showSketchyEdges)
            {
                return;
            }
            this.visibilityIdx = 0;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                //this.drawBoundingboxWithoutBlend(seg.boundingbox, Color.White);
                if (this.enableDepthTest)
                {
                    this.drawBoundingbox(seg.boundingbox, Color.White);
                }
                foreach (GuideLine edge in seg.boundingbox.edges)
                {
                    //this.drawGuideLineEndpoints(edge, Color.Gray, 2.0f);
                    if (!edge.active) continue;
                    int nstrokes = edge.strokes.Count;// rand.Next(1, edge.strokes.Count);                    
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
                            if (this.depthType == Depthtype.hidden)
                            {
                                this.drawTriMeshShaded3D_hiddenLine(stroke, false);
                            }
                            else
                            {
                                this.drawTriMeshShaded3D(stroke, false, this.showOcclusion);
                            }
                        }
                        else
                        {
                            this.drawTriMeshTextured3D(stroke, this.showOcclusion);
                        }
                    }
                }
            }
            
        }// drawAllActiveBoxes

        private void drawAllActiveBoxes_line()
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
                    if (!edge.active) continue;
                    int nstrokes = edge.strokes.Count;                
                    if (this.inGuideMode)
                    {
                        nstrokes = nstrokes > 1 ? 1 : nstrokes;
                    }
                    for (int i = 0; i < nstrokes; ++i)
                    {
                        Stroke stroke = edge.strokes[i];
                        this.drawLines3D(stroke.u3, stroke.v3, stroke.strokeColor, (float)stroke.weight);
                    }
                }
            }
        }// drawAllActiveBoxes_line

        private void drawTheFormationGuidLines()
        {
            if (this.currSegmentClass == null || !this.drawPrevBoxGuideLines) return;
            //if (this.activeSegment != null &&
            //    this.activeSegment.boundingbox.guideBoxIdx != -1)
            if (this.currBoxIdx != -1)
            {
                Box box = this.currSegmentClass.segments[this.currBoxIdx].boundingbox;
                if (box.guideBoxIdx == -1) return;
                int bIdx = box.guideBoxIdx;
                int gIdx = box.guideBoxSeqGroupIdx;
                Box guidebox = this.currSegmentClass.segments[bIdx].boundingbox;

                for (int i = 0; i < box.guideBoxSeqenceIdx.Count; ++i)
                {
                    int sidx =  box.guideBoxSeqenceIdx[i];
                    GuideLine line = guidebox.guideLines[gIdx][sidx];
                    //this.setGuideLineTypeColor(line);
                    foreach (Stroke stroke in line.strokes)
                    {
                        //this.drawLines3D(stroke.u3, stroke.v3, SegmentClass.StrokeColor, (float)stroke.weight);
                        this.drawLines3D(stroke.u3, stroke.v3, SegmentClass.GuideLineWithTypeColor, (float)stroke.weight);
                    }
                }
            }
        }//drawTheFormationGuidLines

        private void drawGuideLines3D()
        {
            if (this.currSegmentClass == null || !this.showGuideLines)
            {
                return;
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                Box box = seg.boundingbox;
                for (int g = 0; g < box.guideLines.Count; ++g)
                {
                    foreach (GuideLine line in box.guideLines[g])
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
            }
            
        }// drawGuideLines3D

        private void drawGuideLines3D_line()
        {
            if (this.currSegmentClass == null || !this.showGuideLines)
            {
                return;
            }
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                if (!seg.active) continue;
                Box box = seg.boundingbox;
                for (int g = 0; g < box.guideLines.Count; ++g)
                {
                    foreach (GuideLine line in box.guideLines[g])
                    {
                        if (!line.active) continue;
                        foreach (Stroke stroke in line.strokes)
                        {
                            this.drawLines3D(stroke.u3, stroke.v3, stroke.strokeColor, (float)stroke.weight);
                        }
                    }
                }
            }
        }// drawGuideLines3D_line

        private void drawTestLines()
        {
            if (this.testLines == null) return;
            foreach (GuideLine line in this.testLines)
            {
                this.drawDashedLines3D(line.u, line.v, SegmentClass.HiddenColor, 4.0f);
            }
        }

        private void drawGuideArrow(Arrow3D a, Color c, float lineWidth)
        {
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glLineWidth(lineWidth);
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

        private void DrawHighlight2D()
        {
            if (this.inExample)
            {
                this.drawVanishingGuide2d(this.currSegmentClass.segments[0]);
            }

            if (this.currUIMode == UIMode.Sketch ||this.currUIMode == UIMode.Eraser)
            {
                DrawCircle2(new Vector2d(this.currMousePos.x, this.Height-this.currMousePos.y), Color.Black, (float)this.penRadius);
            }

            if (this.currBoxIdx == -1 || this.currBoxIdx > this.currSegmentClass.segments.Count)
                return;

            Segment seg = this.currSegmentClass.segments[this.currBoxIdx];
            this.drawVanishingGuide2d(seg);
        }

        private void DrawHighlight3D()
        {
            if (this.currBoxIdx == -1 || this.currBoxIdx > this.currSegmentClass.segments.Count)
                return;

            Segment seg = this.currSegmentClass.segments[this.currBoxIdx];
            Box box = seg.boundingbox;
            if (this.enableDepthTest)
            {
                Gl.glDisable(Gl.GL_DEPTH_TEST);
            }
            if (this.showBlinking && box.highlightFaceIndex != -1 && box.facesToHighlight != null)
            {
                this.drawQuad3d(box.facesToHighlight[box.highlightFaceIndex], SegmentClass.FaceColor);
                this.drawQuadEdge3d(box.facesToHighlight[box.highlightFaceIndex], SegmentClass.StrokeColor);
            }
            
            if (this.showAllFaceToDraw)
            {
                foreach (Plane face in box.facesToDraw)
                {
                    this.drawQuad3d(face, Color.LightGreen);
                    this.drawQuadEdge3d(face, SegmentClass.HiddenColor);
                }
            }

            if (this.showLineOrMesh)
            {
                this.drawActiveBox_line(seg);
            }
            else
            {
                this.drawActiveBox(seg);
            }

            if (box.highlightFaceIndex != -1 && box.facesToHighlight != null)
            {
                this.drawQuadEdge3d(box.facesToHighlight[box.highlightFaceIndex], SegmentClass.StrokeColor);
            }

            if (this.showLineOrMesh)
            {
                this.drawActiveGuideLines_line(seg);
            }
            else
            {
                this.drawActiveGuideLines(seg);
            }

            this.drawAnimatedLine();

            if (this.showArrows && box.arrows != null)
            {
                foreach (Arrow3D arrow in box.arrows)
                {
                    if (!arrow.active) continue;
                    this.drawGuideArrow(arrow, SegmentClass.ArrowColor, 2.0f);
                }
            }
            
            if (this.enableDepthTest)
            {
                Gl.glEnable(Gl.GL_DEPTH_TEST);
            }
        }// DrawHighlight3D

        public bool inExample = false;
        private void drawActiveBox(Segment seg)
        {
            if (!this.inGuideMode) return;
            if (this.enableDepthTest)
            {
                this.drawBoundingbox(seg.boundingbox, Color.White);
            }

            // draw bounding edges
            foreach (GuideLine edge in seg.boundingbox.edges)
            {
                //this.drawGuideLineEndpoints(edge, Color.Gray, 1.0f);
                if (!edge.active) continue;
                foreach (Stroke stroke in edge.strokes)
                {
                    if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                    {
                        this.drawLines3D(stroke.u3, stroke.v3, stroke.strokeColor, 2.0f);
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
                //if (this.showVanishingLines)
                //{
                //    this.drawVanishingLines3d(seg.boundingbox);
                //}
            }
        }// drawActiveBox


        private void drawActiveBox_line(Segment seg)
        {
            if (!this.inGuideMode) return;
            if (this.enableDepthTest)
            {
                this.drawBoundingbox(seg.boundingbox, Color.White);
            }

            // draw bounding edges
            foreach (GuideLine edge in seg.boundingbox.edges)
            {
                if (!edge.active) continue;
                int nstrokes = edge.strokes.Count;
                if (this.inGuideMode && seg.drawn)
                {
                    nstrokes = nstrokes > 1 ? 1 : nstrokes;
                }
                for (int i = 0; i < nstrokes; ++i)
                {
                    Stroke stroke = edge.strokes[i];

                    this.drawLines3D(stroke.u3, stroke.v3, stroke.strokeColor, (float)stroke.weight);
                }
                foreach (Ellipse3D e in seg.boundingbox.ellipses)
                {
                    this.drawEllipseCurve3D(e);
                }
                //if (this.showVanishingLines)
                //{
                //    this.drawVanishingLines3d(seg.boundingbox);
                //}
            }
        }//awActiveBox

        private void drawActiveGuideLines(Segment seg)
        {
            // draw guidelines
            Box box = seg.boundingbox;

            if (box.activeFaceIndex != -1 && box.facesToDraw != null)
            {
                if (this.showFaceToDraw)
                {
                    this.drawQuadEdge3d(box.facesToDraw[box.activeFaceIndex], SegmentClass.FaceColor);
                    this.drawQuad3d(box.facesToDraw[box.activeFaceIndex], SegmentClass.StrokeColor);
                }
                else
                {
                    this.drawQuadEdge3d(box.facesToDraw[box.activeFaceIndex], SegmentClass.StrokeColor);
                }
            }
            // draw assited guide lines
            for (int g = 0; g < box.guideLines.Count; ++g)
            {
                foreach (GuideLine line in box.guideLines[g])
                {
                    if (!line.active) continue;
                    foreach (Stroke stroke in line.strokes)
                    {
                        if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                        {
                            this.drawLines3D(stroke.u3, stroke.v3, stroke.strokeColor, 2.0f);
                        }
                        this.drawTriMeshShaded3D(stroke, false, false);
                    }
                }
                // draw red guide lines
                foreach (GuideLine line in box.guideLines[g])
                {
                    if (!line.active || !line.isGuide) continue;
                    foreach (Stroke stroke in line.strokes)
                    {
                        this.drawTriMeshShaded3D(stroke, false, false);
                    }
                }
            }
            this.drawTheFormationGuidLines();
        }// drawActiveGuideLines

        private void drawActiveGuideLines_line(Segment seg)
        {
            // draw guidelines
            Box box = seg.boundingbox;
            if (box.activeFaceIndex != -1 && box.facesToDraw != null)
            {
                if (this.showFaceToDraw)
                {
                    this.drawQuadEdge3d(box.facesToDraw[box.activeFaceIndex], SegmentClass.FaceColor);
                    this.drawQuad3d(box.facesToDraw[box.activeFaceIndex], SegmentClass.FaceColor);
                }
                else
                {
                    this.drawQuadEdge3d(box.facesToDraw[box.activeFaceIndex], SegmentClass.HiddenColor);
                }
            }
            for (int g = 0; g < box.guideLines.Count; ++g)
            {
                foreach (GuideLine line in box.guideLines[g])
                {
                    if (!line.active) continue;
                    //this.drawGuideLineEndpoints(line, GLViewer.GuideLineColor, 2.0f);
                    foreach (Stroke stroke in line.strokes)
                    {
                        this.drawLines3D(stroke.u3, stroke.v3, stroke.strokeColor, (float)stroke.weight);
                    }
                }
                // draw red guide lines
                foreach (GuideLine line in box.guideLines[g])
                {
                    if (!line.active || !line.isGuide) continue;
                    foreach (Stroke stroke in line.strokes)
                    {
                        this.drawLines3D(stroke.u3, stroke.v3, stroke.strokeColor, (float)stroke.weight);
                    }
                }
            }
            this.drawTheFormationGuidLines();
        }// drawActiveGuideLines

        private void drawVanishingGuide2d(Segment seg)
        {
            if (this.showVanishingLines && this.showBoxVanishingLine && !this.showOnlyGuides)
            {
                this.drawVanishingLines2d(seg.boundingbox, SegmentClass.VanLineColor, 0.5f);
            }
            for (int g = 0; g < seg.boundingbox.guideLines.Count; ++g)
            {
                foreach (GuideLine line in seg.boundingbox.guideLines[g])
                {
                    if (!line.active) continue;
                    if (line.isGuide && this.showGuideLineVanishingLine)
                    {
                        this.drawVanishingLines2d(line, SegmentClass.HighlightColor, 0.5f);
                    }
                }
            }
        }//drawVanishingGuide2d

        private void drawAnimatedLine()
        {
            if (this.animatedLine == null) return;
            this.drawLines3D(this.animatedLine.u3, this.animatedLine.v3, SegmentClass.HighlightColor, 4.0f);
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

        private void drawPoints2d(Vector2d[] points, Color c, float pointSize)
        {
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glPointSize(pointSize);
            Gl.glBegin(Gl.GL_POINTS);
            foreach (Vector2d v in points)
            {
                Gl.glVertex2dv(v.ToArray());
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_POINT_SMOOTH);
        }

        private void drawPoints3d(Vector3d[] points, Color c, float pointSize)
        {
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glPointSize(pointSize);
            Gl.glBegin(Gl.GL_POINTS);
            foreach (Vector3d v in points)
            {
                Gl.glVertex3dv(v.ToArray());
            }
            Gl.glEnd();

            Gl.glDisable(Gl.GL_POINT_SMOOTH);
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
                    case 0:
                        this.drawLines3D(line.u3, line.v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    case 1:
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
                    case 0:
                        this.drawLines3D(line.u3, line.v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    case 1:
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
                case 0:
                    {
                        this.drawLines3D(line.vanLines[0][0].u3, line.vanLines[0][0].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines3D(line.vanLines[0][1].u3, line.vanLines[0][1].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines3D(line.vanLines[1][0].u3, line.vanLines[1][0].v3, SegmentClass.VanLineColor, 1.0f);
                        this.drawLines3D(line.vanLines[1][1].u3, line.vanLines[1][1].v3, SegmentClass.VanLineColor, 1.0f);
                        break;
                    }
                case 1:
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

        private void drawVanishingLines2d(Box box, Color c, float width)
        {
            if (this.showVanishingRay1)
            {
                for (int i = 0; i < vp1.Length; ++i)
                {
                    Line3d line = box.vanLines[0][vp1[i]];
                    switch (this.vanishinglineDrawType)
                    {
                        case 0:
                            this.drawLines2D(line.u2, line.v2, c, width);
                            break;
                        case 1:
                            this.drawDashedLines2D(line.u2, line.v2, c, width);
                            break;
                        default:
                            break;
                    }
                }
            }
            if (this.showVanishingRay2)
            {
                for (int i = 0; i < vp2.Length; ++i)
                {
                    Line3d line = box.vanLines[1][vp2[i]];
                    switch (this.vanishinglineDrawType)
                    {
                        case 0:
                            this.drawLines2D(line.u2, line.v2, c, width);
                            break;
                        case 1:
                            this.drawDashedLines2D(line.u2, line.v2, c, width);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void drawVanishingLines2d(GuideLine line, Color c, float width)
        {
            switch (this.vanishinglineDrawType)
            {
                case 0:
                    {
                        if (this.showVanishingRay1)
                        {
                            this.drawLines2D(line.vanLines[0][0].u2, line.vanLines[0][0].v2, c, width);
                            this.drawLines2D(line.vanLines[0][1].u2, line.vanLines[0][1].v2, c, width);
                        }
                        if (this.showVanishingRay2)
                        {
                            this.drawLines2D(line.vanLines[1][0].u2, line.vanLines[1][0].v2, c, width);
                            this.drawLines2D(line.vanLines[1][1].u2, line.vanLines[1][1].v2, c, width);
                        }
                        break;
                    }
                case 1:
                    {
                        if (this.showVanishingRay1)
                        {
                            this.drawDashedLines2D(line.vanLines[0][0].u2, line.vanLines[0][0].v2, c, width);
                            this.drawDashedLines2D(line.vanLines[0][1].u2, line.vanLines[0][1].v2, c, width);
                        }
                        if (this.showVanishingRay2)
                        {
                            this.drawDashedLines2D(line.vanLines[1][0].u2, line.vanLines[1][0].v2, c, width);
                            this.drawDashedLines2D(line.vanLines[1][1].u2, line.vanLines[1][1].v2, c, width);
                        }
                        break;
                    }
                default:
                    break;
            }   
            
        }

        private void drawLines2D(List<Vector2d> points, Color c, float linewidth)
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(linewidth);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3ub(c.R, c.G, c.B);
            for (int i = 0; i < points.Count - 1;++i )
            {
                Gl.glVertex2dv(points[i].ToArray());
                Gl.glVertex2dv(points[i+1].ToArray());
            }
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glLineWidth(1.0f);
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

        private void drawLines3D(List<Vector3d> points, Color c, float linewidth)
        {

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(linewidth);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glBegin(Gl.GL_LINES);
            foreach (Vector3d p in points)
            {
                Gl.glVertex3dv(p.ToArray());
            }
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glLineWidth(1.0f);
        }

        private void drawLines3D(Vector3d v1, Vector3d v2, Color c, float linewidth)
        {

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

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

        private void drawQuadTransparent3d(Plane q, Color c)
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            // face
            Gl.glColor4ub(c.R, c.G, c.B, 100);
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
                this.drawLines3D(q.points[i], q.points[(i + 1) % q.points.Length], c, (float)SegmentClass.StrokeSize * 1.5f);
            }
        }

        public static float[] matAmbient = { 0.1f, 0.1f, 0.1f, 1.0f };
        public static float[] matDiffuse = { 0.4f, 0.4f, 0.4f, 1.0f };
        public static float[] matSpecular = { 0.5f, 0.5f, 0.5f, 1.0f };
        public static float[] shine = { 7.0f };

        private static void SetDefaultLight()
        {

            float[] col1 = new float[4]  { 0.7f, 0.7f, 0.7f, 1.0f };
            float[] col2 = new float[4] { 0.8f, 0.7f, 0.7f, 1.0f };
            float[] col3 = new float[4] { 0, 0, 0, 1 };//{ 1.0f, 1.0f, 1.0f, 1.0f };

            float[] pos_1 = {10, 0,0};// { 0, -5, 10.0f };
            float[] pos_2 = {0, 10, 0};// { 0, 5, -10.0f };
            float[] pos_3 = {0,0,10};//{ -5, 5, -10.0f };
            float[] pos_4 = { -10, 0, 0 };// { 0, -5, 10.0f };
            float[] pos_5 = { 0, -10, 0 };// { 0, 5, -10.0f };
            float[] pos_6 = { 0, 0, -10 };//{ -5, 5, -10.0f };

            float[] intensity = {0.5f, 0.5f, 0.5f};
            //Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, pos_1);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, col1);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_INTENSITY, intensity);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, col1);

            Gl.glEnable(Gl.GL_LIGHT1);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, pos_2);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, col1);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_INTENSITY, intensity);
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_SPECULAR, col1);

            Gl.glEnable(Gl.GL_LIGHT2);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, pos_3);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, col1);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_SPECULAR, col1);
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_INTENSITY, intensity);

            //Gl.glEnable(Gl.GL_LIGHT3);
            //Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, pos_4);
            //Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, col1);
            //Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_SPECULAR, col1);
            //Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_INTENSITY, intensity);


            Gl.glEnable(Gl.GL_LIGHT4);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_POSITION, pos_5);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_DIFFUSE, col1);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_SPECULAR, col1);
            Gl.glLightfv(Gl.GL_LIGHT4, Gl.GL_INTENSITY, intensity);

            Gl.glEnable(Gl.GL_LIGHT5);
            Gl.glLightfv(Gl.GL_LIGHT5, Gl.GL_POSITION, pos_6);
            Gl.glLightfv(Gl.GL_LIGHT5, Gl.GL_DIFFUSE, col1);
            Gl.glLightfv(Gl.GL_LIGHT5, Gl.GL_SPECULAR, col1);
            Gl.glLightfv(Gl.GL_LIGHT5, Gl.GL_INTENSITY, intensity);

        }
        public void AddLight(Vector3d pos, Color col)
        {
            int lightID = lightPositions.Count + 16387;
            float[] posA = new float[4] { (float)pos.x, (float)pos.y, (float)pos.z, 0.0f };
            lightPositions.Add(posA);
            float[] colA = new float[4] { col.R / 255.0f, col.G / 255.0f, col.B / 255.0f, 1.0f };
            lightcolors.Add(colA);
            lightIDs.Add(lightID);
        }
        private static void SetDefaultMaterial()
        {
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, matAmbient);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, matDiffuse);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, matSpecular);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, shine);

            


        }
        public static List<float[]> lightPositions = new List<float[]>();
        public static List<float[]> lightcolors = new List<float[]>();
        public static List<int> lightIDs = new List<int>();
        private static void SetAdditionalLight()
        {
            if (lightPositions.Count == 0)
            {
                return;
            }
            for (int i = 0; i < lightPositions.Count; ++i)
            {
                Gl.glEnable(lightIDs[i]);
                Gl.glLightfv(lightIDs[i], Gl.GL_POSITION, lightPositions[i]);
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, lightcolors[i]);
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, lightcolors[i]);
            }
        }
        public static void DrawCircle2(Vector2d p, Color c, float radius)
        {
            Gl.glEnable(Gl.GL_BLEND);
            //	Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glColor4ub(c.R, c.G, c.B, 50);

            int nsample = 50;
            double delta = Math.PI * 2 / nsample;

            Gl.glLineWidth(1.0f);
            Gl.glBegin(Gl.GL_LINES);
            for (int i = 0; i < nsample; ++i)
            {
                double theta1 = i * delta;
                double x1 = p.x + radius * Math.Cos(theta1), y1 = p.y + radius * Math.Sin(theta1);
                double theta2 = (i + 1) * delta;
                double x2 = p.x + radius * Math.Cos(theta2), y2 = p.y + radius * Math.Sin(theta2);
                Gl.glVertex2d(x1, y1);
                Gl.glVertex2d(x2, y2);
            }
            Gl.glEnd();
            Gl.glLineWidth(1.0f);

            Gl.glBegin(Gl.GL_POLYGON);
            for (int i = 0; i < nsample; ++i)
            {
                double theta1 = i * delta;
                double x1 = p.x + radius * Math.Cos(theta1), y1 = p.y + radius * Math.Sin(theta1);
                Gl.glVertex2d(x1, y1);
            }
            Gl.glEnd();

            //	Gl.glDisable(Gl.GL_BLEND);
        }

        private void DrawLight()
        {
            for (int i = 0; i < lightPositions.Count; ++i)
            {
                Vector3d pos3 = new Vector3d(lightPositions[i][0],
                    lightPositions[i][1],
                    lightPositions[i][2]);
                Vector3d pos2 = this.camera.Project(pos3.x, pos3.y, pos3.z);
                DrawCircle2(new Vector2d(pos2.x, pos2.y), Color.Yellow, 0.2f);
            }
        }

        // draw mesh
        public void drawMeshFace(Mesh m, Color c, bool useMeshColor)
        {
            if (m == null) return;


            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glDisable(Gl.GL_CULL_FACE);
            //Gl.glEnable(Gl.GL_COLOR_MATERIAL);

            //Gl.glEnable(Gl.GL_LIGHTING);
            //Gl.glEnable(Gl.GL_NORMALIZE);
            //Gl.glPolygonOffset(5.0f, 30.0f);
            //Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL);
            //Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
            float[] mat_a = new float[4] { c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, 1.0f };

            float[] ka = { 0.1f, 0.05f, 0.0f, 1.0f };
            float[] kd = { .9f, .6f, .2f, 1.0f };
            float[] ks = { 0, 0, 0, 0 };//{ .2f, .2f, .2f, 1.0f };
            float[] shine = { 1.0f };
            Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, mat_a);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, mat_a);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, ks);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, shine);

            

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_NORMALIZE);

            //Vector3d newEye = (this.currModelTransformMatrix.Transpose() * new Vector4d(this.eye, 1)).ToVector3D();

            
            if (useMeshColor)
            {
                Gl.glColor3ub(GLViewer.ModelColor.R, GLViewer.ModelColor.G, GLViewer.ModelColor.B);
                for (int i = 0, j = 0; i < m.FaceCount; ++i, j += 3)
                {
                    int vidx1 = m.FaceVertexIndex[j];
                    int vidx2 = m.FaceVertexIndex[j + 1];
                    int vidx3 = m.FaceVertexIndex[j + 2];
                    Vector3d v1 = new Vector3d(
                        m.VertexPos[vidx1 * 3], m.VertexPos[vidx1 * 3 + 1], m.VertexPos[vidx1 * 3 + 2]);
                    Vector3d v2 = new Vector3d(
                        m.VertexPos[vidx2 * 3], m.VertexPos[vidx2 * 3 + 1], m.VertexPos[vidx2 * 3 + 2]);
                    Vector3d v3 = new Vector3d(
                        m.VertexPos[vidx3 * 3], m.VertexPos[vidx3 * 3 + 1], m.VertexPos[vidx3 * 3 + 2]);
                    Color fc = Color.FromArgb(m.FaceColor[i * 4 + 3], m.FaceColor[i * 4], m.FaceColor[i * 4 + 1], m.FaceColor[i * 4 + 2]);
                    Gl.glColor4ub(fc.R, fc.G, fc.B, fc.A);
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    Vector3d centroid = (v1 + v2 + v3) / 3;
                    Vector3d normal = new Vector3d(m.FaceNormal[i * 3], m.FaceNormal[i * 3 + 1], m.FaceNormal[i * 3 + 2]);
                    //if ((centroid - newEye).Dot(normal) > 0)
                    //{
                    //    normal *= -1.0;
                    //}
                    //normal *= -1;
                    Gl.glNormal3dv(normal.ToArray());
                    //Gl.glNormal3d(m.VertexNormal[vidx1 * 3], m.VertexNormal[vidx1 * 3 + 1], m.VertexNormal[vidx1 * 3 + 2]);
                    Gl.glVertex3d(v1.x, v1.y, v1.z);
                    //Gl.glNormal3d(m.VertexNormal[vidx2 * 3], m.VertexNormal[vidx2 * 3 + 1], m.VertexNormal[vidx2 * 3 + 2]);
                    Gl.glVertex3d(v2.x, v2.y, v2.z);
                    //Gl.glNormal3d(m.VertexNormal[vidx3 * 3], m.VertexNormal[vidx3 * 3 + 1], m.VertexNormal[vidx3 * 3 + 2]);
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
                    int vidx1 = m.FaceVertexIndex[j];
                    int vidx2 = m.FaceVertexIndex[j + 1];
                    int vidx3 = m.FaceVertexIndex[j + 2];
                    Vector3d v1 = new Vector3d(
                        m.VertexPos[vidx1 * 3], m.VertexPos[vidx1 * 3 + 1], m.VertexPos[vidx1 * 3 + 2]);
                    Vector3d v2 = new Vector3d(
                        m.VertexPos[vidx2 * 3], m.VertexPos[vidx2 * 3 + 1], m.VertexPos[vidx2 * 3 + 2]);
                    Vector3d v3 = new Vector3d(
                        m.VertexPos[vidx3 * 3], m.VertexPos[vidx3 * 3 + 1], m.VertexPos[vidx3 * 3 + 2]);
                    Gl.glNormal3d(m.FaceNormal[i * 3], m.FaceNormal[i * 3 + 1], m.FaceNormal[i * 3 + 2]);
                    //Gl.glNormal3d(m.VertexNormal[vidx1 * 3], m.VertexNormal[vidx1 * 3 + 1], m.VertexNormal[vidx1 * 3 + 2]);
                    Gl.glVertex3d(v1.x, v1.y, v1.z);
                    //Gl.glNormal3d(m.VertexNormal[vidx2 * 3], m.VertexNormal[vidx2 * 3 + 1], m.VertexNormal[vidx2 * 3 + 2]);
                    Gl.glVertex3d(v2.x, v2.y, v2.z);
                    //Gl.glNormal3d(m.VertexNormal[vidx3 * 3], m.VertexNormal[vidx3 * 3 + 1], m.VertexNormal[vidx3 * 3 + 2]);
                    Gl.glVertex3d(v3.x, v3.y, v3.z);
                }
                Gl.glEnd();
            }

            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_POINT_SMOOTH);
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDepthMask(Gl.GL_TRUE);

            Gl.glDisable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_LIGHT0);
            //Gl.glDisable(Gl.GL_LIGHT1);
            //Gl.glDisable(Gl.GL_LIGHT2);
            //Gl.glDisable(Gl.GL_LIGHT3);
            //Gl.glDisable(Gl.GL_LIGHT4);
            //Gl.glDisable(Gl.GL_LIGHT5);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glDisable(Gl.GL_COLOR_MATERIAL);
        }

        private void drawMeshEdge(Mesh m)
        {
            if (m == null) return;
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glColor3ub(GLViewer.ColorSet[1].R, GLViewer.ColorSet[1].G, GLViewer.ColorSet[1].B);
            Gl.glBegin(Gl.GL_LINES);
            for (int i = 0; i < m.Edges.Length; ++i)
            {
                int fromIdx = m.Edges[i].FromIndex;
                int toIdx = m.Edges[i].ToIndex;
                Gl.glVertex3d(m.VertexPos[fromIdx * 3],
                    m.VertexPos[fromIdx * 3 + 1],
                    m.VertexPos[fromIdx * 3 + 2]);
                Gl.glVertex3d(m.VertexPos[toIdx * 3],
                    m.VertexPos[toIdx * 3 + 1],
                    m.VertexPos[toIdx * 3 + 2]);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            //Gl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
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

        public void drawBoundingboxWithoutBlend(Box box, Color c)
        {
            if (box == null) return;
            for (int i = 0; i < box.planes.Length; ++i)
            {
                // face
                Gl.glDisable(Gl.GL_BLEND);
                Gl.glColor4ub(c.R, c.G, c.B, c.A);
                Gl.glBegin(Gl.GL_POLYGON);
                for (int j = 0; j < 4; ++j)
                {
                    Gl.glVertex3dv(box.planes[i].points[j].ToArray());
                }
                Gl.glEnd();
            }
        }// drawBoundingbox

        

        // draw

        public void drawTriMeshShaded3D_hiddenLine(Stroke stroke, bool highlight)
        {
            //Gl.glPushAttrib(Gl.GL_COLOR_BUFFER_BIT);

            int iMultiSample = 0;
            int iNumSamples = 0;
            Gl.glGetIntegerv(Gl.GL_SAMPLE_BUFFERS, out iMultiSample);
            Gl.glGetIntegerv(Gl.GL_SAMPLES, out iNumSamples);
            if (iNumSamples == 0)
            {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

                Gl.glEnable(Gl.GL_POLYGON_SMOOTH);
                Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
                Gl.glEnable(Gl.GL_LINE_SMOOTH);
                Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);


                Gl.glEnable(Gl.GL_BLEND);
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
                if (visibleTriangleVertices[this.visibilityIdx++] == 0) continue;
                int vidx1 = stroke.faceIndex[j];
                int vidx2 = stroke.faceIndex[j + 1];
                int vidx3 = stroke.faceIndex[j + 2];

                int ipt = (i + stroke.ncapoints) / 2;
                if (ipt < 0) ipt = 0;
                if (ipt >= stroke.strokePoints.Count) ipt = stroke.strokePoints.Count - 1;
                StrokePoint pt = stroke.strokePoints[ipt];

                byte opa = pt.opacity;
                //opa = this.triangleDepth[this.visibilityIdx++];
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
                Gl.glVertex3dv(stroke.meshVertices3d[vidx1].ToArray());
                Gl.glVertex3dv(stroke.meshVertices3d[vidx2].ToArray());
                Gl.glVertex3dv(stroke.meshVertices3d[vidx3].ToArray());
                Gl.glEnd();
            }


            if (iNumSamples == 0)
            {
                Gl.glDisable(Gl.GL_BLEND);
                Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
            }
            else
            {
                Gl.glDisable(Gl.GL_MULTISAMPLE);
            }

            //Gl.glPopAttrib();
        }

        public void drawTriMeshShaded2D(Stroke stroke, bool highlight, bool useOcclusion)
        {
            Gl.glPushAttrib(Gl.GL_COLOR_BUFFER_BIT);

            int iMultiSample = 0;
            int iNumSamples = 0;
            Gl.glGetIntegerv(Gl.GL_SAMPLE_BUFFERS, out iMultiSample);
            Gl.glGetIntegerv(Gl.GL_SAMPLES, out iNumSamples);
            if (iNumSamples == 0)
            {
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

                Gl.glEnable(Gl.GL_POLYGON_SMOOTH);
                Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
                Gl.glEnable(Gl.GL_LINE_SMOOTH);
                Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

                Gl.glEnable(Gl.GL_BLEND);
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
                Gl.glBegin(Gl.GL_POLYGON);
                Gl.glVertex2dv(stroke.meshVertices2d[vidx1].ToArray());
                Gl.glVertex2dv(stroke.meshVertices2d[vidx2].ToArray());
                Gl.glVertex2dv(stroke.meshVertices2d[vidx3].ToArray());
                Gl.glEnd();
            }
            


            if (iNumSamples == 0)
            {
                Gl.glDisable(Gl.GL_BLEND);
                Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
            }
            else
            {
                Gl.glDisable(Gl.GL_MULTISAMPLE);
            }

            Gl.glPopAttrib();
        } // drawTriMeshShaded2D

        public void drawTriMeshTextured2D(Stroke stroke, bool useOcclusion)
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
            //Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, tex_id);

            for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            {
                int j1 = stroke.faceIndex[j];
                int j2 = stroke.faceIndex[j + 1];
                int j3 = stroke.faceIndex[j + 2];
                Vector2d pos1 = stroke.meshVertices2d[j1];
                Vector2d pos2 = stroke.meshVertices2d[j2];
                Vector2d pos3 = stroke.meshVertices2d[j3];

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
                    Gl.glVertex2dv(pos1.ToArray());
                    Gl.glVertex2d(0, 1);
                    Gl.glVertex2dv(pos2.ToArray());
                    Gl.glTexCoord2d(1, 1);
                    Gl.glVertex2dv(pos3.ToArray());
                }
                else
                {
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex2dv(pos1.ToArray());
                    Gl.glTexCoord2d(1, 1);
                    Gl.glVertex2dv(pos3.ToArray());
                    Gl.glTexCoord2d(1, 0);
                    Gl.glVertex2dv(pos2.ToArray());
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

        public void drawSegmentContour(Color c, float width)
        {
            //if (this.currSegmentClass == null) return;

            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.contourPoints == null) continue;
            //    for (int i = 0; i < seg.contourPoints.Count - 2; i += 2)
            //    {
            //        Vector3d p1 = seg.contourPoints[i];
            //        Vector3d p2 = seg.contourPoints[i + 1];
            //        this.drawLines3D(p1, p2, c, width);
            //    }
            //}
            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.contourPoints == null) continue;
            //    this.drawPoints3d(seg.contourPoints.ToArray(), c, width);
            //}

            foreach (Segment seg in this.currSegmentClass.segments)
            {
                this.drawContours(seg.contourPoints, width, c);
            }

        }//drawSegmentContour

        public void drawSegmentSilhouette(Color c, float width)
        {
            if (this.currSegmentClass == null) return;
            
            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.silhouettePoints == null) continue;
            //    for (int i = 0; i < seg.silhouettePoints.Count - 2; i += 2)
            //    {
            //        Vector3d p1 = seg.silhouettePoints[i];
            //        Vector3d p2 = seg.silhouettePoints[i + 1];
            //        this.drawLines3D(p1, p2, c, width);
            //    }
            //}

            //Gl.glBegin(Gl.GL_POINTS);
            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.silhouettePoints == null) continue;
            //    this.drawPoints3d(seg.silhouettePoints.ToArray(), c, width);
            //}
            //Gl.glEnd();

            foreach (Segment seg in this.currSegmentClass.segments)
            {
                this.drawContours(seg.silhouettePoints, width, c);
            }

        }//drawSegmentSilhouette

        public void drawSegmentSuggestiveContour(Color c, float width)
        {
            if (this.currSegmentClass == null) return;
            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.suggestiveContourPoints == null) continue;
            //    for (int i = 0; i < seg.suggestiveContourPoints.Count - 2; i += 2)
            //    {
            //        Vector3d p1 = seg.suggestiveContourPoints[i];
            //        Vector3d p2 = seg.suggestiveContourPoints[i + 1];
            //        this.drawLines3D(p1, p2, c, width);
            //    }
            //}

            //Gl.glBegin(Gl.GL_POINTS);
            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.suggestiveContourPoints == null) continue;
            //    this.drawPoints3d(seg.suggestiveContourPoints.ToArray(), c, width);
            //}
            //Gl.glEnd();

            foreach (Segment seg in this.currSegmentClass.segments)
            {
                this.drawContours(seg.suggestiveContourPoints, width, c);
            }
        }//drawSegmentSuggestiveContour

        public void drawSegmentApparentRige(Color c, float width)
        {
            if (this.currSegmentClass == null) return;

            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.ridgePoints == null) continue;
            //    for (int i = 0; i < seg.ridgePoints.Count - 2; i += 2)
            //    {
            //        Vector3d p1 = seg.ridgePoints[i];
            //        Vector3d p2 = seg.ridgePoints[i + 1];
            //        this.drawLines3D(p1, p2, c, width);
            //    }
            //}
            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.ridgePoints == null) continue;
            //    this.drawPoints3d(seg.ridgePoints.ToArray(), c, width);
            //}
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                this.drawContours(seg.ridgePoints, width, c);
            }
        }//drawSegmentApparentRige

        public void drawSegmentBoundary(Color c, float width)
        {
            if (this.currSegmentClass == null) return;

            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.boundaryPoints == null) continue;
            //    for (int i = 0; i < seg.boundaryPoints.Count - 2; i += 2)
            //    {
            //        Vector3d p1 = seg.boundaryPoints[i];
            //        Vector3d p2 = seg.boundaryPoints[i + 1];
            //        this.drawLines3D(p1, p2, c, width);
            //    }
            //}
            //foreach (Segment seg in this.currSegmentClass.segments)
            //{
            //    if (!seg.active || seg.boundaryPoints == null) continue;
            //    this.drawPoints3d(seg.boundaryPoints.ToArray(), c, width);
            //}
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                this.drawContours(seg.boundaryPoints, width, c);
            }
        }//drawSegmentBoundary

        public void drawContours(List<Vector3d> points, float width, Color c)
        {
            if (points == null) return;

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthMask(Gl.GL_FALSE);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(width);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glBegin(Gl.GL_LINES);

            for (int i = 0; i < points.Count; i++)
            {
                Vector3d p1 = points[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            Gl.glEnd();

            Gl.glPointSize(width * 0.5f);
            Gl.glBegin(Gl.GL_POINTS);
            for (int i = 0; i < points.Count; i++)
            {
                Vector3d p1 = points[i];
                Gl.glVertex3dv(p1.ToArray());
            }
            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_POINT_SMOOTH);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
        }

        public void drawContourPoints()
        {
            if (this.contourPoints == null) return;

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthMask(Gl.GL_FALSE);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glLineWidth(10.0f);
            Gl.glColor3ub(253, 141, 60);
            Gl.glBegin(Gl.GL_LINES);

            for (int i = 0; i < this.contourPoints.Count; i++)
            {
                Vector3d p1 = this.contourPoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }


            for (int i = 0; i < this.suggestiveContourPoints.Count; i++)
            {
                Vector3d p1 = this.suggestiveContourPoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            for (int i = 0; i < this.silhouettePoints.Count; i++)
            {
                Vector3d p1 = this.silhouettePoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            for (int i = 0; i < this.apparentRidgePoints.Count; i++)
            {
                Vector3d p1 = this.apparentRidgePoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            for (int i = 0; i < this.boundaryPoints.Count; i++)
            {
                Vector3d p1 = this.boundaryPoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            Gl.glEnd();

            Gl.glPointSize(10.0f);
            Gl.glBegin(Gl.GL_POINTS);
            for (int i = 0; i < this.contourPoints.Count; i++)
            {
                Vector3d p1 = this.contourPoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }


            for (int i = 0; i < this.suggestiveContourPoints.Count; i++)
            {
                Vector3d p1 = this.suggestiveContourPoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            for (int i = 0; i < this.silhouettePoints.Count; i++)
            {
                Vector3d p1 = this.silhouettePoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            for (int i = 0; i < this.apparentRidgePoints.Count; i++)
            {
                Vector3d p1 = this.apparentRidgePoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            for (int i = 0; i < this.boundaryPoints.Count; i++)
            {
                Vector3d p1 = this.boundaryPoints[i];
                Gl.glVertex3dv(p1.ToArray());
            }

            Gl.glEnd();

            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glDisable(Gl.GL_POINT_SMOOTH);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
        }

        public void drawContourLine()
        {
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            if (this.currSegmentClass != null)
            {
                foreach (Segment seg in this.currSegmentClass.segments)
                {
                    if (seg.contours == null) continue;
                    foreach (List<int> vids in seg.contours)
                    {
                        for (int i = 0; i < vids.Count - 1; ++i)
                        {
                            Vector3d p1 = seg.mesh.getVertexPos(vids[i]);
                            Vector3d p2 = seg.mesh.getVertexPos(vids[i + 1]);
                            this.drawLines3D(p1, p2, SegmentClass.StrokeColor, 2.0f);
                        }
                    }
                }
            }
            if (this.currMeshClass != null && this.contourLines != null)
            {
                foreach (List<Vector3d> points in this.contourLines)
                {
                    for (int i = 0; i < points.Count - 1; ++i)
                    {
                        this.drawLines3D(points[i], points[i + 1], Color.Blue, 2.0f);
                    }
                }
            }
            Gl.glDisable(Gl.GL_DEPTH_TEST);
        }//drawContourLine


        public void drawSharpEdges()
        {
            if (this.sharpEdges == null)
            {
                return;
            }

            for (int i = 0; i < this.sharpEdges.Count; i += 2)
            {
                Vector3d p1 = this.sharpEdges[i];
                Vector3d p2 = this.sharpEdges[i + 1];
                this.drawLines3D(p1, p2, Color.Pink, 2.0f);
            }

        }//drawContourLine


    }// GLViewer
}// namespace
