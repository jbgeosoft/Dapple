//
// BoundingBoxType.cs
//
// This file was generated by XMLSpy 2007r3 Enterprise Edition.
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

namespace WMS_MS_Capabilities
{
	public class BoundingBoxType : Altova.Xml.Node
	{
		#region Documentation
		public static string GetAnnoDocumentation() { return ""; }
		#endregion

		#region Forward constructors

		public BoundingBoxType() : base() { SetCollectionParents(); }

		public BoundingBoxType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public BoundingBoxType(XmlNode node) : base(node) { SetCollectionParents(); }
		public BoundingBoxType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public BoundingBoxType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "SRS" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "SRS", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "minx" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "minx", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "miny" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "miny", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "maxx" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "maxx", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "maxy" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "maxy", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "resx" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "resx", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "resy" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "resy", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}
		}

		public void SetXsiType()
		{
 			XmlElement el = (XmlElement) domNode;
			el.SetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", "BoundingBox");
		}


		#region SRS Documentation
		public static string GetSRSAnnoDocumentation()
		{
			return "";		
		}
		public static string GetSRSDefault()
		{
			return "";		
		}
		#endregion

		#region SRS accessor methods
		public static int GetSRSMinCount()
		{
			return 1;
		}

		public static int SRSMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetSRSMaxCount()
		{
			return 1;
		}

		public static int SRSMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetSRSCount()
		{
			return DomChildCount(NodeType.Attribute, "", "SRS");
		}

		public int SRSCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "SRS");
			}
		}

		public bool HasSRS()
		{
			return HasDomChild(NodeType.Attribute, "", "SRS");
		}

		public SchemaString NewSRS()
		{
			return new SchemaString();
		}

		public SchemaString GetSRSAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "SRS", index)));
		}

		public XmlNode GetStartingSRSCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "SRS" );
		}

		public XmlNode GetAdvancedSRSCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "SRS", curNode );
		}

		public SchemaString GetSRSValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString GetSRS()
		{
			return GetSRSAt(0);
		}

		public SchemaString SRS
		{
			get
			{
				return GetSRSAt(0);
			}
		}

		public void RemoveSRSAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "SRS", index);
		}

		public void RemoveSRS()
		{
			RemoveSRSAt(0);
		}

		public XmlNode AddSRS(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				return AppendDomChild(NodeType.Attribute, "", "SRS", newValue.ToString());
			return null;
		}

		public void InsertSRSAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "SRS", index, newValue.ToString());
		}

		public void ReplaceSRSAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "SRS", index, newValue.ToString());
		}
		#endregion // SRS accessor methods

		#region SRS collection
        public SRSCollection	MySRSs = new SRSCollection( );

        public class SRSCollection: IEnumerable
        {
            BoundingBoxType parent;
            public BoundingBoxType Parent
			{
				set
				{
					parent = value;
				}
			}
			public SRSEnumerator GetEnumerator() 
			{
				return new SRSEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class SRSEnumerator: IEnumerator 
        {
			int nIndex;
			BoundingBoxType parent;
			public SRSEnumerator(BoundingBoxType par) 
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
				return(nIndex < parent.SRSCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetSRSAt(nIndex));
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

        #endregion // SRS collection

		#region minx Documentation
		public static string GetminxAnnoDocumentation()
		{
			return "";		
		}
		public static string GetminxDefault()
		{
			return "";		
		}
		#endregion

		#region minx accessor methods
		public static int GetminxMinCount()
		{
			return 1;
		}

		public static int minxMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetminxMaxCount()
		{
			return 1;
		}

		public static int minxMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetminxCount()
		{
			return DomChildCount(NodeType.Attribute, "", "minx");
		}

		public int minxCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "minx");
			}
		}

		public bool Hasminx()
		{
			return HasDomChild(NodeType.Attribute, "", "minx");
		}

		public SchemaString Newminx()
		{
			return new SchemaString();
		}

		public SchemaString GetminxAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "minx", index)));
		}

		public XmlNode GetStartingminxCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "minx" );
		}

		public XmlNode GetAdvancedminxCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "minx", curNode );
		}

		public SchemaString GetminxValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getminx()
		{
			return GetminxAt(0);
		}

		public SchemaString minx
		{
			get
			{
				return GetminxAt(0);
			}
		}

		public void RemoveminxAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "minx", index);
		}

		public void Removeminx()
		{
			RemoveminxAt(0);
		}

		public XmlNode Addminx(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				return AppendDomChild(NodeType.Attribute, "", "minx", newValue.ToString());
			return null;
		}

		public void InsertminxAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "minx", index, newValue.ToString());
		}

		public void ReplaceminxAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "minx", index, newValue.ToString());
		}
		#endregion // minx accessor methods

		#region minx collection
        public minxCollection	Myminxs = new minxCollection( );

        public class minxCollection: IEnumerable
        {
            BoundingBoxType parent;
            public BoundingBoxType Parent
			{
				set
				{
					parent = value;
				}
			}
			public minxEnumerator GetEnumerator() 
			{
				return new minxEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class minxEnumerator: IEnumerator 
        {
			int nIndex;
			BoundingBoxType parent;
			public minxEnumerator(BoundingBoxType par) 
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
				return(nIndex < parent.minxCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetminxAt(nIndex));
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

        #endregion // minx collection

		#region miny Documentation
		public static string GetminyAnnoDocumentation()
		{
			return "";		
		}
		public static string GetminyDefault()
		{
			return "";		
		}
		#endregion

		#region miny accessor methods
		public static int GetminyMinCount()
		{
			return 1;
		}

		public static int minyMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetminyMaxCount()
		{
			return 1;
		}

		public static int minyMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetminyCount()
		{
			return DomChildCount(NodeType.Attribute, "", "miny");
		}

		public int minyCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "miny");
			}
		}

		public bool Hasminy()
		{
			return HasDomChild(NodeType.Attribute, "", "miny");
		}

		public SchemaString Newminy()
		{
			return new SchemaString();
		}

		public SchemaString GetminyAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "miny", index)));
		}

		public XmlNode GetStartingminyCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "miny" );
		}

		public XmlNode GetAdvancedminyCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "miny", curNode );
		}

		public SchemaString GetminyValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getminy()
		{
			return GetminyAt(0);
		}

		public SchemaString miny
		{
			get
			{
				return GetminyAt(0);
			}
		}

		public void RemoveminyAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "miny", index);
		}

		public void Removeminy()
		{
			RemoveminyAt(0);
		}

		public XmlNode Addminy(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				return AppendDomChild(NodeType.Attribute, "", "miny", newValue.ToString());
			return null;
		}

		public void InsertminyAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "miny", index, newValue.ToString());
		}

		public void ReplaceminyAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "miny", index, newValue.ToString());
		}
		#endregion // miny accessor methods

		#region miny collection
        public minyCollection	Myminys = new minyCollection( );

        public class minyCollection: IEnumerable
        {
            BoundingBoxType parent;
            public BoundingBoxType Parent
			{
				set
				{
					parent = value;
				}
			}
			public minyEnumerator GetEnumerator() 
			{
				return new minyEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class minyEnumerator: IEnumerator 
        {
			int nIndex;
			BoundingBoxType parent;
			public minyEnumerator(BoundingBoxType par) 
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
				return(nIndex < parent.minyCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetminyAt(nIndex));
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

        #endregion // miny collection

		#region maxx Documentation
		public static string GetmaxxAnnoDocumentation()
		{
			return "";		
		}
		public static string GetmaxxDefault()
		{
			return "";		
		}
		#endregion

		#region maxx accessor methods
		public static int GetmaxxMinCount()
		{
			return 1;
		}

		public static int maxxMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetmaxxMaxCount()
		{
			return 1;
		}

		public static int maxxMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetmaxxCount()
		{
			return DomChildCount(NodeType.Attribute, "", "maxx");
		}

		public int maxxCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "maxx");
			}
		}

		public bool Hasmaxx()
		{
			return HasDomChild(NodeType.Attribute, "", "maxx");
		}

		public SchemaString Newmaxx()
		{
			return new SchemaString();
		}

		public SchemaString GetmaxxAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "maxx", index)));
		}

		public XmlNode GetStartingmaxxCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "maxx" );
		}

		public XmlNode GetAdvancedmaxxCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "maxx", curNode );
		}

		public SchemaString GetmaxxValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getmaxx()
		{
			return GetmaxxAt(0);
		}

		public SchemaString maxx
		{
			get
			{
				return GetmaxxAt(0);
			}
		}

		public void RemovemaxxAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "maxx", index);
		}

		public void Removemaxx()
		{
			RemovemaxxAt(0);
		}

		public XmlNode Addmaxx(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				return AppendDomChild(NodeType.Attribute, "", "maxx", newValue.ToString());
			return null;
		}

		public void InsertmaxxAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "maxx", index, newValue.ToString());
		}

		public void ReplacemaxxAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "maxx", index, newValue.ToString());
		}
		#endregion // maxx accessor methods

		#region maxx collection
        public maxxCollection	Mymaxxs = new maxxCollection( );

        public class maxxCollection: IEnumerable
        {
            BoundingBoxType parent;
            public BoundingBoxType Parent
			{
				set
				{
					parent = value;
				}
			}
			public maxxEnumerator GetEnumerator() 
			{
				return new maxxEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class maxxEnumerator: IEnumerator 
        {
			int nIndex;
			BoundingBoxType parent;
			public maxxEnumerator(BoundingBoxType par) 
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
				return(nIndex < parent.maxxCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetmaxxAt(nIndex));
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

        #endregion // maxx collection

		#region maxy Documentation
		public static string GetmaxyAnnoDocumentation()
		{
			return "";		
		}
		public static string GetmaxyDefault()
		{
			return "";		
		}
		#endregion

		#region maxy accessor methods
		public static int GetmaxyMinCount()
		{
			return 1;
		}

		public static int maxyMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetmaxyMaxCount()
		{
			return 1;
		}

		public static int maxyMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetmaxyCount()
		{
			return DomChildCount(NodeType.Attribute, "", "maxy");
		}

		public int maxyCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "maxy");
			}
		}

		public bool Hasmaxy()
		{
			return HasDomChild(NodeType.Attribute, "", "maxy");
		}

		public SchemaString Newmaxy()
		{
			return new SchemaString();
		}

		public SchemaString GetmaxyAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "maxy", index)));
		}

		public XmlNode GetStartingmaxyCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "maxy" );
		}

		public XmlNode GetAdvancedmaxyCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "maxy", curNode );
		}

		public SchemaString GetmaxyValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getmaxy()
		{
			return GetmaxyAt(0);
		}

		public SchemaString maxy
		{
			get
			{
				return GetmaxyAt(0);
			}
		}

		public void RemovemaxyAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "maxy", index);
		}

		public void Removemaxy()
		{
			RemovemaxyAt(0);
		}

		public XmlNode Addmaxy(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				return AppendDomChild(NodeType.Attribute, "", "maxy", newValue.ToString());
			return null;
		}

		public void InsertmaxyAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "maxy", index, newValue.ToString());
		}

		public void ReplacemaxyAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "maxy", index, newValue.ToString());
		}
		#endregion // maxy accessor methods

		#region maxy collection
        public maxyCollection	Mymaxys = new maxyCollection( );

        public class maxyCollection: IEnumerable
        {
            BoundingBoxType parent;
            public BoundingBoxType Parent
			{
				set
				{
					parent = value;
				}
			}
			public maxyEnumerator GetEnumerator() 
			{
				return new maxyEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class maxyEnumerator: IEnumerator 
        {
			int nIndex;
			BoundingBoxType parent;
			public maxyEnumerator(BoundingBoxType par) 
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
				return(nIndex < parent.maxyCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetmaxyAt(nIndex));
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

        #endregion // maxy collection

		#region resx Documentation
		public static string GetresxAnnoDocumentation()
		{
			return "";		
		}
		public static string GetresxDefault()
		{
			return "";		
		}
		#endregion

		#region resx accessor methods
		public static int GetresxMinCount()
		{
			return 0;
		}

		public static int resxMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetresxMaxCount()
		{
			return 1;
		}

		public static int resxMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetresxCount()
		{
			return DomChildCount(NodeType.Attribute, "", "resx");
		}

		public int resxCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "resx");
			}
		}

		public bool Hasresx()
		{
			return HasDomChild(NodeType.Attribute, "", "resx");
		}

		public SchemaString Newresx()
		{
			return new SchemaString();
		}

		public SchemaString GetresxAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "resx", index)));
		}

		public XmlNode GetStartingresxCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "resx" );
		}

		public XmlNode GetAdvancedresxCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "resx", curNode );
		}

		public SchemaString GetresxValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getresx()
		{
			return GetresxAt(0);
		}

		public SchemaString resx
		{
			get
			{
				return GetresxAt(0);
			}
		}

		public void RemoveresxAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "resx", index);
		}

		public void Removeresx()
		{
			RemoveresxAt(0);
		}

		public XmlNode Addresx(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				return AppendDomChild(NodeType.Attribute, "", "resx", newValue.ToString());
			return null;
		}

		public void InsertresxAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "resx", index, newValue.ToString());
		}

		public void ReplaceresxAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "resx", index, newValue.ToString());
		}
		#endregion // resx accessor methods

		#region resx collection
        public resxCollection	Myresxs = new resxCollection( );

        public class resxCollection: IEnumerable
        {
            BoundingBoxType parent;
            public BoundingBoxType Parent
			{
				set
				{
					parent = value;
				}
			}
			public resxEnumerator GetEnumerator() 
			{
				return new resxEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class resxEnumerator: IEnumerator 
        {
			int nIndex;
			BoundingBoxType parent;
			public resxEnumerator(BoundingBoxType par) 
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
				return(nIndex < parent.resxCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetresxAt(nIndex));
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

        #endregion // resx collection

		#region resy Documentation
		public static string GetresyAnnoDocumentation()
		{
			return "";		
		}
		public static string GetresyDefault()
		{
			return "";		
		}
		#endregion

		#region resy accessor methods
		public static int GetresyMinCount()
		{
			return 0;
		}

		public static int resyMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetresyMaxCount()
		{
			return 1;
		}

		public static int resyMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetresyCount()
		{
			return DomChildCount(NodeType.Attribute, "", "resy");
		}

		public int resyCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "resy");
			}
		}

		public bool Hasresy()
		{
			return HasDomChild(NodeType.Attribute, "", "resy");
		}

		public SchemaString Newresy()
		{
			return new SchemaString();
		}

		public SchemaString GetresyAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "resy", index)));
		}

		public XmlNode GetStartingresyCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "resy" );
		}

		public XmlNode GetAdvancedresyCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "resy", curNode );
		}

		public SchemaString GetresyValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getresy()
		{
			return GetresyAt(0);
		}

		public SchemaString resy
		{
			get
			{
				return GetresyAt(0);
			}
		}

		public void RemoveresyAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "resy", index);
		}

		public void Removeresy()
		{
			RemoveresyAt(0);
		}

		public XmlNode Addresy(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				return AppendDomChild(NodeType.Attribute, "", "resy", newValue.ToString());
			return null;
		}

		public void InsertresyAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "resy", index, newValue.ToString());
		}

		public void ReplaceresyAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "resy", index, newValue.ToString());
		}
		#endregion // resy accessor methods

		#region resy collection
        public resyCollection	Myresys = new resyCollection( );

        public class resyCollection: IEnumerable
        {
            BoundingBoxType parent;
            public BoundingBoxType Parent
			{
				set
				{
					parent = value;
				}
			}
			public resyEnumerator GetEnumerator() 
			{
				return new resyEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class resyEnumerator: IEnumerator 
        {
			int nIndex;
			BoundingBoxType parent;
			public resyEnumerator(BoundingBoxType par) 
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
				return(nIndex < parent.resyCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetresyAt(nIndex));
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

        #endregion // resy collection

        private void SetCollectionParents()
        {
            MySRSs.Parent = this; 
            Myminxs.Parent = this; 
            Myminys.Parent = this; 
            Mymaxxs.Parent = this; 
            Mymaxys.Parent = this; 
            Myresxs.Parent = this; 
            Myresys.Parent = this; 
	}
}
}
