using System;
using System.Threading;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.IO;
using System.Diagnostics;
using WorldWind.Net;
using WorldWind.Terrain;
using WorldWind.Menu;
using WorldWind.VisualControl;
using Utility;

namespace WorldWind.Renderable
{
	/// <summary>
	/// Use this class to map a single image to the planet at a desired altitude.
	/// Source image must be in Plate Carree (geographic) map projection:
	/// http://en.wikipedia.org/wiki/Plate_Carr%E9e_Projection
	/// TODO: Update this code to take advantage of the Texture Manager
	/// </summary>
	public class ImageLayer : RenderableObject
	{
		#region Private Members

		protected double layerRadius;
		protected double minLat;
		protected double maxLat;
		protected double minLon;
		protected double maxLon;
		Stream m_TextureStream = null;

		protected bool _disableZbuffer;
		protected CustomVertex.PositionNormalTextured[] vertices;
		protected static CustomVertex.TransformedColored[] progressBarOutline = new CustomVertex.TransformedColored[5];
		protected static CustomVertex.TransformedColored[] progressBar = new CustomVertex.TransformedColored[4];
		protected short[] indices;

		protected Texture texture;
		protected Device device;
		protected string _imageUrl;
		protected string _imagePath;

		// The triangular mesh density for the rendered sector
		protected int meshPointCount = 64;
		protected TerrainAccessor _terrainAccessor;

		protected int progressBarBackColor = System.Drawing.Color.FromArgb(100, 255, 255, 255).ToArgb();
		protected int progressBarOutlineColor = System.Drawing.Color.SlateGray.ToArgb();
		protected int textColor = System.Drawing.Color.Black.ToArgb();

		protected float downloadPercent;
		protected Thread downloadThread;
		protected float verticalExaggeration;
		protected string m_legendImagePath;

		#endregion

		TimeSpan cacheExpiration = TimeSpan.MaxValue;
		System.Timers.Timer refreshTimer = null;
		#region Properties

		/// <summary>
		/// The url of the image (when image is on network)
		/// </summary>
		public string ImageUrl
		{
			get
			{
				return this._imageUrl;
			}
			set
			{
				this._imageUrl = value;
			}
		}

		/// <summary>
		/// The Path of the image (local file)
		/// </summary>
		public string ImagePath
		{
			get
			{
				return this._imagePath;
			}
			set
			{
				if (value != null)
					value = value.Trim();
				if (value == null || value.Trim().Length <= 0)
					_imagePath = null;
				else
					_imagePath = value.Trim();
			}
		}

		/// <summary>
		/// Longitude at left edge of image
		/// </summary>
		public double MinLon
		{
			get
			{
				return this.minLon;
			}
			set
			{
				this.minLon = value;
			}
		}

		/// <summary>
		/// Latitude at lower edge of image
		/// </summary>
		public double MinLat
		{
			get
			{
				return this.minLat;
			}
			set
			{
				this.minLat = value;
			}
		}

		/// <summary>
		/// Longitude at upper edge of image
		/// </summary>
		public double MaxLon
		{
			get
			{
				return this.maxLon;
			}
			set
			{
				this.maxLon = value;
			}
		}

