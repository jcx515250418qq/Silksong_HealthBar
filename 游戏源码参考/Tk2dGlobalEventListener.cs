using UnityEngine;

public class Tk2dGlobalEventListener : Tk2dGlobalEvents.IListener
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		Tk2dGlobalEvents.AddListener(new Tk2dGlobalEventListener());
	}

	public void ColliderUpdated(GameObject gameObject)
	{
		DebugDrawColliderRuntime.AddOrUpdate(gameObject);
	}

	public void TilemapChunkCreated(Transform grandChild)
	{
		grandChild.gameObject.AddComponent<DebugDrawColliderRuntime>();
	}

	public bool IsFrozenCameraRendering()
	{
		return DisplayFrozenCamera.IsRendering;
	}
}
