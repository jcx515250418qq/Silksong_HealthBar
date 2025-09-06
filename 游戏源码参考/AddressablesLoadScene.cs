using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablesLoadScene : MonoBehaviour
{
	[SerializeField]
	private AssetReference loadScene;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasSceneRef", false, false, false)]
	private string address;

	private bool HasSceneRef => !string.IsNullOrEmpty(loadScene.AssetGUID);

	private void Start()
	{
		if (HasSceneRef)
		{
			Addressables.LoadSceneAsync(loadScene);
		}
		else
		{
			Addressables.LoadSceneAsync(address);
		}
	}
}
