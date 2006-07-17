//
// StyleType.cs
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
	public class StyleType : Altova.Xml.Node
	{
		#region Forward constructors

		public StyleType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public StyleType(XmlNode node) : base(node) { SetCollectionParents(); }
		public StyleType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public StyleType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Name" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Name", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Title" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Title", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Abstract" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Abstract", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "LegendURL" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "LegendURL", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
				new LegendURLType(DOMNode).AdjustPrefix();
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
				new StyleSheetURLType(DOMNode).AdjustPrefix();
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "StyleURL" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "StyleURL", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
				new StyleURLType(DOMNode).AdjustPrefix();
			}
		}



		#region Name accessor methods
		public static int GetNameMinCount()
		{
			return 1;
		}

		public static int NameMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetNameMaxCount()
		{
			return 1;
		}

		public static int NameMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetNameCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Name");
		}

		public int NameCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Name");
			}
		}

		public bool HasName()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Name");
		}

		public SchemaString NewName()
		{
			return new SchemaString();
		}

		public SchemaString GetNameAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Name", index)));
		}

		public XmlNode GetStartingNameCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Name" );
		}

		public XmlNode GetAdvancedNameCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Name", curNode );
		}

		public SchemaString GetNameValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetName()
		{
			return GetNameAt(0);
		}

		public SchemaString Name
		{
			get
			{
				return GetNameAt(0);
			}
		}

		public void RemoveNameAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Name", index);
		}

		public void RemoveName()
		{
			while (HasName())
				RemoveNameAt(0);
		}

		public void AddName(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "Name", newValue.ToString());
		}

		public void InsertNameAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Name", index, newValue.ToString());
		}

		public void ReplaceNameAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Name", index, newValue.ToString());
		}
		#endregion // Name accessor methods

		#region Name collection
        public NameCollection	MyNames = new NameCollection( );

        public class NameCollection: IEnumerable
        {
            StyleType parent;
            public StyleType Parent
			{
				set
				{
					parent = value;
				}
			}
			public NameEnumerator GetEnumerator() 
			{
				return new NameEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class NameEnumerator: IEnumerator 
        {
			int nIndex;
			StyleType parent;
			public NameEnumerator(StyleType par) 
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
				return(nIndex < parent.NameCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetNameAt(nIndex));
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

        #endregion // Name collection

		#region Title accessor methods
		public static int GetTitleMinCount()
		{
			return 1;
		}

		public static int TitleMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetTitleMaxCount()
		{
			return 1;
		}

		public static int TitleMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetTitleCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Title");
		}

		public int TitleCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Title");
			}
		}

		public bool HasTitle()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Title");
		}

		public SchemaString NewTitle()
		{
			return new SchemaString();
		}

		public SchemaString GetTitleAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Title", index)));
		}

		public XmlNode GetStartingTitleCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Title" );
		}

		public XmlNode GetAdvancedTitleCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Title", curNode );
		}

		public SchemaString GetTitleValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetTitle()
		{
			return GetTitleAt(0);
		}

		public SchemaString Title
		{
			get
			{
				return GetTitleAt(0);
			}
		}

		public void RemoveTitleAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Title", index);
		}

		public void RemoveTitle()
		{
			while (HasTitle())
				RemoveTitleAt(0);
		}

		public void AddTitle(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "Title", newValue.ToString());
		}

		public void InsertTitleAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Title", index, newValue.ToString());
		}

		public void ReplaceTitleAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Title", index, newValue.ToString());
		}
		#endregion // Title accessor methods

		#region Title collection
        public TitleCollection	MyTitles = new TitleCollection( );

        public class TitleCollection: IEnumerable
        {
            StyleType parent;
            public StyleType Parent
			{
				set
				{
					parent = value;
				}
			}
			public TitleEnumerator GetEnumerator() 
			{
				return new TitleEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class TitleEnumerator: IEnumerator 
        {
			int nIndex;
			StyleType parent;
			public TitleEnumerator(StyleType par) 
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
				return(nIndex < parent.TitleCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetTitleAt(nIndex));
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

        #endregion // Title collection

		#region Abstract2 accessor methods
		public static int GetAbstract2MinCount()
		{
			return 0;
		}

		public static int Abstract2MinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetAbstract2MaxCount()
		{
			return 1;
		}

		public static int Abstract2MaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetAbstract2Count()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Abstract");
		}

		public int Abstract2Count
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Abstract");
			}
		}

		public bool HasAbstract2()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Abstract");
		}

		public SchemaString NewAbstract2()
		{
			return new SchemaString();
		}

		public SchemaString GetAbstract2At(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Abstract", index)));
		}

		public XmlNode GetStartingAbstract2Cursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Abstract" );
		}

		public XmlNode GetAdvancedAbstract2Cursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Abstract", curNode );
		}

		public SchemaString GetAbstract2ValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.InnerText );
		}


		public SchemaString GetAbstract2()
		{
			return GetAbstract2At(0);
		}

		public SchemaString Abstract2
		{
			get
			{
				return GetAbstract2At(0);
			}
		}

		public void RemoveAbstract2At(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Abstract", index);
		}

		public void RemoveAbstract2()
		{
			while (HasAbstract2())
				RemoveAbstract2At(0);
		}

		public void AddAbstract2(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Element, "http://www.opengis.net/wms", "Abstract", newValue.ToString());
		}

		public void InsertAbstract2At(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Abstract", index, newValue.ToString());
		}

		public void ReplaceAbstract2At(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Abstract", index, newValue.ToString());
		}
		#endregion // Abstract2 accessor methods

		#region Abstract2 collection
        public Abstract2Collection	MyAbstract2s = new Abstract2Collection( );

        public class Abstract2Collection: IEnumerable
        {
            StyleType parent;
            public StyleType Parent
			{
				set
				{
					parent = value;
				}
			}
			public Abstract2Enumerator GetEnumerator() 
			{
				return new Abstract2Enumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class Abstract2Enumerator: IEnumerator 
        {
			int nIndex;
			StyleType parent;
			public Abstract2Enumerator(StyleType par) 
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
				return(nIndex < parent.Abstract2Count );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetAbstract2At(nIndex));
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

        #endregion // Abstract2 collection

		#region LegendURL accessor methods
		public static int GetLegendURLMinCount()
		{
			return 0;
		}

		public static int LegendURLMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetLegendURLMaxCount()
		{
			return Int32.MaxValue;
		}

		public static int LegendURLMaxCount
		{
			get
			{
				return Int32.MaxValue;
			}
		}

		public int GetLegendURLCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "LegendURL");
		}

		public int LegendURLCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "LegendURL");
			}
		}

		public bool HasLegendURL()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "LegendURL");
		}

		public LegendURLType NewLegendURL()
		{
			return new LegendURLType(domNode.OwnerDocument.CreateElement("LegendURL", "http://www.opengis.net/wms"));
		}

		public LegendURLType GetLegendURLAt(int index)
		{
			return new LegendURLType(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "LegendURL", index));
		}

		public XmlNode GetStartingLegendURLCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "LegendURL" );
		}

		public XmlNode GetAdvancedLegendURLCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "LegendURL", curNode );
		}

		public LegendURLType GetLegendURLValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new LegendURLType( curNode );
		}


		public LegendURLType GetLegendURL()
		{
			return GetLegendURLAt(0);
		}

		public LegendURLType LegendURL
		{
			get
			{
				return GetLegendURLAt(0);
			}
		}

		public void RemoveLegendURLAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "LegendURL", index);
		}

		public void RemoveLegendURL()
		{
			while (HasLegendURL())
				RemoveLegendURLAt(0);
		}

		public void AddLegendURL(LegendURLType newValue)
		{
			AppendDomElement("http://www.opengis.net/wms", "LegendURL", newValue);
		}

		public void InsertLegendURLAt(LegendURLType newValue, int index)
		{
			InsertDomElementAt("http://www.opengis.net/wms", "LegendURL", index, newValue);
		}

		public void ReplaceLegendURLAt(LegendURLType newValue, int index)
		{
			ReplaceDomElementAt("http://www.opengis.net/wms", "LegendURL", index, newValue);
		}
		#endregion // LegendURL accessor methods

		#region LegendURL collection
        public LegendURLCollection	MyLegendURLs = new LegendURLCollection( );

        public class LegendURLCollection: IEnumerable
        {
            StyleType parent;
            public StyleType Parent
			{
				set
				{
					parent = value;
				}
			}
			public LegendURLEnumerator GetEnumerator() 
			{
				return new LegendURLEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class LegendURLEnumerator: IEnumerator 
        {
			int nIndex;
			StyleType parent;
			public LegendURLEnumerator(StyleType par) 
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
				return(nIndex < parent.LegendURLCount );
			}
			public LegendURLType  Current 
			{
				get 
				{
					return(parent.GetLegendURLAt(nIndex));
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

        #endregion // LegendURL collection

		#region StyleSheetURL accessor methods
		public static int GetStyleSheetURLMinCount()
		{
			return 0;
		}

		public static int StyleSheetURLMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetStyleSheetURLMaxCount()
		{
			return 1;
		}

		public static int StyleSheetURLMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetStyleSheetURLCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL");
		}

		public int StyleSheetURLCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL");
			}
		}

		public bool HasStyleSheetURL()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL");
		}

		public StyleSheetURLType NewStyleSheetURL()
		{
			return new StyleSheetURLType(domNode.OwnerDocument.CreateElement("StyleSheetURL", "http://www.opengis.net/wms"));
		}

		public StyleSheetURLType GetStyleSheetURLAt(int index)
		{
			return new StyleSheetURLType(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL", index));
		}

		public XmlNode GetStartingStyleSheetURLCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL" );
		}

		public XmlNode GetAdvancedStyleSheetURLCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL", curNode );
		}

		public StyleSheetURLType GetStyleSheetURLValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new StyleSheetURLType( curNode );
		}


		public StyleSheetURLType GetStyleSheetURL()
		{
			return GetStyleSheetURLAt(0);
		}

		public StyleSheetURLType StyleSheetURL
		{
			get
			{
				return GetStyleSheetURLAt(0);
			}
		}

		public void RemoveStyleSheetURLAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StyleSheetURL", index);
		}

		public void RemoveStyleSheetURL()
		{
			while (HasStyleSheetURL())
				RemoveStyleSheetURLAt(0);
		}

		public void AddStyleSheetURL(StyleSheetURLType newValue)
		{
			AppendDomElement("http://www.opengis.net/wms", "StyleSheetURL", newValue);
		}

		public void InsertStyleSheetURLAt(StyleSheetURLType newValue, int index)
		{
			InsertDomElementAt("http://www.opengis.net/wms", "StyleSheetURL", index, newValue);
		}

		public void ReplaceStyleSheetURLAt(StyleSheetURLType newValue, int index)
		{
			ReplaceDomElementAt("http://www.opengis.net/wms", "StyleSheetURL", index, newValue);
		}
		#endregion // StyleSheetURL accessor methods

		#region StyleSheetURL collection
        public StyleSheetURLCollection	MyStyleSheetURLs = new StyleSheetURLCollection( );

        public class StyleSheetURLCollection: IEnumerable
        {
            StyleType parent;
            public StyleType Parent
			{
				set
				{
					parent = value;
				}
			}
			public StyleSheetURLEnumerator GetEnumerator() 
			{
				return new StyleSheetURLEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class StyleSheetURLEnumerator: IEnumerator 
        {
			int nIndex;
			StyleType parent;
			public StyleSheetURLEnumerator(StyleType par) 
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
				return(nIndex < parent.StyleSheetURLCount );
			}
			public StyleSheetURLType  Current 
			{
				get 
				{
					return(parent.GetStyleSheetURLAt(nIndex));
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

        #endregion // StyleSheetURL collection

		#region StyleURL accessor methods
		public static int GetStyleURLMinCount()
		{
			return 0;
		}

		public static int StyleURLMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetStyleURLMaxCount()
		{
			return 1;
		}

		public static int StyleURLMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetStyleURLCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "StyleURL");
		}

		public int StyleURLCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "StyleURL");
			}
		}

		public bool HasStyleURL()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "StyleURL");
		}

		public StyleURLType NewStyleURL()
		{
			return new StyleURLType(domNode.OwnerDocument.CreateElement("StyleURL", "http://www.opengis.net/wms"));
		}

		public StyleURLType GetStyleURLAt(int index)
		{
			return new StyleURLType(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StyleURL", index));
		}

		public XmlNode GetStartingStyleURLCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "StyleURL" );
		}

		public XmlNode GetAdvancedStyleURLCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "StyleURL", curNode );
		}

		public StyleURLType GetStyleURLValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new StyleURLType( curNode );
		}


		public StyleURLType GetStyleURL()
		{
			return GetStyleURLAt(0);
		}

		public StyleURLType StyleURL
		{
			get
			{
				return GetStyleURLAt(0);
			}
		}

		public void RemoveStyleURLAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "StyleURL", index);
		}

		public void RemoveStyleURL()
		{
			while (HasStyleURL())
				RemoveStyleURLAt(0);
		}

		public void AddStyleURL(StyleURLType newValue)
		{
			AppendDomElement("http://www.opengis.net/wms", "StyleURL", newValue);
		}

		public void InsertStyleURLAt(StyleURLType newValue, int index)
		{
			InsertDomElementAt("http://www.opengis.net/wms", "StyleURL", index, newValue);
		}

		public void ReplaceStyleURLAt(StyleURLType newValue, int index)
		{
			ReplaceDomElementAt("http://www.opengis.net/wms", "StyleURL", index, newValue);
		}
		#endregion // StyleURL accessor methods

		#region StyleURL collection
        public StyleURLCollection	MyStyleURLs = new StyleURLCollection( );

        public class StyleURLCollection: IEnumerable
        {
            StyleType parent;
            public StyleType Parent
			{
				set
				{
					parent = value;
				}
			}
			public StyleURLEnumerator GetEnumerator() 
			{
				return new StyleURLEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class StyleURLEnumerator: IEnumerator 
        {
			int nIndex;
			StyleType parent;
			public StyleURLEnumerator(StyleType par) 
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
				return(nIndex < parent.StyleURLCount );
			}
			public StyleURLType  Current 
			{
				get 
				{
					return(parent.GetStyleURLAt(nIndex));
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

        #endregion // StyleURL collection

        private void SetCollectionParents()
        {
            MyNames.Parent = this; 
            MyTitles.Parent = this; 
            MyAbstract2s.Parent = this; 
            MyLegendURLs.Parent = this; 
            MyStyleSheetURLs.Parent = this; 
            MyStyleURLs.Parent = this; 
	}
}
}