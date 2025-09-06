using UnityEngine;

public class AmbientLightAnimator : MonoBehaviour
{
	[SerializeField]
	private float value;

	private float initialValue;

	private float oldValue;

	private SceneColorManager sm;

	private void Start()
	{
		sm = GameCameras.instance.sceneColorManager;
		initialValue = sm.AmbientIntensityA;
		sm.AmbientIntensityA = value;
		oldValue = value;
	}

	private void Update()
	{
		if (value != oldValue)
		{
			sm.AmbientIntensityA = value;
			oldValue = value;
		}
	}
}
