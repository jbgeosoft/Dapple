//
// ContactPositionType.cs
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
	public class ContactPositionType : Altova.Xml.Node
	{
		#region Forward constructors

		public ContactPositionType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public ContactPositionType(XmlNode node) : base(node) { SetCollectionParents(); }
		public ContactPositionType(Altova.Xml.Node node) : base(node) { SetCollectionParents(); }
		public ContactPositionType(Altova.Xml.Document doc, string namespaceURI, string prefix, string name) : base(doc, namespaceURI, prefix, name) { SetCollectionParents(); }
		#endregion // Forward constructors

		#region Value accessor methods
		public SchemaString GetValue()
		{
			return new SchemaString(GetDomNodeValue(domNode));
		}

		public void SetValue(ISchemaType newValue)
		{
			SetDomNodeValue(domNode, newValue.ToString());
		}

		public void Assign(ISchemaType newValue)
		{
			SetValue(newValue);
		}

		public SchemaString Value
		{
			get
			{
				return new SchemaString(GetDomNodeValue(domNode));
			}
			set
			{
				SetDomNodeValue(domNode, value.ToString());
			}
		}
		#endregion // Value accessor methods

		public override void AdjustPrefix()
		{
		}



		public void AddTextNode(SchemaString newValue)
		{
			AppendDomChild(NodeType.Text, "", "", newValue.ToString());
		}
		public void AddProcessingInstruction(SchemaString name, SchemaString newValue)
		{
			AppendDomChild(NodeType.ProcessingInstruction , "", name.ToString(), newValue.ToString());
		}
		public void AddCDataNode(SchemaString newValue)
		{
			AppendDomChild(NodeType.CData, "", "", newValue.ToString());
		}
		public void AddComment(SchemaString newValue)
		{
			AppendDomChild(NodeType.Comment, "", "", newValue.ToString());
		}
        private void SetCollectionParents()
        {
	}
}
}