		/// <summary>
		/// Latitude at upper edge of image
		/// </summary>
		public double MaxLat
		{
			get
			{
				return this.maxLat;
			}
			set
			{
				this.maxLat = value;
			}
		}

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public ImageLayer(
			string name,
			World parentWorld,
			double distanceAboveSurface,
			string imagePath,
			double minLatitude,
			double maxLatitude,
			double minLongitude,
			double maxLongitude,
			byte opacity,
			TerrainAccessor terrainAccessor)
			: base(name, parentWorld.Position, parentWorld.Orientation)
		{
			this.layerRadius = (float)parentWorld.EquatorialRadius + distanceAboveSurface;
			this._imagePath = imagePath;
			minLat = minLatitude;
			maxLat = maxLatitude;
			minLon = minLongitude;
			maxLon = maxLongitude;
			this.m_opacity = opacity;
			this._terrainAccessor = terrainAccessor;
			this._imagePath = imagePath;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		internal ImageLayer(string name, World parentWorld, double distanceAboveSurface, Stream textureStream, double minLatitude, double maxLatitude, double minLongitude, double maxLongitude, double opacityPercent, TerrainAccessor terrainAccessor)
			: this(name, parentWorld, distanceAboveSurface, null,
			minLatitude, maxLatitude, minLongitude, maxLongitude,
			(byte)(255 * opacityPercent), terrainAccessor)
		{
			m_TextureStream = textureStream;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ImageLayer(
			string name,
			World parentWorld,
			double distanceAboveSurface,
			string imagePath,
			double minLatitude,
			double maxLatitude,
			double minLongitude,
			double maxLongitude,
			double opacityPercent,
			TerrainAccessor terrainAccessor)
			: this(name, parentWorld, distanceAboveSurface, imagePath,
			minLatitude, maxLatitude, minLongitude, maxLongitude,
			(byte)(255 * opacityPercent), terrainAccessor)
		{
		}

		/// <summary>
		/// Layer initialization code
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				this.device = drawArgs.device;

				if (_imagePath == null && _imageUrl != null && _imageUrl.ToLower().StartsWith("http://"))
				{
					_imagePath = getFilePathFromUrl(_imageUrl);

				}

				FileInfo imageFileInfo = null;
				if (_imagePath != null)
					imageFileInfo = new FileInfo(_imagePath);

				if (downloadThread != null && downloadThread.IsAlive)
					return;

				if (_imagePath != null &&
					cacheExpiration != TimeSpan.MaxValue &&
					cacheExpiration.TotalMilliseconds > 0 &&
					_imageUrl.ToLower().StartsWith("http://") &&
					imageFileInfo != null &&
					imageFileInfo.Exists &&
					imageFileInfo.LastWriteTime < System.DateTime.Now - cacheExpiration)
				{
					//attempt to redownload it
					downloadThread = new Thread(new ThreadStart(DownloadImage));
					downloadThread.Name = ThreadNames.ImageLayerDownload;

					downloadThread.IsBackground = true;
					downloadThread.Start();

					return;
				}

				if (m_TextureStream != null)
				{
					UpdateTexture(m_TextureStream);
					verticalExaggeration = World.Settings.VerticalExaggeration;
					CreateMesh();
					isInitialized = true;
					return;
				}
				else if (imageFileInfo != null && imageFileInfo.Exists)
				{
					UpdateTexture(_imagePath);
					verticalExaggeration = World.Settings.VerticalExaggeration;
					CreateMesh();
					isInitialized = true;
					return;
				}

				if (_imageUrl != null && _imageUrl.ToLower().StartsWith("http://"))
				{
					//download it...
					downloadThread = new Thread(new ThreadStart(DownloadImage));
					downloadThread.Name = ThreadNames.ImageLayerDownload;

					downloadThread.IsBackground = true;
					downloadThread.Start();

					return;
				}

				// No image available
				Dispose();
				isOn = false;
				return;
			}
			catch
			{
			}
		}

		/// <summary>
		/// Downloads image from web
		/// </summary>
		protected virtual void DownloadImage()
		{
			try
			{
				if (_imagePath != null)
					Directory.CreateDirectory(Path.GetDirectoryName(this._imagePath));

				using (WebDownload downloadReq = new WebDownload(this._imageUrl))
				{
					downloadReq.ProgressCallback += new DownloadProgressHandler(UpdateDownloadProgress);

					if (_imagePath == null)
					{
						// Download to RAM
						downloadReq.DownloadMemory();
						texture = ImageHelper.LoadTexture(downloadReq.ContentStream);
					}
					else
					{
						downloadReq.DownloadFile(_imagePath);
						UpdateTexture(_imagePath);
					}
					CreateMesh();
					isInitialized = true;
				}
			}
			catch (ThreadAbortException)
			{ }
			catch (Exception caught)
			{
				if (!showedError)
				{
					string msg = string.Format("Image download of file\n\n{1}\n\nfor layer '{0}' failed:\n\n{2}",
						name, _imageUrl, caught.Message);
					System.Windows.Forms.MessageBox.Show(msg, "Image download failed.",
						System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Error);
					showedError = true;
				}

				if (_imagePath != null)
				{
					FileInfo imageFile = new FileInfo(_imagePath);
					if (imageFile.Exists)
					{
						UpdateTexture(_imagePath);
						CreateMesh();
						isInitialized = true;
					}
				}
				else
				{
					isOn = false;
				}
			}
		}

