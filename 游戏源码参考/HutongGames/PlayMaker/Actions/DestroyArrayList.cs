using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Destroys a PlayMakerArrayListProxy Component of a Game Object.")]
	public class DestroyArrayList : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the ArrayList proxy component is destroyed")]
		public FsmEvent successEvent;

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the ArrayList proxy component was not found")]
		public FsmEvent notFoundEvent;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			successEvent = null;
			notFoundEvent = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				DoDestroyArrayList();
			}
			else
			{
				base.Fsm.Event(notFoundEvent);
			}
			Finish();
		}

		private void DoDestroyArrayList()
		{
			PlayMakerArrayListProxy[] components = proxy.GetComponents<PlayMakerArrayListProxy>();
			foreach (PlayMakerArrayListProxy playMakerArrayListProxy in components)
			{
				if (playMakerArrayListProxy.referenceName == reference.Value)
				{
					Object.Destroy(playMakerArrayListProxy);
					base.Fsm.Event(successEvent);
					return;
				}
			}
			base.Fsm.Event(notFoundEvent);
		}
	}
}
