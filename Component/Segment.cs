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
        public bool active = true;
        public List<List<int>> contours;

        public Segment(Mesh m, Box c)
        {
            this.mesh = m;
            this.boundingbox = c;
        }


        public void regionGrowingContours(List<int> labeled)
        {
            if (this.mesh == null) return;
            this.contours = new List<List<int>>();
            int ndist = 10;
            while (labeled.Count > 0)
            {
                int i = labeled[0];
                labeled.RemoveAt(0);
                if (!this.mesh.Flags[i])
                {
                    continue;
                }
                this.mesh.Flags[i] = false;
                List<int> vids = new List<int>();
                List<int> queue = new List<int>();
                queue.Add(i);
                vids.Add(i);
                int s = 0;
                int d = 0;
                while (s < queue.Count && d < ndist)
                {
                    int j = queue[s];
                    for (int k = 0; k < mesh.VertexFaceIndex.GetLength(1); ++k)
                    {
                        int f = mesh.VertexFaceIndex[j, k];
                        if (f == -1) continue;
                        for (int fi = 0; fi < 3; ++fi)
                        {
                            int kv = mesh.FaceVertexIndex[f * 3 + fi];
                            if (mesh.Flags[kv])
                            {
                                vids.Add(kv);
                                mesh.Flags[kv] = false;
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
                if (vids.Count > 5)
                {
                    this.contours.Add(vids);
                }
            }
        }//regionGrowingContours

    }
}
