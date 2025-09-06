using UnityEngine;

public class SpriteRendererVisibilityCentre : MonoBehaviour
{
	[SerializeField]
	private Transform repositionOverride;

	private SpriteRenderer[] spriteRenderers;

	private Vector2 totalCentre;

	private void Awake()
	{
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		totalCentre = GetCentre(checkVisibility: false);
	}

	public void Evaluate()
	{
		Transform t = (repositionOverride ? repositionOverride : base.transform);
		t.SetLocalPosition2D(Vector2.zero);
		Vector2 centre = GetCentre(checkVisibility: true);
		Vector2 position = totalCentre - centre;
		t.SetLocalPosition2D(position);
	}

	private Vector2 GetCentre(bool checkVisibility)
	{
		GetCurrentBounds(checkVisibility, out var boundsMin, out var boundsMax);
		return (boundsMin + boundsMax) * 0.5f;
	}

	private void GetCurrentBounds(bool checkVisibility, out Vector2 boundsMin, out Vector2 boundsMax)
	{
		boundsMin = Vector2.one * float.MaxValue;
		boundsMax = Vector2.zero * float.MinValue;
		SpriteRenderer[] array = spriteRenderers;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if (!checkVisibility || spriteRenderer.gameObject.activeSelf)
			{
				Bounds bounds = spriteRenderer.bounds;
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (min.x < boundsMin.x)
				{
					boundsMin.x = min.x;
				}
				if (min.y < boundsMin.y)
				{
					boundsMin.y = min.y;
				}
				if (max.x > boundsMax.x)
				{
					boundsMax.x = max.x;
				}
				if (max.y > boundsMax.y)
				{
					boundsMax.y = max.y;
				}
			}
		}
	}
}
