using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using C = com.audionysos.text.parsing.LanguageConstruct;
using static com.audionysos.text.parsing.BasicMatchers;
using System.Collections;
using com.audionysos.text.parsing.expression;
using System.Diagnostics;

namespace com.audionysos.text.parsing; 

/// <summary>Complex <see cref="Pattern"/> which is composed of various child patterns, may specify alternatives and other settings.</summary>
public class LanguageConstruct : Pattern {
	public (int s, int e) currPos; 

	public override int startIndex => currPos.s;
	public override int endIndex => currPos.e;

	private List<PatterElement> _patterns = new List<PatterElement>();
	/// <summary>List of subsequent patterns that need to be matched in order to match this construct.</summary>
	public IReadOnlyList<PatterElement> patterns => _patterns.AsReadOnly();

	/// <summary>Stores last patter element.</summary>
	private PatterElement last => _patterns.Count == 0 ? null :
		_patterns[_patterns.Count - 1];


	private List<Pattern> allowedGaps;

	#region Composition
	/// <summary>Set patter of given type to be allowed between matched target patter elements.</summary>
	public C allowGapsOf<T>() where T : Pattern, new() {
		allowedGaps = allowedGaps ?? new List<Pattern>();
		allowedGaps.Add(Activator.CreateInstance<T>()); return this;
	}

	#region Reguired
	public C REQ(Pattern p) => addPattern(p);
	/// <summary>Specifies list of required litteral char sequence patters.</summary>
	public C REQ(params string[] ss) {
		foreach (var s in ss) REQ(new Sequence(s)); return this;
	}
	/// <summary>Required match of patter with specifc type.</summary>
	public C REQ<T>() where T : Pattern, new() {
		var p = Activator.CreateInstance<T>();
		_patterns.Add(p); return this;
	}

	#region Alternatives
	/// <summary>Specifies alternalive patter for previous pattern.</summary>
	public C or(Pattern p) { last?.Add(p); return this; }
	/// <summary>Specifies alternalive patter for previous pattern.</summary>
	public C or<T>() where T : Pattern, new() {
		var p = Activator.CreateInstance<T>();
		return or(p);
	}
	/// <summary>Specifes list of alternative string litteres patters for previous patter.</summary>
	public C or(params string[] ss) {
		foreach (var s in ss) last.Add(new Sequence(s)); return this;
	}
	#endregion

	#endregion

	#region Optional
	/// <summary>Optional match of litteral chars sequence. Each string argument is treated as separate <see cref="PatterElement"/>.</summary>
	public C OPT(params string[] ss) {
		foreach (var s in ss) opt(new Sequence(s));return this;
	}
	/// <summary>Optional patter match.</summary>
	public C opt(Pattern p) => addPattern(new PatterElement(p, true));
	/// <summary>Optional match of patter of specific type.</summary>
	public C OPT<T>() where T : Pattern, new()
		=> addPattern(new PatterElement(Activator.CreateInstance<T>(), true));
	/// <summary>Specifies new optional, dynamic construct patter and passes it to process by given action.</summary>
	public C OPT(Action<C> consturctProcessor) {
		var c = new C(); consturctProcessor(c);
		return addPattern(new PatterElement(c, true)); 
	}
	#endregion

	public C take(uint count) {last.take = count; return this; }
	/// <summary>Tells that last pattern may repeat and all occurences should be taken as part of this patter match.</summary>
	public C takeAll() {last.take = uint.MaxValue; return this; }


	private C addPattern(PatterElement p) {
		_patterns.Add(p); return this;
	}
	#endregion


}

/// <summary>Patter element is a list of possible child patters at single position within parent <see cref="C"/>.</summary>
public class PatterElement : IEnumerable<Pattern> {
	public int Count => elements.Count;
	private HashSet<Pattern> elements = new HashSet<Pattern>();
	public bool optional { get; private set; }
	/// <summary>Number of repeated matches of the element that can be taken.</summary>
	public uint take { get; set; } = 1;

	public PatterElement(Pattern p, bool optional = false) {
		this.optional = optional;
		elements.Add(p);
	}

	public void Add(Pattern p) => elements.Add(p);

	public IEnumerator<Pattern> GetEnumerator() => elements.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => elements.GetEnumerator();

	public override string ToString() {
		var s = take == uint.MaxValue ? "all" : take.ToString();
		s += optional ? " opt" : " ";
		s += $@" of "; var c = 0;
		foreach (var p in elements) {
			if (c++ > 0) s += " or ";
			s += p;
		}return s;
	}

	public static implicit operator PatterElement(Pattern p)
		=> new PatterElement(p);
}

/// <summary>Generic patter that can be found within a text.</summary>
public abstract class Pattern {
	/// <summary>Stores parameter-based signautre provided by concrete instance of dynamic patter. For example this could contains values passed to consturctor of a matcher.</summary>
	public string signature { get; protected set; }
	/// <summary>Index of first character withing the match.</summary>
	public abstract int startIndex { get; }
	/// <summary>Index of last character withing the match.</summary>
	public abstract int endIndex { get; }

	public override string ToString() {
		return $@"{GetType().Name}({signature})";
	}
}

/// <summary>Represents possible construct match that could be validated or discarded.</summary>
public class ConstructMatchTest : SMatch {
	public Pattern target { get; set; }
	private List<List<ConstructMatchTest>> subs = new List<List<ConstructMatchTest>>();
	public Result result { get; private set; } = Result.PENDING;
	public ConstructMatchTest parent { get; private set; } 

	public ConstructMatchTest(){

	}

	public ConstructMatchTest reset(Pattern p, int start) {
		if (result == Result.VALID) throw new InvalidOperationException("Cannot reset valid match.");
		result = Result.PENDING;
		parent = null;
		subs.Clear();
		target = p;
		pos = (start, -1);
		return this;
	}

	/// <summary>Approve succesful test.</summary>
	/// <param name="end">Index of last base match that belongs to the match.</param>
	public void approve(int end) {
		Debug.Assert(result == Result.PENDING, "Trying to approve costruct match test that was already verified.");
		result = Result.VALID;
		pos = (pos.s, end);
	}

	public ConstructMatchTest AddElementTest(ConstructMatchTest m) {
		subs.Add(new List<ConstructMatchTest>());
		subs[subs.Count - 1].Add(m);
		m.parent = this;
		return m;
	}

	public ConstructMatchTest AddSubElementTest(ConstructMatchTest m) {
		//Debug.Assert(subs.Count > 0, "There is no oppened element test add sub test for.");
		if (subs.Count == 0) subs.Add(new List<ConstructMatchTest>());
		subs[subs.Count - 1].Add(m);
		m.parent = this;
		return m;
	}

	public void RemoveLastTest() {
		var lel = subs[subs.Count - 1];
		if (lel.Count == 0) throw new InvalidOperationException("There is no sub element test to remove.");
		lel.RemoveAt(lel.Count -1);
	}

	public void Skip() {
		subs.Add(null);
	}

	public override string ToString() {
		return $@"{result} {target} test at {pos.s}";
	}

	public enum Result {
		PENDING,
		VALID,
		INVALID,
	}
}

