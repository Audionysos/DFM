using audioysos.display;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GraphicsCore2DTest {

	[TestClass]
	public class DisplayTreeTests {
		[TestMethod]
		public void Basics() {
			var c = new Sprite() { name = "root" };
			var o2 = new Sprite() { name = "o2"};
			var o3 = new Sprite() { name = "o3" };
			c.addChild(o2);
			c.addChild(o3);
			var o4 = new Sprite() { name = "o4" };
			o2.addChild(o4);
			var o5 = new Der() { name = "o5" };
			o4.addChild(o5);
			var o6 = new Der() { name = "o6" };
			o2.addChild(o6);

			Assert.AreEqual(2, c.Count);

			var r = o5.tree.info.root.data;
			 Assert.AreSame(r, c);
			//TODO: Get display container?
			//r.children
		}
	}


}
