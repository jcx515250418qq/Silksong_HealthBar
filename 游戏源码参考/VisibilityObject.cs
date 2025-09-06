using UnityEngine;

public sealed class VisibilityObject : VisibilityEvent
{
	[SerializeField]
	private Renderer renderer;

	private bool init;

	private bool hasRenderer;

	private VisibilityGroup group;

	private void Awake()
	{
		Init();
	}

	private void OnValidate()
	{
		if (!renderer)
		{
			renderer = GetComponent<Renderer>();
		}
	}

	private void Init()
	{
		if (init)
		{
			return;
		}
		init = true;
		FindParent();
		if (hasRenderer)
		{
			return;
		}
		if (!renderer)
		{
			renderer = GetComponent<Renderer>();
			if (!renderer)
			{
				return;
			}
		}
		base.IsVisible = renderer.isVisible;
	}

	public void SetRenderer(Renderer renderer)
	{
		if (!(renderer == null) && !(renderer.gameObject != base.gameObject))
		{
			this.renderer = renderer;
			hasRenderer = true;
			base.IsVisible = renderer.isVisible;
		}
	}

	private void OnBecameVisible()
	{
		base.IsVisible = true;
	}

	private void OnBecameInvisible()
	{
		base.IsVisible = false;
	}
}
