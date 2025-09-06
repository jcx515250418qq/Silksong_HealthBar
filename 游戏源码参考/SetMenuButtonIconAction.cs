using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetMenuButtonIconAction : FsmStateAction
{
	public FsmGameObject target;

	[ObjectType(typeof(Platform.MenuActions))]
	public FsmEnum menuAction;

	public override void Reset()
	{
		target = null;
		menuAction = null;
	}

	public override void OnEnter()
	{
		if ((bool)target.Value)
		{
			MenuButtonIcon componentInChildren = target.Value.GetComponentInChildren<MenuButtonIcon>();
			if ((bool)componentInChildren)
			{
				componentInChildren.menuAction = (Platform.MenuActions)(object)menuAction.Value;
				componentInChildren.RefreshButtonIcon();
			}
		}
		Finish();
	}
}
