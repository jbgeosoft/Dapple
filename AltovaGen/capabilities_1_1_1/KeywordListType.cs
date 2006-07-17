//
// KeywordListType.cs
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
	public class KeywordListType : Altova.Xml.Node
	{
		#region Forward constructors

		public KeywordListType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public KeywordListType(XmlNode node) : base(node) { SetCollectionParents(); }
		public KeywordListType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public KeywordListType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "", "Keyword" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "", "Keyword", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, false);
				new KeywordType(DOMNode).AdjustPrefix();
			}
		}



		#region Keyword accessor methods
		public static int GetKeywordMinCount()
		{
			return 0;
		}

		public static int KeywordMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetKeywordMaxCount()
		{
			return Int32.MaxValue;
		}

		public static int KeywordMaxCount
		{
			get
			{
				return Int32.MaxValue;
			}
		}

		public int GetKeywordCount()
		{
			return DomChildCount(NodeType.Element, "", "Keyword");
		}

		public int KeywordCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "", "Keyword");
			}
		}

		public bool HasKeyword()
		{
			return HasDomChild(NodeType.Element, "", "Keyword");
		}

		public KeywordType NewKeyword()
		{
			return new KeywordType(domNode.OwnerDocument.CreateElement("Keyword", ""));
		}

		public KeywordType GetKeywordAt(int index)
		{
			return new KeywordType(GetDomChildAt(NodeType.Element, "", "Keyword", index));
		}

		public XmlNode GetStartingKeywordCursor()
		{
			return GetDomFirstChild( NodeType.Element, "", "Keyword" );
		}

		public XmlNode GetAdvancedKeywordCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "", "Keyword", curNode );
		}

		public KeywordType GetKeywordValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new KeywordType( curNode );
		}


		public KeywordType GetKeyword()
		{
			return GetKeywordAt(0);
		}

		public KeywordType Keyword
		{
			get
			{
				return GetKeywordAt(0);
			}
		}

		public void RemoveKeywordAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "", "Keyword", index);
		}

		public void RemoveKeyword()
		{
			while (HasKeyword())
				RemoveKeywordAt(0);
		}

		public void AddKeyword(KeywordType newValue)
		{
			AppendDomElement("", "Keyword", newValue);
		}

		public void InsertKeywordAt(KeywordType newValue, int index)
		{
			InsertDomElementAt("", "Keyword", index, newValue);
		}

		public void ReplaceKeywordAt(KeywordType newValue, int index)
		{
			ReplaceDomElementAt("", "Keyword", index, newValue);
		}
		#endregion // Keyword accessor methods

		#region Keyword collection
        public KeywordCollection	MyKeywords = new KeywordCollection( );

        public class KeywordCollection: IEnumerable
        {
            KeywordListType parent;
            public KeywordListType Parent
			{
				set
				{
					parent = value;
				}
			}
			public KeywordEnumerator GetEnumerator() 
			{
				return new KeywordEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class KeywordEnumerator: IEnumerator 
        {
			int nIndex;
			KeywordListType parent;
			public KeywordEnumerator(KeywordListType par) 
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
				return(nIndex < parent.KeywordCount );
			}
			public KeywordType  Current 
			{
				get 
				{
					return(parent.GetKeywordAt(nIndex));
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

        #endregion // Keyword collection

        private void SetCollectionParents()
        {
            MyKeywords.Parent = this; 
	}
}
}
