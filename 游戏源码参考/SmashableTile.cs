using TeamCherry.SharedUtils;
using UnityEngine;

public class SmashableTile : MonoBehaviour
{
	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private PlayerDataTest startSmashedCondition;

	[Space]
	[SerializeField]
	[Range(0f, 1f)]
	private float smashChance = 1f;

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private Sprite[] smashedSprites;

	[SerializeField]
	private GameObject activateOnSmash;

	[Space]
	[SerializeField]
	private MinMaxFloat breakRotation;

	[SerializeField]
	private MinMaxFloat preventRotationZRange;

	private bool isSmashed;

	private void Awake()
	{
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = isSmashed;
			};
			persistent.OnSetSaveState += delegate(bool value)
			{
				isSmashed = value;
				if (isSmashed)
				{
					SetSmashed();
				}
			};
		}
		if ((bool)sprite && smashedSprites.Length == 0)
		{
			sprite.enabled = false;
		}
		if ((bool)activateOnSmash)
		{
			activateOnSmash.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (startSmashedCondition.IsDefined && startSmashedCondition.IsFulfilled)
		{
			SetSmashed();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!isSmashed && collision.CompareTag("Tile Smasher") && !(Random.Range(0f, 1f) > smashChance))
		{
			SetSmashed();
			if ((bool)activateOnSmash)
			{
				activateOnSmash.SetActive(value: true);
			}
		}
	}

	private void SetSmashed()
	{
		isSmashed = true;
		if (!sprite)
		{
			return;
		}
		sprite.enabled = true;
		if (smashedSprites.Length != 0)
		{
			sprite.sprite = smashedSprites[Random.Range(0, smashedSprites.Length)];
			float z = base.transform.position.z;
			if (z < preventRotationZRange.Start || z > preventRotationZRange.End)
			{
				Vector3 localEulerAngles = base.transform.localEulerAngles;
				localEulerAngles.z += breakRotation.GetRandomValue();
				base.transform.localEulerAngles = localEulerAngles;
			}
		}
	}
}
