//
// EnumerationType2.cs
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

namespace WMS_MS_Capabilities
{

	public class EnumerationType2 : SchemaString
	{
		public static  string[] sEnumValues = {
			"0",
			"1",
		};

		public  enum EnumValues
		{
			e0 = 0, /* 0 */
			e1 = 1, /* 1 */
			EnumValueCount
		};

		public EnumerationType2() : base()
		{
		}

		public EnumerationType2(string newValue) : base(newValue)
		{
			Validate();
		}

		public EnumerationType2(SchemaString newValue) : base(newValue)
		{
			Validate();
		}


		#region Documentation
		public static string GetAnnoDocumentation()
		{
			return "";
		}
		#endregion

		public static  int GetEnumerationCount()
		{
			return sEnumValues.Length;
		}

		public static  string GetEnumerationValue(int index)
		{
			return sEnumValues[index];
		}

		public static  bool IsValidEnumerationValue(string val)
		{
			foreach (string s in sEnumValues)
			{
				if (val == s)
					return true;
			}
			return false;
		}

		public  void Validate()
		{

			if (!IsValidEnumerationValue(ToString()))
				throw new System.Exception("Value of Enumeration is invalid.");
		}
	}
}
