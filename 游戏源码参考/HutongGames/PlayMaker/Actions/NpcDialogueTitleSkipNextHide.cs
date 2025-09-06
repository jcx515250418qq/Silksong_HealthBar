namespace HutongGames.PlayMaker.Actions
{
	public class NpcDialogueTitleSkipNextHide : FSMUtility.GetComponentFsmStateAction<NpcDialogueTitle>
	{
		protected override void DoAction(NpcDialogueTitle component)
		{
			component.SkipNextHide();
		}
	}
}
