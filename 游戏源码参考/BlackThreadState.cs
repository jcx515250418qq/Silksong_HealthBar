using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Audio;

public class BlackThreadState : MonoBehaviour, Recoil.IRecoilMultiplier, IInitialisable
{
	public interface IBlackThreadStateReceiver
	{
		void SetIsBlackThreaded(bool isThreaded);
	}

	private class ProbabilityBool : Probability.ProbabilityBase<bool>
	{
		public override bool Item { get; }

		public ProbabilityBool(bool value, float probability)
		{
			Item = value;
			Probability = probability;
		}
	}

	private enum VoiceState
	{
		None = 0,
		NotFound = 1,
		Found = 2,
		ReplacedMixer = 3
	}

	private const float ACTIVATE_RANGE = 8f;

	private const float ACTIIVATE_RANGE_SQR = 64f;

	private const float ACTIVATE_TIMER = 2f;

	private const float ACTIVATE_RANGE_CHECK_DELAY = 0.5f;

	private const float HP_MULTIPLIER = 2f;

	private const float ATTACK_CHECK_TIMER = 2f;

	private const float ATTACK_PROBABILITY = 0.5f;

	private const float THREAD_PROBABILITY = 0.15f;

	private const int TERRAIN_LAYER_MASK = 256;

	private const float RECOIL_MULTIPLIER = 0.2f;

	private const float EFFECT_ACTIVE_FADE_TIME = 0.2f;

	private const float EFFECT_ACTIVE_SLOW_FADE_TIME = 1f;

	public const string BLACK_THREAD_KEYWORD = "BLACKTHREAD";

	[SerializeField]
	private PlayMakerFSM singFsm;

	[SerializeField]
	[HideInInspector]
	[Obsolete("Upgraded, please use attacks array.")]
	private BlackThreadAttack attack;

	[SerializeField]
	[AssetPickerDropdown]
	private BlackThreadAttack[] attacks;

	[SerializeField]
	private bool overrideCentreOffset;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideCentreOffset", true, false, false)]
	private Vector2 centreOffsetOverride;

	[SerializeField]
	private bool force;

	[SerializeField]
	private bool startThreaded;

