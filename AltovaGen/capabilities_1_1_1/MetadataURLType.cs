//
// MetadataURLType.cs
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

namespace capabilities_1_1_1
{
	public class MetadataURLType : Altova.Xml.Node
	{
		#region Forward constructors

		public MetadataURLType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public MetadataURLType(XmlNode node) : base(node) { SetCollectionParents(); }
		public MetadataURLType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public MetadataURLType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "type" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "type", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "", "Format" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "", "Format", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
				new FormatType(DOMNode).AdjustPrefix();
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "", "OnlineResource" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "", "OnlineResource", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
				new OnlineResourceType(DOMNode).AdjustPrefix();
			}
		}



		#region type2 accessor methods
		public static int Gettype2MinCount()
		{
			return 1;
		}

		public static int type2MinCount
		{
			get
			{
				return 1;
			}
		}

		public static int Gettype2MaxCount()
		{
			return 1;
		}

		public static int type2MaxCount
		{
			get
			{
				return 1;
			}
		}

		public int Gettype2Count()
		{
			return DomChildCount(NodeType.Attribute, "", "type");
		}

		public int type2Count
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "type");
			}
		}

		public bool Hastype2()
		{
			return HasDomChild(NodeType.Attribute, "", "type");
		}

		public EnumerationType7 Newtype2()
		{
			return new EnumerationType7();
		}

		public EnumerationType7 Gettype2At(int index)
		{
			return new EnumerationType7(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "type", index)));
		}

		public XmlNode GetStartingtype2Cursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "type" );
		}

		public XmlNode GetAdvancedtype2Cursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "type", curNode );
		}

		public EnumerationType7 Gettype2ValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new EnumerationType7( curNode.Value );
		}


		public EnumerationType7 Gettype2()
		{
			return Gettype2At(0);
		}

		public EnumerationType7 type2
		{
			get
			{
				return Gettype2At(0);
			}
		}

		public void Removetype2At(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "type", index);
		}

		public void Removetype2()
		{
			while (Hastype2())
				Removetype2At(0);
		}

		public void Addtype2(EnumerationType7 newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Attribute, "", "type", newValue.ToString());
		}

		public void Inserttype2At(EnumerationType7 newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "type", index, newValue.ToString());
		}

		public void Replacetype2At(EnumerationType7 newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "type", index, newValue.ToString());
		}
		#endregion // type2 accessor methods

		#region type2 collection
        public type2Collection	Mytype2s = new type2Collection( );

        public class type2Collection: IEnumerable
        {
            MetadataURLType parent;
            public MetadataURLType Parent
			{
				set
				{
					parent = value;
				}
			}
			public type2Enumerator GetEnumerator() 
			{
				return new type2Enumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class type2Enumerator: IEnumerator 
        {
			int nIndex;
			MetadataURLType parent;
			public type2Enumerator(MetadataURLType par) 
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
				return(nIndex < parent.type2Count );
			}
			public EnumerationType7  Current 
			{
				get 
				{
					return(parent.Gettype2At(nIndex));
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

        #endregion // type2 collection

		#region Format accessor methods
		public static int GetFormatMinCount()
		{
			return 1;
		}

		public static int FormatMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetFormatMaxCount()
		{
			return 1;
		}

		public static int FormatMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetFormatCount()
		{
			return DomChildCount(NodeType.Element, "", "Format");
		}

		public int FormatCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "", "Format");
			}
		}

		public bool HasFormat()
		{
			return HasDomChild(NodeType.Element, "", "Format");
		}

		public FormatType NewFormat()
		{
			return new FormatType(domNode.OwnerDocument.CreateElement("Format", ""));
		}

		public FormatType GetFormatAt(int index)
		{
			return new FormatType(GetDomChildAt(NodeType.Element, "", "Format", index));
		}

		public XmlNode GetStartingFormatCursor()
		{
			return GetDomFirstChild( NodeType.Element, "", "Format" );
		}

		public XmlNode GetAdvancedFormatCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "", "Format", curNode );
		}

		public FormatType GetFormatValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new FormatType( curNode );
		}


		public FormatType GetFormat()
		{
			return GetFormatAt(0);
		}

		public FormatType Format
		{
			get
			{
				return GetFormatAt(0);
			}
		}

		public void RemoveFormatAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "", "Format", index);
		}

		public void RemoveFormat()
		{
			while (HasFormat())
				RemoveFormatAt(0);
		}

		public void AddFormat(FormatType newValue)
		{
			AppendDomElement("", "Format", newValue);
		}

		public void InsertFormatAt(FormatType newValue, int index)
		{
			InsertDomElementAt("", "Format", index, newValue);
		}

		public void ReplaceFormatAt(FormatType newValue, int index)
		{
			ReplaceDomElementAt("", "Format", index, newValue);
		}
		#endregion // Format accessor methods

		#region Format collection
        public FormatCollection	MyFormats = new FormatCollection( );

        public class FormatCollection: IEnumerable
        {
            MetadataURLType parent;
            public MetadataURLType Parent
			{
				set
				{
					parent = value;
				}
			}
			public FormatEnumerator GetEnumerator() 
			{
				return new FormatEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class FormatEnumerator: IEnumerator 
        {
			int nIndex;
			MetadataURLType parent;
			public FormatEnumerator(MetadataURLType par) 
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
				return(nIndex < parent.FormatCount );
			}
			public FormatType  Current 
			{
				get 
				{
					return(parent.GetFormatAt(nIndex));
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

        #endregion // Format collection

		#region OnlineResource accessor methods
		public static int GetOnlineResourceMinCount()
		{
			return 1;
		}

		public static int OnlineResourceMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetOnlineResourceMaxCount()
		{
			return 1;
		}

		public static int OnlineResourceMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetOnlineResourceCount()
		{
			return DomChildCount(NodeType.Element, "", "OnlineResource");
		}

		public int OnlineResourceCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "", "OnlineResource");
			}
		}

		public bool HasOnlineResource()
		{
			return HasDomChild(NodeType.Element, "", "OnlineResource");
		}

		public OnlineResourceType NewOnlineResource()
		{
			return new OnlineResourceType(domNode.OwnerDocument.CreateElement("OnlineResource", ""));
		}

		public OnlineResourceType GetOnlineResourceAt(int index)
		{
			return new OnlineResourceType(GetDomChildAt(NodeType.Element, "", "OnlineResource", index));
		}

		public XmlNode GetStartingOnlineResourceCursor()
		{
			return GetDomFirstChild( NodeType.Element, "", "OnlineResource" );
		}

		public XmlNode GetAdvancedOnlineResourceCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "", "OnlineResource", curNode );
		}

		public OnlineResourceType GetOnlineResourceValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new OnlineResourceType( curNode );
		}


		public OnlineResourceType GetOnlineResource()
		{
			return GetOnlineResourceAt(0);
		}

		public OnlineResourceType OnlineResource
		{
			get
			{
				return GetOnlineResourceAt(0);
			}
		}

		public void RemoveOnlineResourceAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "", "OnlineResource", index);
		}

		public void RemoveOnlineResource()
		{
			while (HasOnlineResource())
				RemoveOnlineResourceAt(0);
		}

		public void AddOnlineResource(OnlineResourceType newValue)
		{
			AppendDomElement("", "OnlineResource", newValue);
		}

		public void InsertOnlineResourceAt(OnlineResourceType newValue, int index)
		{
			InsertDomElementAt("", "OnlineResource", index, newValue);
		}

		public void ReplaceOnlineResourceAt(OnlineResourceType newValue, int index)
		{
			ReplaceDomElementAt("", "OnlineResource", index, newValue);
		}
		#endregion // OnlineResource accessor methods

		#region OnlineResource collection
        public OnlineResourceCollection	MyOnlineResources = new OnlineResourceCollection( );

        public class OnlineResourceCollection: IEnumerable
        {
            MetadataURLType parent;
            public MetadataURLType Parent
			{
				set
				{
					parent = value;
				}
			}
			public OnlineResourceEnumerator GetEnumerator() 
			{
				return new OnlineResourceEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class OnlineResourceEnumerator: IEnumerator 
        {
			int nIndex;
			MetadataURLType parent;
			public OnlineResourceEnumerator(MetadataURLType par) 
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
				return(nIndex < parent.OnlineResourceCount );
			}
			public OnlineResourceType  Current 
			{
				get 
				{
					return(parent.GetOnlineResourceAt(nIndex));
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

        #endregion // OnlineResource collection

        private void SetCollectionParents()
        {
            Mytype2s.Parent = this; 
            MyFormats.Parent = this; 
            MyOnlineResources.Parent = this; 
	}
}
}
