//
// PutStylesType.cs
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
	public class PutStylesType : Altova.Xml.Node
	{
		#region Forward constructors

		public PutStylesType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public PutStylesType(XmlNode node) : base(node) { SetCollectionParents(); }
		public PutStylesType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public PutStylesType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "", "Format" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "", "Format", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
				new FormatType(DOMNode).AdjustPrefix();
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "", "DCPType" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "", "DCPType", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
				new DCPTypeType(DOMNode).AdjustPrefix();
			}
		}



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
			return Int32.MaxValue;
		}

		public static int FormatMaxCount
		{
			get
			{
				return Int32.MaxValue;
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
            PutStylesType parent;
            public PutStylesType Parent
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
			PutStylesType parent;
			public FormatEnumerator(PutStylesType par) 
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

		#region DCPType accessor methods
		public static int GetDCPTypeMinCount()
		{
			return 1;
		}

		public static int DCPTypeMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetDCPTypeMaxCount()
		{
			return Int32.MaxValue;
		}

		public static int DCPTypeMaxCount
		{
			get
			{
				return Int32.MaxValue;
			}
		}

		public int GetDCPTypeCount()
		{
			return DomChildCount(NodeType.Element, "", "DCPType");
		}

		public int DCPTypeCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "", "DCPType");
			}
		}

		public bool HasDCPType()
		{
			return HasDomChild(NodeType.Element, "", "DCPType");
		}

		public DCPTypeType NewDCPType()
		{
			return new DCPTypeType(domNode.OwnerDocument.CreateElement("DCPType", ""));
		}

		public DCPTypeType GetDCPTypeAt(int index)
		{
			return new DCPTypeType(GetDomChildAt(NodeType.Element, "", "DCPType", index));
		}

		public XmlNode GetStartingDCPTypeCursor()
		{
			return GetDomFirstChild( NodeType.Element, "", "DCPType" );
		}

		public XmlNode GetAdvancedDCPTypeCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "", "DCPType", curNode );
		}

		public DCPTypeType GetDCPTypeValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new DCPTypeType( curNode );
		}


		public DCPTypeType GetDCPType()
		{
			return GetDCPTypeAt(0);
		}

		public DCPTypeType DCPType
		{
			get
			{
				return GetDCPTypeAt(0);
			}
		}

		public void RemoveDCPTypeAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "", "DCPType", index);
		}

		public void RemoveDCPType()
		{
			while (HasDCPType())
				RemoveDCPTypeAt(0);
		}

		public void AddDCPType(DCPTypeType newValue)
		{
			AppendDomElement("", "DCPType", newValue);
		}

		public void InsertDCPTypeAt(DCPTypeType newValue, int index)
		{
			InsertDomElementAt("", "DCPType", index, newValue);
		}

		public void ReplaceDCPTypeAt(DCPTypeType newValue, int index)
		{
			ReplaceDomElementAt("", "DCPType", index, newValue);
		}
		#endregion // DCPType accessor methods

		#region DCPType collection
        public DCPTypeCollection	MyDCPTypes = new DCPTypeCollection( );

        public class DCPTypeCollection: IEnumerable
        {
            PutStylesType parent;
            public PutStylesType Parent
			{
				set
				{
					parent = value;
				}
			}
			public DCPTypeEnumerator GetEnumerator() 
			{
				return new DCPTypeEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class DCPTypeEnumerator: IEnumerator 
        {
			int nIndex;
			PutStylesType parent;
			public DCPTypeEnumerator(PutStylesType par) 
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
				return(nIndex < parent.DCPTypeCount );
			}
			public DCPTypeType  Current 
			{
				get 
				{
					return(parent.GetDCPTypeAt(nIndex));
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

        #endregion // DCPType collection

        private void SetCollectionParents()
        {
            MyFormats.Parent = this; 
            MyDCPTypes.Parent = this; 
	}
}
}
