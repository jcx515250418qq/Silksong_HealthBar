namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Get Vector3 Length.")]
	public class GetVectorLength : FsmStateAction
	{
		public FsmVector3 vector3;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeLength;

		public override void Reset()
		{
			vector3 = null;
			storeLength = null;
		}

		public override void OnEnter()
		{
			DoVectorLength();
			Finish();
		}

		private void DoVectorLength()
		{
			if (vector3 != null && storeLength != null)
			{
				storeLength.Value = vector3.Value.magnitude;
			}
		}
	}
}
