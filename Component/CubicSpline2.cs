using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Geometry;
using MyCholmodSolver;

namespace Component
{
	public unsafe class CubicSpline2
	{
		private int dimension = 2;		// default 2d
		private int numOfPoints = 0;
		private Vector2d[] inputPoints = null;
		private Vector2d[] B = null;	// bezier control inputPoints

		public List<Vector2d> sampledPoints_ = null;
		public List<Vector2d> sampledPointsTangents_ = null;
		public List<Vector2d> sampledPointsNormals_ = null;
		public List<Vector2d> jointPoints_ = new List<Vector2d>();
		public void SamplePoints(int numpoints)
		{
			// handle degenerated case
			if (numpoints < 2) {
				Console.WriteLine(@" < 2 sample points");
				this.sampledPoints_ = this.GetCurvePoints(1);
				this.sampledPointsTangents_ = this.GetCurvePointsTagent(1);
				this.GetSamplePointsNormals();
				return;
			}

			// sample uniformly distributed points on the curves (#of numofPoints)
			this.sampledPoints_ = new List<Vector2d>();
			this.sampledPointsTangents_ = new List<Vector2d>();


			// get a temp rough curve length
			double tmpLength = 0;
			int K = 5;
			List<Vector2d> tmppoints = this.GetCurvePoints(5);
			for (int i = 0; i < tmppoints.Count-1; ++i)
			{
				tmpLength += (tmppoints[i + 1] - tmppoints[i]).Length();
			}
			
			// resampling
			double len = tmpLength / (numpoints-1);
			int L = 10;
			int[] numi = new int[this.numOfPoints-1];
			int count = 0;
			for (int i = 0; i < tmppoints.Count-1; i+=(K+1))
			{
				double intv_len = 0;
				for (int j = 0; j <= K; ++j)
				{
					intv_len += (tmppoints[i + j + 1] - tmppoints[i + j]).Length();
				}
				numi[count++] = (int)((intv_len / len) * L);
			}
			
			// assign & get samples
			List<Vector2d> resampledPoints = this.GetCurvePoints(numi);
			List<Vector2d> resampledPointsTagent = this.GetCurvePointsTagent(numi);
			tmpLength = 0;
			for (int i = 0; i < resampledPoints.Count - 1; ++i)
			{
				tmpLength += (resampledPoints[i + 1] - resampledPoints[i]).Length();
			}

			// get samples
			double step_len = tmpLength / (numpoints - 1);
			double curr_len = 0;
			int indx = 0;
			for (int i = 0; i < resampledPoints.Count-1; ++i)
			{
				curr_len += (resampledPoints[i + 1] - resampledPoints[i]).Length();
				if (curr_len > indx * step_len)
				{
					this.sampledPoints_.Add(resampledPoints[i]);
					this.sampledPointsTangents_.Add(resampledPointsTagent[i]);
					indx++;
				}
			}
			
			// the last point
			this.sampledPoints_.Add(resampledPoints[resampledPoints.Count - 1]);
			this.sampledPointsTangents_.Add(resampledPointsTagent[resampledPointsTagent.Count - 1]);

			this.GetSamplePointsNormals();
		}
		public void SamplePoints(double dis_thres)
		{
			// get sample points w.r.t. the distance threshold
			// a ||s_i-s_i+1|| < dis_thres is required
			this.sampledPoints_ = new List<Vector2d>();
			this.sampledPointsTangents_ = new List<Vector2d>();

			for (int i = 0; i < this.numOfPoints-1; ++i)
			{
				double len = (this.inputPoints[i+1] - this.inputPoints[i]).Length();
				int npointsPerSegment = (int)(len / dis_thres);
				bool stop = false;
				while (!stop)
				{
					stop = true;

					List<Vector2d> points = new List<Vector2d>();
					List<Vector2d> pointsTagent = new List<Vector2d>();
					for (int j = 0; j < npointsPerSegment; ++j)
					{
						double t1 = (double)j / npointsPerSegment;
						double t2 = (double)(j + 1) / npointsPerSegment;
						Vector2d p1 = this.GetCurvePoint(i, t1);
						Vector2d p2 = this.GetCurvePoint(i, t2);
						
						if ((p2 - p1).Length() > dis_thres)
						{
							stop = false;
							npointsPerSegment++;
							break;
						}
						
						Vector2d pt1 = this.GetCurvePointTagent(i, t1);
						points.Add(p1);
						pointsTagent.Add(pt1);
					}

					if (stop == true)
					{
						this.sampledPoints_.AddRange(points);
						this.sampledPointsTangents_.AddRange(pointsTagent);
					}
				}
			}

			// end point
			this.sampledPoints_.Add(this.GetCurvePoint(this.numOfPoints - 2, 1));
			this.sampledPointsTangents_.Add(this.GetCurvePointTagent(this.numOfPoints - 2, 1));

			// get normals
			this.GetSamplePointsNormals();

		}

