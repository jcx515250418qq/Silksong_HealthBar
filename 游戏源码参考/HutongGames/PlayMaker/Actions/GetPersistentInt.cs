using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetPersistentInt : FsmStateAction
	{
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;

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
				PersistentIntItem component = gameObject.GetComponent<PersistentIntItem>();
				storeValue.Value = component.ItemData.Value;
			}
			Finish();
		}
	}
}
