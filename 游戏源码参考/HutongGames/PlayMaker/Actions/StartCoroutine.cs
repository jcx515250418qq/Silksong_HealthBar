using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Starts a Coroutine in a Behaviour on a Game Object.\nSee Unity <a href=\"http://unity3d.com/support/documentation/ScriptReference/MonoBehaviour.StartCoroutine.html\">StartCoroutine</a> docs for more details.")]
	public class StartCoroutine : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Behaviour)]
		[Tooltip("The Behaviour that contains the method to start as a coroutine.")]
		public FsmString behaviour;

		[RequiredField]
		[UIHint(UIHint.Coroutine)]
		[Tooltip("The name of the coroutine method.")]
		public FunctionCall functionCall;

		[Tooltip("Stop the coroutine when the state is exited.")]
		public bool stopOnExit;

		private MonoBehaviour component;

		public override void Reset()
		{
			gameObject = null;
			behaviour = null;
			functionCall = null;
			stopOnExit = false;
		}

		public override void OnEnter()
		{
			DoStartCoroutine();
			Finish();
		}

		private void DoStartCoroutine()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			component = ownerDefaultTarget.GetComponent(ReflectionUtils.GetGlobalType(behaviour.Value)) as MonoBehaviour;
			if (component == null)
			{
				LogWarning("StartCoroutine: " + ownerDefaultTarget.name + " missing behaviour: " + behaviour.Value);
				return;
			}
			switch (functionCall.ParameterType)
			{
			case "None":
				component.StartCoroutine(functionCall.FunctionName);
				break;
			case "int":
				component.StartCoroutine(functionCall.FunctionName, functionCall.IntParameter.Value);
				break;
			case "float":
				component.StartCoroutine(functionCall.FunctionName, functionCall.FloatParameter.Value);
				break;
			case "string":
				component.StartCoroutine(functionCall.FunctionName, functionCall.StringParameter.Value);
				break;
			case "bool":
				component.StartCoroutine(functionCall.FunctionName, functionCall.BoolParameter.Value);
				break;
			case "Vector2":
				component.StartCoroutine(functionCall.FunctionName, functionCall.Vector2Parameter.Value);
				break;
			case "Vector3":
				component.StartCoroutine(functionCall.FunctionName, functionCall.Vector3Parameter.Value);
				break;
			case "Rect":
				component.StartCoroutine(functionCall.FunctionName, functionCall.RectParamater.Value);
				break;
			case "GameObject":
				component.StartCoroutine(functionCall.FunctionName, functionCall.GameObjectParameter.Value);
				break;
			case "Material":
				component.StartCoroutine(functionCall.FunctionName, functionCall.MaterialParameter.Value);
				break;
			case "Texture":
				component.StartCoroutine(functionCall.FunctionName, functionCall.TextureParameter.Value);
				break;
			case "Quaternion":
				component.StartCoroutine(functionCall.FunctionName, functionCall.QuaternionParameter.Value);
				break;
			case "Object":
				component.StartCoroutine(functionCall.FunctionName, functionCall.ObjectParameter.Value);
				break;
			case "Enum":
				component.StartCoroutine(functionCall.FunctionName, functionCall.EnumParameter.Value);
				break;
			}
		}

		public override void OnExit()
		{
			if (!(component == null) && stopOnExit)
			{
				component.StopCoroutine(functionCall.FunctionName);
			}
		}
	}
}
