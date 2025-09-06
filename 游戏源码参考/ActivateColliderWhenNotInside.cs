using UnityEngine;

public class ActivateColliderWhenNotInside : MonoBehaviour
{
	[SerializeField]
	private Collider2D collider;

	private bool waitingToEnable;

	private int layerMask;

	private void Awake()
	{
		layerMask = Helper.GetCollidingLayerMaskForLayer(collider.gameObject.layer);
	}

	private void Update()
	{
		if (waitingToEnable && (bool)collider && !IsInsideCollider())
		{
			collider.enabled = true;
			waitingToEnable = false;
		}
	}

	[ContextMenu("Activate", true)]
	[ContextMenu("Deactivate", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Activate")]
	public void ActivateCollider()
	{
		if ((bool)collider)
		{
			if (!IsInsideCollider())
			{
				collider.enabled = true;
			}
			else
			{
				waitingToEnable = true;
			}
		}
	}

	[ContextMenu("Deactivate")]
	public void DeactivateCollider()
	{
		if ((bool)collider)
		{
			collider.enabled = false;
			waitingToEnable = false;
		}
	}

	private bool IsInsideCollider()
	{
		if (!base.isActiveAndEnabled)
		{
			return false;
		}
		bool result = false;
		BoxCollider2D boxCollider2D = collider as BoxCollider2D;
		if (boxCollider2D != null)
		{
			result = Physics2D.OverlapBox(base.transform.TransformPoint(boxCollider2D.offset), boxCollider2D.size, base.transform.rotation.z, layerMask);
		}
		else
		{
			CircleCollider2D circleCollider2D = collider as CircleCollider2D;
			if (circleCollider2D != null)
			{
				result = Physics2D.OverlapCircle(base.transform.TransformPoint(circleCollider2D.offset), circleCollider2D.radius, layerMask);
			}
		}
		return result;
	}
}
