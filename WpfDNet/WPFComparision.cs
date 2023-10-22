using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDNet; 
public class WPFComparision {

	public Canvas main { get; private set; }
	public WPFComparision(ContentControl parent) {
		main = new Canvas();
		main.Background = Brushes.LightGray;
		main.Width = 500;
		main.Height = 500;
		parent.Content = main;
		ta = new TextBlock();
		main.Children.Add(ta);

		test();

		CompositionTarget.Rendering += onRender;
	}

	private void onRender(object sender, EventArgs e) {
		foreach (object k in els) {
			onLayout(k, null);
		}
		count();
	}

	private Dictionary<object, (double x, double y)> vels = new Dictionary<object, (double x, double y)>();
	private List<Grid> els = new List<Grid>();
	private void test() {
		var r = new Random();
		vels.Clear();
		for (int i = 0; i < 950; i++) {
			var e = new Grid();
			e.Name = "child" + i;
			main.Children.Add(e);
			var s = r.Next(5, 15);
			var w = s; var h = s;
			e.Background = new SolidColorBrush(Color.FromArgb(33, 0, 255, 0));
			e.Width = w;
			e.Height = h;
			Canvas.SetLeft(e,0); Canvas.SetTop(e,0);
			//e.LayoutUpdated += onLayout;
			els.Add(e);
			vels.Add(e,
				(r.NextDouble() * 20 - 10
				, r.NextDouble() * 20 - 10));
		}
	}

	private void onLayout(object sender, EventArgs e) {
		var obj = sender as Grid;
		var v = vels[obj];
		(double x, double y) t = (Canvas.GetLeft(obj), Canvas.GetTop(obj));
		t.x += v.x; t.y += v.y;
		if (t.y < 0) t.y = 0; //SixLabors complain about this
		Canvas.SetLeft(obj, t.x);
		Canvas.SetTop (obj, t.y);

		if (v.x > 0 && t.x > 450
			|| v.x < 0 && t.x < 0) v.x *= -1;

		if (v.y > 0 && t.y > 450
			|| v.y < 0 && t.y <= 0) v.y *= -1;

		vels[obj] = v;
	}

	private TextBlock ta;
	private DateTime lastCheck = DateTime.Now;
	private int frames = 0;
	private void count() {
		var d = DateTime.Now - lastCheck;
		frames++;
		if (d.Seconds > 1) {
			var fps = frames / d.TotalSeconds;
			ta.Text = $@"FPS: {fps:###.#}";
			frames = 0; lastCheck = DateTime.Now;
		}
	}

}
