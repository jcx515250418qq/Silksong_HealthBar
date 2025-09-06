using UnityEngine;

public class InGameCutsceneInfo : MonoBehaviour
{
	private static InGameCutsceneInfo _instance;

	[SerializeField]
	private Vector2 cameraPosition;

	public static bool IsInCutscene
	{
		get
		{
			if (_instance == null)
			{
				return false;
			}
			GameManager instance = GameManager.instance;
			if ((bool)instance)
			{
				return instance.GetSceneNameString() == _instance.gameObject.scene.name;
			}
			return true;
		}
	}

	public static Vector2 CameraPosition
	{
		get
		{
			if (!(_instance != null))
			{
				return Vector2.zero;
			}
			return _instance.cameraPosition;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
