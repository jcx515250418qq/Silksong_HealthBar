using UnityEngine;

public sealed class SetMappedOnStart : MonoBehaviour
{
	[SerializeField]
	private string sceneName;

	private void Start()
	{
		PlayerData instance = PlayerData.instance;
		if (instance.hasQuill || instance.QuillState > 0)
		{
			instance.scenesMapped.Add(sceneName);
		}
	}

	private void Reset()
	{
		sceneName = base.gameObject.scene.name;
	}
}
