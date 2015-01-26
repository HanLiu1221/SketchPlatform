using System;


namespace Geometry
{
	public class vector2d
	{
		public double x, y;

		// initialization
		public vector2d()
		{
			x = 0; 
			y = 0;
		}

		public vector2d(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public vector2d(double[] array)
		{
			if (array.Length < 2) return;
			this.x = array[0];
			this.y = array[1];
		}

		public double this[int index]
		{
			get
			{
				if (index == 0) return x;
				if (index == 1) return y;
				throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (index == 0) x = value;
				if (index == 1) y = value;
				throw new ArgumentOutOfRangeException();
			}
		}

		// functions
		public double[] ToArray()
		{
			double[] array = new double[2];
			array[0] = x;
			array[1] = y;
			return array;
		}

		public vector3d ToVector3D()
		{
			return new vector3d(x, y, 0);
		}

		public vector3d ToHomoVector3D()
		{
			return new vector3d(x, y, 1.0);
		}

		public double Length()
		{
			return Math.Sqrt(x * x + y * y);
		}

		public void Normalize()
		{
			double length = this.Length();
			x /= length;
			y /= length;
		}

		public double Dot(vector2d v)
		{
			return x * v.x + y * v.y;
		}

		public double Cross(vector2d v)
		{
			return x * v.y - y * v.x;
		}

		public bool Equals(vector2d v)
		{
			return (x == v.x) && (y == v.y) ? true : false;
		}

		// following the guiderlines of implementing operator == and overide Equals, GetHashCode
		// to avoid warning...
		public override bool Equals(Object obj)
		{
			if (obj == null) return false;
			vector2d v = obj as vector2d;
			if ((Object)v == null) return false;
			return (x == v.x) && (y == v.y) ? true : false;
		}

		public override int GetHashCode()
		{ 
			return base.GetHashCode();
		}

		// Operators
		static public vector2d operator +(vector2d v1, vector2d v2)
		{
			return new vector2d(v1.x + v2.x, v1.y + v2.y);
		}

		static public vector2d operator -(vector2d v1, vector2d v2)
		{
			return new vector2d(v1.x - v2.x, v1.y - v2.y);
		}

		static public vector2d operator *(vector2d v, double d)
		{
			return new vector2d(v.x * d, v.y * d);
		}

		static public vector2d operator /(vector2d v, double d)
		{
			return new vector2d(v.x / d, v.y / d);
		}

		static public bool operator ==(vector2d v1, vector2d v2)
		{
			return (v1.x == v2.x) && (v1.y == v2.y) ? true : false;
		}

		static public bool operator !=(vector2d v1, vector2d v2)
		{
			return !(v1 == v2);
		}
	}//class-vector2d

	public class vector3d
	{
		public double x, y, z;

		// initialization
		public vector3d()
		{
			x = 0; 
			y = 0; 
			z = 0;
		}

