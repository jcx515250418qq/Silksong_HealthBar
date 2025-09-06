using System;
using System.Collections;
using System.Text;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ContentPackDetailsUI : MonoBehaviour
{
	[Serializable]
	public class ContentPackDetails
	{
		public Sprite posterSprite;

		public string titleText;

		[Multiline]
		public string descriptionText;

		public int menuStyleIndex;
	}

	public static ContentPackDetailsUI Instance;

	public ContentPackDetails[] details;

	[Space]
	public Image posterImage;

	public Text titleText;

	public Text descriptionText;

	public SoftMaskScript softMask;

	public ScrollRect scrollRect;

	public string languageSheet = "CP3";

	public int beforeSpaces = 1;

	public int afterSpaces = 3;

	private int oldMenuStyleIndex;

	private int packDetailsIndex;

	private void Awake()
	{
		Instance = this;
	}

	public void ShowPackDetails(int index)
	{
		packDetailsIndex = index;
		if ((bool)MenuStyles.Instance)
		{
			oldMenuStyleIndex = MenuStyles.Instance.CurrentStyle;
			MenuStyles.Instance.SetStyle(details[packDetailsIndex].menuStyleIndex, fade: true, save: false);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateDelayed());
	}

	private void OnDisable()
	{
		if ((bool)descriptionText)
		{
			descriptionText.text = "";
		}
	}

	private IEnumerator UpdateDelayed()
	{
		if (packDetailsIndex >= 0 && packDetailsIndex < details.Length)
		{
			ContentPackDetails contentPackDetails = details[packDetailsIndex];
			if ((bool)posterImage)
			{
				posterImage.sprite = contentPackDetails.posterSprite;
			}
			if ((bool)titleText)
			{
				titleText.text = Language.Get(contentPackDetails.titleText, languageSheet);
			}
			if ((bool)scrollRect)
			{
				scrollRect.verticalNormalizedPosition = 1f;
			}
			if ((bool)descriptionText)
			{
				string text = Language.Get(contentPackDetails.descriptionText, languageSheet);
				descriptionText.text = text.Replace("<br>", "\n");
				StringBuilder sb = new StringBuilder();
				sb.Append('\n', beforeSpaces);
				sb.Append(descriptionText.text);
				descriptionText.text = sb.ToString();
				while (!descriptionText.gameObject.activeInHierarchy)
				{
					yield return null;
				}
				yield return null;
				float preferredHeight = LayoutUtility.GetPreferredHeight(descriptionText.rectTransform);
				float height = scrollRect.viewport.rect.height;
				bool flag = preferredHeight > height;
				if (flag)
				{
					sb.Append('\n', afterSpaces);
				}
				if ((bool)softMask)
				{
					softMask.HardBlend = !flag;
				}
				descriptionText.text = sb.ToString();
			}
		}
		else
		{
			Debug.LogError("Content Pack Details do not exist for index " + packDetailsIndex);
		}
	}

	public void UndoMenuStyle()
	{
		if ((bool)MenuStyles.Instance)
		{
			MenuStyles.Instance.SetStyle(oldMenuStyleIndex, fade: true);
		}
	}
}
