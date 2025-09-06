using UnityEngine;

public class CollectableItemHeroReaction : MonoBehaviour
{
	private static CollectableItemHeroReaction _instance;

	[SerializeField]
	private GameObject itemGetEffectPrefab;

	[SerializeField]
	private GameObject itemGetEffectPrefabSmall;

	[SerializeField]
	private Vector2 itemGetEffectOffset;

	[SerializeField]
	private AudioEvent itemGetSound;

	private SpriteFlash spriteFlash;

	public static Vector2 NextEffectOffset { get; set; }

	private void Awake()
	{
		spriteFlash = GetComponent<SpriteFlash>();
	}

	private void Start()
	{
		_instance = this;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public static void DoReaction()
	{
		if ((bool)_instance)
		{
			_instance.InternalDoReaction(NextEffectOffset, smallEffect: false);
			NextEffectOffset = Vector2.zero;
		}
	}

	public static void DoReaction(Vector2 effectOffset, bool smallEffect = false)
	{
		if ((bool)_instance)
		{
			_instance.InternalDoReaction(effectOffset, smallEffect);
		}
	}

	private void InternalDoReaction(Vector2? effectOffset, bool smallEffect)
	{
		if ((bool)spriteFlash)
		{
			spriteFlash.flashFocusHeal();
		}
		Vector2 vector = base.transform.position;
		if (effectOffset.HasValue)
		{
			vector += effectOffset.Value;
		}
		else
		{
			vector += itemGetEffectOffset;
		}
		GameObject gameObject = (smallEffect ? itemGetEffectPrefabSmall : itemGetEffectPrefab);
		if ((bool)gameObject)
		{
			gameObject.Spawn(vector.ToVector3(gameObject.transform.position.z)).transform.SetRotation2D(Random.Range(0f, 360f));
		}
		itemGetSound.SpawnAndPlayOneShot(vector);
	}
}
