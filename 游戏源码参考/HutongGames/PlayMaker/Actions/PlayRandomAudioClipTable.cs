using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PlayRandomAudioClipTable : FsmStateAction
	{
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		public FsmOwnerDefault SpawnPoint;

		public FsmVector3 SpawnPosition;

		public override void Reset()
		{
			AudioPlayerPrefab = new FsmObject
			{
				UseVariable = true
			};
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
					randomAudioClipTable.SpawnAndPlayOneShot(AudioPlayerPrefab.Value as AudioSource, value);
				}
				else
				{
					randomAudioClipTable.SpawnAndPlayOneShot(value);
				}
			}
			Finish();
		}
	}
}
