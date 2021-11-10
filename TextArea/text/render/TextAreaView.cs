using audioysos.display;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;

namespace com.audionysos.text.render {
	public class TextAreaView : IDisplayable<Sprite> {
		public Sprite view { get; } = new Sprite();
		private Sprite carrets = new Sprite();
		
		Int2 size;
		public TextDisplayContext context { get; }
		
		/// <summary>Object which renders glyps on this view.</summary>
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
			view.addChild(carrets);
		}
	}

}
