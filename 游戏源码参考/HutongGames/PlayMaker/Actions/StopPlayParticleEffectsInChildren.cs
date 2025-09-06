using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class StopPlayParticleEffectsInChildren : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				PlayParticleEffects[] componentsInChildren = safe.GetComponentsInChildren<PlayParticleEffects>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].StopParticleSystems();
				}
			}
			Finish();
		}
	}
}
