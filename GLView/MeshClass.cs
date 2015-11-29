using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using Geometry;

namespace GraphicsPlatform
{
    public unsafe class MeshClass
    {
        public MeshClass(Mesh m)
        {
            this.mesh = m;
        }

        private Mesh mesh;
        public int tabIndex; // list of meshes
        private float[] material = { 0.62f, 0.74f, 0.85f, 1.0f };
        private float[] ambient = { 0.2f, 0.2f, 0.2f, 1.0f };
        private float[] diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] specular = { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] position = { 1.0f, 1.0f, 1.0f, 0.0f };

        /******************** Render ********************/
        public void RenderShaded()
        {
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE, material);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambient);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuse);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, specular);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glDepthFunc(Gl.GL_LESS);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_NORMALIZE);

            Gl.glColor3ub(GLViewer.colorSet[0].R, GLViewer.colorSet[0].G, GLViewer.colorSet[0].B);

            fixed (double* vp = this.mesh.VertexPos)
            fixed (double* vn = this.mesh.FaceNormal)
            fixed (int* index = this.mesh.FaceVertex)
            {
                Gl.glBegin(Gl.GL_TRIANGLES);
                for (int i = 0, j = 0; i < this.mesh.FaceCount; ++i, j += 3)
                {
                    Gl.glNormal3dv(new IntPtr(vn + j));
                    Gl.glVertex3dv(new IntPtr(vp + index[j] * 3));
                    Gl.glVertex3dv(new IntPtr(vp + index[j + 1] * 3));
                    Gl.glVertex3dv(new IntPtr(vp + index[j + 2] * 3));
                }
                Gl.glEnd();
            }
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_NORMALIZE);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_LIGHT0);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glDisable(Gl.GL_COLOR_MATERIAL);
        }

        public void RenderWireFrame()
        {
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glColor3ub(0, 0, 0);
            Gl.glBegin(Gl.GL_LINES);
            for (int i = 0; i < this.mesh.Edges.Length; ++i)
            {
                int fromIdx = this.mesh.Edges[i].FromIndex;
                int toIdx = this.mesh.Edges[i].ToIndex;
                Gl.glVertex3d(this.mesh.VertexPos[fromIdx * 3], 
                    this.mesh.VertexPos[fromIdx*3+1],
                    this.mesh.VertexPos[fromIdx*3+2]);
                Gl.glVertex3d(this.mesh.VertexPos[toIdx * 3],
                    this.mesh.VertexPos[toIdx * 3 + 1],
                    this.mesh.VertexPos[toIdx * 3 + 2]);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LINE_SMOOTH);
            Gl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
        }

        public void RenderVertices()
        {
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glColor3ub(255, 0, 0);
            Gl.glPointSize(2.0f);
            Gl.glBegin(Gl.GL_POINTS);
            for (int i = 0; i < this.mesh.VertexCount; ++i)
            {
                Gl.glVertex3d(this.mesh.VertexPos[i * 3], this.mesh.VertexPos[i * 3 + 1], this.mesh.VertexPos[i * 3 + 2]);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_POINT_SMOOTH);
            Gl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
        }
    }
}
