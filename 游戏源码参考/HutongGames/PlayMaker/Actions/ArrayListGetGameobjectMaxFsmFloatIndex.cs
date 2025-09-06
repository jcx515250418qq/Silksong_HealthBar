using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Returns the Gameobject within an arrayList which have the max float value in its FSM")]
	public class ArrayListGetGameobjectMaxFsmFloatIndex : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		public FsmString variableName;

		public bool everyframe;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeMaxValue;

		[UIHint(UIHint.Variable)]
		public FsmGameObject maxGameObject;

		[UIHint(UIHint.Variable)]
		public FsmInt maxIndex;

		private GameObject goLastFrame;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			maxGameObject = null;
			maxIndex = null;
			everyframe = true;
			fsmName = "";
			storeMaxValue = null;
		}

		public override void OnEnter()
		{
			if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				Finish();
			}
			DoFindMaxGo();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFindMaxGo();
		}

		private void DoFindMaxGo()
		{
			float num = 0f;
			if (storeMaxValue.IsNone || !isProxyValid())
			{
				return;
			}
			int num2 = 0;
			foreach (GameObject array in proxy.arrayList)
			{
				if (array != null)
				{
					fsm = ActionHelpers.GetGameObjectFsm(array, fsmName.Value);
					if (fsm == null)
					{
						break;
					}
					FsmFloat fsmFloat = fsm.FsmVariables.GetFsmFloat(variableName.Value);
					if (fsmFloat == null)
					{
						break;
					}
					if (fsmFloat.Value > num)
					{
						storeMaxValue.Value = fsmFloat.Value;
						num = fsmFloat.Value;
						maxGameObject.Value = array;
						maxIndex.Value = num2;
					}
				}
				num2++;
			}
		}
	}
}
