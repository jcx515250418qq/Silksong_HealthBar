using System;
using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class RunDialogueBase : FSMUtility.GetComponentFsmStateAction<PlayMakerNPC>
	{
		public FsmBool OverrideContinue;

		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject PlayerVoiceTableOverride;

		public FsmBool PreventHeroAnimation;

		[ActionSection("Display Options")]
		public FsmBool HideDecorators;

		[ObjectType(typeof(TextAlignmentOptions))]
		public FsmEnum TextAlignment;

		public FsmFloat OffsetY;

		private PlayMakerNPC setEventTarget;

		protected abstract string DialogueText { get; }

		public override void Reset()
		{
			base.Reset();
			OverrideContinue = null;
			PlayerVoiceTableOverride = null;
			PreventHeroAnimation = null;
			HideDecorators = null;
			TextAlignment = new FsmEnum(TextAlignmentOptions.TopLeft);
			OffsetY = null;
		}

		public override void Awake()
		{
			TextAlignment.Value = TextAlignment.Value;
		}

		protected override void DoAction(PlayMakerNPC npc)
		{
			setEventTarget = npc;
			npc.CustomEventTarget = base.Fsm.FsmComponent;
			if (!npc.IsRunningDialogue)
			{
				npc.SetAutoStarting();
				npc.StartDialogueImmediately();
			}
			StartDialogue(npc);
		}

		protected override void DoActionNoComponent(GameObject target)
		{
			PlayMakerNPC newTemp = PlayMakerNPC.GetNewTemp(base.Fsm.FsmComponent);
			newTemp.StartDialogueMove();
			StartDialogue(newTemp);
		}

		public override void OnExit()
		{
			if (setEventTarget != null)
			{
				if (setEventTarget.CustomEventTarget == base.Fsm.FsmComponent)
				{
					setEventTarget.CustomEventTarget = null;
				}
				setEventTarget = null;
			}
		}

		protected virtual void StartDialogue(PlayMakerNPC component)
		{
			Action action = null;
			RandomAudioClipTable randomAudioClipTable = PlayerVoiceTableOverride.Value as RandomAudioClipTable;
			if (randomAudioClipTable != null)
			{
				component.SetTalkTableOverride(randomAudioClipTable);
				action = component.RemoveTalkTableOverride;
			}
			if (PreventHeroAnimation.Value)
			{
				HeroTalkAnimation.SetBlocked(value: true);
				action = ((action != null) ? ((Action)Delegate.Combine(action, new Action(ResetBlocked))) : new Action(ResetBlocked));
			}
			DialogueBox.StartConversation(DialogueText, component, !OverrideContinue.IsNone && OverrideContinue.Value, GetDisplayOptions(new DialogueBox.DisplayOptions
			{
				ShowDecorators = !HideDecorators.Value,
				Alignment = (TextAlignmentOptions)(object)TextAlignment.Value,
				OffsetY = OffsetY.Value,
				TextColor = Color.white
			}), action, action);
			static void ResetBlocked()
			{
				HeroTalkAnimation.SetBlocked(value: false);
			}
		}

		protected virtual DialogueBox.DisplayOptions GetDisplayOptions(DialogueBox.DisplayOptions defaultOptions)
		{
			return defaultOptions;
		}
	}
}
