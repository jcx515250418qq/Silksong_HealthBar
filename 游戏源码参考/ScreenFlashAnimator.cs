using UnityEngine;

public class ScreenFlashAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject requiredVisible;

	[SerializeField]
	private Color[] screenFlashColours = new Color[1]
	{
		new Color(1f, 1f, 1f, 0.5f)
	};

	private Renderer[] requiredVisibleRenderers;

	private void Awake()
	{
		if ((bool)requiredVisible)
		{
			requiredVisibleRenderers = requiredVisible.GetComponentsInChildren<Renderer>();
		}
	}

	private bool CanFlash()
	{
		if (requiredVisibleRenderers == null)
		{
			return true;
		}
		Renderer[] array = requiredVisibleRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isVisible)
			{
				return true;
			}
		}
		return false;
	}

	public void DoScreenFlash(int index)
	{
		if (CanFlash() && index >= 0 && index < screenFlashColours.Length)
		{
			Color colour = screenFlashColours[index];
			GameCameras.instance.cameraController.ScreenFlash(colour);
		}
	}
}
