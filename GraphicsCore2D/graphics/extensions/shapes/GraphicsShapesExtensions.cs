using audionysos.geom;
using com.audionysos;
using System;
using System.Collections.Generic;
using System.Text;

namespace audionysos.graphics.extensions.shapes {

	public static class GraphicsShapesExtensions {

		/// <summary>Draws rectagle using this graphics and returns this graphics.</summary>
		/// <param name="x">Position coorfinate.</param>
		/// <param name="y">Position coorfinate.</param>
		/// <param name="w">Width</param>
		/// <param name="h">Height</param>
		/// <returns></returns>
		public static T drawRect<T>(this T g, double x, double y, double w, double h)
			where T : IMicroGraphics2D	
		{
			g.moveTo(x, y);
			g.lineTo(x + w, y);
			g.lineTo(x + w, y + h);
			g.lineTo(x, y + h);
			g.lineTo(x, y);
			return g;
		}

		/// <summary>Draws simple diamond shape.</summary>
		/// <param name="c">Center of the figure</param>
		/// <param name="width">Width of the figure.</param>
		/// <param name="height">Height if the figure.</param>
		public static T drawDiamond<T>(this T g, IPoint2 c, double width, double height)
			where T : IMicroGraphics2D
		{
			var p = new Point2(c.x, c.y - height * .5);
			p.x += width * 0.5; p.x += height * 0.5; g.lineTo(p);
			p.x -= width * 0.5; p.x += height * 0.5; g.lineTo(p);
			p.x -= width * 0.5; p.x -= height * 0.5; g.lineTo(p);
			p.x += width * 0.5; p.x -= height * 0.5; g.lineTo(p);
			return g;
		}

		public static T template<T>(this T g)
			where T : IMicroGraphics2D
		{

			return g;
		}

	}
}
