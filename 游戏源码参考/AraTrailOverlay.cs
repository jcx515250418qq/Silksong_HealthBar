using Ara;
using UnityEngine;

public class AraTrailOverlay : MonoBehaviour
{
	private AraTrail trailRenderer;

	private Material[] materials;

	private void Awake()
	{
		Refresh();
	}

	private void OnDestroy()
	{
		if (materials == null)
		{
			return;
		}
		Material[] array = materials;
		foreach (Material material in array)
		{
			if ((bool)material)
			{
				Object.Destroy(material);
			}
		}
		materials = null;
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		trailRenderer = GetComponent<AraTrail>();
		if (!trailRenderer)
		{
			return;
		}
		if (materials != null)
		{
			Material[] array = materials;
			foreach (Material material in array)
			{
				if ((bool)material)
				{
					Object.Destroy(material);
				}
			}
		}
		materials = new Material[trailRenderer.materials.Length];
		for (int j = 0; j < materials.Length; j++)
		{
			if ((bool)trailRenderer.materials[j])
			{
				materials[j] = new Material(trailRenderer.materials[j]);
				materials[j].renderQueue = 4000;
			}
		}
		trailRenderer.materials = materials;
	}
}
