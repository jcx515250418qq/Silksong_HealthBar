using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Store all active GameObjects with a specific tag. Tags must be declared in the tag manager before using them")]
	public class ArrayListFindGameObjectsByTag : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("the tag")]
		public FsmString tag;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			tag = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				FindGOByTag();
			}
			Finish();
		}

		public void FindGOByTag()
		{
			if (isProxyValid())
			{
				proxy.arrayList.Clear();
				GameObject[] c = GameObject.FindGameObjectsWithTag(tag.Value);
				proxy.arrayList.InsertRange(0, c);
			}
		}
	}
}
