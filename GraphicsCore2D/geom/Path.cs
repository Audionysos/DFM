using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using V = audionysos.geom.IPoint2;


namespace audionysos.geom;
public class Path : IReadOnlyList<V>, IEnumerable<V> {
	private List<V> _points = new List<V>();

	/// <inheritdoc/>
	public int Count => _points.Count;
	/// <inheritdoc/>
	public V this[int index] {
		get => _points[index];
	}

	public void Add(V p) {
		//p = p.copy();
		//p.x = Math.Round(p.x);
		//p.y = Math.Round(p.y);
		_points.Add(p);
	}

	public void Add(params V[] points) {
		_points.EnsureCapacity(Count + points.Length);
		foreach (var p in points)
			Add(p);
	}

	public void Add(double x, double y) {
		Add(new Point2(x, y));
	}

	/// <inheritdoc/>
	public IEnumerator<V> GetEnumerator()
		=> _points.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
		=> _points.GetEnumerator();

	public override string ToString() {
		return $@"Points[{Count}]";
	}
}
