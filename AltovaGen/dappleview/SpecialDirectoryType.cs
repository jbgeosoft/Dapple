//
// SpecialDirectoryType.cs
//
// This file was generated by XMLSpy 2006r3 Enterprise Edition.
//
// YOU SHOULD NOT MODIFY THIS FILE, BECAUSE IT WILL BE
// OVERWRITTEN WHEN YOU RE-RUN CODE GENERATION.
//
// Refer to the XMLSpy Documentation for further details.
// http://www.altova.com/xmlspy
//


using Altova.Types;

namespace dappleview
{

	public class SpecialDirectoryType : SchemaString
	{

		public static string[] sEnumValues = {
			"DAPServers",
			"ImageServers",
			"WMSServers",
		};

		public enum EnumValues
		{
			eDAPServers = 0, /* DAPServers */
			eImageServers = 1, /* ImageServers */
			eWMSServers = 2, /* WMSServers */
			EnumValueCount
		};

		public SpecialDirectoryType() : base()
		{
		}

		public SpecialDirectoryType(string newValue) : base(newValue)
		{
			Validate();
		}

		public SpecialDirectoryType(SchemaString newValue) : base(newValue)
		{
			Validate();
		}


		#region Documentation
		public static string GetAnnoDocumentation()
		{
			return "";
		}
		#endregion

		public static int GetEnumerationCount()
		{
			return sEnumValues.Length;
		}

		public static string GetEnumerationValue(int index)
		{
			return sEnumValues[index];
		}

		public static bool IsValidEnumerationValue(string val)
		{
			foreach (string s in sEnumValues)
			{
				if (val == s)
					return true;
			}
			return false;
		}

		public void Validate()
		{

			if (!IsValidEnumerationValue(ToString()))
				throw new System.Exception("Value of SpecialDirectoryType is invalid.");
		}
	}
}
