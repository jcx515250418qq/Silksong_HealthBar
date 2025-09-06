using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Destroys a PlayMakerHashTableProxy Component of a Game Object.")]
	public class DestroyHashTable : HashTableActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the HashTable proxy component is destroyed")]
		public FsmEvent successEvent;

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the HashTable proxy component was not found")]
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
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (SetUpHashTableProxyPointer(ownerDefaultTarget, reference.Value))
			{
				DoDestroyHashTable(ownerDefaultTarget);
			}
			else
			{
				base.Fsm.Event(notFoundEvent);
			}
			Finish();
		}

		private void DoDestroyHashTable(GameObject go)
		{
			PlayMakerHashTableProxy[] components = proxy.GetComponents<PlayMakerHashTableProxy>();
			foreach (PlayMakerHashTableProxy playMakerHashTableProxy in components)
			{
				if (playMakerHashTableProxy.referenceName == reference.Value)
				{
					Object.Destroy(playMakerHashTableProxy);
					base.Fsm.Event(successEvent);
					return;
				}
			}
			base.Fsm.Event(notFoundEvent);
		}
	}
}
