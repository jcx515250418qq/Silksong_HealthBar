namespace HutongGames.PlayMaker.Actions
{
	public class ListenForMenuSubmit : ListenForMenuButton
	{
		protected override Platform.MenuActions MenuAction => Platform.MenuActions.Submit;
	}
}
