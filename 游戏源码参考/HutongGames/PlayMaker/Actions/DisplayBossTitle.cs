using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class DisplayBossTitle : FsmStateAction
	{
		public FsmGameObject areaTitleObject;

		public FsmBool displayRight;

		public FsmString bossTitle;

		public override void Reset()
		{
			areaTitleObject = null;
			displayRight = null;
			bossTitle = null;
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(bossTitle.Value))
			{
				Finish();
				return;
			}
			GameObject gameObject = ManagerSingleton<AreaTitle>.Instance.gameObject;
			areaTitleObject.Value = gameObject;
			PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(gameObject, "Area Title Control");
			gameObject.SetActive(value: false);
			gameObjectFsm.FsmVariables.FindFsmBool("Visited").Value = true;
			gameObjectFsm.FsmVariables.FindFsmBool("Display Right").Value = displayRight.Value;
			gameObjectFsm.FsmVariables.FindFsmString("Area Event").Value = bossTitle.Value;
			gameObjectFsm.FsmVariables.FindFsmBool("NPC Title").Value = false;
			gameObject.SetActive(value: true);
			Finish();
		}
	}
}
