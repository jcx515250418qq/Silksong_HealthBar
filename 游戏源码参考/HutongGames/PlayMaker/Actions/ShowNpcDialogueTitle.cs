namespace HutongGames.PlayMaker.Actions
{
	public class ShowNpcDialogueTitle : FSMUtility.GetComponentFsmStateAction<NpcDialogueTitle>
	{
		protected override void DoAction(NpcDialogueTitle component)
		{
			component.EnableAndShow();
		}
	}
}
