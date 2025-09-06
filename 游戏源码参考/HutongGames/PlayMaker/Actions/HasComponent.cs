using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Checks if an Object has a Component. Optionally remove the Component on exiting the state.")]
	public class HasComponent : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to check.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.ScriptComponent)]
		[Tooltip("The name of the component to check for.")]
		public FsmString component;

		[Tooltip("Remove the component on exiting the state.")]
		public FsmBool removeOnExit;

		[Tooltip("Event to send if the Game Object has the component.")]
		public FsmEvent trueEvent;

		[Tooltip("Event to send if the Game Object does not have the component.")]
		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool store;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private Component aComponent;

		public override void Reset()
		{
			aComponent = null;
			gameObject = null;
			trueEvent = null;
			falseEvent = null;
			component = null;
			store = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoHasComponent((gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : gameObject.GameObject.Value);
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoHasComponent((gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : gameObject.GameObject.Value);
		}

		public override void OnExit()
		{
			if (removeOnExit.Value && aComponent != null)
			{
				Object.Destroy(aComponent);
			}
		}

		private void DoHasComponent(GameObject go)
		{
			if (go == null)
			{
				if (!store.IsNone)
				{
					store.Value = false;
				}
				base.Fsm.Event(falseEvent);
				return;
			}
			aComponent = go.GetComponent(ReflectionUtils.GetGlobalType(component.Value));
			if (!store.IsNone)
			{
				store.Value = aComponent != null;
			}
			base.Fsm.Event((aComponent != null) ? trueEvent : falseEvent);
		}
	}
}
