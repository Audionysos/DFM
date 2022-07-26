﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using audioysos.geom;
using com.audionysos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using S = SixLabors.ImageSharp;

namespace WpfDNet {

	/// <summary>Implements <see cref="IMicroGraphics2D"/> interface utilizing <see cref="SixLabors.ImageSharp"/> library.</summary>
	public class SharpGraphics : IMicroGraphics2D {
		public double x { get; }
		public double y { get; }

		/// <summary>Original (raw) firgures prodeced by drawing methods.</summary>
		private List<Figure> figures = new List<Figure>();
		/// <summary>Transformed figures.</summary>
		private List<Figure> tFigures = new List<Figure>();

		private Figure currFigure = new Figure();
		private IFill fill;
		private Stroke stroke;
		private IFillPorovider fillPorovider;
		private Image<Bgra32> img;

		public SharpGraphics(Image<Bgra32> img) {
			this.img = img;
		}

		#region Figures creation
		public IMicroGraphics2D beginFill(uint rgb, double a = 1) {
			var av = (uint)(a * 255) & 0xFF;
			fill = (com.audionysos.Color)((rgb << 8) | av);
			return this;
		}

		public IMicroGraphics2D beginFill(IFillPiece fill) {
			if(fill is com.audionysos.Color c)
				return beginFill(c.rgb, c.a / 255d);
			this.fill = fillPorovider.getFill(fill.fillID);
			startNewFigure();
			return this;
		}

		public IMicroGraphics2D clear() {
			figures.Clear();
			return this;
		}

		public IMicroGraphics2D close() {
			startNewFigure();
			//render();
			return this;
		}

		public IMicroGraphics2D endFill() {
			startNewFigure();
			fill = null;
			return this;
		}

		public IMicroGraphics2D lineSyle(double w = 0, uint rgb = 0, double a = 1) {
			stroke = new Stroke() {
				size = w,
				stroke = (com.audionysos.Color)0xFF0000,
			};
			return this;
		}

		public IMicroGraphics2D lineTo(double x, double y) {
			currFigure.points.Add(new Point2(x, y));
			return this;
		}

		public IMicroGraphics2D moveTo(double x, double y) {
			startNewFigure();
			currFigure.points.Add(new Point2(x, y));
			return this;
		}

		private void startNewFigure() {
			if (currFigure.points.Count == 0) return;
			currFigure.fill = fill;
			currFigure.stroke = stroke;
			figures.Add(currFigure);
			currFigure = new Figure();
		}
		#endregion

		public IMicroGraphics2D wait() {
			return this;
		}

		#region Transform

		private Rect bounds = new Rect(); 
		public IMicroGraphics2D transform(audioysos.display.Transform t) {
			//return this;
			tFigures.Clear(); Figure tf = null;
			bounds.clear();
			for (int i = 0; i < figures.Count; i++) {
				var f = figures[i];
				tf = transformFigure(f, t);
				tFigures.Add(tf);
			}
			if (currFigure.points.Count > 0) {
				tf = transformFigure(currFigure, t);
				tf.fill = fill; tf.stroke = stroke;
				tFigures.Add(tf);
			}
			covertToSharpFigures();
			return this;
		}

		private Figure transformFigure(Figure f, audioysos.display.Transform t) {
			var tf = new Figure() { fill = f.fill, stroke = f.stroke };
			for (int i = 0; i < f.points.Count; i++) {
				var p = f.points[i].copy();
				t.transform(p);
				tf.points.Add(p);
				bounds.grow(p);
			}
			return tf;
		}
		#endregion

		#region Hit testing
		public bool pointInShape(IPoint2 p) {
			for (int i = 0; i < tFigures.Count; i++) {
				var f = tFigures[i];
				pointInShape(f, p);
			}
			return false;
		}

