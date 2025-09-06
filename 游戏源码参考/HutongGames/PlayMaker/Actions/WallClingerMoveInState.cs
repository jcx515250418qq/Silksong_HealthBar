using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class WallClingerMoveInState : FsmStateAction
	{
		public enum InitialMoveStates
		{
			Random = 0,
			Up = 1,
			Down = 2
		}

		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		private WallClinger clinger;

		[ObjectType(typeof(InitialMoveStates))]
		public FsmEnum InitialMoveState;

		public override void Reset()
		{
			Target = null;
			clinger = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				clinger = safe.GetComponent<WallClinger>();
			}
			if (!clinger)
			{
				Finish();
				return;
			}
			switch ((InitialMoveStates)(object)InitialMoveState.Value)
			{
			case InitialMoveStates.Random:
				clinger.IsActive = true;
				break;
			case InitialMoveStates.Up:
				clinger.StartMovingDirection(1);
				break;
			case InitialMoveStates.Down:
				clinger.StartMovingDirection(-1);
				break;
			}
		}

		public override void OnExit()
		{
			if ((bool)clinger)
			{
				clinger.IsActive = false;
			}
		}
	}
}
