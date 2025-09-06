using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetBattleScene : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmGameObject battleScene;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			battleScene = new FsmGameObject();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SetBattleScene(battleScene.Value);
				}
			}
			Finish();
		}
	}
}
