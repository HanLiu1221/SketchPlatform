using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Geometry;

namespace Geometry
{
    public class Polygon
    {
        public static double thresh = 1e-6;

        public Polygon()
        { }

        static public bool isPointInPolygon(Vector2d v, Vector2d[] points)
        {
            bool odd = false;
            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++ )
            {
                if ((points[i].y < v.y && points[j].y >= v.y) ||
                    (points[j].y < v.y && points[i].y >= v.y))
                {
                    if (points[i].x + (v.y - points[i].y) / (points[j].y - points[i].y) * (points[j].x - points[i].x) < v.x)
                    {
                        odd = !odd;
                    }
                }
            }
            return odd;
        }

        static public double getRandomDoubleInRange(Random rand, double s, double e)
        {
            return s + (e - s) * rand.NextDouble();
        }

        public static bool PointInPoly(Vector2d p, Vector2d[] points)
        {
            bool c = false;
            int n = points.Length;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((points[i].y > p.y) != (points[j].y > p.y)) &&
                    (p.x < (points[j].x - points[i].x) * (p.y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
                    c = !c;
            }
            return c;
        }

        public static bool LinePlaneIntersection(Line3d line, Vector3d planeCenter, Vector3d planeNormal, out Vector3d v)
        {
            Vector3d dir = (line.v3 - line.u3).normalize();
            v = new Vector3d();
            if (Math.Abs(dir.Dot(planeNormal)) < thresh)
            {
                return false; // parallel
            }
            Vector3d v0 = planeCenter;
            Vector3d n = planeNormal;
            Vector3d w = line.u3 - v0;
            double s = n.Dot(w) / (n.Dot(dir));
            s = -s;
            v = line.u3 + s * dir;
            return true;
        }

        public static Vector3d LineIntersectionPoint(Line3d l1, Line3d l2)
        {
            double x1 = l1.u3.x, y1 = l1.u3.y, z1 = l1.u3.z;
            double x2 = l2.u3.x, y2 = l2.u3.y, z3 = l2.u3.z;
            Vector3d d1 = (l1.v3 - l1.u3).normalize();
            Vector3d d2 = (l2.v3 - l2.u3).normalize();
            double dx = x2 - x1, dy = y2 - y1;
            double a1 = d1.x, a2 = d1.y, b1 = d2.x, b2 = d2.y;
            double t2 = (dx * a2 - dy * a1) / (a1 * b2 - b1 * a2);
            double t1 = t1 = (dx + b2 * t2) / a1;

            Vector3d l1_0 = l1.u3 + d1 * t1;
            Vector3d l2_0 = l2.u3 + d2 * t2;

            return l1_0;
        }

        public static Vector2d LineIntersectionPoint2d(Line2d l1, Line2d l2)
        {
            double x1 = l1.u.x, y1 = l1.u.y;
            double x2 = l2.u.x, y2 = l2.u.y;
            Vector2d d1 = (l1.v - l1.u).normalize();
            Vector2d d2 = (l2.v - l2.u).normalize();
            double dx = x2 - x1, dy = y2 - y1;
            double a1 = d1.x, a2 = d1.y, b1 = d2.x, b2 = d2.y;
            double t2 = (dx * a2 - dy * a1) / (a1 * b2 - b1 * a2);
            double t1 = 0;
            if (a1 != 0)
            {
                t1 = (dx + b1 * t2) / a1;
            }
            else
            {
                t1 = (dy + b2 * t2) / a2;
            }
            Vector2d vec1 = l1.u + d1 * t1;
            Vector2d vec2 = l2.u + d2 * t2;
            return vec1;
        }

        public static Vector2d FindPointTolineFootPrint(Vector2d pt, Vector2d u, Vector2d v)
        {
            Vector2d uv = (v - u).normalize();
            if (double.IsNaN(uv.x)) return pt;
            return u + (pt - u).Dot(uv) * uv;
        }

        public static double PointDistToPlane(Vector3d pos, Vector3d center, Vector3d normal)
        {
            double d = (pos - center).Dot(normal) / normal.Length();
            return Math.Abs(d);
        }
        public static bool IsLineSegmentIntersectWithCircle(Vector2d u, Vector2d v, Vector2d c, double radius)
        {
            if ((u - c).Length() < radius || (v - c).Length() < radius) return true;
            Vector2d uv = v - u;
            double r = (c - v).Dot(uv) / (u - v).Dot(uv);
            if (r < 0 || r > 1)
                return false;
            Vector2d p = u * r + (1 - r) * v;
            return (p - c).Length() < radius;
        }

        public static Vector2d FindLinesegmentCircleIntersection(Vector2d u, Vector2d v, Vector2d c, double radii)
        {
            if (!IsLineSegmentIntersectWithCircle(u, v, c, radii))
            {
                return u;
            }
            Vector2d v1 = new Vector2d();
            Vector2d v2 = new Vector2d();
            Vector2d fp = u;
            if ((v - c).Length() > (u - c).Length())
            {
                fp = v;
            }
            // line 
            if (Math.Abs(u.x - v.x) < thresh)
            {
                double x = (u.x + v.x) / 2;
                double y = Math.Sqrt(radii * radii - Math.Pow(x - c.x, 2)) + c.y;
                double ny = -Math.Sqrt(radii * radii - Math.Pow(x - c.x, 2)) + c.y;
                v1 = new Vector2d(x, y);
                v2 = new Vector2d(x, ny);
            }
            else if (Math.Abs(u.y - v.y) < thresh)
            {
                double y = (u.x + v.y) / 2;
                double x = Math.Sqrt(radii * radii - Math.Pow(y - c.y, 2)) + c.x;
                double nx = -Math.Sqrt(radii * radii - Math.Pow(y - c.y, 2)) + c.x;
                v1 = new Vector2d(x, y);
                v2 = new Vector2d(nx, y);
            }
            else
            {
                double k = (u.y - v.y) / (u.x - v.x);
                double b = u.y - k * u.x;
                double constant = c.x * c.x + b * b + c.y * c.y - 2 * b * c.y;
                constant = radii * radii - constant;
                double coef1 = 1 + k * k;
                double coef2 = 2 * k * b - 2 * c.x - 2 * k * c.y;
                coef2 /= coef1;
                constant /= coef1;
                constant += coef2 * coef2 / 4;
                double x = Math.Sqrt(constant) - coef2 / 2;
                double nx = -Math.Sqrt(constant) - coef2 / 2;
                v1 = new Vector2d(x, k * x + b);
                v2 = new Vector2d(nx, k * nx + b);
            }
            if ((v1 - fp).Length() < (v2 - fp).Length())
            {
                return v1;
            }
            else
            {
                return v2;
            }
        }//FindLinesegmentCircleIntersection
    }// Polygon

