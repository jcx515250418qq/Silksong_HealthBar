using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BlackThreadStrandSimple : MonoBehaviour
{
	private class CreatureTracker
	{
		public Transform Transform;

		public SpriteRenderer Sprite;

		public BasicSpriteAnimator Animator;

		public float StartX;
	}

	[SerializeField]
	private Transform creaturesParent;

	[SerializeField]
	private MinMaxFloat creatureReposition;

	[SerializeField]
	private Transform rageParent;

	[SerializeField]
	private EventRegister rageStartEvent;

	[SerializeField]
	private EventRegister rageEndEvent;

	private float scaleAdjust;

	private List<ScaleActivation> rageCreatures;

	private void Awake()
	{
		if ((bool)rageStartEvent)
		{
			rageStartEvent.ReceivedEvent += OnRageStart;
		}
		if ((bool)rageEndEvent)
		{
			rageEndEvent.ReceivedEvent += OnRageEnd;
		}
		Vector3 lossyScale = base.transform.lossyScale;
		scaleAdjust = lossyScale.y / lossyScale.x;
		foreach (Transform item in creaturesParent)
		{
			BasicSpriteAnimator component = item.GetComponent<BasicSpriteAnimator>();
			if ((bool)component)
			{
				Vector3 localScale = item.localScale;
				localScale.x *= scaleAdjust;
				item.localScale = localScale;
				CreatureTracker tracker = new CreatureTracker
				{
					Sprite = item.GetComponent<SpriteRenderer>(),
					Animator = component,
					Transform = item,
					StartX = item.localPosition.x
				};
				component.OnAnimEnd.AddListener(delegate
				{
					RepositionAndPlay(tracker);
				});
			}
		}
		rageCreatures = new List<ScaleActivation>(rageParent.childCount);
		rageParent.gameObject.SetActive(value: true);
		foreach (Transform item2 in rageParent)
		{
			item2.gameObject.SetActive(value: false);
			Vector3 localScale2 = item2.localScale;
			localScale2.x *= scaleAdjust;
			item2.localScale = localScale2;
			ScaleActivation component2 = item2.GetComponent<ScaleActivation>();
			rageCreatures.Add(component2);
		}
	}

	private void OnRageStart()
	{
		foreach (ScaleActivation rageCreature in rageCreatures)
		{
			rageCreature.Activate();
		}
	}

	private void OnRageEnd()
	{
		foreach (ScaleActivation rageCreature in rageCreatures)
		{
			rageCreature.Deactivate();
		}
	}

	private void RepositionAndPlay(CreatureTracker tracker)
	{
		float num = creatureReposition.GetRandomValue() * scaleAdjust;
		tracker.Transform.SetLocalPositionX(tracker.StartX + num);
		tracker.Sprite.flipX = Random.Range(0, 2) == 0;
		tracker.Animator.Play();
	}
}
