using System;

namespace Geometry
{
	public class matrix3d
	{
		private const int M = 3, N = 3, length = 9;

		private double[] arr = new double[length];

		public matrix3d()
		{
			for (int i = 0; i < length; ++i)
				arr[i] = 0;
		}

		public matrix3d(double[] array, bool rowWise = true)
		{
			if (rowWise)
			{
				for (int i = 0; i < length; ++i)
					arr[i] = array[i];
			}
			else //col-wise
			{
				for (int i = 0; i < M; ++i)
					for (int j = 0; j < N; ++j)
						arr[i * N + j] = array[j * M + i];
			}
		}

		public matrix3d(double[,] array)
		{
			for (int i = 0; i < M; ++i)
				for (int j = 0; j < N; ++j)
					arr[i * N + j] = array[i, j];
		}

		public double this[int row, int col]
		{
			get
			{
				if (row >= 0 && row < M && col >= 0 && col < N)
					return arr[row * N + col];
				else
					throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (row >= 0 && row < M && col >= 0 && col < N)
					arr[row * N + col] = value;
				else
					throw new ArgumentOutOfRangeException();
			}
		}

		public double this[int index]
		{
			get
			{
				if (index >= 0 && index < length)
					return arr[index];
				else
					throw new ArgumentOutOfRangeException();
			}
			set 
			{
				if (index >= 0 && index < length)
					arr[index] = value;
				else
					throw new ArgumentOutOfRangeException();
			}
		}

		public matrix3d(matrix3d mat)
		{
			for (int i = 0; i < length; ++i)
				arr[i] = mat[i];
		}

		// operators
		static public matrix3d operator +(matrix3d m1, matrix3d m2)
		{
			matrix3d m = new matrix3d(m1);
			for (int i = 0; i < length; ++i)
				m[i] += m2[i];
			return m;
		}

		static public matrix3d operator -(matrix3d m1, matrix3d m2)
		{
			matrix3d m = new matrix3d(m1);
			for (int i = 0; i < length; ++i)
				m[i] -= m2[i];
			return m;
		}

		static public matrix3d operator *(matrix3d m, double d)
		{
			matrix3d mat = new matrix3d(m); 
			for (int i = 0; i < length; ++i)
				mat[i] *= d;
			return mat;
		}

		static public matrix3d operator /(matrix3d m, double d)
		{
			matrix3d mat = new matrix3d(m);
			if (d == 0) throw new DivideByZeroException();
			for (int i = 0; i < length; ++i)
				mat[i] /= d;
			return mat;
		}

		// numerics
		public matrix3d IdentityMatrix()
		{
			matrix3d mat = new matrix3d();
			mat[0, 0] = mat[1, 1] = mat[2, 2] = 1.0;
			return mat;
		}

		public matrix3d Transpose()
		{
			matrix3d mat = new matrix3d();
			for (int i = 0; i < M; ++i)
				for (int j = 0; j < N; ++j)
					mat[j, i] = this[i, j];
			return mat;
		}

		public double Trace()
		{
			return this[0, 0] + this[1, 1] + this[2, 2];
		}

		public double Determinant()
		{
			return this[0, 0] * (this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1])
				- this[0, 1] * (this[1, 0] * this[2, 2] - this[1, 2] * this[2, 0])
				+ this[0, 2] * (this[1, 0] * this[2, 1] - this[1, 1] * this[2, 0]);
		}

