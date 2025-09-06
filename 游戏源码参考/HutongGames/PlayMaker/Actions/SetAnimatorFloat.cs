using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the value of a float parameter")]
	public class SetAnimatorFloat : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.AnimatorFloat)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		[Tooltip("The float value to assign to the animator parameter")]
		public FsmFloat Value;

		[Tooltip("Optional: The time allowed to parameter to reach the value. Requires Every Frame to be checked.")]
		public FsmFloat dampTime;

		private string cachedParameter;

		private int paramID;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			parameter = null;
			dampTime = new FsmFloat
			{
				UseVariable = true
			};
			Value = null;
		}

		public override void OnEnter()
		{
			SetParameter();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			SetParameter();
		}

		private void SetParameter()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			if (cachedParameter != parameter.Value)
			{
				cachedParameter = parameter.Value;
				paramID = Animator.StringToHash(parameter.Value);
			}
			if (dampTime.Value > 0f)
			{
				animator.SetFloat(paramID, Value.Value, dampTime.Value, Time.deltaTime);
			}
			else
			{
				animator.SetFloat(paramID, Value.Value);
			}
		}
	}
}
