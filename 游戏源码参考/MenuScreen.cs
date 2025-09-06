using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
	public enum HighlightDefaultBehaviours
	{
		AfterFade = 0,
		BeforeFade = 1
	}

	public Animator topFleur;

	public Animator bottomFleur;

	public MenuButton backButton;

	public Selectable defaultHighlight;

	[SerializeField]
	private HighlightDefaultBehaviours highlightBehaviour;

	private static readonly int _hidePropId = Animator.StringToHash("hide");

	private static readonly int _showPropId = Animator.StringToHash("show");

	public CanvasGroup ScreenCanvasGroup => GetComponent<CanvasGroup>();

	public HighlightDefaultBehaviours HighlightBehaviour => highlightBehaviour;

	public void HighlightDefault()
	{
		EventSystem current = EventSystem.current;
		if (defaultHighlight == null || current.currentSelectedGameObject != null)
		{
			return;
		}
		Selectable firstInteractable = defaultHighlight.GetFirstInteractable();
		if (!firstInteractable)
		{
			return;
		}
		UIManager.HighlightSelectableNoSound(firstInteractable);
		foreach (Transform item in defaultHighlight.transform)
		{
			Animator component = item.GetComponent<Animator>();
			if (!(component == null))
			{
				component.ResetTrigger(_hidePropId);
				component.SetTrigger(_showPropId);
				break;
			}
		}
	}

	public bool GoBack()
	{
		if (!backButton)
		{
			return false;
		}
		backButton.SkipNextFlashEffect = true;
		backButton.SkipNextSubmitSound = true;
		return ExecuteEvents.Execute(backButton.gameObject, null, ExecuteEvents.submitHandler);
	}
}
