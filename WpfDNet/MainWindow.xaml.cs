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
using WpfDNet.SLtoWPF;
using SixLabors.Fonts.Unicode;

namespace WpfDNet {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : W.Window {
		SixLaborsToWPFAdapter adapter;

		public MainWindow() {
			InitializeComponent();
			//new WPFComparision(this); return;

			adapter = new SixLaborsToWPFAdapter(img);
			//Left = 1800;
			new DisplayTest(adapter);
			//new TextAreaTest(adapter);
			//previous();
		}

		#region Old
		(int x, int y) size = (500, 500);
		Image<Bgra32> image;
		WMI.WriteableBitmap wpfBitmap;

		//private void previous() {
		//	image = new Image<Bgra32>(size.x, size.y);

		//	wpfBitmap = new WMI.WriteableBitmap(size.x, size.y,
		//		96, 96,
		//		WM.PixelFormats.Bgra32,
		//		null);
		//	this.img.Source = wpfBitmap;
		//	img.Stretch = WM.Stretch.None;
		//	img.UseLayoutRounding = true;

		//	var sw = Stopwatch.StartNew();

		//	drawBitmap();
		//	var sg = new SharpGraphics(image);
		//	var g = new com.audionysos.Graphics(sg);
		//	//g.lineSyle(2);
		//	//g.moveTo(100, 100);
		//	//g.lineTo(100, 200);
		//	//g.lineTo(0, 200);
		//	//g.close();

		//	drawGlyphs(g, "Hello world! Prawdopodobnie najlepszy program na świecie!");
		//	//drawGlyphs(g, "apapapapapapapapapapapapapapapapapapap");


		//	transferBitmap();
		//	sw.Stop();
		//	Console.WriteLine($@"done {sw.ElapsedMilliseconds}ms");
		//}

		//private void drawGlyphs(com.audionysos.Graphics g) {
		//	var ch = 'a'; var s = 7;
		//	var x = 0; var y = 0;
		//	for (int i = 0; i < 100; i++) {
		//		x += s;
		//		drawGlyph(g, ch, 1, x, y);
		//		if (ch == 'z') ch = 'A';
		//		else ch++;
		//		if (x > 400) { x = 0; y += 15; }
		//	}
		//	g.close();
		//}

		//private void drawGlyphs(com.audionysos.Graphics g, string text) {
		//	var s = 7;
		//	var x = 0; var y = 0;
		//	for (int i = 0; i < text.Length; i++) {
		//		x += s;
		//		drawGlyph(g, text[i], 1, x, y);
		//		if (x > 400) { x = 0; y += 15; }
		//	}
		//	g.close();
		//}

		//private void drawGlyph(com.audionysos.Graphics g, char ch, float scale = 1, float x = 0, float y = 0) {
		//	if (ch == ' ') return;
		//	FontCollection collection = new FontCollection();
		//	//var ff = SystemFonts.Find("Calibri");
		//	//var ff = SystemFonts.Find("Consolas");

		//	//g.lineSyle(2);
		//	//FontFamily ff = collection.Install("path/to/font.ttf");
		//	//Font font = ff.CreateFont(11);
		//	Font font = SystemFonts.CreateFont("Consolas",11);
		//	//TextBuilder.GenerateGlyphs("dfsfsd", new TextOptions(font)).ElementAt(0)
		//	font.TryGetGlyphs(new CodePoint(ch), out var gls);
		//	GlyphLayout.
		//	gls[0].
		//	//var gl = font.GetGlyph(ch);
		//	var gl = gls[0];
		//	var gi = gl.Instance;
		//	//gi.
		//	//var min = gi.Height - font.EmSize;
		//	var min = gi.Height - font.FontMetrics.UnitsPerEm;

		//	var vm = font.FontMetrics.HorizontalMetrics;
		//	var sf = (vm.Ascender + vm.Descender) / gi.ScaleFactor * scale;
		//	//var sf = (font.Ascender) / gi.ScaleFactor * scale;
		//	var s = new System.Numerics.Vector2(sf, -sf);
		//	var off = new System.Numerics.Vector2(0 + x, (gi.Height - min) * s.X  + y);

		//	var cp = gi.ControlPoints[0] * s + off;
		//	var fp = cp;

		//	var color = 0u;
		//	g.beginFill(color, 1);
			
		//	g.moveTo(cp.X, cp.Y);
		//	var nep = gi.EndPoints[0]; var epi = 0;
		//	var m = false;
		//	for (int i = 1; i < gi.ControlPoints.Length; i++) {
		//		cp = gi.ControlPoints[i] * s + off;
		//		if (m) {
		//			//g.beginFill(color, 1);
		//			//g.endFill();
		//			g.moveTo(cp.X, cp.Y);				
		//			m = false;
		//			fp = cp;
		//			continue;
		//		}
		//		g.lineTo(cp.X, cp.Y);
		//		if (nep == i) {
		//			g.lineTo(fp.X, fp.Y);
		//			epi++;
		//			if (epi < gi.EndPoints.Length)
		//				nep = gi.EndPoints[epi];
		//			m = true;
		//		}
		//	}
		//	//g.close();
		//}

		//private void drawBitmap() {
		//	var p = new Star(100f, 100f, 5, 20f, 50f);
		//	image.Mutate(x => x.Fill(Color.White));
		//	//image.Mutate(x =>
		//	//	x.Fill(Color.Yellow, p)
		//	//	.Draw(Color.AliceBlue, 3, p));
		//}

		//private void transferBitmap() {
		//	var img = image.GetPixelMemoryGroup();
		//	var mg = img.ToArray()[0];
		//	var PixelData = MemoryMarshal.AsBytes(mg.Span).ToArray();

		//	wpfBitmap.Lock();
		//	wpfBitmap.WritePixels(new W.Int32Rect(0, 0, size.x, size.y),
		//		PixelData,
		//		size.x * 4, //stride (bytes per row);
		//		0, 0);
		//	wpfBitmap.Unlock();
		//}

		#endregion

	}
}
