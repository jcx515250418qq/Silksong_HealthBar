using System.Collections.Generic;
using UnityEngine;

public class EnemyCoalBurn : MonoBehaviour
{
	private struct Effects
	{
		public PlayParticleEffects Particles;

		public FollowTransform Follow;
	}

	[SerializeField]
	private PlayParticleEffects burningEffectPrefab;

	private Dictionary<GameObject, Effects> currentlyBurning = new Dictionary<GameObject, Effects>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer != 11 || currentlyBurning.ContainsKey(collision.gameObject))
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		HealthManager component = gameObject.GetComponent<HealthManager>();
		if (!component || !component.ImmuneToCoal)
		{
			Vector3 position = collision.transform.position;
			PlayParticleEffects playParticleEffects = burningEffectPrefab.Spawn();
			playParticleEffects.StopParticleSystems();
			playParticleEffects.ClearParticleSystems();
			Collider2D component2 = gameObject.GetComponent<Collider2D>();
			if ((bool)component2)
			{
				Bounds bounds = component2.bounds;
				float num = position.y - bounds.min.y;
				playParticleEffects.transform.SetPosition2D(position.x, position.y - num);
			}
			FollowTransform component3 = playParticleEffects.GetComponent<FollowTransform>();
			if ((bool)component3)
			{
				component3.Target = gameObject.transform;
			}
			playParticleEffects.PlayParticleSystems();
			currentlyBurning.Add(collision.gameObject, new Effects
			{
				Particles = playParticleEffects,
				Follow = component3
			});
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 11 && currentlyBurning.ContainsKey(collision.gameObject))
		{
			Effects effects = currentlyBurning[collision.gameObject];
			effects.Particles.StopParticleSystems();
			if ((bool)effects.Follow)
			{
				effects.Follow.Target = null;
			}
			currentlyBurning.Remove(collision.gameObject);
		}
	}
}
