using com.audionysos.text.edit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TextEditorTests; 
[TestClass]
public class TextTests {

	[TestMethod]
	public void LineSplitTest() {
		//       |      .     |      .       |       .      |
		//       012\3\4567\8911\2\3456\7\8\9112\3\4\5\6\7891
		var s = "000\r\n111\n222\n\r333\n\r\n444\r\r\n\r\r555";
		//according to notepad++ this should look like
		var t = new Text(s);
		assertLine(t, 0, 0, 5);   //000RN
		assertLine(t, 1, 5, 9);   //111N
		assertLine(t, 2, 9, 13);  //222N
		assertLine(t, 3, 13, 14); //R
		assertLine(t, 4, 14, 18); //333N
		assertLine(t, 5, 18, 20); //RN
		assertLine(t, 6, 20, 24); //444R
		assertLine(t, 7, 24, 26); //RN
		assertLine(t, 8, 26, 27); //R
		assertLine(t, 9, 27, 28); //R
		assertLine(t, 10, 28, 31);//555

	}

	[TestMethod]
	public void emptyTextHasOneEmptyLine() {
		var t = new Text("");
		Assert.IsTrue(t.lines.Count == 1);
		Assert.IsTrue(t.lines[0].length == 0);
	}

	[TestMethod]
	public void emptyTextSapnReturnsEmptyString() {
		var t = new Text();
		Assert.AreEqual(t.lines[0].fullOrError, "");
	}

	[TestMethod]
	public void insertion() {
		var t = new Text("01234 6789 BCDE");
		var s = new TextSpan(t, 6, 10);
		Assert.AreEqual("6789", s.text);

		t.insert("XY", 7);
		Assert.AreEqual("6XY789", s.text);

		t.insert("sss", 0);
		Assert.AreEqual("6XY789", s.text);

		t.insert("eee", t.Count);
		Assert.AreEqual("6XY789", s.text);
		Assert.AreEqual("sss01234 6XY789 BCDEeee", t.ToString());
	}

	[TestMethod]
	public void insertionRemoval() {
		var t = new Text("01234 6789 BCDE");
		var s = new TextSpan(t, 6, 10);
		Assert.AreEqual("6789", s.text);

		t.insert("XYZ", 9);
		Assert.AreEqual("678XYZ9", s.text);
		Assert.AreEqual("01234 678XYZ9 BCDE", t.ToString());

		t.insert("ZYX", 5);
		Assert.AreEqual("678XYZ9", s.text);
		Assert.AreEqual("01234ZYX 678XYZ9 BCDE", t.ToString());

		t.remove(5, 7);
		Assert.AreEqual("XYZ9", s.text);
		Assert.AreEqual("01234XYZ9 BCDE", t.ToString());

		t.remove(6, 8);
		Assert.AreEqual("X", s.text);
		Assert.AreEqual("01234X", t.ToString());

		t.remove(0, 5);
		Assert.AreEqual("X", s.text);
		Assert.AreEqual("X", t.ToString());

		t.remove(0, 1);
		Assert.AreEqual("", s.text);
		Assert.AreEqual("", t.ToString());

		t.insert("abc", 0);
		Assert.AreEqual("", s.text);
		Assert.AreEqual(3, s.start);
		Assert.AreEqual(3, s.end);
		Assert.AreEqual("abc", t.ToString());
	}

	private void assertLine(Text t, int l, int s, int e) {
		Assert.AreEqual(s, t.lines[l].start);
		Assert.AreEqual(e, t.lines[l].end);
	}

}

[TestClass]
public class CaretsTest {

	[TestMethod]
	public void CaretPlacement() {

	}

}

[TestClass]
public class SelectionTest {
	
	[TestMethod]
	public void basicSeletion() {
		var a = new TextManipulator("abcd");
		a.selection.set();
		Assert.AreEqual("abcd", a.selection.text);
		a.selection.set(0, 2);
		Assert.AreEqual("ab", a.selection.text);
		a.selection.set(2, 4);
		Assert.AreEqual("cd", a.selection.text);
		a.selection.set(0, 1);
		Assert.AreEqual("a", a.selection.text);
		a.selection.set(0, 0);
		Assert.AreEqual("", a.selection.text);
	}
}
