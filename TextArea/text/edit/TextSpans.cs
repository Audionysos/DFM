using System.Collections;
using System.Collections.Generic;

namespace com.audionysos.text.edit; 

/// <summary>Collection of <see cref="TextSpan"/> instances associted with a sigle <see cref="Text"/> source.</summary>
public class TextSpans : IEnumerable<TextSpan>, IReadOnlyList<TextSpan> {
	private List<TextSpan> _all = new List<TextSpan>();
	public int Count => _all.Count;
	public TextSpan this[int index] => _all[index];
	private Text soruce;

	public TextSpan Add(int start, int end) {
		return new TextSpan(soruce, start, end);
	}

	public TextSpans(Text source) {

	}

	/// <inheritdoc/>
	public IEnumerator<TextSpan> GetEnumerator() => _all.GetEnumerator();
	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _all.GetEnumerator();

}
