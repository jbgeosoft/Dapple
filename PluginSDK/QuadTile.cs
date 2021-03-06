using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using WorldWind;
using WorldWind.Camera;
using WorldWind.Net;
using WorldWind.Terrain;
using WorldWind.VisualControl;
using Utility;

namespace WorldWind.Renderable
{
	public class QuadTile : IGeoSpatialDownloadTile, IDisposable
	{
		readonly Color LightColor = Color.FromArgb(255, 255, 255);

		/// <summary>
		/// Child tile location
		/// </summary>
		public enum ChildLocation
		{
			NorthWest,
			SouthWest,
			NorthEast,
			SouthEast
		}

		internal double LatitudeSpan;
		internal double LongitudeSpan;

		private QuadTileSet quadTileSet;
		private Angle centerLatitude;
		private Angle centerLongitude;
		private int level;
		private double west;
		private double east;
		private double north;
		private double south;

		// These are the projected coordinate extents for the tile (for image storesa containing projections
		internal UV UL, UR, LL, LR;

		private int row;
		private int col;

		internal bool isInitialized;
		internal BoundingBox BoundingBox;

		private List<GeoSpatialDownloadRequest> downloadRequests;

		protected Texture[] textures;

		/// <summary>
		/// Number of points in child flat mesh grid (times 2)
		/// </summary>
		protected static int vertexCount = 40;

		/// <summary>
		/// Number of points in child terrain mesh grid (times 2)
		/// </summary>
		protected static int vertexCountElevated = 40;

		protected QuadTile northWestChild;
		protected QuadTile southWestChild;
		protected QuadTile northEastChild;
		protected QuadTile southEastChild;

		protected CustomVertex.PositionNormalTextured[] northWestVertices;
		protected CustomVertex.PositionNormalTextured[] southWestVertices;
		protected CustomVertex.PositionNormalTextured[] northEastVertices;
		protected CustomVertex.PositionNormalTextured[] southEastVertices;
		protected short[] vertexIndexes;
		protected Point3d localOrigin; // Add this offset to get world coordinates

		protected bool m_isResetingCache;

		/// <summary>
		/// The vertical exaggeration the tile mesh was computed for
		/// </summary>
		protected float verticalExaggeration;

		protected bool isDownloadingTerrain;

		private string key;

		/// New Cache idea
		/// 
		internal static Dictionary<string, CacheEntry> VerticeCache
			 = new Dictionary<string, CacheEntry>();

		internal static int CacheSize = 256;

		internal class CacheEntry
		{
			internal CustomVertex.PositionNormalTextured[] northWestVertices;
			internal CustomVertex.PositionNormalTextured[] southWestVertices;
			internal CustomVertex.PositionNormalTextured[] northEastVertices;
			internal CustomVertex.PositionNormalTextured[] southEastVertices;
			internal short[] vertexIndexes;

			internal DateTime EntryTime;
		}

		// End New Cache

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.QuadTile"/> class.
		/// </summary>
		/// <param name="_south"></param>
		/// <param name="_north"></param>
		/// <param name="_west"></param>
		/// <param name="_east"></param>
		/// <param name="_level"></param>
		/// <param name="quadTileSet"></param>
		internal QuadTile(double _south, double _north, double _west, double _east, int _level, QuadTileSet quadTileSet)
		{
			this.south = _south;
			this.north = _north;
			this.west = _west;
			this.east = _east;
			centerLatitude = Angle.FromDegrees(0.5f * (north + south));
			centerLongitude = Angle.FromDegrees(0.5f * (west + east));
			LatitudeSpan = Math.Abs(north - south);
			LongitudeSpan = Math.Abs(east - west);

			this.level = _level;
			this.quadTileSet = quadTileSet;

			// --- Make bounding box slightly larger so the tile won't blink in and out near the edges ---
			BoundingBox = new BoundingBox(south - LatitudeSpan / 8.0, north + LatitudeSpan / 8.0, west - LongitudeSpan / 8.0, east + LongitudeSpan / 8.0, quadTileSet.LayerRadius, quadTileSet.LayerRadius + 300000.0);
			//localOrigin = BoundingBox.CalculateCenter();
			localOrigin = MathEngine.SphericalToCartesian(centerLatitude, centerLongitude, quadTileSet.LayerRadius);

			// To avoid gaps between neighbouring tiles truncate the origin to 
			// a number that doesn't get rounded. (nearest 10km)
			localOrigin.X = (float)(Math.Round(localOrigin.X / 10000) * 10000);
			localOrigin.Y = (float)(Math.Round(localOrigin.Y / 10000) * 10000);
			localOrigin.Z = (float)(Math.Round(localOrigin.Z / 10000) * 10000);


			row = MathEngine.GetRowFromLatitude((north + south) / 2.0, north - south);
			col = MathEngine.GetColFromLongitude((east + west) / 2.0, east - west);

			downloadRequests = new List<GeoSpatialDownloadRequest>();

			key = string.Format("{0,4}", this.level)
					+ "_"
					+ string.Format("{0,4}", this.col)
					+ string.Format("{0,4}", this.row)
					+ this.quadTileSet.Name
					+ this.quadTileSet.ParentList.Name;

			quadTileSet.ImageStores[0].GetProjectionCorners(this, out UL, out UR, out LL, out LR);
		}

		public override string ToString()
        {
            return String.Format("QuadTile:Set={0} Level={1} X={2} Y={3} NS EW={4},{5}  {6},{7}",
                quadTileSet.Name, Level, Col, Row, North, South, East, West);
        }

		internal virtual void ResetCache()
		{
			m_isResetingCache = true;
			this.isInitialized = false;

			if (northEastChild != null)
			{
				northEastChild.ResetCache();
			}

			if (northWestChild != null)
			{
				northWestChild.ResetCache();
			}

			if (southEastChild != null)
			{
				southEastChild.ResetCache();
			}

			if (southWestChild != null)
			{
				southWestChild.ResetCache();
			}

			this.Dispose();

			for (int i = 0; i < quadTileSet.ImageStores.Length; i++)
			{
				if ((quadTileSet.ImageStores[i] != null) && quadTileSet.ImageStores[i].IsDownloadableLayer)
					quadTileSet.ImageStores[i].DeleteLocalCopy(this);
			}

			m_isResetingCache = false;
		}

		/// <summary>
		/// Returns the QuadTile for specified location if available.
		/// Tries to queue a download if not available.
		/// </summary>
		/// <returns>Initialized QuadTile if available locally, else null.</returns>
		private QuadTile ComputeChild(double childSouth, double childNorth, double childWest, double childEast)
		{
			QuadTile child = new QuadTile(
				 childSouth,
				 childNorth,
				 childWest,
				 childEast,
				 this.level + 1,
				 quadTileSet);

			return child;
		}

		internal virtual void ComputeChildren(DrawArgs drawArgs)
		{
			if (level + 1 >= quadTileSet.ImageStores[0].LevelCount)
				return;

			double CenterLat = 0.5f * (south + north);
			double CenterLon = 0.5f * (east + west);
			if (northWestChild == null)
				northWestChild = ComputeChild(CenterLat, north, west, CenterLon);

			if (northEastChild == null)
				northEastChild = ComputeChild(CenterLat, north, CenterLon, east);

			if (southWestChild == null)
				southWestChild = ComputeChild(south, CenterLat, west, CenterLon);

			if (southEastChild == null)
				southEastChild = ComputeChild(south, CenterLat, CenterLon, east);
		}

