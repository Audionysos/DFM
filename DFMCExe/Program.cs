using com.audionysos.console;
using com.audionysos.console.handlers;
using com.audionysos.console.pritSetts;
using com.audionysos.data;
using com.audionysos.data.dfm.docFiles;
using com.audionysos.generics.reflection;
using com.audionysos.text;
using System;
using System.IO;
using System.Reflection;
using static com.audionysos.console.StaticConsole;

namespace com.audionysos.data
{
	public class TestContext {

		public string aString = "abc";
		public int anInt = 4;

		public TestContext() {


		}

		public void add() {
			anInt++;
		}

	}


    class Program
    {
		private static ReflContext rc;

		static void Main(string[] args) {
			newLinesTest(); return;

			var c = new console.Console();
			c.handlers.Clear();
			c.handlers.Add(ConsoleHanders.CONSOLE);
			StaticConsole.console = c;
			log("ConsoleTest", new PrS { c = 0x00FF0000 });



			//var dfm = new DFM(false);
			//dfm.root = "test"; Directory.GetCurrentDirectory();
			////dfm.root = @"C:\Users\Suchy\Documents"; 
			//dfm.loadModule<DocFiles>();
			//dfm.initialize();

			var ctx = new TestContext();
			rc = new ReflContext(ctx);
			testMethodExecutor();

			processInput();
			

			//dfm.Scan();
			//var dm = dfm.get.module<DocFiles>();
			//dm.Diagnose();
			//dm.
        }

		private static void newLinesTest() {
			File.WriteAllText("newlines", "000\r\n111\n222\n\r333\n\r\n444\r\r\n\r\r555");
		}

		private static void processInput() {
			(int x, int y) hp = (System.Console.CursorLeft, System.Console.CursorTop + 1);
			var ch = System.Console.ReadKey().KeyChar;
			var li = "" + ch; //last input
			while (li != "quit") {
				System.Console.SetCursorPosition(hp.x, hp.y);
				printHints(li, 25);
				var m = 25 - (System.Console.CursorTop - hp.y);
				while (m-- > 0) {
					System.Console.Write(" ".times(80));
					System.Console.SetCursorPosition(
						hp.x,
						System.Console.CursorTop + 1);
				}

				System.Console.SetCursorPosition(hp.x + li.Length, hp.y - 1);
				var k = System.Console.ReadKey();
				ch = k.KeyChar;
				if(k.Key == ConsoleKey.Enter) {
					rc.execute(li);
				}
				if (ch == "\b"[0]) {
					System.Console.Write(" \b \b");
					if (li.Length > 0) li = li.Substring(0, li.Length - 1);
				} else li += ch;
			}
			//printHints(null);
		}

		private static void testMethodExecutor() {
			//var rf = new ReflContext(null);
			//var m = new MethodExecutor(rf, rf.GetType().GetMethod(nameof(ReflContext.getHints)), new object[] {"abc"}, null);
			//log(m.signature);
		}

		private static void printHints(string v, int h) {
			var hs = rc.getHints(v);
			log(hs, new Exp {
				n = h,
				//inStr = "",
				//endStr = "",
				perItem = {
					new PrS { s = "                                                         \n" }
				}
			});
		}
	}
}
