using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetSpriteValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject Variable;

		[RequiredField]
		[ObjectType(typeof(Sprite))]
		public FsmObject SetValue;

		public bool EveryFrame;

		public override void Reset()
		{
			Variable = null;
			SetValue = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoSet();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSet();
		}

		private void DoSet()
		{
			Variable.Value = SetValue.Value;
		}
	}
}
