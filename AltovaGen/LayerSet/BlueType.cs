//
// BlueType.cs
//
// This file was generated by XMLSpy 2007r3 Enterprise Edition.
//
// YOU SHOULD NOT MODIFY THIS FILE, BECAUSE IT WILL BE
// OVERWRITTEN WHEN YOU RE-RUN CODE GENERATION.
//
// Refer to the XMLSpy Documentation for further details.
// http://www.altova.com/xmlspy
//


using Altova.Types;

namespace LayerSet
{

	public class BlueType : SchemaInt
	{

		public BlueType() : base()
		{
		}

		public BlueType(string newValue) : base(newValue)
		{
			Validate();
		}

		public BlueType(SchemaInt newValue) : base(newValue)
		{
			Validate();
		}


		#region Documentation
		public static string GetAnnoDocumentation()
		{
			return "";
		}
		#endregion

		public  void Validate()
		{

			if (CompareTo(GetMaxInclusive()) > 0)
				throw new System.Exception("Value of Blue is out of range.");

			if (CompareTo(GetMinInclusive()) < 0)
				throw new System.Exception("Value of Blue is out of range.");
		}
		public  SchemaInt GetMaxInclusive()
		{
			return new SchemaInt("255");
		}
		public  SchemaInt GetMinInclusive()
		{
			return new SchemaInt("0");
		}
	}
}
