using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class PlayRandomAudioClipTableLooped : FsmStateAction
	{
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		[Tooltip("If set, the audio clip will be played on a spawned instance of this prefab.")]
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		[Tooltip("If set, the audio clip will be played from this audio source.")]
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerSource;

		[Tooltip("Whether to wait for previous clip to finish playback before playing again.")]
		public FsmBool waitForPreviousClip;

		public FsmFloat minRate;

		public FsmFloat maxRate;

		public FsmOwnerDefault SpawnPoint;

		public FsmVector3 SpawnPosition;

		private AudioSource currentAudioSource;

		private float timer;

		private bool useCurrentSource;

		private bool hasPlayerPrefab;

		private AudioSource audioSourcePrefab;

		private RandomAudioClipTable table;

		private float min;

		private float max;

		private Vector3 position;

		public override void Reset()
		{
			AudioPlayerPrefab = new FsmObject
			{
				UseVariable = true
			};
			AudioPlayerSource = new FsmObject
			{
				UseVariable = true
			};
			waitForPreviousClip = null;
			minRate = null;
			maxRate = null;
		}

		public override void OnEnter()
		{
			table = Table.Value as RandomAudioClipTable;
			if (table != null)
			{
				position = SpawnPosition.Value;
				GameObject safe = SpawnPoint.GetSafe(this);
				if ((bool)safe)
				{
					position += safe.transform.position;
				}
				if (!AudioPlayerSource.IsNone)
				{
					currentAudioSource = AudioPlayerSource.Value as AudioSource;
					useCurrentSource = currentAudioSource != null;
				}
				else
				{
					useCurrentSource = false;
				}
				if (!useCurrentSource)
				{
					audioSourcePrefab = AudioPlayerPrefab.Value as AudioSource;
					hasPlayerPrefab = audioSourcePrefab != null;
				}
				else
				{
					audioSourcePrefab = null;
				}
				if (useCurrentSource)
				{
					table.PlayOneShotUnsafe(currentAudioSource, 0f, forcePlay: true);
				}
				else if (hasPlayerPrefab)
				{
					currentAudioSource = table.SpawnAndPlayOneShot(audioSourcePrefab, position, forcePlay: true);
				}
				else
				{
					currentAudioSource = table.SpawnAndPlayOneShot(position, forcePlay: true);
				}
				if (currentAudioSource == null)
				{
					Finish();
				}
				else
				{
					timer = GetNextPlay();
				}
			}
			else
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			currentAudioSource = null;
			audioSourcePrefab = null;
		}

		private float GetNextPlay()
		{
			float value = minRate.Value;
			float num = maxRate.Value;
			if (num < value)
			{
				num = value;
			}
			return Random.Range(value, num);
		}

		public override void OnUpdate()
		{
			if (waitForPreviousClip.Value && currentAudioSource != null && currentAudioSource.isPlaying)
			{
				return;
			}
			timer -= Time.deltaTime;
			if (!(timer <= 0f))
			{
				return;
			}
			timer = GetNextPlay();
			if (useCurrentSource && currentAudioSource != null)
			{
				if (currentAudioSource != null)
				{
					table.PlayOneShotUnsafe(currentAudioSource, 0f, forcePlay: true);
					return;
				}
				useCurrentSource = false;
			}
			if (hasPlayerPrefab)
			{
				currentAudioSource = table.SpawnAndPlayOneShot(audioSourcePrefab, position, forcePlay: true);
			}
			else
			{
				currentAudioSource = table.SpawnAndPlayOneShot(position, forcePlay: true);
			}
		}
	}
}
