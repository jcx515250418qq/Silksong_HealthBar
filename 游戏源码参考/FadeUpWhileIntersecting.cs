using System;
using System.Linq;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

public class FadeUpWhileIntersecting : MonoBehaviour
{
	private enum RotateTowardsTargets
	{
		None = 0,
		Hero = 1,
		ClosestEnemy = 2
	}

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private MinMaxFloat fadeRange;

	[SerializeField]
	private AnimationCurve fadeCurve;

	[SerializeField]
	private float fadeSpeed;

	[Space]
	[SerializeField]
	private RotateTowardsTargets rotateTowards;

	[SerializeField]
	private float rotateSpeed;

	private float initialAlpha;

	public bool IsTargetIntersecting { get; private set; }

	private void Awake()
	{
		if ((bool)fadeGroup)
		{
			initialAlpha = fadeGroup.AlphaSelf;
		}
	}

	private void OnEnable()
	{
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = 0f;
		}
	}

	private void LateUpdate()
	{
		Transform transform = base.transform;
		Transform target = GetTarget();
		Vector2 vector2;
		float time;
		if ((bool)target)
		{
			Vector2 vector = transform.position;
			vector2 = (Vector2)target.position - vector;
			float magnitude = vector2.magnitude;
			IsTargetIntersecting = fadeRange.IsInRange(magnitude);
			time = fadeRange.GetTBetween(magnitude);
		}
		else
		{
			IsTargetIntersecting = false;
			time = 1f;
			vector2 = Vector2.zero;
		}
		float num = initialAlpha;
		if ((bool)fadeGroup)
		{
			num = fadeCurve.Evaluate(time) * initialAlpha;
			if (num <= 0.001f)
			{
				num = 0f;
			}
			fadeGroup.AlphaSelf = ((Math.Abs(fadeSpeed) <= Mathf.Epsilon) ? num : Mathf.Lerp(fadeGroup.AlphaSelf, num, fadeSpeed * Time.deltaTime));
		}
		if ((bool)target)
		{
			float z = vector2.normalized.DirectionToAngle();
			if (Math.Abs(num) <= 0.001f)
			{
				transform.rotation = Quaternion.Euler(0f, 0f, z);
			}
			else
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, z), rotateSpeed * Time.deltaTime);
			}
		}
	}

	private Transform GetTarget()
	{
		switch (rotateTowards)
		{
		case RotateTowardsTargets.None:
			return null;
		case RotateTowardsTargets.Hero:
			return HeroController.instance.transform;
		case RotateTowardsTargets.ClosestEnemy:
		{
			HealthManager healthManager = HealthManager.EnumerateActiveEnemies().FirstOrDefault();
			if (!(healthManager != null))
			{
				return null;
			}
			return healthManager.transform;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
