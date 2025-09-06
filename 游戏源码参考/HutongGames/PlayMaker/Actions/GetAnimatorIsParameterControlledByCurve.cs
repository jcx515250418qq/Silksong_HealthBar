using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if a parameter is controlled by an additional curve on an animation")]
	public class GetAnimatorIsParameterControlledByCurve : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The parameter's name")]
		public FsmString parameterName;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if controlled by curve")]
		public FsmBool isControlledByCurve;

		[Tooltip("Event send if controlled by curve")]
		public FsmEvent isControlledByCurveEvent;

		[Tooltip("Event send if not controlled by curve")]
		public FsmEvent isNotControlledByCurveEvent;

		public override void Reset()
		{
			gameObject = null;
			parameterName = null;
			isControlledByCurve = null;
			isControlledByCurveEvent = null;
			isNotControlledByCurveEvent = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				bool flag = cachedComponent.IsParameterControlledByCurve(parameterName.Value);
				isControlledByCurve.Value = flag;
				base.Fsm.Event(flag ? isControlledByCurveEvent : isNotControlledByCurveEvent);
			}
			Finish();
		}
	}
}
