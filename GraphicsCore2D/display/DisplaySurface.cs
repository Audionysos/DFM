﻿using audioysos.collections.tree;
using audioysos.geom;
using com.audionysos;
using System;
using System.Collections.Generic;
using System.Text;
//skatching

namespace audioysos.display {
	public abstract class DisplaySurface {
		private List<DisplayObject> _displayed = new List<DisplayObject>(); 

		public DisplaySurface() {
			//var c = new DisplayObjectContainer();
			//var d = new Der();
			//var o = d as DisplayObject;
			//c.addChild(o);
			//c.addChild(d);
			////d.
			////c.
			//c.tree.addChild(c.tree);
			//graphics = new Graphics();
		}

		public void Add(DisplayObject d) {
			if (d.surface != null) throw new Exception("The object is already added to some display surface.");
			_displayed.Add(d);
			d.surface = this;
		}

		public void update() {
			clear();
			for (int i = 0; i < _displayed.Count; i++) {
				_displayed[i].update();
			}
		}

		public void clear() => clear<IPoint2>();
		public abstract void clear<P>(IRect<P> rect = null) where P : IPoint2;

		/// <summary>Create graphics that this sufrace will display.</summary>
		public abstract IMicroGraphics2D createGraphics();

		/// <summary>Render graphics onto the surface. Given object is the one created with <see cref="createGraphics"/> method.</summary>
		public abstract void renderGraphics(IMicroGraphics2D graphics);

		/// <summary>False if null.</summary>
		public static implicit operator bool(DisplaySurface s) => s!=null;
	}

	public class Der : DisplayObject {

	}

	public class DerX : DisplayObjectContainer {

	}

	public class XXX : IDisplayable<DerX> {
		public DerX view { get; }
	}

	public interface IDisplayable<T> where T : DisplayObject {
		/// <summary>Displayable view that could be add to a <see cref="DisplayObject"/>.</summary>
		public T view { get; }
	}

	public class Polygon {

	}

	public interface ITransformProvier {
		Transform transform { get; }

	}

	public static class TransformProviderExtensions {

		public static void get() {

		}

	}

	public class ViewsLayouter {
		private IArrangable container { get; set; }
		private List<IArrangable> childs { get; set; }

		public void layout() {
			var childsSize = getChildrenSize();
		}

		private Point2 getChildrenSize() {
			var s = new Point2(0, 0);
			foreach (var ch in childs) {
				s.add(ch.size);
			}
			return s;

		}
	}

	public class ObjectArranger {

	}

	public interface IArrangable {
		void arragne(int pos, int size);

		IPoint2 pos { get; }
		IPoint2 size { get; }
		IPoint2 minSize { get; }
		IPoint2 maxSize { get; }
	}

}
