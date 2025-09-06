using System;
using System.Linq;
using UnityEngine;

public class ChainInteraction : ChainPushReaction, IHitResponder
{
	[Space]
	[SerializeField]
	private GameObject cutEffectPrefab;

	[SerializeField]
	private float cutDelay = 0.5f;

	[SerializeField]
	private TimerGroup hitEffectTimer;

	[Space]
	[SerializeField]
	private ParticleSystem brokenParticle;

	[SerializeField]
	private bool positionAtNail = true;

	[SerializeField]
	private float cutApplyForce = 10f;

	[SerializeField]
	private GameObject brokenObject;

	[SerializeField]
	private bool canBreak = true;

	private double nextCutTime;

	private Transform player;

	private float startAngle;

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		bool flag;
		if (damageInstance.IsNailDamage)
		{
			if (damageInstance.DamageDealt <= 0)
			{
				if (!damageInstance.CanWeakHit)
				{
					return IHitResponder.Response.None;
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (Time.timeAsDouble < nextCutTime)
		{
			return IHitResponder.Response.None;
		}
		nextCutTime = Time.timeAsDouble + (double)cutDelay;
		if (!player)
		{
			player = HeroController.instance.transform;
		}
		Vector3 position = damageInstance.Source.transform.position;
		float y = position.y;
		Vector3 position2 = base.transform.position;
		if ((bool)cutEffectPrefab && !flag && (!hitEffectTimer || hitEffectTimer.HasEnded))
		{
			ObjectPoolExtensions.Spawn(position: new Vector3(position2.x, y, position2.z), prefab: cutEffectPrefab);
			if ((bool)hitEffectTimer)
			{
				hitEffectTimer.ResetTimer();
			}
		}
		if ((bool)brokenParticle && !flag)
		{
			brokenParticle.gameObject.SetActive(value: false);
			Transform transform = brokenParticle.transform;
			Vector3 position4 = (positionAtNail ? new Vector3(position2.x, y, position2.z) : transform.position);
			Vector3 eulerAngles = transform.eulerAngles;
			if (Math.Abs(startAngle) < Mathf.Epsilon)
			{
				startAngle = eulerAngles.z;
			}
			eulerAngles.z = startAngle;
			if (base.transform.position.x > player.position.x)
			{
				eulerAngles.z += 180f;
			}
			transform.eulerAngles = eulerAngles;
			transform.position = position4;
			brokenParticle.gameObject.SetActive(value: true);
		}
		if (canBreak && !flag)
		{
			if ((bool)brokenObject)
			{
				brokenObject.SetActive(value: true);
				ApplyCollisionForce(brokenObject, damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular), cutApplyForce, y);
			}
			base.gameObject.SetActive(value: false);
			if ((bool)base.Sound)
			{
				base.Sound.PlayHitSound(position);
			}
		}
		else
		{
			ApplyCollisionForce(base.gameObject, damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular), cutApplyForce, y);
			DisableLinks(player);
			if ((bool)base.Sound)
			{
				base.Sound.PlayBrokenHitSound(position);
			}
		}
		return IHitResponder.Response.GenericHit;
	}

	public void ApplyCollisionForce(GameObject parent, Vector2 hitDirection, float cutForce, float hitYPos)
	{
		Rigidbody2D rigidbody2D = (from b in parent.GetComponentsInChildren<Rigidbody2D>()
			where !b.isKinematic
			orderby b.GetComponent<ChainLinkInteraction>() != null descending, Mathf.Abs(b.position.y - hitYPos)
			select b).FirstOrDefault();
		if (!(rigidbody2D == null))
		{
			rigidbody2D.linearVelocity = Vector2.zero;
			rigidbody2D.angularVelocity = 0f;
			rigidbody2D.AddForceAtPosition(hitDirection * cutForce, new Vector2(rigidbody2D.position.x, hitYPos), ForceMode2D.Impulse);
		}
	}
}
