using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveWeathervane : MonoBehaviour
{
	private static readonly int _hitLeftAnim = Animator.StringToHash("Hit Left");

	private static readonly int _hitRightAnim = Animator.StringToHash("Hit Right");

	[SerializeField]
	private Animator rotationAnimator;

	[SerializeField]
	private Animator poleAnimator;

	[SerializeField]
	private TinkEffect tinkEffect;

	[Space]
	[SerializeField]
	private UnityEvent OnHit;

	private void Awake()
	{
		if ((bool)tinkEffect)
		{
			tinkEffect.HitInDirection += OnHitInDirection;
		}
	}

	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		float num;
		switch (direction)
		{
		case HitInstance.HitDirection.Left:
			num = -1f;
			break;
		case HitInstance.HitDirection.Right:
			num = 1f;
			break;
		case HitInstance.HitDirection.Up:
		case HitInstance.HitDirection.Down:
			num = ((source.transform.position.x > base.transform.position.x) ? 1 : (-1));
			break;
		default:
			throw new NotImplementedException();
		}
		int stateNameHash = ((num > 0f) ? _hitRightAnim : _hitLeftAnim);
		if ((bool)rotationAnimator)
		{
			rotationAnimator.Play(stateNameHash, 0, 0f);
		}
		if ((bool)poleAnimator)
		{
			poleAnimator.Play(stateNameHash, 0, 0f);
		}
		OnHit.Invoke();
	}
}
