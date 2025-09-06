using System.Collections;
using UnityEngine;

public class FixParentedJoints : MonoBehaviour
{
	[SerializeField]
	private Transform selfParent;

	[SerializeField]
	private Rigidbody2D kinematicBody;

	[SerializeField]
	[Tooltip("For some reason child hinges all stretch out if body did not exist at runtime.")]
	private Rigidbody2D existingTargetBody;

	[SerializeField]
	private int delayBodyDynamicFrames;

	[Space]
	[SerializeField]
	private bool isActive = true;

	private bool setDynamic;

	private Transform topParent;

	private void Awake()
	{
		if (!isActive)
		{
			return;
		}
		topParent = selfParent.parent;
		if ((bool)topParent)
		{
			Rigidbody2D rigidbody2D;
			Vector2 connectedAnchor;
			if ((bool)existingTargetBody)
			{
				rigidbody2D = existingTargetBody;
				connectedAnchor = existingTargetBody.position - kinematicBody.position;
			}
			else
			{
				GameObject obj = new GameObject("Fixed Joint - " + selfParent.gameObject.name);
				rigidbody2D = obj.AddComponent<Rigidbody2D>();
				obj.transform.position = kinematicBody.transform.position;
				obj.transform.SetParent(topParent, worldPositionStays: true);
				connectedAnchor = Vector2.zero;
			}
			FixedJoint2D fixedJoint2D = rigidbody2D.gameObject.AddComponent<FixedJoint2D>();
			selfParent.transform.SetParent(null, worldPositionStays: true);
			rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
			fixedJoint2D.autoConfigureConnectedAnchor = false;
			fixedJoint2D.anchor = Vector2.zero;
			fixedJoint2D.connectedAnchor = connectedAnchor;
			fixedJoint2D.connectedBody = kinematicBody;
			if (delayBodyDynamicFrames <= 0)
			{
				kinematicBody.bodyType = RigidbodyType2D.Dynamic;
			}
			else
			{
				setDynamic = true;
			}
		}
	}

	private IEnumerator Start()
	{
		int yieldCount = 2;
		if (setDynamic)
		{
			for (int i = 0; i < delayBodyDynamicFrames; i++)
			{
				yieldCount--;
				yield return null;
			}
			kinematicBody.bodyType = RigidbodyType2D.Dynamic;
		}
		if (!(topParent == null))
		{
			while (yieldCount > 0)
			{
				yieldCount--;
				yield return null;
			}
			if (topParent != null && !topParent.gameObject.activeInHierarchy)
			{
				selfParent.gameObject.SetActive(value: false);
			}
		}
	}
}