		private void GetSamplePointsNormals()
		{
			if (this.sampledPointsTangents_ == null) return;

			this.sampledPointsNormals_ = new List<Vector2d>();
			foreach (Vector2d tagent in this.sampledPointsTangents_)
			{
				Vector2d normal = new Vector2d(tagent.y, -tagent.x);
				this.sampledPointsNormals_.Add(normal);
			}
		}

		// interfaces
		public List<Vector2d> GetCurvePoints(int numPerInterval)
		{
			// numPerInterval -- number of points lies in-between two curve points in each interval
			List<Vector2d> curvePoints = new List<Vector2d>();
			int num = numOfPoints + numPerInterval * (numOfPoints - 1);
			double lamb = 1.0 / (numPerInterval + 1);
			for (int i = 0; i < numOfPoints - 1; ++i)
			{
				for (int j = 0; j <= numPerInterval; ++j) // start point + inner inputPoints
				{
					double t = j * lamb; 
					curvePoints.Add(this.GetCurvePoint(i,t));
				}
			}

			// last point
			Vector2d et = inputPoints[numOfPoints - 1];
			curvePoints.Add(
				new Vector2d(et.x, et.y)
			);

			return curvePoints;
		}
		public List<Vector2d> GetCurvePointsTagent(int numPerInterval)
		{
			// numPerInterval -- number of points lies in-between two curve points in each interval
			List<Vector2d> curvePointsTagent = new List<Vector2d>();
			int num = numOfPoints + numPerInterval * (numOfPoints - 1);
			double lamb = 1.0 / (numPerInterval + 1);
			for (int i = 0; i < numOfPoints - 1; ++i)
			{
				for (int j = 0; j <= numPerInterval; ++j) // start point + inner inputPoints
				{
					double t = j * lamb;
					curvePointsTagent.Add(this.GetCurvePointTagent(i, t));
				}
			}
			
			// last point
			Vector2d et = this.GetCurvePointTagent(numOfPoints-2, 1);
			curvePointsTagent.Add(et);
			
			return curvePointsTagent;
		}
		public List<Vector2d> GetCurvePoints(int[] numPerInterval)
		{
			List<Vector2d> curvePoints = new List<Vector2d>();
			for (int i = 0; i < numOfPoints - 1; ++i)
			{
				double lamb = 1.0 / (numPerInterval[i] + 1);
				for (int j = 0; j <= numPerInterval[i]; ++j) // start point + inner inputPoints
				{
					double t = j * lamb;
					curvePoints.Add(this.GetCurvePoint(i, t));
				}
			}

			// last point
			Vector2d et = inputPoints[numOfPoints - 1];
			curvePoints.Add(
				new Vector2d(et.x, et.y)
			);

			return curvePoints;
		}
		public List<Vector2d> GetCurvePointsTagent(int[] numPerInterval)
		{
			List<Vector2d> curvePointsTagent = new List<Vector2d>();
			for (int i = 0; i < numOfPoints - 1; ++i)
			{
				double lamb = 1.0 / (numPerInterval[i] + 1);
				for (int j = 0; j <= numPerInterval[i]; ++j) // start point + inner inputPoints
				{
					double t = j * lamb;
					curvePointsTagent.Add(this.GetCurvePointTagent(i, t));
				}
			}
			
			// last point
			Vector2d et = this.GetCurvePointTagent(numOfPoints - 2, 1);
			curvePointsTagent.Add(et);

			return curvePointsTagent;
		}
		private Vector2d GetCurvePoint(int i, double t)
		{
			Vector2d b0 = inputPoints[i];
			Vector2d b1 = 2 * B[i] / 3.0 + B[i + 1] / 3.0;
			Vector2d b2 = B[i] / 3.0 + 2 * B[i + 1] / 3.0; ;
			Vector2d b3 = inputPoints[i + 1];
			double dt = 1.0 - t;
			double t0 = dt * dt * dt, t1 = 3 * dt * dt * t, t2 = 3 * dt * t * t, t3 = t * t * t;
			return t0 * b0 + t1 * b1 + t2 * b2 + t3 * b3;
		}
		private Vector2d GetCurvePointTagent(int i, double t)
		{
			Vector2d b0 = inputPoints[i];
			Vector2d b1 = 2 * B[i] / 3.0 + B[i + 1] / 3.0;
			Vector2d b2 = B[i] / 3.0 + 2 * B[i + 1] / 3.0; ;
			Vector2d b3 = inputPoints[i + 1];
			double dt = 1.0 - t;
			double t0 = - dt * dt * 3, t1 = 3 * (-2 * dt * t + dt * dt), t2 = 3 * (-t * t + 2*t*dt), t3 = 3 * t * t;
			return t0 * b0 + t1 * b1 + t2 * b2 + t3 * b3;
		}

