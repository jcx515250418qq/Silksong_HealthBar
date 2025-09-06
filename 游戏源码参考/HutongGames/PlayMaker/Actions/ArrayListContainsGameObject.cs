using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Check if a GameObject ( by name and/or tag) is within an arrayList.")]
	public class ArrayListContainsGameObject : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("The name of the GameObject to find in the arrayList. You can leave this empty if you specify a Tag.")]
		public FsmString gameObjectName;

		[UIHint(UIHint.Tag)]
		[Tooltip("Find a GameObject in this arrayList with this tag. If GameObject Name is specified then both name and Tag must match.")]
		public FsmString withTag;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject variable.")]
		public FsmGameObject result;

		[UIHint(UIHint.Variable)]
		public FsmInt resultIndex;

		[Tooltip("Store in a bool wether it contains or not that GameObject")]
		[UIHint(UIHint.Variable)]
		public FsmBool isContained;

		[Tooltip("Event sent if this arraList contains that GameObject")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isContainedEvent;

		[Tooltip("Event sent if this arraList does not contains that GameObject")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isNotContainedEvent;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			gameObjectName = null;
			result = null;
			resultIndex = null;
			isContained = null;
			isContainedEvent = null;
			isNotContainedEvent = null;
		}

		public override void OnEnter()
		{
			if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				Finish();
			}
			int num = DoContainsGo();
			if (num >= 0)
			{
				isContained.Value = true;
				result.Value = (GameObject)proxy.arrayList[num];
				resultIndex.Value = num;
				base.Fsm.Event(isContainedEvent);
			}
			else
			{
				isContained.Value = false;
				base.Fsm.Event(isNotContainedEvent);
			}
			Finish();
		}

		private int DoContainsGo()
		{
			if (!isProxyValid())
			{
				return -1;
			}
			int num = 0;
			string value = gameObjectName.Value;
			string value2 = withTag.Value;
			foreach (GameObject array in proxy.arrayList)
			{
				if (array != null)
				{
					if (value2 == "Untagged" || withTag.IsNone)
					{
						if (array.name.Equals(value))
						{
							return num;
						}
					}
					else if (string.IsNullOrEmpty(value))
					{
						if (array.tag.Equals(value2))
						{
							return num;
						}
					}
					else if (array.name.Equals(value) && array.tag.Equals(value2))
					{
						return num;
					}
				}
				num++;
			}
			return -1;
		}
	}
}
