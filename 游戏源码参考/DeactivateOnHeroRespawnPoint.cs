using UnityEngine;

public class DeactivateOnHeroRespawnPoint : MonoBehaviour
{
	[SerializeField]
	private string respawnPointName;

	[SerializeField]
	private string respawnSceneName;

	private void OnEnable()
	{
		PlayerData instance = PlayerData.instance;
		string respawnMarkerName = instance.respawnMarkerName;
		string respawnScene = instance.respawnScene;
		if (!string.IsNullOrEmpty(respawnMarkerName) && !string.IsNullOrEmpty(respawnScene) && respawnMarkerName == respawnPointName && respawnScene == respawnSceneName)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
