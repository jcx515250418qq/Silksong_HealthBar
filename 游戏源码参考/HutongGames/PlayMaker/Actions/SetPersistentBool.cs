namespace HutongGames.PlayMaker.Actions
{
	public class SetPersistentBool : FsmStateAction
	{
		[RequiredField]
		[ObjectType(typeof(PersistentBoolItem))]
		public FsmOwnerDefault Target;

		public FsmBool SetValue;

		public override void Reset()
		{
			Target = null;
			SetValue = null;
		}

		public override void OnEnter()
		{
			PersistentBoolItem safe = Target.GetSafe<PersistentBoolItem>(this);
			if (safe != null)
			{
				safe.SetValueOverride(SetValue.Value);
			}
			Finish();
		}
	}
}
