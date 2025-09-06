using TeamCherry.SharedUtils;
using UnityEngine;

public class Rigidbody2DDisturberImpulse : Rigidbody2DDisturberBase
{
	[SerializeField]
	[HideInInspector]
	private Vector2 directionRange = Vector2.one;

	[SerializeField]
	private Vector2 minDirection;

	[SerializeField]
	private Vector2 maxDirection;

	[SerializeField]
	private MinMaxFloat magnitudeRange;

	[SerializeField]
	private bool resetVelocity;

	[SerializeField]
	private bool limitByFrequency;

	[SerializeField]
	private bool customCooldown;

	[SerializeField]
	private float cooldown = 0.5f;

	[SerializeField]
	private bool useCoolDownCurve;

	[SerializeField]
	private AnimationCurve cooldownCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[SerializeField]
	private bool useDiminishingReturns;

	[SerializeField]
	private MinMaxFloat diminishingRange = new MinMaxFloat(45f, 65f);

	[SerializeField]
	private MinMaxFloat diminishingMultiplier = new MinMaxFloat(1f, 0.05f);

	[SerializeField]
	private bool debugMe;

	private double lastFullImpulseTime;

	private Coroutine rumbleRoutine;

	private void OnValidate()
	{
		if (directionRange != Vector2.zero)
		{
			minDirection = -directionRange;
			maxDirection = directionRange;
			directionRange = Vector2.zero;
		}
	}

	protected override void Awake()
	{
		OnValidate();
		base.Awake();
	}

	public void Impulse()
	{
		float num = (float)(Time.timeAsDouble - lastFullImpulseTime);
		float num2 = 0.01f;
		if (customCooldown)
		{
			num2 = cooldown;
		}
		float num4;
		if (limitByFrequency && num < num2)
		{
			float num3 = num / num2;
			if (useCoolDownCurve)
			{
				num3 = cooldownCurve.Evaluate(Mathf.Clamp01(num3));
			}
			else if (num3 < 0.5f)
			{
				num3 = 0.5f;
			}
			num4 = num3;
		}
		else
		{
			num4 = 1f;
			lastFullImpulseTime = Time.timeAsDouble;
		}
		Rigidbody2D[] array = bodies;
		foreach (Rigidbody2D rigidbody2D in array)
		{
			Vector2 vector = new Vector2(Random.Range(minDirection.x, maxDirection.x), Random.Range(minDirection.y, maxDirection.y));
			vector.Normalize();
			float num5 = num4;
			if (useDiminishingReturns)
			{
				if (debugMe)
				{
					debugMe = true;
				}
				float magnitude = rigidbody2D.linearVelocity.magnitude;
				float t = Mathf.Clamp01(diminishingRange.GetTBetween(magnitude));
				float lerpedValue = diminishingMultiplier.GetLerpedValue(t);
				if (debugMe)
				{
					debugMe = true;
				}
				num5 *= lerpedValue;
			}
			if (resetVelocity)
			{
				rigidbody2D.linearVelocity = Vector2.zero;
			}
			Vector2 vector2 = vector * (magnitudeRange.GetRandomValue() * num5);
			if (debugMe)
			{
				Debug.Log($"Adding force: {vector2} to {this} {Time.frameCount} {CustomPlayerLoop.FixedUpdateCycle}", this);
			}
			rigidbody2D.AddForce(vector2, ForceMode2D.Impulse);
		}
	}
}
