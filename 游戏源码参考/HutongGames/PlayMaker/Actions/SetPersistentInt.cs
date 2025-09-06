using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetPersistentInt : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmInt setValue;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			setValue = null;
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				gameObject.GetComponent<PersistentIntItem>().SetValueOverride(setValue.Value);
			}
			Finish();
		}
	}
}
