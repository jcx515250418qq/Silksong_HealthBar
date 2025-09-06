using System.Collections;
using System.Collections.Generic;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class MemoryMsgBox : MonoBehaviour
{
	private struct Message
	{
		public Object Source;

		public string Text;
	}

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private TMP_Text[] textDisplays;

	[SerializeField]
	private float fadeUpTime;

	[SerializeField]
	private AnimationCurve fadeUpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float fadeDownTime;

	[SerializeField]
	private AnimationCurve fadeDownCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	[SerializeField]
	private GameObject regularFolder;

	[SerializeField]
	private GameObject whiteBackFolder;

	private readonly List<Message> messageStack = new List<Message>();

	private Coroutine appearRoutine;

	private bool queuedRefresh;

	private Coroutine delayedDisappearRoutine;

	private int currentBackboardSpriteIndex;

	private bool hasAppeared;

	private GameManager gm;

	private static MemoryMsgBox _instance;

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = 0f;
		}
		gm = GameManager.instance;
		gm.NextSceneWillActivate += ClearAllText;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		if ((bool)gm)
		{
			gm.NextSceneWillActivate -= ClearAllText;
			gm = null;
		}
	}

	public static void AddText(Object source, string text)
	{
		if ((bool)_instance)
		{
			_instance.InternalAddText(source, text);
		}
	}

	public static void RemoveText(Object source, float disappearDelay)
	{
		if ((bool)_instance)
		{
			_instance.InternalRemoveText(source, disappearDelay);
		}
	}

	private void InternalAddText(Object source, string text)
	{
		for (int num = messageStack.Count - 1; num >= 0; num--)
		{
			if (messageStack[num].Source == source)
			{
				messageStack.RemoveAt(num);
			}
		}
		messageStack.Add(new Message
		{
			Source = source,
			Text = text
		});
		if (delayedDisappearRoutine != null)
		{
			StopCoroutine(delayedDisappearRoutine);
			delayedDisappearRoutine = null;
		}
		if (appearRoutine == null)
		{
			appearRoutine = StartCoroutine(Appear());
		}
		else
		{
			queuedRefresh = true;
		}
	}

	private void InternalRemoveText(Object source, float disappearDelay)
	{
		for (int num = messageStack.Count - 1; num >= 0; num--)
		{
			if (messageStack[num].Source == source)
			{
				messageStack.RemoveAt(num);
			}
		}
		if (delayedDisappearRoutine != null)
		{
			StopCoroutine(delayedDisappearRoutine);
			delayedDisappearRoutine = null;
		}
		if (appearRoutine != null)
		{
			if (disappearDelay > 0f)
			{
				delayedDisappearRoutine = StartCoroutine(DelayRefresh(disappearDelay));
			}
			else
			{
				queuedRefresh = true;
			}
		}
	}

	private void ClearAllText()
	{
		messageStack.Clear();
		if (delayedDisappearRoutine != null)
		{
			StopCoroutine(delayedDisappearRoutine);
			delayedDisappearRoutine = null;
		}
		if (appearRoutine != null)
		{
			StopCoroutine(appearRoutine);
			appearRoutine = null;
		}
		fadeGroup.AlphaSelf = 0f;
	}

	private IEnumerator DelayRefresh(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (appearRoutine != null)
		{
			queuedRefresh = true;
		}
		delayedDisappearRoutine = null;
	}

	private IEnumerator Appear()
	{
		Color colour = ScreenFaderUtils.GetColour();
		bool flag = colour.a < 0.5f || colour.r < 0.5f;
		regularFolder.SetActive(flag);
		whiteBackFolder.SetActive(!flag);
		while (messageStack.Count > 0)
		{
			Message msg = messageStack[messageStack.Count - 1];
			TMP_Text[] array = textDisplays;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = msg.Text;
			}
			float elapsed;
			for (elapsed = 0f; elapsed < fadeUpTime; elapsed += Time.deltaTime)
			{
				if (queuedRefresh)
				{
					break;
				}
				fadeGroup.AlphaSelf = fadeUpCurve.Evaluate(elapsed / fadeUpTime);
				yield return null;
			}
			fadeGroup.AlphaSelf = 1f;
			while (true)
			{
				if (!queuedRefresh)
				{
					yield return null;
					continue;
				}
				queuedRefresh = false;
				if (messageStack.Count <= 0 || !(messageStack[messageStack.Count - 1].Text == msg.Text))
				{
					break;
				}
			}
			elapsed = 0f;
			float startAlpha = fadeGroup.AlphaSelf;
			for (; elapsed < fadeDownTime; elapsed += Time.deltaTime)
			{
				fadeGroup.AlphaSelf = Mathf.Lerp(0f, startAlpha, fadeDownCurve.Evaluate(elapsed / fadeDownTime));
				yield return null;
			}
			queuedRefresh = false;
		}
		fadeGroup.AlphaSelf = 0f;
		appearRoutine = null;
	}
}
