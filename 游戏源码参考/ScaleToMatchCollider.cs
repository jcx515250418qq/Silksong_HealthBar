using UnityEngine;

[ExecuteInEditMode]
public class ScaleToMatchCollider : MonoBehaviour
{
	[SerializeField]
	private Collider2D collider;

	[SerializeField]
	private Vector2 multiplyScale = Vector2.one;

	[SerializeField]
	private Vector2 contractMax;

	[SerializeField]
	private Vector2 contractMin;

	private void OnDrawGizmosSelected()
	{
		Bounds contractedBounds = GetContractedBounds();
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(contractedBounds.center, contractedBounds.size);
	}

	private void Start()
	{
		UpdateScale();
	}

	private Bounds GetContractedBounds()
	{
		Bounds bounds = collider.bounds;
		bounds.SetMinMax(bounds.min + contractMin.ToVector3(0f), bounds.max - contractMax.ToVector3(0f));
		return bounds;
	}

	private void UpdateScale()
	{
		if ((bool)collider)
		{
			Bounds contractedBounds = GetContractedBounds();
			Vector3 size = contractedBounds.size;
			Transform obj = base.transform;
			Transform parent = obj.parent;
			Vector3 self = (parent ? parent.InverseTransformVector(size) : size);
			obj.localScale = self.MultiplyElements((Vector3)multiplyScale);
			obj.position = contractedBounds.center;
		}
	}
}
