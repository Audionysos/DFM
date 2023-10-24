using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace audionysos.display;
public class DisplayObjectInputEvents {
	public Action<DisplayObject> POINTER_ENTER;
	public Action<DisplayObject> POINTER_LEFT;
	public Action<DisplayObject> POINTER_DOWN;
	public Action<DisplayObject> POINTER_UP;

	public DisplayObjectInputEvents(DisplayObject owner, Action<EventsDispatcher> dispatcherReceiver) {
		var ed = new EventsDispatcher(
			firePointerEnter: () => POINTER_ENTER?.Invoke(owner),
			firePointerLeft: () => POINTER_LEFT?.Invoke(owner),
			firePointerDown: () => POINTER_DOWN?.Invoke(owner),
			firePointerUp: () => POINTER_UP?.Invoke(owner)
		);
		dispatcherReceiver(ed);
	}
	
}

public class EventsDispatcher {
	public Action firePointerEnter { get; }
	public Action firePointerLeft { get; }
	public Action firePointerDown { get; }
	public Action firePointerUp { get; }

	public EventsDispatcher(Action firePointerEnter, Action firePointerLeft
		, Action firePointerDown, Action firePointerUp)
	{
		this.firePointerEnter = firePointerEnter;
		this.firePointerLeft = firePointerLeft;
		this.firePointerDown = firePointerDown;
		this.firePointerUp = firePointerUp;
	}

}
