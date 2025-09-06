using System;
using TeamCherry.SharedUtils;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialPropertyRegion : MonoBehaviour
{
	[Serializable]
	private struct Modifier
	{
		public string PropertyName;

		public MinMaxFloat Range;
	}

	[SerializeField]
	private Modifier[] modifiers;

	[SerializeField]
	private Vector2 minPoint;

	[SerializeField]
	private Vector2 maxPoint;

	private Vector2 minWorldPoint;

	private Vector2 maxWorldPoint;

	private Vector2 distanceMax;

	private float multiplierX;

	private float multiplierY;

	private Renderer[] renderers;

	private MaterialPropertyBlock block;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(minPoint, 0.2f);
		Gizmos.DrawWireSphere(maxPoint, 0.2f);
		Gizmos.DrawLine(minPoint, maxPoint);
	}

	private void Awake()
	{
		Refresh();
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (modifiers != null && modifiers.Length != 0)
		{
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			renderers = GetComponentsInChildren<Renderer>();
			UpdateSharedData();
			Renderer[] array = renderers;
			foreach (Renderer renderer in array)
			{
				ApplyModifiers(renderer);
			}
		}
	}

	private void UpdateSharedData()
	{
		minWorldPoint = base.transform.TransformPoint(minPoint);
		maxWorldPoint = base.transform.TransformPoint(maxPoint);
		distanceMax = maxWorldPoint - minWorldPoint;
		float num = Mathf.Min(distanceMax.x, distanceMax.y) / Mathf.Max(distanceMax.x, distanceMax.y);
		multiplierX = 0f;
		if (Mathf.Abs(distanceMax.x) > 0f)
		{
			multiplierX = ((distanceMax.x > distanceMax.y) ? (1f - num) : num);
		}
		multiplierY = 0f;
		if (Mathf.Abs(distanceMax.y) > 0f)
		{
			multiplierY = ((distanceMax.y > distanceMax.x) ? (1f - num) : num);
		}
	}

	private void ApplyModifiers(Renderer renderer)
	{
		Vector2 vector = renderer.transform.position;
		vector.x = Mathf.Clamp(vector.x, minWorldPoint.x, maxWorldPoint.x);
		vector.y = Mathf.Clamp(vector.y, minWorldPoint.y, maxWorldPoint.y);
		Vector2 vector2 = maxWorldPoint - vector;
		float num = ((multiplierX > 0f) ? (1f - vector2.x / distanceMax.x) : 0f);
		float num2 = ((multiplierY > 0f) ? (1f - vector2.y / distanceMax.y) : 0f);
		float t = num * multiplierX + num2 * multiplierY;
		renderer.GetPropertyBlock(block);
		Modifier[] array = modifiers;
		for (int i = 0; i < array.Length; i++)
		{
			Modifier modifier = array[i];
			float value = Mathf.Lerp(modifier.Range.Start, modifier.Range.End, t);
			block.SetFloat(modifier.PropertyName, value);
		}
		renderer.SetPropertyBlock(block);
	}
}
