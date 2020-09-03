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
