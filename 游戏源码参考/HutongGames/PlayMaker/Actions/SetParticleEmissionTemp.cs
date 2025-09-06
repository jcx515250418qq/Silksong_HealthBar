using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class SetParticleEmissionTemp : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool emission;

		public FsmFloat time;

		private float timer;

		private GameObject go;

		private bool ended;

		public override void Reset()
		{
			gameObject = null;
			emission = false;
			time = 1f;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				go = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (go != null)
				{
					ParticleSystem.EmissionModule emissionModule = go.GetComponent<ParticleSystem>().emission;
					emissionModule.enabled = emission.Value;
				}
			}
			timer = 0f;
			ended = false;
		}

		public override void OnUpdate()
		{
			if (timer < time.Value)
			{
				timer += Time.deltaTime;
			}
			if (timer >= time.Value)
			{
				ParticleSystem.EmissionModule emissionModule = go.GetComponent<ParticleSystem>().emission;
				emissionModule.enabled = !emission.Value;
				ended = true;
				Finish();
			}
		}

		public override void OnExit()
		{
			if (go != null && !ended)
			{
				ParticleSystem.EmissionModule emissionModule = go.GetComponent<ParticleSystem>().emission;
				emissionModule.enabled = !emission.Value;
			}
		}
	}
}
