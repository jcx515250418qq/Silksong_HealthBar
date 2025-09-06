using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
	[SerializeField]
	[ArrayForEnum(typeof(GameManager.SceneLoadVisualizations))]
	private LoadingSpinner[] visualizationContainers;

	[SerializeField]
	private LoadingSpinner defaultLoadingSpinner;

	[SerializeField]
	private float continueFromSaveDelayAdjustment;

	[SerializeField]
	private float threadMemoryDelayAdjustment;

	private bool isLoading;

	private GameManager.SceneLoadVisualizations loadingVisualization;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref visualizationContainers, typeof(GameManager.SceneLoadVisualizations));
	}

	private void Awake()
	{
		OnValidate();
	}

	protected void Start()
	{
		LoadingSpinner[] array = visualizationContainers;
		foreach (LoadingSpinner loadingSpinner in array)
		{
			if (!(loadingSpinner == null))
			{
				loadingSpinner.SetActive(value: false, isInstant: true);
			}
		}
	}

	protected void Update()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance == null || isLoading == silentInstance.IsLoadingSceneTransition)
		{
			return;
		}
		isLoading = silentInstance.IsLoadingSceneTransition;
		if (isLoading)
		{
			LoadingSpinner loadingSpinner = defaultLoadingSpinner;
			loadingSpinner.DisplayDelayAdjustment = silentInstance.LoadVisualization switch
			{
				GameManager.SceneLoadVisualizations.ContinueFromSave => continueFromSaveDelayAdjustment, 
				GameManager.SceneLoadVisualizations.ThreadMemory => threadMemoryDelayAdjustment, 
				_ => 0f, 
			};
		}
		LoadingSpinner loadingSpinner2 = null;
		if (isLoading && silentInstance.LoadVisualization >= GameManager.SceneLoadVisualizations.Default && (int)silentInstance.LoadVisualization < visualizationContainers.Length)
		{
			loadingSpinner2 = visualizationContainers[(int)silentInstance.LoadVisualization];
		}
		LoadingSpinner[] array = visualizationContainers;
		foreach (LoadingSpinner loadingSpinner3 in array)
		{
			if (!(loadingSpinner3 == null))
			{
				loadingSpinner3.SetActive(loadingSpinner3 == loadingSpinner2, isInstant: false);
			}
		}
	}
}