		// constructors and destructors
		public CubicSpline2(Vector2d[] pts)
		{
			if (pts.Length < 3)
			{
				Console.WriteLine("err: must have > 3 points to interpolate!");
			}

			this.inputPoints = pts;
			this.numOfPoints = pts.Length;
			this.B = new Vector2d[numOfPoints];
			for (int i = 0; i < numOfPoints; ++i)
			{
				this.B[i] = new Vector2d();
			}
            //this.ComputeBezierControlPoints();
		}
		~CubicSpline2()
		{

		}

		// main function
		private void ComputeBezierControlPoints()
		{
			CholmodSolver cholmodSolver = new CholmodSolver();

			int n = numOfPoints-2;
			cholmodSolver.InitializeMatrixA(n, n);
			for (int i = 0; i < n; ++i)
			{
				if (i == 0)
				{
					cholmodSolver.Add_Coef(i, i, 4);
					cholmodSolver.Add_Coef(i, i + 1, 1);
				}
				else if (i == n - 1)
				{
					cholmodSolver.Add_Coef(i, i, 4);
					cholmodSolver.Add_Coef(i, i - 1, 1);
				}
				else
				{
					cholmodSolver.Add_Coef(i, i - 1, 1);
					cholmodSolver.Add_Coef(i, i, 4);
					cholmodSolver.Add_Coef(i, i + 1, 1);
				}
			}

			// factorize
			cholmodSolver.InitializeSolver();
			cholmodSolver.SetFinalPack(0);
			cholmodSolver.Factorize();

			// rhs
			double[] b = new double[n]; double[] x = new double[n];
			for (int k = 0; k < dimension; ++k)
			{
				for (int i = 0; i < n; ++i)
				{
					double r = 6.0;
					Vector2d pt;
					if (i == 0)
					{
						pt = r * inputPoints[1] - inputPoints[0];
					}
					else if (i == n - 1)
					{
						pt = r * inputPoints[i+1] - inputPoints[i+2];
					}
					else
					{
						pt = r * inputPoints[i + 1];
					}
					switch (k)
					{
						case 0:
							b[i] = pt.x;// (inputPoints[t].x - inputPoints[s].x);
							break;
						case 1:
							b[i] = pt.y;//(inputPoints[t].y - inputPoints[s].y);
							break;
					}
				}

				// solve
				fixed (double* _b = b)
				{
					cholmodSolver.InitializeMatrixB(_b, n, 1);
				}
				fixed (double* _x = x)
				{
					cholmodSolver.Linear_Solve(_x, false);
				}

				// assign
				for (int i = 0; i < n; ++i)
				{
					int tt = i + 1;
					switch (k)
					{
						case 0:
							this.B[tt].x = x[i];
							break;
						case 1:
							this.B[tt].y = x[i];
							break;
					}
				}
			}
			B[0] = inputPoints[0];
			B[B.Length - 1] = inputPoints[inputPoints.Length - 1];
			
			cholmodSolver.Release();
		}
	}
}

