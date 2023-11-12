using audionysos.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TD = audionysos.display.TempDispatcher;

namespace audionysos.display;
public class DisplayObjectInputEvents {
	#region static config
	public static DisplayObjectInputEvents initialized = new(null);

	static DisplayObjectInputEvents() {
		collectDispatcherProperties();
		initializeStaticDispatchers();
	}

	private static Dictionary<string, Func<object?, object?>> dispatchers = new();
	private static void collectDispatcherProperties() {
		var tt = typeof(DisplayObjectInputEvents);
		var ps = tt.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		var edt = typeof(IEventDispatcher);
		foreach (var p in ps) {
			if(edt.IsAssignableFrom(p.PropertyType))
				dispatchers.Add(p.Name, p.GetValue);
		}
	}
	#endregion

	#region Events
	private static void initializeStaticDispatchers() {
		initialized.POINTER_ENTER += (PointerEvent e) => { };
		initialized.POINTER_LEFT += (PointerEvent e) => { };
		initialized.POINTER_DOWN += (PointerEvent e) => { };
		initialized.POINTER_UP += (PointerEvent e) => { };
		initialized.POINTER_MOVE += (PointerEvent e) => { };
		initialized.KEY_DOWN += (KeyboardEvent e) => { };
		initialized.KEY_UP += (KeyboardEvent e) => { };
	}

	private EventDispatcher<PointerEvent>? _POINTER_ENTER;
	public EventDispatcher<PointerEvent> POINTER_ENTER {
		get => _POINTER_ENTER ?? TD.setting(_POINTER_ENTER, nd => _POINTER_ENTER = nd, accessKey);
		set => _POINTER_ENTER ??= value;
	}

	private EventDispatcher<PointerEvent>? _POINTER_LEFT;
	public EventDispatcher<PointerEvent> POINTER_LEFT {
		get => _POINTER_LEFT ?? TD.setting(_POINTER_LEFT, nd => _POINTER_LEFT = nd, accessKey);
		set => _POINTER_LEFT ??= value;
	}

	private EventDispatcher<PointerEvent>? _POINTER_DOWN;
	public EventDispatcher<PointerEvent> POINTER_DOWN {
		get => _POINTER_DOWN ?? TD.setting(_POINTER_DOWN, nd => _POINTER_DOWN = nd, accessKey);
		set => _POINTER_DOWN ??= value;
	}

	private EventDispatcher<PointerEvent>? _POINTER_UP;
	public EventDispatcher<PointerEvent> POINTER_UP {
		get => _POINTER_UP ?? TD.setting(_POINTER_UP, nd => _POINTER_UP = nd, accessKey);
		set => _POINTER_UP ??= value;
	}

	private EventDispatcher<PointerEvent>? _POINTER_MOVE;
	public EventDispatcher<PointerEvent> POINTER_MOVE {
		get => _POINTER_MOVE ?? TD.setting(_POINTER_MOVE, nd => _POINTER_MOVE = nd, accessKey);
		set => _POINTER_MOVE ??= value;
	}

	private EventDispatcher<KeyboardEvent>? _KEY_DOWN;// = new EventDispatcher<KeyboardEventHandler>();
	public EventDispatcher<KeyboardEvent> KEY_DOWN {
		get => _KEY_DOWN ?? TD.setting(_KEY_DOWN, nd => _KEY_DOWN = nd, accessKey);
		set => _KEY_DOWN ??= value;
	}

	private EventDispatcher<KeyboardEvent>? _KEY_UP;// = new EventDispatcher<KeyboardEventHandler>();
	public EventDispatcher<KeyboardEvent> KEY_UP {
		get => _KEY_UP ?? TD.setting(_KEY_UP, nd => _KEY_UP = nd, accessKey);
		set => _KEY_UP ??= value;
	}
	#endregion

	#region Helper methods
	public IEventDispatcher? getDispatcherByName(string name) {
		dispatchers.TryGetValue(name, out var getter);
		if (getter == null) return null;
		var d = getter(this);
		return d as IEventDispatcher;
	}

	public T? getDispatcher<T>(T d) where T : class, IEventDispatcher {
		if (d.name == null) throw new ArgumentException("Given event dispatcher don't have a name therefore equivalent dispatcher in this object cannot be determined.");
		var cd = getDispatcherByName(d.name);
		return cd as T;
	}
	#endregion


	private object? accessKey { get; }

	public DisplayObjectInputEvents(DisplayObject owner, object? accessKey = null) {
		this.accessKey = accessKey;

		//var ed = new EventsDispatcher(
		//	firePointerEnter: () => POINTER_ENTER?.Invoke(owner),
		//	firePointerLeft: () => POINTER_LEFT?.Invoke(owner),
		//	firePointerDown: () => POINTER_DOWN?.Invoke(owner),
		//	firePointerUp: () => POINTER_UP?.Invoke(owner),
		//	firePointerMove: (p) => POINTER_MOVE?.Invoke(owner, p),
		//	fireKeyDown: (k) => KEY_DOWN?.Invoke(k),
		//	fireKeyUp: (k) => KEY_UP?.Invoke(k)
		//);
		//dispatcherReceiver(ed);
		

		//XXX.addListener(keyHandler);
		//XXX += keyHandler;
		//XXX?.getListeners(EventPhase.BUBBLING, accessKey)?.Invoke(null);
	}

