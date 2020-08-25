using com.audionysos.console;
using com.audionysos.generics.diagnostinc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static com.audionysos.console.StaticConsole;
using static System.Reflection.GenericParameterAttributes;
using static com.audionysos.generics.reflection.TypeHelper;

namespace com.audionysos.generics.reflection {
	public class ReflContext {

		public object obj { get; private set; }
		public Type t { get; private set; }
		public string cc { get; set; } = "";

		public ReflContext(object baseObject) {
			obj = baseObject;
			t = obj.GetType();
		}

		private BindingFlags bf = BindingFlags.Public 
			| BindingFlags.Instance
			| BindingFlags.DeclaredOnly;
		public IEnumerable<string> getHints(string input = null) {
			var nch = splitNameChain(input??"");
			return navigate(nch, t);
		}

		public object execute(string input = null) {
			var nch = splitNameChain(input ?? "");
			navigate(nch, t, 0, StrCompare.ExactMatch);
			return null;
		}

		/// <summary>Returns all members names of last object in chain, for which string fragment at last chain element is marched using given comparer.</summary>
		/// <param name="nch"></param>
		/// <param name="t"></param>
		/// <param name="ind"></param>
		/// <returns></returns>
		private IEnumerable<string> navigate(string[] nch, Type t, int ind = 0 , StrComparition comparer = null) {
			if (t == null) return null;
			comparer = comparer ?? StrCompare.StartMatch;
			if (nch == null || nch.Length < 1) return currentMembers(null, "", comparer);
			if (ind == nch.Length - 1) return currentMembers(t, nch[ind], comparer);
			for (int i = ind; i < nch.Length-1; i++) {
				var ms = t.GetMember(nch[i], bf);
				if (ms.Length == 0) return null;
				if (ms.Length == 1) { var m = ms[0];
					if (filterMember(m)) return null;
					return navigate(nch, getResolvingType(m),  ++i, comparer);
				}
				return null;
				foreach (var m in ms) {
					if (filterMember(m)) continue;
				}
			}

			return null;
		}

		private IEnumerable<string> crawl(string[] nch, Type t, int ind = 0, StrComparition comparer = null) {
			if (t == null) return null;
			comparer = comparer ?? StrCompare.StartMatch;
			if (nch == null || nch.Length < 1) return currentMembers(null, "", comparer);
			if (ind == nch.Length - 1) return currentMembers(t, nch[ind], comparer);
			for (int i = ind; i < nch.Length - 1; i++) {
				var ms = t.GetMember(nch[i], bf);
				if (ms.Length == 0) return null;
				if (ms.Length == 1) {
					var m = ms[0];
					if (filterMember(m)) return null;
					return navigate(nch, getResolvingType(m), ++i, comparer);
				}
				return null;
				foreach (var m in ms) {
					if (filterMember(m)) continue;
				}
			}

			return null;
		}

		/// <summary>Get type produced by given memeber.</summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private Type getResolvingType(MemberInfo m) {
			if (m is PropertyInfo p) return p.PropertyType;
			if (m is FieldInfo f) return f.FieldType;
			if (m is MethodInfo h) return h.ReturnType; //this sould return method info properties - "()" not considered right now
			if (m is ConstructorInfo c) return c.DeclaringType;
			return null;
		}

		/// <summary>Returns memebers of given type which names match given inp string.</summary>
		/// <returns></returns>
		private IEnumerable<string> currentMembers(Type t = null, string inp = "", StrComparition comparer = null) {
			var mbs = (t??this.t).GetMembers(bf);
			comparer = comparer ?? score;
			var r = new HashSet<string>();
			foreach (var m in mbs) {
				var s = comparer(inp, m.Name);
				if (s <= 0) continue;
				if (filterMember(m)) continue;
				r.Add(m.Name);
			}
			return r;
		}

		private object enter(object o, string name = "", object[] fArgs = null, Type[] gTypes = null) {
			//var mbs = o.GetType().GetMember(name, bf);
			//if (mbs.Length == 1) return enterMember(o, mbs[0]);
			//object cho;
			//foreach (var m in mbs) {
			//	if (s <= 0) continue;
			//	if (filterMember(m)) continue;
			//	r.Add(m.Name);
			//}
			//return cho;
			return null;
		}

		/// <summary>Return child object using given child member.</summary>
		/// <param name="o">Parent object from which to get value.</param>
		/// <param name="memberInfo">Info of mamber of parent object (you need to be sure this member belongs to parent object type).</param>
		private (object, Issue) enterMember(object o, MemberInfo m, object[] fArgs, Type[] gTypes) {
			if (m is PropertyInfo p) return (p.GetValue(o), null);
			if (m is FieldInfo fd) return (fd.GetValue(o), null);
			if (m is MethodInfo f) {
				var e = new MethodExecutor(o, f, fArgs, gTypes);
				if (e.issues >= Impact.DANGEROUS) return (null, e.issues);
			}
			return (null, new Issue($@"Given member type ""{m.MemberType}"" is not supported.", Impact.CRITICAL));
		}

		


		private double score(string inp, string name) {
			if (name.StartsWith(inp, StringComparison.InvariantCultureIgnoreCase)) return 1;
			//TODO: Scoring method
			return 0;
		}

		/// <summary>Examines if given memeber should be included in result (returns true if memeber should be ommited).</summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool filterMember(MemberInfo m) {
			var p = m as MethodInfo;
			if (p != null && p.IsSpecialName) return true;
			if (m is ConstructorInfo) return true;
			return false;
		}

		private string[] splitNameChain(string input) {
			//return input.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			return input.Split(".".ToCharArray());
		}
	}

	/// <summary>Represents strings comparition method.
	/// The method should return 1 if test string exactly matches target string and 0 if no match was found at all.
	/// The mathod may return intermediate values to indicate that test string matches target string to some extend but not completelly.
	/// Terms "exact/no match" used here are contractual and are defined strictly at specific method implementation.
	/// Browse <see cref="StrCompare"/> class for common comparitions methods.</summary>
	public delegate double StrComparition(string test, string target);

	public static class StrCompare {
		/// <summary>Returns 1 only if the strings are exactly the same (case sensitive) and 0 in any other case.</summary>
		public static double ExactMatch(string test, string target) => test == target ? 1 : 0;

		/// <summary>Returns 1 if target string starts exactly with test string (case insensitive) and 0 in any other case.</summary>
		public static double StartMatch(string test, string target) {
			if (target.StartsWith(test, StringComparison.InvariantCultureIgnoreCase)) return 1;
			return 0;
		}
	}
}
