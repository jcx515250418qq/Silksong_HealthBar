using TeamCherry.SharedUtils;
using UnityEngine;

public class SpikeCogFall : MonoBehaviour
{
	[SerializeField]
	private GameObject[] disableOnBounce;

	[SerializeField]
	private float lowerYLimit;

	[Space]
	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private MinMaxFloat bounceZSpeed;

	[SerializeField]
	private float foregroundZLimit;

	[Space]
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private MinMaxFloat bounceImpulse;

	[Space]
	[SerializeField]
	private GameObject heroDamager;

	[SerializeField]
	private TrackTriggerObjects heroSafeRange;

	private bool hasBounced;

	private bool hasDisabled;

	private float zSpeed;

	private float bouncedZ;

	private Color initialColor;

	private void OnValidate()
	{
		if (foregroundZLimit < 0f)
		{
			foregroundZLimit = 0f;
		}
	}

	private void Awake()
	{
		if (!body)
		{
			SetBounced();
		}
		if ((bool)sprite)
		{
			initialColor = sprite.color;
		}
		if ((bool)heroSafeRange)
		{
			heroSafeRange.InsideStateChanged += OnHeroSafeRangeInsideStateChanged;
		}
	}

	private void OnDestroy()
	{
		if ((bool)heroSafeRange)
		{
			heroSafeRange.InsideStateChanged -= OnHeroSafeRangeInsideStateChanged;
		}
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		if (position.y < -10f)
		{
			base.gameObject.SetActive(value: false);
		}
		if (!hasBounced)
		{
			return;
		}
		if (!hasDisabled && position.y < lowerYLimit)
		{
			disableOnBounce.SetAllActive(value: false);
			hasDisabled = true;
			if ((bool)heroDamager)
			{
				heroDamager.SetActive(value: false);
			}
		}
		position.z += zSpeed * Time.deltaTime;
		base.transform.position = position;
		if ((bool)sprite)
		{
			float f = position.z - bouncedZ;
			if (zSpeed < 0f)
			{
				float t = Mathf.Clamp01(Mathf.Abs(f) / foregroundZLimit);
				sprite.color = Color.Lerp(initialColor, Color.black, t);
			}
		}
	}

	private void OnCollisionEnter2D()
	{
		disableOnBounce.SetAllActive(value: false);
		hasDisabled = true;
		if ((bool)heroDamager)
		{
			heroDamager.SetActive(value: false);
		}
		if ((bool)body)
		{
			Vector2 linearVelocity = body.linearVelocity;
			linearVelocity.y = bounceImpulse.GetRandomValue();
			body.linearVelocity = linearVelocity;
		}
		SetBounced();
	}

	private void SetBounced()
	{
		hasBounced = true;
		bouncedZ = base.transform.position.z;
		zSpeed = ((Random.Range(0, 2) == 0) ? bounceZSpeed.Start : bounceZSpeed.End);
	}

	private void OnHeroSafeRangeInsideStateChanged(bool isInside)
	{
		if (base.isActiveAndEnabled && !hasDisabled && (bool)heroDamager)
		{
			heroDamager.SetActive(!isInside);
		}
	}
}
