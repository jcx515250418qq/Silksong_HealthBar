using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Subtracts a value to an Integer Variable.")]
	public class IntSubtract : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The int variable to subtract from.")]
		public FsmInt intVariable;

		[RequiredField]
		[Tooltip("Value to subtract from the int variable.")]
		public FsmInt subtract;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		[Tooltip("Used with Every Frame. Subtracts the value over one second to make the operation frame rate independent.")]
		public bool perSecond;

		private float _acc;

		public override void Reset()
		{
			intVariable = null;
			subtract = null;
			everyFrame = false;
			perSecond = false;
		}

		public override void OnEnter()
		{
			doSubtract();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			doSubtract();
		}

		private void doSubtract()
		{
			if (perSecond)
			{
				int num = Mathf.Abs(subtract.Value);
				_acc += (float)num * Time.deltaTime;
				if (_acc >= (float)num)
				{
					_acc = 0f;
					intVariable.Value -= subtract.Value;
				}
			}
			else
			{
				intVariable.Value -= subtract.Value;
			}
		}
	}
}
