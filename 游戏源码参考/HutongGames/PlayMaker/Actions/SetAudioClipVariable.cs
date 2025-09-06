using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class SetAudioClipVariable : FsmStateAction
	{
		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip variable.")]
		public FsmObject audioClipVariable;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip to store.")]
		public FsmObject audioClip;

		public override void Reset()
		{
			audioClipVariable = null;
			audioClip = null;
		}

		public override void OnEnter()
		{
			audioClipVariable.Value = audioClip.Value;
			Finish();
		}
	}
}
