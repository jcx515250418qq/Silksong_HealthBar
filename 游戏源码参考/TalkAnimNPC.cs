using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public abstract class TalkAnimNPC : MonoBehaviour
{
	[Serializable]
	private struct TalkAnims
	{
		public string TalkEnterAnim;

		public string TalkAnim;

		public string TalkToListenAnim;

		public string ListenAnim;

		public string ListenToTalkAnim;

		public string TalkExitAnim;
	}

	[SerializeField]
	private NPCControlBase control;

	[SerializeField]
	private string talkingPageEvent;

	[SerializeField]
	private string idleAnim;

	[SerializeField]
	private AudioSource idleAudioLoop;

	[SerializeField]
	private AudioEventRandom talkEnterAudio;

	[SerializeField]
	private RandomAudioClipTable talkEnterAudioTable;

	[SerializeField]
	private AudioEventRandom talkExitAudio;

	[SerializeField]
	private RandomAudioClipTable talkExitAudioTable;

	[SerializeField]
	private TalkAnims talkLeftAnims;

	[SerializeField]
	private TalkAnims talkRightAnims;

	[SerializeField]
	private MinMaxFloat talkExitDelay;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkEnterAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkToListenAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string listenAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string listenToTalkAnim;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkExitAnim;

	private bool hasStarted;

	private bool isInConversation;

	private bool isTalking;

	private bool shouldStop;

	private bool forceTalkEnter;

	private Coroutine animationRoutine;

	public string IdleAnim
	{
		get
		{
			return idleAnim;
		}
		set
		{
			idleAnim = value;
		}
	}

	public bool IsInConversation
	{
		get
		{
			return isInConversation;
		}
		set
		{
			isInConversation = value;
		}
	}

	public event Action Stopped;

	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(talkEnterAnim))
		{
			talkLeftAnims.TalkEnterAnim = talkEnterAnim;
			talkRightAnims.TalkEnterAnim = talkEnterAnim;
			talkEnterAnim = null;
		}
		if (!string.IsNullOrEmpty(talkAnim))
		{
			talkLeftAnims.TalkAnim = talkAnim;
			talkRightAnims.TalkAnim = talkAnim;
			talkAnim = null;
		}
		if (!string.IsNullOrEmpty(talkToListenAnim))
		{
			talkLeftAnims.TalkToListenAnim = talkToListenAnim;
			talkRightAnims.TalkToListenAnim = talkToListenAnim;
			talkToListenAnim = null;
		}
		if (!string.IsNullOrEmpty(listenAnim))
		{
			talkLeftAnims.ListenAnim = listenAnim;
			talkRightAnims.ListenAnim = listenAnim;
			listenAnim = null;
		}
		if (!string.IsNullOrEmpty(listenToTalkAnim))
		{
			talkLeftAnims.ListenToTalkAnim = listenToTalkAnim;
			talkRightAnims.ListenToTalkAnim = listenToTalkAnim;
			listenToTalkAnim = null;
		}
		if (!string.IsNullOrEmpty(talkExitAnim))
		{
			talkLeftAnims.TalkExitAnim = talkExitAnim;
			talkRightAnims.TalkExitAnim = talkExitAnim;
			talkExitAnim = null;
		}
	}

	protected virtual void Awake()
	{
		OnValidate();
		if (string.IsNullOrEmpty(talkLeftAnims.TalkAnim))
		{
			talkLeftAnims.TalkAnim = talkLeftAnims.ListenAnim;
		}
		if (string.IsNullOrEmpty(talkRightAnims.TalkAnim))
		{
			talkRightAnims.TalkAnim = talkRightAnims.ListenAnim;
		}
	}

	private void OnEnable()
	{
		if (hasStarted && !StopIfRequested())
		{
			StartAnimation();
		}
	}

	private void OnDisable()
	{
		if (animationRoutine != null)
		{
			StopCoroutine(animationRoutine);
			animationRoutine = null;
		}
	}

	private void Start()
	{
		hasStarted = true;
		OnEnable();
		if ((bool)control)
		{
			control.StartedDialogue += delegate
			{
				isInConversation = true;
			};
			control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
			{
				isTalking = line.IsNpcEvent(talkingPageEvent);
			};
			control.EndingDialogue += delegate
			{
				isTalking = false;
			};
			control.EndedDialogue += delegate
			{
				isInConversation = false;
			};
		}
	}

	public void SetForceTalkEnter()
	{
		forceTalkEnter = true;
	}

	public void StartAnimation()
	{
		if (animationRoutine != null)
		{
			StopAnimation(instant: true);
		}
		animationRoutine = StartCoroutine(Animation());
	}

	public void StopAnimation(bool instant)
	{
		shouldStop = true;
		if (instant)
		{
			if (animationRoutine != null)
			{
				StopCoroutine(animationRoutine);
				animationRoutine = null;
			}
			this.Stopped?.Invoke();
		}
		else if (animationRoutine == null)
		{
			this.Stopped?.Invoke();
		}
	}

	private IEnumerator Animation()
	{
		shouldStop = false;
		bool wasInConversation = false;
		while (!StopIfRequested())
		{
			if (!wasInConversation)
			{
				PlayIdle();
				do
				{
					if (StopIfRequested())
					{
						yield break;
					}
					yield return null;
				}
				while (!isInConversation);
			}
			StopIdle();
			Vector3 pos = base.transform.position;
			bool flag = HeroController.instance.transform.position.x > pos.x;
			if (base.transform.localScale.x < 0f)
			{
				flag = !flag;
			}
			TalkAnims talkAnims = (flag ? talkRightAnims : talkLeftAnims);
			if (!wasInConversation || forceTalkEnter || string.IsNullOrEmpty(talkAnims.TalkAnim))
			{
				talkEnterAudio.SpawnAndPlayOneShot(pos);
				talkEnterAudioTable.SpawnAndPlayOneShot(pos);
				if (!string.IsNullOrEmpty(talkAnims.TalkEnterAnim))
				{
					yield return StartCoroutine(PlayAnimWait(talkAnims.TalkEnterAnim));
				}
			}
			wasInConversation = true;
			bool wasTalking = false;
			bool firstLoop = true;
			bool hasTalkAnim = !string.IsNullOrEmpty(talkAnims.TalkAnim);
			bool hasListenAnim = !string.IsNullOrEmpty(talkAnims.ListenAnim);
			while (isInConversation && !StopIfRequested())
			{
				if (!firstLoop)
				{
					if (isTalking && !wasTalking)
					{
						if (!string.IsNullOrEmpty(talkAnims.ListenToTalkAnim))
						{
							yield return StartCoroutine(PlayAnimWait(talkAnims.ListenToTalkAnim));
						}
					}
					else if (!isTalking && wasTalking && !string.IsNullOrEmpty(talkAnims.TalkToListenAnim))
					{
						yield return StartCoroutine(PlayAnimWait(talkAnims.TalkToListenAnim));
					}
				}
				string text = (isTalking ? talkAnims.TalkAnim : talkAnims.ListenAnim);
				bool flag2 = !string.IsNullOrEmpty(text);
				if (!flag2)
				{
					if (!isTalking)
					{
						if (hasTalkAnim)
						{
							text = talkAnims.TalkAnim;
							flag2 = true;
						}
					}
					else if (hasListenAnim)
					{
						text = talkAnims.ListenAnim;
						flag2 = true;
					}
				}
				if (flag2)
				{
					PlayAnim(text);
				}
				wasTalking = isTalking;
				firstLoop = false;
				yield return null;
			}
			float delay = talkExitDelay.GetRandomValue();
			if (delay > 0f)
			{
				if (wasTalking && !string.IsNullOrEmpty(talkAnims.ListenAnim))
				{
					PlayAnim(talkAnims.ListenAnim);
				}
				for (float elapsed = 0f; elapsed < delay; elapsed += Time.deltaTime)
				{
					if (shouldStop)
					{
						break;
					}
					yield return null;
				}
			}
			if (!isInConversation)
			{
				wasInConversation = false;
				talkExitAudio.SpawnAndPlayOneShot(pos);
				talkExitAudioTable.SpawnAndPlayOneShot(pos);
				if (!string.IsNullOrEmpty(talkAnims.TalkExitAnim))
				{
					yield return StartCoroutine(PlayAnimWait(talkAnims.TalkExitAnim));
				}
				PlayIdle();
			}
		}
		animationRoutine = null;
	}

	private void PlayIdle()
	{
		if (!string.IsNullOrEmpty(idleAnim))
		{
			PlayAnim(idleAnim);
		}
		if ((bool)idleAudioLoop)
		{
			idleAudioLoop.Play();
		}
	}

	private void StopIdle()
	{
		if ((bool)idleAudioLoop)
		{
			idleAudioLoop.Stop();
		}
	}

	private bool StopIfRequested()
	{
		if (!shouldStop)
		{
			return false;
		}
		if (this.Stopped != null)
		{
			this.Stopped();
		}
		animationRoutine = null;
		return true;
	}

	protected abstract void PlayAnim(string animName);

	protected abstract IEnumerator PlayAnimWait(string animName);
}
