﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using audionysos.geom;
using com.audionysos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using S = SixLabors.ImageSharp;

namespace WpfDNet; 

/// <summary>Implements <see cref="IMicroGraphics2D"/> interface utilizing <see cref="SixLabors.ImageSharp"/> library.</summary>
public class SharpGraphics2 : IMicroGraphics2D {
	public double x { get; }
	public double y { get; }

	private List<Figure2> figures = new List<Figure2>();
	private List<Figure2> tFigures = new List<Figure2>();

	private Figure2 currFigure = new Figure2();
	private IFill fill;
	private Stroke2 stroke;
	private IFillPorovider fillPorovider;
	private Image<Bgra32> img;

	public SharpGraphics2(Image<Bgra32> img) {
		this.img = img;
	}

	public void render(Image<Bgra32> img) {
		img.Mutate(x => {
			var polys = new List<Polygon>();
			S.Color lfill = S.Color.Transparent;
			Pen lpen = null;
			for (int i = 0; i < tFigures.Count; i++) {
				var f = tFigures[i];
				//var so = new ShapeGraphicsOptions();
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
					&& i < tFigures.Count -1) {
					continue;
				}
				FINISH:
				if(polys.Count == 1) {
					if (lfill != S.Color.Transparent) {
						x.Fill(lfill, polys[0]);
					}
					if(lpen != null) {
						x.DrawLine(lpen, pts);
					}
				}else {
					var cp = new ComplexPolygon(polys);
					if(lfill != S.Color.Transparent)
						x.Fill(f.getBrush(), cp);
					if(lpen != null)
						x.DrawLine(f.getPen(), pts);

					polys.Clear();
					lfill = S.Color.Transparent;
					lpen = null;
				}
			}
		});
	}

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
		fill = null;
		startNewFigure();
		return this;
	}

	public IMicroGraphics2D lineStyle(double w = 0, uint rgb = 0, double a = 1) {
		stroke = new Stroke2() {
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
		currFigure = new Figure2();
	}

	public IMicroGraphics2D wait() {
		return this;
	}

	public IMicroGraphics2D transform(audionysos.display.Transform t) {
		tFigures.Clear(); Figure2 tf = null;
		for (int i = 0; i < figures.Count; i++) {
			var f = figures[i];
			tf = transformFigure(f, t);
			tFigures.Add(tf);
		}
		if (currFigure.points.Count == 0) return this;
		tf = transformFigure(currFigure, t);
		tf.fill = fill; tf.stroke = stroke;
		tFigures.Add(tf);
		return this;
	}

	private Figure2 transformFigure(Figure2 f, audionysos.display.Transform t) {
		var gp = new Point2();
		var tf = new Figure2() { fill = f.fill, stroke = f.stroke };
		for (int i = 0; i < f.points.Count; i++) {
			var p = f.points[i];
			gp.set(p.X, p.Y);
			t.transform(gp);
			tf.points.Add(new PointF((float)gp.x, (float)gp.y));
		}
		return tf;
	}
}

internal class Figure2 {
	public List<PointF> points { get; set; } = new List<PointF>();
	public IFill fill { get; set; }
	public Stroke2 stroke { get; set; }

	public Figure2() {

	}

	internal SixLabors.ImageSharp.Color getBrush() {
		var c = (com.audionysos.Color)fill;
		return SixLabors.ImageSharp.Color
			.FromRgba((byte)c.r, (byte)c.g, (byte)c.b, (byte)c.a);
	}

	internal Pen getPen() {
		var ac = (com.audionysos.Color)stroke.stroke;
		var sc = SixLabors.ImageSharp.Color
			.FromRgba((byte)ac.r, (byte)ac.g, (byte)ac.b, (byte)ac.a);
		var p = //new SixLabors.ImageSharp.Drawing.Processing
			Pens.Solid(sc, (float)stroke.size);
		return p;
	}
}

internal class Stroke2 {
	public double size { get; set; }
	public IFill stroke { get; set; }
}
