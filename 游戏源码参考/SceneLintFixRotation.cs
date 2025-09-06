using System.Collections.Generic;
using UnityEngine;

public class SceneLintFixRotation : MonoBehaviour, ISceneLintUpgrader
{
	private readonly struct TransformRecord
	{
		private readonly Transform transform;

		private readonly Vector3 position;

		private readonly Quaternion rotation;

		public TransformRecord(Transform transform)
		{
			this.transform = transform;
			position = transform.position;
			rotation = transform.rotation;
		}

		public void Restore()
		{
			transform.position = position;
			transform.rotation = rotation;
		}
	}

	[SerializeField]
	private Transform readRotationOf;

	[SerializeField]
	private Transform[] fixChildrenOf;

	private readonly List<TransformRecord> copiedChildren = new List<TransformRecord>();

	private void Reset()
	{
		readRotationOf = base.transform;
	}

	private void Awake()
	{
		OnSceneLintUpgrade(doUpgrade: true);
	}

	[ContextMenu("Do Upgrade")]
	private void DoUpgrade()
	{
		OnSceneLintUpgrade(doUpgrade: true);
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		Vector3 eulerAngles = readRotationOf.eulerAngles;
		if (Mathf.Abs(eulerAngles.z) < Mathf.Epsilon)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return $"Rotation on parent of {eulerAngles.z} degrees.";
		}
		Transform[] array = fixChildrenOf;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Transform item in array[i])
			{
				copiedChildren.Add(new TransformRecord(item));
			}
		}
		Vector3 eulerAngles2 = readRotationOf.eulerAngles;
		eulerAngles2.z = 0f;
		readRotationOf.eulerAngles = eulerAngles2;
		foreach (TransformRecord copiedChild in copiedChildren)
		{
			copiedChild.Restore();
		}
		copiedChildren.Clear();
		return $"Fixed rotation on parent of {eulerAngles.z} degrees, fixed children.";
	}
}
