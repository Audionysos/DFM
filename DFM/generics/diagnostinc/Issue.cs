using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.audionysos.generics.diagnostinc; 

/// <summary>Simple generic type representing any issue.</summary>
public class Issue {
	/// <summary>End user message, writen is source language.</summary>
	public string message { get; private set; }
	/// <summary>Data associated with the issue, may be any object but typically an exception.</summary>
	public object data { get; private set; }
	/// <summary>Spefices how important the issue is.</summary>
	public Impact impact { get; private set; }

	public Issue(string message, Impact impact = null, object data = null) {
		this.impact = impact ?? Impact.NOTICABLE;
		this.message = message;
		this.data = data;
	}

	/// <summary>False if null.</summary>
	public static implicit operator bool(Issue i)=> i!=null;

	/// <summary>Compares two issues in terms of ther <see cref="impact"/>.</summary>
	public static bool operator <(Issue i, Impact b) => i ?  i.impact.level < b.level : true;
	/// <summary>Compares two issues in terms of ther <see cref="impact"/>.</summary>
	public static bool operator <=(Issue i, Impact b) => i ? i.impact.level <= b.level : true;
	/// <summary>Compares two issues in terms of ther <see cref="impact"/>.</summary>
	public static bool operator >(Issue i, Impact b) => i ? i.impact.level > b.level : false;
	/// <summary>Compares two issues in terms of ther <see cref="impact"/>.</summary>
	public static bool operator >=(Issue i, Impact b) => i ?  i.impact.level >= b.level : false;
}

/// <summary>Represent collection of <see cref="Issue"/> instances while been the issue itself.</summary>
public class Issues : Issue {
	private List<Issue> subs = new List<Issue>();
	/// <summary>Number of sub issues this issue stores.</summary>
	public int Count => subs.Count;

	/// <summary></summary>
	/// <param name="message"></param>
	/// <param name="impact"></param>
	/// <param name="data"></param>
	public Issues(string message, Impact impact = null, object data = null)
		: base(message, impact, data) {
	}


}

/// <summary>Specifies possible impacts of an <see cref="Issue"/> thay may occure in a system.</summary>
public class Impact {
	/// <summary>Insignificant issue that not afect sytem operation and wont intrudce any futher problems by itself but may be incovienied and/or could be improved. This could be for example poor naming or formatting of some texts.</summary>
	public static readonly Impact COSMETIC = new Impact("COSMETIC", 0);
	/// <summary>The issue don't directly affect opertion of the system but may lead to some other problems in the future.</summary>
	public static readonly Impact NOTICABLE = new Impact("NOTICABLE", 1);
	/// <summary>The system can still oparate with the issue but it is very likely that it will cause even more serious problems in the future.</summary>
	public static readonly Impact IMPORTANT = new Impact("IMPORTANT", 2);
	/// <summary>The system may be able to oparate with the issue but it will be unstable and can crash during runtime or it may be vulnerable from data security perspective. The system cannot be put into production with this type of issue and it should be fixed right away.</summary>
	public static readonly Impact DANGEROUS = new Impact("DANGEROUS", 3);
	/// <summary>The sytem cannot operated with the issue.</summary>
	public static readonly Impact CRITICAL = new Impact("CRITICAL", 4);

	/// <summary>Word for the impact level.</summary>
	public string name { get; private set; }
	/// <summary>Numerical value for the impact, lower value means less impact.</summary>
	public int level { get; private set; }

	/// <summary>Returns true if this impact is at least on the same level or higher.</summary>
	public bool atLeast(Impact i) => this >= i;

	private Impact(string name, int level) {
		this.name = name;
		this.level = level;
	}

	/// <summary>Compares two impacts in therm of their <see cref="level"/>s.</summary>
	public static bool operator <(Impact a, Impact b) => a.level < b.level;
	/// <summary>Compares two impacts in therm of their <see cref="level"/>s.</summary>
	public static bool operator <=(Impact a, Impact b) => a.level <= b.level;
	/// <summary>Compares two impacts in therm of their <see cref="level"/>s.</summary>
	public static bool operator >(Impact a, Impact b) => a.level > b.level;
	/// <summary>Compares two impacts in therm of their <see cref="level"/>s.</summary>
	public static bool operator >=(Impact a, Impact b) => a.level >= b.level;

}
