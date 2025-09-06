using TMProOld;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupRemap : MonoBehaviour
{
	private SpriteRenderer[] spriteRenderers;

	private TextMeshPro[] textMeshes;

	public CanvasGroup group;

	private float alpha;

	private bool skippedFirstUpdate;

	private void Awake()
	{
		if (!group)
		{
			group = GetComponent<CanvasGroup>();
		}
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		textMeshes = GetComponentsInChildren<TextMeshPro>(includeInactive: true);
		Sync(0f);
	}

	private void Update()
	{
		if (!skippedFirstUpdate)
		{
			skippedFirstUpdate = true;
		}
		else if (group.alpha != alpha)
		{
			alpha = group.alpha;
			Sync(alpha);
		}
	}

	private void Sync(float alpha)
	{
		SpriteRenderer[] array = spriteRenderers;
		foreach (SpriteRenderer obj in array)
		{
			Color color = obj.color;
			color.a = alpha;
			obj.color = color;
		}
		TextMeshPro[] array2 = textMeshes;
		foreach (TextMeshPro obj2 in array2)
		{
			Color color2 = obj2.color;
			color2.a = alpha;
			obj2.color = color2;
		}
	}
}
