using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to a Float Variable.")]
	public class FloatAddRandom : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to add to.")]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat addMin;

		[RequiredField]
		public FsmFloat addMax;

		public override void Reset()
		{
			floatVariable = null;
			addMin = null;
			addMax = null;
		}

		public override void OnEnter()
		{
			DoFloatAdd();
		}

		private void DoFloatAdd()
		{
			floatVariable.Value += Random.Range(addMin.Value, addMax.Value);
			Finish();
		}
	}
}
