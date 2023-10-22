using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using audionysos.display;
using com.audionysos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using com.audionysos.text.render;
using audionysos.geom;
using System.Collections.Generic;
using audionysos.input;
using System.Windows.Input;
using System.Windows;
using System;

namespace WpfDNet {
	public class SLDisplaySurface : DisplaySurface {
		public (int x, int y) size { get; private set; } = (500, 500);
		public Image<Bgra32> image { get; private set; }


		public SLDisplaySurface() {
			image = new Image<Bgra32>(size.x, size.y);
			drawBackground();
			TextDisplayContext.defaulGlyphsProvider = new SLGlyphsProvider(); //TODO: This should find some better placement in future
		}

		private void drawBackground() {
			image.Mutate(x =>
				x.Fill(SixLabors.ImageSharp.Color.LightGray));
		}

		public override IMicroGraphics2D createGraphics() {
			var sg = new SharpGraphics(image);
			//var g = new com.audionysos.Graphics(sg);
			return sg;
		}

		public override void renderGraphics(IMicroGraphics2D graphics) {
			var sg = (SharpGraphics)graphics;
			sg.render(image);
		}

		public override void clear<P>(IRect<P> rect = null) {
			if (rect == null) drawBackground();
		}
	}

	public class WPFInputProcessor : InputProcessor {
		private InputListener il;
		private readonly FrameworkElement root;

		public WPFInputProcessor(FrameworkElement root) {
			root.MouseMove += onMouseMove;
			this.root = root;
		}

		private DisplayPointer dp = new() {
			id = 0,
			type = DisplayPointerType.UNKNOWN
		};

		private void onMouseMove(object sender, MouseEventArgs e) {
			var m = e.GetPosition(root);
			dp.position = new Point2(m.X, m.Y);
			il.pointerMove(this, dp);
		}

		public override void registerInputListener(InputListener il) {
			this.il = il;
		}

		public override IPoint2 getSurfacePosition(DisplayPointer p, DisplaySurface s) {
			return new Point2(0,0);
		}
	}

}
