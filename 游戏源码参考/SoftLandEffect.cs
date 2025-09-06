using GlobalEnums;
using UnityEngine;

public class SoftLandEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject dustEffects;

	[SerializeField]
	private GameObject grassEffects;

	[SerializeField]
	private GameObject boneEffects;

	[SerializeField]
	private GameObject peakPuffEffects;

	[SerializeField]
	private SpriteRenderer splash;

	[SerializeField]
	private AudioClip softLandClip;

	[SerializeField]
	private AudioClip wetLandClip;

	[SerializeField]
	private AudioClip boneLandClip;

	[SerializeField]
	private AudioClip woodLandClip;

	[SerializeField]
	private AudioClip metalLandClip;

	[SerializeField]
	private AudioClip bellLandClip;

	[SerializeField]
	private AudioClip grassLandClip;

	[SerializeField]
	private AudioClip sandLandClip;

	[SerializeField]
	private AudioClip peakPuffClip;

	private GameObject heroObject;

	private AudioSource audioSource;

	private Rigidbody2D heroRigidBody;

	private tk2dSpriteAnimator jumpPuffAnimator;

	private float recycleTimer;

	private void OnEnable()
	{
		PlayerData instance = PlayerData.instance;
		if (audioSource == null)
		{
			audioSource = base.gameObject.GetComponent<AudioSource>();
		}
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
		recycleTimer = 2f;
		HeroController silentInstance = HeroController.SilentInstance;
		if (!(silentInstance == null) && silentInstance.isHeroInPosition)
		{
			switch (instance.environmentType)
			{
			case EnvironmentTypes.Grass:
				grassEffects.SetActive(value: true);
				audioSource.PlayOneShot(grassLandClip);
				break;
			case EnvironmentTypes.Bone:
				boneEffects.SetActive(value: true);
				audioSource.PlayOneShot(boneLandClip);
				break;
			case EnvironmentTypes.ShallowWater:
				audioSource.PlayOneShot(wetLandClip);
				break;
			case EnvironmentTypes.Wood:
				audioSource.PlayOneShot(woodLandClip);
				break;
			case EnvironmentTypes.Metal:
				audioSource.PlayOneShot(metalLandClip);
				break;
			case EnvironmentTypes.ThinMetal:
				audioSource.PlayOneShot(metalLandClip);
				break;
			case EnvironmentTypes.PeakPuff:
				peakPuffEffects.SetActive(value: true);
				audioSource.PlayOneShot(peakPuffClip);
				break;
			case EnvironmentTypes.Moss:
			case EnvironmentTypes.RunningWater:
				PlaySplash();
				break;
			case EnvironmentTypes.WetWood:
				audioSource.PlayOneShot(woodLandClip);
				PlaySplash();
				break;
			case EnvironmentTypes.WetMetal:
				audioSource.PlayOneShot(metalLandClip);
				PlaySplash();
				break;
			case EnvironmentTypes.Bell:
				dustEffects.SetActive(value: true);
				audioSource.PlayOneShot(bellLandClip);
				break;
			case EnvironmentTypes.Sand:
				dustEffects.SetActive(value: true);
				audioSource.PlayOneShot(sandLandClip);
				break;
			case EnvironmentTypes.FlowerField:
				audioSource.PlayOneShot(grassLandClip);
				break;
			default:
				dustEffects.SetActive(value: true);
				audioSource.PlayOneShot(softLandClip);
				break;
			case EnvironmentTypes.NoEffect:
				break;
			}
		}
	}

	private void Update()
	{
		if (recycleTimer <= 0f)
		{
			base.gameObject.Recycle();
		}
		else
		{
			recycleTimer -= Time.deltaTime;
		}
	}

	private void PlaySplash()
	{
		audioSource.PlayOneShot(wetLandClip);
		splash.color = (AreaEffectTint.IsActive(base.transform.position, out var tintColor) ? tintColor : Color.white);
		splash.gameObject.SetActive(value: true);
		if (Random.Range(1, 100) > 50)
		{
			Transform obj = splash.transform;
			Vector3 localScale = obj.localScale;
			obj.localScale = new Vector3(0f - localScale.x, localScale.y, localScale.z);
		}
	}
}