		public virtual void Dispose()
		{
			isInitialized = false;
			if (textures != null)
			{
				for (int i = 0; i < textures.Length; i++)
				{
					if (textures[i] != null && !textures[i].Disposed)
					{
						textures[i].Dispose();
						textures[i] = null;
					}
				}
			}

			if (northWestChild != null)
			{
				northWestChild.Dispose();
				northWestChild = null;
			}
			if (southWestChild != null)
			{
				southWestChild.Dispose();
				southWestChild = null;
			}
			if (northEastChild != null)
			{
				northEastChild.Dispose();
				northEastChild = null;
			}
			if (southEastChild != null)
			{
				southEastChild.Dispose();
				southEastChild = null;
			}

			if (downloadRequests != null)
			{
				foreach (GeoSpatialDownloadRequest request in downloadRequests)
				{
					quadTileSet.RemoveFromDownloadQueue(request, false);
					request.Dispose();
				}
				downloadRequests.Clear();
			}
		}

		private bool waitingForDownload;
		private bool isDownloadingImage;

		public virtual void Initialize()
		{
			if (m_isResetingCache)
				return;

			if (downloadRequests.Count > 0)
			{
				// Waiting for download
				return;
			}
			if (textures == null)
			{
				textures = new Texture[quadTileSet.ImageStores.Length];

				// not strictly necessary
				for (int i = 0; i < textures.Length; i++)
					textures[i] = null;
			}

			// assume we're finished.
			waitingForDownload = false;

			// check for missing textures.
			for (int i = 0; i < textures.Length; i++)
			{
				Texture newTexture = quadTileSet.ImageStores[i].LoadFile(this);
				if (newTexture == null)
				{
					// At least one texture missing, wait for download
					waitingForDownload = true;
				}

				// not entirely sure if this is a good idea...
				if (textures[i] != null)
					textures[i].Dispose();

				textures[i] = newTexture;
			}
			if (waitingForDownload)
				return;

			isDownloadingImage = false;
			CreateTileMesh();
			isInitialized = true;
		}

		/// <summary>
		/// Updates this layer (background)
		/// </summary>
		internal virtual void Update(DrawArgs drawArgs)
		{
			if (m_isResetingCache)
				return;

			double tileSize = north - south;

			if (!isInitialized)
			{
				if ((DrawArgs.Camera.ViewRange * 0.5f < Angle.FromDegrees(quadTileSet.TileDrawDistance * tileSize)
						&& MathEngine.SphericalDistance(centerLatitude, centerLongitude, DrawArgs.Camera.Latitude, DrawArgs.Camera.Longitude) <
						 Angle.FromDegrees(quadTileSet.TileDrawSpread * tileSize * 1.25f)
					 && DrawArgs.Camera.ViewFrustum.Intersects(BoundingBox))
					|| (level == 0 && quadTileSet.AlwaysRenderBaseTiles)
					 )
					Initialize();
			}

			if (isInitialized && World.Settings.VerticalExaggeration != verticalExaggeration ||
				 m_CurrentOpacity != quadTileSet.Opacity)
			{
				CreateTileMesh();
			}

			if (isInitialized)
			{
				if (DrawArgs.Camera.ViewRange < Angle.FromDegrees(quadTileSet.TileDrawDistance * tileSize)
			 && MathEngine.SphericalDistance(centerLatitude, centerLongitude,
																DrawArgs.Camera.Latitude, DrawArgs.Camera.Longitude) <
						 Angle.FromDegrees(quadTileSet.TileDrawSpread * tileSize)
					 && DrawArgs.Camera.ViewFrustum.Intersects(BoundingBox)
			 )
				{
					if (northEastChild == null || northWestChild == null || southEastChild == null ||
						 southWestChild == null)
					{
						ComputeChildren(drawArgs);
					}

					if (northEastChild != null)
					{
						northEastChild.Update(drawArgs);
					}

					if (northWestChild != null)
					{
						northWestChild.Update(drawArgs);
					}

					if (southEastChild != null)
					{
						southEastChild.Update(drawArgs);
					}

					if (southWestChild != null)
					{
						southWestChild.Update(drawArgs);
					}
				}
				else
				{
					if (northWestChild != null)
					{
						northWestChild.Dispose();
						northWestChild = null;
					}

					if (northEastChild != null)
					{
						northEastChild.Dispose();
						northEastChild = null;
					}

					if (southEastChild != null)
					{
						southEastChild.Dispose();
						southEastChild = null;
					}

					if (southWestChild != null)
					{
						southWestChild.Dispose();
						southWestChild = null;
					}
				}
			}

			if (isInitialized)
			{
				if (DrawArgs.Camera.ViewRange / 2 > Angle.FromDegrees(quadTileSet.TileDrawDistance * tileSize * 1.5f)
					 ||
					 MathEngine.SphericalDistance(centerLatitude, centerLongitude, DrawArgs.Camera.Latitude,
															DrawArgs.Camera.Longitude) >
					 Angle.FromDegrees(quadTileSet.TileDrawSpread * tileSize * 1.5f))
				{
					if (level != 0 || (level == 0 && !quadTileSet.AlwaysRenderBaseTiles))
						this.Dispose();
				}
			}
		}

		/// <summary>
		/// Builds flat or terrain mesh for current tile
		/// </summary>
		//internal virtual void CreateTileMesh()
		//{
		//    verticalExaggeration = World.Settings.VerticalExaggeration;
		//    m_CurrentOpacity = quadTileSet.Opacity;
		//    renderStruts = quadTileSet.RenderStruts;
		//    if (quadTileSet.TerrainMapped && Math.Abs(verticalExaggeration) > 1e-3)
		//        CreateElevatedMesh();
		//    else
		//        CreateFlatMesh();
		//}
		internal virtual void CreateTileMesh()
		{
			lock (((System.Collections.ICollection)VerticeCache).SyncRoot)
			{
				if (VerticeCache.ContainsKey(key) && World.Settings.VerticalExaggeration == verticalExaggeration)
				{
					this.northWestVertices = VerticeCache[key].northWestVertices;
					this.southWestVertices = VerticeCache[key].southWestVertices;
					this.northEastVertices = VerticeCache[key].northEastVertices;
					this.southEastVertices = VerticeCache[key].southEastVertices;
					this.vertexIndexes = VerticeCache[key].vertexIndexes;

					VerticeCache[key].EntryTime = DateTime.Now;

					return;
				}

				verticalExaggeration = World.Settings.VerticalExaggeration;
				m_CurrentOpacity = quadTileSet.Opacity;

				Projection proj = quadTileSet.ImageStores[0].Projection;
				bool bTerrain = quadTileSet.TerrainMapped && Math.Abs(verticalExaggeration) > 1e-3;
				if (proj != null)
					CreateProjectedMesh(proj, bTerrain);
				else if (bTerrain)
					CreateElevatedMesh();
				else
					CreateFlatMesh();

				AddToCache();
			}
		}

