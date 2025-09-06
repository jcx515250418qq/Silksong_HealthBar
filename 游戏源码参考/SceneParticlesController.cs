using GlobalEnums;
using UnityEngine;

public class SceneParticlesController : MonoBehaviour
{
	[SerializeField]
	private GameObject defaultParticles;

	[SerializeField]
	[ArrayForEnum(typeof(MapZone))]
	private GameObject[] mapZoneParticles;

	[SerializeField]
	[ArrayForEnum(typeof(CustomSceneParticles))]
	private GameObject[] customParticles;

	[Space]
	[SerializeField]
	private GameObject act3Particles;

	private GameManager gm;

	private CustomSceneManager sm;

	private MapZone sceneParticleZoneType;

	private GameCameras gc;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref mapZoneParticles, typeof(MapZone));
		ArrayForEnumAttribute.EnsureArraySize(ref customParticles, typeof(CustomSceneParticles));
	}

	private void Awake()
	{
		OnValidate();
	}

	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += OnCameraAspectChanged;
		gc = GameCameras.instance;
		gc.cameraController.PositionedAtHero += OnPositionedAtHero;
		OnPositionedAtHero();
	}

	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
		if (gc != null)
		{
			gc.cameraController.PositionedAtHero -= OnPositionedAtHero;
			gc = null;
		}
	}

	private void OnCameraAspectChanged(float aspect)
	{
		float num = aspect / 1.7777778f;
		Transform obj = base.transform;
		Vector3 localScale = obj.localScale;
		localScale.x = localScale.y * num;
		obj.localScale = localScale;
	}

	public void EnableParticles(bool noSceneParticles)
	{
		sceneParticleZoneType = ((sm.overrideParticlesWith == MapZone.NONE) ? sm.mapZone : sm.overrideParticlesWith);
		DisableParticles();
		bool flag = PlayerData.instance.blackThreadWorld;
		if (flag)
		{
			if (sm.act3ParticlesOverride.IsEnabled)
			{
				flag = sm.act3ParticlesOverride.Value == CustomSceneManager.BoolFriendly.On;
			}
			else
			{
				switch (sm.mapZone)
				{
				case MapZone.NONE:
				case MapZone.CLOVER:
				case MapZone.PEAK:
				case MapZone.MEMORY:
				case MapZone.SURFACE:
					flag = false;
					break;
				}
			}
		}
		act3Particles.SetActive(flag);
		if (noSceneParticles)
		{
			return;
		}
		GameObject gameObject = defaultParticles;
		if (sm.overrideParticlesWithCustom != 0)
		{
			GameObject gameObject2 = customParticles[(int)sm.overrideParticlesWithCustom];
			if ((bool)gameObject2)
			{
				gameObject = gameObject2;
			}
		}
		else if (sceneParticleZoneType != 0)
		{
			GameObject gameObject3 = mapZoneParticles[(int)sceneParticleZoneType];
			if ((bool)gameObject3)
			{
				gameObject = gameObject3;
			}
		}
		if ((bool)gameObject)
		{
			gameObject.SetActive(value: true);
		}
	}

	public void DisableParticles()
	{
		if ((bool)defaultParticles)
		{
			defaultParticles.SetActive(value: false);
		}
		GameObject[] array = mapZoneParticles;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: false);
			}
		}
		array = customParticles;
		foreach (GameObject gameObject2 in array)
		{
			if ((bool)gameObject2)
			{
				gameObject2.SetActive(value: false);
			}
		}
		act3Particles.SetActive(value: false);
	}

	public void SceneInit()
	{
		DisableParticles();
	}

	private void OnPositionedAtHero()
	{
		gm = GameManager.instance;
		sm = gm.sm;
		if (sm == null)
		{
			sm = Object.FindObjectOfType<CustomSceneManager>();
		}
		if (gm.IsGameplayScene() && !gm.IsCinematicScene())
		{
			EnableParticles(sm.noParticles);
		}
		else
		{
			DisableParticles();
		}
	}
}
