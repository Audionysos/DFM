using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace audionysos.collections.tree; 

public interface ITreeNodeClient<T> where T : class {
	TreeNode<T> tree { get; }
}

public interface ITreeLeafClient<T> where T : class {
	TreePoint<T> tree { get; }
}

//public static class TreeExtensions {

//	public static void addChild<N, C>(this N n, C child)
//		where N : ITreeNodeClient<C>
//		where C : ITreeLeafClient<C>
//	{
//		n.tree.addChild(child.tree);
//	}

//	public static void removeChild<N, C, B>(this N n, C child)
//		where N : ITreeNodeClient<B>
//		where C : ITreeLeafClient<B> {
//		n.tree.removeChild(child.tree);
//	}
//}


/// <summary>Stores data common for all points in a tree.</summary>
public class TreeInfo {
	/// <summary>Returns root node of parent tree.</summary>
	public TreeNode root;

	internal TreeInfo(TreePoint root) {
		this.root = root as TreeNode;
	}

	private Dictionary<object, object> _attached;
	/// <summary>Returns attached object at given key, or creates new object for the key if no present already.</summary>
	/// <param name="key">Key at wich the object is stored.</param>
	/// <param name="create">Function which creates new object.</param>
	/// <returns></returns>
	public object getAttached(object key, Func<TreeInfo, object> create) {
		if (_attached == null) _attached = new Dictionary<object, object>();
		if (_attached.ContainsKey(key)) return _attached[key];
		var n = create(this);
		_attached.Add(key, n);
		return n;
	}

	/// <summary>Returns attached object of given type (stored with key of this type) or attaches newly created object.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="create"></param>
	/// <returns></returns>
	public T getAttached<T>(Func<TreeInfo, T> create) where T : class {
		return getAttached(typeof(T), create) as T;
	}

}

public class TreePoint {
	/// <summary>Dispatched after this point was added to a tree.</summary>
	public event Action<TreePoint> ADDED;
	/// <summary>Dispatched after this point was removed from a tree.</summary>
	public event Action<TreePoint> REMOVED;


	internal TreeInfo _info;
	/// <summary>Common information object for all tree points in a single tree.
	/// If the property is accessed without first been add to a tree, new info object will be created for this point.</summary>
	public TreeInfo info {
		get => _info ?? _parent?.info ?? (_info = new TreeInfo(this));
		private set => _info = value;
	}
	internal TreeNode _parent;
	/// <summary>Parent node this point is child of.</summary>
	public TreeNode parent {
		get => _parent;
		set {
			if (value == null) {
				if (parent == null) return;
				parent.removeChild(this);
			} else value.addChild(this);
		}
	}
	public object data { get; private set; }

	public TreePoint(object data) {
		this.data = data ?? throw new ArgumentNullException();
	}

	internal void dispatchAdded() {
		ADDED?.Invoke(this);
	}

	#region enumerations / searching etc.
	public void forAncestorNodes(Action<TreeNode> a) {
		var p = _parent;
		while (p != null) {
			a(p); p = p.parent;
		}
	}

	public TreeNode findAncestorNode(Predicate<TreeNode> a) {
		var p = _parent;
		while (p != null) {
			if(a(p)) return p;
			p = p.parent;
		}return null;
	}
	#endregion

	#region Conversions
	/// <inheritdoc/>
	public override string ToString() {
		if (data == null) return null;
		if (data is TreeNode tn) return $@"N:{tn.data}"; 
		if (data is TreePoint tp) return $@"P:{tp.data}"; 
		return data.ToString();
	}

	/// <summary>Fals if null.</summary>
	public static implicit operator bool(TreePoint l) => l != null;
	#endregion
}

public class TreeNode : TreePoint {
	internal List<TreePoint> _chs = new List<TreePoint>();
	public int Count { get; }
	public TreePoint lastChild => _chs.Count == 0 ? null : _chs[_chs.Count - 1];

	public TreeNode(object data) : base(data) {}

	public void addChild(TreePoint ch) {
		if (!ch) throw new ArgumentNullException();
		if (ch == this) throw new ArgumentException("Tree node cannot be added to itself");
		if (isAncestor(ch)) throw new ArgumentException("Given tree node is ancestor of this node and cannot be set as a child of this node.");
		if (ch.parent == this) {
			var li = _chs.IndexOf(ch);
			if (li == _chs.Count - 1) return;
			_chs.RemoveAt(li);
			_chs.Add(ch);
			return;
		}
		if (ch.parent == null) {
			_chs.Add(ch);
			ch._parent = this;
			ch.dispatchAdded();
			return;
		}
		ch._parent._chs.Remove(ch);
		_chs.Add(ch);
		ch.parent = this;
	}

