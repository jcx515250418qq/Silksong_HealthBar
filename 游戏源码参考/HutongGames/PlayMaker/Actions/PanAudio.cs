using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class PanAudio : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		public FsmFloat targetPan;

		public FsmFloat overTime;

		private float startPan;

		private float timeElapsed;

		private float timePercentage;

		private bool panningLeft;

		public override void Reset()
		{
			gameObject = null;
			targetPan = 0f;
			overTime = 1f;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				startPan = base.audio.panStereo;
			}
			if (startPan > targetPan.Value)
			{
				panningLeft = true;
			}
			else
			{
				panningLeft = false;
			}
		}

		public override void OnExit()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				base.audio.panStereo = targetPan.Value;
			}
		}

		public override void OnUpdate()
		{
			DoPan();
		}

		private void DoPan()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				timeElapsed += Time.deltaTime;
				timePercentage = timeElapsed / overTime.Value * 100f;
				float num = (targetPan.Value - startPan) * (timePercentage / 100f);
				base.audio.panStereo = base.audio.panStereo + num;
				if (panningLeft && base.audio.panStereo <= targetPan.Value)
				{
					base.audio.panStereo = targetPan.Value;
					Finish();
				}
				else if (!panningLeft && base.audio.panStereo >= targetPan.Value)
				{
					base.audio.panStereo = targetPan.Value;
					Finish();
				}
				timeElapsed = 0f;
			}
		}
	}
}
