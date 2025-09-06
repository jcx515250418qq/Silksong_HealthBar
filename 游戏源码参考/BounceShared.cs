using System.Collections;
using UnityEngine;

public static class BounceShared
{
	private const float BOUNCE_PULL_DURATION_DEFAULT = 0.03f;

	private const float BOUNCE_PULL_DURATION_WITCH = 0.08f;

	private const float REAPER_RANGE_X = 1f;

	private const float REAPER_RANGE_Y = 1.75f;

	private const float SHAMAN_RANGE_Y = 1f;

	private static int pullingCount;

	private static int controlVersion;

	private static int animationVersion;

	public static IEnumerator BouncePull(Transform transform, Vector2 heroBouncePos, HeroController hc, HitInstance hit)
	{
		Rigidbody2D body = hc.Body;
		Vector2 fromPos = body.position;
		Vector2 toPos = new Vector2(transform.position.x, heroBouncePos.y);
		bool flag = false;
		float bouncePullDuration = 0.03f;
		if (!hit.IsHarpoon && hit.IsNailTag)
		{
			switch (hc.playerData.CurrentCrestID)
			{
			case "Reaper":
				toPos.x = Mathf.Clamp(toPos.x, fromPos.x - 1f, fromPos.x + 1f);
				toPos.y = Mathf.Clamp(toPos.y, fromPos.y - 1.75f, fromPos.y + 1.75f);
				break;
			case "Wanderer":
				toPos.x = fromPos.x;
				break;
			case "Toolmaster":
				flag = true;
				break;
			case "Witch":
				bouncePullDuration = 0.08f;
				flag = true;
				break;
			case "Spell":
				toPos.x = fromPos.x;
				toPos.y = Mathf.Clamp(toPos.y, fromPos.y - 1f, fromPos.y + 1f);
				break;
			}
		}
		if (toPos.y < fromPos.y && !flag)
		{
			toPos.y = fromPos.y;
		}
		if (hc.cState.facingRight)
		{
			if (toPos.x < fromPos.x)
			{
				toPos.x = fromPos.x;
			}
		}
		else if (toPos.x > fromPos.x)
		{
			toPos.x = fromPos.x;
		}
		if (hc.Config.DownSlashType != 0)
		{
			hc.RelinquishControl();
			animationVersion = hc.StopAnimationControlVersioned();
			hc.AffectedByGravity(gravityApplies: false);
			controlVersion = HeroController.ControlVersion;
			pullingCount++;
			tk2dSpriteAnimationClip clip = hc.GetComponent<HeroAnimationController>().GetClip("Pod Bounce");
			hc.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(clip, 0);
			for (float elapsed = 0f; elapsed < bouncePullDuration; elapsed += Time.deltaTime)
			{
				Vector2 position = Vector2.Lerp(fromPos, toPos, elapsed / bouncePullDuration);
				body.MovePosition(position);
				yield return null;
			}
			if (controlVersion == HeroController.ControlVersion)
			{
				hc.RegainControl();
			}
			hc.StartAnimationControl(animationVersion);
			hc.AffectedByGravity(gravityApplies: true);
			pullingCount--;
		}
		body.MovePosition(toPos);
	}

	public static void OnBouncePullInterrupted()
	{
		if (pullingCount <= 0)
		{
			return;
		}
		pullingCount--;
		if (pullingCount != 0)
		{
			return;
		}
		HeroController instance = HeroController.instance;
		if (instance != null)
		{
			if (controlVersion == HeroController.ControlVersion)
			{
				instance.RegainControl();
			}
			instance.StartAnimationControl(animationVersion);
			instance.AffectedByGravity(gravityApplies: true);
		}
	}
}
