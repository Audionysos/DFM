using System;
using System.Collections.Generic;
using System.Linq;
using com.audionysos.files;

namespace com.audionysos.data.items; 
public abstract class ItemProvider {
	#region Dynamic
	private List<Type> types = new List<Type>();

	public void registerType<T>() where T : ItemInterface {
		types.Add(typeof(T));
	}

	public void registerType(Type t) {
		if (!ItemInterface.type.IsAssignableFrom(t)) throw new ArgumentException($@"Given type must be ""{t}"" is not ""{ItemInterface.type}"".");
		types.Add(t);
	}
	#endregion

	/// <summary>Root provider this one is descendant of.</summary>
	public ItemProvider root { get; private set; }
	private List<ItemProvider> subs = new List<ItemProvider>();

	public ItemProvider(ItemProvider root) {
		this.root = root != null ? root : this;
	}


	/// <summary>Adds child item provider.</summary>
	/// <param name="ip"></param>
	public void Add(ItemProvider ip) {
		if (ip == null) throw new ArgumentException("ItemProvider cannot be null");
		subs.Add(ip);
	}

	/// <summary>Returns child item provider of specifed type or null if provider was not found.</summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T get<T>() where T : ItemProvider {
		if (this is T ip) return ip; 
		var pt = typeof(T);
		ip = subs.FirstOrDefault(p => p.GetType() == pt) as T;
		if (ip) return ip as T;
		foreach (var i in subs) {
			ip = i.get<T>();
			if (ip) return ip;
		}return null;
	}

	/// <summary>Creates item for entry at given path or returns null if no items were recognized.</summary>
	/// <param name="p"></param>
	/// <returns></returns>
	public Item create(Pah p) {
		var i = createItem(p);
		if (i) return i;
		foreach (var ip in subs) {
			i = ip.create(p);
			if (i) return i;
		}
		return null;
		foreach (var t in types) {

		}
	}

	public I getItefrace<I>(Pah p) where I : ItemInterface {
		var c = new ItemInfResolveContext(this, p);
		return getItefrace<I>(c);
	}

	//private Dictionary<Type, Type> qued = new Dictionary<Type, Type>();
	private I getItefrace<I>(ItemInfResolveContext c) where I : ItemInterface {
		//var t = typeof(I);
		//if (qued.ContainsKey(t)) throw new InvalidItemsProviderException($@"Circular dependency detected. {}");
		//qued.Add(t, this.GetType());
		//var i = create<I>(p);
		//if()
		return null;
	}


	/// <summary>Create item interface for entry at given path.
	/// You can use context to access other interfaces if the target interface existance depends on them.
	/// Do not call any other base methods from this one.
	/// Note that even if you can get other interface from the context, not all it's properties have to be available at this point so use it with caution.</summary>
	/// <param name="p"></param>
	/// <returns></returns>
	protected abstract I create<I>(Pah p, ItemInfResolveContext c) where I : ItemInterface;

	/// <summary>Create item at specifed path or return null if entry at given path is not an item of provided type.</summary>
	/// <param name="p"></param>
	/// <returns></returns>
	protected abstract Item createItem(Pah p);

	public static implicit operator bool(ItemProvider ip) => ip!=null;

	/// <summary>Context for resolving item interface.</summary>
	protected class ItemInfResolveContext {
		/// <summary>Resolved interfaces</summary>
		List<ItemInterface> res = new List<ItemInterface>();
		ItemProvider root;
		Pah pah;

		internal ItemInfResolveContext(ItemProvider root, Pah itemPath) {
			this.root = root;
			this.pah = itemPath;
		}

		/// <summary>Retruns some other interface resolved for this item.</summary>
		/// <typeparam name="I"></typeparam>
		/// <returns></returns>
		public I get<I>() where I : ItemInterface {
			var t = typeof(I);
			var r = res.FirstOrDefault(i => i.GetType() == t);
			if (r) return r as I;
			root.getItefrace<I>(this);
			return null;
		}
	}
}