		private void AddToCache()
		{
			if (!VerticeCache.ContainsKey(key))
			{
				if (VerticeCache.Count >= CacheSize)
				{
					for (int i = 0; i < 10; i++)
					{
						// Remove least recently used tile
						CacheEntry oldestTile = null;
						string k = "";
						foreach (KeyValuePair<string, CacheEntry> curEntry in VerticeCache)
						{
							if (oldestTile == null)
								oldestTile = curEntry.Value;
							else
							{
								if (curEntry.Value.EntryTime < oldestTile.EntryTime)
								{
									oldestTile = curEntry.Value;
									k = curEntry.Key;
								}
							}
						}
						VerticeCache.Remove(k);
					}
				}

				CacheEntry c = new CacheEntry();
				c.EntryTime = DateTime.Now;
				c.northEastVertices = this.northEastVertices;
				c.northWestVertices = this.northWestVertices;
				c.southEastVertices = this.southEastVertices;
				c.southWestVertices = this.southWestVertices;
				c.vertexIndexes = this.vertexIndexes;

				VerticeCache.Add(key, c);
			}
		}


		/// <summary>
		/// Builds a mesh using a projection (with terrain if requested)
		/// </summary>
		protected virtual void CreateProjectedMesh(Projection proj, bool bTerrain)
		{
			int baseIndex;
			UV geo;
			double sinLat, cosLat, sinLon, cosLon, height, latRange = North - South;
			double layerRadius = (double)quadTileSet.LayerRadius;
			double scaleFactor = 1.0 / (double)vertexCount;
			int thisVertexCount = vertexCount / 2 + (vertexCount % 2);
			int thisVertexCountPlus1 = thisVertexCount + 1;

			const double Degrees2Radians = Math.PI / 180.0;

			int totalVertexCount = thisVertexCountPlus1 * thisVertexCountPlus1;
			northWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			southWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			northEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			southEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];

			double uStep = (UR.U - UL.U) * scaleFactor;
			double vStep = (UL.V - LL.V) * scaleFactor;

			// figure out latrange (for terrain detail)
			if (bTerrain)
			{
				UV geoUL = proj.Inverse(UL);
				UV geoLR = proj.Inverse(LR);
				latRange = (geoUL.V - geoLR.V) * 180 / Math.PI;
			}

			baseIndex = 0;
			UV curUnprojected = new UV(UL.U, UL.V);
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					geo = proj.Inverse(curUnprojected);

					sinLat = Math.Sin(geo.V);
					sinLon = Math.Sin(geo.U);
					cosLat = Math.Cos(geo.V);
					cosLon = Math.Cos(geo.U);

					height = layerRadius;
					if (bTerrain)
					{
						// Radians -> Degrees
						geo.U /= Degrees2Radians;
						geo.V /= Degrees2Radians;
						height += verticalExaggeration * quadTileSet.World.TerrainAccessor.GetElevationAt(geo.V, geo.U, Math.Abs(vertexCount / latRange));
					}

					northWestVertices[baseIndex].X = (float)(height * cosLat * cosLon - localOrigin.X);
					northWestVertices[baseIndex].Y = (float)(height * cosLat * sinLon - localOrigin.Y);
					northWestVertices[baseIndex].Z = (float)(height * sinLat - localOrigin.Z);
					northWestVertices[baseIndex].Tu = (float)(j * scaleFactor);
					northWestVertices[baseIndex].Tv = (float)(i * scaleFactor);
					northWestVertices[baseIndex].Normal =
						 new Vector3(northWestVertices[baseIndex].X + (float)localOrigin.X,
										 northWestVertices[baseIndex].Y + (float)localOrigin.Y,
										 northWestVertices[baseIndex].Z + (float)localOrigin.Z);
					northWestVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;

