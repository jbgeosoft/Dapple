using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWind
{
   public class GeographicQuad
   {
      public double X1, Y1; // lower left
      public double X2, Y2; // lower right
      public double X3, Y3; // upper right
      public double X4, Y4; // upper left

      public GeographicQuad(double _X1, double _Y1, double _X2, double _Y2, double _X3, double _Y3, double _X4, double _Y4)
      {
         X1 = _X1; Y1 = _Y1;
         X2 = _X2; Y2 = _Y2;
         X3 = _X3; Y3 = _Y3;
         X4 = _X4; Y4 = _Y4;
      }
   }

   public class GeographicBoundingBox
   {
      public double North;
      public double South;
      public double West;
      public double East;

      public GeographicBoundingBox(double north, double south, double west, double east)
      {
         North = north;
         South = south;
         West = west;
         East = east;
      }

      public static GeographicBoundingBox FromQuad(GeographicQuad quad)
      {
         return new GeographicBoundingBox(Math.Max(Math.Max(Math.Max(quad.Y1, quad.Y2), quad.Y3), quad.Y4),
            Math.Min(Math.Min(Math.Min(quad.Y1, quad.Y2), quad.Y3), quad.Y4),
            Math.Min(Math.Min(Math.Min(quad.X1, quad.X2), quad.X3), quad.X4),
            Math.Max(Math.Max(Math.Max(quad.X1, quad.X2), quad.X3), quad.X4));
      }

      public bool IntersectsWith(GeographicBoundingBox test)
      {
          return (test.West < this.East && this.West < test.East && test.South < this.North && this.South < test.North);
      }

      public bool Contains(GeographicBoundingBox test)
      {
         return (test.West >= this.West && test.East <= this.East && test.South >= this.South && test.North < this.North);
      }
   }
}
