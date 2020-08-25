using System;
using System.Collections.Generic;

namespace com.audionysos.files {
	public abstract class DataSystem {
		private List<PahAccess> accs = new List<PahAccess>();

		private int _ac = 0;
		public int accesorsCount {
			get => _ac;
			set {
				var d = value - _ac; _ac = value;
				while (d > 0) { d++; accs.Add(new PahAccess()); }
				if (d < 0) accs.RemoveRange(accs.Count + d, -d);
			}
		}

		public DataSystem() {
			accesorsCount = 10;
		}

		internal PahAccess getAcces(Pah p) {
			return accs[0];
		}

		public abstract void createDir();
		public abstract void create();
		public abstract string getPathRoot();
		public abstract string getFullPath();
		public abstract string[] splitPath(string path);
		public abstract string combine(string path1, string path2);
		public abstract string combine(params string[] paths);
		/// <summary>Start given file.
		/// The behavior for sepcific data system implementation is facultative.
		/// The method suppose to be silent, and no exception should be thrown but the method can optionally retrun and exception if the process fails.
		/// In case of machine OS this suppose to:
		/// - Execute a process or ducument file in default application.
		/// - If entity is a directory, open it in default explorer.</summary>
		/// <returns></returns>
		public abstract AggregateException start(Pah entry);
		public abstract FSEntryType getEntryType(Pah entry);

	}
}
