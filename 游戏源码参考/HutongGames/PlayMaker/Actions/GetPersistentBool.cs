using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetPersistentBool : FsmStateAction
	{
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			storeValue = null;
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				PersistentBoolItem component = gameObject.GetComponent<PersistentBoolItem>();
				storeValue.Value = component.ItemData.Value;
			}
			Finish();
		}
	}
}
