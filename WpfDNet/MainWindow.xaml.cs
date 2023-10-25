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

namespace WpfDNet; 
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
		//new HitTestTests(adapter);
		//new DisplayTest(adapter);
		new TextAreaTest(adapter);
		//previous();
	}

}
