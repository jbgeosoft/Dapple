using System;
using System.Collections;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using WorldWind.Renderable;
using Utility;

namespace WorldWind.Menu
{
	public sealed class MenuUtils
	{
		private MenuUtils(){}

		public static void DrawLine(Vector2[] linePoints, int color, Device device)
		{
			CustomVertex.TransformedColored[] lineVerts = new CustomVertex.TransformedColored[linePoints.Length];

			for(int i = 0; i < linePoints.Length; i++)
			{
				lineVerts[i].X = linePoints[i].X;
				lineVerts[i].Y = linePoints[i].Y;
				lineVerts[i].Z = 0.0f;

				lineVerts[i].Color = color;
			}

			device.TextureState[0].ColorOperation = TextureOperation.Disable;
			device.VertexFormat = CustomVertex.TransformedColored.Format;

			device.DrawUserPrimitives(PrimitiveType.LineStrip, lineVerts.Length - 1, lineVerts);
		}

		public static void DrawBox(int ulx, int uly, int width, int height, float z, int color, Device device)
		{
			CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[4];
			verts[0].X = (float)ulx;
			verts[0].Y = (float)uly;
			verts[0].Z = z;
			verts[0].Color = color;

			verts[1].X = (float)ulx;
			verts[1].Y = (float)uly + height;
			verts[1].Z = z;
			verts[1].Color = color;

			verts[2].X = (float)ulx + width;
			verts[2].Y = (float)uly;
			verts[2].Z = z;
			verts[2].Color = color;

			verts[3].X = (float)ulx + width;
			verts[3].Y = (float)uly + height;
			verts[3].Z = z;
			verts[3].Color = color;

			device.VertexFormat = CustomVertex.TransformedColored.Format;
			device.TextureState[0].ColorOperation = TextureOperation.Disable;
			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts.Length - 2, verts);
		}
	}
}
