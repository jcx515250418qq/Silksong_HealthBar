using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GlobControl : MonoBehaviour
{
	public Renderer rend;

	[Space]
	public float minScale = 0.6f;

	public float maxScale = 1.6f;

	[Space]
	public string landAnim = "Glob Land";

	public string wobbleAnim = "Glob Wobble";

	public string breakAnim = "Glob Break";

	[Space]
	public AudioSource audioPlayerPrefab;

	public AudioEvent breakSound;

	public AudioEvent landSound;

	public AudioEvent wobbleSound;

	public Color bloodColorOverride = new Color(1f, 0.537f, 0.188f);

	[Space]
	public GameObject splatChild;

	[Space]
	public Collider2D groundCollider;

	private bool landed;

	private bool broken;

	private tk2dSpriteAnimator anim;

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	private void OnEnable()
	{
		float num = Random.Range(minScale, maxScale);
		base.transform.localScale = new Vector3(num, num, 1f);
		if ((bool)splatChild)
		{
			splatChild.SetActive(value: false);
		}
		landed = false;
		broken = false;
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
						if (CheckForGround())
						{
							anim.Play(landAnim);
							landSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
						}
						else
						{
							StartCoroutine(Break());
						}
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
				wobbleSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			}
		};
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (landed && !broken)
		{
			if (col.tag == "Nail Attack")
			{
				StartCoroutine(Break());
			}
			else if (col.tag == "HeroBox")
			{
				wobbleSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
				anim.Play(wobbleAnim);
			}
		}
	}

	private IEnumerator Break()
	{
		broken = true;
		breakSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		BloodSpawner.SpawnBlood(base.transform.position, 4, 5, 5f, 20f, 80f, 100f, bloodColorOverride);
		if ((bool)splatChild)
		{
			splatChild.SetActive(value: true);
		}
		yield return anim.PlayAnimWait(breakAnim);
		if ((bool)rend)
		{
			rend.enabled = false;
		}
	}

	private bool CheckForGround()
	{
		if (!groundCollider)
		{
			return true;
		}
		Vector2 origin = groundCollider.bounds.min;
		Vector2 origin2 = groundCollider.bounds.max;
		float num = origin2.y - origin.y;
		origin.y = origin2.y;
		origin.x += 0.1f;
		origin2.x -= 0.1f;
		RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, Vector2.down, num + 0.25f, 256);
		RaycastHit2D raycastHit2D2 = Helper.Raycast2D(origin2, Vector2.down, num + 0.25f, 256);
		if (raycastHit2D.collider != null)
		{
			return raycastHit2D2.collider != null;
		}
		return false;
	}
}
