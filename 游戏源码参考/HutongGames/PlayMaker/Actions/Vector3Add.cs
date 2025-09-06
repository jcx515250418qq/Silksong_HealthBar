using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Adds a value to Vector3 Variable.")]
	public class Vector3Add : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 variable to add to.")]
		public FsmVector3 vector3Variable;

		[RequiredField]
		[Tooltip("Vector3 to add.")]
		public FsmVector3 addVector;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Add over one second (multiplies values by Time.deltaTime). Note: Needs Every Frame checked.")]
		public bool perSecond;

		public override void Reset()
		{
			vector3Variable = null;
			addVector = new FsmVector3
			{
				UseVariable = true
			};
			everyFrame = false;
			perSecond = false;
		}

		public override void OnEnter()
		{
			DoVector3Add();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoVector3Add();
		}

		private void DoVector3Add()
		{
			if (perSecond)
			{
				vector3Variable.Value += addVector.Value * Time.deltaTime;
			}
			else
			{
				vector3Variable.Value += addVector.Value;
			}
		}
	}
}
