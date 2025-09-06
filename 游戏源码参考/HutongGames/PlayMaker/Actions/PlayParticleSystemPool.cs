using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PlayParticleSystemPool : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				ParticleSystemPool component = safe.GetComponent<ParticleSystemPool>();
				if ((bool)component)
				{
					component.PlayParticles();
				}
			}
			Finish();
		}
	}
}
