using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the value of a Game Object Variable.")]
	public class SetGameObjectSelf : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject variable;

		public FsmOwnerDefault gameObject;

		public bool everyFrame;

		public override void Reset()
		{
			variable = null;
			gameObject = new FsmOwnerDefault();
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject safe = gameObject.GetSafe(this);
			if (safe != null)
			{
				variable.Value = safe;
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GameObject safe = gameObject.GetSafe(this);
			if (safe != null)
			{
				variable.Value = safe;
			}
		}
	}
}
