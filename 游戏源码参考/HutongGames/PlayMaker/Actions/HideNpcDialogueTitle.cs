namespace HutongGames.PlayMaker.Actions
{
	public class HideNpcDialogueTitle : FSMUtility.GetComponentFsmStateAction<NpcDialogueTitle>
	{
		protected override void DoAction(NpcDialogueTitle component)
		{
			component.Hide();
		}
	}
}
