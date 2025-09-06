using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class FadeAudioV2 : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault Target;

		public FsmFloat StartVolume;

		public FsmFloat EndVolume;

		public FsmFloat Time;

		public FsmBool SetOnExit;

		private float timeElapsed;

		private float timePercentage;

		private bool fadingDown;

		public override void Reset()
		{
			Target = null;
			StartVolume = 1f;
			EndVolume = 0f;
			Time = 1f;
			SetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(Target);
			if (UpdateCache(ownerDefaultTarget))
			{
				if (StartVolume.IsNone)
				{
					StartVolume.Value = base.audio.volume;
				}
				else
				{
					base.audio.volume = StartVolume.Value;
				}
			}
			if (StartVolume.Value > EndVolume.Value)
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
			if (SetOnExit.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(Target);
				if (UpdateCache(ownerDefaultTarget))
				{
					base.audio.volume = EndVolume.Value;
				}
			}
		}

		public override void OnUpdate()
		{
			DoSetAudioVolume();
		}

		private void DoSetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(Target);
			if (UpdateCache(ownerDefaultTarget))
			{
				timeElapsed += UnityEngine.Time.deltaTime;
				timePercentage = timeElapsed / Time.Value * 100f;
				float num = (EndVolume.Value - StartVolume.Value) * (timePercentage / 100f);
				base.audio.volume = base.audio.volume + num;
				if (fadingDown && base.audio.volume <= EndVolume.Value)
				{
					base.audio.volume = EndVolume.Value;
					Finish();
				}
				else if (!fadingDown && base.audio.volume >= EndVolume.Value)
				{
					base.audio.volume = EndVolume.Value;
					Finish();
				}
				timeElapsed = 0f;
			}
		}
	}
}
