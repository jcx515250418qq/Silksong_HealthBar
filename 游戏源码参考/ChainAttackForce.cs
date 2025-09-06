using UnityEngine;

public class ChainAttackForce : MonoBehaviour, IHitResponder
{
	public float force = 5f;

	public float delay = 0.25f;

	private double nextAttackTime;

	private ChainInteraction interaction;

	public bool HitRecurseUpwards => false;

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		if (Time.timeAsDouble < nextAttackTime)
		{
			return IHitResponder.Response.None;
		}
		nextAttackTime = Time.timeAsDouble + (double)delay;
		if (!interaction)
		{
			interaction = GetComponentInParent<ChainInteraction>();
		}
		if ((bool)interaction)
		{
			interaction.ApplyCollisionForce(interaction.gameObject, damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular), force, damageInstance.Source.transform.position.y);
		}
		return IHitResponder.Response.GenericHit;
	}
}
