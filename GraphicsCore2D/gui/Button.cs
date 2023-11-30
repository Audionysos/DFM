using System;
using System.Reflection;
using System.Security.Principal;
using System.Collections.Generic;
using System.Collections;
using System.Reflection.Metadata;
using audionysos.display;
using audionysos.graphics.extensions.shapes;
using audionysos.gui.layout;
using audionysos.gui.style;

namespace audionysos.gui;

public class Button {
	private Action<Button>? _PRESSED;
	public Action<Button>? PRESSED {
		get => _PRESSED;
		set => _PRESSED += value;
	}
	public event Action<Button>? RELEASED;
	public event Action<Button>? STATE_CHANGED;

	public ButtonStates states { get; set; } = new ButtonStates();

	private ButtonState _state;
	public ButtonState state {
		get => _state;
	}
	#region Default state data
	public object? content {
		get => states[0].content;
		set => states[0].content = value;
	}
	public LayoutSettings layout {
		get => states[0].layout;
		set => states[0].layout = value;
	}
	#endregion

	public GUIPainter painter { get; set; } = new DefaultButtonPainter();
	public VisualStyle style { get; set; } = new VisualStyle();
	public Sprite view { get; } = new Sprite();


	public Button() {
		_state = states[0];
		
	}
}

public class DefaultButtonPainter : GUIPainter {
	public override void paint(UIElement e, DisplayObject view) {
		throw new NotImplementedException();
	}
}


public class ButtonStates : IReadOnlyList<ButtonState> {
	private List<ButtonState> _all = new() {
		new ButtonState() { content = "Button" },
	};

	public ButtonState this[int index] => _all[index];
	public int Count => _all.Count;

	public ButtonState defaul {
		get => _all[0];
		set => _all[0] = value;
	}
	public ButtonStates others => this;

	public void Add(ButtonState s) {
		_all.Add(s);
		var b = new Button();
	}

	public IEnumerator<ButtonState> GetEnumerator() 
		=> _all.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator()
		=> _all.GetEnumerator();

	public override string ToString() {
		return $@"ButtonStates {Count} ";
	}
}

public class ButtonState {
	public object? content { get; set; }
	public LayoutSettings layout { get; set; } = new LayoutSettings();

	public ButtonState? highlighted { get; set; }
	public ButtonState? pressed { get; set; }

	public override string ToString() {
		return $"ButtonState({content})";
	}
}
