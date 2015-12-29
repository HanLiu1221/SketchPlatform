using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Geometry;

namespace Component
{
    public class Segment
    {
        public int idx = -1;
        public Mesh mesh = null;
        public Box boundingbox = null;
        public Color color = Color.Black;
        public Segment(Mesh m, Box c)
        {
            this.mesh = m;
            this.boundingbox = c;
        }
    }
}