		private bool pointInShape(Figure f, IPoint2 tp) {
			IPoint2 pp = null;
			IRect<IPoint2> totalBounds = (IRect<IPoint2>)bounds;//of all figures
			var tbp = totalBounds.postion;
			var il = new Line2(tbp - (Point2)(10, 10), tp);
			var ic = 0;
			for (int i = 0; i < f.points.Count; i++) {
				var p = f.points[i];
				if (pp != null) {
					//TODO: implement intersection method
					var ir = Line2.intersection(il, new Line2(p, pp));
					if (ir != null) ic++;
				}
				pp = p;
			}
			return ic % 2 == 1;
		}
		#endregion

		#region Image sharp rendering
		/// <summary>ImageSharp ready figures</summary>
		private List<SharpFigure> sFigures = new List<SharpFigure>();
		private void covertToSharpFigures() {
			sFigures.Clear();
			var joined = new List<Figure>(); //figures with the same brush are joined so the ImageSharp will create holes (rendering glyphs)
			Figure last = null;
			for (int i = 0; i < tFigures.Count; i++) {
				var tf = tFigures[i];
				if (last == null || tf.sameStyle(last)) {
					joined.Add(tf);
					last = tf;
					continue;
				}
				var sf = new SharpFigure(joined, last.fill, last.stroke);
				sFigures.Add(sf);
				joined.Clear(); last = null;
			}
			if (joined.Count == 0) return;
			sFigures.Add(new SharpFigure(joined, last.fill, last.stroke));
		}

		public void render(Image<Bgra32> img) {
			img.Mutate(x => {
				for (int i = 0; i < sFigures.Count; i++) {
					var f = sFigures[i];
					if (f.brush != null)
						x.Fill(f.brush, f.path);
					if (f.pen == null) continue;
					for (int j = 0; j < f.points.Length; j++) {
						x.DrawLines(f.pen, f.points[j]);
					}
				}
			});
		}
		#endregion

	}

	public class SharpFigure {
		public IPath path;
		public PointF[][] points;
		public IBrush brush;
		public IPen pen;

		public SharpFigure(List<Figure> figures, IFill fill, Stroke stroke) {
			brush = getBrush(fill);
			pen = getPen(stroke);
			Polygon[] polys = null;
			if(figures.Count > 1) polys = new Polygon[figures.Count];
			points = new PointF[figures.Count][];
			for (int i = 0; i < figures.Count; i++) {
				var f = figures[i]; var pts = new PointF[f.points.Count];
				for (int j = 0; j < f.points.Count; j++) {
					var p = f.points[j];
					pts[j] = new PointF((float)p.x, (float)p.y);
				}
				var ls = new LinearLineSegment(pts);
				var pol = new Polygon(ls);
				points[i] = pts;
				if (polys != null) polys[i] = pol;
				else path = pol;
			}
			if (polys != null) path = new ComplexPolygon(polys);
		}

		#region Styling conversion
		internal IBrush getBrush(IFill fill) {
			if (fill == null) return null;
			var c = (com.audionysos.Color)fill;
			var sc = SixLabors.ImageSharp.Color
				.FromRgba((byte)c.r, (byte)c.g, (byte)c.b, (byte)c.a);
			return new SolidBrush(sc);
		}

		internal IPen getPen(Stroke stroke) {
			if (stroke == null) return null;
			var ac = (com.audionysos.Color)stroke.stroke;
			var sc = SixLabors.ImageSharp.Color
				.FromRgba((byte)ac.r, (byte)ac.g, (byte)ac.b, (byte)ac.a);
			var p = new SixLabors.ImageSharp.Drawing.Processing
				.Pen(sc, (float)stroke.size);
			return p;
		}
		#endregion

	}

	public class Figure {
		public List<IPoint2> points { get; set; } = new List<IPoint2>();
		public IFill fill { get; set; }
		public Stroke stroke { get; set; }

		public Figure() {

		}

		//TODO: Compare stroke properly
		public bool sameStyle(Figure f)
			=> f.fill?.Equals(fill) ?? false && f.stroke == stroke; 

	}

	public class Stroke {
		public double size { get; set; }
		public IFill stroke { get; set; }
	}


}
