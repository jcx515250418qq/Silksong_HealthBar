using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetNPCSpeakingAudioTable : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NPCSpeakingAudio))]
		public FsmOwnerDefault Target;

		[RequiredField]
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		public override void Reset()
		{
			Target = null;
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
					component.SetTableForSpeaker(null, Table.Value as RandomAudioClipTable);
				}
			}
			Finish();
		}
	}
}
