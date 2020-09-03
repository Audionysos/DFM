using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.IO.Path;
using static System.IO.File;
using com.audionysos.data.items;
using com.audionysos.files;
using System.Diagnostics;
using Markdig;
using HtmlAgilityPack;
using System.Xml.XPath;
using System.Threading;
using System.Threading.Tasks;
using com.audionysos.generics.diagnostinc;
using System.Collections.ObjectModel;
using System.Reflection;

namespace com.audionysos.data {

	public class DFM {
		/// <summary>Specifes root directory on which the dfm will operate.
		/// This can be either absolute or relative path. Note that when DFM scans the file system, it will change current directory (see <see cref="Directory.GetCurrentDirectory"/>) to the one you specified in this property.</summary>
		public string root { get; set; }
		private Pah rot;
		/// <summary>Items associated with the <see cref="root"/> directory.
		/// For this property to be available you must first call <see cref="Scan"/> method.</summary>
		public Item top { get; private set; }

		/// <summary>Loaded modules used in the system.</summary>
		private List<DFMModule> mods = new List<DFMModule>();
		/// <summary>Loaded modules used in the system.</summary>
		public ReadOnlyCollection<DFMModule> modules => mods.AsReadOnly();
		
		public DFMAccesor get { get; }
		/// <summary>Main item provider </summary>
		private ItemProvider itemProvder;

		public DFM(bool init = true) {
			get = new DFMAccesor(this);
			mods.Add(new DFMModule(this));
			if (init) initialize();
		}

		public void initialize() {
			constructItemProvider();
		}

		/// <summary>Creates main <see cref="itemProvder"/> out of all item providers of all modules.</summary>
		private void constructItemProvider() {
			if (mods.Count == 1) itemProvder = mods[0].itemProvider;
			if (mods.Count <= 1) return;
			var mp = itemProvder = new DefaultItemProvdier();
			foreach (var m in mods) {
				var ip = m.itemProvider; if (!ip) continue;
				mp.Add(ip);
			}
		}

		#region Scanning
		/// <summary>Scans files system for items at given <see cref="root"/> directory.</summary>
		public async void Scan() {
			setRootPath();
			var r = rot.fsRoot;

			top = Item.newTree(rot, itemProvder);
			var sits = Directory.EnumerateFileSystemEntries(root); //system items

			var st = new Stopwatch();
			//Task s = Scan3(st);
			await Scan3(st);
			//s = Scan3NoPreview(dip, st);
			//Scan2(dip, st);
			return;
		}

		private System.Threading.Tasks.Task Scan3(Stopwatch st) {
			st.Start();
			var s = top.Scan3(true);
			Console.WriteLine("Scanning");
			var l = Console.CursorTop;
			while (!s.IsCompleted) printCounts(top, l);
			printCounts(top, l);
			st.Stop();
			Console.WriteLine($"\nScan3 {st.ElapsedMilliseconds / 1000d}s");
			Console.WriteLine("----------------------------------------");
			return s;
		}

		private System.Threading.Tasks.Task Scan3NoPreview(DefaultItemProvdier dip, Stopwatch st) {
			System.Threading.Tasks.Task s;
			top = Item.newTree(rot, dip);
			st.Reset(); st.Start();
			s = top.Scan3(true);
			Console.WriteLine("Scanning");
			s.Wait();
			printCounts(top, Console.CursorTop);
			st.Stop();
			Console.WriteLine($"\nScan3 (no preview) {st.ElapsedMilliseconds / 1000d}s");
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("");
			return s;
		}

		private void Scan2(DefaultItemProvdier dip, Stopwatch st) {
			top = Item.newTree(rot, dip);
			st.Reset(); st.Start();
			top.Scan2(true);
			Console.WriteLine("Scanning");
			printCounts(top, Console.CursorTop);
			st.Stop();
			Console.WriteLine($"\nScan2 {st.ElapsedMilliseconds / 1000d}s");
			Console.WriteLine("----------------------------------------");
			Console.WriteLine("");
		}

		private void printCounts(Item it, int il) {
			var c = it.counters;
			Console.SetCursorPosition(0, il++);
			Console.Write($"Items found: {it.Count + it.subCount}");
			foreach (var t in c.Keys) {
				Console.SetCursorPosition(0, il++);
				Console.Write($"{t.Name}: {getCount(t, c)}");
			}
			//Console.SetCursorPosition(0, il);
			//Console.Write($"Items found: {i.Count + i.subCount}");
			//Console.SetCursorPosition(0, il + 1);
			//Console.Write($"Topics: {getCount<Topic>(c)}");
			//Console.SetCursorPosition(0, il + 2);
			//Console.Write($"Folders: {getCount<Folder>(c)}");
			//Console.SetCursorPosition(0, il + 3);
			//Console.Write($"Files: {getCount<FilE>(c)}");
			Thread.Sleep(40);
		}

		private int getCount<T>(Dictionary<Type, int> c)
			=> c.ContainsKey(typeof(T)) ? c[typeof(T)] : 0;
		private int getCount(Type t, Dictionary<Type, int> c)
			=> c.ContainsKey(t) ? c[t] : 0;
		#endregion

		private void setRootPath() {
			if (!Directory.Exists(root)) throw new Exception($@"Given root directory ""{root}"" does not exist.");
			Directory.SetCurrentDirectory(Combine(GetFullPath(root), ".."));
			rot = new Pah(GetFileName(root));
		}

		public void loadModule(string file) {
			//Assembly.lo
		}

		public void loadModule<M>() where M : DFMModule {
			var m = Activator.CreateInstance(typeof(M), this) as M;
			mods.Add(m);
		}

	}

	/// <summary>Class providing access to <see cref="DFM"/> features.</summary>
	public class DFMAccesor {
		internal DFM s;
		internal DFMAccesor(DFM s) => this.s = s;


		public T module<T>() where T : DFMModule
			=> s.modules.FirstOrDefault(m => m.GetType() == typeof(T)) as T;

	}

	/// <summary>Base class for external modules.</summary>
	public class DFMModule {

		public Item top { get; private set; }
		public DFM sytem { get; private set; }

		public virtual ItemProvider itemProvider { get; }

		public DFMModule(DFM sytem) {
			this.sytem = sytem;
		}

	}

	public class DefaultItemProvdier : ItemProvider {

		public DefaultItemProvdier(ItemProvider root = null) : base(root) {}

		protected override I create<I>(Pah p, ItemInfResolveContext c) {
			throw new NotImplementedException();
		}

		protected override Item createItem(Pah p) => null;
	}

}
