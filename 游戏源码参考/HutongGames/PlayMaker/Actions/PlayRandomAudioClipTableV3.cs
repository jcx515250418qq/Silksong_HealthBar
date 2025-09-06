using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PlayRandomAudioClipTableV3 : FsmStateAction
	{
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		public FsmOwnerDefault SpawnPoint;

		public FsmVector3 SpawnPosition;

		public FsmBool ForcePlay;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreSpawned;

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
			StoreSpawned = null;
		}

		public override void OnEnter()
		{
			StoreSpawned.Value = null;
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
					AudioSource audioSource = randomAudioClipTable.SpawnAndPlayOneShot(AudioPlayerPrefab.Value as AudioSource, value, ForcePlay.Value);
					if ((bool)audioSource)
					{
						StoreSpawned.Value = audioSource.gameObject;
					}
				}
				else
				{
					AudioSource audioSource2 = randomAudioClipTable.SpawnAndPlayOneShot(value, ForcePlay.Value);
					if ((bool)audioSource2)
					{
						StoreSpawned.Value = audioSource2.gameObject;
					}
				}
			}
			Finish();
		}
	}
}
