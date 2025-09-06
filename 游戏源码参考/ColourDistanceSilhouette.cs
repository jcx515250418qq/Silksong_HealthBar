using TeamCherry.SharedUtils;
using UnityEngine;

public class ColourDistanceSilhouette : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite tk2dSprite;

	[SerializeField]
	private MinMaxFloat distanceRange;

	[SerializeField]
	private AnimationCurve distanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private Color targetColour = Color.black;

	[SerializeField]
	private bool isActive = true;

	private Color startColour;

	private Transform hero;

	private void Reset()
	{
		tk2dSprite = GetComponent<tk2dSprite>();
	}

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
		Vector3 position = base.transform.position;
		Gizmos.DrawWireSphere(position, distanceRange.Start);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(position, distanceRange.End);
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
			Gizmos.DrawWireSphere(position, distanceRange.GetLerpedValue(t));
		}
	}

	private void OnEnable()
	{
		if (!tk2dSprite)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		hero = HeroController.instance.transform;
		startColour = tk2dSprite.color;
	}

	private void Update()
	{
		if (isActive)
		{
			float t = distanceCurve.Evaluate(GetT());
			Color color = Color.Lerp(startColour, targetColour, t);
			tk2dSprite.color = color;
		}
	}

	private float GetT()
	{
		Vector2 a = hero.position;
		Vector2 b = base.transform.position;
		return (Mathf.Clamp(Vector2.Distance(a, b), distanceRange.Start, distanceRange.End) - distanceRange.Start) / (distanceRange.End - distanceRange.Start);
	}

	public void SetActive(bool setActive)
	{
		isActive = setActive;
	}
}
