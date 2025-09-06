using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs a detailed deprecation message.")]
	public class DebugLogDeprecatedEvent : BaseLogAction
	{
		[Tooltip("Text to send to the log.")]
		public FsmString noteText;

		public override void Reset()
		{
			noteText = "";
			base.Reset();
		}

		public override void OnEnter()
		{
			string arg = ((Fsm.EventData.SentByFsm != null) ? string.Format("{0}.{1}.{2}.{3}", (Fsm.EventData.SentByFsm.GameObject == null) ? "<unknown-game-object>" : Fsm.EventData.SentByFsm.GameObject.name, Fsm.EventData.SentByFsm.Name, (Fsm.EventData.SentByState == null) ? "<unknown-state>" : Fsm.EventData.SentByState.Name, (Fsm.EventData.SentByAction == null) ? "<unknown-action>" : Fsm.EventData.SentByAction.Name) : "<native-code>");
			PlayMakerFSM playMakerFSM = base.Fsm.Owner as PlayMakerFSM;
			string arg2 = ((!(playMakerFSM == null)) ? $"{playMakerFSM.gameObject.name}.{base.Fsm.Name}.{base.Fsm.ActiveStateName}" : "<no-owner>");
			string value = noteText.Value;
			string text = $"Entry to {arg2} (sent by {arg}) is deprecated";
			if (!string.IsNullOrEmpty(value))
			{
				text = text + ": " + value;
			}
			Debug.LogError(text, playMakerFSM);
			Finish();
		}
	}
}
