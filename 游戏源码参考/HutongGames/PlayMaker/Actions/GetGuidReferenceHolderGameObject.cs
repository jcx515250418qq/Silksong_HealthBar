namespace HutongGames.PlayMaker.Actions
{
	public class GetGuidReferenceHolderGameObject : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreGameObject;

		public override void Reset()
		{
			Target = null;
			StoreGameObject = null;
		}

		public override void OnEnter()
		{
			GuidReferenceHolder component = Target.GetSafe(this).GetComponent<GuidReferenceHolder>();
			StoreGameObject.Value = component.ReferencedGameObject;
			Finish();
		}
	}
}
