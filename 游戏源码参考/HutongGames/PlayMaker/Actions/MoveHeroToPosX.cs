using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class MoveHeroToPosX : FsmStateAction
	{
		public FsmOwnerDefault Hero;

		public FsmGameObject PositionTarget;

		[HideIf("IsUsingTarget")]
		public FsmFloat PositionX;

		public FsmEvent FinishEvent;

		private Coroutine moveRoutine;

		public bool IsUsingTarget()
		{
			return !PositionTarget.IsNone;
		}

		public override void Reset()
		{
			Hero = null;
			PositionTarget = new FsmGameObject
			{
				UseVariable = true
			};
			PositionX = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Hero.GetSafe(this);
			if ((bool)safe)
			{
				HeroController component = safe.GetComponent<HeroController>();
				if ((bool)component)
				{
					float targetX = PositionX.Value;
					if ((bool)PositionTarget.Value)
					{
						targetX = PositionTarget.Value.transform.position.x;
					}
					moveRoutine = StartCoroutine(component.MoveToPositionX(targetX, End));
					return;
				}
			}
			Finish();
		}

		private void End()
		{
			base.Fsm.Event(FinishEvent);
			Finish();
		}

		public override void OnExit()
		{
			if (moveRoutine != null)
			{
				StopCoroutine(moveRoutine);
				moveRoutine = null;
			}
		}
	}
}
