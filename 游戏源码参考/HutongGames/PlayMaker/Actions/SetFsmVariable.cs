using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a variable in another FSM.")]
	public class SetFsmVariable : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		[Tooltip("The name of the variable in the target FSM.")]
		public FsmString variableName;

		[RequiredField]
		[Tooltip("Set the value.")]
		public FsmVar setValue;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private PlayMakerFSM targetFsm;

		private NamedVariable targetVariable;

		private GameObject cachedGameObject;

		private string cachedFsmName;

		private string cachedVariableName;

		public override void Reset()
		{
			gameObject = null;
			fsmName = "";
			setValue = new FsmVar();
		}

		public override void OnEnter()
		{
			DoSetFsmVariable();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetFsmVariable();
		}

		private void DoSetFsmVariable()
		{
			if (setValue.IsNone || string.IsNullOrEmpty(variableName.Value))
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != cachedGameObject || fsmName.Value != cachedFsmName)
			{
				targetFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, fsmName.Value);
				if (targetFsm == null)
				{
					return;
				}
				cachedGameObject = ownerDefaultTarget;
				cachedFsmName = fsmName.Value;
				cachedVariableName = string.Empty;
			}
			if (variableName.Value != cachedVariableName)
			{
				targetVariable = targetFsm.FsmVariables.FindVariable(setValue.Type, variableName.Value);
				cachedVariableName = variableName.Value;
			}
			if (targetVariable == null)
			{
				LogWarning("Missing Variable: " + variableName.Value);
				return;
			}
			setValue.UpdateValue();
			setValue.ApplyValueTo(targetVariable);
		}
	}
}
