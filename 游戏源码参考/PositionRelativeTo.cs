using System;
using UnityEngine;

[ExecuteInEditMode]
public class PositionRelativeTo : MonoBehaviour
{
	[Serializable]
	private struct ExtensionPair
	{
		public GameObject Target;

		public Vector3 AddOffset;
	}

	[SerializeField]
	private Transform inSpace;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private ExtensionPair[] extensions;

	[SerializeField]
	private bool positionX;

	[SerializeField]
	private bool positionY;

	[SerializeField]
	private bool positionZ;

	[SerializeField]
	private Vector3 offset;

	private Vector3 previousPos;

	public Vector3 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
			Reposition();
		}
	}

	private void Awake()
	{
		previousPos = base.transform.position;
	}

	private void Update()
	{
		Reposition();
	}

	private void Reposition()
	{
		Vector3 position = (target ? target.position : base.transform.position);
		if ((bool)inSpace)
		{
			position = inSpace.InverseTransformPoint(position);
		}
		if ((bool)target)
		{
			position += offset;
		}
		else
		{
			position = offset;
		}
		ExtensionPair[] array = extensions;
		for (int i = 0; i < array.Length; i++)
		{
			ExtensionPair extensionPair = array[i];
			if ((bool)extensionPair.Target && extensionPair.Target.activeInHierarchy)
			{
				position += extensionPair.AddOffset;
			}
		}
		Vector3 vector = base.transform.position;
		if ((bool)inSpace)
		{
			vector = inSpace.InverseTransformPoint(vector);
		}
		if (positionX)
		{
			vector.x = position.x;
		}
		if (positionY)
		{
			vector.y = position.y;
		}
		if (positionZ)
		{
			vector.z = position.z;
		}
		if ((bool)inSpace)
		{
			vector = inSpace.TransformPoint(vector);
		}
		if (!(Vector3.Distance(vector, previousPos) <= Mathf.Epsilon))
		{
			previousPos = vector;
			base.transform.position = vector;
		}
	}
}
