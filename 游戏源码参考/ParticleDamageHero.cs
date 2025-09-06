using UnityEngine;

public class ParticleDamageHero : MonoBehaviour
{
	private ParticleSystem system;

	private void Start()
	{
		HeroBox heroBox = HeroController.instance.heroBox;
		system = GetComponent<ParticleSystem>();
		ParticleSystem.TriggerModule trigger = system.trigger;
		trigger.enabled = true;
		while (trigger.colliderCount > 0)
		{
			trigger.RemoveCollider(0);
		}
		trigger.AddCollider(heroBox.GetComponent<Collider2D>());
		trigger.inside = ParticleSystemOverlapAction.Ignore;
		trigger.outside = ParticleSystemOverlapAction.Ignore;
		trigger.enter = ParticleSystemOverlapAction.Callback;
		trigger.exit = ParticleSystemOverlapAction.Ignore;
		trigger.colliderQueryMode = ParticleSystemColliderQueryMode.One;
		trigger.radiusScale = system.collision.radiusScale;
	}

	private void OnParticleTrigger()
	{
		if (system.GetSafeTriggerParticlesSize(ParticleSystemTriggerEventType.Enter) > 0)
		{
			HeroBox heroBox = HeroController.instance.heroBox;
			if (!HeroBox.Inactive)
			{
				heroBox.CheckForDamage(base.gameObject);
			}
		}
	}
}
