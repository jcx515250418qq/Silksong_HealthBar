using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Finds an ArrayList by reference. Warning: this function can be very slow.")]
	public class FindArrayList : CollectionsActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component")]
		public FsmString ArrayListReference;

		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("Store the GameObject hosting the PlayMaker ArrayList Proxy component here")]
		public FsmGameObject store;

		public FsmEvent foundEvent;

		public FsmEvent notFoundEvent;

		public override void Reset()
		{
			ArrayListReference = "";
			store = null;
			foundEvent = null;
			notFoundEvent = null;
		}

		public override void OnEnter()
		{
			PlayMakerArrayListProxy[] array = Object.FindObjectsOfType(typeof(PlayMakerArrayListProxy)) as PlayMakerArrayListProxy[];
			foreach (PlayMakerArrayListProxy playMakerArrayListProxy in array)
			{
				if (playMakerArrayListProxy.referenceName == ArrayListReference.Value)
				{
					store.Value = playMakerArrayListProxy.gameObject;
					base.Fsm.Event(foundEvent);
					return;
				}
			}
			store.Value = null;
			base.Fsm.Event(notFoundEvent);
			Finish();
		}
	}
}
