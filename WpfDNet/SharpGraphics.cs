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

		private List<Figure> figures = new List<Figure>();

		private Figure currFigure = new Figure();
		private IFill fill;
		private Stroke stroke;
		private IFillPorovider fillPorovider;
		private Image<Bgra32> img;

		public SharpGraphics(Image<Bgra32> img) {
			this.img = img;
		}

		private void render() {
			img.Mutate(x => {
				var polys = new List<Polygon>();
				S.Color lfill = S.Color.Transparent;
				IPen lpen = null;
				for (int i = 0; i < figures.Count; i++) {
					var f = figures[i];
					var so = new ShapeGraphicsOptions();
					PointF[] pts = null;
					Polygon p = null;
					if (f.points.Count < 2) goto FINISH;
					pts = f.points.ToArray();
					var ls = new LinearLineSegment(f.points.ToArray());
					p = new Polygon(ls);
					//so.GraphicsOptions.
					var cfill = f.fill != null ? f.getBrush() : S.Color.Transparent;
					var cpen = f.stroke != null ? f.getPen() : null;
					polys.Add(p);
					if(polys.Count == 1) {
						lfill = cfill;
						lpen = cpen;
					}
					if(lfill == cfill && lpen == cpen
						&& i < figures.Count -1) {
						continue;
					}
					FINISH:
					if(polys.Count == 1) {
						if (lfill != S.Color.Transparent) {
							x.Fill(lfill, polys[0]);
						}
						if(lpen != null) {
							x.DrawLines(lpen, pts);
						}
					}else {
						var cp = new ComplexPolygon(polys);
						if(lfill != S.Color.Transparent)
							x.Fill(f.getBrush(), cp);
						if(lpen != null)
							x.DrawLines(f.getPen(), pts);

						polys.Clear();
						lfill = S.Color.Transparent;
						lpen = null;
					}
				}
			});
		}

		public IMicroGraphics2D beginFill(uint rgb, double a = 1) {
			fill = (com.audionysos.Color)((rgb << 8) | 0xFF);
			return this;
		}

		public IMicroGraphics2D beginFill(IFillPiece fill) {
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
			render();
			return this;
		}

		public IMicroGraphics2D endFill() {
			fill = null;
			startNewFigure();
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
			currFigure.points.Add(new PointF((float)x, (float)y));
			return this;
		}

		public IMicroGraphics2D moveTo(double x, double y) {
			startNewFigure();
			currFigure.points.Add(new PointF((float)x, (float)y));
			return this;
		}

		private void startNewFigure() {
			if (currFigure.points.Count == 0) return;
			currFigure.fill = fill;
			currFigure.stroke = stroke;
			figures.Add(currFigure);
			currFigure = new Figure();
		}

		public IMicroGraphics2D wait() {
			return this;
		}
	}

	internal class Figure {
		public List<PointF> points { get; set; } = new List<PointF>();
		public IFill fill { get; set; }
		public Stroke stroke { get; set; }

		public Figure() {

		}

		internal SixLabors.ImageSharp.Color getBrush() {
			var c = (com.audionysos.Color)fill;
			return SixLabors.ImageSharp.Color
				.FromRgb((byte)c.r, (byte)c.g, (byte)c.b);
		}

		internal IPen getPen() {
			var ac = (com.audionysos.Color)stroke.stroke;
			var sc = SixLabors.ImageSharp.Color
				.FromRgb((byte)ac.r, (byte)ac.g, (byte)ac.b);
			var p = new SixLabors.ImageSharp.Drawing.Processing
				.Pen(sc, (float)stroke.size);
			return p;
		}
	}

	internal class Stroke {
		public double size { get; set; }
		public IFill stroke { get; set; }
	}


}
