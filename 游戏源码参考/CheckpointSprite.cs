using UnityEngine;
using UnityEngine.UI;

public class CheckpointSprite : MonoBehaviour
{
	private enum States
	{
		NotStarted = 0,
		Starting = 1,
		Looping = 2,
		Ending = 3
	}

	private Image image;

	private AudioSource audioSource;

	[SerializeField]
	private Sprite[] startSprites;

	[SerializeField]
	private Sprite[] loopSprites;

	[SerializeField]
	private Sprite[] endSprites;

	[SerializeField]
	private float framesPerSecond;

	private bool isShowing;

	private States state;

	private float frameTimer;

	protected void Awake()
	{
		image = GetComponent<Image>();
		audioSource = GetComponent<AudioSource>();
	}

	protected void OnEnable()
	{
		state = States.NotStarted;
		image.enabled = false;
		Update(0f);
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			instance.NextSceneWillActivate += StopAudio;
		}
	}

	private void OnDisable()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			unsafeInstance.NextSceneWillActivate -= StopAudio;
		}
	}

	private void StopAudio()
	{
		if ((bool)audioSource)
		{
			audioSource.Stop();
		}
	}

	[ContextMenu("Show")]
	public void Show()
	{
		isShowing = true;
		if (base.isActiveAndEnabled)
		{
			Update(0f);
		}
	}

	[ContextMenu("Hide")]
	public void Hide()
	{
		isShowing = false;
	}

	protected void Update()
	{
		Update(Mathf.Min(1f / 60f, Time.unscaledDeltaTime));
	}

	private void Update(float deltaTime)
	{
		frameTimer += deltaTime * framesPerSecond;
		if (state == States.NotStarted && isShowing)
		{
			frameTimer = 0f;
			state = States.Starting;
			audioSource.Play();
			image.enabled = true;
		}
		if (state == States.Starting)
		{
			int num = (int)frameTimer;
			if (num < startSprites.Length)
			{
				SetImageSprite(startSprites[num]);
			}
			else
			{
				frameTimer -= startSprites.Length;
				if (isShowing)
				{
					state = States.Looping;
				}
				else
				{
					state = States.Ending;
				}
			}
		}
		if (state == States.Looping)
		{
			int num2 = (int)frameTimer;
			if (num2 >= loopSprites.Length)
			{
				frameTimer -= loopSprites.Length * (num2 / loopSprites.Length);
				if (!isShowing)
				{
					state = States.Ending;
				}
				else
				{
					SetImageSprite(loopSprites[num2 % loopSprites.Length]);
				}
			}
			else
			{
				SetImageSprite(loopSprites[num2]);
			}
		}
		if (state == States.Ending)
		{
			int num3 = (int)frameTimer;
			if (num3 < endSprites.Length)
			{
				SetImageSprite(endSprites[num3]);
				return;
			}
			image.enabled = false;
			state = States.NotStarted;
		}
	}

	private void SetImageSprite(Sprite sprite)
	{
		if (image.sprite != sprite)
		{
			image.sprite = sprite;
		}
	}
}