	[SerializeField]
	private bool useCustomHPMultiplier;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("useCustomHPMultiplier", true, false, false)]
	private float customHPMultiplier = 4f;

	[Space]
	[SerializeField]
	private SpriteRenderer[] extraSpriteRenderers;

	[SerializeField]
	private MeshRenderer[] extraMeshRenderers;

	[Space]
	[UnityEngine.Tooltip("Effects / Logic updates will pause when off screen + buffer amount")]
	[SerializeField]
	private Vector2 activityRangeBuffer = new Vector2(4f, 4f);

	private int lastActiveRangeCheck;

	private bool isInActiveRange;

	private bool isBlackThreadWorld;

	private bool willBeThreaded;

	private bool isThreaded;

	private bool queuedForceAttack;

	private DamageHero[] childDamagers;

	private tk2dSprite[] tk2dSprites;

	private MeshRenderer[] tk2dSpriteRenderers;

	private HealthManager healthManager;

	private List<IBlackThreadStateReceiver> stateReceivers = new List<IBlackThreadStateReceiver>();

	private static List<tk2dSprite> _tempSprites;

	private bool hasCollider;

	private Collider2D collider;

	private Rigidbody2D rb2d;

	private bool hasRb2d;

	private PlayMakerFSM stunControlFsm;

	private FsmBool stunControlAttackBool;

	private BlackThreadAttack chosenAttack;

	private readonly Dictionary<BlackThreadAttack, GameObject> spawnedAttackObjs = new Dictionary<BlackThreadAttack, GameObject>();

	private Color[] initialColors;

	private Color[] startColors;

	private Coroutine pulseRoutine;

	private MaterialPropertyBlock block;

	private Coroutine becomeThreadedRoutine;

	private Coroutine waitRangeRoutine;

	private Recoil recoil;

	private Vector2 centreOffset;

	private static readonly string[] _singingStateNames = new string[6] { "Sing", "Pray", "Needolin", "Praying", "Sing Ground", "Sing Air" };

	private readonly ProbabilityBool[] attackProbability = new ProbabilityBool[2]
	{
		new ProbabilityBool(value: true, 0.5f),
		new ProbabilityBool(value: false, 0.5f)
	};

	private float[] attackProbOverrides;

	private static readonly ProbabilityBool[] _threadProbability = new ProbabilityBool[2]
	{
		new ProbabilityBool(value: true, 0.15f),
		new ProbabilityBool(value: false, 0.85f)
	};

	private static float[] _threadProbOverrides;

	private static readonly int _blackThreadAmountProp = Shader.PropertyToID("_BlackThreadAmount");

	private GameObject activeAttack;

	private Func<bool> attackEndedFunc;

	private bool blackThreadEffectsIsActive;

	private bool hasSpawnedBlackThreadEffects;

	private NestedFadeGroupBase blackThreadEffectsFader;

	private GameObject blackThreadEffectsObject;

	private Coroutine attackTestRoutine;

	private bool hasChosenAttack;

	private VoiceState voiceState;

	private bool startAlreadyBlackThreadedOnEnable;

	private List<BlackThreadedEffect> blackThreadedEffects = new List<BlackThreadedEffect>();

	private bool hasAwaken;

	private bool hasStarted;

	private bool hasDoneFirstSetUp;

	private List<AudioSource> blackThreadVoiceSources = new List<AudioSource>();

	private EnemyDeathEffects enemyDeathEffect;

	private bool IsInActiveRange
	{
		get
		{
			if (lastActiveRangeCheck == Time.frameCount)
			{
				return isInActiveRange;
			}
			bool flag = CameraInfoCache.IsWithinBounds(base.transform.position, activityRangeBuffer);
			if (isInActiveRange == flag)
			{
				return isInActiveRange;
			}
			isInActiveRange = flag;
			return isInActiveRange;
		}
	}

	public bool IsBlackThreaded => isThreaded;

	private Vector2 CentreOffset
	{
		get
		{
			if (!overrideCentreOffset)
			{
				return centreOffset;
			}
			return centreOffsetOverride;
		}
	}

	public bool IsInForcedSing { get; private set; }

	public bool IsVisiblyThreaded { get; private set; }

	private bool IsInAttack => activeAttack;

	private bool IsEnemyHidden
	{
		get
		{
			if (base.gameObject.layer != 11)
			{
				return true;
			}
			if (hasCollider && !collider.enabled)
			{
				return true;
			}
			MeshRenderer[] array = tk2dSpriteRenderers;
			if (array != null && array.Length > 0)
			{
				array = tk2dSpriteRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].enabled)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}

	GameObject IInitialisable.gameObject => base.gameObject;

	private void Reset()
	{
		GetSingFsm();
	}

	private void OnDrawGizmos()
	{
		if (attacks == null)
		{
			return;
		}
		BlackThreadAttack[] array = attacks;
		foreach (BlackThreadAttack blackThreadAttack in array)
		{
			if ((bool)blackThreadAttack)
			{
				Collider2D component = GetComponent<Collider2D>();
				blackThreadAttack.DrawAttackRangeGizmos(component ? component.bounds.center : base.transform.position);
			}
		}
		if (overrideCentreOffset)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireSphere(centreOffsetOverride, 0.2f);
		}
	}

	private void OnValidate()
	{
		if ((bool)attack)
		{
			attacks = new BlackThreadAttack[1] { attack };
			attack = null;
		}
	}

	private void Awake()
	{
		OnAwake();
	}

	private void Start()
	{
		OnStart();
		if (isBlackThreadWorld)
		{
			SetupThreaded(isFirst: true);
		}
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		OnValidate();
		healthManager = base.gameObject.GetComponent<HealthManager>();
		if (!healthManager)
		{
			Debug.LogError("Enemy has BlackThreadState but no HealthManager! Disabling script.", this);
			base.enabled = false;
			return true;
		}
		healthManager.OnDeath += delegate
		{
			StopAllCoroutines();
			if (IsVisiblyThreaded)
			{
				for (int i = 0; i < tk2dSprites.Length; i++)
				{
					tk2dSprites[i].color = initialColors[i];
				}
			}
		};
		attackEndedFunc = () => !IsInAttack;
		collider = GetComponent<Collider2D>();
		hasCollider = collider;
		if (!singFsm)
		{
			GetSingFsm();
		}
		EnemyHitEffectsRegular component = GetComponent<EnemyHitEffectsRegular>();
		if ((bool)component)
		{
			component.ReceivedHitEffect += OnReceivedHitEffect;
		}
		stateReceivers = GetComponentsInChildren<IBlackThreadStateReceiver>(includeInactive: true).ToList();
		EnemyDeathEffects component2 = GetComponent<EnemyDeathEffects>();
		if ((bool)component2)
		{
			component2.CorpseEmitted += delegate(GameObject corpseObj)
			{
				PassBlackThreadState component3 = corpseObj.GetComponent<PassBlackThreadState>();
				if ((bool)component3)
				{
					component3.IsBlackThreaded = IsVisiblyThreaded;
					component3.ChosenAttack = chosenAttack;
				}
			};
		}
		tk2dSprites = base.gameObject.GetComponentsInChildren<tk2dSprite>(includeInactive: true);
		tk2dSpriteRenderers = new MeshRenderer[tk2dSprites.Length];
		initialColors = new Color[tk2dSprites.Length];
		startColors = new Color[tk2dSprites.Length];
		for (int j = 0; j < tk2dSpriteRenderers.Length; j++)
		{
			tk2dSpriteRenderers[j] = tk2dSprites[j].GetComponent<MeshRenderer>();
		}
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			string fsmName = playMakerFSM.FsmName;
			if (fsmName == "Stun Control" || fsmName == "Stun")
			{
				stunControlFsm = playMakerFSM;
				stunControlAttackBool = stunControlFsm.FsmVariables.FindFsmBool("Abyss Attacking");
				break;
			}
		}
		rb2d = GetComponent<Rigidbody2D>();
		hasRb2d = rb2d != null;
		if (enemyDeathEffect == null)
		{
			enemyDeathEffect = GetComponent<EnemyDeathEffects>();
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		if (GameManager.instance.GetCurrentMapZoneEnum() == MapZone.MEMORY)
		{
			return true;
		}
		if (!PlayerData.instance.blackThreadWorld)
		{
			return true;
		}
		isBlackThreadWorld = true;
		BlackThreadAttack[] array = ((attacks != null && attacks.Length != 0) ? attacks : Effects.BlackThreadAttacksDefault);
		if (array != null)
		{
			foreach (BlackThreadAttack blackThreadAttack in array)
			{
				if (!(blackThreadAttack == null))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(blackThreadAttack.Prefab, base.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.SetActive(value: false);
					spawnedAttackObjs[blackThreadAttack] = gameObject;
				}
			}
		}
		PreSpawnBlackThreadEffects();
		FirstThreadedSetUp();
		return true;
	}

	private void OnEnable()
	{
		recoil = GetComponent<Recoil>();
		if ((bool)recoil)
		{
			recoil.AddRecoilMultiplier(this);
		}
		if (startAlreadyBlackThreadedOnEnable)
		{
			StartCoroutine(WaitForActive());
			SetBlackThreadAmount(1f);
			StartAttackTest();
		}
	}

	private void OnDisable()
	{
		if ((bool)recoil)
		{
			recoil.RemoveRecoilMultiplier(this);
			recoil = null;
		}
	}

	public void PassState(PassBlackThreadState pass)
	{
		force = pass.IsBlackThreaded;
		startThreaded = pass.IsBlackThreaded;
		attacks = new BlackThreadAttack[1] { pass.ChosenAttack };
	}

	private void GetSingFsm()
	{
		singFsm = GetComponents<PlayMakerFSM>().FirstOrDefault((PlayMakerFSM fsmComp) => (fsmComp.FsmTemplate ? fsmComp.FsmTemplate.fsm : fsmComp.Fsm).States.Any((FsmState state) => state.Name.IsAny(_singingStateNames)));
	}

	private void OnReceivedHitEffect(HitInstance damageInstance, Vector2 origin)
	{
		if (!isThreaded)
		{
			return;
		}
		if (!IsVisiblyThreaded)
		{
			if (waitRangeRoutine != null)
			{
				StopCoroutine(waitRangeRoutine);
				waitRangeRoutine = null;
			}
			BecomeThreaded();
		}
		else
		{
			Effects.DoBlackThreadHit(base.gameObject, damageInstance, origin);
		}
	}

	private void SetupThreaded(bool isFirst)
	{
		if (!isBlackThreadWorld || isThreaded)
		{
			return;
		}
		if (!singFsm)
		{
			Debug.LogError("No Sing FSM set for " + base.gameObject.name + "! Will not be black threaded.");
			return;
		}
		bool isAnyActive = BlackThreadCore.IsAnyActive;
		if (force || isAnyActive || willBeThreaded)
		{
			isThreaded = true;
		}
		else
		{
			GameObject gameObject = base.gameObject;
			string sceneName = gameObject.scene.name;
			string id = gameObject.name + "_BlackThread";
			SceneData sd = SceneData.instance;
			if (sd.PersistentBools.TryGetValue(sceneName, id, out var value))
			{
				isThreaded = value.Value;
			}
			else
			{
				isThreaded = Probability.GetRandomItemByProbabilityFair<ProbabilityBool, bool>(_threadProbability, ref _threadProbOverrides);
				sd.PersistentBools.SetValue(new PersistentItemData<bool>
				{
					SceneName = sceneName,
					ID = id,
					Value = isThreaded,
					IsSemiPersistent = true
				});
			}
			if (isFirst)
			{
				healthManager.OnDeath += delegate
				{
					sd.PersistentBools.SetValue(new PersistentItemData<bool>
					{
						SceneName = sceneName,
						ID = id,
						Value = false,
						IsSemiPersistent = true
					});
				};
			}
		}
		PreSpawnBlackThreadEffects();
		if (isFirst)
		{
			FirstThreadedSetUp();
		}
		SetBlackThreadAmount(0f);
		if (isThreaded)
		{
			ChooseAttack(force: true);
			float num = 2f;
			if (useCustomHPMultiplier)
			{
				num = customHPMultiplier;
			}
			healthManager.hp = Mathf.FloorToInt((float)healthManager.hp * num);
			if (startThreaded || isAnyActive)
			{
				startAlreadyBlackThreadedOnEnable = true;
				StartCoroutine(WaitForActive());
				SetBlackThreadAmount(1f);
				StartAttackTest();
			}
			else if (!willBeThreaded)
			{
				waitRangeRoutine = StartCoroutine(WaitForInRange());
			}
		}
	}

	private void FirstThreadedSetUp()
	{
		if (hasDoneFirstSetUp)
		{
			return;
		}
		FindVoice();
		healthManager.OnDeath += delegate
		{
			if (IsVisiblyThreaded)
			{
				GameObject blackThreadEnemyDeathEffect = Effects.BlackThreadEnemyDeathEffect;
				if ((bool)blackThreadEnemyDeathEffect)
				{
					blackThreadEnemyDeathEffect.Spawn(base.transform).transform.SetParent(null, worldPositionStays: true);
				}
			}
		};
		tk2dSprite[] array = tk2dSprites;
		foreach (tk2dSprite obj in array)
		{
			obj.ForceBuild();
			obj.EnableKeyword("BLACKTHREAD");
		}
		if (!(enemyDeathEffect != null))
		{
			enemyDeathEffect = GetComponent<EnemyDeathEffects>();
			_ = enemyDeathEffect != null;
		}
		blackThreadedEffects.AddRange(GetComponentsInChildren<BlackThreadedEffect>(includeInactive: true));
		SpriteRenderer[] array2 = extraSpriteRenderers;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].material.EnableKeyword("BLACKTHREAD");
		}
		MeshRenderer[] array3 = extraMeshRenderers;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].material.EnableKeyword("BLACKTHREAD");
		}
		hasDoneFirstSetUp = true;
	}

	public void ChooseAttack(bool force)
	{
		if (!force && hasChosenAttack)
		{
			return;
		}
		if (CheatManager.ForceChosenBlackThreadAttack && (bool)CheatManager.ChosenBlackThreadAttack)
		{
			chosenAttack = CheatManager.ChosenBlackThreadAttack;
			hasChosenAttack = true;
			return;
		}
		if (attacks == null || attacks.Length == 0)
		{
			chosenAttack = Effects.BlackThreadAttacksDefault.GetRandomElement();
		}
		else
		{
			chosenAttack = attacks.GetRandomElement();
		}
		hasChosenAttack = true;
	}

	private void PreSpawnBlackThreadEffects()
	{
		if (!hasSpawnedBlackThreadEffects)
		{
			GameObject blackThreadEnemyEffect = Effects.BlackThreadEnemyEffect;
			if ((bool)blackThreadEnemyEffect)
			{
				blackThreadEffectsObject = UnityEngine.Object.Instantiate(blackThreadEnemyEffect, base.transform);
				MatchEffectsToObject(base.gameObject, blackThreadEffectsObject, out centreOffset);
				hasSpawnedBlackThreadEffects = true;
				blackThreadEffectsFader = blackThreadEffectsObject.GetComponent<NestedFadeGroupBase>();
				blackThreadEffectsFader.AlphaSelf = 0f;
				blackThreadEffectsObject.SetActive(value: false);
			}
		}
	}

	private void UpdateBlackThreadEffects(bool forceOff = false)
	{
		bool flag = !forceOff && IsVisiblyThreaded && IsInActiveRange && !IsEnemyHidden;
		if (flag == blackThreadEffectsIsActive)
		{
			return;
		}
		PreSpawnBlackThreadEffects();
		blackThreadEffectsIsActive = flag;
		if (!hasSpawnedBlackThreadEffects)
		{
			return;
		}
		if ((bool)blackThreadEffectsFader)
		{
			if (!blackThreadEffectsObject.activeSelf)
			{
				blackThreadEffectsObject.SetActive(value: true);
			}
			if (flag)
			{
				blackThreadEffectsFader.FadeTo(1f, 0.2f);
				return;
			}
			float fadeTime = 0.2f;
			if (!IsEnemyHidden)
			{
				fadeTime = 1f;
			}
			blackThreadEffectsFader.FadeTo(0f, fadeTime, null, isRealtime: false, delegate(bool finished)
			{
				if (finished)
				{
					blackThreadEffectsObject.SetActive(value: false);
				}
			});
		}
		else
		{
			blackThreadEffectsObject.SetActive(blackThreadEffectsIsActive);
		}
	}

	private IEnumerator WaitForActive()
	{
		yield return null;
		while (IsEnemyHidden)
		{
			yield return null;
		}
		SetVisiblyThreaded();
	}

	private IEnumerator MonitorActive()
	{
		while (true)
		{
			UpdateBlackThreadEffects();
			yield return null;
		}
	}

	private IEnumerator WaitForInRange()
	{
		HeroController hc = HeroController.instance;
		Transform hero = hc.transform;
		Transform self = base.transform;
		while (!hc.isHeroInPosition)
		{
			yield return null;
		}
		WaitForSeconds wait = new WaitForSeconds(0.5f);
		float timeLeft = 2f;
		while (timeLeft > 0f)
		{
			Vector2 vector = hero.position;
			Vector2 vector2 = self.position;
			timeLeft = ((!(Vector2.SqrMagnitude(vector - vector2) <= 64f)) ? 2f : (timeLeft - 0.5f));
			yield return wait;
		}
		waitRangeRoutine = null;
		BecomeThreaded();
	}

	private void SetBlackThreadAmount(float amount)
	{
		if (block == null)
		{
			block = new MaterialPropertyBlock();
		}
		MeshRenderer[] array = tk2dSpriteRenderers;
		foreach (MeshRenderer obj in array)
		{
			obj.GetPropertyBlock(block);
			block.SetFloat(_blackThreadAmountProp, amount);
			obj.SetPropertyBlock(block);
		}
		foreach (BlackThreadedEffect blackThreadedEffect in blackThreadedEffects)
		{
			blackThreadedEffect.SetBlackThreadAmount(amount);
		}
	}

	public void BecomeThreaded()
	{
		ChooseAttack(force: false);
		if (becomeThreadedRoutine == null)
		{
			becomeThreadedRoutine = StartCoroutine(BecomeThreadedRoutine(waitForSing: true));
		}
	}

	public void BecomeThreadedNoSing()
	{
		ChooseAttack(force: false);
		if (becomeThreadedRoutine == null)
		{
			becomeThreadedRoutine = StartCoroutine(BecomeThreadedRoutine(waitForSing: false));
		}
	}

	private IEnumerator BecomeThreadedRoutine(bool waitForSing)
	{
		if (waitForSing)
		{
			IsInForcedSing = true;
			string currentName = singFsm.ActiveStateName;
			if (!currentName.Trim().IsAny(_singingStateNames))
			{
				while (true)
				{
					yield return null;
					string activeStateName = singFsm.ActiveStateName;
					if (currentName != activeStateName)
					{
						currentName = activeStateName;
						if (currentName.Trim().IsAny(_singingStateNames))
						{
							break;
						}
					}
				}
			}
		}
		SetVisiblyThreaded();
		GameObject blackThreadEnemyStartEffect = Effects.BlackThreadEnemyStartEffect;
		if ((bool)blackThreadEnemyStartEffect)
		{
			blackThreadEnemyStartEffect.Spawn(base.transform, CentreOffset);
		}
		yield return new WaitForSeconds(0.4f);
		for (float elapsed = 0f; elapsed < 0.3f; elapsed += Time.deltaTime)
		{
			SetBlackThreadAmount(elapsed / 0.3f);
			yield return null;
		}
		SetBlackThreadAmount(1f);
		yield return new WaitForSeconds(0.05f);
		if (waitForSing)
		{
			IsInForcedSing = false;
		}
		StartAttackTest();
	}

	private void SetVisiblyThreaded()
	{
		IsVisiblyThreaded = true;
		for (int i = 0; i < tk2dSpriteRenderers.Length; i++)
		{
			initialColors[i] = tk2dSprites[i].color;
		}
		foreach (IBlackThreadStateReceiver stateReceiver in stateReceivers)
		{
			stateReceiver?.SetIsBlackThreaded(isThreaded: true);
		}
		StartPulseRoutine();
		StartCoroutine(MonitorActive());
		childDamagers = base.gameObject.GetComponentsInChildren<DamageHero>(includeInactive: true);
		DamageHero[] array = childDamagers;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].damagePropertyFlags |= DamagePropertyFlags.Void;
		}
		ChangeVoiceOutput();
	}

	private void StartAttackTest()
	{
		if ((bool)chosenAttack)
		{
			if (attackTestRoutine != null)
			{
				StopCoroutine(attackTestRoutine);
			}
			attackTestRoutine = StartCoroutine(ThreadAttackTest());
		}
	}

	private void StopPulseRoutine()
	{
		if (pulseRoutine != null)
		{
			StopCoroutine(pulseRoutine);
			pulseRoutine = null;
		}
	}

	private void StartPulseRoutine()
	{
		StopPulseRoutine();
		pulseRoutine = StartCoroutine(PulseBlack());
	}

	public void ResetThreaded()
	{
		StopAllCoroutines();
		if (IsVisiblyThreaded)
		{
			IsVisiblyThreaded = false;
			for (int i = 0; i < tk2dSpriteRenderers.Length; i++)
			{
				tk2dSprites[i].color = initialColors[i];
			}
			UpdateBlackThreadEffects(forceOff: true);
			foreach (IBlackThreadStateReceiver stateReceiver in stateReceivers)
			{
				stateReceiver?.SetIsBlackThreaded(isThreaded: false);
			}
		}
		childDamagers = base.gameObject.GetComponentsInChildren<DamageHero>(includeInactive: true);
		DamageHero[] array = childDamagers;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].damagePropertyFlags &= ~DamagePropertyFlags.Void;
		}
		isThreaded = false;
		IsInForcedSing = false;
		SetupThreaded(isFirst: false);
	}

	private IEnumerator ThreadAttackTest()
	{
		Transform hero = HeroController.instance.transform;
		WaitForSecondsInterruptable threadAttackWait = new WaitForSecondsInterruptable(2f, () => queuedForceAttack);
		while (true)
		{
			if ((!IsInActiveRange || IsEnemyHidden) && !queuedForceAttack)
			{
				yield return null;
				continue;
			}
			if (!queuedForceAttack)
			{
				threadAttackWait.ResetTimer();
				yield return threadAttackWait;
				if (IsEnemyHidden)
				{
					continue;
				}
				Vector3 vector = base.transform.TransformPoint(CentreOffset);
				Vector3 position = hero.position;
				if (!chosenAttack.IsInRange(position, vector) || (bool)Physics2D.Linecast(vector, position, 256) || !Probability.GetRandomItemByProbabilityFair<ProbabilityBool, bool>(attackProbability, ref attackProbOverrides))
				{
					continue;
				}
			}
			queuedForceAttack = false;
			IsInForcedSing = true;
			if (stunControlAttackBool != null)
			{
				stunControlAttackBool.Value = true;
			}
			string currentName = singFsm.ActiveStateName;
			string value = currentName.Trim();
			float timeOutLeft = 5f;
			if (!value.IsAny(_singingStateNames))
			{
				do
				{
					yield return null;
					string activeStateName = singFsm.ActiveStateName;
					if (currentName != activeStateName)
					{
						currentName = activeStateName;
						if (currentName.Trim().IsAny(_singingStateNames))
						{
							break;
						}
					}
					timeOutLeft -= Time.deltaTime;
				}
				while (!(timeOutLeft <= 0f));
			}
			float duration = Effects.BlackThreadEnemyAttackTintDuration;
			AnimationCurve curve = Effects.BlackThreadEnemyAttackTintCurve;
			if (timeOutLeft > 0f)
			{
				StopPulseRoutine();
				for (int i = 0; i < tk2dSpriteRenderers.Length; i++)
				{
					startColors[i] = tk2dSprites[i].color;
				}
				for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
				{
					float t = curve.Evaluate(elapsed / duration);
					for (int j = 0; j < tk2dSpriteRenderers.Length; j++)
					{
						tk2dSprites[j].color = Color.Lerp(startColors[j], Color.black, t);
					}
					yield return null;
				}
				if (hasRb2d)
				{
					rb2d.linearVelocity = Vector2.zero;
				}
				DoAttack(chosenAttack);
				yield return new WaitForSecondsInterruptable(chosenAttack.Duration, attackEndedFunc);
				activeAttack = null;
				IsInForcedSing = false;
				if (stunControlAttackBool != null)
				{
					stunControlAttackBool.Value = false;
				}
				for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
				{
					float t2 = curve.Evaluate(elapsed / duration);
					for (int k = 0; k < tk2dSpriteRenderers.Length; k++)
					{
						tk2dSprites[k].color = Color.Lerp(Color.black, initialColors[k], t2);
					}
					yield return null;
				}
				StartPulseRoutine();
			}
			else
			{
				IsInForcedSing = false;
				if (stunControlAttackBool != null)
				{
					stunControlAttackBool.Value = false;
				}
			}
		}
	}

	private IEnumerator PulseBlack()
	{
		float elapsed = 0f;
		float duration = Effects.BlackThreadEnemyPulseTintDuration;
		AnimationCurve curve = Effects.BlackThreadEnemyPulseTintCurve;
		while (true)
		{
			if (!IsInActiveRange)
			{
				yield return null;
				continue;
			}
			float t = curve.Evaluate(elapsed / duration);
			for (int i = 0; i < tk2dSpriteRenderers.Length; i++)
			{
				tk2dSprites[i].color = Color.Lerp(initialColors[i], Color.black, t);
			}
			yield return null;
			elapsed += Time.deltaTime;
		}
	}

	private void DoAttack(BlackThreadAttack currentAttack)
	{
		if (!currentAttack || !spawnedAttackObjs.TryGetValue(currentAttack, out var value) || !value)
		{
			return;
		}
		value.SetActive(value: false);
		activeAttack = value;
		Transform transform = value.transform;
		transform.localPosition = CentreOffset;
		transform.localScale = currentAttack.Prefab.transform.localScale;
		Vector3 localScale = transform.localScale;
		Vector3 lossyScale = transform.lossyScale;
		Vector3 vector = base.transform.TransformPoint(CentreOffset);
		Transform transform2 = HeroController.instance.transform;
		if (lossyScale.x < 0f)
		{
			localScale.x *= -1f;
		}
		if (transform2.position.x < vector.x)
		{
			localScale.x *= -1f;
		}
		if (currentAttack.CounterRotate)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.z = 0f - base.transform.localEulerAngles.z;
			if (lossyScale.x < 0f && Mathf.Abs(Mathf.Abs(Mathf.DeltaAngle(localEulerAngles.z, 0f)) - 90f) < 20f)
			{
				localEulerAngles.z += 180f;
			}
			transform.localEulerAngles = localEulerAngles;
		}
		transform.localScale = localScale;
		value.SetActive(value: true);
	}

	public void CancelAttack()
	{
		if (IsInAttack)
		{
			activeAttack.SetActive(value: false);
			activeAttack = null;
		}
	}

	private static void MatchEffectsToObject(GameObject gameObject, GameObject effectObj, out Vector2 centreOffset, bool recycles = false)
	{
		centreOffset = Vector2.zero;
		Transform transform = gameObject.transform;
		Transform transform2 = effectObj.transform;
		transform2.localPosition = Vector3.zero;
		Collider2D component = gameObject.GetComponent<Collider2D>();
		if (!component)
		{
			return;
		}
		Vector2 self;
		Vector3 vector;
		if (component.enabled)
		{
			Bounds bounds = component.bounds;
			self = bounds.size;
			vector = bounds.center;
		}
		else if (!(component is BoxCollider2D boxCollider2D))
		{
			if (!(component is CircleCollider2D circleCollider2D))
			{
				Debug.LogError("Black Thread Enemy \"" + gameObject.name + "\" has inactive collider that can't be manually handled!", gameObject);
				return;
			}
			float num = circleCollider2D.radius * 2f;
			self = new Vector2(num, num);
			vector = transform.TransformPoint(circleCollider2D.offset);
		}
		else
		{
			self = boxCollider2D.size;
			vector = transform.TransformPoint(boxCollider2D.offset);
		}
		transform2.SetPosition2D(vector);
		centreOffset = transform.InverseTransformPoint(vector);
		Vector3 localScale = transform2.localScale;
		Vector2 other = localScale;
		Vector2 vector2 = self.DivideElements(other);
		float num2 = localScale.x * localScale.y;
		float num3 = vector2.x * vector2.y;
		localScale.x = vector2.x;
		localScale.y = vector2.y;
		transform2.localScale = localScale;
		float num4 = num3 / num2;
		if (recycles)
		{
			num4 = Mathf.Clamp(num4, 1f, 3f);
		}
		ParticleSystem[] componentsInChildren = effectObj.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			int initialMaxParticles = main.maxParticles;
			main.maxParticles = Mathf.CeilToInt((float)main.maxParticles * num4);
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			float initialRateOverTimeMultiplier = emission.rateOverTimeMultiplier;
			emission.rateOverTimeMultiplier *= num4;
			float initialRateOverDistanceMultiplier = emission.rateOverDistanceMultiplier;
			emission.rateOverDistanceMultiplier *= num4;
			NestedFadeGroupParticleSystem particleFadeGroup = particleSystem.GetComponent<NestedFadeGroupParticleSystem>();
			if ((bool)particleFadeGroup)
			{
				particleFadeGroup.UpdateParticlesArraySize();
			}
			if (!recycles)
			{
				continue;
			}
			RecycleResetHandler.Add(particleSystem.gameObject, (Action)delegate
			{
				main.maxParticles = initialMaxParticles;
				emission.rateOverTimeMultiplier = initialRateOverTimeMultiplier;
				emission.rateOverDistanceMultiplier = initialRateOverDistanceMultiplier;
				if ((bool)particleFadeGroup)
				{
					particleFadeGroup.UpdateParticlesArraySize();
				}
			});
		}
	}

	public static void HandleDamagerSpawn(GameObject owner, GameObject spawned)
	{
		BlackThreadState component = owner.GetComponent<BlackThreadState>();
		if (!component || !component.IsVisiblyThreaded)
		{
			return;
		}
		CustomTag component2 = spawned.GetComponent<CustomTag>();
		if (((bool)component2 && component2.CustomTagType == CustomTag.CustomTagTypes.PreventBlackThread) || !spawned.GetComponentInChildren<DamageHero>())
		{
			return;
		}
		if (_tempSprites == null)
		{
			_tempSprites = new List<tk2dSprite>();
		}
		try
		{
			spawned.GetComponentsInChildren(_tempSprites);
			foreach (tk2dSprite tempSprite in _tempSprites)
			{
				tempSprite.EnableKeyword("BLACKTHREAD");
			}
		}
		finally
		{
			_tempSprites.Clear();
		}
		GameObject blackThreadPooledEffect = Effects.BlackThreadPooledEffect;
		GameObject effectObj = blackThreadPooledEffect.Spawn(spawned.transform.position);
		effectObj.transform.localScale = blackThreadPooledEffect.transform.localScale;
		effectObj.transform.SetParent(spawned.transform, worldPositionStays: true);
		MatchEffectsToObject(spawned, effectObj, out var _, recycles: true);
		PlayParticleEffects playParticles = effectObj.GetComponent<PlayParticleEffects>();
		playParticles.ClearParticleSystems();
		playParticles.PlayParticleSystems();
		DamageHero[] heroDamagers = spawned.GetComponentsInChildren<DamageHero>();
		DamageHero[] array = heroDamagers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].damagePropertyFlags |= DamagePropertyFlags.Void;
		}
		RecycleResetHandler.Add(spawned, delegate(GameObject self)
		{
			effectObj.transform.SetParent(null, worldPositionStays: true);
			playParticles.StopParticleSystems();
			DamageHero[] array2 = heroDamagers;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].damagePropertyFlags &= ~DamagePropertyFlags.Void;
			}
			try
			{
				self.GetComponentsInChildren(_tempSprites);
				foreach (tk2dSprite tempSprite2 in _tempSprites)
				{
					tempSprite2.DisableKeyword("BLACKTHREAD");
				}
			}
			finally
			{
				_tempSprites.Clear();
			}
		});
	}

	public bool CheckIsBlackThreaded()
	{
		return IsVisiblyThreaded;
	}

	public void SetAttackQueued(bool value)
	{
		queuedForceAttack = value;
	}

	public float GetRecoilMultiplier()
	{
		if (!IsInAttack)
		{
			return 1f;
		}
		return 0.2f;
	}

	public void ReportWillBeThreaded()
	{
		willBeThreaded = true;
	}

	private void FindVoice()
	{
		if (voiceState != 0)
		{
			return;
		}
		bool flag = false;
		AudioEffectTag[] componentsInChildren = GetComponentsInChildren<AudioEffectTag>(includeInactive: true);
		foreach (AudioEffectTag audioEffectTag in componentsInChildren)
		{
			if (audioEffectTag.EffectType != 0)
			{
				continue;
			}
			AudioSource audioSource = audioEffectTag.AudioSource;
			if ((bool)audioSource)
			{
				if (!flag && audioSource.name == "Audio Loop Voice")
				{
					flag = true;
				}
				blackThreadVoiceSources.Add(audioSource);
			}
		}
		if (!flag)
		{
			AudioSource[] componentsInChildren2 = GetComponentsInChildren<AudioSource>(includeInactive: true);
			foreach (AudioSource audioSource2 in componentsInChildren2)
			{
				if (audioSource2.name == "Audio Loop Voice")
				{
					voiceState = VoiceState.Found;
					blackThreadVoiceSources.Add(audioSource2);
					break;
				}
			}
		}
		if (blackThreadVoiceSources.Count == 0)
		{
			voiceState = VoiceState.NotFound;
		}
		else
		{
			voiceState = VoiceState.Found;
		}
	}

	private void ChangeVoiceOutput()
	{
		FindVoice();
		if (voiceState != VoiceState.Found)
		{
			return;
		}
		AudioMixerGroup blackThreadVoiceMixerGroup = Effects.BlackThreadVoiceMixerGroup;
		if (!blackThreadVoiceMixerGroup)
		{
			return;
		}
		foreach (AudioSource blackThreadVoiceSource in blackThreadVoiceSources)
		{
			if ((bool)blackThreadVoiceSource)
			{
				blackThreadVoiceSource.outputAudioMixerGroup = blackThreadVoiceMixerGroup;
			}
		}
		voiceState = VoiceState.ReplacedMixer;
	}
}
