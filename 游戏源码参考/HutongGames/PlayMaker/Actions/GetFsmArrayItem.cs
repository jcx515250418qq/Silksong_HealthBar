using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Gets an item in an Array Variable in another FSM.")]
	public class GetFsmArrayItem : BaseFsmVariableIndexAction
	{
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		[RequiredField]
		[UIHint(UIHint.FsmArray)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		[Tooltip("The index into the array.")]
		public FsmInt index;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the value of the array at the specified index.")]
		public FsmVar storeValue;

		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			fsmName = "";
			storeValue = null;
		}

		public override void OnEnter()
		{
			DoGetFsmArray();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoGetFsmArray()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget, fsmName.Value))
			{
				return;
			}
			FsmArray fsmArray = fsm.FsmVariables.GetFsmArray(variableName.Value);
			if (fsmArray != null)
			{
				if (index.Value < 0 || index.Value >= fsmArray.Length)
				{
					base.Fsm.Event(indexOutOfRange);
					Finish();
				}
				else if (fsmArray.ElementType == storeValue.NamedVar.VariableType)
				{
					storeValue.SetValue(fsmArray.Get(index.Value));
				}
				else
				{
					LogWarning("Incompatible variable type: " + variableName.Value);
				}
			}
			else
			{
				DoVariableNotFound(variableName.Value);
			}
		}

		public override void OnUpdate()
		{
			DoGetFsmArray();
		}
	}
}
