using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Invokes a Method in a Behaviour attached to a Game Object. See Unity InvokeMethod docs.")]
	public class InvokeMethod : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object that owns the behaviour.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Script)]
		[Tooltip("The behaviour that contains the method.")]
		public FsmString behaviour;

		[RequiredField]
		[UIHint(UIHint.Method)]
		[Tooltip("The name of the method to invoke.")]
		public FsmString methodName;

		[HasFloatSlider(0f, 10f)]
		[Tooltip("Optional time delay in seconds.")]
		public FsmFloat delay;

		[Tooltip("Call the method repeatedly.")]
		public FsmBool repeating;

		[HasFloatSlider(0f, 10f)]
		[Tooltip("Delay between repeated calls in seconds.")]
		public FsmFloat repeatDelay;

		[Tooltip("Stop calling the method when the state is exited.")]
		public FsmBool cancelOnExit;

		private MonoBehaviour component;

		public override void Reset()
		{
			gameObject = null;
			behaviour = null;
			methodName = "";
			delay = null;
			repeating = false;
			repeatDelay = 1f;
			cancelOnExit = false;
		}

		public override void OnEnter()
		{
			DoInvokeMethod(base.Fsm.GetOwnerDefaultTarget(gameObject));
			Finish();
		}

		private void DoInvokeMethod(GameObject go)
		{
			if (!(go == null))
			{
				component = go.GetComponent(ReflectionUtils.GetGlobalType(behaviour.Value)) as MonoBehaviour;
				if (component == null)
				{
					LogWarning("InvokeMethod: " + go.name + " missing behaviour: " + behaviour.Value);
				}
				else if (repeating.Value)
				{
					component.InvokeRepeating(methodName.Value, delay.Value, repeatDelay.Value);
				}
				else
				{
					component.Invoke(methodName.Value, delay.Value);
				}
			}
		}

		public override void OnExit()
		{
			if (!(component == null) && cancelOnExit.Value)
			{
				component.CancelInvoke(methodName.Value);
			}
		}
	}
}
