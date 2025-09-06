using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetHeroAffectedByGravity : FsmStateAction
	{
		public FsmFloat SetTime;

		public FsmBool CanSet;

		[RequiredField]
		public FsmBool SetAffectedByGravity;

		public override void Reset()
		{
			SetTime = null;
			CanSet = new FsmBool
			{
				UseVariable = true
			};
			SetAffectedByGravity = null;
		}

		public override void OnEnter()
		{
			if (SetTime.Value <= 0f)
			{
				Set();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		public override void OnExit()
		{
			if (!base.Finished)
			{
				Set();
			}
		}

		private void DoAction()
		{
			if (Time.time >= SetTime.Value)
			{
				Set();
			}
		}

		private void Set()
		{
			if (CanSet.IsNone || CanSet.Value)
			{
				HeroController.instance.AffectedByGravity(SetAffectedByGravity.Value);
				Finish();
			}
		}
	}
}
