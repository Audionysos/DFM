using System;
using System.Collections.Generic;
using System.Linq;
using C = com.audionysos.text.parsing.LanguageConstruct;
using SM = com.audionysos.text.parsing.expression.SequenceMatcher;
using com.audionysos.text.parsing.expression;
using LCSContext = com.audionysos.text.parsing.SyntaxParser.LanguageConstuctSearchContext;
using System.Diagnostics;
using com.audionysos.generics.containers;

namespace com.audionysos.text.parsing; 
public class SyntaxParser {
	public C construct { get; private set; }

	private List<SM> usedMatchers = new List<SM>();
	private Dictionary<Type, Dictionary<string, SM>> matchersMap
		= new Dictionary<Type, Dictionary<string, SM>>();

	private SCSExpressionParser textParser;
	public string source { get; private set; }

	public SyntaxParser(string source, C construct) {
		this.construct = construct;
		this.source = source;
	}

	public void produece() {
		collectMatchers(construct);
		textParser = new SCSExpressionParser(source, false);
		textParser.matchers = usedMatchers;
		textParser.parse();
		examinePatter(construct);
	}

	private void examinePatter(C c) {
		var ctx = new LCSContext(source, textParser.validMaches);
		while (ctx.nextMatch() && !isAtIndex(c, ctx)) ;
		var r = ctx.result;
	}

	private bool isAtIndex(C c, LCSContext ctx) {
		ctx.addNewTest(c);
		for (int i = 0; i < c.patterns.Count; i++) {
			var pe = c.patterns[i];
			if (!isAtIndex(pe, ctx)) {
				if (pe.optional) continue;
				ctx.removeTest();
				return false;
			}else {
				ctx.moveAfterLastApproved();
			}
			if (!ctx.currBase) {
				if (!pe.optional || i < c.patterns.Count - 1)
					ctx.removeTest();
				else break;
				return false;
			}
		}
		ctx.approveMatch();
		return true;
	}

	//TODO: consider "take" count
	private bool isAtIndex(PatterElement pe, LCSContext ctx) {
		var m = ctx.currBase; var mc = 0;
		while (mc < pe.take) {
			ctx.addNewSubTest(); var continu = false;
			foreach (var p in pe) {
				ctx.currentTest.target = p;
				if (p is SM pm && pm.equivalent(m.matcher)) {
					ctx.approveMatch(); mc++;
					ctx.moveAfterLastApproved();
					continu = true; break;
				}
				else if(p is C ic && isAtIndex(ic, ctx)) {
					ctx.approveMatch(); mc++;
					continu = true; break;
				}
			}
			if (continu) continue;
			ctx.removeTest();
			break;
		}
		//if (mc == 0) ctx.removeTest();
		return mc > 0;
	}

	#region Collecting used matchers
	private void collectMatchers(C c) {
		foreach (var pe in c.patterns) {
			foreach (var p in pe) {
				if (p is SM m) reqisterMatcher(m);
				else if (p is C cc) collectMatchers(cc); 
			}
		}
	}

	private void reqisterMatcher(SM m) {
		Dictionary<string, SM> mtl; var mt = m.GetType();
		if (!matchersMap.ContainsKey(mt)) {
			mtl = new Dictionary<string, SM>(); 
			mtl.Add(m.signature??"", m);
			matchersMap.Add(mt, mtl); usedMatchers.Add(m);
			return;
		}
		mtl = matchersMap[mt];
		if (mtl.ContainsKey(m.signature??"")) return;
		mtl.Add(m.signature??"", m); usedMatchers.Add(m);
	}
	#endregion

	public class LanguageConstuctSearchContext {
		private int _i = -1;
		/// <summary>Base match index.</summary>
		public int i {
			get => _i;
			set {
				_i = value;
				currBase = i >= baseMatches.Count || i < 0 ? null : baseMatches[i];
			}
		}
		private string source;
		/// <summary>List of base matches from which construct can be matched.</summary>
		public IReadOnlyList<SMatch> baseMatches { get; private set; }
		/// <summary>Current base match from <see cref="baseMatches"/>.</summary>
		public SMatch currBase { get; private set; }

		/// <summary>Result match of the search. If search was not sucessful this should be null.</summary>
		public ConstructMatchTest result { get; private set; }
		public ConstructMatchTest lastApproved { get; private set; }

		/// <summary>Stores top(curretly deepest) test that is opened to match a patter.</summary>
		private ConstructMatchTest ct;
		public ConstructMatchTest currentTest => ct;
		private ObjectsPool<ConstructMatchTest> testsPool = new ObjectsPool<ConstructMatchTest>();

		public LanguageConstuctSearchContext(string source, IReadOnlyList<SMatch> baseMatches) {
			this.source = source;
			this.baseMatches = baseMatches;
		}

		/// <summary>Increments interator and returns new base match.</summary>
		/// <returns></returns>
		public SMatch nextMatch() { i++; return currBase; }
		public SMatch moveAfterLastApproved() {
			if (!lastApproved) throw new ArgumentException("There were no approved tests.");
			i = lastApproved.pos.e + 1;
			return currBase;
		}

		/// <summary>Add new child test for <see cref="ct"/> to search for <see cref="Pattern"/> match at current interator position.</summary>
		/// <param name="c"></param>
		public void addNewTest(Pattern c) {
			if (!ct) { ct = testsPool.take().reset(c, i); return; }
			ct = ct.AddElementTest(testsPool.take().reset(c, i));
		}

		internal void addNewSubTest(Pattern c = null) {
			Debug.Assert(ct, "There is no currently active test to add sub test for.");
			ct = ct.AddSubElementTest(testsPool.take().reset(c, i));
		}

		/// <summary>Approves current test mach as valid restores interator positon to it's parent start position.</summary>
		internal void approveMatch() {
			Debug.Assert(ct, "There is no currelty active test to approve match for.");
			ct.approve(i);
			lastApproved = ct;
			if (ct.parent == null) result = ct; //root node matched
			ct = ct.parent;
			if (ct) i = ct.pos.s;
		}

		/// <summary>Removes current test as unmached and restores interator positon to it's parent start position.</summary>
		internal void removeTest() {
			Debug.Assert(ct, "There is no currently active test to remove.");
			testsPool.take(ct);
			ct = ct.parent;
			if (!ct) return;
			if (lastApproved) moveAfterLastApproved();
			else i = ct.pos.s;
			ct.RemoveLastTest();
		}

		public override string ToString() {
			return $@"{i}/{baseMatches?.Count}[{currBase}] test: {ct} ";
		}

		///// <summary>Increments interator and adds constuct as matched if <see cref="baseMatches"/> depleated, returns true in such case.</summary>
		///// <param name="c">Construct to ad ass been matched</param>
		//public bool addMatchIfCurrentBaseIsLast(C c) {
		//	if (nextMatch()) return false;
		//	constMatches.Add(new SMatch(c, source)); return true;
		//}

		///// <summary>Result construct matches</summary>
		//public List<SMatch> constMatches { get; private set; } = new List<SMatch>();

	}

}