    public class Quad2d : Polygon
    {
        public Vector2d[] points = new Vector2d[4];
        public Quad2d(Vector2d v1, Vector2d v2)
        {
            points[0] = new Vector2d(v1);
            points[1] = new Vector2d(v2.x, v1.y);
            points[2] = new Vector2d(v2);
            points[3] = new Vector2d(v1.x, v2.y);
        }

        static public bool isPointInQuad(Vector2d v, Quad2d q)
        {
            Vector2d minv = Vector2d.MaxCoord();
            Vector2d maxv = Vector2d.MinCoord();
            for (int i = 0; i < q.points.Length; ++i)
            {
                minv = Vector2d.Min(minv, q.points[i]);
                maxv = Vector2d.Max(maxv, q.points[i]);
            }
            return v.x <= maxv.x && v.x >= minv.x && v.y <= maxv.y && v.y >= minv.y;
        }
    }// Quad2d

    public class Plane
    {
        public Vector3d[] points = null;
        public Vector2d[] points2 = null;
        public Vector3d normal;
        public Vector3d center;
        private int Npoints = 0;
        public double depth = 0;

        public Plane() { }

        public Plane(Vector3d[] vs)
        {
            if (vs == null) return;
            // newly created
            this.Npoints = vs.Length;
            this.points = new Vector3d[Npoints];
            for (int i = 0; i < Npoints; ++i)
            {
                this.points[i] = new Vector3d(vs[i]);
            }
            this.calclulateCenterNormal();
        }

