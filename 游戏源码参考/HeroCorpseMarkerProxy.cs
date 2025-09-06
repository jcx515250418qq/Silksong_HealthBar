using UnityEngine;

public class HeroCorpseMarkerProxy : MonoBehaviour
{
	[SerializeField]
	private byte[] targetGuid;

	[SerializeField]
	private string readScenePosFromStaticVar;

	public byte[] TargetGuid => targetGuid;

	public string TargetSceneName => FastTravelScenes.GetSceneName(PlayerData.instance.FastTravelNPCLocation);

	public Vector2 TargetScenePos => StaticVariableList.GetValue<Vector2>(readScenePosFromStaticVar);

	public static HeroCorpseMarkerProxy Instance { get; private set; }

	private void OnEnable()
	{
		Instance = this;
	}

	private void OnDisable()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
