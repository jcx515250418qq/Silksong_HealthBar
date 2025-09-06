using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Adds a PlayMakerArrayList Component to a Game Object. Use this to create arrayList on the fly instead of during authoring.\n Optionally remove the ArrayList component on exiting the state.\n Simply point to existing if the reference exists already.")]
	public class ArrayListCreate : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject to add the PlayMaker ArrayList Proxy component to")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker arrayList proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		[Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the arrayList exists already")]
		public FsmEvent alreadyExistsEvent;

		private PlayMakerArrayListProxy addedComponent;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			alreadyExistsEvent = null;
		}

		public override void OnEnter()
		{
			DoAddPlayMakerArrayList();
			Finish();
		}

		public override void OnExit()
		{
			if (removeOnExit.Value && addedComponent != null)
			{
				Object.Destroy(addedComponent);
			}
		}

		private void DoAddPlayMakerArrayList()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (GetArrayListProxyPointer(ownerDefaultTarget, reference.Value, silent: true) != null)
			{
				base.Fsm.Event(alreadyExistsEvent);
				return;
			}
			addedComponent = ownerDefaultTarget.AddComponent<PlayMakerArrayListProxy>();
			if (addedComponent == null)
			{
				LogError("Can't add PlayMakerArrayListProxy");
			}
			else
			{
				addedComponent.referenceName = reference.Value;
			}
		}
	}
}
