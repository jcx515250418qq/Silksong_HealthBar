namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class DisplayNPCTitle : FsmStateAction
	{
		public FsmGameObject areaTitleObject;

		public FsmBool displayRight;

		public FsmString npcTitle;

		public override void Reset()
		{
			areaTitleObject = null;
			displayRight = null;
			npcTitle = null;
		}

		public override void OnEnter()
		{
			try
			{
				areaTitleObject.Value = ManagerSingleton<AreaTitle>.Instance.gameObject;
				PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(areaTitleObject.Value, "Area Title Control");
				areaTitleObject.Value.SetActive(value: false);
				gameObjectFsm.FsmVariables.FindFsmBool("Visited").Value = true;
				gameObjectFsm.FsmVariables.FindFsmBool("NPC Title").Value = true;
				gameObjectFsm.FsmVariables.FindFsmBool("Display Right").Value = displayRight.Value;
				gameObjectFsm.FsmVariables.FindFsmString("Area Event").Value = npcTitle.Value;
				areaTitleObject.Value.SetActive(value: true);
			}
			finally
			{
				Finish();
			}
		}
	}
}
