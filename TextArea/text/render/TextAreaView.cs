using audionysos.display;
using audionysos.input;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using audionysos.graphics.extensions.shapes;
using audionysos.geom;

namespace com.audionysos.text.render; 
public class TextAreaView : IDisplayable<Sprite> {
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
		isDragging = true;
		
	}

	private void onPointerUp(PointerEvent e) {
		isDragging = false;
		var p = view.localCoordinates(e.pointer.position);
		if (p - pdp > 2) return; //was moved
		
		manipulator.carets.pos = renderer.getPosition(p);
		renderer.renderCarets();
	}

	private void onPointerMove(PointerEvent e) {
		var lp = view.localCoordinates(e.pointer.position);
		plp.set(lp);
		var cl = context.renderer.getPosition(lp, true);

		if (isDragging) {
			var m = manipulator;
			var chi = m.getCharacter(cl);
			m.selection.end = chi;
			return;
		}
		var r = renderer.getGlyphRect(cl);
		Debug.WriteLine($"Pointer: {cl}");
		if(r.size.x == 0)
			return;
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
		else if (Keyboard.anyPressed(Keyboard.Key.LeftCtrl, Keyboard.Key.RightCtrl)) {
			if (e.key == Keyboard.Key.X) cut();
			else if (e.key == Keyboard.Key.C) copy();
			else if (e.key == Keyboard.Key.V) paste();
		} else if (e.character != '\0') {
			//var ch = e.key.ToString();
			manipulator.insert(e.character);
		}

		renderer.renderCarets();
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

	}

	private void cut() {

	}

	private void onKeyUp(KeyboardEvent e) {
		Debug.WriteLine($@"KeyUp: {e.key}");
		if (e.key.isShift())
			manipulator.isSelecting = false;
	}
}
