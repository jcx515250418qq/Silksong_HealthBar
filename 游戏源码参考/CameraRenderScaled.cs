using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class CameraRenderScaled : MonoBehaviour
{
	[SerializeField]
	private CameraRenderScaledApply applyTo;

	private RenderTexture renderTex;

	private Camera camera;

	public static ScreenRes Resolution;

	private static bool forceFullResolutionV2;

	private static HashSet<MonoBehaviour> fullResotionSources = new HashSet<MonoBehaviour>();

	public bool ForceFullResolution { get; set; }

	public static void Clear()
	{
		Resolution = new ScreenRes(0, 0);
	}

	private void Awake()
	{
		camera = GetComponent<Camera>();
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
		Scene activeScene = SceneManager.GetActiveScene();
		OnActiveSceneChanged(activeScene, activeScene);
	}

	private void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnActiveSceneChanged;
		ClearRenderTexture();
	}

	private void ClearRenderTexture()
	{
		camera.targetTexture = null;
		if (renderTex != null)
		{
			renderTex.Release();
			Object.Destroy(renderTex);
			renderTex = null;
		}
	}

	private void OnPreCull()
	{
		if (!Application.isPlaying || !applyTo || !applyTo.IsActive)
		{
			ClearRenderTexture();
			return;
		}
		ScreenRes resolution = Resolution;
		int width = resolution.Width;
		int height = resolution.Height;
		bool flag = ForceFullResolution || forceFullResolutionV2 || width <= 0 || height <= 0;
		if (flag)
		{
			width = Screen.width;
			height = Screen.height;
		}
		if (renderTex != null && (flag || width != renderTex.width || height != renderTex.height))
		{
			Object.Destroy(renderTex);
			renderTex = null;
			if (flag && (bool)applyTo)
			{
				applyTo.Texture = null;
				applyTo.SourceCamera = null;
			}
		}
		if (flag)
		{
			ClearRenderTexture();
		}
		else if (!(renderTex != null))
		{
			renderTex = new RenderTexture(width, height, 32, RenderTextureFormat.Default)
			{
				hideFlags = HideFlags.HideAndDontSave,
				name = "CameraRenderScaled" + GetInstanceID()
			};
			camera.targetTexture = renderTex;
			if ((bool)applyTo)
			{
				applyTo.Texture = renderTex;
				applyTo.SourceCamera = camera;
			}
		}
	}

	private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
	{
		string value = newScene.name;
		bool forceFullResolution = value.IsAny(WorldInfo.MenuScenes) || value.IsAny(WorldInfo.NonGameplayScenes);
		fullResotionSources.RemoveWhere((MonoBehaviour o) => o == null);
		ForceFullResolution = forceFullResolution;
	}

	public static void AddForceFullResolution(MonoBehaviour source)
	{
		if (!(source == null) && fullResotionSources.Add(source))
		{
			forceFullResolutionV2 = true;
		}
	}

	public static void RemoveForceFullResolution(MonoBehaviour source)
	{
		if (fullResotionSources.Remove(source) && fullResotionSources.Count == 0)
		{
			forceFullResolutionV2 = false;
		}
	}
}
