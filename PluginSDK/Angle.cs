using System;
using System.Globalization;

namespace WorldWind
{
	/// <summary>
	/// A geometric angle
	/// </summary>
	public struct Angle
	{
		[NonSerialized]
		public double Radians;

		/// <summary>
		/// Creates a new angle from angle in radians.
		/// </summary>
		public static Angle FromRadians(double radians)
		{
			Angle res = new Angle();
			res.Radians = radians;	
			return res;
		}

		/// <summary>
		/// Creates a new angle from angle in degrees.
		/// </summary>
		public static Angle FromDegrees(double degrees)
		{
			Angle res = new Angle();
			res.Radians = Math.PI * degrees / 180.0;
			return res;
		}

		/// <summary>
		/// A zeroed angle
		/// </summary>
		public static readonly Angle Zero;

		/// <summary>
		/// Minimum value for angle
		/// </summary>
		internal static readonly Angle MinValue = Angle.FromRadians(double.MinValue);

		/// <summary>
		/// Maximum value for angle
		/// </summary>
		internal static readonly Angle MaxValue = Angle.FromRadians(double.MaxValue);

		/// <summary>
		/// Angle containing Not a Number
		/// </summary>
		public static readonly Angle NaN = Angle.FromRadians(double.NaN);

		public double Degrees
		{
			get { return MathEngine.RadiansToDegrees(this.Radians);}
			set { this.Radians = MathEngine.DegreesToRadians(value); }
		}

		/// <summary>
		/// Checks for angle containing "Not a Number"
		/// </summary>
		public static bool IsNaN(Angle a)
		{
			return double.IsNaN(a.Radians);
		}

		public override bool Equals(object obj) 
		{
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			Angle a = (Angle)obj;
			return Math.Abs(Radians - a.Radians) < Single.Epsilon;
		}

		public static bool operator ==(Angle a, Angle b)
		{
			return Math.Abs(a.Radians - b.Radians) < Single.Epsilon;
		}

		public static bool operator !=(Angle a, Angle b)
		{
			return Math.Abs(a.Radians - b.Radians) > Single.Epsilon;
		}

		public static bool operator <(Angle a, Angle b) 
		{
			return a.Radians < b.Radians;
		}

		public static bool operator >(Angle a, Angle b) 
		{
			return a.Radians > b.Radians;
		}

		public static Angle operator +(Angle a, Angle b) 
		{
			double res = a.Radians + b.Radians;
			return Angle.FromRadians(res);
		}

		public static Angle operator -(Angle a, Angle b) 
		{
			double res = a.Radians - b.Radians;
			return Angle.FromRadians(res);
		}

		public static Angle operator *(Angle a, double times) 
		{
			return Angle.FromRadians(a.Radians * times);
		}

		public static Angle operator *(double times, Angle a) 
		{
			return Angle.FromRadians(a.Radians * times);
		}

		public static Angle operator /(double divisor, Angle a) 
		{
			return Angle.FromRadians(a.Radians / divisor);
		}

		public static Angle operator /(Angle a, double divisor) 
		{
			return Angle.FromRadians(a.Radians / divisor);
		}

		public override int GetHashCode() 
		{
			return (int)(Radians*100000);
		}

		public override string ToString()
		{
			return Degrees.ToString(CultureInfo.InvariantCulture)+"�";
		}
	}
}