					curUnprojected.U += uStep;
				}
				curUnprojected.U = UL.U;
				curUnprojected.V -= vStep;
			}

			baseIndex = 0;
			curUnprojected = new UV(UL.U, UL.V - (UL.V - LL.V) / 2.0);
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					geo = proj.Inverse(curUnprojected);

					sinLat = Math.Sin(geo.V);
					sinLon = Math.Sin(geo.U);
					cosLat = Math.Cos(geo.V);
					cosLon = Math.Cos(geo.U);

					height = layerRadius;
					if (bTerrain)
					{
						// Radians -> Degrees
						geo.U /= Degrees2Radians;
						geo.V /= Degrees2Radians;
						height += verticalExaggeration * quadTileSet.World.TerrainAccessor.GetElevationAt(geo.V, geo.U, Math.Abs(vertexCount / latRange));
					}

					southWestVertices[baseIndex].X = (float)(height * cosLat * cosLon - localOrigin.X);
					southWestVertices[baseIndex].Y = (float)(height * cosLat * sinLon - localOrigin.Y);
					southWestVertices[baseIndex].Z = (float)(height * sinLat - localOrigin.Z);
					southWestVertices[baseIndex].Tu = (float)(j * scaleFactor);
					southWestVertices[baseIndex].Tv = (float)((i + thisVertexCount) * scaleFactor);
					southWestVertices[baseIndex].Normal =
						 new Vector3(southWestVertices[baseIndex].X + (float)localOrigin.X,
										 southWestVertices[baseIndex].Y + (float)localOrigin.Y,
										 southWestVertices[baseIndex].Z + (float)localOrigin.Z);
					southWestVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;
					curUnprojected.U += uStep;
				}
				curUnprojected.U = UL.U;
				curUnprojected.V -= vStep;
			}

			baseIndex = 0;
			curUnprojected = new UV(UL.U + (UR.U - UL.U) / 2.0, UL.V);
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					geo = proj.Inverse(curUnprojected);

					sinLat = Math.Sin(geo.V);
					sinLon = Math.Sin(geo.U);
					cosLat = Math.Cos(geo.V);
					cosLon = Math.Cos(geo.U);

					height = layerRadius;
					if (bTerrain)
					{
						// Radians -> Degrees
						geo.U /= Degrees2Radians;
						geo.V /= Degrees2Radians;
						height += verticalExaggeration * quadTileSet.World.TerrainAccessor.GetElevationAt(geo.V, geo.U, Math.Abs(vertexCount / latRange));
					}

					northEastVertices[baseIndex].X = (float)(height * cosLat * cosLon - localOrigin.X);
					northEastVertices[baseIndex].Y = (float)(height * cosLat * sinLon - localOrigin.Y);
					northEastVertices[baseIndex].Z = (float)(height * sinLat - localOrigin.Z);
					northEastVertices[baseIndex].Tu = (float)((j + thisVertexCount) * scaleFactor);
					northEastVertices[baseIndex].Tv = (float)(i * scaleFactor);
					northEastVertices[baseIndex].Normal =
						 new Vector3(northEastVertices[baseIndex].X + (float)localOrigin.X,
										 northEastVertices[baseIndex].Y + (float)localOrigin.Y,
										 northEastVertices[baseIndex].Z + (float)localOrigin.Z);
					northEastVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;
					curUnprojected.U += uStep;
				}
				curUnprojected.U = UL.U + (UR.U - UL.U) / 2.0;
				curUnprojected.V -= vStep;
			}

			baseIndex = 0;
			curUnprojected = new UV(UL.U + (UR.U - UL.U) / 2.0, UL.V - (UL.V - LL.V) / 2.0);
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					geo = proj.Inverse(curUnprojected);

					sinLat = Math.Sin(geo.V);
					sinLon = Math.Sin(geo.U);
					cosLat = Math.Cos(geo.V);
					cosLon = Math.Cos(geo.U);

					height = layerRadius;
					if (bTerrain)
					{
						// Radians -> Degrees
						geo.U /= Degrees2Radians;
						geo.V /= Degrees2Radians;
						height += verticalExaggeration * quadTileSet.World.TerrainAccessor.GetElevationAt(geo.V, geo.U, Math.Abs(vertexCount / latRange));
					}

					southEastVertices[baseIndex].X = (float)(height * cosLat * cosLon - localOrigin.X);
					southEastVertices[baseIndex].Y = (float)(height * cosLat * sinLon - localOrigin.Y);
					southEastVertices[baseIndex].Z = (float)(height * sinLat - localOrigin.Z);
					southEastVertices[baseIndex].Tu = (float)((j + thisVertexCount) * scaleFactor);
					southEastVertices[baseIndex].Tv = (float)((i + thisVertexCount) * scaleFactor);
					southEastVertices[baseIndex].Normal =
						 new Vector3(southEastVertices[baseIndex].X + (float)localOrigin.X,
										 southEastVertices[baseIndex].Y + (float)localOrigin.Y,
										 southEastVertices[baseIndex].Z + (float)localOrigin.Z);
					southEastVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;
					curUnprojected.U += uStep;
				}
				curUnprojected.U = UL.U + (UR.U - UL.U) / 2.0;
				curUnprojected.V -= vStep;
			}

			vertexIndexes = new short[2 * thisVertexCount * thisVertexCount * 3];

			for (int i = 0; i < thisVertexCount; i++)
			{
				baseIndex = (2 * 3 * i * thisVertexCount);

				for (int j = 0; j < thisVertexCount; j++)
				{
					vertexIndexes[baseIndex] = (short)(i * thisVertexCountPlus1 + j);
					vertexIndexes[baseIndex + 1] = (short)((i + 1) * thisVertexCountPlus1 + j);
					vertexIndexes[baseIndex + 2] = (short)(i * thisVertexCountPlus1 + j + 1);

					vertexIndexes[baseIndex + 3] = (short)(i * thisVertexCountPlus1 + j + 1);
					vertexIndexes[baseIndex + 4] = (short)((i + 1) * thisVertexCountPlus1 + j);
					vertexIndexes[baseIndex + 5] = (short)((i + 1) * thisVertexCountPlus1 + j + 1);

					baseIndex += 6;
				}
			}
			// JBTODO: Should we normalize here for elevated mesh?
		}

		// Edits by Patrick Murris : fixing mesh sides normals (2006-11-18)

		/// <summary>
		/// Builds a flat mesh (no terrain)
		/// </summary>
		protected virtual void CreateFlatMesh()
		{
			int baseIndex;
			double layerRadius = (double)quadTileSet.LayerRadius;
			double scaleFactor = 1.0 / (double)vertexCount;
			int thisVertexCount = vertexCount / 2 + (vertexCount % 2);
			int thisVertexCountPlus1 = thisVertexCount + 1;

			const double Degrees2Radians = Math.PI / 180.0;

			int totalVertexCount = thisVertexCountPlus1 * thisVertexCountPlus1;
			northWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			southWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			northEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			southEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];

			// Cache western sin/cos of longitude values
			double[] sinLon = new double[thisVertexCountPlus1];
			double[] cosLon = new double[thisVertexCountPlus1];

			double angle = west * Degrees2Radians;
			double angleConst;
			double deltaAngle = scaleFactor * LongitudeSpan * Degrees2Radians;

			angleConst = west * Degrees2Radians;
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				angle = angleConst + i * deltaAngle;
				sinLon[i] = Math.Sin(angle);
				cosLon[i] = Math.Cos(angle);
				//angle += deltaAngle;
			}

			baseIndex = 0;
			angleConst = north * Degrees2Radians;
			deltaAngle = -scaleFactor * LatitudeSpan * Degrees2Radians;
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				angle = angleConst + i * deltaAngle;
				double sinLat = Math.Sin(angle);
				double radCosLat = Math.Cos(angle) * layerRadius;

				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					northWestVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
					northWestVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
					northWestVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
					northWestVertices[baseIndex].Tu = (float)(j * scaleFactor);
					northWestVertices[baseIndex].Tv = (float)(i * scaleFactor);
					northWestVertices[baseIndex].Normal =
						 new Vector3(northWestVertices[baseIndex].X + (float)localOrigin.X,
										 northWestVertices[baseIndex].Y + (float)localOrigin.Y,
										 northWestVertices[baseIndex].Z + (float)localOrigin.Z);
					northWestVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;
				}
				//angle += deltaAngle;
			}

			baseIndex = 0;
			angleConst = 0.5 * (north + south) * Degrees2Radians;
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				angle = angleConst + i * deltaAngle;
				double sinLat = Math.Sin(angle);
				double radCosLat = Math.Cos(angle) * layerRadius;

				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					southWestVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
					southWestVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
					southWestVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
					southWestVertices[baseIndex].Tu = (float)(j * scaleFactor);
					southWestVertices[baseIndex].Tv = (float)((i + thisVertexCount) * scaleFactor);
					southWestVertices[baseIndex].Normal =
						 new Vector3(southWestVertices[baseIndex].X + (float)localOrigin.X,
										 southWestVertices[baseIndex].Y + (float)localOrigin.Y,
										 southWestVertices[baseIndex].Z + (float)localOrigin.Z);
					southWestVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;
				}
				//angle += deltaAngle;
			}

			// Cache eastern sin/cos of longitude values
			angleConst = 0.5 * (west + east) * Degrees2Radians;
			deltaAngle = scaleFactor * LongitudeSpan * Degrees2Radians;
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				angle = angleConst + i * deltaAngle;
				sinLon[i] = Math.Sin(angle);
				cosLon[i] = Math.Cos(angle);
				//angle += deltaAngle;
			}

			baseIndex = 0;
			angleConst = north * Degrees2Radians;
			deltaAngle = -scaleFactor * LatitudeSpan * Degrees2Radians;
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				angle = angleConst + i * deltaAngle;
				double sinLat = Math.Sin(angle);
				double radCosLat = Math.Cos(angle) * layerRadius;

				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					northEastVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
					northEastVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
					northEastVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
					northEastVertices[baseIndex].Tu = (float)((j + thisVertexCount) * scaleFactor);
					northEastVertices[baseIndex].Tv = (float)(i * scaleFactor);
					northEastVertices[baseIndex].Normal =
						 new Vector3(northEastVertices[baseIndex].X + (float)localOrigin.X,
										 northEastVertices[baseIndex].Y + (float)localOrigin.Y,
										 northEastVertices[baseIndex].Z + (float)localOrigin.Z);
					northEastVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;
				}
				//angle += deltaAngle;
			}

			baseIndex = 0;
			angleConst = 0.5f * (north + south) * Degrees2Radians;
			for (int i = 0; i < thisVertexCountPlus1; i++)
			{
				angle = angleConst + i * deltaAngle;
				double sinLat = Math.Sin(angle);
				double radCosLat = Math.Cos(angle) * layerRadius;

				for (int j = 0; j < thisVertexCountPlus1; j++)
				{
					southEastVertices[baseIndex].X = (float)(radCosLat * cosLon[j] - localOrigin.X);
					southEastVertices[baseIndex].Y = (float)(radCosLat * sinLon[j] - localOrigin.Y);
					southEastVertices[baseIndex].Z = (float)(layerRadius * sinLat - localOrigin.Z);
					southEastVertices[baseIndex].Tu = (float)((j + thisVertexCount) * scaleFactor);
					southEastVertices[baseIndex].Tv = (float)((i + thisVertexCount) * scaleFactor);
					southEastVertices[baseIndex].Normal =
						 new Vector3(southEastVertices[baseIndex].X + (float)localOrigin.X,
										 southEastVertices[baseIndex].Y + (float)localOrigin.Y,
										 southEastVertices[baseIndex].Z + (float)localOrigin.Z);
					southEastVertices[baseIndex].Normal.Normalize();

					baseIndex += 1;
				}
				//angle += deltaAngle;
			}

			vertexIndexes = new short[2 * thisVertexCount * thisVertexCount * 3];

			for (int i = 0; i < thisVertexCount; i++)
			{
				baseIndex = (2 * 3 * i * thisVertexCount);

				for (int j = 0; j < thisVertexCount; j++)
				{
					vertexIndexes[baseIndex] = (short)(i * thisVertexCountPlus1 + j);
					vertexIndexes[baseIndex + 1] = (short)((i + 1) * thisVertexCountPlus1 + j);
					vertexIndexes[baseIndex + 2] = (short)(i * thisVertexCountPlus1 + j + 1);

					vertexIndexes[baseIndex + 3] = (short)(i * thisVertexCountPlus1 + j + 1);
					vertexIndexes[baseIndex + 4] = (short)((i + 1) * thisVertexCountPlus1 + j);
					vertexIndexes[baseIndex + 5] = (short)((i + 1) * thisVertexCountPlus1 + j + 1);

					baseIndex += 6;
				}
			}
		}

		/// <summary>
		/// Build the elevated terrain mesh
		/// </summary>
		protected virtual void CreateElevatedMesh()
		{
			isDownloadingTerrain = true;
			// Get height data with one extra sample around the tile
			double degreePerSample = LatitudeSpan / vertexCountElevated;
			TerrainTile tile =
				 quadTileSet.World.TerrainAccessor.GetElevationArray(north + degreePerSample, south - degreePerSample,
																					  west - degreePerSample, east + degreePerSample,
																					  vertexCountElevated + 3);
			float[,] heightData = tile.ElevationData;

			int vertexCountElevatedPlus3 = vertexCountElevated / 2 + 3;
			int totalVertexCount = vertexCountElevatedPlus3 * vertexCountElevatedPlus3;
			northWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			southWestVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			northEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			southEastVertices = new CustomVertex.PositionNormalTextured[totalVertexCount];
			double layerRadius = (double)quadTileSet.LayerRadius;

			// Calculate mesh base radius (bottom vertices)
			// Find minimum elevation to account for possible bathymetry
			float minimumElevation = float.MaxValue;
			float maximumElevation = float.MinValue;
			foreach (float height in heightData)
			{
				if (height < minimumElevation)
					minimumElevation = height;
				if (height > maximumElevation)
					maximumElevation = height;
			}
			minimumElevation *= verticalExaggeration;
			maximumElevation *= verticalExaggeration;

			if (minimumElevation > maximumElevation)
			{
				// Compensate for negative vertical exaggeration
				minimumElevation = maximumElevation;
				maximumElevation = minimumElevation;
			}

			CreateElevatedMesh(ChildLocation.NorthWest, northWestVertices, heightData);
			CreateElevatedMesh(ChildLocation.SouthWest, southWestVertices, heightData);
			CreateElevatedMesh(ChildLocation.NorthEast, northEastVertices, heightData);
			CreateElevatedMesh(ChildLocation.SouthEast, southEastVertices, heightData);

			// --- Make bounding box slightly larger so the tile won't blink in and out near the edges ---
			BoundingBox = new BoundingBox(south - 1.0, north + 1.0, west - 1.0, east + 1.0, layerRadius, layerRadius + 10000.0 * this.verticalExaggeration);

			quadTileSet.IsDownloadingElevation = false;

			// Build common set of indexes for the 4 child meshes	
			int vertexCountElevatedPlus2 = vertexCountElevated / 2 + 2;
			vertexIndexes = new short[2 * vertexCountElevatedPlus2 * vertexCountElevatedPlus2 * 3];

			int elevated_idx = 0;
			for (int i = 0; i < vertexCountElevatedPlus2; i++)
			{
				for (int j = 0; j < vertexCountElevatedPlus2; j++)
				{
					vertexIndexes[elevated_idx++] = (short)(i * vertexCountElevatedPlus3 + j);
					vertexIndexes[elevated_idx++] = (short)((i + 1) * vertexCountElevatedPlus3 + j);
					vertexIndexes[elevated_idx++] = (short)(i * vertexCountElevatedPlus3 + j + 1);

					vertexIndexes[elevated_idx++] = (short)(i * vertexCountElevatedPlus3 + j + 1);
					vertexIndexes[elevated_idx++] = (short)((i + 1) * vertexCountElevatedPlus3 + j);
					vertexIndexes[elevated_idx++] = (short)((i + 1) * vertexCountElevatedPlus3 + j + 1);
				}
			}

			calculate_normals(ref northWestVertices, vertexIndexes);
			calculate_normals(ref southWestVertices, vertexIndexes);
			calculate_normals(ref northEastVertices, vertexIndexes);
			calculate_normals(ref southEastVertices, vertexIndexes);

			isDownloadingTerrain = false;
		}

		protected byte m_CurrentOpacity = 255;

		/// <summary>
		/// Create child tile terrain mesh
		/// Build the mesh with one extra vertice all around for proper normals calculations later on.
		/// Use the struts vertices to that effect. Struts are properly folded after normals calculations.
		/// </summary>
		protected void CreateElevatedMesh(ChildLocation corner, CustomVertex.PositionNormalTextured[] vertices, float[,] heightData)
		{
			// Figure out child lat/lon boundaries (radians)
			double _north = MathEngine.DegreesToRadians(north);
			double _west = MathEngine.DegreesToRadians(west);

			// Texture coordinate offsets
			float TuOffset = 0;
			float TvOffset = 0;

			switch (corner)
			{
				case ChildLocation.NorthWest:
					// defaults are all good
					break;
				case ChildLocation.NorthEast:
					_west = MathEngine.DegreesToRadians(0.5 * (west + east));
					TuOffset = 0.5f;
					break;
				case ChildLocation.SouthWest:
					_north = MathEngine.DegreesToRadians(0.5 * (north + south));
					TvOffset = 0.5f;
					break;
				case ChildLocation.SouthEast:
					_north = MathEngine.DegreesToRadians(0.5 * (north + south));
					_west = MathEngine.DegreesToRadians(0.5 * (west + east));
					TuOffset = 0.5f;
					TvOffset = 0.5f;
					break;
			}

			double latitudeRadianSpan = MathEngine.DegreesToRadians(LatitudeSpan);
			double longitudeRadianSpan = MathEngine.DegreesToRadians(LongitudeSpan);

			double layerRadius = (double)quadTileSet.LayerRadius;
			double scaleFactor = 1.0 / vertexCountElevated;
			int terrainLongitudeIndex = (int)(TuOffset * vertexCountElevated) + 1;
			int terrainLatitudeIndex = (int)(TvOffset * vertexCountElevated) + 1;

			int vertexCountElevatedPlus1 = vertexCountElevated / 2 + 1;

			double radius = 0;
			int vertexIndex = 0;
			for (int latitudeIndex = -1; latitudeIndex <= vertexCountElevatedPlus1; latitudeIndex++)
			{
				double latitudeFactor = latitudeIndex * scaleFactor;
				double latitude = _north - latitudeFactor * latitudeRadianSpan;

				// Cache trigonometric values
				double cosLat = Math.Cos(latitude);
				double sinLat = Math.Sin(latitude);

				for (int longitudeIndex = -1; longitudeIndex <= vertexCountElevatedPlus1; longitudeIndex++)
				{
					// Top of mesh for all (real terrain + struts)
					radius = layerRadius +
								heightData[terrainLatitudeIndex + latitudeIndex, terrainLongitudeIndex + longitudeIndex]
								* verticalExaggeration;

					double longitudeFactor = longitudeIndex * scaleFactor;

					// Texture coordinates
					vertices[vertexIndex].Tu = TuOffset + (float)longitudeFactor;
					vertices[vertexIndex].Tv = TvOffset + (float)latitudeFactor;

					// Convert from spherical (radians) to cartesian
					double longitude = _west + longitudeFactor * longitudeRadianSpan;
					double radCosLat = radius * cosLat;
					vertices[vertexIndex].X = (float)(radCosLat * Math.Cos(longitude) - localOrigin.X);
					vertices[vertexIndex].Y = (float)(radCosLat * Math.Sin(longitude) - localOrigin.Y);
					vertices[vertexIndex].Z = (float)(radius * sinLat - localOrigin.Z);

					vertexIndex++;
				}
			}
		}

		private void calculate_normals(ref CustomVertex.PositionNormalTextured[] vertices, short[] indices)
		{
			ArrayList[] normal_buffer = new ArrayList[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				normal_buffer[i] = new ArrayList();
			}
			for (int i = 0; i < indices.Length; i += 3)
			{
				Vector3 p1 = vertices[indices[i + 0]].Position;
				Vector3 p2 = vertices[indices[i + 1]].Position;
				Vector3 p3 = vertices[indices[i + 2]].Position;

				Vector3 v1 = p2 - p1;
				Vector3 v2 = p3 - p1;
				Vector3 normal = Vector3.Cross(v1, v2);

				normal.Normalize();

				// Store the face's normal for each of the vertices that make up the face.
				normal_buffer[indices[i + 0]].Add(normal);
				normal_buffer[indices[i + 1]].Add(normal);
				normal_buffer[indices[i + 2]].Add(normal);
			}

			// Now loop through each vertex vector, and avarage out all the normals stored.
			for (int i = 0; i < vertices.Length; ++i)
			{
				for (int j = 0; j < normal_buffer[i].Count; ++j)
				{
					Vector3 curNormal = (Vector3)normal_buffer[i][j];

					if (vertices[i].Normal == Vector3.Empty)
						vertices[i].Normal = curNormal;
					else
						vertices[i].Normal += curNormal;
				}

				vertices[i].Normal.Multiply(1.0f / normal_buffer[i].Count);
			}

			// Adjust/Fold struts vertices using terrain border vertices positions
			short vertexDensity = (short)Math.Sqrt(vertices.Length);
			for (int i = 0; i < vertexDensity; i++)
			{
				if (i == 0 || i == vertexDensity - 1)
				{
					for (int j = 0; j < vertexDensity; j++)
					{
						int offset = (i == 0) ? vertexDensity : -vertexDensity;
						if (j == 0) offset++;
						if (j == vertexDensity - 1) offset--;
						Point3d p =
							 new Point3d(vertices[i * vertexDensity + j + offset].Position.X,
											 vertices[i * vertexDensity + j + offset].Position.Y,
											 vertices[i * vertexDensity + j + offset].Position.Z);
						vertices[i * vertexDensity + j].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
					}
				}
				else
				{
					Point3d p =
						 new Point3d(vertices[i * vertexDensity + 1].Position.X, vertices[i * vertexDensity + 1].Position.Y,
										 vertices[i * vertexDensity + 1].Position.Z);
					vertices[i * vertexDensity].Position = new Vector3((float)p.X, (float)p.Y, (float)p.Z);

					p =
						 new Point3d(vertices[i * vertexDensity + vertexDensity - 2].Position.X,
										 vertices[i * vertexDensity + vertexDensity - 2].Position.Y,
										 vertices[i * vertexDensity + vertexDensity - 2].Position.Z);
					vertices[i * vertexDensity + vertexDensity - 1].Position =
						 new Vector3((float)p.X, (float)p.Y, (float)p.Z);
				}
			}
		}

		// End edits by Patrick Murris : fixing mesh sides normals (2006-11-18)


		private string imageFilePath = null;
		private int textureSizePixels = -1;

		internal virtual bool Render(DrawArgs drawArgs)
		{
			m_CurrentOpacity = quadTileSet.Opacity;
			try
			{
				if (!isInitialized ||
					 this.northWestVertices == null ||
					 this.northEastVertices == null ||
					 this.southEastVertices == null ||
					 this.southWestVertices == null)
					return false;

				if (!DrawArgs.Camera.ViewFrustum.Intersects(BoundingBox))
					return false;

				bool northWestChildRendered = false;
				bool northEastChildRendered = false;
				bool southWestChildRendered = false;
				bool southEastChildRendered = false;

				if (northWestChild != null)
					if (northWestChild.Render(drawArgs))
						northWestChildRendered = true;

				if (southWestChild != null)
					if (southWestChild.Render(drawArgs))
						southWestChildRendered = true;

				if (northEastChild != null)
					if (northEastChild.Render(drawArgs))
						northEastChildRendered = true;

				if (southEastChild != null)
					if (southEastChild.Render(drawArgs))
						southEastChildRendered = true;

				if (northWestChildRendered && northEastChildRendered && southWestChildRendered && southEastChildRendered)
				{
					return true;
				}

				Device device = DrawArgs.Device;

				for (int i = 0; i < textures.Length; i++)
				{
					if (textures[i] == null || textures[i].Disposed)
						return false;

					device.SetTexture(i, textures[i]);
				}

				drawArgs.numberTilesDrawn++;

				int numpasses = 1;
				int pass;

				DrawArgs.Device.Transform.World = Matrix.Translation(
					 (float)(localOrigin.X - drawArgs.WorldCamera.ReferenceCenter.X),
					 (float)(localOrigin.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
					 (float)(localOrigin.Z - drawArgs.WorldCamera.ReferenceCenter.Z)
					 );

				for (pass = 0; pass < numpasses; pass++)
				{
					if (!northWestChildRendered)
						Render(device, northWestVertices, northWestChild);
					if (!southWestChildRendered)
						Render(device, southWestVertices, southWestChild);
					if (!northEastChildRendered)
						Render(device, northEastVertices, northEastChild);
					if (!southEastChildRendered)
						Render(device, southEastVertices, southEastChild);
				}

				DrawArgs.Device.Transform.World = ConvertDX.FromMatrix4d(DrawArgs.Camera.WorldMatrix);

				return true;
			}
			catch (DirectXException)
			{
			}
			return false;
		}

		/// <summary>
		/// Render one of the 4 quadrants with optional download indicator
		/// </summary>
		private void Render(Device device, CustomVertex.PositionNormalTextured[] verts, QuadTile child)
		{
			bool isMultitexturing = false;

			if (!World.Settings.EnableSunShading)
			{
				if (World.Settings.ShowDownloadIndicator && child != null)
				{
					// Check/display download activity
					//GeoSpatialDownloadRequest request = child.DownloadRequest;
					if (child.isDownloadingTerrain)
					{
						device.SetTexture(1, QuadTileSet.DownloadTerrainTexture);
						isMultitexturing = true;
					}
					//else if (request != null)
					else if (child.WaitingForDownload)
					{
						if (child.IsDownloadingImage)
							device.SetTexture(1, QuadTileSet.DownloadInProgressTexture);
						else
							device.SetTexture(1, QuadTileSet.DownloadQueuedTexture);
						isMultitexturing = true;
					}
				}
			}

			if (isMultitexturing)
				device.SetTextureStageState(1, TextureStageStates.ColorOperation,
													 (int)TextureOperation.BlendTextureAlpha);

			if (verts != null && vertexIndexes != null)
			{
				if (quadTileSet.Effect != null)
				{
					Effect effect = quadTileSet.Effect;

					int tc1 = device.GetTextureStageStateInt32(1, TextureStageStates.TextureCoordinateIndex);
					device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, 1);


					// FIXME: just use the first technique for now
					effect.Technique = effect.GetTechnique(0);
					EffectHandle param;
					param = (EffectHandle)quadTileSet.EffectParameters["WorldViewProj"];
					if (param != null)
						effect.SetValue(param,
											 Matrix.Multiply(device.Transform.World,
																  Matrix.Multiply(device.Transform.View,
																						device.Transform.Projection)));
					try
					{
						param = (EffectHandle)quadTileSet.EffectParameters["World"];
						if (param != null)
							effect.SetValue(param, device.Transform.World);
						param = (EffectHandle)quadTileSet.EffectParameters["ViewInverse"];
						if (param != null)
						{
							Matrix viewInv = Matrix.Invert(device.Transform.View);
							effect.SetValue(param, viewInv);
						}

						// set textures as available
						for (int i = 0; i < textures.Length; i++)
						{
							string name = string.Format("Tex{0}", i);
							param = (EffectHandle)quadTileSet.EffectParameters[name];
							if (param != null)
							{
								effect.SetValue(param, textures[i]);
							}
						}

						// brightness & opacity values
						param = (EffectHandle)quadTileSet.EffectParameters["Brightness"];
						if (param != null)
							effect.SetValue(param, quadTileSet.GrayscaleBrightness);

						param = (EffectHandle)quadTileSet.EffectParameters["Opacity"];
						if (param != null)
						{
							float opacity = (float)quadTileSet.Opacity / 255.0f;
							effect.SetValue(param, opacity);
						}

						param = (EffectHandle)quadTileSet.EffectParameters["LayerRadius"];
						if (param != null)
						{
							effect.SetValue(param, (float)quadTileSet.LayerRadius);
						}

						param = (EffectHandle)quadTileSet.EffectParameters["TileLevel"];
						if (param != null)
						{
							effect.SetValue(param, level);
						}

						param = (EffectHandle)quadTileSet.EffectParameters["LocalOrigin"];
						if (param != null)
						{
							effect.SetValue(param, localOrigin.Vector4);
						}

						// sun position
						param = (EffectHandle)quadTileSet.EffectParameters["LightDirection"];
						if (param != null)
						{
							Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
							sunPosition.normalize();
							Vector4 sunVector = new Vector4(
								 (float)sunPosition.X,
								 (float)sunPosition.Y,
								 (float)sunPosition.Z,
								 0.0f);
							effect.SetValue(param, sunVector);
						}

						// local origin
						param = (EffectHandle)quadTileSet.EffectParameters["LocalFrameOrigin"];
						if (param != null)
						{
							//localOrigin = BoundingBox.CalculateCenter();
							Point3d centerPoint =
								 MathEngine.SphericalToCartesian(centerLatitude, centerLongitude,
																			 quadTileSet.LayerRadius);
							Point3d northHalf =
								 MathEngine.SphericalToCartesian(Angle.FromDegrees(north), centerLongitude,
																			 quadTileSet.LayerRadius);
							Point3d eastHalf =
								 MathEngine.SphericalToCartesian(centerLatitude, Angle.FromDegrees(east),
																			 quadTileSet.LayerRadius);

							Vector4 xdir = (2 * (eastHalf - centerPoint)).Vector4;
							Vector4 ydir = (2 * (northHalf - centerPoint)).Vector4;
							// up vector is radius at center point, normalized
							Point3d zdir3 = centerPoint;
							zdir3.normalize();
							Vector4 zdir = zdir3.Vector4;
							// local frame origin at SW corner, relative to local origin
							Point3d localFrameOrigin = northHalf + eastHalf - centerPoint - localOrigin;
							Vector4 lfoW = localFrameOrigin.Vector4;
							lfoW.W = 1;
							lfoW.Transform(device.Transform.World);
							effect.SetValue(param, localFrameOrigin.Vector4); // JBTODO: Should this be lfoW?

							param = (EffectHandle)quadTileSet.EffectParameters["LocalFrameXAxis"];
							if (param != null) effect.SetValue(param, xdir);
							param = (EffectHandle)quadTileSet.EffectParameters["LocalFrameYAxis"];
							if (param != null) effect.SetValue(param, ydir);
							param = (EffectHandle)quadTileSet.EffectParameters["LocalFrameZAxis"];
							if (param != null) effect.SetValue(param, zdir);
						}
					}
					catch
					{
					}

					int numPasses = effect.Begin(0);
					for (int i = 0; i < numPasses; i++)
					{
						effect.BeginPass(i);
						device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
																	verts.Length, vertexIndexes.Length / 3, vertexIndexes, true,
																	verts);

						effect.EndPass();
					}

					effect.End();
					device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, tc1);
				}
				else
				{
					if (World.Settings.EnableSunShading)
					{
						Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
						Vector3 sunVector = new Vector3(
							 (float)sunPosition.X,
							 (float)sunPosition.Y,
							 (float)sunPosition.Z);

						device.RenderState.Lighting = true;
						Material material = new Material();
						material.Diffuse = Color.White;
						material.Ambient = Color.White;

						device.Material = material;
						device.RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
						device.RenderState.NormalizeNormals = true;
						device.RenderState.AlphaBlendEnable = true;

						device.Lights[0].Enabled = true;
						device.Lights[0].Type = LightType.Directional;
						device.Lights[0].Diffuse = LightColor;
						device.Lights[0].Direction = sunVector;

						device.TextureState[0].ColorOperation = TextureOperation.Modulate;
						device.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
						device.TextureState[0].ColorArgument2 = TextureArgument.TextureColor;
					}
					else
					{
						device.RenderState.Lighting = false;
						device.RenderState.Ambient = World.Settings.StandardAmbientColor;
					}

					device.RenderState.TextureFactor = Color.FromArgb(m_CurrentOpacity, 255, 255, 255).ToArgb();
					device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
					device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
					device.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;

					device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
																verts.Length, vertexIndexes.Length / 3, vertexIndexes, true, verts);

               device.RenderState.TextureFactor = Color.FromArgb(255, 255, 255, 255).ToArgb();
				}
			}
			if (isMultitexturing)
				device.SetTextureStageState(1, TextureStageStates.ColorOperation, (int)TextureOperation.Disable);
		}

		internal void InitExportInfo(DrawArgs drawArgs, RenderableObject.ExportInfo info)
		{
			if (isInitialized)
			{
				info.dMaxLat = Math.Max(info.dMaxLat, this.north);
				info.dMinLat = Math.Min(info.dMinLat, this.south);
				info.dMaxLon = Math.Max(info.dMaxLon, this.east);
				info.dMinLon = Math.Min(info.dMinLon, this.west);

				info.iPixelsY = Math.Max(info.iPixelsY, (int)Math.Round((info.dMaxLat - info.dMinLat) / (this.north - this.south)) * textureSizePixels);
				info.iPixelsX = Math.Max(info.iPixelsX, (int)Math.Round((info.dMaxLon - info.dMinLon) / (this.east - this.west)) * textureSizePixels);
			}

			if (northWestChild != null && northWestChild.isInitialized)
				northWestChild.InitExportInfo(drawArgs, info);
			if (northEastChild != null && northEastChild.isInitialized)
				northEastChild.InitExportInfo(drawArgs, info);
			if (southWestChild != null && southWestChild.isInitialized)
				southWestChild.InitExportInfo(drawArgs, info);
			if (southEastChild != null && southEastChild.isInitialized)
				southEastChild.InitExportInfo(drawArgs, info);
		}

		internal void ExportProcess(DrawArgs drawArgs, RenderableObject.ExportInfo expInfo)
		{
			try
			{
				bool bChildren = false;

				if (!isInitialized || textures == null || textures[0] == null)
					return;
				if (!drawArgs.WorldCamera.ViewFrustum.Intersects(BoundingBox))
					return;

				if (northWestChild != null && northWestChild.isInitialized)
				{
					northWestChild.ExportProcess(drawArgs, expInfo);
					bChildren = true;
				}

				if (northEastChild != null && northEastChild.isInitialized)
				{
					northEastChild.ExportProcess(drawArgs, expInfo);
					bChildren = true;
				}

				if (southWestChild != null && southWestChild.isInitialized)
				{
					southWestChild.ExportProcess(drawArgs, expInfo);
					bChildren = true;
				}

				if (southEastChild != null && southEastChild.isInitialized)
				{
					southEastChild.ExportProcess(drawArgs, expInfo);
					bChildren = true;
				}

				if (!bChildren && this.textures != null && this.textures[0] != null)
				{
					Image img = null;

					try
					{
						int iWidth, iHeight, iX, iY;

						img = Image.FromFile(imageFilePath);

						iWidth = (int)Math.Ceiling((this.east - this.west) * (double)expInfo.iPixelsX / (expInfo.dMaxLon - expInfo.dMinLon));
						iHeight = (int)Math.Ceiling((this.north - this.south) * (double)expInfo.iPixelsY / (expInfo.dMaxLat - expInfo.dMinLat));
						iX = (int)Math.Floor((this.west - expInfo.dMinLon) * (double)expInfo.iPixelsX / (expInfo.dMaxLon - expInfo.dMinLon));
						iY = (int)Math.Floor((expInfo.dMaxLat - this.north) * (double)expInfo.iPixelsY / (expInfo.dMaxLat - expInfo.dMinLat));
						System.Drawing.Imaging.ImageAttributes oAttrs = new System.Drawing.Imaging.ImageAttributes();
						oAttrs.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY); // This "eliminates" gaps between images
						expInfo.gr.DrawImage(img, new Rectangle(iX, iY, iWidth, iHeight), 0.0f, 0.0f, (float)img.Width, (float)img.Height, GraphicsUnit.Pixel, oAttrs);
					}
					catch
					{
#if DEBUG
						System.Diagnostics.Debug.WriteLine("Thrown in image export");
#endif
					}
					finally
					{
						if (img != null)
							img.Dispose();
					}
				}
			}
			catch
			{
			}
		}

		#region IGeoSpatialDownloadTile Implementation


		public Angle CenterLatitude
		{
			get
			{
				return centerLatitude;
			}
		}

		public Angle CenterLongitude
		{
			get
			{
				return centerLongitude;
			}
		}

		public int Level
		{
			get
			{
				return level;
			}
		}

		public int Row
		{
			get
			{
				return row;
			}
		}

		public int Col
		{
			get
			{
				return col;
			}
		}

		/// <summary>
		/// North bound for this Tile
		/// </summary>
		public double North
		{
			get
			{
				return north;
			}
		}

		/// <summary>
		/// West bound for this Tile
		/// </summary>
		public double West
		{
			get
			{
				return west;
			}
		}

		/// <summary>
		/// South bound for this Tile
		/// </summary>
		public double South
		{
			get
			{
				return south;
			}
		}

		/// <summary>
		/// East bound for this Tile
		/// </summary>
		public double East
		{
			get
			{
				return east;
			}
		}

		public int TextureSizePixels
		{
			get
			{
				return textureSizePixels;
			}
			set
			{
				textureSizePixels = value;
			}
		}

		public IGeoSpatialDownloadTileSet TileSet
		{
			get
			{
				return quadTileSet;
			}
		}

		public List<GeoSpatialDownloadRequest> DownloadRequests
		{
			get
			{
				return downloadRequests;
			}
		}

		public string ImageFilePath
		{
			get
			{
				return imageFilePath;
			}
			set
			{
				imageFilePath = value;
			}
		}

		public bool IsDownloadingImage
		{
			get
			{
				return isDownloadingImage;
			}
			set
			{
				isDownloadingImage = value;
			}
		}

		public bool WaitingForDownload
		{
			get
			{
				return waitingForDownload;
			}
			set
			{
				waitingForDownload = value;
			}

		}

		#endregion
	}
}
