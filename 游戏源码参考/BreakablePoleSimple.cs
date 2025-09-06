using System;
using UnityEngine;

public class BreakablePoleSimple : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private GameObject bottom;

	[SerializeField]
	private GameObject top;

	[SerializeField]
	private float speed = 17f;

	[SerializeField]
	private float angleMin = 40f;

	[SerializeField]
	private float angleMax = 60f;

	[Space]
	[SerializeField]
	private GameObject slashEffectPrefab;

	[SerializeField]
	private float slashAngleMin = 340f;

	[SerializeField]
	private float slashAngleMax = 380f;

	[Space]
	[SerializeField]
	private float audioPitchMin = 0.85f;

	[SerializeField]
	private float audioPitchMax = 1.15f;

	private bool activated;

	private AudioSource source;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > 1f)
		{
			if ((bool)source)
			{
				source.enabled = false;
			}
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			base.enabled = false;
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (activated)
		{
			return IHitResponder.Response.None;
		}
		bool flag = false;
		float num = 1f;
		if (damageInstance.IsNailDamage)
		{
			float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			if (overriddenDirection < 45f)
			{
				flag = true;
				num = 1f;
			}
			else if (overriddenDirection < 135f)
			{
				flag = false;
			}
			else if (overriddenDirection < 225f)
			{
				flag = true;
				num = -1f;
			}
			else if (overriddenDirection < 360f)
			{
				flag = false;
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Spell)
		{
			flag = true;
		}
		if (!flag)
		{
			return IHitResponder.Response.None;
		}
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		if ((bool)bottom)
		{
			bottom.SetActive(value: true);
		}
		if ((bool)top)
		{
			top.SetActive(value: true);
			float num2 = UnityEngine.Random.Range(angleMin, angleMax);
			Vector2 linearVelocity = default(Vector2);
			linearVelocity.x = speed * Mathf.Cos(num2 * (MathF.PI / 180f)) * num;
			linearVelocity.y = speed * Mathf.Sin(num2 * (MathF.PI / 180f));
			Rigidbody2D component2 = top.GetComponent<Rigidbody2D>();
			if ((bool)component2)
			{
				component2.linearVelocity = linearVelocity;
			}
		}
		if ((bool)slashEffectPrefab)
		{
			GameObject obj = slashEffectPrefab.Spawn(base.transform.position);
			obj.transform.SetScaleX(num);
			obj.transform.SetRotationZ(UnityEngine.Random.Range(slashAngleMin, slashAngleMax));
		}
		if ((bool)source)
		{
			source.pitch = UnityEngine.Random.Range(audioPitchMin, audioPitchMax);
			source.Play();
		}
		activated = true;
		return IHitResponder.Response.GenericHit;
	}
}
