using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Tao.OpenGl;
using Tao.Platform.Windows;
using Geometry;
namespace SketchPlatform
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
        public List<StrokePoint> strokePoints;
        public List<Vector3d> meshVertices3d;
        public List<Vector2d> meshVertices2d;
        public List<int> faceIndex;
        private int facecount = 0;
        private double size2 = 2.0;
        private double size3 = 0.008;
        public Color stokeColor = Color.DarkGray;
        public int opacity = 0;
        public Plane hostPlane;
        public double depth = 1.0;
        public int ncapoints = 5;

        public Stroke(Vector3d v1, Vector3d v2)
        {
            this.u3 = v1;
            this.v3 = v2;
            this.npoints = (int)((v1 - v2).Length() / 0.01);
            this.npoints = this.npoints > 0 ? this.npoints : 1;
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
            this.strokePoints = new List<StrokePoint>();
            double step = (this.v3 - this.u3).Length() / this.npoints;
            for (int i = 0; i < this.npoints; ++i)
            {
                Vector3d p = new Vector3d(this.u3 + step * i * dir);
                this.strokePoints.Add(new StrokePoint(p));
            }
        }//sampleStrokePoints

        public void setStrokeMeshPoints(List<Vector2d> points, List<Vector2d> normals)
        {
            this.meshVertices2d = new List<Vector2d>();
            double radius = this.size2 / 2;
            this.npoints = points.Count;
            for (int i = 0; i < this.npoints; ++i)
            {
                Vector2d p = points[i];
                Vector2d nor = normals[i].normalize();
                Vector2d v1 = p + nor * radius;
                Vector2d v2 = p - nor * radius;
                this.meshVertices2d.Add(v1);
                this.meshVertices2d.Add(v2);
            }
            this.buildStrokeMeshFace();
        }// setStrokeMeshPoints


        public void setStrokeMeshPoints(Vector2d normal)
        {
            this.meshVertices2d = new List<Vector2d>();
            double radius = this.size2 / 2;
            int n = this.strokePoints.Count;
            for (int i = 0; i < n; ++i)
            {
                Vector2d p = this.strokePoints[i].pos2;
                Vector2d v1 = p + normal * radius;
                Vector2d v2 = p - normal * radius;
                this.meshVertices2d.Add(v1);
                this.meshVertices2d.Add(v2);
            }
            this.buildStrokeMeshFace();
        }// setStrokeMeshPoints

        private void buildStrokeMeshFace()
        {
            // face
            this.faceIndex = new List<int>();
            for (int i = 0, j = 0; i < this.npoints - 1; ++i, j += 2)
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

        public void addCap(int tag)
        {
            // tag = 1: add one head and one tail
            // tag = 2: round cap
            if (tag == 1)
            {
                this.addHeadTail2D();
            }
            else
            {
                this.addRoundCap2D();
            }
            this.addHeadTailFaceIndex(tag);
        }
        public void addHeadTail2D()
        {            
            Vector2d u = this.meshVertices2d[1];
            Vector2d v = this.meshVertices2d[0];
            Vector2d d = (u - v).normalize();
            double len = (u - v).Length();
            Vector2d n = new Vector2d(-d.y, d.x).normalize();
            n = -1.0 * n;
            Vector2d c = (u + v) / 2;
            Vector2d p = c + n * len / 2;
            //head
            this.meshVertices2d.Insert(0, p);            
            //tail
            int tailIdx = this.meshVertices2d.Count;
            u = this.meshVertices2d[tailIdx - 1];
            v = this.meshVertices2d[tailIdx - 2];
            d = (u - v).normalize();
            len = (u - v).Length();
            n = new Vector2d(-d.y, d.x).normalize();
            c = (u + v) / 2;
            p = c + n * len;
            this.meshVertices2d.Add(p);
        }// addHeadTail2D

        public void addHeadTail3D()
        {
            if (this.hostPlane == null)
            {
                return;
            }
            Vector3d u = this.meshVertices3d[1];
            Vector3d v = this.meshVertices3d[0];
            Vector3d d = (u - v).normalize();
            double len = (u - v).Length();
            Vector3d n = d.Cross(this.hostPlane.normal).normalize();
            Vector3d c = (u + v) / 2;
            Vector3d p = c + n * len / 2;
            //head
            this.meshVertices3d.Insert(0, p);
            //tail
            int tailIdx = this.meshVertices3d.Count;
            u = this.meshVertices3d[tailIdx - 1];
            v = this.meshVertices3d[tailIdx - 2];
            d = (u - v).normalize();
            len = (u - v).Length();
            n = this.hostPlane.normal.Cross(d).normalize();
            c = (u + v) / 2;
            p = c + n * len;
            this.meshVertices3d.Add(p);
        }// addHeadTail3D

        private void addRoundCap2D()
        {
            //if (this.meshVertices2d == null || this.meshVertices2d.Count == 0) return;
            //head
            int N = this.ncapoints; // odd number
            Vector2d u = this.meshVertices2d[1];
            Vector2d v = this.meshVertices2d[0];
            Vector2d d = (u - v).normalize();
            double len = (u - v).Length() / 2;
            Vector2d n = new Vector2d(-d.y, d.x).normalize();
            n = -1.0 * n;
            Vector2d c = (u + v) / 2;
            Vector2d p = c + n * len;
            int N2 = N / 2;
            Vector2d cv = (v - c).normalize();
            Vector2d vp = (p - v).normalize();
            double vp_len = (p - v).Length() / (N2 + 1);
            int loc = 0;
            for (int i = 0; i < N2; ++i)
            {
                Vector2d m = v + vp * (i + 1) * vp_len;
                Vector2d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices2d.Insert(loc++, p2);
            }
            this.meshVertices2d.Insert(loc++, p);
            Vector2d up = (p - u).normalize();
            double up_len = (p - u).Length() / (N2 + 1);
            for (int i = N2 - 1; i >= 0; --i)
            {
                Vector2d m = u + up * (i + 1) * up_len;
                Vector2d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices2d.Insert(loc++, p2);
            }
            this.meshVertices2d.Insert(loc++, c);
            // tail
            int tailIdx = this.meshVertices2d.Count;
            u = this.meshVertices2d[tailIdx - 1];
            v = this.meshVertices2d[tailIdx - 2];
            d = (u - v).normalize();
            len = (u - v).Length() / 2;
            n = new Vector2d(-d.y, d.x).normalize();
            c = (u + v) / 2;
            p = c + n * len;
            cv = (v - c).normalize();
            vp = (p - v).normalize();
            vp_len = (p - v).Length() / (N2 + 1);
            for (int i = 0; i < N2; ++i)
            {
                Vector2d m = v + vp * (i + 1) * vp_len;
                Vector2d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices2d.Add(p2);
            }
            this.meshVertices2d.Add(p);
            up = (p - u).normalize();
            up_len = (p - u).Length() / (N2 + 1);
            for (int i = N2 - 1; i >= 0; --i)
            {
                Vector2d m = u + up * (i + 1) * up_len;
                Vector2d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices2d.Add(p2);
            }
            this.meshVertices2d.Add(c);
        }// addRoundCap2D

        private void addHeadTailFaceIndex(int tag)
        {
            if (tag == 1)
            {
                // head
                this.InsertTriFace(1, 2, 0);
                // add one more mesh vertex in front
                for (int i = 3; i < this.faceIndex.Count; ++i)
                {
                    this.faceIndex[i]++;
                }
                // tail
                int tailIdx = this.meshVertices2d.Count - 1;
                this.AddTriFace(tailIdx - 2, tailIdx - 1, tailIdx);
            }
            else
            {
                //head
                int N = this.ncapoints; // odd number 
                for (int i = 0; i < this.faceIndex.Count; ++i)
                {
                    this.faceIndex[i] += N + 1;
                }                               
                this.InsertTriFace(N, N + 2, N - 1);
                for (int i = N - 2; i >= 0; --i)
                {
                    this.InsertTriFace(N, i + 1, i);
                }
                this.InsertTriFace(N, 0, N + 1);                
                //tail
                int tailIdx = this.meshVertices2d.Count - 1;
                this.AddTriFace(tailIdx - N - 1, tailIdx, tailIdx - 1);
                for (int i = 1; i < N; ++i)
                {
                    this.AddTriFace(tailIdx - i, tailIdx, tailIdx - i - 1);
                }
                this.AddTriFace(tailIdx - N, tailIdx, tailIdx - N - 2);
            }
            this.facecount = this.faceIndex.Count / 3;
        }// addHeadTailFaceIndex

        private void InsertTriFace(int i, int j, int k)
        {
            this.faceIndex.Insert(0, i);
            this.faceIndex.Insert(0, j);
            this.faceIndex.Insert(0, k);
        }
        private void AddTriFace(int i, int j, int k)
        {
            this.faceIndex.Add(i);
            this.faceIndex.Add(j);
            this.faceIndex.Add(k);
        }

        public void changeStyle(int type, Matrix4d T, Camera camera)
        {
            this.meshVertices2d = new List<Vector2d>();
            this.meshVertices3d = new List<Vector3d>();
            int N = this.npoints;
            float start = (float)N;
            for (int i = 0; i < N; ++i)
            {
                int I = i - 1 >= 0 ? i - 1 : i;
                int J = i + 1 < N ? i + 1 : i;
                Vector2d u2 = this.strokePoints[I].pos2;
                Vector2d v2 = this.strokePoints[J].pos2;
                Vector2d o2 = this.strokePoints[i].pos2;
                Vector2d d2 = (v2 - u2).normalize();	// dir
                Vector2d n2 = new Vector2d(-d2.y, d2.x).normalize();

                Vector3d u3 = this.strokePoints[I].pos3;
                Vector3d v3 = this.strokePoints[J].pos3;
                Vector3d o3 = this.strokePoints[i].pos3;
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
                //this.meshVertices3d.Add(o3 + isize3 * n3);
                //this.meshVertices3d.Add(o3 - isize3 * n3);
            }
            this.buildStrokeMeshFace();
            //this.addCap(1);
            this.addCap(2);
            this.meshVertices3d = new List<Vector3d>();
            //T = T.Transpose();
            Matrix4d invMat =  T.Inverse();
            Plane plane = this.hostPlane.clone() as Plane;
            plane.Transform(T);
            foreach (Vector2d v2 in this.meshVertices2d)
            {
                Vector3d v3 = camera.ProjectPointToPlane(v2, plane.center, plane.normal);
                Vector4d v4 = (invMat * new Vector4d(v3, 1));
                v3 = v4.ToVector3D();
                this.meshVertices3d.Add(v3);
            }
        }// changeStyle

        
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

    public class StrokePoint
    {
        // contorl the representation of stroke mesh
        public Vector2d pos2;
        public Vector3d pos3;
        public Vector2d pos2_origin;
        public Vector3d pos3_origin;
        public byte opacity = 255;
        public Color color = Color.Black;
        public double depth = 1.0;
        public double speed = 1.0;

        public StrokePoint(Vector2d p)
        {
            this.pos2 = new Vector2d(p);
        }

        public StrokePoint(Vector3d p)
        {
            this.pos3 = new Vector3d(p);
        }

        public void Transform(Matrix4d T)
        {
            this.pos3 = (T * new Vector4d(pos3, 1)).ToVector3D();
        }
        public void Transform_from_origin(Matrix4d T)
        {
            this.pos3 = (T * new Vector4d(pos3_origin, 1)).ToVector3D();
        }
        public void Transform_to_origin(Matrix4d T)
        {
            this.pos3_origin = (T * new Vector4d(this.pos3, 1)).ToVector3D();
        }

    }// StrokePoint
}
