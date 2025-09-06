using UnityEngine;

public class KeepRelativeWorldDirection : MonoBehaviour
{
	[SerializeField]
	private Vector3 referenceDirection = Vector3.up;

	private Vector3 referenceOffset;

	private Transform parent;

	private bool hasParent;

	private void Awake()
	{
		if (InitParent())
		{
			referenceOffset = base.transform.localPosition;
		}
	}

	private bool InitParent()
	{
		if (!hasParent)
		{
			parent = base.transform.parent;
			hasParent = parent != null;
			if (!hasParent)
			{
				return false;
			}
		}
		return true;
	}

	private void LateUpdate()
	{
		if (hasParent)
		{
			Vector3 normalized = parent.InverseTransformDirection(referenceDirection).normalized;
			Vector3 position = Quaternion.LookRotation(base.transform.forward, normalized) * referenceOffset;
			base.transform.position = parent.TransformPoint(position);
		}
	}

	private void OnTransformParentChanged()
	{
		if (InitParent() && !(parent == base.transform.parent))
		{
			referenceOffset = base.transform.localPosition;
		}
	}

	[ContextMenu("Record Current World Direction and Offset")]
	private void RecordCurrentWorldDirection()
	{
		if (parent == null)
		{
			Debug.LogError("Parent is not assigned. Cannot record world direction and offset.");
			return;
		}
		referenceDirection = parent.InverseTransformDirection((base.transform.position - parent.position).normalized);
		Debug.Log($"Recorded current world direction as: {referenceDirection} and offset as: {referenceOffset}");
	}
}