		public matrix3d Inverse()
		{
			matrix3d mat = new matrix3d();
			double det = this.Determinant();
			if (det == 0) throw new DivideByZeroException("Determinant equals to 0!");
			mat[0, 0] = this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1];
			mat[0, 1] = this[0, 2] * this[2, 1] - this[0, 1] * this[2, 2];
			mat[0, 2] = this[0, 1] * this[1, 2] - this[0, 2] * this[1, 1];
			mat[1, 0] = this[1, 2] * this[2, 0] - this[1, 0] * this[2, 2];
			mat[1, 1] = this[0, 0] * this[2, 2] - this[0, 2] * this[2, 0];
			mat[1, 2] = this[0, 2] * this[1, 0] - this[0, 0] * this[1, 2];
			mat[2, 0] = this[1, 0] * this[2, 1] - this[1, 1] * this[2, 0];
			mat[2, 1] = this[0, 1] * this[2, 0] - this[0, 0] * this[2, 1];
			mat[2, 2] = this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
			mat = mat / det;
			return mat;
		}

	}//class-matrix3d

	public class matrix4d
	{
		private const int M = 4, N = 4, length = 16;

		private double[] arr = new double[length];

		public matrix4d()
		{
			for (int i = 0; i < length; ++i)
				arr[i] = 0;
		}

		public matrix4d(double[] array, bool rowWise = true)
		{
			if (rowWise)
			{
				for (int i = 0; i < length; ++i)
					arr[i] = array[i];
			}
			else //col-wise
			{
				for (int i = 0; i < M; ++i)
					for (int j = 0; j < N; ++j)
						arr[i * N + j] = array[j * M + i];
			}
		}

		public matrix4d(double[,] array)
		{
			for (int i = 0; i < M; ++i)
				for (int j = 0; j < N; ++j)
					arr[i * N + j] = array[i, j];
		}

		public double this[int row, int col]
		{
			get
			{
				if (row >= 0 && row < M && col >= 0 && col < N)
					return arr[row * N + col];
				else
					throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (row >= 0 && row < M && col >= 0 && col < N)
					arr[row * N + col] = value;
				else
					throw new ArgumentOutOfRangeException();
			}
		}

		public double this[int index]
		{
			get
			{
				if (index >= 0 && index < length)
					return arr[index];
				else
					throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (index >= 0 && index < length)
					arr[index] = value;
				else
					throw new ArgumentOutOfRangeException();
			}
		}

		public matrix4d(matrix4d mat)
		{
			for (int i = 0; i < length; ++i)
				arr[i] = mat[i];
		}

		// operators
		static public matrix4d operator +(matrix4d m1, matrix4d m2)
		{
			matrix4d m = new matrix4d(m1);
			for (int i = 0; i < length; ++i)
				m[i] += m2[i];
			return m;
		}

		static public matrix4d operator -(matrix4d m1, matrix4d m2)
		{
			matrix4d m = new matrix4d(m1);
			for (int i = 0; i < length; ++i)
				m[i] -= m2[i];
			return m;
		}

		static public matrix4d operator *(matrix4d m, double d)
		{
			matrix4d mat = new matrix4d(m);
			for (int i = 0; i < length; ++i)
				mat[i] *= d;
			return mat;
		}

		static public matrix4d operator /(matrix4d m, double d)
		{
			matrix4d mat = new matrix4d(m);
			if (d == 0) throw new DivideByZeroException();
			for (int i = 0; i < length; ++i)
				mat[i] /= d;
			return mat;
		}

		// numerics
		public matrix4d IdentityMatrix()
		{
			matrix4d mat = new matrix4d();
			mat[0, 0] = mat[1, 1] = mat[2, 2] = mat[3, 3] = 1.0;
			return mat;
		}

		public matrix4d Transpose()
		{
			matrix4d mat = new matrix4d();
			for (int i = 0; i < M; ++i)
				for (int j = 0; j < N; ++j)
					mat[j, i] = this[i, j];
			return mat;
		}

		public double Trace()
		{
			return this[0, 0] + this[1, 1] + this[2, 2] + this[3, 3];
		}

		public double Determinant()
		{
			return this[0, 0] * FormMatrix3D(0, 0).Determinant()
				- this[0, 1] * FormMatrix3D(0, 1).Determinant()
				+ this[0, 2] * FormMatrix3D(0, 2).Determinant()
				- this[0, 3] * FormMatrix3D(0, 3).Determinant();
		}

		public matrix3d FormMatrix3D(int row, int col)
		{
			// remove the element at [row, col]
			// for calculating the determinant
			matrix3d mat = new matrix3d();
			int r = 0;
			for (int i = 0; i < M; ++i)
			{
				if(i == row) continue;
				int c = 0;
				for (int j = 0; j < N; ++j)
				{
					if (j == col) continue;
					mat[r, c++] = this[i, j];
				}
				++r;
			}
			return mat;
		}

		// transformation
		public matrix4d TranslationMatrix(vector3d v)
		{
			matrix4d mat = IdentityMatrix();
			for (int i = 0; i < 3; ++i)
				mat[i, 3] = v[i];
			return mat;
		}

		public matrix4d ScalingMatrix(vector3d v)
		{
			matrix4d mat = IdentityMatrix();
			for (int i = 0; i < 3; ++i)
				mat[i, i] = v[i];
			return mat;
		}

		public matrix4d RotationMatrix(vector3d axis, double angle)
		{
			matrix4d mat = new matrix4d();

			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);

			axis.Normalize();
			double x = axis[0], y = axis[1], z = axis[2];

			mat[0, 0] = cos + x * x * (1 - cos);
			mat[0, 1] = x * y * (1 - cos) - z * sin;
			mat[0, 2] = x * z * (1 - cos) + y * sin;
			mat[1, 0] = y * x * (1 - cos) + z * sin;
			mat[1, 1] = cos + y * y * (1 - cos);
			mat[1, 2] = y * z * (1 - cos) - x * sin;
			mat[2, 0] = z * x * (1 - cos) - y * sin;
			mat[2, 1] = z * y * (1 - cos) + x * sin;
			mat[2, 2] = cos + z * z * (1 - cos);
			mat[3, 3] = 1;

			return mat;
		}

	}//class-Matrix4D

	public class matrixNd
	{
		private int M, N, length;

		private double[] arr;

		public int Row
		{
			get
			{
				return this.M;
			}
		}

		public int Col
		{
			get
			{
				return this.N;
			}
		}

		public matrixNd(int nr, int nc)
		{
			M = nr;
			N = nc;
			length = M * N;
			arr = new double[length];
			for (int i = 0; i < length; ++i)
				arr[i] = 0;
		}

		public matrixNd(int nr, int nc, double[] array)
		{
			M = nr;
			N = nc;
			length = M * N;
			arr = new double[length];
			for (int i = 0; i < length; ++i)
				arr[i] = array[i];
		}

		public matrixNd(matrixNd mat)
		{
			M = mat.Row;
			N = mat.Col;
			length = M * N;
			arr = new double[length];
			for (int i = 0; i < length; ++i)
				arr[i] = mat.arr[i];
		}

		public double this[int row, int col]
		{
			get
			{
				if (row >= 0 && row < M && col >= 0 && col < N)
					return arr[row * N + col];
				else
					throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (row >= 0 && row < M && col >= 0 && col < N)
					arr[row * N + col] = value;
				else
					throw new ArgumentOutOfRangeException();
			}
		}

		public double this[int index]
		{
			get
			{
				if (index >= 0 && index < length)
					return arr[index];
				else
					throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (index >= 0 && index < length)
					arr[index] = value;
				else
					throw new ArgumentOutOfRangeException();
			}
		}

		// operators
		static public matrixNd operator +(matrixNd m1, matrixNd m2)
		{
			if(m1.M != m2.M || m1.N != m2.N)
			{
				return null;
			}
			matrixNd m = new matrixNd(m1);
			for (int i = 0; i < m1.length; ++i)
				m[i] += m2[i];
			return m;
		}

		static public matrixNd operator -(matrixNd m1, matrixNd m2)
		{
			if (m1.M != m2.M || m1.N != m2.N)
			{
				return null;
			}
			matrixNd m = new matrixNd(m1);
			for (int i = 0; i < m1.length; ++i)
				m[i] -= m2[i];
			return m;
		}

		static public matrixNd operator *(matrixNd m, double d)
		{
			matrixNd mat = new matrixNd(m);
			for (int i = 0; i < m.length; ++i)
				mat[i] *= d;
			return mat;
		}

		static public matrixNd operator /(matrixNd m, double d)
		{
			matrixNd mat = new matrixNd(m);
			if (d == 0) throw new DivideByZeroException();
			for (int i = 0; i < m.length; ++i)
				mat[i] /= d;
			return mat;
		}

		// numerics
		public matrixNd IdentityMatrix(int r, int c)
		{
			matrixNd mat = new matrixNd(r, c);
			for (int i = 0; i < r;++i )
			{
				mat[i, i] = 1.0;
			}
			return mat;
		}

		public matrixNd Transpose()
		{
			matrixNd mat = new matrixNd(N, M);
			for (int i = 0; i < M; ++i)
				for (int j = 0; j < N; ++j)
					mat[j, i] = this[i, j];
			return mat;
		}

		public double Trace()
		{
			return this[0, 0] + this[1, 1] + this[2, 2] + this[3, 3];
		}

	}//class-matrixNd
}
