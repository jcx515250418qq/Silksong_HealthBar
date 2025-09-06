using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the values of multiple variables in another FSM and store in variables of the same name in this FSM.")]
	public class GetFsmVariables : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		[RequiredField]
		[HideTypeFilter]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the values of the FsmVariables")]
		public FsmVar[] getVariables;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject cachedGO;

		private string cachedFsmName;

		private PlayMakerFSM sourceFsm;

		private INamedVariable[] sourceVariables;

		private NamedVariable[] targetVariables;

		public override void Reset()
		{
			gameObject = null;
			fsmName = "";
			getVariables = null;
		}

		private void InitFsmVars()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null || (!(ownerDefaultTarget != cachedGO) && !(cachedFsmName != fsmName.Value)))
			{
				return;
			}
			sourceVariables = new INamedVariable[getVariables.Length];
			targetVariables = new NamedVariable[getVariables.Length];
			for (int i = 0; i < getVariables.Length; i++)
			{
				string variableName = getVariables[i].variableName;
				sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, fsmName.Value);
				sourceVariables[i] = sourceFsm.FsmVariables.GetVariable(variableName);
				targetVariables[i] = base.Fsm.Variables.GetVariable(variableName);
				getVariables[i].Type = targetVariables[i].VariableType;
				if (!string.IsNullOrEmpty(variableName) && sourceVariables[i] == null)
				{
					LogWarning("Missing Variable: " + variableName);
				}
				cachedGO = ownerDefaultTarget;
				cachedFsmName = fsmName.Value;
			}
		}

		public override void OnEnter()
		{
			InitFsmVars();
			DoGetFsmVariables();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetFsmVariables();
		}

		private void DoGetFsmVariables()
		{
			InitFsmVars();
			for (int i = 0; i < getVariables.Length; i++)
			{
				getVariables[i].GetValueFrom(sourceVariables[i]);
				getVariables[i].ApplyValueTo(targetVariables[i]);
			}
		}
	}
}