		public vector3d(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public vector3d(double[] array)
		{
			if (array.Length < 3) return;
			x = array[0];
			y = array[1];
			z = array[2];
		}

		public double this[int index]
		{
			get
			{
				if (index == 0) return x;
				if (index == 1) return y;
				if (index == 2) return z;
				throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (index == 0) x = value;
				if (index == 1) y = value;
				if (index == 2) z = value;
				throw new ArgumentOutOfRangeException();
			}
		}

		// functions
		public double[] ToArray()
		{
			double[] array = new double[3];
			array[0] = x;
			array[1] = y;
			array[2] = z;
			return array;
		}

		public vector2d ToVector2D()
		{
			return new vector2d(x, y);
		}

		public vector2d HomogeneousVector2D()
		{
			if (z == 0) return ToVector2D();
			return new vector2d(x / z, y / z);
		}

		public double Length()
		{
			return Math.Sqrt(x * x + y * y + z * z);
		}

		public void Normalize()
		{
			double length = this.Length();
			x /= length;
			y /= length;
			z /= length;
		}

		public void HomogeneousNormalize()
		{
			if (z == 0) return;
			x /= z;
			y /= z;
			z = 1.0;
		}

		public double Dot(vector3d v)
		{
			return x * v.x + y * v.y + z * v.z;
		}

		public vector3d Cross(vector3d v)
		{
			return new vector3d(
				y * v.z - z * v.y,
				z * v.x - x * v.z,
				x * v.y - y * v.x);
		}

		public bool Equals(vector3d v)
		{
			return (x == v.x) && (y == v.y) && (z == v.z) ? true : false;
		}

		// following the guiderlines of implementing operator == and overide Equals, GetHashCode
		// to avoid warning...
		public override bool Equals(Object obj)
		{
			if (obj == null) return false;
			vector3d v = obj as vector3d;
			if ((Object)v == null) return false;
			return (x == v.x) && (y == v.y) && (z == v.z) ? true : false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Operators
		static public vector3d operator +(vector3d v1, vector3d v2)
		{
			return new vector3d(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
		}

		static public vector3d operator -(vector3d v1, vector3d v2)
		{
			return new vector3d(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
		}

		static public vector3d operator *(vector3d v, double d)
		{
			return new vector3d(v.x * d, v.y * d, v.z * d);
		}

		static public vector3d operator /(vector3d v, double d)
		{
			return new vector3d(v.x / d, v.y / d, v.z / d);
		}

		static public bool operator ==(vector3d v1, vector3d v2)
		{
			return (v1.x == v2.x) && (v1.y == v2.y) && (v1.z == v2.z) ? true : false;
		}

		static public bool operator !=(vector3d v1, vector3d v2)
		{
			return !(v1 == v2);
		}
	}//class-vector3d

	public class vector4d
	{ 
		public double x, y, z, w;

		// initialization
		public vector4d()
		{
			x = 0; y = 0; z = 0;
		}

		public vector4d(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public vector4d(double[] array)
		{
			if (array.Length < 4) return;
			x = array[0];
			y = array[1];
			z = array[2];
			w = array[3];
		}

		public double this[int index]
		{
			get
			{
				if (index == 0) return x;
				if (index == 1) return y;
				if (index == 2) return z;
				if (index == 3) return w;
				throw new ArgumentOutOfRangeException();
			}
			set
			{
				if (index == 0) x = value;
				if (index == 1) y = value;
				if (index == 2) z = value;
				if (index == 3) w = value;
				throw new ArgumentOutOfRangeException();
			}
		}

		// functions
		public double[] ToArray()
		{
			double[] array = new double[4];
			array[0] = x;
			array[1] = y;
			array[2] = z;
			array[3] = w;
			return array;
		}

		public vector3d ToVector3D()
		{
			return new vector3d(x, y, z);
				
		}

		public vector3d ToHomogeneousVector()
		{
			if (w == 0) return ToVector3D();
			return new vector3d(x / w, y / w, z / w);
		}

		public double Length()
		{
			return Math.Sqrt(x * x + y * y + z * z + w * w);
		}

		public void Normalize()
		{
			double length = this.Length();
			x /= length;
			y /= length;
			z /= length;
			z /= length;
		}

		public void HomogeneousNormalize()
		{
			if (w == 0) return;
			x /= w;
			y /= w;
			z /= w;
			w = 1.0;
		}

		public double Dot(vector4d v)
		{
			return x * v.x + y * v.y + z * v.z + z * v.w;
		}


		public bool Equals(vector4d v)
		{
			return (x == v.x) && (y == v.y) && (z == v.z) && (w == v.w) ? true : false;
		}

		// Operators
		static public vector4d operator +(vector4d v1, vector4d v2)
		{
			return new vector4d(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
		}

		static public vector4d operator -(vector4d v1, vector4d v2)
		{
			return new vector4d(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
		}

		static public vector4d operator *(vector4d v, double d)
		{
			return new vector4d(v.x * d, v.y * d, v.z * d, v.w * d);
		}

		static public vector4d operator /(vector4d v, double d)
		{
			return new vector4d(v.x / d, v.y / d, v.z / d, v.w / d);
		}

		static public bool operator ==(vector4d v1, vector4d v2)
		{
			return (v1.x == v2.x) && (v1.y == v2.y) 
				&& (v1.z == v2.z) && (v1.w == v2.w)? true : false;
		}

		static public bool operator !=(vector4d v1, vector4d v2)
		{
			return !(v1 == v2);
		}

	}//class-vector4d

	public class vectorNd
	{
		public double[] val;
		int dim = 0;
		// initialization
		public vectorNd(int n)
		{
			val = new double[n];
			dim = n;
		}

		public vectorNd(double[] array)
		{
			if (array == null) return;
			dim = array.Length;
			this.val = new double[dim];
			for (int i = 0; i < dim; ++i)
			{
				this.val[i] = array[i];
			}
		}

		public double this[int index]
		{
			get
			{
				if(val != null && index < dim)
				{
					return this.val[index];
				}
				throw new ArgumentOutOfRangeException();
			}
			set
			{
				if(val!= null)
				{
					this.val[index] = value;
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		// functions
		public double[] ToArray()
		{
			if(val == null)
			{
				return null;
			}
			double[] array = new double[dim];
			for (int i = 0; i < dim;++i)
			{
				array[i] = val[i];
			}
			return array;
		}

		public double Length()
		{
			double sum = 0.0;
			for (int i = 0; i < dim; ++i)
			{
				sum += val[i];
			}
			return Math.Sqrt(sum);
		}

		public void Normalize()
		{
			double length = this.Length();
			for (int i = 0; i < dim; ++i)
			{
				val[i] /= length;
			}
		}

		public double Dot(vectorNd v)
		{
			if(v.dim != this.dim)
			{
				return 0;
			}
			double sum = 0.0;
			for (int i = 0; i < dim; ++i)
			{
				sum += val[i] * v[i];
			}
			return sum;
		}
		
		public bool Equals(vectorNd v)
		{
			bool isEqual = true;
			for (int i = 0; i < dim; ++i)
			{
				if(Math.Abs(val[i]-v[i]) < 1e-6)
				{
					isEqual = false;
					break;
				}
			}
			return isEqual;
		}

		// Operators
		static public vectorNd operator +(vectorNd v1, vectorNd v2)
		{
			if(v1.dim!=v2.dim)
			{
				return null;
			}
			vectorNd v = new vectorNd(v1.dim);
			for (int i = 0; i < v.dim; ++i)
			{
				v[i] = v1[i] + v2[i];
			}
			return v;
		}

		static public vectorNd operator -(vectorNd v1, vectorNd v2)
		{
			if (v1.dim != v2.dim)
			{
				return null;
			}
			vectorNd v = new vectorNd(v1.dim);
			for (int i = 0; i < v.dim; ++i)
			{
				v[i] = v1[i] - v2[i];
			}
			return v;
		}

		static public vectorNd operator *(vectorNd v, double d)
		{
			vectorNd vn = new vectorNd(v.dim);
			for (int i = 0; i < vn.dim; ++i)
			{
				vn[i] = v[i] * d;
			}
			return vn;
		}

		static public vectorNd operator /(vectorNd v, double d)
		{
			if(Math.Abs(d) < 1e-6)
			{
				throw new DivideByZeroException();
			}
			vectorNd vn = new vectorNd(v.dim);
			for (int i = 0; i < vn.dim; ++i)
			{
				vn[i] = v[i] / d;
			}
			return vn;
		}

		static public bool operator ==(vectorNd v1, vectorNd v2)
		{
			bool isEqual = true;
			for (int i = 0; i < v1.dim; ++i)
			{
				if (Math.Abs(v1[i] - v2[i]) < 1e-6)
				{
					isEqual = false;
					break;
				}
			}
			return isEqual;
		}

		static public bool operator !=(vectorNd v1, vectorNd v2)
		{
			return !(v1 == v2);
		}
	}//class-vectorNd
}