	//private void keyHandler(KeyboardEvent e) {
	//	EventPhase ph = default;
		
	//}
}

//public class EventsDispatcher {
//	public Action firePointerEnter { get; }
//	public Action firePointerLeft { get; }
//	public Action firePointerDown { get; }
//	public Action firePointerUp { get; }
//	public Action<DisplayPointer> firePointerMove { get; }
//	public KeyboardEventHandler fireKeyDown { get; }
//	public KeyboardEventHandler fireKeyUp { get; }

//	public EventsDispatcher(Action firePointerEnter, Action firePointerLeft
//		, Action firePointerDown, Action firePointerUp
//		, Action<DisplayPointer> firePointerMove
//		, KeyboardEventHandler fireKeyDown, KeyboardEventHandler fireKeyUp
//		)
//	{
//		this.firePointerEnter = firePointerEnter;
//		this.firePointerLeft = firePointerLeft;
//		this.firePointerDown = firePointerDown;
//		this.firePointerUp = firePointerUp;
//		this.fireKeyDown = fireKeyDown;
//		this.fireKeyUp = fireKeyUp;
//		this.firePointerMove = firePointerMove;
//	}

//}

public class PointerEvent : Event {
	new public DisplayObject target {
		get => (DisplayObject)base.target;
		init => base.target = value;
	}
	required public DisplayPointer pointer { get; init; }
	public char character { get; init; }
	public static implicit operator PointerEvent(DisplayPointer p)
		=> new PointerEvent() { pointer = p };
}

public class KeyboardEvent : Event {
	new public DisplayObject target {
		get => (DisplayObject)base.target;
		init => base.target = value;
	}
	required public Keyboard.Key key { get; init; }
	public char character { get; init; }
	public static implicit operator KeyboardEvent
		((Keyboard.Key k, char ch) t) => new KeyboardEvent() {
			key = t.k,
			character = t.ch,
		};
}

public abstract class Event {
	private object? _t;
	public object? target { get => _t; set => _t ??= value; }
	public object? currentTarget => _c.currentTarget;
	public EventPhase phase => _c.phase;
	private bool _h;
	public bool handled { get => _h;
		set {
			if (_h || value == false) return;
			_h = true;
			handler = _c.currentTarget;
		}
	}
	public object? handler { get; private set; }

	private bool _cn;
	public bool canceled {
		get => _cn;
		set {
			if (_cn || value == false) return;
			_cn = true;
			handler = _c.currentTarget;
		}
	}

	private EvenControl _c;
	public EvenControl control { set => _c ??= value; }

}

/// <summary><see cref="Event"/>'s variables that could be changed only by an events router.</summary>
public class EvenControl {
	public EventPhase phase { get; set; }
	public object? currentTarget { get; set; }
}

public class TempDispatcher {
	public static EventDispatcher<T> setting<T>(EventDispatcher<T>? target,
			Action<EventDispatcher<T>> setter, object? accessKey = null
		, [CallerMemberName]string? name = null) where T : Event
		=> TempDispatcher<T>.with(setter, accessKey, name);
}

public class TempDispatcher<T> : EventDispatcher<T> where T : Event {
	[ThreadStatic]
	private static TempDispatcher<T> td = new TempDispatcher<T>();
	//[MemberNotNull("setter")]
	public static TempDispatcher<T> with(Action<EventDispatcher<T>> d, object? accessKey, string? name) {
		td.setter = d ?? throw new ArgumentNullException(nameof(d));
		td.accessKey = accessKey;
		td.targetName = name;
		return td;
	}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	private TempDispatcher() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	private Action<EventDispatcher<T>> setter;
	private object? accessKey;
	private string? targetName;

	protected override EventDispatcher<T> registerHandler(Action<T> h, EventPhase p = EventPhase.BUBBLING) {
		var nd = new EventDispatcher<T>(h, p, accessKey);
		nd.name = targetName;
		setter(nd);
		return nd;
	}

	public override Action<T>? getListeners(EventPhase p, object? accessKey = null)
		=> null;

	public override string ToString() {
		return "￣\\_(ツ)_/￣";
	}
}

public interface IEventDispatcher {
	public string? name { get; set; }

}

[Flags]
public enum EventPhase {
	CAPTURING = 0b01,
	BUBBLING = 0b10, //default
	ALL = 0b11,
}

public static class EDExtension {
	public static EventDispatcher<T> addListener<T>(this EventDispatcher<T> d, Action<T> h)
		where T : Event
		=> d + h;

	public static EventDispatcher<T> addListener<T>(this EventDispatcher<T> d, Action<T> h, EventPhase p)
		where T : Event
		=> EventDispatcher<T>.addListener(d, h, p);
}