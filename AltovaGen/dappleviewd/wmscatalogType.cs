//
// wmscatalogType.cs
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

namespace dappleview
{
	public class wmscatalogType : Altova.Xml.Node
	{
		#region Forward constructors

		public wmscatalogType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public wmscatalogType(XmlNode node) : base(node) { SetCollectionParents(); }
		public wmscatalogType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public wmscatalogType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "capabilitiesurl" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "capabilitiesurl", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "", "activelayers" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "", "activelayers", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
				new activelayersType(DOMNode).AdjustPrefix();
			}
		}



		#region capabilitiesurl accessor methods
		public static int GetcapabilitiesurlMinCount()
		{
			return 1;
		}

		public static int capabilitiesurlMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetcapabilitiesurlMaxCount()
		{
			return 1;
		}

		public static int capabilitiesurlMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetcapabilitiesurlCount()
		{
			return DomChildCount(NodeType.Attribute, "", "capabilitiesurl");
		}

		public int capabilitiesurlCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "capabilitiesurl");
			}
		}

		public bool Hascapabilitiesurl()
		{
			return HasDomChild(NodeType.Attribute, "", "capabilitiesurl");
		}

		public SchemaString Newcapabilitiesurl()
		{
			return new SchemaString();
		}

		public SchemaString GetcapabilitiesurlAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "capabilitiesurl", index)));
		}

		public XmlNode GetStartingcapabilitiesurlCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "capabilitiesurl" );
		}

		public XmlNode GetAdvancedcapabilitiesurlCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "capabilitiesurl", curNode );
		}

		public SchemaString GetcapabilitiesurlValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getcapabilitiesurl()
		{
			return GetcapabilitiesurlAt(0);
		}

		public SchemaString capabilitiesurl
		{
			get
			{
				return GetcapabilitiesurlAt(0);
			}
		}

		public void RemovecapabilitiesurlAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "capabilitiesurl", index);
		}

		public void Removecapabilitiesurl()
		{
			while (Hascapabilitiesurl())
				RemovecapabilitiesurlAt(0);
		}

		public void Addcapabilitiesurl(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Attribute, "", "capabilitiesurl", newValue.ToString());
		}

		public void InsertcapabilitiesurlAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "capabilitiesurl", index, newValue.ToString());
		}

		public void ReplacecapabilitiesurlAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "capabilitiesurl", index, newValue.ToString());
		}
		#endregion // capabilitiesurl accessor methods

		#region capabilitiesurl collection
        public capabilitiesurlCollection	Mycapabilitiesurls = new capabilitiesurlCollection( );

        public class capabilitiesurlCollection: IEnumerable
        {
            wmscatalogType parent;
            public wmscatalogType Parent
			{
				set
				{
					parent = value;
				}
			}
			public capabilitiesurlEnumerator GetEnumerator() 
			{
				return new capabilitiesurlEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class capabilitiesurlEnumerator: IEnumerator 
        {
			int nIndex;
			wmscatalogType parent;
			public capabilitiesurlEnumerator(wmscatalogType par) 
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
				return(nIndex < parent.capabilitiesurlCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetcapabilitiesurlAt(nIndex));
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

        #endregion // capabilitiesurl collection

		#region activelayers accessor methods
		public static int GetactivelayersMinCount()
		{
			return 0;
		}

		public static int activelayersMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetactivelayersMaxCount()
		{
			return 1;
		}

		public static int activelayersMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetactivelayersCount()
		{
			return DomChildCount(NodeType.Element, "", "activelayers");
		}

		public int activelayersCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "", "activelayers");
			}
		}

		public bool Hasactivelayers()
		{
			return HasDomChild(NodeType.Element, "", "activelayers");
		}

		public activelayersType Newactivelayers()
		{
			return new activelayersType(domNode.OwnerDocument.CreateElement("activelayers", ""));
		}

		public activelayersType GetactivelayersAt(int index)
		{
			return new activelayersType(GetDomChildAt(NodeType.Element, "", "activelayers", index));
		}

		public XmlNode GetStartingactivelayersCursor()
		{
			return GetDomFirstChild( NodeType.Element, "", "activelayers" );
		}

		public XmlNode GetAdvancedactivelayersCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "", "activelayers", curNode );
		}

		public activelayersType GetactivelayersValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new activelayersType( curNode );
		}


		public activelayersType Getactivelayers()
		{
			return GetactivelayersAt(0);
		}

		public activelayersType activelayers
		{
			get
			{
				return GetactivelayersAt(0);
			}
		}

		public void RemoveactivelayersAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "", "activelayers", index);
		}

		public void Removeactivelayers()
		{
			while (Hasactivelayers())
				RemoveactivelayersAt(0);
		}

		public void Addactivelayers(activelayersType newValue)
		{
			AppendDomElement("", "activelayers", newValue);
		}

		public void InsertactivelayersAt(activelayersType newValue, int index)
		{
			InsertDomElementAt("", "activelayers", index, newValue);
		}

		public void ReplaceactivelayersAt(activelayersType newValue, int index)
		{
			ReplaceDomElementAt("", "activelayers", index, newValue);
		}
		#endregion // activelayers accessor methods

		#region activelayers collection
        public activelayersCollection	Myactivelayerss = new activelayersCollection( );

        public class activelayersCollection: IEnumerable
        {
            wmscatalogType parent;
            public wmscatalogType Parent
			{
				set
				{
					parent = value;
				}
			}
			public activelayersEnumerator GetEnumerator() 
			{
				return new activelayersEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class activelayersEnumerator: IEnumerator 
        {
			int nIndex;
			wmscatalogType parent;
			public activelayersEnumerator(wmscatalogType par) 
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
				return(nIndex < parent.activelayersCount );
			}
			public activelayersType  Current 
			{
				get 
				{
					return(parent.GetactivelayersAt(nIndex));
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

        #endregion // activelayers collection

        private void SetCollectionParents()
        {
            Mycapabilitiesurls.Parent = this; 
            Myactivelayerss.Parent = this; 
	}
}
}