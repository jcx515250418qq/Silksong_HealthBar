using UnityEngine;
using UnityEngine.UI;

public class EngagedUserPanel : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text displayNameText;

	[SerializeField]
	private RawImage displayImageImage;

	protected void Awake()
	{
		Platform.Current.EngagedDisplayInfoChanged += UpdateContent;
	}

	protected void OnDestroy()
	{
		Platform.Current.EngagedDisplayInfoChanged -= UpdateContent;
	}

	protected void Start()
	{
		UpdateContent();
		base.enabled = false;
	}

	private void UpdateContent()
	{
		if (Platform.Current.EngagementRequirement == Platform.EngagementRequirements.Invisible)
		{
			canvasGroup.alpha = 0f;
			displayNameText.enabled = false;
			displayImageImage.enabled = false;
			return;
		}
		canvasGroup.alpha = 1f;
		displayNameText.enabled = true;
		displayNameText.text = Platform.Current.EngagedDisplayName ?? "";
		Texture2D engagedDisplayImage = Platform.Current.EngagedDisplayImage;
		displayImageImage.enabled = engagedDisplayImage != null;
		displayImageImage.texture = engagedDisplayImage;
	}
}
