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
			manipulator = new TextManipulator(value);
			renderer.render();
		}
	}

	public TextAreaView() {
		var x = context = new TextDisplayContext() {};
		x.view = this;
		manipulator = new TextManipulator();
		renderer = new TextAreaRenderer(context);
		x.gfx = view.graphics;

		configureView();
	}

	private void configureView() {
		view.input.KEY_DOWN += onKeyDown;
	}

	private void onKeyDown(KeyboardEvent e) {
		if (e.key == Keyboard.Key.Right)
			manipulator.carets.pos.x += 1;
		else if (e.key == Keyboard.Key.Left)
			manipulator.carets.pos.x -= 1;


		renderer.renderCarets();
	}
}
