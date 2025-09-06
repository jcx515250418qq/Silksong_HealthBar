using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LifebloodGlob : MonoBehaviour
{
	public Renderer rend;

	public float recycleTime;

	[Space]
	public float minScale = 0.6f;

	public float maxScale = 1.6f;

	[Space]
	public string fallAnim = "Glob Fall";

	public string landAnim = "Glob Land";

	public string wobbleAnim = "Glob Wobble";

	public string breakAnim = "Glob Break";

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent breakSound;

	public Color bloodColorOverride = new Color(1f, 0.537f, 0.188f);

	[Space]
	public GameObject splatChild;

	[Space]
	public Collider2D groundCollider;

	[Space]
	public GameObject pickupPlink;

	public CollectableItem pickup;

	private bool landed;

	private bool broken;

	private bool isPlinkActive;

	private tk2dSpriteAnimator anim;

	public event Action PickedUp;

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	private void OnEnable()
	{
		float num = UnityEngine.Random.Range(minScale, maxScale);
		base.transform.localScale = new Vector3(num, num, 1f);
		if ((bool)splatChild)
		{
			splatChild.SetActive(value: false);
		}
		if ((bool)rend)
		{
			rend.enabled = true;
		}
		anim.Play(fallAnim);
		landed = false;
		broken = false;
		ShowPlink(value: false);
	}

	private void Start()
	{
		CollisionEnterEvent collision = GetComponent<CollisionEnterEvent>();
		if ((bool)collision)
		{
			collision.CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D col)
			{
				if (!landed)
				{
					if (direction == CollisionEnterEvent.Direction.Bottom)
					{
						landed = true;
						collision.DoCollisionStay = false;
						anim.Play(landAnim);
					}
					else
					{
						collision.DoCollisionStay = true;
					}
				}
			};
		}
		TriggerEnterEvent componentInChildren = GetComponentInChildren<TriggerEnterEvent>();
		if (!componentInChildren)
		{
			return;
		}
		componentInChildren.OnTriggerEntered += delegate(Collider2D col, GameObject sender)
		{
			if (landed && !broken && col.gameObject.layer == 11)
			{
				anim.Play(wobbleAnim);
			}
		};
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!landed || broken)
		{
			return;
		}
		if (col.tag == "Nail Attack")
		{
			DoBreak();
		}
		else if (col.tag == "HeroBox")
		{
			if (isPlinkActive)
			{
				DoBreak();
			}
			else
			{
				anim.Play(wobbleAnim);
			}
		}
	}

	private IEnumerator Break()
	{
		broken = true;
		breakSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		BloodSpawner.SpawnBlood(base.transform.position, 4, 5, 5f, 20f, 80f, 100f, bloodColorOverride);
		ShowPlink(value: false);
		if ((bool)splatChild)
		{
			splatChild.SetActive(value: true);
		}
		yield return anim.PlayAnimWait(breakAnim);
		if ((bool)rend)
		{
			rend.enabled = false;
		}
		if (recycleTime > 0f)
		{
			yield return new WaitForSeconds(recycleTime);
		}
		base.gameObject.Recycle();
	}

	private void DoBreak()
	{
		if (!broken)
		{
			if (isPlinkActive && this.PickedUp != null)
			{
				this.PickedUp();
			}
			StartCoroutine(Break());
		}
	}

	public void ShowPlink(bool value)
	{
		if ((bool)pickupPlink)
		{
			pickupPlink.SetActive(value);
		}
		isPlinkActive = value;
	}

	public void SetTempQuestHandler(Action questHandler)
	{
		ShowPlink(IsQuestActive());
		if (isPlinkActive)
		{
			Action temp = null;
			temp = delegate
			{
				if ((bool)pickup)
				{
					pickup.Get();
				}
				if (questHandler != null)
				{
					questHandler();
				}
				PickedUp -= temp;
			};
			PickedUp += temp;
		}
		else if (questHandler != null)
		{
			questHandler();
		}
	}

	private bool IsQuestActive()
	{
		Quest lifeBloodQuest = Effects.LifeBloodQuest;
		if ((bool)lifeBloodQuest && lifeBloodQuest.IsAccepted)
		{
			return !lifeBloodQuest.CanComplete;
		}
		return false;
	}
}
