using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class DamageEffectTicker : MonoBehaviour
{
	private enum SpriteFlashMethods
	{
		None = 0,
		Curse = 1,
		Dazzle = 2,
		Coal = 3
	}

	[SerializeField]
	private float damageInterval = 0.2f;

	[SerializeField]
	private int damageAmount = 1;

	[SerializeField]
	private SpriteFlashMethods enemySpriteFlash;

	[SerializeField]
	private float damageStopTime;

	private float timeAlive;

	private float timer;

	private List<GameObject> enemyList = new List<GameObject>();

	private void OnEnable()
	{
		enemyList.Clear();
		timeAlive = 0f;
	}

	private void Update()
	{
		timeAlive += Time.deltaTime;
		if (damageStopTime > 0f && timeAlive >= damageStopTime)
		{
			return;
		}
		timer += Time.deltaTime;
		if (!(timer >= damageInterval))
		{
			return;
		}
		for (int i = 0; i < enemyList.Count; i++)
		{
			GameObject gameObject = enemyList[i];
			if (!(gameObject == null))
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if ((bool)component)
				{
					component.ApplyExtraDamage(damageAmount);
					DoFlashOnEnemy(gameObject);
				}
			}
		}
		timer -= damageInterval;
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (enemySpriteFlash != SpriteFlashMethods.Coal || !otherCollider.GetComponent<HealthManager>().ImmuneToCoal)
		{
			enemyList.AddIfNotPresent(otherCollider.gameObject);
		}
	}

	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		enemyList.Remove(otherCollider.gameObject);
	}

	public void EmptyDamageList()
	{
		enemyList.Clear();
	}

	public void SetDamageInterval(float newInterval)
	{
		damageInterval = newInterval;
	}

	private void DoFlashOnEnemy(GameObject enemy)
	{
		Effects.EnemyDamageTickSoundTable.SpawnAndPlayOneShot(enemy.transform.position);
		if (enemySpriteFlash == SpriteFlashMethods.None)
		{
			return;
		}
		SpriteFlash component = enemy.GetComponent<SpriteFlash>();
		if ((bool)component)
		{
			switch (enemySpriteFlash)
			{
			case SpriteFlashMethods.Curse:
				component.FlashWitchPoison();
				break;
			case SpriteFlashMethods.Dazzle:
				component.FlashDazzleQuick();
				break;
			case SpriteFlashMethods.Coal:
				component.FlashCoal();
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}
}
