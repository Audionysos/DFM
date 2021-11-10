using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using audioysos.display;
using com.audionysos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using com.audionysos.text.render;
using audioysos.geom;

namespace WpfDNet {
	public class SLDiplaySurface : DisplaySurface {
		public (int x, int y) size { get; private set; } = (500, 500);
		public Image<Bgra32> image { get; private set; }
		

		public SLDiplaySurface() {
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

}
