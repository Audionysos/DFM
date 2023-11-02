using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace audionysos.math;

public readonly struct Range<N> where N : INumber<N> {
	/// <summary> Represents an empty range. This value should be used only for comparisons.
	/// Empty range should not be used as a parameter to any other method or operation.
	/// Result of any operation on the empty range is undefined.</summary>
	public static readonly Range<N> empty = new Range<N>();// new Range<N>(N.Zero, N.Zero); //wanted minValue here
	public readonly N start;
	public readonly N end;
	public N length => end - start;

	public Range(N start, N end) {
		if(start < end) {
			this.start = start;
			this.end = end;
		}else {
			this.start = end;
			this.end = start;
		}
	}

	/// <summary>Create <see cref="empty"/> range.</summary>
	public Range() {
		start = N.One;
		end = N.Zero;
	}

	public bool contains(N n)
		=> n >= start && n <= end;

	public bool contains(Range<N> o)
		=> o.start >= start && o.end <= end;

	public N inter(double u)
		=>  N.CreateChecked(double.CreateChecked(length) * u)
			+ start;

	public double normal(N v) {
		var fs = v - start;
		return double.CreateChecked(fs) / double.CreateChecked(length);
	}


	/// <summary>Returns true if this range ends before given other range starts.</summary>
	public bool endsBefore(Range<N> o) => o.start > end;
	/// <summary>Returns true if this range starts after given other range ends.</summary>
	public bool startsAfter(Range<N> o) => o.end < start;

	/// <summary>Returns sub range that is part of both ranges or <see cref="empty"/> range if there is no overlap.</summary>
	public Range<N> overlap(Range<N> o) {
		var (a, b) = order(this, o);
		var s = b.start;
		var e = a.end;
		if (s > e) return empty;
		return (s, e);
	}

	/// <summary>Returns range from end of one range to the start of other range or <see cref="empty"/> range if the ranges are overlapping</summary>
	public Range<N> gap(Range<N> o) {
		var (a, b) = order(this, o);
		if(b.start <= a.end) return empty;
		return (a.end, b.start);
	}

	/// <summary>Returns range(s) that is result of subtraction of given range from this range.</summary>
	public RangeOperationResult<N> subtract(Range<N> o) {
		if (this.gap(o) != empty) return (this, this );
		if(this.contains(o)) return ( (start, o.start) , (o.end, end) );
		if (o.contains(this)) return ( empty, empty );
		if (o.start < start) return ( empty, (o.end, end) );
		else return ( (start, o.start), empty );
	}

	/// <summary>Returns range that contains both ranges.</summary>
	public Range<N> expand(Range<N> o)
		=> (N.Min(start, o.start), N.Max(end, o.end));

	/// <summary>Orders ranges by their start values.</summary>
	public static (Range<N> a, Range<N> b) order(Range<N> a, Range<N> b)
		=> a.start < b.start ? (a, b) : (b, a);

	public static bool operator ==(Range<N> a, Range<N> b)
		=> a.start == b.start && a.end == b.end;
	public static bool operator !=(Range<N> a, Range<N> b)
		=> a.start != b.start || a.end != b.end;

	public static implicit operator Range<N>((N start, N end) t)
		=> new(t.start, t.end);

	//public static implicit operator Range<N>(Range r)
	//	=> new Range<int>(r.Start.Value, r.End.Value);

	public static implicit operator bool(Range<N> r)
		=> r != empty;


	public override string ToString() {
		if (this == empty) return "(empty range)";
		return $"({start} <= n <= {end})";
	}

}

/// <summary>Represents result of an operation on a <see cref="Range{N}"/> objects that may be split into two different <see cref="Range{N}"/> objects (<see cref="left"/>) and <see cref="right"/>).
/// If both (<see cref="left"/> and <see cref="right"/>) ranges are not empty, it's guaranteed that those ranges are not overlapping.
/// If both ranges are <see cref="Range{N}.empty"/> it means the result is also <see cref="empty"/>.
/// If both ranges are the same no-empty range it means that the operation result is a single, continuous range.</summary>
/// <typeparam name="N"></typeparam>
public struct RangeOperationResult<N> where N : INumber<N> {
	/// <summary>Result range on the left side or <see cref="Range{N}.empty"/>. If present, it always precedes the <see cref="right"/> result in terms of ranges order.</summary>
	public readonly Range<N> left;
	/// <summary>Result range on the right side or <see cref="Range{N}.empty"/>. If present, it always follows the <see cref="left"/> result.</summary>
	public readonly Range<N> right;

