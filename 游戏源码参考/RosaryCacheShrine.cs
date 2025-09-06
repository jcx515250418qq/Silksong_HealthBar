using UnityEngine;
using UnityEngine.Events;

public class RosaryCacheShrine : RosaryCache
{
	[SerializeField]
	private Rigidbody2D flingDish;

	[SerializeField]
	private Vector2 flingDishForce;

	[SerializeField]
	private float flingDishTorque;

	[SerializeField]
	private GameObject regularDish;

	[SerializeField]
	private VectorCurveAnimator hitReactAnimator;

	[SerializeField]
	private GameObject brokenDish;

	[Space]
	public UnityEvent OnCompleted;

	protected override float GetHitSourceY(float sourceHeight)
	{
		return base.transform.position.y;
	}

	protected override void RespondToHit(HitInstance damageInstance, Vector2 hitPos)
	{
		Vector2 force = flingDishForce;
		float num = flingDishTorque;
		if ((bool)hitReactAnimator)
		{
			switch (damageInstance.GetHitDirection(HitInstance.TargetType.Regular))
			{
			case HitInstance.HitDirection.Left:
				hitReactAnimator.StartAnimation(isFlipped: true);
				force.x *= -1f;
				num *= -1f;
				break;
			case HitInstance.HitDirection.Right:
				hitReactAnimator.StartAnimation(isFlipped: false);
				break;
			default:
				hitReactAnimator.StartAnimation(Random.Range(0, 2) > 0);
				force.x = 0f;
				num = Random.Range(0f - num, num);
				break;
			}
		}
		if (base.State >= base.StateCount - 1)
		{
			if ((bool)regularDish)
			{
				regularDish.gameObject.SetActive(value: false);
			}
			if ((bool)flingDish)
			{
				flingDish.gameObject.SetActive(value: true);
				flingDish.AddForce(force, ForceMode2D.Impulse);
				flingDish.AddTorque(num, ForceMode2D.Impulse);
			}
			if (OnCompleted != null)
			{
				OnCompleted.Invoke();
			}
		}
	}

	protected override void SetCompletedReturning()
	{
		base.SetCompletedReturning();
		if ((bool)regularDish)
		{
			regularDish.gameObject.SetActive(value: false);
		}
		if ((bool)flingDish)
		{
			flingDish.gameObject.SetActive(value: false);
		}
		if ((bool)brokenDish)
		{
			brokenDish.gameObject.SetActive(value: true);
		}
		if (OnCompleted != null)
		{
			OnCompleted.Invoke();
		}
	}
}
