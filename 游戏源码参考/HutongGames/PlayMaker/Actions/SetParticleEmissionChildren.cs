using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetParticleEmissionChildren : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		public FsmBool SetEmission;

		public bool EveryFrame;

		public bool stopOnExit;

		private ParticleSystem[] childSystems;

		public override void Reset()
		{
			Target = null;
			SetEmission = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe == null)
			{
				Finish();
				return;
			}
			childSystems = safe.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			ParticleSystem[] array = childSystems;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.EmissionModule emission = array[i].emission;
				emission.enabled = SetEmission.Value;
			}
		}

		public override void OnExit()
		{
			if (stopOnExit)
			{
				ParticleSystem[] array = childSystems;
				for (int i = 0; i < array.Length; i++)
				{
					ParticleSystem.EmissionModule emission = array[i].emission;
					emission.enabled = false;
				}
			}
			base.OnExit();
		}
	}
}
