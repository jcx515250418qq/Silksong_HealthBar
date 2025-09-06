using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class LogoLanguage : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	[Space]
	public Image uiImage;

	public bool setNativeSize = true;

	[Space]
	public Sprite englishSprite;

	public Sprite chineseSprite;

	private GameManager gm;

	private void OnEnable()
	{
		gm = GameManager.SilentInstance;
		if ((bool)gm)
		{
			gm.RefreshLanguageText += SetSprite;
		}
		SetSprite();
	}

	private void OnDisable()
	{
		if ((bool)gm)
		{
			gm.RefreshLanguageText -= SetSprite;
			gm = null;
		}
	}

	public void SetSprite()
	{
		Sprite sprite = ((!(Language.CurrentLanguage().ToString() == "ZH")) ? englishSprite : chineseSprite);
		if ((bool)spriteRenderer)
		{
			spriteRenderer.sprite = sprite;
		}
		if ((bool)uiImage)
		{
			uiImage.sprite = sprite;
			if (setNativeSize)
			{
				uiImage.SetNativeSize();
			}
		}
	}
}
