using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DestroyPersistentBoolIfPdBool : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmString PdBoolName;

		public override void Reset()
		{
			Target = new FsmOwnerDefault();
			PdBoolName = null;
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(PdBoolName.Value))
			{
				Finish();
				return;
			}
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				PersistentBoolItem component = safe.GetComponent<PersistentBoolItem>();
				if ((bool)component)
				{
					Object.Destroy(component);
				}
			}
			Finish();
		}
	}
}
