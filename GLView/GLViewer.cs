using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Tao.OpenGl;
using Tao.Platform.Windows;
using System.Drawing;
using Geometry;

namespace GraphicsPlatform
{
    public class GLViewer : SimpleOpenGlControl
    {
        /******************** Initialization ********************/
        public GLViewer() 
        {
            this.InitializeComponent();
            this.InitializeContexts();

            this.InitializeVariables();
        }

        private void InitializeComponent() 
        {
            this.SuspendLayout();

            this.Name = "GlViewer";

            this.ResumeLayout(false);
        }

        private void InitializeVariables()
        {
            this.meshClasses = new List<MeshClass>();
            this.currModelTransformMatrix = Matrix4d.IdentityMatrix();
            this.arcBall = new ArcBall(this.Width, this.Height);
            this.InitializeColors();
        }

        private void InitializeColors()
        {
            colorSet = new Color[256];
            colorSet[0] = Color.LightBlue;
        }

        // modes
        public enum UIMode 
        {
            Viewing, VertexSelection, EdgeSelection, FaceSelection, ComponentSelection, NONE
        }

        private bool drawVertex = false;
        private bool drawEdge = false;
        private bool drawFace = true;

        private bool isDrawAxes = false;

        /******************** Variables ********************/
        private UIMode currUIMode = UIMode.Viewing;
        private Matrix4d currModelTransformMatrix;
        private ArcBall arcBall;

        private Vector2d prevMousePos;
        private Vector2d currMousePos;
        private bool isMouseDown = false;
        private List<MeshClass> meshClasses;
        private MeshClass currMeshClass;
        private Vector3d[] axes = { new Vector3d(-1.2, 0, 0), new Vector3d(1.2, 0, 0),
                                      new Vector3d(1, 0.2, 0), new Vector3d(1.2, 0, 0), 
                                      new Vector3d(1, -0.2, 0), new Vector3d(1.2, 0, 0), 
                                      new Vector3d(0, -1.2, 0), new Vector3d(0, 1.2, 0), 
                                      new Vector3d(-0.2, 1, 0), new Vector3d(0, 1.2, 0),
                                      new Vector3d(0.2, 1, 0), new Vector3d(0, 1.2, 0),
                                      new Vector3d(0, 0, -1.2), new Vector3d(0, 0, 1.2),
                                      new Vector3d(-0.2, 0, 1), new Vector3d(0, 0, 1.2),
                                      new Vector3d(0.2, 0, 1), new Vector3d(0, 0, 1.2)};

        //########## static vars ##########//
        public static Color[] colorSet;
        

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

        public void LoadMesh(string filename)
        {
            Mesh m = new Mesh(filename);
            MeshClass mc = new MeshClass(m);
            this.meshClasses.Add(mc);
            this.currMeshClass = mc;
        }

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
            this.Refresh();
        }

        //########## Mouse ##########//

        private void viewMouseDown(System.Windows.Forms.MouseEventArgs e)
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
        }

        private void viewMouseMove(int x, int y)
        {
            if (!this.isMouseDown) return;
            this.arcBall.mouseMove(x, y);
        }

        private void viewMouseUp()
        {
            this.currModelTransformMatrix = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
            this.arcBall.mouseUp();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.currMousePos = new Vector2d(e.X, e.Y);
            this.isMouseDown = true;

            switch (this.currUIMode)
            {
                case UIMode.VertexSelection:
                    {
                        break;
                    }
                case UIMode.EdgeSelection:
                    {
                        break;
                    }
                case UIMode.FaceSelection:
                    {
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
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.prevMousePos = this.currMousePos;
            this.currMousePos = new Vector2d(e.X, e.Y);

            switch (this.currUIMode)
            {
                case UIMode.VertexSelection:
                    {
                        break;
                    }
                case UIMode.EdgeSelection:
                    {
                        break;
                    }
                case UIMode.FaceSelection:
                    {
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
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.prevMousePos = this.currMousePos;
            this.currMousePos = new Vector2d(e.X, e.Y);
            this.isMouseDown = false;

            switch (this.currUIMode)
            {
                case UIMode.VertexSelection:
                    {
                        break;
                    }
                case UIMode.EdgeSelection:
                    {
                        break;
                    }
                case UIMode.FaceSelection:
                    {
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
        }

        private void setViewMatrix()
        {
            int w = this.Width;
            int h = this.Height;
            if (h == 0)
            {
                h = 1;
            }
            this.MakeCurrent();

            this.clearScene();
            Gl.glViewport(0, 0, w, h);

            double aspect = (double)w / h;

            //Glu.gluPerspective(180.0, aspect, 0.01, 100);           

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            if (w >= h)
            {
                Glu.gluOrtho2D(-1.0 * aspect, 1.0 * aspect, -1.0, 1.0);
                //Gl.glOrtho(-1.0 * aspect, 1.0 * aspect, -1.0, 1.0, -100, 100);
            }
            else
            {
                Glu.gluOrtho2D(-1.0, 1.0, -1.0 * aspect, 1.0 * aspect);
                //Gl.glOrtho(-1.0, 1.0, -1.0 * aspect, 1.0 * aspect, -100, 100);
            }

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            //Glu.gluLookAt(0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //base.OnPaint(e);

            this.MakeCurrent();

            this.setViewMatrix();

            Matrix4d m = this.arcBall.getTransformMatrix() * this.currModelTransformMatrix;
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glMultMatrixd(m.Transpose().toArray());

            /***** Draw *****/
            clearScene();

            if (this.isDrawAxes)
            {
                this.DrawAxes();
            }

            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL);
            if (this.currMeshClass != null)
            {
                if (this.drawFace)
                {
                    this.currMeshClass.RenderShaded();
                }
                if (this.drawEdge)
                {
                    this.currMeshClass.RenderWireFrame();
                }
                if (this.drawVertex)
                {
                    this.currMeshClass.RenderVertices();
                }
            }

            Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL);
            Gl.glPopMatrix();
            this.SwapBuffers();
        }// onPaint

        private void DrawAxes()
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

    }// GLViewer
}// namespace