		bool showedError;

		/// <summary>
		/// Download progress callback 
		/// </summary>
		protected void UpdateDownloadProgress(int bytesRead, int bytesTotal)
		{
			if (bytesRead < bytesTotal)
				downloadPercent = (float)bytesRead / bytesTotal;
		}

		/// <summary>
		/// Update layer (called from worker thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
			try
			{
				if (!this.isInitialized)
				{
					this.Initialize(drawArgs);
					if (!this.isInitialized)
						return;
				}

				if (isInitialized && Math.Abs(this.verticalExaggeration - World.Settings.VerticalExaggeration) > 0.01f)
				{
					// Vertical exaggeration changed - rebuild mesh
					this.verticalExaggeration = World.Settings.VerticalExaggeration;
					this.isInitialized = false;
					CreateMesh();
					this.isInitialized = true;
				}

				if (cacheExpiration != TimeSpan.MaxValue && cacheExpiration.TotalMilliseconds > 0)
				{
					if (refreshTimer == null)
					{
						refreshTimer = new System.Timers.Timer(cacheExpiration.TotalMilliseconds);
						refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(refreshTimer_Elapsed);
						refreshTimer.Start();
					}

					if (refreshTimer.Interval != cacheExpiration.TotalMilliseconds)
					{
						refreshTimer.Interval = cacheExpiration.TotalMilliseconds;
					}
				}
				else if (refreshTimer != null && refreshTimer.Enabled)
					refreshTimer.Stop();
			}
			catch
			{
			}
		}

		public override byte Opacity
		{
			get
			{
				return m_opacity;
			}
			set
			{
				m_opacity = value;
			}
		}

		/// <summary>
		/// Builds the image's mesh 
		/// </summary>
		protected virtual void CreateMesh()
		{
			int upperBound = meshPointCount - 1;
			float scaleFactor = (float)1 / upperBound;
			double latrange = Math.Abs(maxLat - minLat);
			double lonrange;
			if (minLon < maxLon)
				lonrange = maxLon - minLon;
			else
				lonrange = 360.0f + maxLon - minLon;

			vertices = new CustomVertex.PositionNormalTextured[meshPointCount * meshPointCount];
			for (int i = 0; i < meshPointCount; i++)
			{
				for (int j = 0; j < meshPointCount; j++)
				{
					double height = 0;
					if (this._terrainAccessor != null)
						height = this.verticalExaggeration * this._terrainAccessor.GetElevationAt(
							(double)maxLat - scaleFactor * latrange * i,
							(double)minLon + scaleFactor * lonrange * j,
							(double)upperBound / latrange);

					Point3d pos = MathEngine.SphericalToCartesian(
						maxLat - scaleFactor * latrange * i,
						minLon + scaleFactor * lonrange * j,
						layerRadius + height);

					vertices[i * meshPointCount + j].X = (float)pos.X;
					vertices[i * meshPointCount + j].Y = (float)pos.Y;
					vertices[i * meshPointCount + j].Z = (float)pos.Z;

					vertices[i * meshPointCount + j].Tu = j * scaleFactor;
					vertices[i * meshPointCount + j].Tv = i * scaleFactor;
					//	vertices[i*meshPointCount + j].Color = opacityColor;
				}
			}

			indices = new short[2 * upperBound * upperBound * 3];
			for (int i = 0; i < upperBound; i++)
			{
				for (int j = 0; j < upperBound; j++)
				{
					indices[(2 * 3 * i * upperBound) + 6 * j] = (short)(i * meshPointCount + j);
					indices[(2 * 3 * i * upperBound) + 6 * j + 1] = (short)((i + 1) * meshPointCount + j);
					indices[(2 * 3 * i * upperBound) + 6 * j + 2] = (short)(i * meshPointCount + j + 1);

					indices[(2 * 3 * i * upperBound) + 6 * j + 3] = (short)(i * meshPointCount + j + 1);
					indices[(2 * 3 * i * upperBound) + 6 * j + 4] = (short)((i + 1) * meshPointCount + j);
					indices[(2 * 3 * i * upperBound) + 6 * j + 5] = (short)((i + 1) * meshPointCount + j + 1);
				}
			}

			calculate_normals(ref vertices, indices);
		}