        public Plane(List<Vector3d> vs)
        {
            // copy points
            this.points = vs.ToArray();
            this.Npoints = this.points.Length;
            this.calclulateCenterNormal();
        }

        public Plane(Vector3d center, Vector3d normal)
        {
            this.center = center;
            this.normal = normal;
        }

        private void calclulateCenterNormal()
        {
            if (this.Npoints == 0) return;
            this.center = new Vector3d();
            for (int i = 0; i < this.Npoints; ++i)
            {
                this.center += this.points[i];
            }
            this.center /= this.Npoints;
            Vector3d v1 = (this.points[1] - this.points[0]).normalize();
            Vector3d v2 = (this.points[this.Npoints - 1] - this.points[0]).normalize();
            this.normal = (v1.Cross(v2)).normalize();
        }

        public void Transform(Matrix4d T)
        {
            for (int i = 0; i < this.Npoints; ++i )
            {
                Vector3d v = this.points[i];
                Vector4d v4 = (T * new Vector4d(v, 1));
                double t = T[0, 3];
                this.points[i] = (T * new Vector4d(v, 1)).ToVector3D();
            }
            if (this.Npoints > 0)
            {
                this.calclulateCenterNormal();
            }
            else
            {
                this.center = (T * new Vector4d(this.center, 1)).ToVector3D();
                this.normal = (T * new Vector4d(this.normal, 1)).ToVector3D();
                this.normal.normalize();
            }
        }

        public Object clone()
        {
            Plane cloned = new Plane(this.points);
            return cloned;
        }
    }

    public class Line2d
    {
        public Vector2d u;
        public Vector2d v;

        public Line2d(Vector2d v1, Vector2d v2)
        {
            this.u = v1;
            this.v = v2;
        }
    }//Line2d

    public class Line3d
    {
        public Vector3d u3;
        public Vector3d v3;
        public Vector2d u2;
        public Vector2d v2;
        public bool active = true;

        public Line3d(Vector3d v1, Vector3d v2)
        {
            this.u3 = v1;
            this.v3 = v2;
        }

        public Line3d(Vector2d v1, Vector2d v2)
        {
            this.u2 = v1;
            this.v2 = v2;
        }
    }//Line3d

    public class Triangle
    {
        public Vector3d u;
        public Vector3d v;
        public Vector3d w;

        public Triangle(Vector3d p1, Vector3d p2, Vector3d p3)
        {
            this.u = new Vector3d(p1);
            this.v = new Vector3d(p2);
            this.w = new Vector3d(p3);
        }
    }

    public class Arrow3D
    {
        public Vector3d u;
        public Vector3d v;
        public Vector3d[] points; // curved arrow
        public Triangle cap;
        private double dcap = 0.03;
        public bool active = false;
        public Arrow3D(Vector3d p1, Vector3d p2, Vector3d normal)
        {
            this.u = new Vector3d(p1);
            this.v = new Vector3d(p2);
            // triangle
            double d = this.dcap; // (p2 - p1).Length() / 8;
            Vector3d lineDir = (p2-p1).normalize();
            Vector3d c = p2 - lineDir * d;
            Vector3d dir = normal.Cross(lineDir).normalize();
            double d2 = d * 0.8;
            Vector3d v1 = c + dir * d2;
            Vector3d v2 = c - dir * d2;
            this.cap = new Triangle(v1, p2, v2);
        }
    }

    public class Ellipse3D
    {
        public Vector3d[] points = null;
        public int npoints = 20;

        public Ellipse3D(Vector3d[] pts)
        {
            this.points = pts;
        }

        public Ellipse3D(Vector3d c, Vector3d u, Vector3d v, double a, double b)
        {
            points = new Vector3d[this.npoints];
            double alpha = Math.PI * 2 / this.npoints;
            for (int i = 0; i < this.npoints; ++i)
            {
                double angle = alpha * i;
                Vector3d p = c + a * Math.Cos(angle) * u + b * Math.Sin(angle) * v;
                points[i] = p;
            }
        }

    }// Ellipse3D
    
}
