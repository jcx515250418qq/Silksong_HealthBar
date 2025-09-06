using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroAddExtraAirMoveVelocityV2 : FsmStateAction
	{
		public FsmVector2 Velocity;

		public FsmFloat Decay;

		public FsmBool CancelOnTurn;

		[ObjectType(typeof(HeroController.DecayingVelocity.SkipBehaviours))]
		public FsmEnum SkipBehaviour;

		public override void Reset()
		{
			Velocity = null;
			Decay = 4f;
			CancelOnTurn = true;
			SkipBehaviour = HeroController.DecayingVelocity.SkipBehaviours.WhileMoving;
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
				SkipBehaviour = (HeroController.DecayingVelocity.SkipBehaviours)(object)SkipBehaviour.Value
			});
			Finish();
		}
	}
}
