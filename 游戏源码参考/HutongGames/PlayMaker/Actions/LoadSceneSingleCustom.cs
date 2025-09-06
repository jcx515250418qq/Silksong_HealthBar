using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	public class LoadSceneSingleCustom : FsmStateAction
	{
		[RequiredField]
		public FsmString LevelName;

		private AsyncOperationHandle<SceneInstance>? asyncOperation;

		public override void Reset()
		{
			LevelName = null;
		}

		public override void OnEnter()
		{
			string text = "Scenes/" + LevelName.Value;
			asyncOperation = ScenePreloader.TakeSceneLoadOperation(text, LoadSceneMode.Single);
			if (asyncOperation.HasValue)
			{
				if (asyncOperation.Value.IsDone)
				{
					asyncOperation.Value.Result.ActivateAsync();
				}
				else
				{
					AsyncOperationHandle<SceneInstance> value = asyncOperation.Value;
					value.Completed += delegate
					{
						asyncOperation.Value.Result.ActivateAsync();
					};
				}
			}
			else
			{
				asyncOperation = Addressables.LoadSceneAsync(text);
			}
			GameManager.instance.LastSceneLoad = new SceneLoad(asyncOperation.Value, new GameManager.SceneLoadInfo
			{
				IsFirstLevelForPlayer = true,
				SceneName = LevelName.Value
			});
			Finish();
		}
	}
}
