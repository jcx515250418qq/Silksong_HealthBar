using System;
using UnityEngine;

public class HeroBouncer : MonoBehaviour
{
	private enum Behaviours
	{
		BounceUp = 0,
		RecoilLeft = 1,
		RecoilRight = 2
	}

	[SerializeField]
	private GameObject bounceEffectPrefab;

	[SerializeField]
	private Vector2 bounceEffectOffset;

	[SerializeField]
	private Behaviours behaviour;

	private void OnCollisionStay2D(Collision2D other)
	{
		HeroController component = other.gameObject.GetComponent<HeroController>();
		if (!component)
		{
			return;
		}
		switch (behaviour)
		{
		case Behaviours.BounceUp:
			component.BounceShort();
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				component.RecoilLeft();
			}
			else
			{
				component.RecoilRight();
			}
			break;
		case Behaviours.RecoilLeft:
			component.RecoilLeft();
			break;
		case Behaviours.RecoilRight:
			component.RecoilRight();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if ((bool)bounceEffectPrefab)
		{
			bounceEffectPrefab.Spawn().transform.SetPosition2D(component.transform.TransformPoint(bounceEffectOffset));
		}
	}
}
