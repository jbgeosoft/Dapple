﻿using System;
using System.ComponentModel;

namespace NewServerTree
{
	internal class AvailableServersModelNode : ModelNode
	{
		#region Member Variables

		private WMSRootModelNode m_oWMSRootNode;
		private ArcIMSRootModelNode m_oArcIMSRootNode;
		private DapServerRootModelNode m_oDAPRootNode;
		private ImageTileSetRootModelNode m_oTileRootNode;
		private VERootModelNode m_oVERootNode;
		private PersonalDapServerModelNode m_oPersonalDAPServer;

		#endregion


		#region Constructors

		internal AvailableServersModelNode(DappleModel oModel)
			: base(oModel)
		{
			m_oDAPRootNode = new DapServerRootModelNode(m_oModel);
			AddChildSilently(m_oDAPRootNode);

			if (PersonalDapServerModelNode.PersonalDapRunning)
			{
				m_oPersonalDAPServer = new PersonalDapServerModelNode(m_oModel);
				AddChildSilently(m_oPersonalDAPServer);
				m_oPersonalDAPServer.BeginLoad();
			}

			m_oTileRootNode = new ImageTileSetRootModelNode(m_oModel);
			AddChildSilently(m_oTileRootNode);

			m_oVERootNode = new VERootModelNode(m_oModel);
			AddChildSilently(m_oVERootNode);

			m_oWMSRootNode = new WMSRootModelNode(m_oModel);
			AddChildSilently(m_oWMSRootNode);

			m_oArcIMSRootNode = new ArcIMSRootModelNode(m_oModel);
			AddChildSilently(m_oArcIMSRootNode);

			MarkLoaded();
		}

		#endregion


		#region Properties

		[Browsable(false)]
		internal override bool ShowAllChildren
		{
			get { return UseShowAllChildren; }
		}

		internal override String DisplayText
		{
			get { return "Available Servers"; }
		}

		[Browsable(false)]
		internal override string IconKey
		{
			get { return IconKeys.AvailableServers; }
		}

		internal DapServerRootModelNode DAPServers
		{
			get { return m_oDAPRootNode; }
		}

		internal ImageTileSetRootModelNode ImageTileSets
		{
			get { return m_oTileRootNode; }
		}

		internal WMSRootModelNode WMSServers
		{
			get { return m_oWMSRootNode; }
		}

		internal ArcIMSRootModelNode ArcIMSServers
		{
			get { return m_oArcIMSRootNode; }
		}

		internal PersonalDapServerModelNode PersonalDapServer
		{
			get { return m_oPersonalDAPServer; }
		}

		#endregion


		#region Public Methods

		internal void Clear()
		{
			m_oDAPRootNode.ClearSilently();
			m_oTileRootNode.ClearSilently();
			m_oWMSRootNode.ClearSilently();
			m_oArcIMSRootNode.ClearSilently();
		}

		internal ServerModelNode SetFavouriteServer(String strUri)
		{
			ServerModelNode temp, result = null;

			temp = m_oDAPRootNode.SetFavouriteServer(strUri);
			if (temp != null) result = temp;
			if (m_oPersonalDAPServer != null && m_oPersonalDAPServer.UpdateFavouriteStatus(strUri))
				result = m_oPersonalDAPServer;
			temp = m_oWMSRootNode.SetFavouriteServer(strUri);
			if (temp != null) result = temp;
			temp = m_oArcIMSRootNode.SetFavouriteServer(strUri);
			if (temp != null) result = temp;

			return result;
		}

		#region Saving and Loading old Dapple Views

		internal void SaveToView(Dapple.DappleView oView)
		{
			dappleview.serversType oServers = oView.View.Newservers();

			m_oDAPRootNode.SaveToView(oServers);
			m_oTileRootNode.SaveToView(oServers);
			m_oVERootNode.SaveToView(oServers);

			dappleview.builderentryType oWMSBuilder = oServers.Newbuilderentry();
			dappleview.builderdirectoryType oWMSDir = oWMSBuilder.Newbuilderdirectory();
			oWMSDir.Addname(new Altova.Types.SchemaString("WMS Servers"));
			oWMSDir.Addspecialcontainer(new dappleview.SpecialDirectoryType("WMSServers"));

			m_oWMSRootNode.SaveToView(oWMSDir);
			m_oArcIMSRootNode.SaveToView(oWMSDir);


			oWMSBuilder.Addbuilderdirectory(oWMSDir);
			oServers.Addbuilderentry(oWMSBuilder);

			oView.View.Addservers(oServers);
		}

		#endregion

		#endregion


		#region Helper Methods

		protected override ModelNode[] Load()
		{
			throw new ApplicationException(ErrLoadedBadNode);
		}

		#endregion
	}
}