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
		#region Documentation
		public static string GetAnnoDocumentation() { return ""; }
		#endregion

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
		}



		#region path Documentation
		public static string GetpathAnnoDocumentation()
		{
			return "";		
		}
		public static string GetpathDefault()
		{
			return "";		
		}
		#endregion

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

        private void SetCollectionParents()
        {
            Mypaths.Parent = this; 
	}
}
}
