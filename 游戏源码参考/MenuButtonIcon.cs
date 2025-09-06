using GlobalEnums;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MenuButtonIcon : ActionButtonIconBase
{
	public Platform.MenuActions menuAction;

	public override HeroActionButton Action
	{
		get
		{
			if (Platform.Current.WasLastInputKeyboard)
			{
				switch (menuAction)
				{
				case Platform.MenuActions.Submit:
					return HeroActionButton.JUMP;
				case Platform.MenuActions.Cancel:
					return HeroActionButton.CAST;
				case Platform.MenuActions.Extra:
					return HeroActionButton.DASH;
				case Platform.MenuActions.Super:
					return HeroActionButton.DREAM_NAIL;
				}
			}
			else
			{
				switch (menuAction)
				{
				case Platform.MenuActions.Submit:
					return HeroActionButton.MENU_SUBMIT;
				case Platform.MenuActions.Cancel:
					return HeroActionButton.MENU_CANCEL;
				case Platform.MenuActions.Extra:
					return HeroActionButton.MENU_EXTRA;
				case Platform.MenuActions.Super:
					return HeroActionButton.MENU_SUPER;
				}
			}
			return HeroActionButton.MENU_CANCEL;
		}
	}
}
