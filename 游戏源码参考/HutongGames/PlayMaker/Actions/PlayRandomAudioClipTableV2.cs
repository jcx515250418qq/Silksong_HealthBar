using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PlayRandomAudioClipTableV2 : FsmStateAction
	{
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		public FsmOwnerDefault SpawnPoint;

		public FsmVector3 SpawnPosition;

		public FsmBool ForcePlay;

		public override void Reset()
		{
			Table = null;
			AudioPlayerPrefab = new FsmObject
			{
				UseVariable = true
			};
			SpawnPoint = null;
			SpawnPosition = null;
			ForcePlay = null;
		}

		public override void OnEnter()
		{
			Vector3 value = SpawnPosition.Value;
			GameObject safe = SpawnPoint.GetSafe(this);
			if ((bool)safe)
			{
				value += safe.transform.position;
			}
			RandomAudioClipTable randomAudioClipTable = Table.Value as RandomAudioClipTable;
			if (randomAudioClipTable != null)
			{
				if ((bool)AudioPlayerPrefab.Value)
				{
					randomAudioClipTable.SpawnAndPlayOneShot(AudioPlayerPrefab.Value as AudioSource, value, ForcePlay.Value);
				}
				else
				{
					randomAudioClipTable.SpawnAndPlayOneShot(value, ForcePlay.Value);
				}
			}
			Finish();
		}
	}
}
