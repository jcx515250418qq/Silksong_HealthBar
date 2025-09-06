using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public class SetNPCEncounterState : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NPCEncounterStateController))]
		public FsmOwnerDefault Target;

		[ObjectType(typeof(NPCEncounterState))]
		public new FsmEnum State;

		public override void Reset()
		{
			Target = null;
			State = null;
		}

		public override void OnEnter()
		{
			NPCEncounterStateController safe = Target.GetSafe<NPCEncounterStateController>(this);
			if (safe != null)
			{
				safe.SetState((NPCEncounterState)(object)State.Value);
			}
			Finish();
		}
	}
}
