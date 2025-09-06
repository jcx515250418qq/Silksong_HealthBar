using UnityEngine;

public class Unmasker : MaskerBase
{
	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private AudioSource audioSource;

	private bool isUncovered;

	[Space]
	[SerializeField]
	private float fadeTime = 0.5f;

	[SerializeField]
	private bool playSound = true;

	private void Reset()
	{
		persistent = GetComponent<PersistentBoolItem>();
		audioSource = GetComponent<AudioSource>();
	}

	private new void Awake()
	{
		if (!persistent)
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out bool value)
		{
			value = isUncovered;
		};
		persistent.OnSetSaveState += delegate(bool value)
		{
			isUncovered = value;
			if (isUncovered)
			{
				base.AlphaSelf = 0f;
			}
		};
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		base.AlphaSelf = 1f;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.tag != "Player"))
		{
			Uncover();
		}
	}

	public void Uncover()
	{
		if (!isUncovered)
		{
			isUncovered = true;
			if (playSound)
			{
				EventRegister.SendEvent("SECRET TONE");
			}
			FadeTo(0f, fadeTime);
		}
	}
}
