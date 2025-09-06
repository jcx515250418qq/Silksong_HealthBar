using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HeroTouchForce : MonoBehaviour
{
	[SerializeField]
	private float touchTime;

	[SerializeField]
	private float playerRangeEnable;

	[SerializeField]
	private float playerRangeEnableDelay;

	private float disableTimeLeft;

	private float enableTimeLeft;

	private Transform hero;

	private Collider2D collider;

	private void Awake()
	{
		collider = GetComponent<Collider2D>();
		if ((bool)collider && !base.transform.IsOnHeroPlane())
		{
			collider.enabled = false;
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		hero = HeroController.instance.transform;
	}

	private void OnCollisionEnter2D()
	{
		if (disableTimeLeft <= 0f)
		{
			disableTimeLeft = touchTime;
		}
	}

	private void Update()
	{
		if (collider.enabled)
		{
			if (disableTimeLeft > 0f)
			{
				disableTimeLeft -= Time.deltaTime;
				if (disableTimeLeft <= 0f)
				{
					collider.enabled = false;
				}
			}
		}
		else if (Mathf.Abs(hero.position.x - base.transform.position.x) < playerRangeEnable)
		{
			enableTimeLeft = playerRangeEnableDelay;
		}
		else if (enableTimeLeft > 0f)
		{
			enableTimeLeft -= Time.deltaTime;
			if (enableTimeLeft <= 0f)
			{
				collider.enabled = true;
			}
		}
	}
}
