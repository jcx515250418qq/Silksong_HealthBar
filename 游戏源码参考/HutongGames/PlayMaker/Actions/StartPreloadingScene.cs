using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	public class StartPreloadingScene : FsmStateAction
	{
		public FsmString SceneName;

		[ObjectType(typeof(LoadSceneMode))]
		public FsmEnum LoadMode;

		public override void Reset()
		{
			SceneName = null;
			LoadMode = null;
		}

		public override void OnEnter()
		{
			ScenePreloader.SpawnPreloader(SceneName.Value, (LoadSceneMode)(object)LoadMode.Value);
			Finish();
		}
	}
}
