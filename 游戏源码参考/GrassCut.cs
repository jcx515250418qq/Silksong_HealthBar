using UnityEngine;
using UnityEngine.Events;

public class GrassCut : MonoBehaviour, IBreakerBreakable
{
	[SerializeField]
	private SpriteRenderer[] disable;

	[SerializeField]
	private SpriteRenderer[] enable;

	[Space]
	[SerializeField]
	private Collider2D[] disableColliders;

	[SerializeField]
	private Collider2D[] enableColliders;

	[SerializeField]
	private GameObject[] enableGameObjects;

	[Space]
	[SerializeField]
	private GameObject particles;

	[SerializeField]
	private GameObject cutEffectPrefab;

	[Space]
	[SerializeField]
	private bool callOnChildren;

	[SerializeField]
	private PersistentBoolItem persistent;

	private bool isCut;

	private Collider2D col;

	private GrassCut[] children;

	public UnityEvent OnCut;

	public BreakableBreaker.BreakableTypes BreakableType => BreakableBreaker.BreakableTypes.Grass;

	GameObject IBreakerBreakable.gameObject => base.gameObject;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
		children = GetComponentsInChildren<GrassCut>();
		if (!(persistent != null))
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out bool val)
		{
			val = isCut;
		};
		persistent.OnSetSaveState += delegate(bool val)
		{
			isCut = val;
			if (isCut)
			{
				base.gameObject.SetActive(value: false);
			}
		};
	}

	private void Start()
	{
		enableGameObjects.SetAllActive(value: false);
		SpriteRenderer[] array = enable;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if ((bool)spriteRenderer)
			{
				spriteRenderer.enabled = false;
			}
		}
		Collider2D[] array2 = enableColliders;
		foreach (Collider2D collider2D in array2)
		{
			if ((bool)collider2D)
			{
				collider2D.enabled = false;
			}
		}
		if ((bool)particles)
		{
			particles.SetActive(value: false);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (ShouldCut(collision))
		{
			DoCut(collision);
		}
	}

	private void DoCut(Collider2D collision)
	{
		if (callOnChildren)
		{
			GrassCut[] array = children;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Cut(collision);
			}
		}
		else
		{
			Cut(collision);
		}
		isCut = true;
		if (OnCut != null)
		{
			OnCut.Invoke();
		}
	}

	public static bool ShouldCut(Collider2D collision)
	{
		NailAttackBase component = collision.GetComponent<NailAttackBase>();
		if ((bool)component && !component.CanHitSpikes)
		{
			return false;
		}
		if (collision.CompareTag("Sharp Shadow"))
		{
			return true;
		}
		DamageEnemies component2 = collision.GetComponent<DamageEnemies>();
		if (!component2 || (component2.damageDealt <= 0 && !component2.useNailDamage) || component2.OnlyDamageEnemies)
		{
			if (collision.CompareTag("HeroBox"))
			{
				return HeroController.instance.cState.superDashing;
			}
			return false;
		}
		return true;
	}

	public void Cut(Collider2D collision)
	{
		GrassBehaviour componentInParent = GetComponentInParent<GrassBehaviour>();
		SpriteRenderer[] array = disable;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if (!(spriteRenderer == null))
			{
				spriteRenderer.enabled = false;
			}
		}
		array = enable;
		foreach (SpriteRenderer spriteRenderer2 in array)
		{
			if (!(spriteRenderer2 == null))
			{
				spriteRenderer2.enabled = true;
			}
		}
		Collider2D[] array2 = disableColliders;
		foreach (Collider2D collider2D in array2)
		{
			if (!(collider2D == null))
			{
				collider2D.enabled = false;
			}
		}
		array2 = enableColliders;
		foreach (Collider2D collider2D2 in array2)
		{
			if (!(collider2D2 == null))
			{
				collider2D2.enabled = true;
			}
		}
		GameObject[] array3 = enableGameObjects;
		foreach (GameObject gameObject in array3)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(value: true);
			}
		}
		if ((bool)particles)
		{
			particles.SetActive(value: true);
		}
		if ((bool)componentInParent)
		{
			componentInParent.CutReact(collision);
		}
		if ((bool)cutEffectPrefab)
		{
			Vector3 position;
			float num;
			if ((bool)collision)
			{
				position = (collision.bounds.center + col.bounds.center) / 2f;
				num = Mathf.Sign(base.transform.position.x - collision.transform.position.x);
			}
			else
			{
				position = col.bounds.center;
				num = 1f;
			}
			cutEffectPrefab.Spawn(position);
			cutEffectPrefab.transform.SetScaleX((0f - num) * 0.6f);
			cutEffectPrefab.transform.SetScaleY(1f);
		}
		Object.Destroy(this);
	}

	public void BreakSelf()
	{
		DoCut(null);
	}

	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		DoCut(null);
	}

	public void HitFromBreaker(BreakableBreaker breaker)
	{
		DoCut(null);
	}
}