	/// <summary>Returns <see cref="left"/> result only if <see cref="right"/> is empty.</summary>
	public Range<N> onlyLeft => right ? Range<N>.empty : left;
	/// <summary>Returns <see cref="right"/> result only if <see cref="left"/> is empty.</summary>
	public Range<N> onlyRight => left ? Range<N>.empty : right;

	/// <summary>Returns the result range if the result is a single continuous range, otherwise returns <see cref="Range{N}.empty"/> range.
	/// If result is a single range both <see cref="left"/> and <see cref="right"/> point to the same range.</summary>
	public Range<N> single => left == right ? left : Range<N>.empty;
	/// <summary>Returns <see cref="left"/> range if it's not <see cref="Range{N}.empty"/> or <see cref="right"/> range.</summary>
	public Range<N> any => left == Range<N>.empty ? right : left;
	/// <summary>True if both <see cref="left"/> and <see cref="right"/> ranges are <see cref="Range{N}.empty"/>.</summary>
	public bool empty => left == Range<N>.empty && right == Range<N>.empty;
	/// <summary>True if the result contains both no-empty, unique ranges.</summary>
	public bool splitted => left && right && (left != right);

	public RangeOperationResult(Range<N> left, Range<N> right) {
		this.left = left;
		this.right = right;
	}

	public static bool operator ==(RangeOperationResult<N> a, RangeOperationResult<N> b)
		=> a.left == b.left && a.right == b.right;
	public static bool operator !=(RangeOperationResult<N> a, RangeOperationResult<N> b)
		=> a.left != b.left || a.right != b.right;


	public static implicit operator RangeOperationResult<N>
		((Range<N> left, Range<N> right) t)
			=> new(t.left, t.right);

	public static implicit operator bool(RangeOperationResult<N> r)
		=> !r.empty;

	public override string ToString() {
		return $"({left}, {right})";
	}

}

public class RangeEnumerator<N> : IEnumerable<N>, IEnumerator<N> where N : INumber<N> {
	private readonly Range<N> range;
	private readonly N step;
	public N Current { get; private set; }
	object IEnumerator.Current => Current;

	public RangeEnumerator(Range<N> range, N step) {
		this.range = range;
		if (N.IsZero(step)) step = N.One;
		this.step = step;
		Reset();
	}

	public bool MoveNext() {
		Current += step;
		return range.contains(Current);
	}

	public void Reset() {
		if (N.IsPositive(step)) Current = range.start - step;
		else Current = range.end + step;
	}

	public IEnumerator<N> GetEnumerator() => this;
	IEnumerator IEnumerable.GetEnumerator() => this;

	public void Dispose() { }
}

public class FloatRangeEnumerator<N> : IEnumerable<N>, IEnumerator<N> where N : INumber<N>, IBinaryFloatingPointIeee754<N> {
	private readonly Range<N> range;
	private readonly N step;
	public N Current { get; private set; }
	object IEnumerator.Current => Current;

	public FloatRangeEnumerator(Range<N> range, N step) {
		this.range = range;
		this.step = step;
		Reset();
	}

	public void Reset() {
		if (step == N.Zero) {
			switchToBitCrement();
			if (N.IsPositive(step)) //not working for 0 :( Actually can't use -0 N.NegativeZero works :|
				Current = N.BitDecrement(range.start);
			else Current = N.BitIncrement(range.end);
		} else {
			next = () => Current += step;
			if (N.IsPositive(step))
				Current = range.start - step;
			else Current = range.end + step;
		}
	}

	private void switchToBitCrement() {
		if(N.IsPositive(step))
			next = () => Current = N.BitIncrement(Current);
		else next = () => Current = N.BitDecrement(Current);
	}

	private Action next;
	public bool MoveNext() {
		var p = Current;
		next();
		if (Current == p) {
			switchToBitCrement();
			next();
		}
		return range.contains(Current);
	}


	public IEnumerator<N> GetEnumerator() => this;
	IEnumerator IEnumerable.GetEnumerator() => this;

	public void Dispose() { }
}
