using UnityEngine;

public class BreakableTriggerBreak : MonoBehaviour
{
	private enum TriggerConditions
	{
		Hero = 0,
		HeroSlide = 1
	}

	[SerializeField]
	private Breakable breakable;

	[SerializeField]
	private TriggerConditions triggerCondition;

	private void OnTriggerEnter2D(Collider2D col)
	{
		TriggerConditions triggerConditions = triggerCondition;
		int num;
		if (triggerConditions != 0)
		{
			if (triggerConditions != TriggerConditions.HeroSlide)
			{
				goto IL_003c;
			}
			num = 1;
		}
		else
		{
			num = 0;
		}
		int layer = col.gameObject.layer;
		if (layer != 9 && layer != 20)
		{
			return;
		}
		switch (num)
		{
		default:
			return;
		case 1:
			if (!SlideSurface.IsHeroSliding)
			{
				return;
			}
			break;
		case 0:
			break;
		}
		goto IL_003c;
		IL_003c:
		breakable.BreakSelf();
	}
}
