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
using AI = audionysos.input;
using System.Diagnostics;
using com.audionysos.text.utils;

namespace WpfDNet; 
public class SLDisplaySurface : DisplaySurface {
	public (int x, int y) size { get; private set; } = (500, 500);
	public Image<Bgra32> image { get; private set; }


	public SLDisplaySurface() {
		image = new Image<Bgra32>(size.x, size.y);
		drawBackground();
		TextDisplayContext.defaultGlyphsProvider = new SLGlyphsProvider(); //TODO: This should find some better placement in future
	}

	private void drawBackground() {
		image.Mutate(x =>
			x.Fill(background.toSL()));
			//x.Fill(SixLabors.ImageSharp.Color.LightGray));
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
		this.root = root;
		root.Focusable = true;
		root.Focus();
		System.Windows.Input.Keyboard.Focus(root);
		Console.WriteLine(root.IsKeyboardFocused);
		root.MouseMove += onMouseMove;
		root.MouseDown += onMouseDown;
		root.MouseUp += onMouseUp;
		root.KeyDown += onKeyDown;
		root.KeyUp += onKeyUp;

		root.TextInput += onTextInput;
	}

	private void onTextInput(object sender, TextCompositionEventArgs e) {
		Debug.WriteLine(e.Text);
		if (e.Text.isEmpty()) return;
		il.keyDown(this, AI.Keyboard.Key.None, e.Text[0]);
	}

	private void onKeyDown(object sender, KeyEventArgs e) {
		il.keyDown(this, (AI.Keyboard.Key)e.Key);
	}

	private void onKeyUp(object sender, KeyEventArgs e) {
		il.keyUp(this, (AI.Keyboard.Key)e.Key);
	}

	private void onMouseDown(object sender, MouseButtonEventArgs e) {
		var m = e.GetPosition(root);
		dp.position = new Point2(m.X, m.Y);
		il.pointerDown(this, dp);
	}

	private void onMouseUp(object sender, MouseButtonEventArgs e) {
		var m = e.GetPosition(root);
		dp.position = new Point2(m.X, m.Y);
		il.pointerUp(this, dp);
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

	public override bool isCurrentClipboard(object o) {
		if(o as IDataObject == null) return false;
		return Clipboard.IsCurrent(o as IDataObject);
	}

	public override object getClipboard(Type t) {
		var ido = Clipboard.GetDataObject();
		if (ido == null) return null;
		var d = ido.GetData(t);
		if (d == null) return ido;
		return d;
	}

	public override void setClipboard(object o) {
		Clipboard.SetData(DataFormats.Serializable, o);
	}
}
