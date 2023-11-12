using System;
using System.Diagnostics.CodeAnalysis;

namespace audionysos.display;

public class EventDispatcher<T> : IEventDispatcher where T : Event {

	private Action<T>? capturingInvoke;
	private Action<T>? bubblingInvoke;
	private object? accessKey;

	public string? name { get; set; }

	public EventDispatcher([DisallowNull]Action<T> h, EventPhase p = EventPhase.BUBBLING, object? accessKey = null) {
		this.accessKey = accessKey;
		registerHandler(h, p);
	}

	protected EventDispatcher() { }

	public virtual Action<T>? getListeners(EventPhase p, object? accessKey = null) {
		if (accessKey != this.accessKey) throw new ArgumentException("Invalid access key given.");
		if (p.HasFlag(EventPhase.BUBBLING)) return bubblingInvoke;
		if (p.HasFlag(EventPhase.CAPTURING)) return capturingInvoke;
		throw new ArgumentException($"Invalid event phase specified ({p}).");
	}

	protected virtual EventDispatcher<T> registerHandler(Action<T> h, EventPhase p = EventPhase.BUBBLING) {
		if (p.HasFlag(EventPhase.BUBBLING)) bubblingInvoke
				= (Action<T>)Delegate.Combine(bubblingInvoke, h);
		if (p.HasFlag(EventPhase.CAPTURING)) capturingInvoke
				= (Action<T>)Delegate.Combine(capturingInvoke, h);
		return this;
	}

	public static EventDispatcher<T> addListener(EventDispatcher<T> d, Action<T> handler, EventPhase phase = EventPhase.BUBBLING)
		=> d.registerHandler(handler, phase) ?? new(handler, phase);

	public static EventDispatcher<T> operator +(EventDispatcher<T> d, Action<T> a)
		=> d?.registerHandler(a) ?? new (a);

	public override string ToString() {
		return $@"""{name}"" event dispatcher ({typeof(T).Name})";
	}
}
