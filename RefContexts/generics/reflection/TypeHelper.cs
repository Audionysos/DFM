using com.audionysos.generics.diagnostinc;
using System;
using System.Collections.Generic;
using static System.Reflection.GenericParameterAttributes;

namespace com.audionysos.generics.reflection {
	public static class TypeHelper {

		/// <summary>Determine if given type can be substituted for given generic type based on generic type constrians.
		/// Variance flags are not considered here.</summary>
		/// <param name="t">Type to match</param>
		/// <param name="g">Generic type template.</param>
		public static (double v, Issue issue) rateGenericTypeSubstitution(Type t, Type g) {
			var cts = g.GetGenericParameterConstraints();
			if (cts.Length == 0) return (1, null);
			var r = rateTypeMatch(t, cts[0]);
			if (r == 0) return (0, new Issue($@"Type ""{t}"" does not inherit from ""{g}"".", Impact.CRITICAL));
			for (int i = 1; i < cts.Length; i++)
				if (rateTypeMatch(t, cts[i]) == 0) return (0, new Issue($@"Type ""{t}"" does not inherit from ""{g}"".", Impact.CRITICAL));

			var ats = g.GenericParameterAttributes;
			if (ats.HasFlag(None)) return (r, null);
			if (ats.HasFlag(NotNullableValueTypeConstraint) && !t.IsValueType) return (0, new Issue($@"Type ""{t}"" is not a value type.", Impact.CRITICAL));
			if (ats.HasFlag(ReferenceTypeConstraint) && t.IsValueType) return (0, new Issue($@"Type ""{t}"" is not a reference Type.", Impact.CRITICAL));
			if (ats.HasFlag(DefaultConstructorConstraint) && t.GetConstructor(Type.EmptyTypes) == null) return (0, new Issue($@"Type ""{t}"" does not provide default constructor.", Impact.CRITICAL));
			//TODO: Enusure I don't need to check variance flags for dynamic method invocation.
			return (r, null);
		}


		/// <summary>Rate type match of given type <paramref name="t"/> to base type <paramref name="b"/>.
		/// Returned value is just simply 1 divided by depth of inheritance.
		/// So if <paramref name="t"/> is exctly <paramref name="b"/>, or <paramref name="b"/> is an interface which is implmented by <paramref name="t"/>, 1 is returned.
		/// If <paramref name="t"/> directrly inherits form <paramref name="b"/>, returned value will be 0.5.
		/// If <paramref name="t"/> inherits from SomeClass which directly inherits from <paramref name="b"/>, returned value will be 0.3(3).
		/// And so one.
		/// If <paramref name="t"/> does not inherits from <paramref name="b"/>, 0 will be returned.</summary>
		/// <param name="t"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double rateTypeMatch(Type t, Type b) {
			if (b == null) return 0;
			if (b.IsInterface && b.IsAssignableFrom(t)) return 1;
			var d = 0; while (t != null) {
				d++; t = t.BaseType;
				if (t == b) return 1 / d;
			}return 0;
		}

		/// <summary>Find common ancestor type for given set of types.</summary>
		/// <param name="ts"></param>
		/// <returns></returns>
		public static Type findCommonType(params Type[] ts) {
			if (ts == null || ts.Length < 1) return typeof(object);
			(Type t, double r) ct = (ts[0], 0);
			for (int i = 0; i < ts.Length - 1; i++) {
				ct = findCommonType(ct.t, ts[i + 1]);
				if (ct.t == typeof(object)) return typeof(object);
			}return ct.t;
		}

		/// <summary>Find common ancestor for given two types and rates type similarity based on how much types need to be degraded to meet base ancestor.
		/// If given types are the same type rate will be 1. If given types don't share common ancestor it will be 0 (theoretically).
		/// Itermediate value is calculted using formula (1 / (ad + bd  +1)) where ad and bd are types degradations (depths of inheritance from common ancestor).</summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static (Type t, double r) findCommonType(Type a, Type b) {
			if (a == b) return (a, 1);
			var ach = new List<Type>(); while (a != null) { ach.Add(a); a = a.BaseType; }
			var bch = new List<Type>(); while (b != null) { ach.Add(b); b = b.BaseType; }
			var i = 0; while (a == b && ach.Count > i && bch.Count > i) {
				a = ach[i]; b = bch[i++];
			}
			return (ach[--i], 1 / ((ach.Count + bch.Count - i * 2) + 1));
		}
	}
}
