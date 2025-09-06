using UnityEngine;

public class WaterfallRegion : MonoBehaviour
{
	[SerializeField]
	private float downVelocity;

	[SerializeField]
	private FlingUtils.Config heroDropletFling;

	[SerializeField]
	private Vector3 heroDropletFlingOffset;

	[SerializeField]
	private float heroDropletFlingDelay;

	private bool heroInside;

	private double nextHeroDropletFlingTime;

	private HeroController hc;

	private Rigidbody2D body;

	private void OnValidate()
	{
		if (downVelocity < 0f)
		{
			downVelocity = 0f;
		}
	}

	private void FixedUpdate()
	{
		if (!heroInside)
		{
			return;
		}
		if (Time.timeAsDouble >= nextHeroDropletFlingTime)
		{
			FlingHeroDroplets();
		}
		if (!hc.cState.swimming && !hc.cState.onGround)
		{
			Vector2 linearVelocity = body.linearVelocity;
			linearVelocity.y -= downVelocity * Time.fixedDeltaTime;
			float num = 0f - hc.GetMaxFallVelocity();
			if (linearVelocity.y < num)
			{
				linearVelocity.y = num;
			}
			body.linearVelocity = linearVelocity;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if ((bool)hc)
		{
			return;
		}
		hc = other.GetComponent<HeroController>();
		if ((bool)hc)
		{
			body = other.GetComponent<Rigidbody2D>();
			heroInside = true;
			FlingHeroDroplets();
			if (!hc.controlReqlinquished)
			{
				hc.RecoilDown();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if ((bool)other.GetComponent<HeroController>())
		{
			hc = null;
			body = null;
			heroInside = false;
		}
	}

	private void FlingHeroDroplets()
	{
		nextHeroDropletFlingTime = Time.timeAsDouble + (double)heroDropletFlingDelay;
		FlingUtils.SpawnAndFling(heroDropletFling, hc.transform, heroDropletFlingOffset);
	}
}
