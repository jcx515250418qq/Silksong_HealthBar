namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the maximum value within an arrayList. It can use float, int, vector2 and vector3 ( uses magnitude), rect ( uses surface), gameobject ( using bounding box volume), and string ( use lenght)")]
	public class ArrayListGetMaxValue : ArrayListActions
	{
		private static VariableType[] supportedTypes = new VariableType[7]
		{
			VariableType.Float,
			VariableType.Int,
			VariableType.Rect,
			VariableType.Vector2,
			VariableType.Vector3,
			VariableType.GameObject,
			VariableType.String
		};

		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("Performs every frame. WARNING, it could be affecting performances.")]
		public bool everyframe;

		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Maximum Value")]
		public FsmVar maximumValue;

		[UIHint(UIHint.Variable)]
		[Tooltip("The index of the Maximum Value within that arrayList")]
		public FsmInt maximumValueIndex;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			maximumValue = null;
			maximumValueIndex = null;
			everyframe = true;
		}

		public override void OnEnter()
		{
			if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				Finish();
			}
			DoFindMaximumValue();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFindMaximumValue();
		}

		private void DoFindMaximumValue()
		{
			if (!isProxyValid())
			{
				return;
			}
			VariableType type = maximumValue.Type;
			if (!supportedTypes.Contains(maximumValue.Type))
			{
				return;
			}
			float num = float.NegativeInfinity;
			int num2 = 0;
			int num3 = 0;
			foreach (object array in proxy.arrayList)
			{
				float floatFromObject = PlayMakerUtils.GetFloatFromObject(array, type, fastProcessingIfPossible: true);
				if (num < floatFromObject)
				{
					num = floatFromObject;
					num2 = num3;
				}
				num3++;
			}
			maximumValueIndex.Value = num2;
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, maximumValue, proxy.arrayList[num2]);
		}

		public override string ErrorCheck()
		{
			if (!supportedTypes.Contains(maximumValue.Type))
			{
				return "A " + maximumValue.Type.ToString() + " can not be processed as a minimum";
			}
			return "";
		}
	}
}
