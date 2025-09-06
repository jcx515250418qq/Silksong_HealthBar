namespace HutongGames.PlayMaker.Actions
{
	public class ListenForMenuCancel : ListenForMenuButton
	{
		protected override Platform.MenuActions MenuAction => Platform.MenuActions.Cancel;
	}
}
