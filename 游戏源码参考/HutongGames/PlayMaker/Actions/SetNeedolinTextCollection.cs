using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetNeedolinTextCollection : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NeedolinTextOwner))]
		public FsmOwnerDefault Target;

		[ObjectType(typeof(LocalisedTextCollection))]
		public FsmObject Collection;

		public override void Reset()
		{
			Target = null;
			Collection = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				NeedolinTextOwner component = safe.GetComponent<NeedolinTextOwner>();
				if ((bool)component)
				{
					component.SetTextCollection(Collection.Value as LocalisedTextCollection);
				}
			}
			Finish();
		}
	}
}
