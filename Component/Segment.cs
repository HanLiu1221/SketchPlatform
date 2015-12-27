using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Geometry;

namespace SketchPlatform
{
    public class Segment
    {
        public int idx = -1;
        public Mesh mesh = null;
        public Cube boundingbox = null;
        public Color color;
        public Segment(Mesh m, Cube c)
        {
            this.mesh = m;
            this.boundingbox = c;
            Random r = new Random();
            int idx = r.Next(0, GLViewer.ColorSet.Length - 1);
            this.color = GLViewer.ColorSet[idx];
        }


    }
}
