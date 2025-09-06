using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Inventory")]
	public class OpenMarkerMenu : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				MapMarkerMenu component = gameObject.GetComponent<MapMarkerMenu>();
				if (component != null)
				{
					component.Open();
				}
			}
			Finish();
		}
	}
}
