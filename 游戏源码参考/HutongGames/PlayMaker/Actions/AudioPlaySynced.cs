using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AudioPlaySynced : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmGameObject ReadSource;

		public override void Reset()
		{
			Target = null;
			ReadSource = null;
		}

		public override void OnEnter()
		{
			DoAction();
			Finish();
		}

		private void DoAction()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			GameObject value = ReadSource.Value;
			if (!value)
			{
				return;
			}
			AudioSource component = safe.GetComponent<AudioSource>();
			AudioSource component2 = value.GetComponent<AudioSource>();
			if ((bool)component && (bool)component2)
			{
				if (!component.isPlaying)
				{
					component.Play();
				}
				if (component2.isPlaying)
				{
					component.timeSamples = component2.timeSamples;
				}
			}
		}
	}
}
