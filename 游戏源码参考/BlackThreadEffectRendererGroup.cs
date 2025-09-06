using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class BlackThreadEffectRendererGroup : BlackThreadedEffect
{
	[SerializeField]
	private List<Renderer> renderers = new List<Renderer>();

	[SerializeField]
	private bool autoFindRenderers;

	[ContextMenu("Find Renderers In Children")]
	private void FindRenderersInChildren()
	{
		renderers.RemoveAll((Renderer o) => o == null);
		renderers = renderers.Union(GetComponentsInChildren<Renderer>(includeInactive: true)).ToList();
	}

	protected override void OnValidate()
	{
		if (Application.isPlaying)
		{
			renderers.RemoveAll((Renderer o) => o == null);
		}
	}

	protected override void Initialize()
	{
		renderers.RemoveAll((Renderer o) => o == null);
		if (autoFindRenderers)
		{
			renderers = renderers.Union(GetComponentsInChildren<Renderer>(includeInactive: true)).ToList();
		}
	}

	protected override void DoSetBlackThreadAmount(float amount)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			Renderer renderer = renderers[i];
			renderer.GetPropertyBlock(block);
			block.SetFloat(BlackThreadedEffect.BLACK_THREAD_AMOUNT, amount);
			renderer.SetPropertyBlock(block);
		}
	}

	protected override void EnableKeyword()
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].material.EnableKeyword("BLACKTHREAD");
		}
	}

	protected override void DisableKeyword()
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].material.DisableKeyword("BLACKTHREAD");
		}
	}
}
