using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	public class EnemySingControl : ComponentAction<AudioSource>
	{
		private const float SING_DURATION_MIN = 4f;

		private const float SING_DURATION_MAX = 6.75f;

		private const float SING_DURATION_STRONG_MIN = 6.5f;

		private const float SING_DURATION_STRONG_MAX = 8f;

		private const float COLLIDER_REF_SCALE_MIN = 3f;

		private const float COLLIDER_REF_SCALE_MAX = 6f;

		private const float EFFECT_SCALE_MIN = 1f;

		private const float EFFECT_SCALE_MAX = 1.6f;

		public FsmOwnerDefault enemyGameObject;

		public FsmGameObject audioPlayer;

		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject singAudioTable;

		public FsmBool noThreadEffects;

		public FsmBool noPuppetString;

		public FsmBool randomSingStartTime;

		public FsmBool dontStopAudioOnExit;

		public FsmGameObject altThreadSpawnPoint;

		private Vector2 followOffset;

		private AudioSource audioSource;

		private GameObject possessionObj;

		private GameObject possessionObjEnd;

		private NeedolinTextOwner needolinTextOwner;

		private EnemySingDuration durationController;

		private float singDuration;

		private bool inForcedSing;

		private bool sentEndEvent;

		private bool startedJitter;

		public override void Reset()
		{
			enemyGameObject = null;
			audioPlayer = null;
			singAudioTable = null;
			noThreadEffects = null;
			noPuppetString = null;
			randomSingStartTime = null;
			dontStopAudioOnExit = null;
			startedJitter = false;
			altThreadSpawnPoint = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(enemyGameObject);
			needolinTextOwner = null;
			inForcedSing = false;
			bool flag = true;
			if (ownerDefaultTarget != null)
			{
				BlackThreadState component = ownerDefaultTarget.GetComponent<BlackThreadState>();
				if ((bool)component)
				{
					flag = false;
					if (!component.IsVisiblyThreaded && !component.IsBlackThreaded)
					{
						PlaySing();
					}
					if (component.IsInForcedSing)
					{
						inForcedSing = true;
						return;
					}
				}
				needolinTextOwner = ownerDefaultTarget.GetComponent<NeedolinTextOwner>();
				if ((bool)needolinTextOwner)
				{
					needolinTextOwner.AddNeedolinText();
				}
				if (!noThreadEffects.Value)
				{
					EnemyHitEffectsRegular component2 = ownerDefaultTarget.GetComponent<EnemyHitEffectsRegular>();
					followOffset = (component2 ? component2.EffectOrigin : Vector3.zero);
					Vector3 position = ownerDefaultTarget.transform.TransformPoint(followOffset);
					if (altThreadSpawnPoint.Value != null)
					{
						position = altThreadSpawnPoint.Value.transform.position;
					}
					bool blackThreadWorld = GameManager.instance.playerData.blackThreadWorld;
					if (!noPuppetString.Value && !blackThreadWorld)
					{
						possessionObj = Effects.SilkPossesionObjSing.Spawn(position);
					}
					else
					{
						possessionObj = Effects.SilkPossesionObjSingNoPuppet.Spawn(position);
					}
					MatchEffectsToObject(ownerDefaultTarget, possessionObj, out followOffset);
				}
				durationController = ownerDefaultTarget.GetComponent<EnemySingDuration>();
			}
			else
			{
				Debug.LogError(string.Format("{0} : {1}{2} : {3} - Missing Game Object", base.Owner, base.Fsm.Name, (base.Fsm.Template != null) ? (" : Template : " + base.Fsm.Template.name) : string.Empty, base.Fsm.ActiveStateName), base.Owner);
			}
			if (flag)
			{
				PlaySing();
			}
			if (Gameplay.MusicianCharmTool.IsEquipped)
			{
				singDuration = Random.Range(6.5f, 8f);
			}
			else
			{
				singDuration = Random.Range(4f, 6.75f);
			}
			sentEndEvent = false;
		}

		private void PlaySing()
		{
			audioSource = audioPlayer.GetSafe<AudioSource>();
			if (audioSource != null)
			{
				RandomAudioClipTable randomAudioClipTable = singAudioTable.Value as RandomAudioClipTable;
				if ((bool)randomAudioClipTable)
				{
					audioSource.clip = randomAudioClipTable.SelectClip(forcePlay: true);
					audioSource.pitch = randomAudioClipTable.SelectPitch();
					audioSource.volume = randomAudioClipTable.SelectVolume();
				}
				audioSource.Stop();
				if (randomSingStartTime.Value && audioSource.clip != null)
				{
					audioSource.time = Random.Range(0f, audioSource.clip.length);
				}
				audioSource.Play();
			}
		}

		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(enemyGameObject);
			if (!inForcedSing && !sentEndEvent)
			{
				if (singDuration > 0f)
				{
					singDuration -= Time.deltaTime;
				}
				else if (ownerDefaultTarget != null)
				{
					FSMUtility.SendEventToGameObject(ownerDefaultTarget, "SING DURATION END");
					sentEndEvent = true;
				}
			}
			if (!noThreadEffects.Value && ownerDefaultTarget != null && possessionObj != null)
			{
				if (altThreadSpawnPoint.Value != null)
				{
					possessionObj.transform.position = altThreadSpawnPoint.Value.transform.position;
				}
				else
				{
					possessionObj.transform.position = ownerDefaultTarget.transform.TransformPoint(followOffset);
				}
			}
		}

		public override void OnExit()
		{
			if (audioSource != null && !dontStopAudioOnExit.Value)
			{
				audioSource.Stop();
			}
			if (!noThreadEffects.Value && (bool)possessionObj)
			{
				possessionObj.Recycle();
				possessionObj = null;
			}
			if ((bool)needolinTextOwner)
			{
				needolinTextOwner.RemoveNeedolinText();
				needolinTextOwner = null;
			}
			if ((bool)durationController)
			{
				durationController.StartSingCooldown();
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(enemyGameObject);
			if (ownerDefaultTarget != null && !noThreadEffects.Value && !inForcedSing)
			{
				possessionObjEnd = Effects.SilkPossesionObjSingEnd.Spawn(ownerDefaultTarget.transform.position);
				possessionObjEnd.transform.parent = ownerDefaultTarget.transform;
				possessionObjEnd = null;
			}
		}

		private static void MatchEffectsToObject(GameObject gameObject, GameObject effectObj, out Vector2 centreOffset)
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
			Vector2 vector;
			Vector3 vector2;
			if (component.enabled)
			{
				UnityEngine.Bounds bounds = component.bounds;
				vector = bounds.size;
				vector2 = bounds.center;
			}
			else if (!(component is BoxCollider2D boxCollider2D))
			{
				if (!(component is CircleCollider2D circleCollider2D))
				{
					Debug.LogError("EnemySingControl \"" + gameObject.name + "\" has inactive collider that can't be manually handled!", gameObject);
					return;
				}
				float num = circleCollider2D.radius * 2f;
				vector = new Vector2(num, num);
				vector2 = transform.TransformPoint(circleCollider2D.offset);
			}
			else
			{
				vector = boxCollider2D.size;
				vector2 = transform.TransformPoint(boxCollider2D.offset);
			}
			transform2.SetPosition2D(vector2);
			centreOffset = transform.InverseTransformPoint(vector2);
			float value = Mathf.Max(vector.x, vector.y);
			MinMaxFloat minMaxFloat = new MinMaxFloat(3f, 6f);
			float lerpedValue = new MinMaxFloat(1f, 1.6f).GetLerpedValue(minMaxFloat.GetTBetween(value));
			transform2.SetScale2D(new Vector2(lerpedValue, lerpedValue));
		}
	}
}
