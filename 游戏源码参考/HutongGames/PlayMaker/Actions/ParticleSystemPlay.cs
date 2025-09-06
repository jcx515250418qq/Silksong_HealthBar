using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Plays a ParticleSystem. Optionally destroy the GameObject when the ParticleSystem is finished.")]
	public class ParticleSystemPlay : ComponentAction<ParticleSystem>
	{
		[RequiredField]
		[Tooltip("The GameObject with the ParticleSystem.")]
		[CheckForComponent(typeof(ParticleSystem))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Play ParticleSystems on all child GameObjects too.")]
		public FsmBool withChildren;

		[Tooltip("''With Children'' can be quite expensive since it has to find Particle Systems in all children. If the hierarchy doesn't change, use Cache Children to speed this up.")]
		public FsmBool cacheChildren;

		[Tooltip("Stop playing when state exits")]
		public FsmBool stopOnExit;

		[Tooltip("Destroy the GameObject when the ParticleSystem has finished playing. 'With Children' means all child particle systems also need to finish.")]
		public FsmBool destroyOnFinish;

		private GameObject go;

		private ParticleSystem[] childParticleSystems;

		public override void Reset()
		{
			gameObject = null;
			withChildren = null;
			cacheChildren = null;
			destroyOnFinish = null;
		}

		public override void Awake()
		{
			if (withChildren.Value && cacheChildren.Value)
			{
				go = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (UpdateCache(go))
				{
					childParticleSystems = go.GetComponentsInChildren<ParticleSystem>();
				}
			}
		}

		public override void OnEnter()
		{
			DoParticleSystemPlay();
			if (!destroyOnFinish.Value)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (!stopOnExit.Value)
			{
				return;
			}
			if (withChildren.Value && cacheChildren.Value)
			{
				cachedComponent.Stop(withChildren: false);
				for (int i = 0; i < childParticleSystems.Length; i++)
				{
					ParticleSystem particleSystem = childParticleSystems[i];
					if (particleSystem != null)
					{
						particleSystem.Stop(withChildren: false);
					}
				}
			}
			else
			{
				cachedComponent.Stop(withChildren.Value);
			}
		}

		public override void OnUpdate()
		{
			if (withChildren.Value && cacheChildren.Value)
			{
				if (cachedComponent.IsAlive(withChildren: false))
				{
					return;
				}
				for (int i = 0; i < childParticleSystems.Length; i++)
				{
					ParticleSystem particleSystem = childParticleSystems[i];
					if (particleSystem != null && particleSystem.IsAlive(withChildren: false))
					{
						return;
					}
				}
			}
			else if (cachedComponent.IsAlive(withChildren.Value))
			{
				return;
			}
			Object.Destroy(go);
			Finish();
		}

		private void DoParticleSystemPlay()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(go))
			{
				return;
			}
			if (withChildren.Value && cacheChildren.Value)
			{
				cachedComponent.Play(withChildren: false);
				for (int i = 0; i < childParticleSystems.Length; i++)
				{
					ParticleSystem particleSystem = childParticleSystems[i];
					if (particleSystem != null)
					{
						particleSystem.Play(withChildren: false);
					}
				}
			}
			else
			{
				cachedComponent.Play(withChildren.Value);
			}
		}
	}
}
