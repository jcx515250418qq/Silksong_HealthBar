using System.Collections;
using UnityEngine;

public class BossStatueFlashEffect : MonoBehaviour
{
	public delegate void FlashCompleteDelegate();

	public SpriteRenderer templateSprite;

	public GameObject statueSpritesParent;

	private SpriteRenderer[] statueSprites;

	public GameObject inspect;

	public TriggerEnterEvent triggerEvent;

	private BossStatue parentStatue;

	private Animator animator;

	private Material mat;

	private MaterialPropertyBlock propBlock;

	public event FlashCompleteDelegate OnFlashBegin;

	private void Awake()
	{
		parentStatue = GetComponentInParent<BossStatue>();
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		if ((bool)templateSprite)
		{
			templateSprite.transform.localPosition += new Vector3(0f, -2000f, 0f);
			mat = new Material(templateSprite.sharedMaterial);
		}
		if (!parentStatue.StatueState.hasBeenSeen && !parentStatue.isAlwaysUnlocked)
		{
			if ((bool)statueSpritesParent)
			{
				statueSprites = statueSpritesParent.GetComponentsInChildren<SpriteRenderer>();
				SpriteRenderer[] array = statueSprites;
				foreach (SpriteRenderer obj in array)
				{
					obj.color = Color.clear;
					obj.sharedMaterial = mat;
				}
			}
			if ((bool)triggerEvent)
			{
				TriggerEnterEvent.CollisionEvent temp = null;
				temp = delegate
				{
					base.gameObject.SetActive(value: true);
					statueSpritesParent.SetActive(value: true);
					if ((bool)inspect)
					{
						inspect.SetActive(value: false);
					}
					StartCoroutine(FlashRoutine());
					triggerEvent.OnTriggerEntered -= temp;
				};
				triggerEvent.OnTriggerEntered += temp;
			}
		}
		propBlock = new MaterialPropertyBlock();
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		if (mat != null)
		{
			Object.Destroy(mat);
			mat = null;
		}
	}

	private IEnumerator FlashRoutine()
	{
		if (this.OnFlashBegin != null)
		{
			this.OnFlashBegin();
		}
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		float duration = animator.GetCurrentAnimatorStateInfo(0).length;
		for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
		{
			SpriteRenderer[] array = statueSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = templateSprite.color;
			}
			templateSprite.GetPropertyBlock(propBlock);
			mat.SetFloat("_FlashAmount", propBlock.GetFloat("_FlashAmount"));
			yield return null;
		}
		yield return null;
		animator.enabled = false;
		SpriteFadeMaterial component = statueSpritesParent.GetComponent<SpriteFadeMaterial>();
		if ((bool)component)
		{
			component.FadeBack();
		}
	}

	public void FlashApex()
	{
		if ((bool)inspect)
		{
			inspect.SetActive(value: true);
		}
		parentStatue.SetPlaquesVisible(isEnabled: true);
	}
}
