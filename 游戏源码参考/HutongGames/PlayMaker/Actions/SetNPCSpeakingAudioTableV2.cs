using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetNPCSpeakingAudioTableV2 : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NPCSpeakingAudio))]
		public FsmOwnerDefault Target;

		public FsmString SpeakerEvent;

		[RequiredField]
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		public override void Reset()
		{
			Target = null;
			SpeakerEvent = new FsmString
			{
				UseVariable = true
			};
			Table = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				NPCSpeakingAudio component = safe.GetComponent<NPCSpeakingAudio>();
				if ((bool)component)
				{
					component.SetTableForSpeaker(SpeakerEvent.IsNone ? null : SpeakerEvent.Value, Table.Value as RandomAudioClipTable);
				}
			}
			Finish();
		}
	}
}
