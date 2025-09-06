namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Check if an item is contains in a particula PlayMaker ArrayList Proxy component")]
	public class ArrayListContains : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Data")]
		[RequiredField]
		[Tooltip("The variable to check.")]
		public FsmVar variable;

		[ActionSection("Result")]
		[Tooltip("Store in a bool wether it contains or not that element (described below)")]
		[UIHint(UIHint.Variable)]
		public FsmBool isContained;

		[Tooltip("Event sent if this arraList contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isContainedEvent;

		[Tooltip("Event sent if this arraList does not contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isNotContainedEvent;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			variable = null;
			isContained = null;
			isContainedEvent = null;
			isNotContainedEvent = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				doesArrayListContains();
			}
			Finish();
		}

		public void doesArrayListContains()
		{
			if (isProxyValid())
			{
				bool flag = false;
				PlayMakerUtils.RefreshValueFromFsmVar(base.Fsm, variable);
				switch (variable.Type)
				{
				case VariableType.Bool:
					flag = proxy.arrayList.Contains(variable.boolValue);
					break;
				case VariableType.Color:
					flag = proxy.arrayList.Contains(variable.colorValue);
					break;
				case VariableType.Float:
					flag = proxy.arrayList.Contains(variable.floatValue);
					break;
				case VariableType.GameObject:
					flag = proxy.arrayList.Contains(variable.gameObjectValue);
					break;
				case VariableType.Int:
					flag = proxy.arrayList.Contains(variable.intValue);
					break;
				case VariableType.Material:
					flag = proxy.arrayList.Contains(variable.materialValue);
					break;
				case VariableType.Object:
					flag = proxy.arrayList.Contains(variable.objectReference);
					break;
				case VariableType.Quaternion:
					flag = proxy.arrayList.Contains(variable.quaternionValue);
					break;
				case VariableType.Rect:
					flag = proxy.arrayList.Contains(variable.rectValue);
					break;
				case VariableType.String:
					flag = proxy.arrayList.Contains(variable.stringValue);
					break;
				case VariableType.Texture:
					flag = proxy.arrayList.Contains(variable.textureValue);
					break;
				case VariableType.Vector3:
					flag = proxy.arrayList.Contains(variable.vector3Value);
					break;
				case VariableType.Vector2:
					flag = proxy.arrayList.Contains(variable.vector2Value);
					break;
				}
				isContained.Value = flag;
				if (flag)
				{
					base.Fsm.Event(isContainedEvent);
				}
				else
				{
					base.Fsm.Event(isNotContainedEvent);
				}
			}
		}
	}
}
