using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.IO.Path;
using static System.IO.File;
using static com.audionysos.files.FSEntryType;
using System.Diagnostics;

namespace com.audionysos.files {
	public class Pah {
		#region Properties
		/// <summary>A files system root. This is absolute bottom where relative path can refer.</summary>
		public Pah fsRoot {
			get {
				var r = parent; var ar = Path.GetPathRoot(full);
				while (r.full != ar) {
					r = r.parent;
				} return r;
			}
		}
		/// <summary>Returns full route from system root to this entry (this entry is included).</summary>
		public List<Pah> route {
			get {
				var r = parent; var ar = Path.GetPathRoot(full);
				var ro = new List<Pah>() { this, r };
				while (r.full != ar) {
					r = r.parent;
					ro.Add(r);
				}ro.Reverse();
				return ro;
			}
		}

		/// <summary>Full system path to this entry.</summary>
		public string full => GetFullPath(Combine(rel, routeFromWD));
		/// <summary>The first entry name from were this pahts starts - this is relative root, not the file system root.</summary>
		public Pah root { get; private set; }
		private Pah _par;
		public Pah parent {
			get {
				if (_par != null) return _par;
				var pp = GetFullPath(Combine(full, ".."));
				_par = new Pah(pp, Combine(routeFromWD, ".."));
				_par.Add(this);
				return _par;
			}
			private set {
				_par = value;
			}
		}
		/// <summary>Returns the entry's file system path relative to <see cref="root"/> if the entry exist or null otherwise.</summary>
		public string sys {
			get {
				var p = rel;
				return (Directory.Exists(p) | Exists(p)) ? p : null;
			}
		}
		public bool isAbsolute => throw new NotImplementedException();
		
		/// <summary>Path relative to <see cref="root"/>.</summary>
		public string rel =>
			root == this ? name : Combine(parent, name);

		/// <summary>Name of this file or direcotry.</summary>
		public string name { get; }

		/// <summary>Type of file system entry.</summary>
		public FSEntryType type {
			get {
				var p = rel;
				if (Exists(p)) return FILE;
				if (Directory.Exists(p)) return FOLDER;
				return NON_FSENTRY;
			}
		}
		/// <summary>Tells if this entry exist in file system.</summary>
		public bool exist => type != NON_FSENTRY;

		private List<Pah> subs = new List<Pah>();
		/// <summary>Route to <see cref="root"/> path from current working directory.</summary>
		public string routeFromWD { get; private set; }
		#endregion

		#region Construction
		public Pah(string root, string routeFromWD = "") {
			this.root = this;
			this.routeFromWD = routeFromWD;
			//parent = this;
			name = GetFileName(root);
		}

		public Pah(Pah parent, string name) {
			this.root = parent.root;
			this.parent = parent;
			this.routeFromWD = parent.routeFromWD;
			this.name = name;
		}

		public Pah Add(string name) {
			var ch = new Pah(this, name);
			subs.Add(ch); return ch;
		}

		public Pah Add(Pah child) {
			//TODO: Make clone probably istead of exception
			if (child.root != child) throw new Exception("You can only add rooted paths as a child.");
			subs.Add(child); child.parent = this;
			return child;
		}
		#endregion

		#region Routing
		public string routeTo(Pah o) {
			var tr = route; var or = o.route;
			if (tr[0] != or[0]) return o.full;
			var i = 0;
			while (i < tr.Count && i < or.Count && tr[i] == or[i])  i++;
			var bck = ""; var frw = ""; var si = i;
			while (i++ < tr.Count) bck += @"..\"; i = si;
			while (i < or.Count) frw += @"\"  + or[i++].name;
			return Combine(bck, frw.Substring(1));
		}

		/// <summary>Returns route to specifed path from current one. If target entry is on the same <see cref="fsRoot"/>, returned path will be relative.</summary>
		/// <param name="path">Path to find route to. The string will be joined into current <see cref="Pah"/> system tree.</param>
		/// <returns></returns>
		public string routeTo(string path) {
			throw new NotImplementedException();
		}


		#endregion

		#region Operations
		public Exception create(FSEntryType t = FILE) {
			if(t == NON_FSENTRY) throw new ArgumentException("Invalid entry type specifed.");
			try {
				if (t == FILE) Create(this).Close();
				else if (t == FOLDER) Directory.CreateDirectory(this);
			}catch (Exception e) { return e; }
			return null;
		}

		/// <summary>Creates this file entry if it already don't exist in the stystem.</summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Exception ensure(FSEntryType t = FILE) {
			if (exist) return null;
			return create(t);
		}

		public void start() {
			Process.Start(new ProcessStartInfo(this) { UseShellExecute = true });
		}

		public string text() => ReadAllText(this);

		#endregion

		#region Conversions
		public override string ToString() {
			return this.rel;
		}

		public IEnumerable<Pah> scan {
			get {
				if (type != FOLDER) yield break;
				string[] es = null; 
				try {
					es = Directory.GetFileSystemEntries(this);
				} catch (UnauthorizedAccessException u) {
					yield break;
				}
				foreach (var e in es) {
					yield return Add(GetFileName(e));
				}
			}
		}

		public static implicit operator string(Pah p) => p.rel;
		public static implicit operator Pah(string p) => new Pah(p);
		#endregion
	}



	/// <summary>File system entry type.</summary>
	public enum FSEntryType {
		FILE,
		FOLDER,
		/// <summary>Type entry was not found in file system.</summary>
		NON_FSENTRY,
	}


	public sealed class PahAccess {
		internal Pah p { get; set; }

		internal PahAccess() {}
	}
}
