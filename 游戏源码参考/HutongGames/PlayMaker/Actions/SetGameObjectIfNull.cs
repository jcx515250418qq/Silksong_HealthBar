namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("CategoryName")]
	[Tooltip("TOOLTIP")]
	public class SetGameObjectIfNull : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmOwnerDefault replaceWith;

		public override void Reset()
		{
			target = null;
			replaceWith = null;
		}

		public override void OnEnter()
		{
			if (target.Value == null)
			{
				target.Value = replaceWith.GetSafe(this);
			}
			Finish();
		}
	}
}