		private void calculate_normals(ref CustomVertex.PositionNormalTextured[] vertices, short[] indices)
		{
			System.Collections.ArrayList[] normal_buffer = new System.Collections.ArrayList[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				normal_buffer[i] = new System.Collections.ArrayList();
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
		}

		/// <summary>
		/// Draws the layer
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			if (downloadThread != null && downloadThread.IsAlive)
			{
				RenderProgress(drawArgs);
			}

			if (!this.isInitialized)
				return;

			try
			{

				if (texture == null)
					return;

				drawArgs.device.SetTexture(0, this.texture);

				if (this._disableZbuffer)
				{
					if (drawArgs.device.RenderState.ZBufferEnable)
						drawArgs.device.RenderState.ZBufferEnable = false;
				}
				else
				{
					if (!drawArgs.device.RenderState.ZBufferEnable)
						drawArgs.device.RenderState.ZBufferEnable = true;
				}

				drawArgs.device.RenderState.ZBufferEnable = true;
				drawArgs.device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);

				drawArgs.device.Transform.World = Matrix.Translation(
						  (float)-drawArgs.WorldCamera.ReferenceCenter.X,
						  (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
						  (float)-drawArgs.WorldCamera.ReferenceCenter.Z
						  );
				device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

				if (World.Settings.EnableSunShading)
				{
					Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
					Vector3 sunVector = new Vector3(
						 (float)sunPosition.X,
						 (float)sunPosition.Y,
						 (float)sunPosition.Z);

					device.RenderState.Lighting = true;
					Material material = new Material();
					material.Diffuse = System.Drawing.Color.White;
					material.Ambient = System.Drawing.Color.White;

					device.Material = material;
					device.RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
					device.RenderState.NormalizeNormals = true;
					device.RenderState.AlphaBlendEnable = true;

					device.Lights[0].Enabled = true;
					device.Lights[0].Type = LightType.Directional;
					device.Lights[0].Diffuse = System.Drawing.Color.White;
					device.Lights[0].Direction = sunVector;

					device.TextureState[0].ColorOperation = TextureOperation.Modulate;
					device.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
					device.TextureState[0].ColorArgument2 = TextureArgument.TextureColor;
				}
				else
				{
					device.RenderState.Lighting = false;
					device.RenderState.Ambient = World.Settings.StandardAmbientColor;

					drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
					drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
				}

				device.RenderState.TextureFactor = System.Drawing.Color.FromArgb(m_opacity, 255, 255, 255).ToArgb();
				device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
				device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
				device.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;

				drawArgs.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

				drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
					 vertices.Length, indices.Length / 3, indices, true, vertices);

				device.RenderState.TextureFactor = System.Drawing.Color.FromArgb(255, 255, 255, 255).ToArgb();

				drawArgs.device.Transform.World = ConvertDX.FromMatrix4d(drawArgs.WorldCamera.WorldMatrix);

			}
			finally
			{
				if (m_opacity < 255)
				{
					// Restore alpha blend state
					device.RenderState.SourceBlend = Blend.SourceAlpha;
					device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
				}

				if (this._disableZbuffer)
					drawArgs.device.RenderState.ZBufferEnable = true;
			}
		}

		protected virtual void RenderProgress(DrawArgs drawArgs)
		{

			drawArgs.device.Transform.World = Matrix.Translation(
				(float)-drawArgs.WorldCamera.ReferenceCenter.X,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Y,
				(float)-drawArgs.WorldCamera.ReferenceCenter.Z
			);

			device.RenderState.ZBufferEnable = false;
			double centerLat = 0.5 * (maxLat + minLat);
			double centerLon = 0.5 * (maxLon + minLon);

			Point3d v = MathEngine.SphericalToCartesian(centerLat, centerLon, this.layerRadius);

			if (drawArgs.WorldCamera.ViewFrustum.ContainsPoint(v) &&
				MathEngine.SphericalDistanceDegrees(centerLat, centerLon, drawArgs.WorldCamera.Latitude.Degrees, drawArgs.WorldCamera.Longitude.Degrees) < 2 * drawArgs.WorldCamera.ViewRange.Degrees
				)
			{
				v.Project(drawArgs.device.Viewport, ConvertDX.ToMatrix4d(drawArgs.device.Transform.Projection), ConvertDX.ToMatrix4d(drawArgs.device.Transform.View), ConvertDX.ToMatrix4d(drawArgs.device.Transform.World));

				MenuUtils.DrawBox((int)v.X, (int)v.Y, 200, 40, 0.0f, progressBarBackColor, drawArgs.device);
				Vector2[] boxOutline = new Vector2[5];
				boxOutline[0].X = (int)v.X;
				boxOutline[0].Y = (int)v.Y;

				boxOutline[1].X = (int)v.X + 200;
				boxOutline[1].Y = (int)v.Y;

				boxOutline[2].X = (int)v.X + 200;
				boxOutline[2].Y = (int)v.Y + 40;

				boxOutline[3].X = (int)v.X;
				boxOutline[3].Y = (int)v.Y + 40;

				boxOutline[4].X = (int)v.X;
				boxOutline[4].Y = (int)v.Y;

				MenuUtils.DrawLine(boxOutline, progressBarOutlineColor, drawArgs.device);

				drawArgs.defaultDrawingFont.DrawText(null,
					"Downloading Remote Image...",
					new System.Drawing.Rectangle((int)v.X + 5, (int)v.Y + 5, 200, 50),
					DrawTextFormat.NoClip, textColor);

				DrawProgressBar(drawArgs, (float)v.X + 100, (float)v.Y + 30, 180, 10, World.Settings.downloadProgressColor);
			}
			device.RenderState.ZBufferEnable = true;
			drawArgs.device.Transform.World = ConvertDX.FromMatrix4d(drawArgs.WorldCamera.WorldMatrix);
		}

		/// <summary>
		/// Displays a progress bar
		/// </summary>
		void DrawProgressBar(DrawArgs drawArgs, float x, float y, float width, float height, int color)
		{
			float halfWidth = width / 2;
			float halfHeight = height / 2;
			progressBarOutline[0].X = x - halfWidth;
			progressBarOutline[0].Y = y - halfHeight;
			progressBarOutline[0].Z = 0.5f;
			progressBarOutline[0].Color = color;

			progressBarOutline[1].X = x + halfWidth;
			progressBarOutline[1].Y = y - halfHeight;
			progressBarOutline[1].Z = 0.5f;
			progressBarOutline[1].Color = color;

			progressBarOutline[2].X = x + halfWidth;
			progressBarOutline[2].Y = y + halfHeight;
			progressBarOutline[2].Z = 0.5f;
			progressBarOutline[2].Color = color;

			progressBarOutline[3].X = x - halfWidth;
			progressBarOutline[3].Y = y + halfHeight;
			progressBarOutline[3].Z = 0.5f;
			progressBarOutline[3].Color = color;

			progressBarOutline[4].X = x - halfWidth;
			progressBarOutline[4].Y = y - halfHeight;
			progressBarOutline[4].Z = 0.5f;
			progressBarOutline[4].Color = color;

			drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
			drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
			drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, progressBarOutline);

			int barlength = (int)(this.downloadPercent * 2 * halfWidth);

			progressBar[0].X = x - halfWidth;
			progressBar[0].Y = y - halfHeight;
			progressBar[0].Z = 0.5f;
			progressBar[0].Color = color;

			progressBar[1].X = x - halfWidth;
			progressBar[1].Y = y + halfHeight;
			progressBar[1].Z = 0.5f;
			progressBar[1].Color = color;

			progressBar[2].X = x - halfWidth + barlength;
			progressBar[2].Y = y - halfHeight;
			progressBar[2].Z = 0.5f;
			progressBar[2].Color = color;

			progressBar[3].X = x - halfWidth + barlength;
			progressBar[3].Y = y + halfHeight;
			progressBar[3].Z = 0.5f;
			progressBar[3].Color = color;

			drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, progressBar);
		}

		private static string getFilePathFromUrl(string url)
		{
			if (url.ToLower().StartsWith("http://"))
			{
				url = url.Substring(7);
			}

			// ShockFire: Remove any illegal characters from the path
			foreach (char invalidChar in Path.GetInvalidPathChars())
			{
				url = url.Replace(invalidChar.ToString(), "");
			}

			// ShockFire: Also remove other illegal chars that are not included in InvalidPathChars for no good reason
			url = url.Replace(":", "").Replace("*", "").Replace("?", "");

			return Path.Combine(
				Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), Path.Combine("Cache\\ImageUrls", url));
		}


		/// <summary>
		/// Switch to a different image
		/// </summary>
		internal void UpdateTexture(string fileName)
		{
			try
			{
				if (this.device != null)
				{
					Texture oldTexture = this.texture;

					this._imagePath = fileName;
					Texture newTexture = ImageHelper.LoadTexture(fileName);
					this.texture = newTexture;

					if (oldTexture != null)
						oldTexture.Dispose();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Switch to a different image
		/// </summary>
		internal void UpdateTexture(Stream textureStream)
		{
			if (this.device != null)
			{
				Texture oldTexture = this.texture;

				Texture newTexture = ImageHelper.LoadTexture(textureStream);
				this.texture = newTexture;

				if (oldTexture != null)
					oldTexture.Dispose();
			}
		}


		/// <summary>
		/// Cleanup when layer is disabled
		/// </summary>
		public override void Dispose()
		{
			this.isInitialized = false;

			if (downloadThread != null)
			{
				if (downloadThread.IsAlive)
				{
					downloadThread.Abort();
				}
				downloadThread = null;
			}

			if (this.texture != null)
			{
				this.texture.Dispose();
				this.texture = null;
			}

			if (refreshTimer != null && refreshTimer.Enabled)
			{
				refreshTimer.Stop();
				refreshTimer = null;
			}
		}

		bool abortedFirstRefresh;

		private void refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!abortedFirstRefresh)
			{
				abortedFirstRefresh = true;
				return;
			}

			if (_imageUrl == null && _imagePath != null)
			{
				UpdateTexture(_imagePath);
			}
			else if (_imageUrl != null && _imageUrl.ToLower().StartsWith("http://"))
			{
				_imagePath = getFilePathFromUrl(_imageUrl);
				DownloadImage();
			}

		}

		public override void InitExportInfo(DrawArgs drawArgs, RenderableObject.ExportInfo info)
		{
			if (File.Exists(_imagePath))
			{
				info.dMaxLat = this.maxLat;
				info.dMaxLon = this.maxLon;
				info.dMinLat = this.minLat;
				info.dMinLon = this.minLon;

				using (System.Drawing.Image oBitmap = System.Drawing.Image.FromFile(_imagePath))
				{
					info.iPixelsX = oBitmap.Width;
					info.iPixelsY = oBitmap.Height;
				}
			}
		}

		public override void ExportProcess(DrawArgs drawArgs, ExportInfo expInfo)
		{
			using (System.Drawing.Image oBitmap = System.Drawing.Image.FromFile(_imagePath))
			{
				int iWidth = (int)Math.Round((this.maxLon - this.minLon) * (double)expInfo.iPixelsX / (expInfo.dMaxLon - expInfo.dMinLon));
				int iHeight = (int)Math.Round((this.maxLat - this.minLat) * (double)expInfo.iPixelsY / (expInfo.dMaxLat - expInfo.dMinLat));
				int iX = (int)Math.Round((this.minLon - expInfo.dMinLon) * (double)expInfo.iPixelsX / (expInfo.dMaxLon - expInfo.dMinLon));
				int iY = (int)Math.Round((expInfo.dMaxLat - this.maxLat) * (double)expInfo.iPixelsY / (expInfo.dMaxLat - expInfo.dMinLat));
				expInfo.gr.DrawImage(oBitmap, new System.Drawing.Rectangle(iX, iY, iWidth, iHeight));
			}
		}
	}
}
