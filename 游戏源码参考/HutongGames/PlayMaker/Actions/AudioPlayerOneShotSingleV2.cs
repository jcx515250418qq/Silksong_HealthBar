using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Instantiate an Audio Player object and play a oneshot sound via its Audio Source.")]
	public class AudioPlayerOneShotSingleV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The object to spawn. Select Audio Player prefab.")]
		public FsmGameObject audioPlayer;

		[RequiredField]
		[Tooltip("Object to use as the spawn point of Audio Player")]
		public FsmGameObject spawnPoint;

		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		public FsmBool playVibration;

		public FsmFloat pitchMin;

		public FsmFloat pitchMax;

		public FsmFloat volume = 1f;

		public FsmFloat delay;

		public FsmGameObject storePlayer;

		private AudioSource audio;

		private float timer;

		public override void Reset()
		{
			spawnPoint = null;
			pitchMin = 1f;
			pitchMax = 1f;
			volume = 1f;
			playVibration = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
			if (delay.Value == 0f)
			{
				DoPlayRandomClip();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (delay.Value > 0f)
			{
				if (timer < delay.Value)
				{
					timer += Time.deltaTime;
					return;
				}
				DoPlayRandomClip();
				Finish();
			}
		}

		private void DoPlayRandomClip()
		{
			if (!this.audioClip.Value)
			{
				return;
			}
			if (audioPlayer.IsNone || audioPlayer.Value == null)
			{
				Debug.LogError("AudioPlayer object not set!");
			}
			else
			{
				if (spawnPoint.IsNone || spawnPoint.Value == null)
				{
					return;
				}
				Vector3 position = spawnPoint.Value.transform.position;
				Vector3 up = Vector3.up;
				GameObject gameObject = audioPlayer.Value.Spawn(position, Quaternion.Euler(up));
				audio = gameObject.GetComponent<AudioSource>();
				storePlayer.Value = gameObject;
				AudioClip audioClip = this.audioClip.Value as AudioClip;
				if ((bool)audioClip)
				{
					float pitch = Random.Range(pitchMin.Value, pitchMax.Value);
					audio.pitch = pitch;
					audio.volume = volume.Value;
					audio.PlayOneShot(audioClip);
					if ((bool)vibrationDataAsset.Value)
					{
						VibrationManager.PlayVibrationClipOneShot((VibrationDataAsset)vibrationDataAsset.Value, null);
					}
				}
			}
		}
	}
}
