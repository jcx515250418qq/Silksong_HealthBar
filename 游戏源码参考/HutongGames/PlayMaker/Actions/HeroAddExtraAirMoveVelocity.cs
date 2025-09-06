using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroAddExtraAirMoveVelocity : FsmStateAction
	{
		public FsmVector2 Velocity;

		public FsmFloat Decay;

		public FsmBool CancelOnTurn;

		public FsmBool SkipApplyWhileMoving;

		public override void Reset()
		{
			Velocity = null;
			Decay = 4f;
			CancelOnTurn = true;
			SkipApplyWhileMoving = true;
		}

		public override void OnEnter()
		{
			if (Velocity.Value.magnitude <= Mathf.Epsilon)
			{
				Finish();
				return;
			}
			HeroController.instance.AddExtraAirMoveVelocity(new HeroController.DecayingVelocity
			{
				Velocity = Velocity.Value,
				Decay = Decay.Value,
				CancelOnTurn = CancelOnTurn.Value,
				SkipBehaviour = (SkipApplyWhileMoving.Value ? HeroController.DecayingVelocity.SkipBehaviours.WhileMoving : HeroController.DecayingVelocity.SkipBehaviours.None)
			});
			Finish();
		}
	}
}
