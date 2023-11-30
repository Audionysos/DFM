using audionysos.gui;
using audionysos.gui.layout;
using com.audionysos;
using System.Diagnostics;
using WpfDNet.SLtoWPF;
using static audionysos.gui.layout.RelativePlacement;

namespace WpfDNet;

internal class LayoutAPI_Tests {
	private readonly SixLaborsToWPFAdapter adapter;

	public LayoutAPI_Tests(SixLaborsToWPFAdapter adapter) {
		this.adapter = adapter;
		basic();
		//size();
	}

	private void basic() {
		var p = new Panel() {
			layout = {
				placement = (CENTER, CENTER),
				size = (200, 200)
			},
			style = {
				background = (Color)0xFF000055,
			},
			children = {
				new Block() {
					layout = {
						placement = (-.9, -.9),
						size = (25, 25)
					},
					style = {
						background = (Color)0x0000FF55,
					}
				}
			}
		};
		p.layout.size.design = adapter.displaySurface.size;
		adapter.displaySurface.Add(p.view);
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
		//b.state.

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
