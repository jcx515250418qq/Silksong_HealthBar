using System;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UISelectionListItem : MonoBehaviour
{
	private Func<string> _inactiveConditionText;

	[SerializeField]
	private tk2dSpriteAnimator[] selectionIndicators;

	[SerializeField]
	private string selectionIndicatorUpAnim = "Pointer Up";

	[SerializeField]
	private string selectionIndicatorDownAnim = "Pointer Down";

	[SerializeField]
	[Space]
	private NestedFadeGroupBase selectionFade;

	private float? selectionFadeInitialAlpha;

	[SerializeField]
	private float selectionFadeTime = 0.2f;

	[SerializeField]
	private GameObject submitEffect;

	[SerializeField]
	private AudioSource audioPlayerPrefab;

	[SerializeField]
	private AudioEvent selectSound;

	[SerializeField]
	private AudioEvent submitSound;

	[SerializeField]
	private AudioEvent cancelSound;

	[SerializeField]
	private AudioEvent failSound;

	[Space]
	[SerializeField]
	private GameObject activeAppearance;

	[SerializeField]
	private GameObject inactiveAppearance;

	[SerializeField]
	private TMP_Text inactiveMessage;

	[Space]
	[FormerlySerializedAs("submitPressed")]
	public UnityEvent SubmitPressed;

	[FormerlySerializedAs("cancelPressed")]
	public UnityEvent CancelPressed;

	public UnityEvent Selected;

	public UnityEvent Deselected;

	public UnityEvent Failed;

	private bool? previouslySelected;

	private bool skipNextSelectSound;

	public Func<bool> AutoSelect { get; set; }

	public Func<string> InactiveConditionText
	{
		get
		{
			return _inactiveConditionText;
		}
		set
		{
			_inactiveConditionText = value;
			UpdateAppearance();
		}
	}

	private void OnEnable()
	{
		if ((bool)submitEffect)
		{
			submitEffect.SetActive(value: false);
		}
		UpdateAppearance();
	}

	private void OnDisable()
	{
		skipNextSelectSound = false;
	}

	public void SetSelected(bool value, bool isInstant)
	{
		if (previouslySelected == value)
		{
			skipNextSelectSound = false;
			return;
		}
		previouslySelected = value;
		string text;
		UnityEvent unityEvent;
		if (value)
		{
			text = selectionIndicatorUpAnim;
			unityEvent = Selected;
			if (!isInstant && !skipNextSelectSound)
			{
				selectSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			}
			skipNextSelectSound = false;
		}
		else
		{
			text = selectionIndicatorDownAnim;
			unityEvent = Deselected;
		}
		tk2dSpriteAnimator[] array = selectionIndicators;
		foreach (tk2dSpriteAnimator tk2dSpriteAnimator2 in array)
		{
			tk2dSpriteAnimationClip clipByName = tk2dSpriteAnimator2.GetClipByName(text);
			if (clipByName != null)
			{
				if (isInstant)
				{
					tk2dSpriteAnimator2.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
				}
				else
				{
					tk2dSpriteAnimator2.Play(clipByName);
				}
			}
		}
		if ((bool)selectionFade)
		{
			if (!selectionFadeInitialAlpha.HasValue)
			{
				selectionFadeInitialAlpha = selectionFade.AlphaSelf;
			}
			selectionFade.FadeTo(value ? selectionFadeInitialAlpha.Value : 0f, isInstant ? 0f : selectionFadeTime);
		}
		unityEvent?.Invoke();
	}

	public void Submit()
	{
		if (InactiveConditionText != null && !string.IsNullOrEmpty(InactiveConditionText()))
		{
			failSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			Failed.Invoke();
			return;
		}
		SubmitPressed.Invoke();
		if ((bool)submitEffect)
		{
			submitEffect.SetActive(value: true);
		}
		submitSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		skipNextSelectSound = false;
	}

	public void Cancel()
	{
		CancelPressed.Invoke();
		cancelSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		skipNextSelectSound = false;
	}

	public void SubmitEffect()
	{
		if ((bool)submitEffect)
		{
			submitEffect.SetActive(value: true);
		}
	}

	private void UpdateAppearance()
	{
		string text = ((InactiveConditionText != null) ? InactiveConditionText() : null);
		bool flag = string.IsNullOrEmpty(text);
		if ((bool)activeAppearance)
		{
			activeAppearance.SetActive(flag);
		}
		if ((bool)inactiveAppearance)
		{
			inactiveAppearance.SetActive(!flag);
		}
		if ((bool)inactiveMessage && !string.IsNullOrEmpty(text))
		{
			inactiveMessage.text = text;
		}
	}

	public void SkipNextSelectSound()
	{
		skipNextSelectSound = true;
	}
}
