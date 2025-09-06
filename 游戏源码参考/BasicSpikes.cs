using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BasicSpikes : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string activeBool;

	private int activeBoolHash;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string shineTrigger;

	private int shineTriggerHash;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string hitTrigger;

	private int hitTriggerHash;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string idleOffsetFloat;

	private int idleOffsetFloatHash;

	[Space]
	[SerializeField]
	private TrackTriggerObjects activateTrigger;

	[SerializeField]
	private MinMaxFloat stayOutDuration;

	[SerializeField]
	private MinMaxFloat activateDelay;

	[SerializeField]
	private MinMaxFloat shineDelay;

	[Space]
	[SerializeField]
	private DamageHero damager;

	private int damageAmount;

	[Space]
	[SerializeField]
	private GameObject[] hitSpawnPrefabs;

	private bool isOut;

	private bool didHit;

	private bool? IsAnimatorParamValid(string name)
	{
		if (!animator || string.IsNullOrEmpty(name))
		{
			return null;
		}
		return animator.HasParameter(name, null);
	}

	private void OnValidate()
	{
		activeBoolHash = Animator.StringToHash(activeBool);
		shineTriggerHash = Animator.StringToHash(shineTrigger);
		hitTriggerHash = Animator.StringToHash(hitTrigger);
		idleOffsetFloatHash = Animator.StringToHash(idleOffsetFloat);
	}

	private void Awake()
	{
		OnValidate();
		if ((bool)damager)
		{
			damageAmount = damager.damageDealt;
		}
	}

	private void OnEnable()
	{
		if ((bool)activateTrigger && (bool)animator)
		{
			StartCoroutine(Behaviour());
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator Behaviour()
	{
		DisableSpikeDamager();
		while (true)
		{
			animator.SetFloatIfExists(idleOffsetFloatHash, Random.Range(0f, 1f));
			animator.SetBoolIfExists(activeBoolHash, value: false);
			isOut = false;
			yield return new WaitUntil(() => isOut || activateTrigger.InsideCount > 0);
			yield return new WaitForSecondsInterruptable(activateDelay.GetRandomValue(), () => isOut);
			animator.SetBoolIfExists(activeBoolHash, value: true);
			isOut = true;
			float shineDelay = this.shineDelay.GetRandomValue();
			float shineElapsed = 0f;
			float stayOutDuration = this.stayOutDuration.GetRandomValue();
			float noneInsideElapsed = 0f;
			while (noneInsideElapsed < stayOutDuration)
			{
				if (didHit || activateTrigger.InsideCount > 0)
				{
					noneInsideElapsed = 0f;
				}
				if (shineElapsed > shineDelay)
				{
					shineElapsed = 0f;
					if (!didHit)
					{
						animator.SetTriggerIfExists(shineTriggerHash);
					}
				}
				didHit = false;
				yield return null;
				noneInsideElapsed += Time.deltaTime;
				shineElapsed += Time.deltaTime;
			}
		}
	}

	public void EnableSpikeDamager()
	{
		if ((bool)damager)
		{
			damager.damageDealt = damageAmount;
		}
	}

	public void DisableSpikeDamager()
	{
		if ((bool)damager)
		{
			damager.damageDealt = 0;
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsNailDamage || damageInstance.DamageDealt <= 0)
		{
			return IHitResponder.Response.None;
		}
		if (isOut)
		{
			animator.SetTriggerIfExists(hitTriggerHash);
			didHit = true;
			GameObject[] array = hitSpawnPrefabs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Spawn(base.transform.position);
			}
		}
		else
		{
			isOut = true;
		}
		return IHitResponder.Response.GenericHit;
	}
}
