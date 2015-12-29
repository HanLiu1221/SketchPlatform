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
            Viewing, VertexSelection, EdgeSelection, FaceSelection, ComponentSelection, NONE
        }

        private bool drawVertex = false;
        private bool drawEdge = false;
        private bool drawFace = true;
        private bool isDrawAxes = false;
        private bool isDrawQuad = false;

        public bool showBoundingbox = true;
        public bool showMesh = false;
        public bool showSketchyEdges = true;
        public bool showGuideLines = true;

        private float[] material = { 0.62f, 0.74f, 0.85f, 1.0f };
        private float[] ambient = { 0.2f, 0.2f, 0.2f, 1.0f };
        private float[] diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] specular = { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] position = { 1.0f, 1.0f, 1.0f, 0.0f };

        /******************** Variables ********************/
        private UIMode currUIMode = UIMode.Viewing;
        private Matrix4d currModelTransformMatrix = Matrix4d.IdentityMatrix();
        private Matrix4d modelTransformMatrix = Matrix4d.IdentityMatrix();
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
        private Vector3d[] axes = { new Vector3d(-1.2, 0, 0), new Vector3d(1.2, 0, 0),
                                      new Vector3d(1, 0.2, 0), new Vector3d(1.2, 0, 0), 
                                      new Vector3d(1, -0.2, 0), new Vector3d(1.2, 0, 0), 
                                      new Vector3d(0, -1.2, 0), new Vector3d(0, 1.2, 0), 
                                      new Vector3d(-0.2, 1, 0), new Vector3d(0, 1.2, 0),
                                      new Vector3d(0.2, 1, 0), new Vector3d(0, 1.2, 0),
                                      new Vector3d(0, 0, -1.2), new Vector3d(0, 0, 1.2),
                                      new Vector3d(-0.2, 0, 1), new Vector3d(0, 0, 1.2),
                                      new Vector3d(0.2, 0, 1), new Vector3d(0, 0, 1.2)};
        private Quad2d highlightQuad;
        private int[] meshStats;
        private int[] segStats;
        private Cube cameraBox;
        private Camera camera;
        private Shader shader;
        public static uint pencilTextureId, crayonTextureId, inkTextureId, waterColorTextureId, charcoalTextureId,
            brushTextureId;
        public int currBoxIdx = -1;
        private bool drawShadedOrTexturedStroke = true;

        //########## static vars ##########//
        public static Color[] ColorSet;
        static public Color ModelColor;

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
            //System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

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
            this.segmentClasses.Add(sc);
            this.currSegmentClass = sc;
            this.calculateSketchMesh();
            segStats = new int[2];
            segStats[0] = sc.segments.Count;
        }// loadSegments

        public void loadJSONFile(string jsonFile)
        {
            SegmentClass sc = new SegmentClass();
            Matrix4d m = sc.DeserializeJSON(jsonFile);
            this.segmentClasses.Add(sc);
            this.currSegmentClass = sc;
            this.calculateSketchMesh();
            segStats = new int[2];
            segStats[0] = sc.segments.Count;
            this.setGuideLineColor(Color.FromArgb(222, 45, 38));
        }// loadJSONFile

        public int[] getMeshStatistics()
        {
            return this.meshStats;
        }

        public int[] getSegmentStatistics()
        {
            return this.segStats;
        }

        public void setStrokeSize(int size)
        {
            SegmentClass.strokeSize = size;
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine edge in allLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.setStrokeSize(size);
                    }
                }
            }
            this.calculateSketchMesh();
        }//setStrokeSize

        public void setGuideLineColor(Color c)
        {
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                foreach (GuideLine edge in seg.boundingbox.guideLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        stroke.stokeColor = c;
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
                        stroke.stokeColor = c;
                    }
                }
            }
        }//setStrokeSize

        public void setGuideLineStyle(int idx)
        {
            this.currSegmentClass.ChangeGuidelineStyle(idx);
            this.calculateSketchMesh();
        }

        public void setStrokeStyle(int idx)
        {
            this.currSegmentClass.setStrokeStyle(idx);
            this.calculateSketchMesh();
            this.drawShadedOrTexturedStroke = this.currSegmentClass.shadedOrTexture();
        }//setStrokeStyle

        public void nextBox()
        {
            this.currBoxIdx = (this.currBoxIdx + 1) % this.currSegmentClass.segments.Count;
        }

        public void prevBox()
        {
            this.currBoxIdx = (this.currBoxIdx - 1 + this.currSegmentClass.segments.Count)
                % this.currSegmentClass.segments.Count;
        }

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
            this.UpdateCamera();
            foreach (Segment seg in this.currSegmentClass.segments)
            {
                List<GuideLine> allLines = seg.boundingbox.getAllLines();
                foreach (GuideLine edge in allLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        //Vector3d u3 = (this.modelTransformMatrix * new Vector4d(stroke.u3, 1)).ToVector3D();
                        //Vector3d v3 = (this.modelTransformMatrix * new Vector4d(stroke.v3, 1)).ToVector3D();
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
            this.Refresh();
        }

        private void UpdateCamera()
        {
            Matrix4d m = this.currModelTransformMatrix;
            double[] ballmat =  m.Transpose().ToArray();	// matrix applied with arcball
            //this.camera.Update();
            this.camera.SetBallMatrix(ballmat);
            this.camera.Update();
            this.camera.SetModelViewMatrix(this.modelTransformMatrix.Transpose().ToArray());
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
                case System.Windows.Forms.Keys.R:
                    {
                        this.resetView();
                        break;
                    }
                default: 
                    break;
            }
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
            if (w >= h)
            {
                Glu.gluOrtho2D(-1.0 * aspect, 1.0 * aspect, -1.0, 1.0);
            }
            else
            {
                Glu.gluOrtho2D(-1.0, 1.0, -1.0 * aspect, 1.0 * aspect);
            }
            //Glu.gluPerspective(50, 1.0, -1, 3);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

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

            Gl.glPopMatrix();
            Gl.glPopMatrix();
        }

        private void Draw3D()
        {
            this.setViewMatrix();

            Matrix4d m = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glMultMatrixd(m.Transpose().ToArray());
            //this.modelTransformMatrix = m.Transpose();

            /***** Draw *****/
            //clearScene();

            if (this.isDrawAxes)
            {
                this.drawAxes();
            }

            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL);
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
                    this.drawBoundingbox(seg.boundingbox, seg.color);
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
                foreach(GuideLine edge in seg.boundingbox.guideLines)
                {
                    foreach (Stroke stroke in edge.strokes)
                    {
                        //this.drawLines2D(stroke.u2, stroke.v2, Color.Red);
                        this.drawStrokeMesh2d(stroke);
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
                for(int i = 0; i < seg.boundingbox.edges.Length;++i)
                {
                    GuideLine edge = seg.boundingbox.edges[i];
                    this.drawGuideLineEndpoints(edge, Color.Gray);
                    foreach (Stroke stroke in edge.strokes)
                    {
                        if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                        {
                            this.drawLines3D(stroke.u3, stroke.v3, Color.Gray, 2.0f);
                        }
                        if (this.drawShadedOrTexturedStroke)
                        {
                            this.drawTriMeshShaded3D(stroke, false, false);
                            //this.drawStrokeMesh3d(stroke);
                        }
                        else
                        {
                            this.drawTriMeshTextured3D(stroke, false);
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
                foreach(GuideLine edge in seg.boundingbox.guideLines)
                {
                    this.drawGuideLineEndpoints(edge, Color.Red);
                    //this.drawLines3D(edge.u, edge.v, Color.Red, 2.0f);
                    foreach (Stroke stroke in edge.strokes)
                    {
                        if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                        {
                            this.drawLines3D(stroke.u3, stroke.v3, Color.Pink, 2.0f);
                        }
                        if (this.drawShadedOrTexturedStroke)
                        {
                            this.drawTriMeshShaded3D(stroke, false, false);
                        }
                        else
                        {
                            this.drawTriMeshTextured3D(stroke, false);
                        }
                    }
                }
            }
        }// drawSketchyLines2D

        private void DrawHighlight3D()
        {
            if (this.currBoxIdx == -1) return;
            Segment seg = this.currSegmentClass.segments[this.currBoxIdx];
            this.drawBoundingbox(seg.boundingbox, Color.Salmon);

            List<GuideLine> allLines = seg.boundingbox.getAllLines();
            foreach (GuideLine edge in allLines)
            {
                this.drawGuideLineEndpoints(edge, Color.Red);
                foreach (Stroke stroke in edge.strokes)
                {
                    if (stroke.meshVertices3d == null || stroke.meshVertices3d.Count == 0)
                    {
                        this.drawLines3D(stroke.u3, stroke.v3, Color.Pink, 2.0f);
                    }
                    if (this.drawShadedOrTexturedStroke)
                    {
                        this.drawTriMeshShaded3D(stroke, true, false);
                        //this.drawStrokeMesh3d(stroke);
                    }
                    else
                    {
                        this.drawTriMeshTextured3D(stroke, false);
                    }
                }
            }
            
        }// DrawHighlight3D

        private void drawGuideLineEndpoints(GuideLine gline, Color c)
        {
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glPointSize(6.0f);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glVertex3dv(gline.u.ToArray());
            Gl.glVertex3dv(gline.v.ToArray());
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

            Gl.glColor4ub(stroke.stokeColor.R, stroke.stokeColor.G, stroke.stokeColor.B, stroke.stokeColor.A);

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

        private void drawStrokeMesh2d(Stroke stroke)
        {
            if(stroke.meshVertices2d == null || stroke.faceIndex == null)
            {
                return;
            }

            //Gl.glColor3ub(0, 255, 0);
            //Gl.glPointSize(2.0f);
            //Gl.glBegin(Gl.GL_POINTS);
            //foreach (StrokePoint p in stroke.strokePoints)
            //{
            //    Gl.glVertex2dv(p.pos2.ToArray());
            //}
            //Gl.glEnd();

            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glColor4ub(stroke.stokeColor.R, stroke.stokeColor.G, stroke.stokeColor.B, stroke.stokeColor.A);

            Gl.glBegin(Gl.GL_TRIANGLES);
            for (int i = 0, j = 0; i < stroke.FaceCount; ++i, j += 3)
            {
                int vidx1 = stroke.faceIndex[j];
                int vidx2 = stroke.faceIndex[j + 1];
                int vidx3 = stroke.faceIndex[j + 2];
                Gl.glVertex2dv(stroke.meshVertices2d[vidx1].ToArray());
                Gl.glVertex2dv(stroke.meshVertices2d[vidx2].ToArray());
                Gl.glVertex2dv(stroke.meshVertices2d[vidx3].ToArray());
            }
            Gl.glEnd();

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

            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_CULL_FACE);

            //Gl.glLineWidth(2.0f);
        }

        private void drawLines2D(Vector2d v1, Vector2d v2, Color c)
        {
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glVertex2dv(v1.ToArray());
            Gl.glVertex2dv(v2.ToArray());
            Gl.glEnd();
        }

        private void drawLines3D(Vector3d v1, Vector3d v2, Color c, float linewidth)
        {
            Gl.glLineWidth(linewidth);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glVertex3dv(v1.ToArray());
            Gl.glVertex3dv(v2.ToArray());
            Gl.glEnd();
            Gl.glLineWidth(1.0f);
        }

        private void drawAxes()
        {
            // draw axes with arrows
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3ub(255, 0, 0);
            for (int i = 0; i < 6; i += 2)
            {
                Gl.glVertex3dv(axes[i].ToArray());
                Gl.glVertex3dv(axes[i + 1].ToArray());
            }

            Gl.glColor3ub(0, 255, 0);
            for (int i = 6; i < 12; i += 2)
            {
                Gl.glVertex3dv(axes[i].ToArray());
                Gl.glVertex3dv(axes[i + 1].ToArray());
            }

            Gl.glColor3ub(0, 0, 255);
            for (int i = 12; i < 18; i += 2)
            {
                Gl.glVertex3dv(axes[i].ToArray());
                Gl.glVertex3dv(axes[i + 1].ToArray());
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
            Gl.glColor4ub(c.R, c.G, c.B, 100);
            Gl.glBegin(Gl.GL_POLYGON);
            for (int i = 0; i < 4; ++i)
            {
                Gl.glVertex3dv(q.points[i].ToArray());
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_BLEND);
            // lines
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glColor3ub(c.R, c.G, c.B);
            Gl.glLineWidth(2.0f);
            Gl.glBegin(Gl.GL_LINES);
            for (int i = 0; i < 4; ++i)
            {
                Gl.glVertex3dv(q.points[i].ToArray());
                Gl.glVertex3dv(q.points[(i + 1) % 4].ToArray());
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_SMOOTH);
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

        public void drawBoundingbox(Cube cube, Color c)
        {
            if (cube == null) return;
            for (int i = 0; i < cube.planes.Length; ++i)
            {
                this.drawQuad3d(cube.planes[i], c);
            }
            //int j = 0;            
            //Gl.glPointSize(10.0f);            
            //foreach (Vector3d v in cube.points)
            //{
            //    Color cl = GLViewer.ColorSet[j++];
            //    Gl.glColor3ub(cl.R,cl.G,cl.B);
            //    Gl.glBegin(Gl.GL_POINTS);
            //    Gl.glVertex3dv(v.ToArray());
            //    Gl.glEnd();
            //}
        }

        // draw

        public void drawTriMeshShaded3D(Stroke stroke, bool highlight, bool useOcclusion)
        {
            Gl.glPushAttrib(Gl.GL_COLOR_BUFFER_BIT);

            //Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);
            //Gl.glEnable(Gl.GL_LIGHT0);
            //Gl.glEnable(Gl.GL_LIGHTING);
            //Gl.glEnable(Gl.GL_NORMALIZE);


            int iMultiSample = 0;
            int iNumSamples = 0;
            Gl.glGetIntegerv(Gl.GL_SAMPLE_BUFFERS, out iMultiSample);
            Gl.glGetIntegerv(Gl.GL_SAMPLES, out iNumSamples);
            if (iNumSamples == 0)
            {
                //Gl.glEnable(Gl.GL_CULL_FACE);
                Gl.glEnable(Gl.GL_DEPTH_TEST);
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

                Gl.glEnable(Gl.GL_POLYGON_SMOOTH);
                Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
                Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);


                Gl.glEnable(Gl.GL_BLEND);
                Gl.glBlendEquation(Gl.GL_ADD);
                //Gl.glBlendFunc(Gl.GL_SRC_ALPHA_SATURATE, Gl.GL_ONE_MINUS_SRC_ALPHA);
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                Gl.glShadeModel(Gl.GL_SMOOTH);
                Gl.glDepthMask(Gl.GL_FALSE);
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
                    Gl.glColor4ub(stroke.stokeColor.R, stroke.stokeColor.G, stroke.stokeColor.B, (byte)opa);
                }
                else
                {
                    Gl.glColor4ub(240, 59, 32, (byte)opa);
                }
                Gl.glBegin(Gl.GL_TRIANGLES);
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
                Gl.glDepthMask(Gl.GL_TRUE);
                Gl.glDisable(Gl.GL_DEPTH_TEST);
                //Gl.glDisable(Gl.GL_CULL_FACE);
            }
            else
            {
                Gl.glDisable(Gl.GL_MULTISAMPLE);
            }

            //Gl.glDisable(Gl.GL_NORMALIZE);
            //Gl.glDisable(Gl.GL_LIGHTING);
            //Gl.glDisable(Gl.GL_LIGHT0);

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

            for (int j = 0; j < stroke.FaceCount; j += 3)
            {
                int i = j / 3;

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
    }// GLViewer
}// namespace
