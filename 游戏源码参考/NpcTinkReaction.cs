using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NpcTinkReaction : MonoBehaviour
{
	private enum LookAnimNpcActivateBehaviours
	{
		Any = 0,
		FaceLeft = 1,
		FaceRight = 2
	}

	[SerializeField]
	private TinkEffect tinkEffect;

	[Space]
	[SerializeField]
	private NPCControlBase npcControl;

	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private LookAnimNPC lookAnimNPC;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("lookAnimNPC", true, false, false)]
	private LookAnimNpcActivateBehaviours lookAnimNPCActivate;

	[Space]
	[SerializeField]
	private string bounceAnim;

	[SerializeField]
	private string returnToIdleAnim;

	[SerializeField]
	private ConditionalAnimation conditionalBounceAnim;

	[Space]
	public UnityEvent OnBounced;

	public UnityEvent OnBounceAnimEnd;

	[Space]
	[SerializeField]
	private bool sendEndEventWhenInterrupted;

	private Coroutine animateRoutine;

	private void Reset()
	{
		npcControl = GetComponent<NPCControlBase>();
	}

	private void Awake()
	{
		if (!npcControl)
		{
			npcControl = GetComponent<NPCControlBase>();
		}
	}

	private void OnEnable()
	{
		tinkEffect.HitInDirection += OnHitInDirection;
	}

	private void OnDisable()
	{
		if (sendEndEventWhenInterrupted && animateRoutine != null)
		{
			OnBounceAnimEnd.Invoke();
		}
		StopBehaviour();
		tinkEffect.HitInDirection -= OnHitInDirection;
	}

	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		StopBehaviour();
		animateRoutine = StartCoroutine(Animate());
	}

	private void StopBehaviour()
	{
		if (animateRoutine != null)
		{
			StopCoroutine(animateRoutine);
			animateRoutine = null;
		}
	}

	private IEnumerator Animate()
	{
		if ((bool)lookAnimNPC)
		{
			lookAnimNPC.Deactivate();
		}
		if ((bool)npcControl)
		{
			npcControl.Deactivate(allowQueueing: false);
		}
		OnBounced.Invoke();
		if ((bool)animator)
		{
			if (conditionalBounceAnim != null && conditionalBounceAnim.CanPlayAnimation())
			{
				yield return conditionalBounceAnim.PlayAndWait();
			}
			else if (!string.IsNullOrEmpty(bounceAnim))
			{
				yield return StartCoroutine(animator.PlayAnimWait(bounceAnim));
			}
			if (!string.IsNullOrEmpty(returnToIdleAnim))
			{
				yield return StartCoroutine(animator.PlayAnimWait(returnToIdleAnim));
			}
		}
		if ((bool)lookAnimNPC)
		{
			switch (lookAnimNPCActivate)
			{
			case LookAnimNpcActivateBehaviours.Any:
				lookAnimNPC.Activate();
				break;
			case LookAnimNpcActivateBehaviours.FaceLeft:
				lookAnimNPC.Activate(facingLeft: true);
				break;
			case LookAnimNpcActivateBehaviours.FaceRight:
				lookAnimNPC.Activate(facingLeft: false);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		OnBounceAnimEnd.Invoke();
		yield return new WaitForSeconds(0.5f);
		if ((bool)npcControl)
		{
			npcControl.Activate();
		}
		animateRoutine = null;
	}
}
