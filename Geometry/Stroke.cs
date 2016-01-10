using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Tao.OpenGl;
using Tao.Platform.Windows;
using Geometry;

namespace Component
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
        private double size2 = SegmentClass.StrokeSize;
        private double size3 = (double)SegmentClass.StrokeSize / 500;
        public Color strokeColor = SegmentClass.StrokeColor;
        public int opacity = 0;
        public Plane hostPlane;
        public double depth = 1.0;
        public int ncapoints = 5;
        private static readonly Random rand = new Random();
        private bool isBoxEdge = true;
        public double weight = SegmentClass.StrokeSize; // for line drawing

        public Stroke(Vector3d v1, Vector3d v2, bool isBoxEdge)
        {
            this.u3 = v1;
            this.v3 = v2;
            this.npoints = (int)((v1 - v2).Length() / 0.01);
            this.npoints = this.npoints > 0 ? this.npoints : 1;
            this.isBoxEdge = isBoxEdge;
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
            this.size3 = s / 500;
            this.weight = s;
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


        public void setStrokeMeshPoints2D(Vector2d normal)
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

        public void addCapfrom2D(int tag)
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

        public void addCap(int tag)
        {
            // tag = 1: add one head and one tail
            // tag = 2: round cap
            if (tag == 1)
            {
                this.addHeadTail2D();
                this.addHeadTail3D();
            }
            else
            {
                this.addRoundCap2D();
                this.addRoundCap3D();
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

        private void addRoundCap3D()
        {
            //head
            int N = this.ncapoints; // odd number
            Vector3d u = this.meshVertices3d[1];
            Vector3d v = this.meshVertices3d[0];
            Vector3d d = (u - v).normalize();
            double len = (u - v).Length() / 2;
            Vector3d n = d.Cross(this.hostPlane.normal).normalize();
            Vector3d c = (u + v) / 2;
            Vector3d p = c + n * len;
            int N2 = N / 2;
            Vector3d cv = (v - c).normalize();
            Vector3d vp = (p - v).normalize();
            double vp_len = (p - v).Length() / (N2 + 1);
            int loc = 0;
            for (int i = 0; i < N2; ++i)
            {
                Vector3d m = v + vp * (i + 1) * vp_len;
                Vector3d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices3d.Insert(loc++, p2);
            }
            this.meshVertices3d.Insert(loc++, p);
            Vector3d up = (p - u).normalize();
            double up_len = (p - u).Length() / (N2 + 1);
            for (int i = N2 - 1; i >= 0; --i)
            {
                Vector3d m = u + up * (i + 1) * up_len;
                Vector3d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices3d.Insert(loc++, p2);
            }
            this.meshVertices3d.Insert(loc++, c);
            // tail
            int tailIdx = this.meshVertices3d.Count;
            u = this.meshVertices3d[tailIdx - 1];
            v = this.meshVertices3d[tailIdx - 2];
            d = (u - v).normalize();
            len = (u - v).Length() / 2;
            n = this.hostPlane.normal.Cross(d).normalize();
            c = (u + v) / 2;
            p = c + n * len;
            cv = (v - c).normalize();
            vp = (p - v).normalize();
            vp_len = (p - v).Length() / (N2 + 1);
            for (int i = 0; i < N2; ++i)
            {
                Vector3d m = v + vp * (i + 1) * vp_len;
                Vector3d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices3d.Add(p2);
            }
            this.meshVertices3d.Add(p);
            up = (p - u).normalize();
            up_len = (p - u).Length() / (N2 + 1);
            for (int i = N2 - 1; i >= 0; --i)
            {
                Vector3d m = u + up * (i + 1) * up_len;
                Vector3d p2 = (m - c).normalize();
                p2 = c + p2 * len;
                this.meshVertices3d.Add(p2);
            }
            this.meshVertices3d.Add(c);
        }// addRoundCap3D

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

        public void changeStyle(int type)
        {
            this.meshVertices2d = new List<Vector2d>();
            this.meshVertices3d = new List<Vector3d>();
            int N = this.npoints;
            float start = (float)N;
            
            double r_size2 = Polygon.getRandomDoubleInRange(rand, this.size2 / 4, this.size2);
            double r_size3 = Polygon.getRandomDoubleInRange(rand, this.size3 / 4, this.size3);
            if (this.isBoxEdge)
            {
                r_size2 = this.size2;
                r_size3 = this.size3;
            }
            double isize2 = r_size2 / 2;
            double isize3 = r_size3 / 2;
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
                            isize2 = (start - i + 1) / start * r_size2;
                            isize3 = (start - i + 1) / start * r_size3;
                            op = 255; //(int)((start - i) / start * 255.0);
                            break;
                        }
                    case 2:// pen-2
                        {
                            isize2 = (i + 1) / start * r_size2;
                            isize3 = (i + 1) / start * r_size3;
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
                            isize2 = diff * r_size2;
                            isize3 = diff * r_size3;
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
                            isize2 = diff * r_size2;
                            isize3 = diff * r_size3;
                            op = (int)(diff * 255.0);
                            break;
                        }
                }
                this.strokePoints[i].opacity = (byte)(op);
                this.meshVertices2d.Add(o2 + isize2 * n2);
                this.meshVertices2d.Add(o2 - isize2 * n2);
                this.meshVertices3d.Add(o3 + isize3 * n3);
                this.meshVertices3d.Add(o3 - isize3 * n3);
            }
            this.buildStrokeMeshFace();
            this.addCap(2);
            //this.addCapfrom2D(1);
            //this.addCapfrom2D(2);
            //this.meshVertices3d = new List<Vector3d>();
            //Matrix4d invMat =  T.Inverse();
            //Plane plane = this.hostPlane.clone() as Plane;
            //plane.Transform(T);
            //foreach (Vector2d v2 in this.meshVertices2d)
            //{
            //    Vector3d v3 = camera.ProjectPointToPlane(v2, plane.center, plane.normal);
            //    Vector4d v4 = (invMat * new Vector4d(v3, 1));
            //    v3 = v4.ToVector3D();
            //    this.meshVertices3d.Add(v3);
            //}
        }// changeStyle

        
    }//Stroke

    public class GuideLine
    {
        public Vector3d u;
        public Vector3d v;
        public Vector2d u2;
        public Vector2d v2;
        private int nSketch = 0;
        public List<Stroke> strokes;
        public Plane hostPlane;
        private static readonly Random rand = new Random();
        public Arrow3D guideArrow;
        private bool isBoxEdge = true; // false -> guide line, less random strokes
        public bool active = true;
        public Color color = SegmentClass.StrokeColor;
        public bool isGuide = false;
        public Line3d[][] vanLines;
        public bool makeVisible = false;
        public double strokeGap = 0.1; // for overshotting
        // 1: normal 
        // 2: 1/2
        // 3: 1/3
        // 4: 1/4
        // 5: reflection
        public int type = 1;
        public double weight = SegmentClass.StrokeSize;

        public GuideLine(Vector3d v1, Vector3d v2, Plane plane, bool isBoxEdge)
        {
            this.u = v1;
            this.v = v2;
            this.hostPlane = plane;
            this.isBoxEdge = isBoxEdge;
            //if (!isBoxEdge)
            //{
            //    this.DefineGuideLineStroke();
            //}
            //else
            //{
            //    //this.DefineRandomStrokes();
            //    this.DefineCrossStrokes();
            //}
            this.DefineCrossStrokes();
            if (plane != null)
            {
                this.guideArrow = new Arrow3D(v1, v2, plane.normal);
            }
        }

        public void setHostPlane(Plane plane)
        {
            this.hostPlane = plane.clone() as Plane;
            if (this.strokes != null && this.strokes.Count > 0)
            {
                this.guideArrow = new Arrow3D(this.strokes[0].u3, this.strokes[0].v3, plane.normal);
            }
            else
            {
                this.guideArrow = new Arrow3D(u, v, plane.normal);
            }
        }

        private double angleTransform(int degree)
        {
            return Math.PI * (double)degree / 180.0;
        }

        private double getRandomDoubleInRange(Random rand, double s, double e)
        {
            return s + (e - s) * rand.NextDouble();
        }

        public void DefineRandomStrokes()
        {
            //if (!this.isBoxEdge)
            //{
            //    this.DefineGuideLineStroke();
            //    return;
            //}
            this.strokes = new List<Stroke>();
            double strokeLen = (v - u).Length();
            double len = strokeGap * strokeLen;
            Vector3d lineDir = (v - u).normalize();
            if (!this.isBoxEdge)
            {
                this.nSketch = rand.Next(1, 2);
            }
            else
            {
                this.nSketch = rand.Next(1, 4);
            }
            // now the first one is always the correct one without errors
            double dis = this.getRandomDoubleInRange(rand, -len/4, len);
            Stroke line = new Stroke(u - dis * lineDir, v + dis * lineDir, this.isBoxEdge);
            this.strokes.Add(line);
            double dirfloating = strokeLen / 40;
            for (int i = 1; i < this.nSketch; ++i)
            {
                Vector3d[] endpoints = new Vector3d[2];
                for (int j = 0; j < 2; ++j)
                {
                    // find an arbitrary point
                    dis = this.getRandomDoubleInRange(rand, -len, len);
                    // find a random normal
                    Vector3d normal = new Vector3d();
                    for (int k = 0; k < 3; ++k)
                    {
                        normal[k] = this.getRandomDoubleInRange(rand, -1, 1);
                    }
                    normal.normalize();
                    Vector3d step1 = this.getRandomDoubleInRange(rand, -dirfloating, dirfloating) * normal;
                    for (int k = 0; k < 3; ++k)
                    {
                        normal[k] = this.getRandomDoubleInRange(rand, -1, 1);
                    }
                    normal.normalize();
                    Vector3d step2 = this.getRandomDoubleInRange(rand, -dirfloating, dirfloating) * normal;
                    if (!this.isBoxEdge)
                    {
                        dis = this.getRandomDoubleInRange(rand, 0, len/2);
                        step1 = new Vector3d();
                        step2 = new Vector3d();
                    }
                    if (j == 0)
                    {
                        endpoints[j] = u + dis * lineDir;
                        endpoints[j] += step1;
                        if (!this.isBoxEdge)
                        {
                            endpoints[j] = u - dis * lineDir;
                        }
                    }
                    else
                    {
                        endpoints[j] = v + dis * lineDir;
                        endpoints[j] += step2;
                    }
                }
                line = new Stroke(endpoints[0], endpoints[1], this.isBoxEdge);
                line.weight *= 0.7;
                line.strokeColor = SegmentClass.sideStrokeColor;
                this.strokes.Add(line);
            }
        }//DefineRandomStrokes

        public void DefineCrossStrokes()
        {
            //if (!this.isBoxEdge)
            //{
            //    this.DefineGuideLineStroke();
            //    return;
            //}
            this.strokes = new List<Stroke>();
            double len = strokeGap *(v - u).Length();
            if (!this.isBoxEdge)
            {
                len /= 2;
            }
            Vector3d lineDir = (v - u).normalize();
            this.nSketch = 1;
            Vector3d[] endpoints = new Vector3d[2];
            double dis = this.getRandomDoubleInRange(rand, 0, len);
            endpoints[0] = u - dis * lineDir;
            endpoints[1] = v + dis * lineDir;
            Stroke line = new Stroke(endpoints[0], endpoints[1], this.isBoxEdge);
            this.strokes.Add(line);
        }

        public void DefineGuideLineStroke()
        {
            this.strokes = new List<Stroke>();
            Stroke line = new Stroke(this.u, this.v, this.isBoxEdge);
            this.strokes.Add(line);
        }

        public void buildVanishingLines(Vector3d v, int vidx)
        {
            double ext = 0.1;
            if (this.vanLines == null)
            {
                this.vanLines = new Line3d[2][];
            }
            this.vanLines[vidx] = new Line3d[2];
            Vector3d[] points = new Vector3d[2] { this.u, this.v };
            for (int i = 0; i < points.Length; ++i)
            {
                Vector3d vi = points[i];
                Vector3d d = (vi - v).normalize();
                ext = getRandomDoubleInRange(rand, 0, 0.2);
                Vector3d v1 = vi + ext * d;
                ext = getRandomDoubleInRange(rand, 0, 0.2);
                Vector3d v2 = v - ext * d;
                Line3d line = new Line3d(v1, v2);
                this.vanLines[vidx][i] = line;
            }
        }

        public void buildVanishingLines2d(Vector2d v, int vidx)
        {
            double ext = 0.1;
            if (this.vanLines == null)
            {
                this.vanLines = new Line3d[2][];
            }
            this.vanLines[vidx] = new Line3d[2];
            Vector2d[] points = new Vector2d[2] { this.u2, this.v2 };
            for (int i = 0; i < points.Length; ++i)
            {
                Vector2d vi = points[i];
                Vector2d d = (vi - v).normalize();
                ext = getRandomDoubleInRange(rand, 0, 20);
                Vector2d v1 = vi + ext * d;
                ext = getRandomDoubleInRange(rand, 0, 20);
                Vector2d v2 = v - ext * d;
                Line3d line = new Line3d(v1, v2);
                this.vanLines[vidx][i] = line;
            }
        }

    }//GuideLine

    public class Box
    {
        public Vector3d[] points = null;
        public Vector2d[] points2 = null;
        public Plane[] planes = null;
        public GuideLine[] edges = null;
        public List<List<GuideLine>> guideLines = null;
        public List<Ellipse3D> ellipses = null;
        public Line3d[][] vanLines;
        private static readonly Random rand = new Random();
        public List<Plane> facesToDraw;
        public List<Plane> facesToHighlight;
        public int activeFaceIndex = -1;
        public int highlightFaceIndex = -1;
        public List<int> hasGuides;

        public Box()
        { }

        public Box(Vector3d[] vs)
        {
            this.points = new Vector3d[vs.Length];
            for (int i = 0; i < vs.Length; ++i)
            {
                this.points[i] = new Vector3d(vs[i]);
            }
            // faces
            this.planes = new Plane[6];
            List<Vector3d> vslist = new List<Vector3d>();
            for (int i = 0; i < 4; ++i)
            {
                vslist.Add(this.points[i]);
            }
            this.planes[0] = new Plane(vslist);
            vslist = new List<Vector3d>();
            for (int i = 4; i < 8; ++i)
            {
                vslist.Add(this.points[i]);
            }
            this.planes[1] = new Plane(vslist);
            int r = 2;
            for (int i = 0; i < 4; ++i)
            {
                vslist = new List<Vector3d>();
                vslist.Add(this.points[i]);
                vslist.Add(this.points[(i + 1) % 4]);
                vslist.Add(this.points[((i + 1) % 4 + 4) % 8]);
                vslist.Add(this.points[(i + 4) % 8]);
                this.planes[r++] = new Plane(vslist);
            }
            this.edges = new GuideLine[12];
            int s = 0;
            Plane plane = new Plane();
            int[] series = { 0, 3, 0, 5 };
            for (int i = 0; i < 4; ++i)
            {
                plane = this.planes[series[i]].clone() as Plane;
                edges[s++] = new GuideLine(this.points[i], this.points[(i + 1) % 4], plane, true);
            }
            series = new int[] { 5, 3, 3, 5 };
            for (int i = 0; i < 4; ++i)
            {
                plane = this.planes[series[i]].clone() as Plane;
                edges[s++] = new GuideLine(this.points[i], this.points[i + 4], plane, true);
            }
            series = new int[] { 1, 3, 1, 5 };
            for (int i = 0; i < 4; ++i)
            {
                plane = this.planes[series[i]].clone() as Plane;
                edges[s++] = new GuideLine(this.points[i + 4], this.points[4 + (i + 1) % 4], plane, true);
            }
            this.guideLines = new List<List<GuideLine>>();
            this.facesToDraw = new List<Plane>();
            this.facesToHighlight = new List<Plane>();
            this.ellipses = new List<Ellipse3D>();
        }

        public List<GuideLine> getAllLines()
        {
            List<GuideLine> allLines = new List<GuideLine>();
            foreach (GuideLine edge in this.edges)
            {
                allLines.Add(edge);
            }
            for (int g = 0; g < this.guideLines.Count; ++g)
            {
                allLines.AddRange(this.guideLines[g]);
            }
            return allLines;
        }

        public void normalize(Vector3d center, double scale)
        {
            for (int i = 0;i < points.Length;++i)
            {
                points[i] /= scale;
                points[i] -= center;
            }
            foreach (GuideLine line in this.edges)
            {
                line.u /= scale;
                line.v /= scale;
                line.u -= center;
                line.v -= center;
                foreach (Stroke s in line.strokes)
                {
                    s.u3 /= scale;
                    s.u3 -= center;
                    s.v3 /= scale;
                    s.v3 -= center;
                }
            }
        }

        public void buildVanishingLines(Vector3d v, int vidx)
        {
            double ext = 0.1;
            if (this.vanLines == null)
            {
                this.vanLines = new Line3d[2][];
            }
            this.vanLines[vidx] = new Line3d[this.points.Length];
            for (int i = 0; i < this.points.Length; ++i)
            {
                Vector3d vi = this.points[i];
                Vector3d d = (vi - v).normalize();
                ext = Polygon.getRandomDoubleInRange(rand, 0, 0.2);
                Vector3d v1 = vi + ext * d;
                ext = Polygon.getRandomDoubleInRange(rand, 0, 0.2);
                Vector3d v2 = v - ext * d;
                Line3d line = new Line3d(v1, v2);
                this.vanLines[vidx][i] = line;
            }
        }

        public void buildVanishingLines2d(Vector2d v, int vidx)
        {
            double ext = 0.1;
            if (this.vanLines == null)
            {
                this.vanLines = new Line3d[2][];
            }
            this.vanLines[vidx] = new Line3d[this.points.Length];
            for (int i = 0; i < this.points.Length; ++i)
            {
                Vector2d vi = this.points2[i];
                Vector2d d = (vi - v).normalize();
                ext = Polygon.getRandomDoubleInRange(rand, 0, 20);
                Vector2d v1 = vi + ext * d;
                ext = Polygon.getRandomDoubleInRange(rand, 0, 20);
                Vector2d v2 = v - ext * d;
                Line3d line = new Line3d(v1, v2);
                this.vanLines[vidx][i] = line;
            }
        }

        public Plane[] GetYPairFaces()
        {
            return new Plane[2] {
				this.planes[0], this.planes[2]
			};
        }
        public Plane[] GetXPairFaces()
        {
            return new Plane[2] {
				this.planes[3], this.planes[5]
			};
        }
        public Plane[] GetZPairFaces()
        {
            return new Plane[2] {
				this.planes[1], this.planes[4]
			};
        }

        //public Vector2d GetVanishingPoints()
        //{
        //    // compute vanishing points vx, vy, vz for the cuboid
        //    Plane[] xfaces = this.GetXPairFaces();
        //    Plane[] yfaces = this.GetYPairFaces();
        //    Plane[] zfaces = this.GetZPairFaces();
        //    Vector3d[] dirs = new Vector3d[3] {
        //        (xfaces[1].center - xfaces[0].center).normalize(),
        //        (yfaces[1].center - yfaces[0].center).normalize(),
        //        (zfaces[1].center - zfaces[0].center).normalize()
        //    };
        //    List<Vector2d>[] xyz_lines = new List<Vector2d>[3] {
        //        new List<Vector2d>(),
        //        new List<Vector2d>(),
        //        new List<Vector2d>()
        //    };
        //    foreach (Plane planar in this.planes)
        //    {
        //        for (int i = 0; i < 4; ++i)
        //        {
        //            Vector3d u3 = planar.points[i], v3 = planar.points[(i + 1) % 4];
        //            Vector2d u2 = planar.points2[i], v2 = planar.points2[(i + 1) % 4];
        //            Vector3d uv = (v3 - u3).normalize();
        //            for (int j = 0; j < 3; ++j)
        //            {
        //                if (Math.Abs(uv.Dot(dirs[j])) > 0.5)
        //                {
        //                    xyz_lines[j].Add(u2);
        //                    xyz_lines[j].Add(v2);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    Vector2d[] vp = new Vector2d[3];
        //    for (int i = 0; i < 3; ++i)
        //    {
        //        List<Vector2d> lines = xyz_lines[i];
        //        int N = lines.Count / 2;	// N lines
        //        Vector2d mean = new Vector2d();
        //        for (int j = 0, k = 0; j < N; ++j, k += 2)
        //        {
        //            Vector2d u = lines[k], v = lines[k + 1];
        //            Vector2d p = lines[(k + 2) % lines.Count], q = lines[(k + 3) % lines.Count];
        //            Vector2d o = Utils.GetIntersectPoint(u, v, p, q);
        //            mean += o;
        //        }
        //        this.vp[i] = mean / N;
        //    }
        //    // 
        //    //Console.WriteLine("cuboid vp: vx(" + this.vp[0].x + ", " + this.vp[0].y + "), vy(" +
        //    //    this.vp[1].x + ", " + this.vp[1].y + "), vz(" +
        //    //    this.vp[2].x + ", " + this.vp[2].y + ")"
        //    //    ); 
        //}
    }// Box

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
