using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class StopClimber : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool WaitForTurn;

		private Climber climber;

		public override void Reset()
		{
			Target = null;
			WaitForTurn = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				climber = safe.GetComponent<Climber>();
			}
			if (climber == null)
			{
				Finish();
			}
			else if (CanStop())
			{
				Stop();
			}
		}

		public override void OnUpdate()
		{
			if (CanStop())
			{
				Stop();
			}
		}

		private void Stop()
		{
			climber.enabled = false;
			Finish();
		}

		private bool CanStop()
		{
			if (WaitForTurn.Value)
			{
				return !climber.IsTurning;
			}
			return true;
		}
	}
}
