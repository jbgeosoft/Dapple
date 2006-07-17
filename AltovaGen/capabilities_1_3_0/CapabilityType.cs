//
// CapabilityType.cs
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
	public class CapabilityType : Altova.Xml.Node
	{
		#region Forward constructors

		public CapabilityType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public CapabilityType(XmlNode node) : base(node) { SetCollectionParents(); }
		public CapabilityType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public CapabilityType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Request" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Request", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
				new RequestType(DOMNode).AdjustPrefix();
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Exception" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Exception", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
				new ExceptionType(DOMNode).AdjustPrefix();
			}

		    for (	XmlNode DOMNode = GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Layer" );
					DOMNode != null; 
					DOMNode = GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Layer", DOMNode )
				)
			{
				InternalAdjustPrefix(DOMNode, true);
				new LayerType(DOMNode).AdjustPrefix();
			}
		}



		#region Request accessor methods
		public static int GetRequestMinCount()
		{
			return 1;
		}

		public static int RequestMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetRequestMaxCount()
		{
			return 1;
		}

		public static int RequestMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetRequestCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Request");
		}

		public int RequestCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Request");
			}
		}

		public bool HasRequest()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Request");
		}

		public RequestType NewRequest()
		{
			return new RequestType(domNode.OwnerDocument.CreateElement("Request", "http://www.opengis.net/wms"));
		}

		public RequestType GetRequestAt(int index)
		{
			return new RequestType(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Request", index));
		}

		public XmlNode GetStartingRequestCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Request" );
		}

		public XmlNode GetAdvancedRequestCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Request", curNode );
		}

		public RequestType GetRequestValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new RequestType( curNode );
		}


		public RequestType GetRequest()
		{
			return GetRequestAt(0);
		}

		public RequestType Request
		{
			get
			{
				return GetRequestAt(0);
			}
		}

		public void RemoveRequestAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Request", index);
		}

		public void RemoveRequest()
		{
			while (HasRequest())
				RemoveRequestAt(0);
		}

		public void AddRequest(RequestType newValue)
		{
			AppendDomElement("http://www.opengis.net/wms", "Request", newValue);
		}

		public void InsertRequestAt(RequestType newValue, int index)
		{
			InsertDomElementAt("http://www.opengis.net/wms", "Request", index, newValue);
		}

		public void ReplaceRequestAt(RequestType newValue, int index)
		{
			ReplaceDomElementAt("http://www.opengis.net/wms", "Request", index, newValue);
		}
		#endregion // Request accessor methods

		#region Request collection
        public RequestCollection	MyRequests = new RequestCollection( );

        public class RequestCollection: IEnumerable
        {
            CapabilityType parent;
            public CapabilityType Parent
			{
				set
				{
					parent = value;
				}
			}
			public RequestEnumerator GetEnumerator() 
			{
				return new RequestEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class RequestEnumerator: IEnumerator 
        {
			int nIndex;
			CapabilityType parent;
			public RequestEnumerator(CapabilityType par) 
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
				return(nIndex < parent.RequestCount );
			}
			public RequestType  Current 
			{
				get 
				{
					return(parent.GetRequestAt(nIndex));
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

        #endregion // Request collection

		#region Exception accessor methods
		public static int GetExceptionMinCount()
		{
			return 1;
		}

		public static int ExceptionMinCount
		{
			get
			{
				return 1;
			}
		}

		public static int GetExceptionMaxCount()
		{
			return 1;
		}

		public static int ExceptionMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetExceptionCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Exception");
		}

		public int ExceptionCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Exception");
			}
		}

		public bool HasException()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Exception");
		}

		public ExceptionType NewException()
		{
			return new ExceptionType(domNode.OwnerDocument.CreateElement("Exception", "http://www.opengis.net/wms"));
		}

		public ExceptionType GetExceptionAt(int index)
		{
			return new ExceptionType(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Exception", index));
		}

		public XmlNode GetStartingExceptionCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Exception" );
		}

		public XmlNode GetAdvancedExceptionCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Exception", curNode );
		}

		public ExceptionType GetExceptionValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new ExceptionType( curNode );
		}


		public ExceptionType GetException()
		{
			return GetExceptionAt(0);
		}

		public ExceptionType Exception
		{
			get
			{
				return GetExceptionAt(0);
			}
		}

		public void RemoveExceptionAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Exception", index);
		}

		public void RemoveException()
		{
			while (HasException())
				RemoveExceptionAt(0);
		}

		public void AddException(ExceptionType newValue)
		{
			AppendDomElement("http://www.opengis.net/wms", "Exception", newValue);
		}

		public void InsertExceptionAt(ExceptionType newValue, int index)
		{
			InsertDomElementAt("http://www.opengis.net/wms", "Exception", index, newValue);
		}

		public void ReplaceExceptionAt(ExceptionType newValue, int index)
		{
			ReplaceDomElementAt("http://www.opengis.net/wms", "Exception", index, newValue);
		}
		#endregion // Exception accessor methods

		#region Exception collection
        public ExceptionCollection	MyExceptions = new ExceptionCollection( );

        public class ExceptionCollection: IEnumerable
        {
            CapabilityType parent;
            public CapabilityType Parent
			{
				set
				{
					parent = value;
				}
			}
			public ExceptionEnumerator GetEnumerator() 
			{
				return new ExceptionEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class ExceptionEnumerator: IEnumerator 
        {
			int nIndex;
			CapabilityType parent;
			public ExceptionEnumerator(CapabilityType par) 
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
				return(nIndex < parent.ExceptionCount );
			}
			public ExceptionType  Current 
			{
				get 
				{
					return(parent.GetExceptionAt(nIndex));
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

        #endregion // Exception collection

		#region Layer accessor methods
		public static int GetLayerMinCount()
		{
			return 0;
		}

		public static int LayerMinCount
		{
			get
			{
				return 0;
			}
		}

		public static int GetLayerMaxCount()
		{
			return 1;
		}

		public static int LayerMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetLayerCount()
		{
			return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Layer");
		}

		public int LayerCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.opengis.net/wms", "Layer");
			}
		}

		public bool HasLayer()
		{
			return HasDomChild(NodeType.Element, "http://www.opengis.net/wms", "Layer");
		}

		public LayerType NewLayer()
		{
			return new LayerType(domNode.OwnerDocument.CreateElement("Layer", "http://www.opengis.net/wms"));
		}

		public LayerType GetLayerAt(int index)
		{
			return new LayerType(GetDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Layer", index));
		}

		public XmlNode GetStartingLayerCursor()
		{
			return GetDomFirstChild( NodeType.Element, "http://www.opengis.net/wms", "Layer" );
		}

		public XmlNode GetAdvancedLayerCursor( XmlNode curNode )
		{
			return GetDomNextChild( NodeType.Element, "http://www.opengis.net/wms", "Layer", curNode );
		}

		public LayerType GetLayerValueAtCursor( XmlNode curNode )
		{
			if( curNode == null )
				  throw new Altova.Xml.XmlException("Out of range");
			else
				return new LayerType( curNode );
		}


		public LayerType GetLayer()
		{
			return GetLayerAt(0);
		}

		public LayerType Layer
		{
			get
			{
				return GetLayerAt(0);
			}
		}

		public void RemoveLayerAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.opengis.net/wms", "Layer", index);
		}

		public void RemoveLayer()
		{
			while (HasLayer())
				RemoveLayerAt(0);
		}

		public void AddLayer(LayerType newValue)
		{
			AppendDomElement("http://www.opengis.net/wms", "Layer", newValue);
		}

		public void InsertLayerAt(LayerType newValue, int index)
		{
			InsertDomElementAt("http://www.opengis.net/wms", "Layer", index, newValue);
		}

		public void ReplaceLayerAt(LayerType newValue, int index)
		{
			ReplaceDomElementAt("http://www.opengis.net/wms", "Layer", index, newValue);
		}
		#endregion // Layer accessor methods

		#region Layer collection
        public LayerCollection	MyLayers = new LayerCollection( );

        public class LayerCollection: IEnumerable
        {
            CapabilityType parent;
            public CapabilityType Parent
			{
				set
				{
					parent = value;
				}
			}
			public LayerEnumerator GetEnumerator() 
			{
				return new LayerEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class LayerEnumerator: IEnumerator 
        {
			int nIndex;
			CapabilityType parent;
			public LayerEnumerator(CapabilityType par) 
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
				return(nIndex < parent.LayerCount );
			}
			public LayerType  Current 
			{
				get 
				{
					return(parent.GetLayerAt(nIndex));
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

        #endregion // Layer collection

        private void SetCollectionParents()
        {
            MyRequests.Parent = this; 
            MyExceptions.Parent = this; 
            MyLayers.Parent = this; 
	}
}
}
