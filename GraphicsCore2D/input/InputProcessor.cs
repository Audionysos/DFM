using audionysos.display;
using audionysos.geom;
using audionysos.graphics.extensions.shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static audionysos.utils.Fantastatics;

namespace audionysos.input;
public abstract class InputProcessor {
	public FocusManager focus { get; private set; } = new FocusManager();

	public InputProcessor() {
		PinBoard.ip ??= this;
	}

	public abstract void registerInputListener(InputListener il);
	public abstract IPoint2 getSurfacePosition(DisplayPointer p, DisplaySurface s);

	public abstract bool isCurrentClipboard(object? o);
	public abstract object? getClipboard(Type t);
	public abstract void setClipboard(object? o);

	/// <summary>False if null.</summary>
	public static implicit operator bool([NotNullWhen(true)]InputProcessor? p) => p != null;

}

public class InputListener {
	private List<DisplaySurface> _surfs = new List<DisplaySurface>();

	//GC2D internal
	/// <summary>Register surface for input.</summary>
	/// <param name="surface"></param>
	public void registerSurface(DisplaySurface surface) {
		_surfs.Add(surface);
	}

	#region Temp/debug
	private IReadOnlyList<DisplayObject> noHit = new DisplayObject[0];
	public IReadOnlyList<DisplayObject> hit = new DisplayObject[0];
	private IReadOnlyList<DisplayObject> prevHit = new DisplayObject[0];
	#endregion

	/// <summary>Deepest object with positive hit test.</summary>
	private InteractiveObject? last => (hit.Count > 0 ? hit[0] : null) as InteractiveObject;

	public void pointerMove(InputProcessor ip, DisplayPointer dp) {
		hit = noHit;
		for (int i = 0; i < _surfs.Count; i++) {
			var s = _surfs[i];
			var sp = ip.getSurfacePosition(dp, s);
			dp.position.add(sp);
			var ht = s.hitTest(dp.position);

			//TODO: PointerLeft is fired when it enters a child which should not happen.
			if (ht != null) {
				hit = ht;
				var f = ht[0] as Sprite; Debug.Assert(f != null);
				var p = (prevHit.Count > 0 ? prevHit[0] : null) as InteractiveObject;
				if (p != f) {
					//p?.dispatcher.firePointerLeft();
					dispatch(events.POINTER_LEFT, p, prevHit, dp);
					prevHit = new List<DisplayObject>(hit);
					Debug.WriteLine($"PointerEnter {RapidTimeStamp}");
					//f.dispatcher.firePointerEnter();
					dispatch(events.POINTER_ENTER, f, ht, dp);
				} else {
					Debug.WriteLine($"PointerMoved {RapidTimeStamp}");
					//f.dispatcher.firePointerMove(dp);
					dispatch(events.POINTER_MOVE, f, ht, dp);
				}
			} else {
				hit = noHit;
				if (prevHit.Count > 0 && prevHit[0] is InteractiveObject p) {
					Debug.WriteLine($"PointerLeft {RapidTimeStamp}");
					//io.dispatcher.firePointerLeft();
					dispatch(events.POINTER_LEFT, p, prevHit, dp);
					prevHit = noHit;
				}
			}
			s.pointerMove(dp);
		}
	}

	public void pointerDown(InputProcessor ip, DisplayPointer dp) {
		dispatch(events.POINTER_DOWN, last, hit, dp);
	}

	public void pointerUp(InputProcessor ip, DisplayPointer dp) {
		dispatch(events.POINTER_UP, last, hit, dp);
	}

	public void keyDown(InputProcessor ip, Keyboard.Key k, char ch = '\0') {
		Keyboard.press(k);
		dispatch(events.KEY_DOWN, ip.focus.current, null, (k, ch));
	}

	public void keyUp(InputProcessor ip, Keyboard.Key k, char ch = '\0') {
		Keyboard.release(k);
		dispatch(events.KEY_UP, ip.focus.current, null, (k, ch));
	}

	private DisplayObjectInputEvents events = DisplayObjectInputEvents.initialized;
	private void dispatch<T>(EventDispatcher<T>? dis, object? target, IReadOnlyList<DisplayObject>? route
		, T e) where T : Event
	{
		if (target is not InteractiveObject t) return;
		if (route == null || route.Count == 0)
			route = t.tree.routeToRoot();
		if (dis == null) return;
		e.target = t;
		var ec = e.control = new EvenControl();
		
		ec.phase = EventPhase.CAPTURING;
		for (int i = route.Count-1; i >= 0; i--) {
			var ct = route[i] as InteractiveObject;
			if (!ct) continue;
			ec.currentTarget = ct;
			var cd = ct.input.getDispatcher(dis);
			cd?.getListeners(ec.phase, ct.dispatcherAccessKey)
				?.Invoke(e);
			if (e.canceled) return;
		}

		ec.phase = EventPhase.BUBBLING;
		for (int i = 0; i < route.Count; i++) {
			var ct = route[i] as InteractiveObject;
			if (!ct) continue;
			ec.currentTarget = ct;
			var cd = ct.input.getDispatcher(dis);
			cd?.getListeners(ec.phase, ct.dispatcherAccessKey)
				?.Invoke(e);
			if (e.canceled) return;
		}
	}
}

public class PointerTracker {
	public DisplayPointer p { get; }


	public PointerTracker(DisplayPointer p) {
		this.p = p;
	}

}

public class DisplayPointer {
	public int id { get; init; }
	public DisplayPointerType type { get; init; }
	private Point2 p = new Point2();
	public IPoint2 position {
		get => p.copy();
		set => p.set(value);
	}

}


public enum DisplayPointerType {
	UNKNOWN,
	MOUSE,
	FINGER,
	PEN,
	OTHER

}

public class Converters : IEnumerable<IConverter> {
	private List<IConverter> _all = new ();

	public void Add<I, O>(IConverter<I,O> c) {
		_all.Add(c);
	}

	public IConverter<Input, Output>? get<Input, Output>()
		=> get(typeof(Input), typeof(Output)) as IConverter<Input, Output>;

	public IConverter? get(Type from, Type to) {
		foreach (var c in _all) {
			if (c.inputType == from && c.outputType == to)
				return c;
		}
		return null;
	}

	public IEnumerator<IConverter> GetEnumerator()
		=> _all.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> _all.GetEnumerator();
}

public abstract class ConverterBase<From, To> : IConverter<From, To> {
	public Type inputType => typeof(From);
	public Type outputType => typeof(To);
	public virtual int grade => 0;

	public object convert(object input)
		=> convert((From)input);
	[return: NotNull]
	public abstract To convert(From o);
}

public interface IConverter<in From, out To> : IConverter {
	public To convert(From o);
}

public interface IConverter {
	Type inputType { get; }
	Type outputType { get; }
	/// <summary>Specifies number of intermediate converters.</summary>
	int grade { get; }

	public object convert(object input);
}