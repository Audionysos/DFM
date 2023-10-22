using audionysos.display;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TreeTesting {

	[TestClass]
	public class TreeTesting {

		[TestMethod]
		public void TestMethod1() {
			var c = new Sprite();
			c.name = "root";
			var ch = new Sprite();
			ch.name = "ch1";
			c.addChild(ch);

			Assert.IsTrue(c.tree.children.Count == 1);
			Assert.AreSame(c, ch.parent);
			c.transform.x = 100;
			ch.transform.x = 20;
			Assert.IsTrue(ch.getGlobalTransform().x == 120);
			c.transform.sX = 2;
			Assert.IsTrue(ch.getGlobalTransform().x == 140);

			var ch2 = new Sprite();
			ch2.name = "ch2";
			ch.addChild(ch2);
			Assert.AreSame(c, ch.parent);

			ch2.transform.x = 10;
			Assert.AreEqual(160, ch2.getGlobalTransform().x);
		}
	}
}
