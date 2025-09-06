using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
	public struct DialogueLine
	{
		public bool IsPlayer;

		public string Text;

		public string Event;

		public bool IsNpcEvent(string eventName)
		{
			if (IsPlayer)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(eventName))
			{
				return eventName == Event;
			}
			return true;
		}
	}

	[Serializable]
	public struct DisplayOptions
	{
		public bool ShowDecorators;

		public TextAlignmentOptions Alignment;

		public float OffsetY;

		public float StopOffsetY;

		public Color TextColor;

		public static DisplayOptions Default
		{
			get
			{
				DisplayOptions result = default(DisplayOptions);
				result.ShowDecorators = true;
				result.Alignment = TextAlignmentOptions.TopLeft;
				result.OffsetY = 0f;
				result.StopOffsetY = 0f;
				result.TextColor = Color.white;
				return result;
			}
		}
	}

	private static DialogueBox _instance;

	private static Action _onCancelledCallback;

	private static int _conversationID;

	[SerializeField]
	private TextMeshPro textMesh;

	[SerializeField]
	private float regularRevealSpeed = 65f;

	[SerializeField]
	private float fastRevealSpeed = 200f;

	private float currentRevealSpeed;

	[SerializeField]
	private GameObject defaultAppearance;

	[SerializeField]
	private GameObject playerAppearance;

	[SerializeField]
	private Animator stopAnimator;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer stopSprite;

	[SerializeField]
	private float firstOpenDelay = 0.1f;

	[SerializeField]
	private float lineEndPause = 0.1f;

	[SerializeField]
	private NestedFadeGroupBase group;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private PlayMakerFSM hudFSM;

	[SerializeField]
	private GameObject[] decorators;

	private bool isDialogueRunning;

	private bool waitingToAdvance;

	private bool isPrintingText;

	private bool isBoxOpen;

	private DialogueLine currentLine;

	private Vector3 initialPos;

	private Vector3 initialStopPos;

	private NPCControlBase instigator;

	private bool conversationEnded;

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
	}

	private void Start()
	{
		initialPos = base.transform.localPosition;
		initialStopPos = stopAnimator.transform.localPosition;
		currentRevealSpeed = regularRevealSpeed;
		group.AlphaSelf = 0f;
		animator.Play("Close", 0, 1f);
		EventRegister.GetRegisterGuaranteed(base.gameObject, "LEAVING SCENE").ReceivedEvent += OnLeavingScene;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void OnLeavingScene()
	{
		RunCancelledCallback();
		EndConversation(_instance.isDialogueRunning);
	}

	public static void StartConversation(LocalisedString text, NPCControlBase instigator, bool overrideContinue = false)
	{
		StartConversation(text, instigator, overrideContinue, DisplayOptions.Default);
	}

	public static void StartConversation(LocalisedString text, NPCControlBase instigator, bool overrideContinue, DisplayOptions displayOptions, Action onDialogueEnd = null)
	{
		StartConversation(CheatManager.IsDialogueDebugEnabled ? (text.Sheet + " / " + text.Key) : text.ToString(allowBlankText: false), instigator, overrideContinue, displayOptions, onDialogueEnd);
	}

	public static void StartConversation(string text, NPCControlBase instigator, bool overrideContinue, DisplayOptions displayOptions, Action onDialogueEnd = null, Action onDialogueCancelled = null)
	{
		if ((bool)_instance)
		{
			_instance.ShowShared(text, displayOptions, out var lines);
			_instance.instigator = instigator;
			_onCancelledCallback = onDialogueCancelled;
			_instance.StartCoroutine(_instance.RunDialogue(lines, overrideContinue, onDialogueEnd, ++_conversationID));
			_instance.isDialogueRunning = true;
		}
	}

	public static void ShowInstant(string text, DisplayOptions displayOptions, int lineIndex, int pageIndex, out int lineCount, out int pageCount)
	{
		if (!_instance)
		{
			lineCount = 0;
			pageCount = 0;
			return;
		}
		_instance.ShowShared(text, displayOptions, out var lines);
		_instance.instigator = null;
		lineCount = lines.Count;
		DialogueLine dialogueLine = lines[lineIndex];
		_instance.UpdateAppearance(lines[lineIndex]);
		_instance.hudFSM.SendEventSafe("OUT INSTANT");
		_instance.animator.Play("Open", 0, 1f);
		_instance.animator.Update(0f);
		_instance.isBoxOpen = true;
		_instance.textMesh.text = dialogueLine.Text;
		_instance.textMesh.maxVisibleCharacters = int.MaxValue;
		TMP_TextInfo textInfo = _instance.textMesh.GetTextInfo(dialogueLine.Text);
		pageCount = textInfo.pageCount;
		_instance.textMesh.pageToDisplay = pageIndex + 1;
	}

	public static void HideInstant()
	{
		_instance.isBoxOpen = false;
		_instance.animator.Play("Close", 0, 1f);
		_instance.animator.Update(0f);
	}

	private void ShowShared(string text, DisplayOptions displayOptions, out List<DialogueLine> lines)
	{
		base.transform.localPosition = initialPos + new Vector3(0f, displayOptions.OffsetY);
		if ((bool)textMesh)
		{
			textMesh.alignment = displayOptions.Alignment;
			textMesh.color = displayOptions.TextColor;
		}
		if ((bool)stopSprite)
		{
			stopSprite.BaseColor = displayOptions.TextColor;
		}
		GameObject[] array = decorators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(displayOptions.ShowDecorators);
		}
		lines = ParseTextForDialogueLines(text);
		if (lines.Count == 0)
		{
			lines.Add(new DialogueLine
			{
				IsPlayer = false,
				Text = "ERROR: Empty dialogue line!"
			});
		}
		RunCancelledCallback();
		StopAllCoroutines();
		conversationEnded = false;
	}

	private static void RunCancelledCallback()
	{
		if (_onCancelledCallback != null)
		{
			Action onCancelledCallback = _onCancelledCallback;
			_onCancelledCallback = null;
			onCancelledCallback();
		}
	}

	private void Update()
	{
		if (isDialogueRunning && (isPrintingText || waitingToAdvance) && ManagerSingleton<InputHandler>.Instance.WasSkipButtonPressed)
		{
			AdvanceConversation();
		}
	}

	private void AdvanceConversation()
	{
		currentRevealSpeed = (waitingToAdvance ? regularRevealSpeed : fastRevealSpeed);
		waitingToAdvance = false;
	}

	public static void EndConversation(bool returnHud = true, Action onBoxHidden = null)
	{
		if (!_instance)
		{
			return;
		}
		if (!_instance.isDialogueRunning)
		{
			if (returnHud)
			{
				_instance.hudFSM.SendEventSafe("IN");
			}
			onBoxHidden?.Invoke();
		}
		else
		{
			_instance.conversationEnded = true;
			_instance.StartCoroutine(_instance.CloseAndEnd(returnHud, onBoxHidden));
		}
	}

	private IEnumerator CloseAndEnd(bool returnHud, Action onBoxHidden)
	{
		yield return StartCoroutine(Close());
		NPCControlBase nPCControlBase = instigator;
		instigator = null;
		isDialogueRunning = false;
		if (returnHud)
		{
			hudFSM.SendEventSafe("IN");
		}
		if ((bool)nPCControlBase)
		{
			nPCControlBase.EndDialogue();
		}
		onBoxHidden?.Invoke();
	}

	public static bool IsSpeakerDifferent(DialogueLine currentLine, DialogueLine nextLine)
	{
		if (nextLine.IsPlayer == currentLine.IsPlayer)
		{
			return nextLine.Event != currentLine.Event;
		}
		return true;
	}

	public static bool WillShowFullStop(IReadOnlyList<DialogueLine> lines, int lineIndex, bool overrideContinue)
	{
		if (lineIndex >= lines.Count - 1)
		{
			return !overrideContinue;
		}
		return IsSpeakerDifferent(lines[lineIndex], lines[lineIndex + 1]);
	}

	private void UpdateAppearance(DialogueLine line)
	{
		currentLine = line;
		if ((bool)defaultAppearance)
		{
			defaultAppearance.SetActive(!line.IsPlayer);
		}
		if ((bool)playerAppearance)
		{
			playerAppearance.SetActive(line.IsPlayer);
		}
	}

	private IEnumerator RunDialogue(List<DialogueLine> lines, bool overrideContinue, Action onEnd, int iD)
	{
		if (!textMesh)
		{
			yield break;
		}
		NPCControlBase dialogueInstigator = instigator;
		if (!dialogueInstigator)
		{
			yield break;
		}
		EventRegister.SendEvent(EventRegisterEvents.DialogueBoxAppearing);
		if ((bool)dialogueInstigator)
		{
			dialogueInstigator.OnOpeningDialogueBox(lines[0]);
		}
		if (!isBoxOpen)
		{
			UpdateAppearance(lines[0]);
			if ((bool)stopAnimator)
			{
				stopAnimator.gameObject.SetActive(value: false);
			}
			textMesh.text = string.Empty;
			hudFSM.SendEventSafe("OUT");
			yield return new WaitForSeconds(firstOpenDelay);
			if (!conversationEnded)
			{
				animator.Play("Open");
				isBoxOpen = true;
			}
		}
		for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
		{
			if (conversationEnded)
			{
				break;
			}
			DialogueLine line = lines[lineIndex];
			textMesh.text = string.Empty;
			if (IsSpeakerDifferent(currentLine, line))
			{
				yield return StartCoroutine(Close());
				if (conversationEnded)
				{
					break;
				}
				UpdateAppearance(line);
				animator.Play("Open");
				isBoxOpen = true;
			}
			if ((bool)dialogueInstigator)
			{
				dialogueInstigator.NewLineStarted(line);
			}
			textMesh.text = line.Text;
			TMP_TextInfo textInfo = textMesh.GetTextInfo(line.Text);
			int pageCount = textInfo.pageCount;
			for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
			{
				textMesh.pageToDisplay = pageIndex + 1;
				yield return StartCoroutine(PrintText(textInfo, pageIndex));
				yield return new WaitForSeconds(lineEndPause);
				if (pageIndex < pageCount - 1)
				{
					yield return LineEndedWait(isFullStop: false);
				}
			}
			if ((bool)dialogueInstigator)
			{
				dialogueInstigator.NewLineEnded(line);
			}
			yield return StartCoroutine(LineEndedWait(WillShowFullStop(lines, lineIndex, overrideContinue)));
		}
		if ((bool)dialogueInstigator)
		{
			dialogueInstigator.OnDialogueBoxEnded();
		}
		onEnd?.Invoke();
		if (_conversationID == iD)
		{
			_onCancelledCallback = null;
		}
		if ((bool)dialogueInstigator && dialogueInstigator.AutoEnd)
		{
			EndConversation();
		}
	}

	private IEnumerator LineEndedWait(bool isFullStop)
	{
		string animPrefix = (isFullStop ? "Stop" : "Arrow");
		if ((bool)stopAnimator)
		{
			stopAnimator.gameObject.SetActive(value: true);
			stopAnimator.Play(animPrefix + " " + (currentLine.IsPlayer ? "Hornet" : "NPC") + " Up");
		}
		waitingToAdvance = true;
		while (waitingToAdvance && !conversationEnded)
		{
			yield return null;
		}
		Audio.StopConfirmSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		if ((bool)stopAnimator)
		{
			stopAnimator.Play(animPrefix + " " + (currentLine.IsPlayer ? "Hornet" : "NPC") + " Down");
		}
	}

	private IEnumerator Close()
	{
		isBoxOpen = false;
		animator.Play("Close");
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
	}

	private IEnumerator PrintText(TMP_TextInfo textInfo, int pageIndex)
	{
		TMP_PageInfo pageInfo = textInfo.pageInfo[pageIndex];
		isPrintingText = true;
		textMesh.maxVisibleCharacters = pageInfo.firstCharacterIndex;
		if (!CheatManager.IsTextPrintSkipEnabled)
		{
			float visibleCharacters = pageInfo.firstCharacterIndex;
			while (textMesh.maxVisibleCharacters < pageInfo.lastCharacterIndex)
			{
				yield return null;
				visibleCharacters += currentRevealSpeed * Time.deltaTime;
				textMesh.maxVisibleCharacters = Mathf.RoundToInt(visibleCharacters);
			}
		}
		textMesh.maxVisibleCharacters = int.MaxValue;
		isPrintingText = false;
	}

	public static List<DialogueLine> ParseTextForDialogueLines(string text)
	{
		List<DialogueLine> lines = new List<DialogueLine>();
		if (string.IsNullOrEmpty(text))
		{
			lines.Add(new DialogueLine
			{
				IsPlayer = false,
				Text = string.Empty
			});
			return lines;
		}
		StringBuilder builder = Helper.GetTempStringBuilder();
		bool wasPlayer = false;
		string eventName = null;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == '<')
			{
				int num = i + 1;
				int num2 = -1;
				while (true)
				{
					switch (text[i])
					{
					case '=':
						num2 = i;
						goto IL_0093;
					default:
						goto IL_0093;
					case '>':
						break;
					}
					break;
					IL_0093:
					i++;
					if (i >= text.Length)
					{
						Debug.LogError($"DialogueText tag opened with '{'<'}' but no closing '{'>'}' was found!");
						return lines;
					}
				}
				string text2;
				string newEventName2;
				if (num2 < 0)
				{
					text2 = text.Substring(num, i - num);
					newEventName2 = null;
				}
				else
				{
					text2 = text.Substring(num, num2 - num);
					newEventName2 = text.Substring(num2 + 1, i - num2 - 1);
				}
				if (text2 == "page")
				{
					BeginNewLine(isPlayer: false, newEventName2);
				}
				else if (text2 == "hpage")
				{
					BeginNewLine(isPlayer: true, newEventName2);
				}
			}
			else
			{
				builder.Append(text[i]);
			}
		}
		EndCurrentLine();
		return lines;
		void BeginNewLine(bool isPlayer, string newEventName)
		{
			EndCurrentLine();
			builder.Clear();
			wasPlayer = isPlayer;
			eventName = newEventName;
		}
		void EndCurrentLine()
		{
			string text3 = builder.ToString().Trim();
			if (!string.IsNullOrEmpty(text3))
			{
				lines.Add(new DialogueLine
				{
					IsPlayer = wasPlayer,
					Text = text3,
					Event = eventName
				});
			}
		}
	}
}
