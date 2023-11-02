using audionysos.display;
using audionysos.input;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

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
		x.view = this;
		x.manipulator = manipulator = new TextManipulator(context);
		x.renderer = renderer = new TextAreaRenderer(context);
		//x.gfx = view.graphics;

		configureView();
	}

	private void configureView() {
		TEXT_CHANGED += onTextChanged;
		view.input.KEY_DOWN += onKeyDown;
		view.input.KEY_UP += onKeyUp;
		//TODO: the whole manipulator and it's selection is changed when text is set to the text area.
		context.background.addChild(selectionView);
	}

	private void onTextChanged(TextAreaView view) {
		onSelectionChanged(manipulator.selection);
		manipulator.selection.CHANGED += onSelectionChanged;
	}

	private Sprite selectionView = new ();
	private void onSelectionChanged(TextSpan span) {
		renderer.drawBorder(span, selectionView.graphics);
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
		else if (e.character != '\0') {
			//var ch = e.key.ToString();
			manipulator.insert(e.character);
		}

		renderer.renderCarets();
		var c = manipulator.carets;
		var s = $"Ln: {c.pos.y}\tCh: {c.lCh}\tCol: {c.actualPos.x}\tACh: {c.ch}\t(x: {c.pos.x}\ty: {c.pos.y})";
		Debug.WriteLine(s);
	}

	private void onKeyUp(KeyboardEvent e) {
		Debug.WriteLine($@"KeyUp: {e.key}");
		if (e.key.isShift())
			manipulator.isSelecting = false;
	}
}
