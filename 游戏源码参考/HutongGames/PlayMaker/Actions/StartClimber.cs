using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class StartClimber : FsmStateAction
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
				Climber component = safe.GetComponent<Climber>();
				if ((bool)component)
				{
					component.enabled = true;
				}
			}
			Finish();
		}
	}
}
