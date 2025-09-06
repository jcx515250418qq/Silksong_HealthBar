using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Instantiate an Audio Player object and play a oneshot sound via its Audio Source.")]
	public class AudioPlayerOneShotSingle : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The object to spawn. Select Audio Player prefab.")]
		public FsmGameObject audioPlayer;

		[RequiredField]
		[Tooltip("Object to use as the spawn point of Audio Player")]
		public FsmGameObject spawnPoint;

		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

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
			AudioClip audioClip = this.audioClip.Value as AudioClip;
			if (audioClip == null || audioPlayer.IsNone || spawnPoint.IsNone || !(spawnPoint.Value != null))
			{
				return;
			}
			_ = audioPlayer.Value;
			Vector3 position = spawnPoint.Value.transform.position;
			Vector3 up = Vector3.up;
			if (audioPlayer.Value != null)
			{
				GameObject newObject = audioPlayer.Value.Spawn(position, Quaternion.Euler(up));
				audio = newObject.GetComponent<AudioSource>();
				if (!(audio != null))
				{
					return;
				}
				if (!storePlayer.IsNone)
				{
					storePlayer.Value = newObject;
					RecycleResetHandler.Add(newObject, (Action)delegate
					{
						if (!storePlayer.IsNone && storePlayer.Value == newObject)
						{
							storePlayer.Value = null;
						}
					});
				}
				float pitch = UnityEngine.Random.Range(pitchMin.Value, pitchMax.Value);
				audio.pitch = pitch;
				audio.volume = volume.Value;
				audio.PlayOneShot(audioClip);
			}
			else
			{
				Debug.LogError("AudioPlayer object not set!");
			}
		}
	}
}
