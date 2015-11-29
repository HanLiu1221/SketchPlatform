using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Geometry
{
	public class HalfEdge
	{
		private int fromVertexIndex = -1;
		private int toVertexIndex = -1;
		private int faceIndex = -1;
		public HalfEdge nextHalfEdge = null;
		public HalfEdge prevHalfEdge = null;
		public HalfEdge invHalfEdge = null;

		public HalfEdge(int from, int to, int findex)
		{
			fromVertexIndex = from;
			toVertexIndex = to;
			faceIndex = findex;
		}

        public int FromIndex
        {
            get
            {
                return this.fromVertexIndex;
            }
        }

        public int ToIndex
        {
            get
            {
                return this.toVertexIndex;
            }
        }

        public int FaceInde
        {
            get
            {
                return this.faceIndex;
            }
        }
	}//HalfEdge

	public class Mesh
	{
		private double[] vertexPos = null;
		private int[] faceVertexIndex = null;
		private HalfEdge[] halfEdges = null;
        private HalfEdge[] edges = null;
		private double[] vertexNormal = null;
		private double[] faceNormal = null;
		private int vertexCount = 0;
		private int faceCount = 0;
        private Vector3d minCoord = Vector3d.MaxCoord();
        private Vector3d maxCoord = Vector3d.MinCoord();

        public HalfEdge edgeIter = null;
		
		// to avoid changing the count of vertex/face
		public int VertexCount
		{
			get
			{
				return vertexCount;
			}
		}

		public int FaceCount
		{
			get 
			{ 
				return faceCount; 
			}
		}

        public double[] VertexPos
        {
            get
            {
                return this.vertexPos;
            }
        }

        public int[] FaceVertex
        {
            get
            {
                return this.faceVertexIndex;
            }
        }

        public HalfEdge[] Edges
        {
            get
            {
                return edges;
            }
        }

        public double[] FaceNormal
        {
            get
            {
                return this.faceNormal;
            }
        } 
		public Mesh()
		{ }

		public Mesh(string meshFileName)
		{
			if(!File.Exists(meshFileName))
			{
				return;
			}
			StreamReader sr = new StreamReader(meshFileName);
			// mesh file type
			string extension = Path.GetExtension(meshFileName); 
			if(extension.Equals(".off"))
			{
				LoadOffMesh(sr);
			} else // default ".obj"
            {
                LoadObjMesh(sr);
            }
			sr.Close();
		}

		private void LoadObjMesh(StreamReader sr)
		{
			List<double> vertexArray = new List<double>();
			List<int> faceArray = new List<int>();
			List<HalfEdge> halfEdgeArray = new List<HalfEdge>();
            List<HalfEdge> edgeArray = new List<HalfEdge>();
			Dictionary<int, int> edgeHashTable = new Dictionary<int, int>();
			char[] separator = new char[]{' ', '\t'};
			this.vertexCount = 0;
			this.faceCount = 0;
			
			while(sr.Peek() > -1)
			{
				string line = sr.ReadLine();
				string[] array = line.Split(separator);
				if (array.Length != 4)
				{
					Console.WriteLine(line);
					Console.WriteLine("Vertex/Face read error.");
					return;
				}
				if(line[0] == 'v')
				{
                    Vector3d v = new Vector3d();
                    for (int i = 1; i < 4; ++i) 
                    {
                        v[i - 1] = double.Parse(array[i]);
                        vertexArray.Add(v[i - 1]);
                    }
					++this.vertexCount;
                    this.minCoord = Vector3d.Min(this.minCoord, v);
                    this.maxCoord = Vector3d.Max(this.maxCoord, v);
				}
				else if(line[0] == 'f')
				{
					List<int> currFaceArray = new List<int>();
					List<HalfEdge> currHalfEdgeArray = new List<HalfEdge>();
					for (int i = 1; i < 4; ++i)
					{
						currFaceArray.Add(int.Parse(array[i]) - 1); // face index from 1
					}
					faceArray.AddRange(currFaceArray);
					// hash map here for opposite halfedge
					for (int i = 0; i < 3; ++i)
					{
						int v1 = currFaceArray[i];
						int v2 = currFaceArray[(i + 1) % 3];
						HalfEdge halfedge = new HalfEdge(v1, v2, faceCount);
						int key = Math.Min(v1, v2) * vertexCount + Math.Max(v1, v2);
						if (edgeHashTable.ContainsKey(key)) // find a halfedge
						{
							HalfEdge oppHalfEdge = halfEdgeArray[edgeHashTable[key]];
							halfedge.invHalfEdge = oppHalfEdge;
							oppHalfEdge.invHalfEdge = halfedge;
						}
						else
						{
							edgeHashTable.Add(key, halfEdgeArray.Count);
                            edgeArray.Add(halfedge);
						}
						halfEdgeArray.Add(halfedge);
						currHalfEdgeArray.Add(halfedge);
					}
					for (int i = 0; i < 3;++i )
					{
						currHalfEdgeArray[i].nextHalfEdge = currHalfEdgeArray[(i + 1) % 3];
						currHalfEdgeArray[(i + 1) % 3].prevHalfEdge = currHalfEdgeArray[i];
						currHalfEdgeArray[i].prevHalfEdge = currHalfEdgeArray[(i - 1 + 3) % 3];
						currHalfEdgeArray[(i - 1 + 3) % 3].nextHalfEdge = currHalfEdgeArray[i].prevHalfEdge;
					}
					++faceCount;
				} 
				else if(line.Length > 1 && line.Substring(0,2).Equals("vt"))
				{
				}
			}//while
			this.vertexPos = vertexArray.ToArray();
			this.faceVertexIndex = faceArray.ToArray();
			this.halfEdges = halfEdgeArray.ToArray();
            this.edges = edgeArray.ToArray();
            this.edgeIter = this.halfEdges[0];
            this.Normalize();
			this.CalculateFaceVertexNormal();
		}//LoadObjMesh

		private void LoadOffMesh(StreamReader sr)
		{

		}//LoadOffMesh

		private void CalculateFaceVertexNormal()
		{
			if(this.faceVertexIndex == null || this.faceVertexIndex.Length == 0)
			{
				return;
			}
			this.faceNormal = new double[this.faceCount * 3];
			this.vertexNormal = new double[this.vertexCount * 3];
			for (int i = 0; i < this.faceCount; ++i)
			{
				int vidx1 = this.faceVertexIndex[3 * i];
                int vidx2 = this.faceVertexIndex[3 * i + 1];
                int vidx3 = this.faceVertexIndex[3 * i + 2];
                Vector3d v1 = new Vector3d(
                    this.vertexPos[vidx1 * 3], this.vertexPos[vidx1 * 3 + 1], this.vertexPos[vidx1 * 3 + 2]);
                Vector3d v2 = new Vector3d(
                    this.vertexPos[vidx2 * 3], this.vertexPos[vidx2 * 3 + 1], this.vertexPos[vidx2 * 3 + 2]);
                Vector3d v3 = new Vector3d(
                    this.vertexPos[vidx3 * 3], this.vertexPos[vidx3 * 3 + 1], this.vertexPos[vidx3 * 3 + 2]);
                Vector3d v21 = v2 - v1;
                Vector3d v31 = v3 - v1;
                Vector3d normal = v21.Cross(v31);
                normal.Normalize();
                for (int j = 0; j < 3; ++j)
                {
                    this.faceNormal[3 * i + j] = normal[j];
                    this.vertexNormal[vidx1 * 3 + j] += normal[j];
                    this.vertexNormal[vidx2 * 3 + j] += normal[j];
                    this.vertexNormal[vidx3 * 3 + j] += normal[j];
                }
			}
			for (int i = 0; i < this.vertexCount; ++i)
			{
                Vector3d vn = new Vector3d(this.vertexNormal[3 * i],
                    this.vertexNormal[3 * i + 1],
                    this.vertexNormal[3 * i + 2]);
                vn.Normalize();
                this.vertexNormal[3 * i] = vn.x;
                this.vertexNormal[3 * i + 1] = vn.y;
                this.vertexNormal[3 * i + 2] = vn.z;
			}
		}//CalculateFaceVertexNormal

        private void Normalize()
        {
            Vector3d c = (this.maxCoord + this.minCoord) / 2;
            Vector3d d = this.maxCoord - this.minCoord;
            double scale = d.x > d.y ? d.x : d.y;
            scale = d.z > scale ? d.z : scale;
            scale /= 2; // [-1, 1]
            for (int i = 0, j = 0; i < this.VertexCount; ++i, j += 3)
            {
                for (int k = 0; k < 3; ++k)
                {
                    this.vertexPos[j + k] /= scale;
                    this.vertexPos[j + k] -= c[k];
                }
            }
        }
	}//Mesh

}