	public void removeChild(TreePoint leaf) {
		if (!leaf) throw new ArgumentNullException();
		if (leaf.parent != this) throw new ArgumentException("Given leaf is not part of this node");
		_chs.Remove(leaf);
		leaf._info = null;
	}

	/// <summary>Tells if given tree point is ancestor of this node.</summary>
	public bool isAncestor(TreePoint p)
		=> findAncestorNode(n => n == p);
}

public class TreeInfo<T> where T : class {
	public TreeInfo bass { get; private set; }
	public TreeNode<T> root => bass.root?.data as TreeNode<T>;

	public TreeInfo(TreeInfo bass) {
		this.bass = bass;
	}
}

public class TreePoint<T> where T : class {
	/// <summary>Dispatched when this point was add to a tree.</summary>
	public event Action<TreePoint<T>> ADDED;
	private void addEventsPassHandlers() {
		bass.ADDED += (b) => ADDED?.Invoke(this);
	}

	protected virtual bool isContainer => false;
	public TreePoint bass { get; private set; }
	public T data { get; private set; }
	public TreeInfo<T> info => bass.info
		.getAttached((i) => new TreeInfo<T>(i));
	public TreeNode<T> parent => bass?.parent.data as TreeNode<T>;

	public TreePoint(T data){
		this.data = data;
		bass = isContainer ? new TreeNode(this)
			: new TreePoint(this);
		addEventsPassHandlers();
	}

	public void forAncestors(Action<T> a) {
		bass.forAncestorNodes((tn) => {
			var p = tn.data as TreePoint<T>;
			a(p.data);
		});
	}

	/// <inheritdoc/>
	public override string ToString() {
		return $@"TN<{typeof(T).Name}>";
	}
}

public class TreeNode<T> : TreePoint<T> where T : class {
	/// <inheritdoc/>
	protected override bool isContainer => true;
	new public TreeNode bass => base.bass as TreeNode;
	private TreePoints<T> _chs;
	public TreePoints<T> children => _chs;

	////TODO: this is bad :/
	//private TreePoints<T> childWraper;
	//public IReadOnlyList<TreePoint<T>> children
	//	=> childWraper;

	public TreeNode(T data) : base(data) {
		_chs = new TreePoints<T>(bass._chs);
		//baseNode = new TreeNode(data);
		//childWraper = new TreePoints<T>(baseNode._chs);
	}

	public void addChild(TreePoint<T> leaf)
		=> bass.addChild(leaf.bass);

	public void removeChild(TreePoint<T> leaf)
		=> bass.removeChild(leaf.bass);

	public void forDescendants(Action<T> a) {
		for (int i = 0; i < _chs.Count; i++) {
			var c = _chs[i];
			a(c.data);
			if (c is TreeNode<T> n) n.forDescendants(a);
		}
	}

	/// <summary></summary>
	/// <param name="a"></param>
	/// <param name="backward">Last child object of this node will invoked first.</param>
	public void forDescendants(Action<T> a, bool backward) {
		if (!backward) { forDescendants(a); return; }
		for (int i = _chs.Count -1; i > -1; i--) {
			var c = _chs[i];
			a(c.data);
			if (c is TreeNode<T> n) n.forDescendants(a, backward: true);
		}
	}

	/// <summary></summary>
	/// <param name="a"></param>
	/// <param name="backward">Last child object of this node will invoked first. [Warning: false not implemented]</param>
	public bool forDescendants(Func<T, bool> a, bool backward) {
		if (!backward) throw new NotImplementedException("forward traversing with breaking is not implemented.");
		for (int i = _chs.Count - 1; i > -1; i--) {
			var c = _chs[i];
			var brk = a(c.data);
			if (brk) return true;
			if (c is TreeNode<T> n) {
				if (n.forDescendants(a, backward: true))
					return true;
			}
		}
		return false;
	}

	/// <inheritdoc/>
	public override string ToString() {
		return $@"TN<{typeof(T).Name}>";
	}
}

/// <summary>Wraps collection of <see cref="TreePoint{T}"/>.</summary>
public class TreePoints<T> : IReadOnlyList<TreePoint<T>> where T : class {
	private IReadOnlyList<TreePoint> points;

	public TreePoints(IReadOnlyList<TreePoint> points) {
		this.points = points;
	}

	public int Count => points.Count;
	public TreePoint<T> this[int index]
		=> points[index].data as TreePoint<T>;

	/// <inheritdoc/>
	public IEnumerator<TreePoint<T>> GetEnumerator() {
		foreach (var tp in points) {
			yield return tp.data as TreePoint<T>;
		}
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
		=> points.GetEnumerator();
}
