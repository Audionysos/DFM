using audionysos.gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static audionysos.gui.RelativePlacement;

namespace WpfDNet;
internal class LayoutAPI_Tests {
	public LayoutAPI_Tests() {
		size();
	}

	public void size() {
		var b = new Button() {
			content = "Click Me",
			PRESSED = (b) => { Debug.WriteLine(""); }, 
			layout = {
				placement = (TOP, LEFT),
				size = (10, 10),
			},
			
		};
		b.state.

		var x = "";
		b = new Button() {
			content = x = "Click Me",
			layout = {
				placement = (TOP, LEFT),
				size = {
					desired = (10, 10).relative(),
					maximal = (10, 50)
				},
			},
			states = {
				defaul = { content = x },
				others = {
					new () {
						content = "s1",
						highlighted = {
							content = "s1H"
						}
					}
				}
			},
		};
	}
}
