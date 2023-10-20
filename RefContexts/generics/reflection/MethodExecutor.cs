using com.audionysos.generics.diagnostinc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.GenericParameterAttributes;
using static com.audionysos.generics.reflection.TypeHelper;

namespace com.audionysos.generics.reflection {
	public class MethodExecutor {
		#region Arguments
		/// <summary>Parent object on which to invoke the <see cref="call"/>.</summary>
		private object o;
		/// <summary>Type of method owner object.</summary>
		private Type ot;
		/// <summary>Initial method provide to constructor.</summary>
		private MethodInfo f;
		/// <summary>Function call arguments.</summary>
		private object[] fArgs;
		/// <summary>Rates for function call arguments which indicted to what extend each arguments is matched with method's parameters definition.</summary>
		private double[] ars;

		/// <summary>Explicitly specified generic arguments.</summary>
		private Type[] gTypes;
		#endregion

		/// <summary>List of initial method's generic types.</summary>
		private Type[] gts;
		/// <summary>inffered generic types.</summary>
		private Type[] its;
		/// <summary>Stores indexes of arguments from which corresponding generic type in "its" array was inferred.</summary>
		private int[][] ias;

		#region Public 
		/// <summary>Final method that will be invoked using <see cref="call"/> - if method provided in constructor is generic this will be result of type inference that can be safely called with specified argument types.</summary>
		public MethodInfo method { get; private set; }
		public Issue issues { get; private set; }

		public MethodExecutor(object o, MethodInfo m, object[] fArgs = null, Type[] gTypes = null) {
			this.o = o;
			ot = o.GetType();
			f = m as MethodInfo;
			this.fArgs = fArgs??new object[0];
			this.gTypes = gTypes??new Type[0];

			gts = f.GetGenericArguments();
			its = new Type[gts.Length];
			ias = new int[gts.Length][];

			checkExplicitGenericArgumets();
			checkArguments();
			produceMethod();
		}

		public (object r, bool retruned)  call() {
			if (issues?.impact >= Impact.DANGEROUS) throw new Exception($@"The method cannot be called as the are some serious issues with the arguments. Check ""{nameof(issues)}"" property to before calling the method.");
			if (method.ReturnType == null)
				return (method.Invoke(o, fArgs), false);
			return (method.Invoke(o, fArgs), true);
		}
		
		public string signature {
			get {
				if (issues) return issues.message;
				var m = method;
				if (m == null) return "No issues were found but method signature was not yet determined";
				//Method assumed OK here
				var acc = "";
				if (m.IsAbstract) acc = "public";
				else if (m.IsPrivate) acc = "private";
				else if (m.IsAssembly) acc = "internal";
				else if (m.IsFamily) acc = "protected";

				var s = $"{acc} {If(m.IsStatic, "static")} " +
						$"{IfElse(m.ReturnType!=null,m.ReturnType.Name,"void")} " +
						$"{m.Name}";
				if(its.Length > 0) {
					s += "<";
					foreach (var gt in its) s += gt.Name + ", ";
					s = s.Substring(0, s.Length - 2);
					s += ">";
				}
				s += "(";
				foreach (var p in m.GetParameters()) {
					if (p.IsIn) s += "in ";
					else if (p.IsOut) s += "out ";
					else if (p.ParameterType.IsByRef) s += "ref ";
					if (p.IsDefined(typeof(ParamArrayAttribute))) s += "params ";
					s += p.ParameterType.Name;
					if (p.HasDefaultValue) s += " = " + (p.DefaultValue??"null").ToString();
					s += ", ";
				}
				s = s.Substring(0, s.Length - 2);
				return s + ")";
			}
		}

		private string If(bool condition, string s) => condition ? s : "";
		private string IfElse(bool condition, string s, string a) => condition ? s : a;

		#endregion

		private (MethodInfo m, Issue issue) produceMethod() {
			if (gts.Length == 0) return (f, null);
			for (int i = 0; i < its.Length; i++) {
				var it = its[i];
				if (it == null) return (null, new Issue($@"Type for generic type argument ""{gts[i]}"" was not inferred."));
			}return (f.MakeGenericMethod(its), null);
		}

		/// <summary>Index of generic type for currently examined function argument</summary>
		private int gi = -1;
		/// <summary>Type of generic argument that is used for currently examined function argument.</summary>
		private Type gt;
		/// <summary>Current argument index.</summary>
		private int ai = -1;
		/// <summary>Current argument object.</summary>
		private object ao;
		/// <summary>Current argument object type.</summary>
		private Type at;
		/// <summary>Currently examined parameter.</summary>
		private ParameterInfo pr;
		/// <summary>Currently examined parameter type.</summary>
		private Type pt;

