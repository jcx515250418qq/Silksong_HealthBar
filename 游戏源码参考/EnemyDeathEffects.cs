using System;
using System.Collections.Generic;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class EnemyDeathEffects : MonoBehaviour, IInitialisable, BlackThreadState.IBlackThreadStateReceiver
{
	[Serializable]
	private struct AltCorpse
	{
		[EnumPickerBitmask(typeof(AttackTypes))]
		public int AttackTypeMask;

		public GameObject Prefab;
	}

	[SerializeField]
	private GameObject corpsePrefab;

	[SerializeField]
	private AltCorpse[] altCorpses;

	[SerializeField]
	private bool isCorpseRecyclable;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("isCorpseRecyclable", false, false, false)]
	private bool manualPreInstantiate;

	[SerializeField]
	private bool corpseFacesRight;

	[SerializeField]
	private bool overrideDeathDirection;

	[SerializeField]
	private float corpseFlingSpeed;

	[SerializeField]
	public Vector3 corpseSpawnPoint;

	[SerializeField]
	public Vector3 effectOrigin;

	[SerializeField]
	private bool lowCorpseArc;

	[SerializeField]
	private bool recycle;

	[SerializeField]
	private bool rotateCorpse;

	[SerializeField]
	private bool corpseMatchesEnemyScale;

	[SerializeField]
	private AudioMixerSnapshot audioSnapshotOnDeath;

	[SerializeField]
	[FormerlySerializedAs("deathBroadcastEvent")]
	private string sendEventRegister;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private List<AudioEvent> deathSounds = new List<AudioEvent>();

	[SerializeField]
	[HideInInspector]
	private AudioEvent enemyDeathSwordAudio;

	[SerializeField]
	[HideInInspector]
	private AudioEvent enemyDamageAudio;

	[SerializeField]
	private CameraShakeTarget deathCameraShake;

	[SerializeField]
	private EnemyJournalRecord journalRecord;

	[SerializeField]
	private bool awardFullJournalEntry;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	public string setPlayerDataBool;

	[SerializeField]
	private string awardAchievement;

	public bool doNotSetHasKilled;

	public bool doNotSpawnCorpse;

	private bool didFire;

	private GameObject[] instantiatedCorpses;

	private bool preInstantiated;

	private bool hasAwaken;

	private bool hasStarted;

	private bool disabledSpecialQuestDrops;

	private bool isBlackThreaded;

	protected virtual Color? OverrideBloodColor => null;

	private bool IsVisible
	{
		get
		{
			Renderer renderer = GetComponent<Renderer>();
			if (renderer == null)
			{
				renderer = GetComponentInChildren<Renderer>();
			}
			if (renderer != null)
			{
				return renderer.isVisible;
			}
			return false;
		}
	}

	public bool SkipKillFreeze { get; set; }

	public GameObject CorpsePrefab
	{
		get
		{
			return corpsePrefab;
		}
		set
		{
			if (!(corpsePrefab != value))
			{
				return;
			}
			corpsePrefab = value;
			if (!preInstantiated)
			{
				return;
			}
			preInstantiated = false;
			if (instantiatedCorpses != null)
			{
				GameObject[] array = instantiatedCorpses;
				for (int i = 0; i < array.Length; i++)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
				instantiatedCorpses = null;
			}
			PreInstantiate();
		}
	}

	protected bool IsCorpseRecyclable => isCorpseRecyclable;

	GameObject IInitialisable.gameObject => base.gameObject;

	public event Action<GameObject> CorpseEmitted;

	private void OnValidate()
	{
		if ((bool)enemyDeathSwordAudio.Clip)
		{
			deathSounds.Add(enemyDeathSwordAudio);
			enemyDeathSwordAudio = default(AudioEvent);
		}
		if ((bool)enemyDamageAudio.Clip)
		{
			deathSounds.Add(enemyDamageAudio);
			enemyDamageAudio = default(AudioEvent);
		}
	}

	public virtual bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		OnValidate();
		if (!manualPreInstantiate)
		{
			PreInstantiate();
		}
		if (isCorpseRecyclable && (bool)corpsePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, corpsePrefab, 1, finished: true, initialiseSpawned: true);
		}
		return true;
	}

	public virtual bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	protected void Awake()
	{
		OnAwake();
	}

	public void PreInstantiate()
	{
		if (isCorpseRecyclable || preInstantiated)
		{
			return;
		}
		preInstantiated = true;
		if (instantiatedCorpses != null || (!corpsePrefab && altCorpses.Length == 0))
		{
			return;
		}
		Transform transform = base.transform;
		instantiatedCorpses = new GameObject[altCorpses.Length + 1];
		for (int i = 0; i < instantiatedCorpses.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate((i == 0) ? corpsePrefab : altCorpses[i - 1].Prefab, transform.TransformPoint(corpseSpawnPoint), Quaternion.identity, transform);
			tk2dSprite[] componentsInChildren = gameObject.GetComponentsInChildren<tk2dSprite>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].ForceBuild();
			}
			IInitialisable.DoFullInit(gameObject);
			gameObject.SetActive(value: false);
			instantiatedCorpses[i] = gameObject;
			if (disabledSpecialQuestDrops)
			{
				RemoveQuestDropsFromGameObject(gameObject);
			}
		}
		disabledSpecialQuestDrops = false;
	}

	private GameObject GetInstantiatedCorpse(AttackTypes attackType)
	{
		if (instantiatedCorpses == null)
		{
			return null;
		}
		for (int i = 0; i < altCorpses.Length; i++)
		{
			if (altCorpses[i].AttackTypeMask.IsBitSet((int)attackType))
			{
				return instantiatedCorpses[i + 1];
			}
		}
		return instantiatedCorpses[0];
	}

	public void DisableSpecialQuestDrops()
	{
		if (isCorpseRecyclable)
		{
			disabledSpecialQuestDrops = true;
		}
		else if (preInstantiated)
		{
			if (instantiatedCorpses == null)
			{
				return;
			}
			GameObject[] array = instantiatedCorpses;
			foreach (GameObject gameObject in array)
			{
				if (!(gameObject == null))
				{
					RemoveQuestDropsFromGameObject(gameObject);
				}
			}
		}
		else
		{
			disabledSpecialQuestDrops = true;
		}
	}

	private void RemoveQuestDropsFromGameObject(GameObject gameObject)
	{
		SpecialQuestItemVariant[] componentsInChildren = gameObject.GetComponentsInChildren<SpecialQuestItemVariant>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetInactive();
		}
	}

	public void ReceiveDeathEvent(float attackDirection)
	{
		ReceiveDeathEvent(attackDirection, AttackTypes.Generic);
	}

	public void ReceiveDeathEvent(float? attackDirection, AttackTypes attackType, bool resetDeathEvent = false)
	{
		ReceiveDeathEvent(attackDirection, attackType, 1f, resetDeathEvent);
	}

	public void ReceiveDeathEvent(float? attackDirection, AttackTypes attackType, float corpseFlingMultiplier, bool resetDeathEvent = false)
	{
		ReceiveDeathEvent(attackDirection, attackType, NailElements.None, null, corpseFlingMultiplier, resetDeathEvent, null, out var _, out var _);
	}

	public void ReceiveDeathEvent(float? attackDirection, AttackTypes attackType, NailElements nailElement, GameObject damageSource, float corpseFlingMultiplier, bool resetDeathEvent, Action<Transform> onCorpseBegin, out bool didCallCorpseBegin, out GameObject corpseObj)
	{
		didCallCorpseBegin = false;
		corpseObj = null;
		if (didFire && !isCorpseRecyclable)
		{
			return;
		}
		didFire = true;
		RecordKillForJournal();
		if (corpseFlingMultiplier > 1.35f)
		{
			corpseFlingMultiplier = 1.35f;
		}
		if (attackType == AttackTypes.Lava)
		{
			ShakeCameraIfVisible();
			if ((bool)GlobalSettings.Corpse.EnemyLavaDeath)
			{
				GlobalSettings.Corpse.EnemyLavaDeath.Spawn().transform.SetPosition2D(base.transform.TransformPoint(effectOrigin));
			}
		}
		else
		{
			bool num = attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Acid;
			corpseObj = EmitCorpse(attackDirection, corpseFlingMultiplier, attackType, nailElement, damageSource, onCorpseBegin, out didCallCorpseBegin);
			if (!num)
			{
				EmitEffects(corpseObj);
			}
		}
		GameManager instance = GameManager.instance;
		if (!string.IsNullOrEmpty(setPlayerDataBool))
		{
			instance.playerData.SetBool(setPlayerDataBool, value: true);
		}
		if (!string.IsNullOrWhiteSpace(awardAchievement))
		{
			instance.AwardAchievement(awardAchievement);
		}
		if (!doNotSetHasKilled)
		{
			instance.playerData.SetBool("hasKilled", value: true);
		}
		if (audioSnapshotOnDeath != null)
		{
			audioSnapshotOnDeath.TransitionTo(2f);
		}
		if (!string.IsNullOrEmpty(sendEventRegister))
		{
			EventRegister.SendEvent(sendEventRegister);
		}
		if (!resetDeathEvent)
		{
			PersistentBoolItem component = GetComponent<PersistentBoolItem>();
			if ((bool)component)
			{
				component.SaveState();
			}
			if (recycle)
			{
				PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "health_manager_enemy");
				if (playMakerFSM != null)
				{
					playMakerFSM.FsmVariables.GetFsmBool("Activated").Value = false;
				}
				HealthManager component2 = GetComponent<HealthManager>();
				if (component2 != null)
				{
					component2.SetIsDead(set: false);
				}
				didFire = false;
				base.gameObject.Recycle();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "CENTIPEDE DEATH");
			didFire = false;
		}
	}

	public void RecordKillForJournal()
	{
		if (journalRecord == null)
		{
			return;
		}
		HealthManager component = GetComponent<HealthManager>();
		if (component == null || !component.WillAwardJournalKill)
		{
			return;
		}
		if (awardFullJournalEntry)
		{
			while (journalRecord.KillCount < journalRecord.KillsRequired - 1)
			{
				EnemyJournalManager.RecordKill(journalRecord, showPopup: false);
			}
		}
		EnemyJournalManager.RecordKill(journalRecord);
	}

	protected GameObject EmitCorpse(float? attackDirection, float flingMultiplier, AttackTypes attackType, NailElements nailElement, GameObject damageSource, Action<Transform> onCorpseBegin, out bool didCallCorpseBegin)
	{
		didCallCorpseBegin = false;
		if (doNotSpawnCorpse)
		{
			return null;
		}
		bool flag = attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Acid;
		float num = UnityEngine.Random.Range(0.008f, 0.009f);
		Transform parent = (GetComponentInParent<PersistentFolderReset>() ? base.transform.parent : null);
		GameObject gameObject;
		if (isCorpseRecyclable)
		{
			Transform obj = base.transform;
			Vector3 position = obj.TransformPoint(corpseSpawnPoint);
			position.z = num;
			gameObject = ObjectPool.Spawn(corpsePrefab, parent, position, Quaternion.identity, stealActiveSpawned: true);
			if (disabledSpecialQuestDrops)
			{
				RemoveQuestDropsFromGameObject(gameObject);
			}
			Vector3 localScale = obj.localScale;
			Vector3 localScale2 = gameObject.transform.localScale;
			localScale2.x = Mathf.Abs(localScale2.x) * Mathf.Sign(localScale.x);
			localScale2.y = Mathf.Abs(localScale2.y) * Mathf.Sign(localScale.y);
			localScale2.z = Mathf.Abs(localScale2.z) * Mathf.Sign(localScale.z);
			gameObject.transform.localScale = localScale2;
			DropRecycle.AddInactive(gameObject);
		}
		else
		{
			GameObject instantiatedCorpse = GetInstantiatedCorpse(attackType);
			if (!instantiatedCorpse)
			{
				return null;
			}
			instantiatedCorpse.transform.SetParent(parent);
			instantiatedCorpse.transform.SetPositionZ(num);
			instantiatedCorpse.SetActive(value: true);
			gameObject = instantiatedCorpse;
		}
		if (!gameObject)
		{
			return null;
		}
		if (GetBlackThreadAmount() > 0f)
		{
			BlackThreadState.IBlackThreadStateReceiver[] blackThreadEffects = gameObject.GetComponents<BlackThreadState.IBlackThreadStateReceiver>();
			if (blackThreadEffects != null && blackThreadEffects.Length > 0)
			{
				BlackThreadState.IBlackThreadStateReceiver[] array = blackThreadEffects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetIsBlackThreaded(isThreaded: true);
				}
				if (IsCorpseRecyclable)
				{
					RecycleResetHandler.Add(gameObject, (Action)delegate
					{
						if (blackThreadEffects != null && blackThreadEffects.Length > 0)
						{
							BlackThreadState.IBlackThreadStateReceiver[] array2 = blackThreadEffects;
							for (int j = 0; j < array2.Length; j++)
							{
								array2[j]?.SetIsBlackThreaded(isThreaded: false);
							}
						}
					});
				}
			}
		}
		this.CorpseEmitted?.Invoke(gameObject);
		CorpseItems component = gameObject.GetComponent<CorpseItems>();
		ActiveCorpse component2 = gameObject.GetComponent<ActiveCorpse>();
		HealthManager component3 = GetComponent<HealthManager>();
		if ((bool)component3)
		{
			if (component3.GetLastAttackType() == AttackTypes.Explosion)
			{
				FSMUtility.SendEventToGameObject(gameObject, "EXPLOSION DEATH");
			}
			if (component3.HasClearedItemDrops && (bool)component)
			{
				component.ClearPickupItems();
			}
		}
		tk2dSprite component4 = GetComponent<tk2dSprite>();
		PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
		for (int i = 0; i < components.Length; i++)
		{
			FsmString[] stringVariables = components[i].FsmVariables.StringVariables;
			foreach (FsmString fsmString in stringVariables)
			{
				if (fsmString.Name == "Owner Name")
				{
					fsmString.Value = base.gameObject.name;
				}
			}
		}
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "corpse");
		if (playMakerFSM != null)
		{
			FsmBool fsmBool = playMakerFSM.FsmVariables.GetFsmBool("spellBurn");
			if (fsmBool != null)
			{
				fsmBool.Value = false;
			}
		}
		Corpse component5 = gameObject.GetComponent<Corpse>();
		if ((bool)component5)
		{
			component5.Setup(OverrideBloodColor, onCorpseBegin, isCorpseRecyclable);
			didCallCorpseBegin = true;
		}
		TagDamageTaker component6 = GetComponent<TagDamageTaker>();
		bool flag2 = false;
		if ((bool)component2)
		{
			component2.QueueBurnEffects(component4, attackType, nailElement, damageSource, 1f, component6);
			flag2 = true;
		}
		else
		{
			if (component5 is CorpseRegular corpseRegular)
			{
				if (attackType == AttackTypes.Fire || attackType == AttackTypes.Explosion || nailElement == NailElements.Fire)
				{
					corpseRegular.SpawnElementalEffects(ElementalEffectType.Fire);
				}
				else if (attackType == AttackTypes.Lightning)
				{
					corpseRegular.SpawnElementalEffects(ElementalEffectType.Lightning);
				}
				else if (damageSource != null)
				{
					DamageEnemies component7 = damageSource.GetComponent<DamageEnemies>();
					if ((bool)component7)
					{
						_ = component7.RepresentingTool;
						if (component7.ZapDamageTicks > 0)
						{
							corpseRegular.SpawnElementalEffects(ElementalEffectType.Lightning);
						}
					}
				}
			}
			ActiveCorpse[] componentsInChildren = gameObject.GetComponentsInChildren<ActiveCorpse>(includeInactive: true);
			float[] array3 = new float[componentsInChildren.Length];
			float num2 = 0f;
			for (int l = 0; l < componentsInChildren.Length; l++)
			{
				Collider2D component8 = componentsInChildren[l].GetComponent<Collider2D>();
				if ((bool)component8)
				{
					num2 += (array3[l] = component8.bounds.size.magnitude);
				}
				else
				{
					array3[l] = 0f;
				}
			}
			for (int m = 0; m < componentsInChildren.Length; m++)
			{
				ActiveCorpse obj2 = componentsInChildren[m];
				float scale = array3[m] / num2;
				obj2.QueueBurnEffects(component4, attackType, nailElement, damageSource, scale, component6);
				flag2 = true;
			}
		}
		if (!flag2 && component6 != null)
		{
			foreach (DamageTag key in component6.TaggedDamage.Keys)
			{
				if ((bool)key.CorpseBurnEffect)
				{
					key.SpawnDeathEffects(gameObject.transform.position);
					break;
				}
			}
		}
		if (flag)
		{
			return gameObject;
		}
		Rigidbody2D component9 = gameObject.GetComponent<Rigidbody2D>();
		float rotation = (rotateCorpse ? base.transform.GetLocalRotation2D() : 0f);
		gameObject.transform.SetRotation2D(rotation);
		if ((bool)component9)
		{
			component9.rotation = rotation;
		}
		if (corpseMatchesEnemyScale)
		{
			gameObject.transform.localScale = base.transform.localScale;
			return gameObject;
		}
		if (Mathf.Abs(base.transform.eulerAngles.z) >= 45f)
		{
			Collider2D component10 = GetComponent<Collider2D>();
			Collider2D component11 = gameObject.GetComponent<Collider2D>();
			if (!rotateCorpse && (bool)component10 && (bool)component11)
			{
				Vector3 vector = component10.bounds.center - component11.bounds.center;
				vector.z = 0f;
				gameObject.transform.position += vector;
			}
		}
		float num3 = 1f;
		if (!attackDirection.HasValue)
		{
			flingMultiplier = 0f;
			num3 = Mathf.Sign(base.transform.lossyScale.x);
		}
		int num4 = DirectionUtils.GetCardinalDirection(attackDirection.GetValueOrDefault());
		float num5 = gameObject.transform.localScale.x * (corpseFacesRight ? 1f : (-1f)) * num3;
		if (overrideDeathDirection)
		{
			num4 = ((!(num5 < 0f)) ? 2 : 0);
		}
		if (component9 == null)
		{
			return gameObject;
		}
		float num6 = corpseFlingSpeed;
		float num7 = 60f;
		float num8 = 120f;
		if (flingMultiplier > 1.25f)
		{
			num7 = 45f;
			num8 = 135f;
		}
		float num9;
		switch (num4)
		{
		case 0:
			num9 = (lowCorpseArc ? 10f : num7);
			gameObject.transform.SetScaleX((0f - num5) * Mathf.Sign(base.transform.localScale.x));
			break;
		case 2:
			num9 = (lowCorpseArc ? 170f : num8);
			gameObject.transform.SetScaleX(num5 * Mathf.Sign(base.transform.localScale.x));
			break;
		case 3:
			num9 = 270f;
			break;
		case 1:
			num9 = UnityEngine.Random.Range(75f, 105f);
			num6 *= 1.3f;
			break;
		default:
			num9 = 90f;
			break;
		}
		if (flingMultiplier < 0.5f && Math.Abs(flingMultiplier) > Mathf.Epsilon)
		{
			flingMultiplier = 0.5f;
		}
		if (flingMultiplier > 1.5f)
		{
			flingMultiplier = 1.5f;
		}
		component9.linearVelocity = new Vector2(Mathf.Cos(num9 * (MathF.PI / 180f)), Mathf.Sin(num9 * (MathF.PI / 180f))) * (num6 * flingMultiplier);
		return gameObject;
	}

	protected virtual void EmitEffects(GameObject corpseObj)
	{
		Debug.Log("EnemyDeathEffects EmitEffects not overidden!", this);
	}

	public void EmitSound()
	{
	}

	protected void ShakeCameraIfVisible(string eventName)
	{
		if (IsVisible)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent(eventName);
		}
	}

	protected void ShakeCameraIfVisible()
	{
		if (IsVisible)
		{
			deathCameraShake.DoShake(this, !SkipKillFreeze);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(corpseSpawnPoint, 0.25f);
		Gizmos.DrawWireSphere(effectOrigin, 0.25f);
	}

	public float GetBlackThreadAmount()
	{
		return isBlackThreaded ? 1 : 0;
	}

	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			isBlackThreaded = true;
		}
		else
		{
			isBlackThreaded = false;
		}
	}
}
