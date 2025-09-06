using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to a Float Variable.")]
	public class FloatAddV2 : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to add to.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("Amount to add.")]
		public FsmFloat add;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		[Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
		public bool perSecond;

		public bool fixedUpdate;

		[UIHint(UIHint.Variable)]
		public FsmBool activeBool;

		public override void Reset()
		{
			floatVariable = null;
			add = null;
			everyFrame = false;
			perSecond = false;
			fixedUpdate = false;
			activeBool = false;
		}

		public override void Awake()
		{
			if (fixedUpdate)
			{
				base.Fsm.HandleFixedUpdate = true;
			}
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			DoFloatAdd();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!fixedUpdate)
			{
				DoFloatAdd();
			}
		}

		public override void OnFixedUpdate()
		{
			if (fixedUpdate)
			{
				DoFloatAdd();
			}
		}

		private void DoFloatAdd()
		{
			if (activeBool.IsNone || activeBool.Value)
			{
				if (!perSecond)
				{
					floatVariable.Value += add.Value;
				}
				else
				{
					floatVariable.Value += add.Value * Time.deltaTime;
				}
			}
		}
	}
}
