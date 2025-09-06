using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Level)]
	[Note("Reloads the current scene.")]
	[Tooltip("Reloads the current scene.")]
	public class RestartLevel : FsmStateAction
	{
		public override void OnEnter()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
			Finish();
		}
	}
}
