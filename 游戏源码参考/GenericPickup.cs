using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Events;

public class GenericPickup : MonoBehaviour
{
	private enum PickupAnimations
	{
		Normal = 0,
		Stand = 1
	}

	[SerializeField]
	private InteractEvents interactEvents;

	[SerializeField]
	private NestedFadeGroupBase group;

	[SerializeField]
	private float fadeTime;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("interactEvents", true, false, false)]
	private PickupAnimations pickupAnim;

	[Space]
	public UnityEvent OnBecameActive;

	public UnityEvent OnBecameInactive;

	private bool activated;

	public static bool IsPickupPaused { get; set; }

	public event Func<bool> PickupAction;

	private void Awake()
	{
		if ((bool)interactEvents)
		{
			interactEvents.Interacted += DoPickup;
		}
	}

	private void DoPickup()
	{
		if (!activated)
		{
			StartCoroutine(Pickup());
		}
	}

	private bool DoPickupAction(bool breakIfAtMax)
	{
		if (this.PickupAction == null)
		{
			return false;
		}
		CollectableItemHeroReaction.DoReaction(new Vector2(0f, -0.76f));
		return this.PickupAction();
	}

	private IEnumerator Pickup()
	{
		HeroController.instance.StopAnimationControl();
		tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
		animator.Play((pickupAnim == PickupAnimations.Normal) ? "Collect Normal 1" : "Collect Stand 1");
		yield return new WaitForSeconds(0.75f);
		bool didPickup = DoPickupAction(breakIfAtMax: false);
		animator.Play((pickupAnim == PickupAnimations.Normal) ? "Collect Normal 2" : "Collect Stand 2");
		if (didPickup)
		{
			SetActive(value: false);
			activated = true;
			float waitTime = 0.5f;
			while (IsPickupPaused)
			{
				waitTime -= Time.deltaTime;
				yield return null;
			}
			if (waitTime > 0f)
			{
				yield return new WaitForSeconds(waitTime);
			}
		}
		yield return StartCoroutine(animator.PlayAnimWait((pickupAnim == PickupAnimations.Normal) ? "Collect Normal 3" : "Collect Stand 3"));
		HeroController.instance.StartAnimationControl();
		if ((bool)interactEvents)
		{
			interactEvents.EndInteraction();
			if (didPickup)
			{
				interactEvents.Deactivate(allowQueueing: false);
			}
		}
	}

	public void SetActive(bool value, bool isInstant = false)
	{
		if (!activated)
		{
			if (value)
			{
				interactEvents.Activate();
				OnBecameActive.Invoke();
			}
			else
			{
				interactEvents.Deactivate(allowQueueing: false);
				OnBecameInactive.Invoke();
			}
			if ((bool)group)
			{
				group.FadeTo(value ? 1f : 0f, isInstant ? 0f : fadeTime);
			}
		}
	}
}
