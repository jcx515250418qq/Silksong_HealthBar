using UnityEngine;

public class GradeOverride : MonoBehaviour
{
	[Header("Overriding Color Grade")]
	[Range(0f, 5f)]
	public float saturation;

	public AnimationCurve redChannel;

	public AnimationCurve greenChannel;

	public AnimationCurve blueChannel;

	[Header("Overriding Scene Lighting")]
	[Range(0f, 1f)]
	public float ambientIntensity;

	public Color ambientColor;

	[Header("Overriding Hero Light")]
	public Color heroLightColor;

	private float o_saturation;

	private AnimationCurve o_redChannel;

	private AnimationCurve o_greenChannel;

	private AnimationCurve o_blueChannel;

	private float o_ambientIntensity;

	private Color o_ambientColor;

	private Color o_heroLightColor;

	private GameCameras gc;

	private HeroController hero;

	private SceneColorManager scm;

	private void Start()
	{
		gc = GameCameras.instance;
		hero = HeroController.instance;
	}

	private void OnEnable()
	{
		Invoke("Activate", 0.1f);
	}

	private void OnDisable()
	{
		Deactivate();
	}

	public void Activate()
	{
		gc = GameCameras.instance;
		scm = gc.sceneColorManager;
		hero = HeroController.instance;
		o_saturation = scm.SaturationA;
		o_redChannel = scm.RedA;
		o_greenChannel = scm.GreenA;
		o_blueChannel = scm.BlueA;
		o_ambientIntensity = scm.AmbientIntensityA;
		o_ambientColor = scm.AmbientColorA;
		scm.SaturationA = CustomSceneManager.AdjustSaturationForPlatform(saturation, null);
		scm.RedA = redChannel;
		scm.GreenA = greenChannel;
		scm.BlueA = blueChannel;
		scm.AmbientColorA = ambientColor;
		scm.AmbientIntensityA = ambientIntensity;
		CustomSceneManager.SetLighting(ambientColor, ambientIntensity);
		if (GameManager.instance.IsGameplayScene())
		{
			o_heroLightColor = scm.HeroLightColorA;
			scm.HeroLightColorA = heroLightColor;
		}
		scm.SetMarkerActive(active: true);
		StartCoroutine(scm.ForceRefresh());
	}

	public void Deactivate()
	{
		scm.SaturationA = o_saturation;
		scm.RedA = o_redChannel;
		scm.GreenA = o_greenChannel;
		scm.BlueA = o_blueChannel;
		scm.AmbientColorA = o_ambientColor;
		scm.AmbientIntensityA = o_ambientIntensity;
		CustomSceneManager.SetLighting(o_ambientColor, o_ambientIntensity);
		if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
		{
			scm.HeroLightColorA = o_heroLightColor;
		}
	}
}
