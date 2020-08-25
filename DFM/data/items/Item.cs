using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.audionysos.files;
using static com.audionysos.files.FSEntryType;

namespace com.audionysos.data.items {
	public abstract class Item {

		#region Statics
		public static Item newTree(Pah path, ItemProvider ip) {
			return newItem(path, ip);
		}

		private static Item newItem(Pah p, ItemProvider ip) {
			Item si = ip.create(p);
			return si;
		}
		#endregion

		#region Properties
		/// <summary>Base type of interface for this item.</summary>
		public abstract Type type { get; protected set; }
		/// <summary>Path to the item in file system.</summary>
		public Pah path { get; private set; }
		/// <summary>Item interface object.</summary>
		public ItemInterface inface { get; private set; }
		/// <summary>Parent <see cref="ItemProvider"/> which produced this item.</summary>
		public ItemProvider ip { get; private set; }
		public Item parent { get; private set; }

		public int Count => subs.Count;
		#region SubCouting
		public Dictionary<Type, int> counters = new Dictionary<Type, int>(); 
		private object cLock = new object();
		private int _sc = 0;
		public int subCount => _sc;
		private void inCounter(Type t) {
			lock (cLock) {
				if (!counters.ContainsKey(t)) counters.Add(t, 1);
				counters[t]++;
				_sc++;
			}
			if (parent) parent.inCounter(t);
		}
		#endregion

		private List<Item> subs = new List<Item>();
		#endregion

		internal Item(Pah path, Type type, ItemProvider ip) {
			if (!ItemInterface.type.IsAssignableFrom(type)) throw new ArgumentException("");
			this.type = type;
			this.path = path;
			this.ip = ip;
			inface = Activator.CreateInstance(type, new object[] { this }) as ItemInterface;
		}

		#region Additon
		public Item Add(Pah p) {
			var i = ip.create(p);
			if (!i) throw new ArgumentException($@"Could find item at given adress ""{p}"" ");
			subs.Add(i); i.parent = this;
			if (parent) parent.inCounter(i.type);
			return i;
		}

		public Item Add(string name) {
			return Add(path.Add(name));
		}
		#endregion

		#region Scanning
		public async Task Scan(bool recursive = false) {
			var t = new Task(() => {
				foreach (var p in path.scan) Add(p);
				if (!recursive) return;
				var tks = new Task[subs.Count]; var i = 0;
				foreach (var s in subs) tks[i++] = s.Scan(true);
				Task.WaitAll(tks);
			}); t.Start();
			await t;
		}

		public void Scan2(bool recursive = false) {
			foreach (var p in path.scan) Add(p);
			if (!recursive) return;
			foreach (var s in subs) s.Scan2(true);
		}

		private static TaskFactory tf = new TaskFactory();

		public async Task Scan3(bool recursive = false) {
			await Task.Run(async () => {
				foreach (var p in path.scan) Add(p);
				if (!recursive) return;
				var tks = new Task[subs.Count]; var i = 0;
				foreach (var s in subs) tks[i++] = s.Scan3(true);
				await Task.WhenAll(tks);
				//await Task.WhenAll(subs.Select(s => Scan3(s)));
			});
		}
		#endregion

		public IEnumerable<T> Enum<T>() where T : ItemInterface {
			foreach (var i in subs) if(i.inface is T) yield return i as T;
			foreach (var i in subs) {
				foreach (var m in i.Enum<T>()) yield return m;
			}yield break;
		}


		#region Coversions
		public override string ToString() {
			return $@"{path} ({type.Name} Item)";
		}


		public T aS<T>() where T : ItemInterface => inface as T;

		public static implicit operator bool(Item i) => i!=null;
		#endregion
	}

	public class Item<T> : Item where T : ItemInterface {
		public override Type type { get; protected set; } = typeof(T);

		public Item(Pah path, ItemProvider ip) : base(path, typeof(T), ip) {}

	}

	#region Item Interface
	public class ItemInterface {
		public static readonly Type type = typeof(ItemInterface);

		/// <summary>Parent item of this interface instance.</summary>
		public Item item { get; private set;}

		public ItemInterface(Item i) {
			this.item = i;
		}

		public static implicit operator bool(ItemInterface t) => t != null;
	}

	

	public class Folder : ItemInterface { public Folder(Item i) : base(i) { } }
	public class FilE : ItemInterface { public FilE(Item i) : base(i) { } }
	#endregion

	

	public class SystemFileItemsProvider : ItemProvider {
		public SystemFileItemsProvider(ItemProvider root = null) : base(root) {}

		protected override I create<I>(Pah p, ItemInfResolveContext c) {
			throw new NotImplementedException();
		}

		protected override Item createItem(Pah p) {
			Item si = null;
			if (p.type == FILE) si = new Item<FilE>(p, root);
			else if (p.type == FOLDER)  si = new Item<Folder>(p, root);
			//} else throw new ArgumentException($@"The path ""{p}"" was not recognized in file system.");
			return si;
		}
	}

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

	


	[Serializable]
	public class InvalidItemsProviderException : Exception {
		public InvalidItemsProviderException() { }
		public InvalidItemsProviderException(string message) : base(message) { }
		public InvalidItemsProviderException(string message, Exception inner) : base(message, inner) { }
		protected InvalidItemsProviderException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

}
