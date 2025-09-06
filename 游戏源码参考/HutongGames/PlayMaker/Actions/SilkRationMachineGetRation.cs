using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SilkRationMachineGetRation : FsmStateAction
	{
		[CheckForComponent(typeof(SilkRationMachine))]
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmBool DidDropRation;

		private SilkRationMachine rationMachine;

		public override void Reset()
		{
			Target = null;
			DidDropRation = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			rationMachine = safe.GetComponent<SilkRationMachine>();
			if ((bool)rationMachine)
			{
				rationMachine.RationDropped += OnRationDropped;
				bool value = rationMachine.TryDropRation();
				if (!DidDropRation.IsNone)
				{
					DidDropRation.Value = value;
				}
			}
		}

		public override void OnExit()
		{
			UnsubscribeEvents();
		}

		private void OnRationDropped()
		{
			UnsubscribeEvents();
			Finish();
		}

		private void UnsubscribeEvents()
		{
			if ((bool)rationMachine)
			{
				rationMachine.RationDropped -= OnRationDropped;
				rationMachine = null;
			}
		}
	}
}
