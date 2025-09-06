using System;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
	public delegate void SelfEvent(AchievementPopup sender);

	private enum FadeState
	{
		None = 0,
		WaitAppear = 1,
		FadeUp = 2,
		WaitHold = 3,
		FadeDown = 4,
		Finish = 5
	}

	public Image image;

	public Text nameText;

	public Text descriptionText;

	private CanvasGroup group;

	[Space]
	public float appearDelay;

	public float fadeInTime = 0.25f;

	public float holdTime = 3f;

	public float fadeOutTime = 0.5f;

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent sound;

	[Space]
	public Animator fluerAnimator;

	public string fluerCloseName = "Close";

	private FadeState currentState;

	private FadeState previousState;

	private float elapsed;

	public event SelfEvent OnFinish;

	private void Awake()
	{
		group = GetComponent<CanvasGroup>();
	}

	public void Setup(Sprite icon, string name, string description)
	{
		if ((bool)image)
		{
			image.sprite = icon;
		}
		if ((bool)nameText)
		{
			nameText.text = name;
		}
		if ((bool)descriptionText)
		{
			descriptionText.text = description;
		}
		sound.SpawnAndPlayOneShot(audioPlayerPrefab, Vector3.zero);
		currentState = FadeState.WaitAppear;
	}

	private void Update()
	{
		switch (currentState)
		{
		case FadeState.WaitAppear:
			if (currentState != previousState)
			{
				elapsed = 0f;
				previousState = currentState;
				group.alpha = 0f;
			}
			elapsed += Time.unscaledDeltaTime;
			if (elapsed >= appearDelay)
			{
				currentState = FadeState.FadeUp;
			}
			break;
		case FadeState.FadeUp:
			if (currentState != previousState)
			{
				elapsed = 0f;
				previousState = currentState;
			}
			group.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
			elapsed += Time.unscaledDeltaTime;
			if (elapsed >= fadeInTime)
			{
				group.alpha = 1f;
				currentState = FadeState.WaitHold;
			}
			break;
		case FadeState.WaitHold:
			if (currentState != previousState)
			{
				elapsed = 0f;
				previousState = currentState;
			}
			elapsed += Time.unscaledDeltaTime;
			if (elapsed >= holdTime)
			{
				currentState = FadeState.FadeDown;
			}
			break;
		case FadeState.FadeDown:
			if (currentState != previousState)
			{
				elapsed = 0f;
				previousState = currentState;
				if ((bool)fluerAnimator)
				{
					fluerAnimator.Play(fluerCloseName);
				}
			}
			group.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
			elapsed += Time.unscaledDeltaTime;
			if (elapsed >= fadeOutTime)
			{
				group.alpha = 0f;
				currentState = FadeState.Finish;
			}
			break;
		case FadeState.Finish:
			if (currentState != previousState)
			{
				previousState = currentState;
				if (this.OnFinish != null)
				{
					this.OnFinish(this);
				}
				else
				{
					base.gameObject.SetActive(value: false);
				}
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case FadeState.None:
			break;
		}
	}
}
