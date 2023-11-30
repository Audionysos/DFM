using audionysos.collections.tree;
using audionysos.display;
using audionysos.gui.layout;
using audionysos.gui.style;
using System;
using System.Collections.Generic;
//sketching

namespace audionysos.gui;

public abstract class UIElement : ITreeLeafClient<UIElement>
{
    public static readonly GUIPainter defaultPainter = new DefaultButtonPainter();

    public TreePoint<UIElement> tree { get; }
    public virtual LayoutSettings layout { get; set; } = new LayoutSettings();
    public UIElementContainer? parent => tree.parent?.data as UIElementContainer;

    public abstract DisplayObject view { get; protected set; }

    internal LayoutArranger arranger = new LayoutArranger();
    public GUIPainter painter { get; set; } = new DefaultPainter();
    public VisualStyle style { get; set; } = new VisualStyle();

    public UIElement()
    {
        tree = !isContainer() ? new TreePoint<UIElement>(this)
            : new TreeNode<UIElement>(this);
        //tree.ADDED += onAdded;
        view.ENTER_FRAME += onEnterFrame;
    }

    private void onEnterFrame(DisplayObject @object)
    {
        update();
    }

    internal void update()
    {
        arranger.arrange(parent, this);
        painter.paint(this, view);
    }

    /// <summary>Indicate if this object suppose to be container that is able to store children and thus <see cref="TreeNode{T}"/> should be created for it as oppose to <see cref="TreePoint{T}"/>.</summary>
    protected virtual bool isContainer() => false;
}

public abstract class UIElementContainer : UIElement, ITreeNodeClient<UIElement> {

	protected override bool isContainer() => true;
	new public TreeNode<UIElement> tree
		=> (TreeNode<UIElement>)base.tree;

	public int Count => tree.children.Count;

	protected UIElementContainer() {
		view = view ?? throw new Exception($@"View of {nameof(UIElementContainer)} must be a {nameof(DisplayObjectContainer)}");
	}

	//public event Action<UIElementContainer, UIElement>? CHILD_ADD;
	//public event Action<UIElementContainer, UIElement>? CHILD_REMOVED;

	//public void addChild(UIElement child) {
	//	tree.addChild(child.tree);
	//	CHILD_ADD?.Invoke(this, child);
	//}
	//public void removeChild(UIElement child) {
	//	tree.removeChild(child.tree);
	//	CHILD_REMOVED?.Invoke(this, child);
	//}

}

public interface IUIElementsCollection : IReadOnlyList<UIElement>, IEnumerable<UIElement> {
	public void Add(UIElement child);
}
