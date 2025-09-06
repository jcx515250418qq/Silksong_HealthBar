using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class WallClingerSetMovementState : FsmStateAction
	{
		public enum MovementStates
		{
			Inactive = 0,
			Active = 1,
			MovingUp = 2,
			MovingDown = 3
		}

		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		private WallClinger clinger;

		[ObjectType(typeof(MovementStates))]
		public FsmEnum MovementState;

		public override void Reset()
		{
			Target = null;
			clinger = null;
			MovementState = null;
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
			switch ((MovementStates)(object)MovementState.Value)
			{
			case MovementStates.Inactive:
				clinger.IsActive = false;
				break;
			case MovementStates.Active:
				clinger.IsActive = true;
				break;
			case MovementStates.MovingUp:
				clinger.StartMovingDirection(1);
				break;
			case MovementStates.MovingDown:
				clinger.StartMovingDirection(-1);
				break;
			}
			Finish();
		}
	}
}
