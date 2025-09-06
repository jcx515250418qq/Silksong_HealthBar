using TeamCherry.SharedUtils;
using UnityEngine;

public class SpriteFlashDistanceSilhouette : MonoBehaviour
{
	[SerializeField]
	private SpriteFlash spriteFlash;

	[SerializeField]
	private MinMaxFloat distanceRange;

	[SerializeField]
	private Color color;

	[SerializeField]
	private AnimationCurve colorMixCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private Transform hero;

	private void OnValidate()
	{
		if (distanceRange.Start < 0f)
		{
			distanceRange.Start = 0f;
		}
		if (distanceRange.End < distanceRange.Start)
		{
			distanceRange.End = distanceRange.Start;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, distanceRange.Start);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, distanceRange.End);
		if (!hero)
		{
			HeroController silentInstance = HeroController.SilentInstance;
			if ((bool)silentInstance)
			{
				hero = silentInstance.transform;
			}
		}
		if ((bool)hero)
		{
			float t = GetT();
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, distanceRange.GetLerpedValue(t));
		}
	}

	private void OnEnable()
	{
		if (!spriteFlash)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		hero = HeroController.instance.transform;
	}

	private void Update()
	{
		float t = GetT();
		spriteFlash.ExtraMixColor = color;
		spriteFlash.ExtraMixAmount = colorMixCurve.Evaluate(t);
	}

	private float GetT()
	{
		Vector2 a = hero.position;
		Vector2 b = base.transform.position;
		return (Mathf.Clamp(Vector2.Distance(a, b), distanceRange.Start, distanceRange.End) - distanceRange.Start) / (distanceRange.End - distanceRange.Start);
	}
}
