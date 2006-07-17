//
// localfilesystemType.cs
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
	public class localfilesystemType : Altova.Xml.Node
	{
		#region Forward constructors

		public localfilesystemType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public localfilesystemType(XmlNode node) : base(node) { SetCollectionParents(); }
		public localfilesystemType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public localfilesystemType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Attribute, "", "path" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Attribute, "", "path", DOMNode )
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



		#region path accessor methods
		public static int GetpathMinCount()
		{
			return 1;
		}

		public static int pathMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetpathMaxCount()
		{
			return 1;
		}

		public static int pathMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetpathCount()
		{
			return DomChildCount(NodeType.Attribute, "", "path");
		}

		public int pathCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "path");
			}
		}

		public bool Haspath()
		{
			return HasDomChild(NodeType.Attribute, "", "path");
		}

		public SchemaString Newpath()
		{
			return new SchemaString();
		}

		public SchemaString GetpathAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "path", index)));
		}

		public XmlNode GetStartingpathCursor()
		{
			return GetDomFirstChild( NodeType.Attribute, "", "path" );
		}

		public XmlNode GetAdvancedpathCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Attribute, "", "path", curNode );
		}

		public SchemaString GetpathValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new SchemaString( curNode.Value );
		}


		public SchemaString Getpath()
		{
			return GetpathAt(0);
		}

		public SchemaString path
		{
			get
			{
				return GetpathAt(0);
			}
		}

		public void RemovepathAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "path", index);
		}

		public void Removepath()
		{
			while (Haspath())
				RemovepathAt(0);
		}

		public void Addpath(SchemaString newValue)
		{
			if( newValue.IsNull() == false )
				AppendDomChild(NodeType.Attribute, "", "path", newValue.ToString());
		}

		public void InsertpathAt(SchemaString newValue, int index)
		{
			if( newValue.IsNull() == false )
				InsertDomChildAt(NodeType.Attribute, "", "path", index, newValue.ToString());
		}

		public void ReplacepathAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "path", index, newValue.ToString());
		}
		#endregion // path accessor methods

		#region path collection
        public pathCollection	Mypaths = new pathCollection( );

        public class pathCollection: IEnumerable
        {
            localfilesystemType parent;
            public localfilesystemType Parent
			{
				set
				{
					parent = value;
				}
			}
			public pathEnumerator GetEnumerator() 
			{
				return new pathEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class pathEnumerator: IEnumerator 
        {
			int nIndex;
			localfilesystemType parent;
			public pathEnumerator(localfilesystemType par) 
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
				return(nIndex < parent.pathCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetpathAt(nIndex));
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

        #endregion // path collection

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
            localfilesystemType parent;
            public localfilesystemType Parent
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
			localfilesystemType parent;
			public activelayersEnumerator(localfilesystemType par) 
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
            Mypaths.Parent = this; 
            Myactivelayerss.Parent = this; 
	}
}
}
