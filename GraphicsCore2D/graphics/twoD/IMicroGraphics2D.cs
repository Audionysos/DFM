using audioysos.geom;
using System.Collections.Generic;

namespace com.audionysos {

	public interface IMicroGraphics2D {

		/// <summary>Current positon from which the figures will be drawn.</summary>
		double x { get; }
		/// <summary>Current positon from which the figures will be drawn.</summary>
		double y { get; }

		#region Styling configurator

		/// <summary>Sets visual apparance of lines drawed with subsequent calls to darwing API.</summary>
		/// <param name="w">Withd/Thinckness of the lines.</param>
		/// <param name="rgba">Red-Green-Blue color of lines</param>
		/// <param name="a">Apha transparency where 1 is full opaque and 0 is fully transparent.</param>
		IMicroGraphics2D lineSyle(double w = 0, uint rgb = 0, double a = 1);

		/// <summary>Starts filling insides of drawed shape with given color.</summary>
		/// <param name="rgba">Red-Green-Blue color of lines</param>
		/// <param name="a">Apha transparency where 1 is full opaque and 0 is fully transparent.</param>
		IMicroGraphics2D beginFill(uint rgb, double a = 1);

		/// <summary>Starts filling insides of drawed shape with given fill piece (texture).</summary>
		/// <param name="fill"></param>
		IMicroGraphics2D beginFill(IFillPiece fill);

		/// <summary>Stops filling drawed shape.</summary>
		IMicroGraphics2D endFill();

		#endregion

		#region Drawing methods

		/// <summary>Moves "brush" to given position without drawing anything.</summary>
		/// <param name="x">Corrdinate on drawing surface.</param>
		/// <param name="y">Corrdinate on drawing surface.</param>
		IMicroGraphics2D moveTo(double x, double y);

		/// <summary>Draws line from previous to given position.</summary>
		/// <param name="x">Corrdinate on drawing surface.</param>
		/// <param name="y">Corrdinate on drawing surface.</param>
		IMicroGraphics2D lineTo(double x, double y);

		#endregion

		#region Steering controls
		/// <summary>Approves provious drawing calls telling it's ready to render.
		/// Closing the graphics doesn not prevents form further drawing.</summary>
		IMicroGraphics2D close();
		/// <summary>Clears all graphics constructed from previous drawing calls.</summary>
		IMicroGraphics2D clear();

		/// <summary>Waits for rendering thread and returns when the graphics is rendered and visible on the screen.
		/// This method is not required to be implemented as it's used only for debuggin.</summary>
		IMicroGraphics2D wait();
		#endregion

	}

	public interface IBasicGraphics2D : IMicroGraphics2D {
		/// <summary>Current position from which the figure will be drawn.</summary>
		IPoint2 p { get; }

		/// <summary>Draws series of points from current position through all of given points.</summary>
		IBasicGraphics2D drawPath(IReadOnlyList<IPoint2> points);

		IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3);
		IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3, IPoint2 p4);

	}

	public interface IGraphics2D : IBasicGraphics2D {

		IGraphics2D drawCircle(double x = 0, double y = 0, double r = 5);

		/// <summary>Draws eliptical arc to a given point..</summary>
		/// <param name="ep">Ellipse end point.</param>
		/// <param name="r">Radii for x and y.</param>
		/// <param name="d">Rotation of the ellipse in degrees</param>
		/// <param name="fa">Large arc flag.</param>
		/// <param name="fs">Sweep flag.</param>
		/// <param name="e">Rounding error.</param>
		IGraphics2D arcTo(IPoint2 ep, IPoint2 r, double d, int fa = 0, int fs = 0, double e = 0.5);

		IGraphics2D drawTraingles(IPoint2[] points, IPoint2[] uv, IFillPiece fill);
	}


	public static class MicroGraphicsExtesions {
		public static T lineTo<T>(this T g, IPoint2 p)
			where T : IMicroGraphics2D {
			g.lineTo(p.x, p.y);
			return g;
		}

		public static T moveTo<T>(this T g, IPoint2 p)
			where T : IMicroGraphics2D {
			g.lineTo(p.x, p.y);
			return g;
		}
	}

}
