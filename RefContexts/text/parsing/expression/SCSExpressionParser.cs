using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static com.audionysos.text.parsing.BasicMatchers;
using Range = com.audionysos.text.parsing.BasicMatchers.Range;


namespace com.audionysos.text.parsing.expression; 
public class SCSExpressionParser {
	public List<SequenceMatcher> matchers = new List<SequenceMatcher>() {
		typeof(Name),
		typeof(Digits),
		new Range("<", ">"),
		new Range("(", ")"),
		new Character('=', '+', '-', '*', '/'),

	};
	private List<SequenceMatcher> actives = new List<SequenceMatcher>();
	private List<SequenceMatcher> continuedMatchers = new List<SequenceMatcher>();

	private List<SMatch> valids = new List<SMatch>();
	public IReadOnlyList<SMatch> validMaches => valids.AsReadOnly();

	/// <summary>Sotres lists of matches keyed to specific <see cref="SequenceMatcher"/> instances.</summary>
	private Dictionary<SequenceMatcher, List<SMatch>> map = new Dictionary<SequenceMatcher, List<SMatch>>();

	private string s;

	public SCSExpressionParser(string s, bool autoStart = true) {
		this.s = s;
		actives.AddRange(matchers);
		continuedMatchers = new List<SequenceMatcher>(matchers.Count);
		if(autoStart) parse();
	}

	private ParsingContext pc;

	public void parse() {
		pc = new ParsingContext { source = s };
		for (; pc.i < s.Length; pc.i++) {
			var si = pc.i;
			while (actives.Count > 0 && pc.i <= s.Length) {
				if(pc.i < s.Length) pc.c = s[pc.i];
				foreach (var m in actives) {
					if (m.take(pc)) { continuedMatchers.Add(m); continue; }
					if (m.isValid) { storeMach(m); }
				}
				swapMatchers(); pc.i++;
			}
			pc.i = si;
			activateMatchers();
		}
	}

	/// <summary>Repopulate <see cref="actives"/> from inital <see cref="matchers"/>.</summary>
	private void activateMatchers() {
		foreach (var m in matchers) {
			if (m.allowOverlap || !map.ContainsKey(m)
				|| map[m].Last().pos.e < pc.i + 1)
					actives.Add(m.reset()); 
		}
	}

	private void storeMach(SequenceMatcher m) {
		List<SMatch> l;
		if (map.ContainsKey(m)) l = map[m];
		else {
			l = new List<SMatch>();
			map.Add(m, l);
		}
		var mi = new SMatch(m, pc.source); 
		l.Add(mi); valids.Add(mi);
		if (m.multiOut) continuedMatchers.Add(m);
		m.reset();
	}

	private void swapMatchers() {
		var t = actives;
		actives = continuedMatchers;
		continuedMatchers = t;
		continuedMatchers.Clear();
	}

	public struct ParsingContext {
		/// <summary>Index of currently procesed char.</summary>
		public int i;
		/// <summary>Currently processed char.</summary>
		public char c;
		/// <summary>Source string from were the char is taken.</summary>
		public string source;
	}
}

public class SMatch {
	public Pattern matcher { get; protected set; }
	public string source;
	public string value => source?.Substring(pos.s, pos.e - pos.s + 1);
	public (int s, int e) pos { get; protected set; }

	public SMatch(Pattern m, string source) {
		matcher = m;
		this.source = source;
		pos = (m.startIndex, m.endIndex);
	}

	protected SMatch() {}

	public override string ToString() {
		return $@"""{value}"" at ({pos.s}, {pos.e}) by {matcher.GetType().Name}({matcher.signature}) ";
	}

	/// <summary>False if null</summary>
	public static implicit operator bool(SMatch m) => m!=null;
	//public static implicit operator SMatch(Pattern m)
	//	=> new SMatch(m);
}
