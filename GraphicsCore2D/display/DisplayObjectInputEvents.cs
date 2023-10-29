using audionysos.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace audionysos.display;
public class DisplayObjectInputEvents {
	public event Action<DisplayObject> POINTER_ENTER;
	public event Action<DisplayObject> POINTER_LEFT;
	public event Action<DisplayObject> POINTER_DOWN;
	public event Action<DisplayObject> POINTER_UP;
	public event KeyboardEventHandler KEY_DOWN;
	public event KeyboardEventHandler KEY_UP;

	public DisplayObjectInputEvents(DisplayObject owner, Action<EventsDispatcher> dispatcherReceiver) {
		var ed = new EventsDispatcher(
			firePointerEnter: () => POINTER_ENTER?.Invoke(owner),
			firePointerLeft: () => POINTER_LEFT?.Invoke(owner),
			firePointerDown: () => POINTER_DOWN?.Invoke(owner),
			firePointerUp: () => POINTER_UP?.Invoke(owner),
			fireKeyDown: (k) => KEY_DOWN?.Invoke(k),
			fireKeyUp: (k) => KEY_UP?.Invoke(k)
		);
		dispatcherReceiver(ed);
	}
	
}

public class EventsDispatcher {
	public Action firePointerEnter { get; }
	public Action firePointerLeft { get; }
	public Action firePointerDown { get; }
	public Action firePointerUp { get; }
	public KeyboardEventHandler fireKeyDown { get; }
	public KeyboardEventHandler fireKeyUp { get; }

	public EventsDispatcher(Action firePointerEnter, Action firePointerLeft
		, Action firePointerDown, Action firePointerUp
		, KeyboardEventHandler fireKeyDown, KeyboardEventHandler fireKeyUp)
	{
		this.firePointerEnter = firePointerEnter;
		this.firePointerLeft = firePointerLeft;
		this.firePointerDown = firePointerDown;
		this.firePointerUp = firePointerUp;
		this.fireKeyDown = fireKeyDown;
		this.fireKeyUp = fireKeyUp;
	}

}


public delegate void KeyboardEventHandler(KeyboardEvent e);
public class KeyboardEvent {
	required public DisplayObject target { get; init; }
	required public Keyboard.Key key { get; init; }
	public char character { get; init; }
}