using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetRandomAudioClipFromTable : FsmStateAction
	{
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		public FsmBool ForcePlay;

		[ObjectType(typeof(AudioClip))]
		public FsmObject StoreClip;

		public override void Reset()
		{
			Table = null;
			ForcePlay = null;
			StoreClip = null;
		}

		public override void OnEnter()
		{
			RandomAudioClipTable randomAudioClipTable = Table.Value as RandomAudioClipTable;
			if (randomAudioClipTable != null)
			{
				StoreClip.Value = randomAudioClipTable.SelectClip(ForcePlay.Value);
			}
			Finish();
		}
	}
}