		private Issue checkArguments() {
			var pars = f.GetParameters();
			foreach (var pr in pars) {
				ai = pr.Position;
				var a = getArgumentAt(pr.Position);
				if (!a.provided && !pr.IsOptional) return new Issue($@"Required function argument ""{pr.Name}"" was not specified.", Impact.CRITICAL);
				ao = a.o;
				pt = pr.ParameterType;
				var iss = infferGenericType();
				if (iss) return iss;
			}
			return null;
		}

		/// <summary>Assume generic type substitution for currently examined parameter (it was defined as generic type).
		/// return null in no issues were found.</summary>
		/// <returns></returns>
		private Issue infferGenericType() {
			gi = getGenericArgumentIndex(pt); //Check if argument need to be substituted for generic type argument
			if (gi < 0) return null;
			gt = gts[gi];
			at = ao.GetType();
			if (its[gi] != null) {//Generic type is assumed already
				if (updateGenericType() is Issue i) return i;
			}
			//Generic type was not assumed yet
			var sr = rateGenericTypeSubstitution(at, gt);
			if (sr.v > 0) {
				its[gi] = at;
				updateInfferedArgumentsIndices();
			} else return new Issue($@"Type of object given as ""{pr.Name}"" argument, does not fulfill generic type constrain ""{gt}"".", Impact.CRITICAL, sr.issue);
			return null;
		}

		/// <summary>Examines compatibility of current argument type with already provided or assumed generic substitution type in <see cref="its"/> array.
		/// If current argument type is not compatible with provided or assumed type substitution, new substitution type will be searched for, that is common for all arguments of current generic type.
		/// Method returns null if substitution type is correct or new one was found successfully.</summary>
		/// <returns></returns>
		private Issue updateGenericType() {
			var r = rateTypeMatch(at, its[gi]);
			if (r == 0) {
				//Generic type argument was provided explicitly
				if (ias[gi] == null) return new Issue($@"Object given as ""{pr.Name}"" argument is not of type specified in generic argument ""{its[gi]}"".", Impact.CRITICAL);
				//type was assumed from previous arguments
				updateInfferedArgumentsIndices();
				var ct = findCommonType(extract(fArgs, ias[gi], to => to.GetType()));
				var ctr = rateGenericTypeSubstitution(ct, gt);
				if (ctr.v == 0) return new Issue($@"Substitution type ""{ct}"" inffered from arguments for generic type argument ""{gt}"" does not fulfill it's constrains.", Impact.CRITICAL, ctr.issue);
				its[gi] = ct;
			}
			ars[pr.Position] = r;
			return null; //Given argument object is OK
		}



		#region Helper functions
		/// <summary>Adds index of current function argument to list indexes from which current generic type was inffered.</summary>
		private void updateInfferedArgumentsIndices() {
			if (ias[gi] == null) { ias[gi] = new int[] { ai }; }
			var nis = new int[ias.Length + 1];
			ias[gi].CopyTo(nis, 0); nis[ias.Length] = ai;
			ias[gi] = nis;
		}

		/// <summary>Check provided generic types arguments in terms of constrains compatibility and set all compatible types in inffered types array <see cref="its"/>.</summary>
		private void checkExplicitGenericArgumets() {
			for (int i = 0; i < gTypes.Length; i++) { //check explicit generic arguments (some types may be skipped so no return here)
				var r = rateGenericTypeSubstitution(gTypes[i], gts[i]);
				if (r.v > 0) its[i] = gTypes[i];
			}
		}

		/// <summary>Find if given type is on of generic types arguments of initial method definition.</summary>
		/// <param name="pt">Parameter type to test if it is generic parameter</param>
		/// <returns></returns>
		private int getGenericArgumentIndex(Type pt) {
			for (int i = 0; i < gts.Length; i++)
				if (pt == gts[i]) return i;
			return -1;
		}

		/// <summary>Returns argument at given position (if position does not exceed provided arguments array.</summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private (object o, bool provided) getArgumentAt(int p) {
			if (p >= fArgs.Length) return (null, false);
			else return (fArgs[p], true);
		}

		private R[] extract<T, R>(T[] a, int[] inds, Func<T, R> transform) {
			var r = new R[inds.Length]; var ri = 0;
			foreach (var i in inds) r[ri++] = transform(a[i]);
			return r;
		}
		#endregion
	}
}
