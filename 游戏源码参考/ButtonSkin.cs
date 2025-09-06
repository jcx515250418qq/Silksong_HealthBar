using GlobalEnums;
using UnityEngine;

public class ButtonSkin
{
	public Sprite sprite;

	public string symbol;

	public ButtonSkinType skinType;

	public ButtonSkin(Sprite startSprite, string startSymbol, ButtonSkinType startSkinType)
	{
		sprite = startSprite;
		symbol = startSymbol;
		skinType = startSkinType;
	}
}
