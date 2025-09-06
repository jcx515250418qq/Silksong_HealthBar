using UnityEngine;

namespace StateMachineBehaviours
{
	public sealed class AnimatorStateVibration : StateMachineBehaviour
	{
		[SerializeField]
		private VibrationDataAsset vibrationDataAsset;

		[SerializeField]
		private float strength = 1f;

		[SerializeField]
		private bool loop;

		[SerializeField]
		private bool isRealTime;

		[SerializeField]
		private string tag;

		[Space]
		[SerializeField]
		private bool stopOnExit;

		[SerializeField]
		private bool stopOnAnimFinished;

		private VibrationEmission emission;

		private void OnDisable()
		{
			StopVibration();
		}

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PlayVibration();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stopOnExit || loop)
			{
				StopVibration();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stopOnAnimFinished && !stateInfo.loop && (stateInfo.normalizedTime >= 1f || stateInfo.normalizedTime <= -1f))
			{
				StopVibration();
			}
		}

		public void PlayVibration()
		{
			if (!loop || emission == null)
			{
				VibrationData vibrationData = vibrationDataAsset;
				bool isLooping = loop;
				bool isRealtime = isRealTime;
				string text = tag;
				emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, text, isRealtime);
				emission?.SetStrength(strength);
			}
		}

		private void StopVibration()
		{
			emission?.Stop();
			emission = null;
		}
	}
}
