using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class Gate : UnlockablePropBase
{
	private enum StartStates
	{
		Default = 0,
		FreezeAtOpenStart = 1,
		StartOpen = 2
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string openAnim;

	[SerializeField]
	private string closeAnim;

	[SerializeField]
	private StartStates startState;

	[SerializeField]
	private string openedAnim;

	[SerializeField]
	private bool deactivateIfOpened;

	[SerializeField]
	private MinMaxFloat openDelay;

	[Space]
	[SerializeField]
	private UnityEvent onBeforeDelay;

	[SerializeField]
	private UnityEvent onOpen;

	[SerializeField]
	private UnityEvent onOpened;

	[SerializeField]
	private UnityEvent onClose;

	[Space]
	[SerializeField]
	private UnlockablePropBase[] openChildren;

	[SerializeField]
	private float openChildDelay;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private AnimationClip openClip;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool startAtOpenStart;

	private AnimatorCullingMode initialCullingMode;

	private AudioSource source;

	private bool activated;

	private bool isClosed;

	private void OnValidate()
	{
		if ((bool)openClip)
		{
			openAnim = openClip.name;
			openClip = null;
		}
		if (startAtOpenStart)
		{
			startState = StartStates.FreezeAtOpenStart;
			startAtOpenStart = false;
		}
	}

	private void Reset()
	{
		animator = GetComponent<Animator>();
	}

	private void Awake()
	{
		OnValidate();
		if (!animator)
		{
			animator = GetComponent<Animator>();
		}
		if ((bool)animator)
		{
			initialCullingMode = animator.cullingMode;
		}
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if ((bool)component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = activated;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				activated = value;
				if (activated)
				{
					Opened();
				}
			};
		}
		if (activated || !animator)
		{
			return;
		}
		switch (startState)
		{
		case StartStates.FreezeAtOpenStart:
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.Play(openAnim);
			animator.enabled = false;
			animator.Update(0f);
			animator.playbackTime = 0f;
			animator.cullingMode = initialCullingMode;
			break;
		case StartStates.StartOpen:
			if (!string.IsNullOrEmpty(openedAnim))
			{
				animator.Play(openedAnim);
			}
			else
			{
				animator.Play(openAnim, 0, 1f);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case StartStates.Default:
			break;
		}
	}

	public override void Open()
	{
		if ((startState != StartStates.StartOpen || isClosed) && !activated)
		{
			activated = true;
			float randomValue = openDelay.GetRandomValue();
			OpenWithDelay(randomValue);
		}
	}

	public void OpenWithDelay(float delay)
	{
		if (delay <= 0f)
		{
			onBeforeDelay.Invoke();
			DoOpen();
		}
		else
		{
			onBeforeDelay.Invoke();
			this.ExecuteDelayed(delay, DoOpen);
		}
	}

	private void DoOpen()
	{
		onOpen.Invoke();
		isClosed = false;
		if ((bool)animator)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.enabled = true;
			animator.Play(openAnim);
		}
		UnlockablePropBase[] array;
		if (openChildDelay <= 0f)
		{
			array = openChildren;
			foreach (UnlockablePropBase unlockablePropBase in array)
			{
				if ((bool)unlockablePropBase)
				{
					unlockablePropBase.Open();
				}
			}
			return;
		}
		array = openChildren;
		foreach (UnlockablePropBase unlockablePropBase2 in array)
		{
			if ((bool)unlockablePropBase2)
			{
				unlockablePropBase2.ExecuteDelayed(openChildDelay, unlockablePropBase2.Open);
			}
		}
	}

	public override void Opened()
	{
		activated = true;
		isClosed = false;
		onOpened.Invoke();
		if (deactivateIfOpened)
		{
			base.gameObject.SetActive(value: false);
		}
		else if ((bool)animator)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.enabled = true;
			if (!string.IsNullOrEmpty(openedAnim))
			{
				animator.Play(openedAnim);
			}
			else
			{
				animator.Play(openAnim, 0, 1f);
			}
			animator.Update(0f);
			animator.cullingMode = initialCullingMode;
		}
		UnlockablePropBase[] array = openChildren;
		foreach (UnlockablePropBase unlockablePropBase in array)
		{
			if ((bool)unlockablePropBase)
			{
				unlockablePropBase.Opened();
			}
		}
	}

	public void Close()
	{
		if (startState == StartStates.StartOpen)
		{
			ForceClose();
		}
		else
		{
			DoClose();
		}
	}

	public void ForceClose()
	{
		onClose.Invoke();
		isClosed = true;
		if ((bool)animator)
		{
			animator.enabled = true;
			animator.Play(closeAnim);
		}
	}

	private void DoClose()
	{
		if (activated)
		{
			ForceClose();
			activated = false;
		}
	}

	public void PlaySound()
	{
		if ((bool)source)
		{
			source.Play();
		}
	}

	public void StartRumble()
	{
		GameCameras gameCameras = UnityEngine.Object.FindObjectOfType<GameCameras>();
		if ((bool)gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("AverageShake");
		}
	}
}
