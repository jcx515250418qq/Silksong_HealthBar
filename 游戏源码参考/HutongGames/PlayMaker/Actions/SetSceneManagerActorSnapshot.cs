using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
	public class SetSceneManagerActorSnapshot : FsmStateAction
	{
		[ObjectType(typeof(AudioMixerSnapshot))]
		public FsmObject snapshot;

		public override void Reset()
		{
			snapshot = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if ((bool)instance)
			{
				instance.sm.actorSnapshot = snapshot.Value as AudioMixerSnapshot;
			}
			Finish();
		}
	}
}
