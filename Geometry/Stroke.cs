using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace Geometry
{
    public class Stroke
    {
        //endpoints
        public Vector3d u3;
        public Vector3d v3;
        public Vector2d u2;
        public Vector2d v2;
        private int npoints;
        //stroke points
        public List<Vector2d> strokePoints2d;
        public List<Vector3d> strokePoints3d;
        public List<Vector3d> meshVertices3d;
        public List<Vector2d> meshVertices2d;
        public List<int> faceIndex;
        private int facecount = 0;
        private double size2 = 3.0;
        private double size3 = 0.008;
        public Color stokeColor = Color.DarkGray;
        public int opacity = 0;
        public Plane hostPlane;
        public double depth = 1.0;

        public Stroke(Vector3d v1, Vector3d v2)
        {
            this.u3 = v1;
            this.v3 = v2;
            this.npoints = (int)((v1 - v2).Length() / 0.01);
            this.sampleStrokePoints();
        }

        public int FaceCount
        {
            get
            {
                return this.facecount;
            }
        }

        public void setStrokeSize(double s)
        {
            this.size2 = s;
        }

        private void sampleStrokePoints()
        {
            Vector3d dir = (this.v3 - this.u3).normalize();
            this.strokePoints3d = new List<Vector3d>();
            double step = (this.v3 - this.u3).Length() / this.npoints;
            for (int i = 0; i < this.npoints; ++i)
            {
                this.strokePoints3d.Add(new Vector3d(this.u3 + step * i * dir));
            }
        }//sampleStrokePoints

        public void buildStrokeMesh(List<Vector2d> points, List<Vector2d> normals)
        {
            this.meshVertices2d = new List<Vector2d>();
            double radius = this.size2 / 2;
            int n = points.Count;
            for (int i = 0; i < n; ++i)
            {
                Vector2d p = points[i];
                Vector2d nor = normals[i].normalize();
                Vector2d v1 = p + nor * radius;
                Vector2d v2 = p - nor * radius;
                this.meshVertices2d.Add(v1);
                this.meshVertices2d.Add(v2);
            }
            // face
            this.faceIndex = new List<int>();
            for (int i = 0, j = 0; i < n - 1; ++i, j += 2)
            {
                this.faceIndex.Add(j);
                this.faceIndex.Add(j + 1);
                this.faceIndex.Add(j + 3);
                this.faceIndex.Add(j);
                this.faceIndex.Add(j + 3);
                this.faceIndex.Add(j + 2);
            }
            this.facecount = this.faceIndex.Count / 3;
        }

        public void changeStyle(int type)
        {
            this.meshVertices2d = new List<Vector2d>();
            this.meshVertices3d = new List<Vector3d>();
            int N = this.npoints;
            float start = (float)N;
            for (int i = 0; i < N; ++i)
            {
                int I = i - 1 >= 0 ? i - 1 : i;
                int J = i + 1 < N ? i + 1 : i;
                Vector2d u2 = this.strokePoints2d[I];
                Vector2d v2 = this.strokePoints2d[J];
                Vector2d o2 = this.strokePoints2d[i];
                Vector2d d2 = (v2 - u2).normalize();	// dir
                Vector2d n2 = new Vector2d(-d2.y, d2.x).normalize();

                Vector3d u3 = this.strokePoints3d[I];
                Vector3d v3 = this.strokePoints3d[J];
                Vector3d o3 = this.strokePoints3d[i];
                Vector3d d3 = (v3 - u3).normalize();	// dir
                Vector3d n3 = this.hostPlane.normal.Cross(d3).normalize();

                double isize2 = this.size2 / 2;
                double isize3 = this.size3 / 2;
                int op = 255;
                switch (type)
                {
                    case 0: // pencil
                        {
                            op = 255;
                            break;
                        }
                    case 1: // pen
                        {
                            isize2 = (start - i + 1) / start * this.size2;
                            isize3 = (start - i + 1) / start * this.size3;
                            op = 255; //(int)((start - i) / start * 255.0);
                            break;
                        }
                    case 2:// pen-2
                        {
                            isize2 = (i + 1) / start * this.size2;
                            isize3 = (i + 1) / start * size3;
                            op = 255; // (int)(i / start * 255.0);
                            break;
                        }
                    case 3: // crayon
                        {
                            op = 155;
                            break;
                        }                 
                    case 4://ink - 1
                        {
                            double diff = (i - start / 2) / start * 8;
                            diff = Math.Pow(Math.E, -(diff * diff) / 10);
                            isize2 = diff * this.size2;
                            isize3 = diff * size3;
                            op = (int)((diff + 0.1) * 255.0);
                            if (op > 255)
                                op = 255;
                            break;
                        }
                    case 5: //ink-2 watercolor
                        {
                            double diff = (i - start / 2) / start * 8;
                            diff = Math.Pow(Math.E, -(diff * diff) / 10);
                            if (diff < 1)
                                diff = 1 - diff;
                            else
                                diff = diff - 1;
                            diff += 0.2;
                            if (diff > 1) diff = 1.0;
                            isize2 = diff * this.size2;
                            isize3 = diff * size3;
                            op = (int)(diff * 255.0);
                            break;
                        }
                }
                this.opacity = (byte)(op);
                this.meshVertices2d.Add(o2 + isize2 * n2);
                this.meshVertices2d.Add(o2 - isize2 * n2);
                this.meshVertices3d.Add(o3 + isize3 * n3);
                this.meshVertices3d.Add(o3 - isize3 * n3);
            }
        }

        
    }//Stroke

    public class GuideLine
    {
        public Vector3d u;
        public Vector3d v;
        private int nSketch = 0;
        public List<Stroke> strokes;
        public Plane hostPlane;
        private static readonly Random rand = new Random();

        public GuideLine(Vector3d v1, Vector3d v2, Plane plane)
        {
            this.u = v1;
            this.v = v2;
            this.hostPlane = plane;
            this.DefineSketchyEdges();
        }

        private double angleTransform(int degree)
        {
            return Math.PI * (double)degree / 180.0;
        }

        private double getRandomDoubleInRange(Random rand, double s, double e)
        {
            return s + (e - s) * rand.NextDouble();
        }

        private void DefineSketchyEdges()
        {
            this.strokes = new List<Stroke>();
            double gap = 0.1;
            double len = gap * (v - u).Length();
            Vector3d lineDir = (v - u).normalize();
            this.nSketch = rand.Next(2, 4);
            for (int i = 0; i < this.nSketch; ++i)
            {
                Vector3d[] endpoints = new Vector3d[2];
                for (int j = 0; j < 2; ++j)
                {
                    // find an arbitrary point
                    double dis = this.getRandomDoubleInRange(rand, -len, len);
                    // find a random normal
                    Vector3d normal = new Vector3d();
                    for (int k = 0; k < 3; ++k)
                    {
                        normal[k] = this.getRandomDoubleInRange(rand, -1, 1);
                    }
                    normal.normalize();
                    Vector3d step = this.getRandomDoubleInRange(rand, -len / 4, len / 4) * normal;
                    if (j == 0)
                    {
                        endpoints[j] = u + dis * lineDir;
                        endpoints[j] += step;
                    }
                    else
                    {
                        endpoints[j] = v + dis * lineDir;
                        endpoints[j] += step;
                    }
                }
                Stroke line = new Stroke(endpoints[0], endpoints[1]);
                this.strokes.Add(line);
            }
        }//DefineSketchyEdges
    }//GuideLine
}
