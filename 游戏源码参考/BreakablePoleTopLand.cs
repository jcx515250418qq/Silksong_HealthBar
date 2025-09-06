using UnityEngine;
using UnityEngine.Events;

public class BreakablePoleTopLand : MonoBehaviour, MetronomePlat.INotify
{
	[SerializeField]
	private float angleMin = 165f;

	[SerializeField]
	private float angleMax = 195f;

	[SerializeField]
	private bool keepSimulating;

	[Space]
	[SerializeField]
	private GameObject[] effects;

	[SerializeField]
	private RandomAudioClipTable stickAudioTable;

	[Space]
	public UnityEvent OnStick;

	private Rigidbody2D body;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Collider2D otherCollider = collision.otherCollider;
		if (collision.gameObject.layer != 8)
		{
			return;
		}
		Vector2 point = collision.GetSafeContact().Point;
		if (point.y > otherCollider.bounds.center.y)
		{
			return;
		}
		float num = NormalizeAngle(base.transform.eulerAngles.z);
		float num2 = angleMin;
		float num3 = angleMax;
		if (base.transform.lossyScale.x < 0f)
		{
			num2 = NormalizeAngle(360f - angleMax);
			num3 = NormalizeAngle(360f - angleMin);
		}
		if (num < num2 || num > num3)
		{
			return;
		}
		GameObject[] array = effects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Spawn(point);
		}
		stickAudioTable.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)body)
		{
			body.isKinematic = true;
			if (!keepSimulating)
			{
				body.simulated = false;
			}
			body.linearVelocity = Vector2.zero;
			body.angularVelocity = 0f;
			MetronomePlat componentInParent = collision.gameObject.GetComponentInParent<MetronomePlat>();
			if ((bool)componentInParent)
			{
				componentInParent.RegisterNotifier(this);
			}
		}
		OnStick.Invoke();
		static float NormalizeAngle(float angle)
		{
			angle %= 360f;
			if (angle < 0f)
			{
				angle += 360f;
			}
			return angle;
		}
	}

	public void PlatRetracted(MetronomePlat plat)
	{
		plat.UnregisterNotifier(this);
		body.isKinematic = false;
		body.simulated = true;
		CollectableItemPickup componentInChildren = GetComponentInChildren<CollectableItemPickup>();
		if ((bool)componentInChildren)
		{
			componentInChildren.CancelPickup();
		}
	}
}
