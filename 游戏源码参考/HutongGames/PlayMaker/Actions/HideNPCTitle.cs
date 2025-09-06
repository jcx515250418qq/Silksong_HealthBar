namespace HutongGames.PlayMaker.Actions
{
	public class HideNPCTitle : FsmStateAction
	{
		public FsmGameObject AreaTitleObject;

		public override void Reset()
		{
			AreaTitleObject = null;
		}

		public override void OnEnter()
		{
			try
			{
				AreaTitleObject.Value = ManagerSingleton<AreaTitle>.Instance.gameObject;
				ActionHelpers.GetGameObjectFsm(AreaTitleObject.Value, "Area Title Control").SendEventSafe("NPC TITLE DOWN");
			}
			finally
			{
				Finish();
			}
		}
	}
}
