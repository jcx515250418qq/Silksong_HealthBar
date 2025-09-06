using System.Collections;
using UnityEngine;

public class BossStatueDreamToggle : MonoBehaviour, IBossStatueToggle
{
	public GameObject litPieces;

	public ParticleSystem[] particles;

	public GameObject dreamImpactPrefab;

	public Vector3 dreamImpactScale = new Vector3(4f, 4f, 1f);

	public Transform dreamImpactPoint;

	private bool canToggle = true;

	private ColorFader[] colorFaders;

	private int waitingForFade;

	public GameObject dreamBurstEffectPrefab;

	public GameObject dreamBurstEffectOffPrefab;

	public Transform dreamBurstSpawnPoint;

	private GameObject dreamBurstEffect;

	private GameObject dreamBurstEffectOff;

	private BossStatue bossStatue;

	private void OnEnable()
	{
		if ((bool)bossStatue && !bossStatue.UsingDreamVersion)
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	private void Start()
	{
		if ((bool)litPieces)
		{
			litPieces.SetActive(value: true);
			if (!bossStatue || !bossStatue.UsingDreamVersion)
			{
				litPieces.SetActive(value: false);
			}
			colorFaders = litPieces.GetComponentsInChildren<ColorFader>(includeInactive: true);
			ColorFader[] array = colorFaders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnFadeEnd += delegate(bool up)
				{
					if (!up)
					{
						waitingForFade--;
					}
				};
			}
		}
		if ((bool)dreamBurstEffectPrefab)
		{
			dreamBurstEffect = Object.Instantiate(dreamBurstEffectPrefab, dreamBurstSpawnPoint);
			dreamBurstEffect.transform.localPosition = Vector3.zero;
			dreamBurstEffect.SetActive(value: false);
		}
		if ((bool)dreamBurstEffectOffPrefab)
		{
			dreamBurstEffectOff = Object.Instantiate(dreamBurstEffectOffPrefab, dreamBurstSpawnPoint);
			dreamBurstEffectOff.transform.localPosition = Vector3.zero;
			dreamBurstEffectOff.SetActive(value: false);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.gameObject.activeInHierarchy && canToggle && (bool)bossStatue && collision.tag == "Dream Attack")
		{
			bool flag = !bossStatue.UsingDreamVersion;
			bossStatue.SetDreamVersion(flag);
			if ((bool)dreamImpactPoint && (bool)dreamImpactPrefab)
			{
				dreamImpactPrefab.Spawn(dreamImpactPoint.position).transform.localScale = dreamImpactScale;
			}
			if ((bool)dreamBurstEffect)
			{
				dreamBurstEffect.SetActive(flag);
			}
			if ((bool)dreamBurstEffectOff)
			{
				dreamBurstEffectOff.SetActive(!flag);
			}
			StartCoroutine(Fade(bossStatue.UsingDreamVersion));
		}
	}

	private IEnumerator Fade(bool usingDreamVersion)
	{
		if (usingDreamVersion)
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
		}
		else
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
		if ((bool)litPieces)
		{
			litPieces.SetActive(value: true);
		}
		ColorFader[] array2 = colorFaders;
		foreach (ColorFader obj in array2)
		{
			if (!usingDreamVersion)
			{
				waitingForFade++;
			}
			obj.Fade(usingDreamVersion);
		}
		if (!usingDreamVersion)
		{
			while (waitingForFade > 0)
			{
				yield return null;
			}
			if ((bool)litPieces)
			{
				litPieces.SetActive(value: false);
			}
		}
	}

	public void SetOwner(BossStatue statue)
	{
		bossStatue = statue;
		if (!bossStatue.UsingDreamVersion)
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			return;
		}
		if ((bool)litPieces)
		{
			litPieces.SetActive(value: true);
		}
		ColorFader[] array2 = colorFaders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Fade(up: true);
		}
	}

	public void SetState(bool value)
	{
		canToggle = value;
		if (!value)
		{
			base.gameObject.SetActive(canToggle);
		}
	}
}
