using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using W = System.Windows;
using WM = System.Windows.Media;
using WMI = System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using System.Diagnostics;
using System;
using SixLabors.Fonts;
using F = SixLabors.Fonts;

namespace WpfDNet {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : W.Window {
		(int x, int y) size = (500, 500);
		Image<Bgra32> image;
		WMI.WriteableBitmap wpfBitmap;

		public MainWindow() {
			InitializeComponent();
			image = new Image<Bgra32>(size.x, size.y);

			wpfBitmap = new WMI.WriteableBitmap(size.x, size.y,
				96, 96,
				WM.PixelFormats.Bgra32,
				null);
			this.img.Source = wpfBitmap;

			var sw = Stopwatch.StartNew();

			drawBitmap();
			var sg = new SharpGraphics(image);
			var g = new com.audionysos.Graphics(sg);
			g.lineSyle(2);
			//g.moveTo(100, 100);
			//g.lineTo(100, 200);
			//g.lineTo(0, 200);
			//g.close();

			drawGlyph(g);

			transferBitmap();
			sw.Stop();
			Console.WriteLine($@"done {sw.ElapsedMilliseconds}ms");
		}

		private void drawGlyph(com.audionysos.Graphics g) {
			FontCollection collection = new FontCollection();
			var ff = SystemFonts.Find("Consolas");

			//FontFamily ff = collection.Install("path/to/font.ttf");
			Font font = ff.CreateFont(12);
			var gl = font.GetGlyph('A');
			var gi = gl.Instance;

			var off = new System.Numerics.Vector2(0, -2000);

			var s = 0.2;
			var cp = gi.ControlPoints[0] + off;
			var fp = cp;
			g.moveTo(cp.X * s, -cp.Y * s);
			var nep = gi.EndPoints[0]; var epi = 0;
			var m = false;
			for (int i = 1; i < gi.ControlPoints.Length; i++) {
				cp = gi.ControlPoints[i] + off;
				if(m) {
					g.moveTo(cp.X, -cp.Y);
					m = false;
					fp = cp;
					continue;
				}
				g.lineTo(cp.X * s, -cp.Y * s);
				if(nep == i) {
					g.lineTo(fp.X * s, -fp.Y * s);
					epi++;
					if (epi < gi.EndPoints.Length)
						nep = gi.EndPoints[epi];
					m = true;
				}
			}
			g.close();
		}

		private void drawBitmap() {
			var p = new Star(100f, 100f, 5, 20f, 50f);
			image.Mutate(x =>
				x.Fill(Color.Yellow, p)
				.Draw(Color.AliceBlue, 3, p));
		}

		private void transferBitmap() {
			var img = image.GetPixelMemoryGroup();
			var mg = img.ToArray()[0];
			var PixelData = MemoryMarshal.AsBytes(mg.Span).ToArray();

			wpfBitmap.Lock();
			wpfBitmap.WritePixels(new W.Int32Rect(0, 0, size.x, size.y),
				PixelData,
				size.x * 4, //stride (bytes per row);
				0, 0);
			wpfBitmap.Unlock();
		}


}
