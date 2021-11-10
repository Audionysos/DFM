using audioysos.display;
using com.audionysos.text.render;
using SixLabors.ImageSharp.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using W = System.Windows;
using WM = System.Windows.Media;
using WMI = System.Windows.Media.Imaging;

namespace WpfDNet.SLtoWPF {
	public class SixLaborsToWPFAdapter {
		public WMI.WriteableBitmap wpfBitmap { get; private set; }
		public SLDiplaySurface displaySurface;
		public W.Controls.Image image { get; private set; }
		private DispatcherTimer timer;

		public SixLaborsToWPFAdapter(W.Controls.Image image) {
			this.image = image;

			var ds = new SLDiplaySurface();
			displaySurface = ds;
			wpfBitmap = new WMI.WriteableBitmap(ds.size.x, ds.size.y,
				96, 96,
				WM.PixelFormats.Bgra32,
				null);

			image.Source = wpfBitmap;
			image.Stretch = WM.Stretch.None;
			image.UseLayoutRounding = true;

			timer = new DispatcherTimer(DispatcherPriority.Normal);
			timer.Interval = TimeSpan.FromMilliseconds(30);
			timer.Tick += onTick;
			timer.Start();
		}

		private void onTick(object sender, EventArgs e) {
			displaySurface.update();
			transferBitmap();
		}

		public void transferBitmap() {
			var ds = displaySurface;
			var img = ds.image.GetPixelMemoryGroup();
			var mg = img.ToArray()[0];
			var PixelData = MemoryMarshal.AsBytes(mg.Span).ToArray();

			wpfBitmap.Lock();
			wpfBitmap.WritePixels(
				new W.Int32Rect(0, 0, ds.size.x, ds.size.y),
				PixelData,
				ds.size.x * 4, //stride (bytes per row);
				0, 0);
			wpfBitmap.Unlock();
		}

	}
}
