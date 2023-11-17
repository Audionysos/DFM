using audionysos.display;
using audionysos.input;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using audionysos.graphics.extensions.shapes;
using audionysos.geom;
using System.Numerics;

namespace com.audionysos.text.render; 
public class TextAreaView : IDisplayable<Sprite> {
	private static Action<string> WL = s => Debug.WriteLine(s);

	public event Action<TextAreaView> TEXT_CHANGED;

	public Sprite view { get; } = new Sprite();
	
	Int2 size;
	public TextDisplayContext context { get; }
	
	/// <summary>Object which renders glyphs on this view.</summary>
	public TextAreaRenderer renderer {
		get => context.renderer;
		private set => context.renderer = value;
	}
	public TextManipulator manipulator {
		get => context.manipulator;
		private set => context.manipulator = value;
	}

	/// <summary>Gets or set displayed text as raw string.</summary>
	public string text {
		get {
			return manipulator.text.ToString();
		}
		set {
			manipulator = new TextManipulator(value, context);
			TEXT_CHANGED?.Invoke(this);
			renderer.render();
		}
	}

	public TextAreaView() {
		view.isFocusable = true;
		var x = context = new TextDisplayContext() {};
		x.background = new Sprite();
		x.container = new Sprite();
		view.addChild(x.background);
		view.addChild(x.container);
		view.name = "textarea";
		x.view = this;
		x.renderer = renderer = new TextAreaRenderer(context);
		x.manipulator = manipulator = new TextManipulator(context);

		//x.gfx = view.graphics;

		configureView();
	}

	private void onCaretsChanged(TextCaret caret, int arg2) {
		renderer.renderCarets();
	}

	private void configureView() {
		TEXT_CHANGED += onTextChanged;
		view.input.KEY_DOWN += onKeyDown;
		view.input.KEY_UP += onKeyUp;

		view.input.POINTER_ENTER += onPoinerEnter;
		view.input.POINTER_LEFT += onPoinerLeft;
		view.input.POINTER_MOVE += onPointerMove;
		view.input.POINTER_DOWN += onPointerDown;
		view.input.POINTER_UP += onPointerUp;

		//TODO: the whole manipulator and it's selection is changed when text is set to the text area.
		drawBackground();
		context.background.addChild(selectionView);
	}

	private Sprite bg = new Sprite();
	private Sprite chRect = new Sprite();
	private void drawBackground() {
		context.background.addChild(bg);
		//TODO: Investigate why cached graphics are no flushed
		bg.graphics.clear();
		bg.graphics.beginFill(0xfffff4, 1);
		bg.graphics.drawRect(0,0, 300, 300);

		context.background.addChild(chRect);
	}

	#region Pointer events handlers 
	/// <summary>Pointer down position.</summary>
	private Point2 pdp = new Point2();
	/// <summary>Last know pointer position when it was moved.</summary>
	private Point2 plp = new Point2();
	bool isDragging = false;
	private void onPointerDown(PointerEvent e) {
		var lp = view.localCoordinates(e.pointer.position);
		pdp.set(lp);
		WL("Pointer Down");
		isDragging = true;


		var m = manipulator;
		m.selection.clear();
		var cl = context.renderer.getPosition(lp, true);
		var chi = m.getCharacter(cl);
		WL($"Moving caret from {m.carets.ch,3} to {chi,3}");
		m.carets.ch = chi;
		WL($"Current: {m.carets.ch}");
	}

	private void onPointerUp(PointerEvent e) {
		isDragging = false;
		var p = view.localCoordinates(e.pointer.position);
		WL("Pointer Up");
		manipulator.isSelecting = false;
	}

	private void onPointerMove(PointerEvent e) {
		var lp = view.localCoordinates(e.pointer.position);
		plp.set(lp);
		var cl = context.renderer.getPosition(lp, true);

		if (isDragging) {
			var m = manipulator;
			m.isSelecting = true;
			m.carets.pos = cl;
			return;
		}
		var r = renderer.getGlyphRect(cl);
		if (r.size.x == 0) return;
		chRect.graphics.clear();
		chRect.graphics.lineStyle(1, 0xFF0000);
		chRect.graphics.drawRect(r.position.x, r.position.y , r.size.x, r.size.y);
	}

	private void onPoinerEnter(PointerEvent e) {
		Cursor.set(Cursor.TEXT);
	}

	private void onPoinerLeft(PointerEvent e) {
		Cursor.set(null);
	}
	#endregion

	private void onTextChanged(TextAreaView view) {
		onSelectionChanged(manipulator.selection);
		manipulator.selection.CHANGED += onSelectionChanged;
		manipulator.carets.CHANGED += onCaretsChanged;
	}

	private Sprite selectionView = new ();
	private void onSelectionChanged(TextSpan span) {
		renderer.drawBorder(span, selectionView.graphics);

		bg.graphics.clear();
		bg.graphics.beginFill(0xfffff4, 1);
		bg.graphics.drawRect(0, 0, 300, 300);
	}

	private void onKeyDown(KeyboardEvent e) {
		if (e.key == Keyboard.Key.Right)
			manipulator.carets++;
		else if (e.key == Keyboard.Key.Left)
			manipulator.carets--;
		else if (e.key == Keyboard.Key.Down)
			manipulator.carets.pos.y += 1;
		else if (e.key == Keyboard.Key.Up)
			manipulator.carets.pos.y -= 1;
		else if (e.key == Keyboard.Key.Tab)
			manipulator.insert('\t');
		else if (e.key.isShift())
			manipulator.isSelecting = true;
		//TODO: When button is released while the application is in break mode it will not register the release event 
		else if (Keyboard.anyPressed(Keyboard.Key.LeftCtrl, Keyboard.Key.RightCtrl)) {
			if (e.key == Keyboard.Key.X) cut();
			else if (e.key == Keyboard.Key.C) copy();
			else if (e.key == Keyboard.Key.V) paste();
		} else if (e.character != '\0') {
			//var ch = e.key.ToString();
			manipulator.insert(e.character);
		}

		var c = manipulator.carets;
		var s = $"Ln: {c.pos.y}\tCh: {c.lCh}\tCol: {c.actualPos.x}\tACh: {c.ch}\t(x: {c.pos.x}\ty: {c.pos.y})";
		Debug.WriteLine(s);
	}

	private void paste() {
		var d = PinBoard.get<string>();
		if (d == null) return;
		manipulator.insert(d);
	}

	private void copy() {
		PinBoard.set(manipulator.selection.text);
	}

	private void cut() {

	}

	private void onKeyUp(KeyboardEvent e) {
		Debug.WriteLine($@"KeyUp: {e.key}");
		if (e.key.isShift())
			manipulator.isSelecting = false;
	}
}
