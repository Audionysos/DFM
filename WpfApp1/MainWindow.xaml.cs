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

namespace WpfApp1 {
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
				96,96,
				WM.PixelFormats.Bgra32,
				null);
			this.img.Source = wpfBitmap;


			drawGlyph();
			

			var sw = Stopwatch.StartNew();
			
			drawBitmap();
			transferBitmap();
			
			sw.Stop();
			Console.WriteLine($@"done {sw.ElapsedMilliseconds}ms");
		}

		private void drawGlyph() {
			FontCollection collection = new FontCollection();
			FontFamily family = collection.Install("path/to/font.ttf");
			Font font = family.CreateFont(12, F.FontStyle.Italic);
			var g = font.GetGlyph('A');
			var gi = g.Instance;
			//image.Mutate(x => {
			//	var p = new Polygon(gi.ControlPoints);
			//	foreach (var p in gi.ControlPoints) {
			//		x.DrawLines(new Pen(Color.Red, 2), gi.ControlPoints);
			//	}

			//});
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
}
