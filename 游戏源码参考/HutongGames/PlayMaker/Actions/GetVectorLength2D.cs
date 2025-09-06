namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Get Vector2 Length.")]
	public class GetVectorLength2D : FsmStateAction
	{
		public FsmVector2 Vector2;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreLength;

		public bool EveryFrame;

		public override void Reset()
		{
			Vector2 = null;
			StoreLength = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoVectorLength();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoVectorLength();
		}

		private void DoVectorLength()
		{
			if (Vector2 != null && StoreLength != null)
			{
				StoreLength.Value = Vector2.Value.magnitude;
			}
		}
	}
}
