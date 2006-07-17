//
// DimensionType.cs
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
	public class DimensionType : Altova.Xml.Node
	{
		#region Forward constructors

		public DimensionType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public DimensionType(XmlNode node) : base(node) { SetCollectionParents(); }
		public DimensionType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public DimensionType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "name" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "name", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "units" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "units", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "unitSymbol" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "unitSymbol", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}
		}



		#region name accessor methods
		public static int GetnameMinCount()
		{
			return 1;
		}

		public static int nameMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetnameMaxCount()
		{
			return 1;
		}

		public static int nameMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetnameCount()
		{
			return DomChildCount(NodeType.Attribute, "", "name");
		}

		public int nameCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "name");
			}
		}

		public bool Hasname()
		{
			return HasDomChild(NodeType.Attribute, "", "name");
		}

		public SchemaString Newname()
		{
			return new SchemaString();
		}

		public SchemaString GetnameAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "name", index)));
		}

		public XmlNode GetStartingnameCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "name" );
		}

		public XmlNode GetAdvancednameCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "name", curNode );
		}

		public SchemaString GetnameValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getname()
		{
			return GetnameAt(0);
		}

		public SchemaString name
		{
			get
			{
				return GetnameAt(0);
			}
		}

		public void RemovenameAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "name", index);
		}

		public void Removename()
		{
			while (Hasname())
				RemovenameAt(0);
		}

		public void Addname(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Attribute, "", "name", newValue.ToString());
		}

		public void InsertnameAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "name", index, newValue.ToString());
		}

		public void ReplacenameAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "name", index, newValue.ToString());
		}
		#endregion // name accessor methods

		#region name collection
        public nameCollection	Mynames = new nameCollection( );

        public class nameCollection: IEnumerable
        {
            DimensionType parent;
            public DimensionType Parent
			{
				set
				{
					parent = value;
				}
			}
			public nameEnumerator GetEnumerator() 
			{
				return new nameEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class nameEnumerator: IEnumerator 
        {
			int nIndex;
			DimensionType parent;
			public nameEnumerator(DimensionType par) 
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
				return(nIndex < parent.nameCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetnameAt(nIndex));
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

        #endregion // name collection

		#region units accessor methods
		public static int GetunitsMinCount()
		{
			return 1;
		}

		public static int unitsMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetunitsMaxCount()
		{
			return 1;
		}

		public static int unitsMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetunitsCount()
		{
			return DomChildCount(NodeType.Attribute, "", "units");
		}

		public int unitsCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "units");
			}
		}

		public bool Hasunits()
		{
			return HasDomChild(NodeType.Attribute, "", "units");
		}

		public SchemaString Newunits()
		{
			return new SchemaString();
		}

		public SchemaString GetunitsAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "units", index)));
		}

		public XmlNode GetStartingunitsCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "units" );
		}

		public XmlNode GetAdvancedunitsCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "units", curNode );
		}

		public SchemaString GetunitsValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getunits()
		{
			return GetunitsAt(0);
		}

		public SchemaString units
		{
			get
			{
				return GetunitsAt(0);
			}
		}

		public void RemoveunitsAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "units", index);
		}

		public void Removeunits()
		{
			while (Hasunits())
				RemoveunitsAt(0);
		}

		public void Addunits(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Attribute, "", "units", newValue.ToString());
		}

		public void InsertunitsAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "units", index, newValue.ToString());
		}

		public void ReplaceunitsAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "units", index, newValue.ToString());
		}
		#endregion // units accessor methods

		#region units collection
        public unitsCollection	Myunitss = new unitsCollection( );

        public class unitsCollection: IEnumerable
        {
            DimensionType parent;
            public DimensionType Parent
			{
				set
				{
					parent = value;
				}
			}
			public unitsEnumerator GetEnumerator() 
			{
				return new unitsEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class unitsEnumerator: IEnumerator 
        {
			int nIndex;
			DimensionType parent;
			public unitsEnumerator(DimensionType par) 
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
				return(nIndex < parent.unitsCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetunitsAt(nIndex));
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

        #endregion // units collection

		#region unitSymbol accessor methods
		public static int GetunitSymbolMinCount()
		{
			return 0;
		}

		public static int unitSymbolMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetunitSymbolMaxCount()
		{
			return 1;
		}

		public static int unitSymbolMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetunitSymbolCount()
		{
			return DomChildCount(NodeType.Attribute, "", "unitSymbol");
		}

		public int unitSymbolCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "unitSymbol");
			}
		}

		public bool HasunitSymbol()
		{
			return HasDomChild(NodeType.Attribute, "", "unitSymbol");
		}

		public SchemaString NewunitSymbol()
		{
			return new SchemaString();
		}

		public SchemaString GetunitSymbolAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "unitSymbol", index)));
		}

		public XmlNode GetStartingunitSymbolCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "unitSymbol" );
		}

		public XmlNode GetAdvancedunitSymbolCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "unitSymbol", curNode );
		}

		public SchemaString GetunitSymbolValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString GetunitSymbol()
		{
			return GetunitSymbolAt(0);
		}

		public SchemaString unitSymbol
		{
			get
			{
				return GetunitSymbolAt(0);
			}
		}

		public void RemoveunitSymbolAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "unitSymbol", index);
		}

		public void RemoveunitSymbol()
		{
			while (HasunitSymbol())
				RemoveunitSymbolAt(0);
		}

		public void AddunitSymbol(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Attribute, "", "unitSymbol", newValue.ToString());
		}

		public void InsertunitSymbolAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "unitSymbol", index, newValue.ToString());
		}

		public void ReplaceunitSymbolAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "unitSymbol", index, newValue.ToString());
		}
		#endregion // unitSymbol accessor methods

		#region unitSymbol collection
        public unitSymbolCollection	MyunitSymbols = new unitSymbolCollection( );

        public class unitSymbolCollection: IEnumerable
        {
            DimensionType parent;
            public DimensionType Parent
			{
				set
				{
					parent = value;
				}
			}
			public unitSymbolEnumerator GetEnumerator() 
			{
				return new unitSymbolEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class unitSymbolEnumerator: IEnumerator 
        {
			int nIndex;
			DimensionType parent;
			public unitSymbolEnumerator(DimensionType par) 
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
				return(nIndex < parent.unitSymbolCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetunitSymbolAt(nIndex));
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

        #endregion // unitSymbol collection

        private void SetCollectionParents()
        {
            Mynames.Parent = this; 
            Myunitss.Parent = this; 
            MyunitSymbols.Parent = this; 
	}
}
}
