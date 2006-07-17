//
// ContactAddressType.cs
//
// This file was generated by XMLSpy 2006r3 Enterprise Edition.
//
// YOU SHOULD NOT MODIFY THIS FILE, BECAUSE IT WILL BE
// OVERWRITTEN WHEN YOU RE-RUN CODE GENERATION.
//
// Refer to the XMLSpy Documentation for further details.
// http://www.altova.com/xmlspy
//


using System;
using System.Collections;
using System.Xml;
using Altova.Types;

namespace capabilities_1_3_0.wms
{
	public class ContactAddressType : Altova.Xml.Node
	{
		#region Forward constructors

		public ContactAddressType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public ContactAddressType(XmlNode node) : base(node) { SetCollectionParents(); }
		public ContactAddressType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public ContactAddressType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "AddressType" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "AddressType", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Address" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Address", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "City" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "City", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "PostCode" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "PostCode", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Country" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Country", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}
		}



		#region AddressType accessor methods
		public static int GetAddressTypeMinCount()
		{
			return 1;
		}

		public static int AddressTypeMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetAddressTypeMaxCount()
		{
			return 1;
		}

		public static int AddressTypeMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetAddressTypeCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "AddressType");
		}

		public int AddressTypeCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "AddressType");
			}
		}

		public bool HasAddressType()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "AddressType");
		}

		public SchemaString NewAddressType()
		{
			return new SchemaString();
		}

		public SchemaString GetAddressTypeAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "AddressType", index)));
		}

		public XmlNode GetStartingAddressTypeCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "AddressType" );
		}

		public XmlNode GetAdvancedAddressTypeCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "AddressType", curNode );
		}

		public SchemaString GetAddressTypeValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetAddressType()
		{
			return GetAddressTypeAt(0);
		}

		public SchemaString AddressType
		{
			get
			{
				return GetAddressTypeAt(0);
			}
		}

		public void RemoveAddressTypeAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "AddressType", index);
		}

		public void RemoveAddressType()
		{
			while (HasAddressType())
				RemoveAddressTypeAt(0);
		}

		public void AddAddressType(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "AddressType", newValue.ToString());
		}

		public void InsertAddressTypeAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "AddressType", index, newValue.ToString());
		}

		public void ReplaceAddressTypeAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "AddressType", index, newValue.ToString());
		}
		#endregion // AddressType accessor methods

		#region AddressType collection
        public AddressTypeCollection	MyAddressTypes = new AddressTypeCollection( );

        public class AddressTypeCollection: IEnumerable
        {
            ContactAddressType parent;
            public ContactAddressType Parent
			{
				set
				{
					parent = value;
				}
			}
			public AddressTypeEnumerator GetEnumerator() 
			{
				return new AddressTypeEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class AddressTypeEnumerator: IEnumerator 
        {
			int nIndex;
			ContactAddressType parent;
			public AddressTypeEnumerator(ContactAddressType par) 
			{
				parent = par;
				nIndex = -1;
			}
			public void Reset() 
			{
				nIndex = -1;
			}
			public bool MoveNext() 
			{
				nIndex++;
				return(nIndex < parent.AddressTypeCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetAddressTypeAt(nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(Current);
				}
			}
    	}

        #endregion // AddressType collection

		#region Address accessor methods
		public static int GetAddressMinCount()
		{
			return 1;
		}

		public static int AddressMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetAddressMaxCount()
		{
			return 1;
		}

		public static int AddressMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetAddressCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Address");
		}

		public int AddressCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Address");
			}
		}

		public bool HasAddress()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Address");
		}

		public SchemaString NewAddress()
		{
			return new SchemaString();
		}

		public SchemaString GetAddressAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Address", index)));
		}

		public XmlNode GetStartingAddressCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Address" );
		}

		public XmlNode GetAdvancedAddressCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Address", curNode );
		}

		public SchemaString GetAddressValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetAddress()
		{
			return GetAddressAt(0);
		}

		public SchemaString Address
		{
			get
			{
				return GetAddressAt(0);
			}
		}

		public void RemoveAddressAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Address", index);
		}

		public void RemoveAddress()
		{
			while (HasAddress())
				RemoveAddressAt(0);
		}

		public void AddAddress(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "Address", newValue.ToString());
		}

		public void InsertAddressAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Address", index, newValue.ToString());
		}

		public void ReplaceAddressAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Address", index, newValue.ToString());
		}
		#endregion // Address accessor methods

		#region Address collection
        public AddressCollection	MyAddresss = new AddressCollection( );

        public class AddressCollection: IEnumerable
        {
            ContactAddressType parent;
            public ContactAddressType Parent
			{
				set
				{
					parent = value;
				}
			}
			public AddressEnumerator GetEnumerator() 
			{
				return new AddressEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class AddressEnumerator: IEnumerator 
        {
			int nIndex;
			ContactAddressType parent;
			public AddressEnumerator(ContactAddressType par) 
			{
				parent = par;
				nIndex = -1;
			}
			public void Reset() 
			{
				nIndex = -1;
			}
			public bool MoveNext() 
			{
				nIndex++;
				return(nIndex < parent.AddressCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetAddressAt(nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(Current);
				}
			}
    	}

        #endregion // Address collection

		#region City accessor methods
		public static int GetCityMinCount()
		{
			return 1;
		}

		public static int CityMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetCityMaxCount()
		{
			return 1;
		}

		public static int CityMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetCityCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "City");
		}

		public int CityCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "City");
			}
		}

		public bool HasCity()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "City");
		}

		public SchemaString NewCity()
		{
			return new SchemaString();
		}

		public SchemaString GetCityAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "City", index)));
		}

		public XmlNode GetStartingCityCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "City" );
		}

		public XmlNode GetAdvancedCityCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "City", curNode );
		}

		public SchemaString GetCityValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetCity()
		{
			return GetCityAt(0);
		}

		public SchemaString City
		{
			get
			{
				return GetCityAt(0);
			}
		}

		public void RemoveCityAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "City", index);
		}

		public void RemoveCity()
		{
			while (HasCity())
				RemoveCityAt(0);
		}

		public void AddCity(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "City", newValue.ToString());
		}

		public void InsertCityAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "City", index, newValue.ToString());
		}

		public void ReplaceCityAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "City", index, newValue.ToString());
		}
		#endregion // City accessor methods

		#region City collection
        public CityCollection	MyCitys = new CityCollection( );

        public class CityCollection: IEnumerable
        {
            ContactAddressType parent;
            public ContactAddressType Parent
			{
				set
				{
					parent = value;
				}
			}
			public CityEnumerator GetEnumerator() 
			{
				return new CityEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class CityEnumerator: IEnumerator 
        {
			int nIndex;
			ContactAddressType parent;
			public CityEnumerator(ContactAddressType par) 
			{
				parent = par;
				nIndex = -1;
			}
			public void Reset() 
			{
				nIndex = -1;
			}
			public bool MoveNext() 
			{
				nIndex++;
				return(nIndex < parent.CityCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetCityAt(nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(Current);
				}
			}
    	}

        #endregion // City collection

		#region StateOrProvince accessor methods
		public static int GetStateOrProvinceMinCount()
		{
			return 1;
		}

		public static int StateOrProvinceMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetStateOrProvinceMaxCount()
		{
			return 1;
		}

		public static int StateOrProvinceMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetStateOrProvinceCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince");
		}

		public int StateOrProvinceCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince");
			}
		}

		public bool HasStateOrProvince()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince");
		}

		public SchemaString NewStateOrProvince()
		{
			return new SchemaString();
		}

		public SchemaString GetStateOrProvinceAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince", index)));
		}

		public XmlNode GetStartingStateOrProvinceCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince" );
		}

		public XmlNode GetAdvancedStateOrProvinceCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince", curNode );
		}

		public SchemaString GetStateOrProvinceValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetStateOrProvince()
		{
			return GetStateOrProvinceAt(0);
		}

		public SchemaString StateOrProvince
		{
			get
			{
				return GetStateOrProvinceAt(0);
			}
		}

		public void RemoveStateOrProvinceAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince", index);
		}

		public void RemoveStateOrProvince()
		{
			while (HasStateOrProvince())
				RemoveStateOrProvinceAt(0);
		}

		public void AddStateOrProvince(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince", newValue.ToString());
		}

		public void InsertStateOrProvinceAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince", index, newValue.ToString());
		}

		public void ReplaceStateOrProvinceAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StateOrProvince", index, newValue.ToString());
		}
		#endregion // StateOrProvince accessor methods

		#region StateOrProvince collection
        public StateOrProvinceCollection	MyStateOrProvinces = new StateOrProvinceCollection( );

        public class StateOrProvinceCollection: IEnumerable
        {
            ContactAddressType parent;
            public ContactAddressType Parent
			{
				set
				{
					parent = value;
				}
			}
			public StateOrProvinceEnumerator GetEnumerator() 
			{
				return new StateOrProvinceEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class StateOrProvinceEnumerator: IEnumerator 
        {
			int nIndex;
			ContactAddressType parent;
			public StateOrProvinceEnumerator(ContactAddressType par) 
			{
				parent = par;
				nIndex = -1;
			}
			public void Reset() 
			{
				nIndex = -1;
			}
			public bool MoveNext() 
			{
				nIndex++;
				return(nIndex < parent.StateOrProvinceCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetStateOrProvinceAt(nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(Current);
				}
			}
    	}

        #endregion // StateOrProvince collection

		#region PostCode accessor methods
		public static int GetPostCodeMinCount()
		{
			return 1;
		}

		public static int PostCodeMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetPostCodeMaxCount()
		{
			return 1;
		}

		public static int PostCodeMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetPostCodeCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "PostCode");
		}

		public int PostCodeCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "PostCode");
			}
		}

		public bool HasPostCode()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "PostCode");
		}

		public SchemaString NewPostCode()
		{
			return new SchemaString();
		}

		public SchemaString GetPostCodeAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "PostCode", index)));
		}

		public XmlNode GetStartingPostCodeCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "PostCode" );
		}

		public XmlNode GetAdvancedPostCodeCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "PostCode", curNode );
		}

		public SchemaString GetPostCodeValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetPostCode()
		{
			return GetPostCodeAt(0);
		}

		public SchemaString PostCode
		{
			get
			{
				return GetPostCodeAt(0);
			}
		}

		public void RemovePostCodeAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "PostCode", index);
		}

		public void RemovePostCode()
		{
			while (HasPostCode())
				RemovePostCodeAt(0);
		}

		public void AddPostCode(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "PostCode", newValue.ToString());
		}

		public void InsertPostCodeAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "PostCode", index, newValue.ToString());
		}

		public void ReplacePostCodeAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "PostCode", index, newValue.ToString());
		}
		#endregion // PostCode accessor methods

		#region PostCode collection
        public PostCodeCollection	MyPostCodes = new PostCodeCollection( );

        public class PostCodeCollection: IEnumerable
        {
            ContactAddressType parent;
            public ContactAddressType Parent
			{
				set
				{
					parent = value;
				}
			}
			public PostCodeEnumerator GetEnumerator() 
			{
				return new PostCodeEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class PostCodeEnumerator: IEnumerator 
        {
			int nIndex;
			ContactAddressType parent;
			public PostCodeEnumerator(ContactAddressType par) 
			{
				parent = par;
				nIndex = -1;
			}
			public void Reset() 
			{
				nIndex = -1;
			}
			public bool MoveNext() 
			{
				nIndex++;
				return(nIndex < parent.PostCodeCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetPostCodeAt(nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(Current);
				}
			}
    	}

        #endregion // PostCode collection

		#region Country accessor methods
		public static int GetCountryMinCount()
		{
			return 1;
		}

		public static int CountryMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetCountryMaxCount()
		{
			return 1;
		}

		public static int CountryMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetCountryCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Country");
		}

		public int CountryCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Country");
			}
		}

		public bool HasCountry()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Country");
		}

		public SchemaString NewCountry()
		{
			return new SchemaString();
		}

		public SchemaString GetCountryAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Country", index)));
		}

		public XmlNode GetStartingCountryCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Country" );
		}

		public XmlNode GetAdvancedCountryCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Country", curNode );
		}

		public SchemaString GetCountryValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetCountry()
		{
			return GetCountryAt(0);
		}

		public SchemaString Country
		{
			get
			{
				return GetCountryAt(0);
			}
		}

		public void RemoveCountryAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Country", index);
		}

		public void RemoveCountry()
		{
			while (HasCountry())
				RemoveCountryAt(0);
		}

		public void AddCountry(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "Country", newValue.ToString());
		}

		public void InsertCountryAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Country", index, newValue.ToString());
		}

		public void ReplaceCountryAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Country", index, newValue.ToString());
		}
		#endregion // Country accessor methods

		#region Country collection
        public CountryCollection	MyCountrys = new CountryCollection( );

        public class CountryCollection: IEnumerable
        {
            ContactAddressType parent;
            public ContactAddressType Parent
			{
				set
				{
					parent = value;
				}
			}
			public CountryEnumerator GetEnumerator() 
			{
				return new CountryEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class CountryEnumerator: IEnumerator 
        {
			int nIndex;
			ContactAddressType parent;
			public CountryEnumerator(ContactAddressType par) 
			{
				parent = par;
				nIndex = -1;
			}
			public void Reset() 
			{
				nIndex = -1;
			}
			public bool MoveNext() 
			{
				nIndex++;
				return(nIndex < parent.CountryCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetCountryAt(nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(Current);
				}
			}
    	}

        #endregion // Country collection

        private void SetCollectionParents()
        {
            MyAddressTypes.Parent = this; 
            MyAddresss.Parent = this; 
            MyCitys.Parent = this; 
            MyStateOrProvinces.Parent = this; 
            MyPostCodes.Parent = this; 
            MyCountrys.Parent = this; 
	}
}
}