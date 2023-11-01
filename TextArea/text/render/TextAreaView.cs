using audionysos.display;
using audionysos.input;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;

namespace com.audionysos.text.render; 
public class TextAreaView : IDisplayable<Sprite> {
	public Sprite view { get; } = new Sprite();
	//private Sprite carrets = new Sprite();
	
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
		view.input.KEY_DOWN += onKeyDown;
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
		else if (e.character != '\0') {
			//var ch = e.key.ToString();
			manipulator.insert(e.character);
		}


		renderer.renderCarets();
	}
}
