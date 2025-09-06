using GlobalEnums;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
	public GameObject heroDashPuff;

	public GameObject dashDust;

	public GameObject dashBone;

	public GameObject dashGrass;

	public GameObject waterCut;

	public tk2dSpriteAnimator heroDashPuff_anim;

	public AudioClip splashClip;

	private GameManager gm;

	private GameObject heroObject;

	private AudioSource audioSource;

	private Rigidbody2D heroRigidBody;

	private tk2dSpriteAnimator jumpPuffAnimator;

	private float recycleTimer;

	private void Awake()
	{
		gm = GameManager.instance;
		audioSource = base.gameObject.GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}

	public void Play(GameObject owner)
	{
		EnviroRegionListener component = GetComponent<EnviroRegionListener>();
		EnvironmentTypes environmentTypes = (component ? component.CurrentEnvironmentType : gm.sm.environmentType);
		recycleTimer = 2f;
		switch (environmentTypes)
		{
		case EnvironmentTypes.Grass:
			PlayDashPuff();
			PlayGrass();
			break;
		case EnvironmentTypes.Bone:
			PlayDashPuff();
			PlayBone();
			break;
		case EnvironmentTypes.ShallowWater:
			PlaySpaEffects();
			break;
		default:
			PlayDashPuff();
			PlayDust();
			break;
		case EnvironmentTypes.Moss:
			break;
		}
	}

	private void PlayDashPuff()
	{
		heroDashPuff.SetActive(value: true);
		heroDashPuff_anim.PlayFromFrame(0);
	}

	private void PlayDust()
	{
		dashDust.SetActive(value: true);
	}

	private void PlayGrass()
	{
		dashGrass.SetActive(value: true);
	}

	private void PlayBone()
	{
		dashBone.SetActive(value: true);
	}

	private void PlaySpaEffects()
	{
		waterCut.SetActive(value: true);
		audioSource.PlayOneShot(splashClip);
	}

	private void Update()
	{
		if (recycleTimer > 0f)
		{
			recycleTimer -= Time.deltaTime;
			if (recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
			}
		}
	}
}
