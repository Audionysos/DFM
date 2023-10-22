using System;
using System.Collections.Generic;

namespace com.audionysos.generics.containers; 
/// <summary>Provides convinient way for simple pooling of reusable objects of specific type.</summary>
/// <typeparam name="T"></typeparam>
public class ObjectsPool<T> where T : class {
	private Queue<T> all = new Queue<T>();
	private Func<T> constructor;
	/// <summary>Optional reseter action to that is invoked on each object take out of the pool.</summary>
	public Action<T> resetter;

	/// <summary>Creates new objects pool. Consturctor parameter is optional only for types that have a default constructors.</summary>
	/// <param name="constructor">Costructor function for creating new instance of <see cref="T"/> type.</param>
	public ObjectsPool(Func<T> constructor = null) {
		this.constructor = constructor;
		if (constructor != null) return;
		var cm = typeof(T).GetConstructor(Type.EmptyTypes);
		if (cm == null) throw new ArgumentNullException($@"No constructor method provided for object's pool and type ""{typeof(T).Name}"" has no default constructor.");
		this.constructor = () => Activator.CreateInstance<T>();
	}

	/// <summary>Creates new or takes free object from the pool.</summary>
	/// <returns></returns>
	public T take() {
		var o = all.Count == 0 ? constructor() : all.Dequeue();
		resetter?.Invoke(o);
		return o;
	}

	/// <summary>Stores given object in the pool so it can later be reused.</summary>
	public void take(T o) {
		all.Enqueue(o);
	}

}
