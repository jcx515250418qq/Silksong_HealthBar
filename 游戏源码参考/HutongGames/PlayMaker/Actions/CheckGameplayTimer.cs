using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CheckGameplayTimer : FSMUtility.CheckFsmStateEveryFrameAction
	{
		public FsmOwnerDefault Target;

		private GameplayTimer timer;

		public override bool IsTrue
		{
			get
			{
				if ((bool)timer)
				{
					return timer.IsTimerComplete;
				}
				return false;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			timer = (safe ? safe.GetComponent<GameplayTimer>() : null);
			base.OnEnter();
		}
	}
}
