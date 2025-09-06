using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class FadeAudio : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		public FsmFloat startVolume;

		public FsmFloat endVolume;

		public FsmFloat time;

		private float timeElapsed;

		private float timePercentage;

		private bool fadingDown;

		private bool hasBlendController;

		private VolumeModifier volumeModifier;

		private int lastCacheVersion;

		public override void Reset()
		{
			gameObject = null;
			startVolume = 1f;
			endVolume = 0f;
			time = 1f;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				UpdateBlendCache();
				if (startVolume.IsNone)
				{
					startVolume.Value = GetVolume();
				}
				else
				{
					SetVolume(startVolume.Value);
				}
			}
			if (startVolume.Value > endVolume.Value)
			{
				fadingDown = true;
			}
			else
			{
				fadingDown = false;
			}
		}

		public override void OnExit()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				UpdateBlendCache();
				SetVolume(endVolume.Value);
			}
		}

		public override void OnUpdate()
		{
			DoSetAudioVolume();
		}

		private void DoSetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				UpdateBlendCache();
				timeElapsed += Time.deltaTime;
				timePercentage = timeElapsed / time.Value * 100f;
				float num = (endVolume.Value - startVolume.Value) * (timePercentage / 100f);
				SetVolume(GetVolume() + num);
				if (fadingDown && base.audio.volume <= endVolume.Value)
				{
					SetVolume(endVolume.Value);
					Finish();
				}
				else if (!fadingDown && base.audio.volume >= endVolume.Value)
				{
					SetVolume(endVolume.Value);
					Finish();
				}
				timeElapsed = 0f;
			}
		}

		private float GetVolume()
		{
			if (hasBlendController)
			{
				return volumeModifier.Volume;
			}
			return base.audio.volume;
		}

		private void SetVolume(float volume)
		{
			if (hasBlendController)
			{
				volumeModifier.Volume = volume;
			}
			else
			{
				base.audio.volume = volume;
			}
		}

		private void UpdateBlendCache()
		{
			if (lastCacheVersion != cacheVersion)
			{
				lastCacheVersion = cacheVersion;
				VolumeBlendController component = base.audio.GetComponent<VolumeBlendController>();
				if (component != null)
				{
					hasBlendController = true;
					volumeModifier = component.GetSharedFSMModifier();
				}
			}
		}
	}
}
