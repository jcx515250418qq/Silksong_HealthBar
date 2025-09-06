using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitSlidePlatform : MonoBehaviour
{
	[SerializeField]
	private PersistentIntItem persistent;

	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private TinkEffect tinker;

	[SerializeField]
	private TinkEffect[] tinkers;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private CameraShakeTarget arriveShake;

	[SerializeField]
	private PlayParticleEffects hitParticles;

	[SerializeField]
	private PlayParticleEffects slamParticles;

	[SerializeField]
	private ParticleSystem trailParticles;

	[SerializeField]
	private JitterSelfForTime jitter;

	[SerializeField]
	private DamageEnemies enemyKiller;

	[SerializeField]
	private DamageHero heroDamager;

	[SerializeField]
	private Vector2 heroDamagerDirectionalOffset;

	[SerializeField]
	private GameObject terrainCollider;

	[SerializeField]
	private AudioSource slideLoopSource;

	[SerializeField]
	private AudioEvent hitSound;

	[SerializeField]
	private AudioEvent arriveSound;

	[SerializeField]
	private AudioSource audioEventPlayer;

	[SerializeField]
	private AnimationCurve directionFailMove;

	[SerializeField]
	private float directionFailMoveDuration;

	[SerializeField]
	private float directionFailMoveMagnitude;

	[Space]
	[SerializeField]
	private HitSlidePlatformNode initialNode;

	[SerializeField]
	private List<HitSlidePlatformNode> nodes;

	private Vector2 heroDamageInitialPosition;

	private int currentNodeIndex;

	private HitSlidePlatformNode currentNode;

	private Coroutine moveRoutine;

	private float moveDuration;

	private Vector2 moveStartPosition;

	private Vector2 moveEndPosition;

	private Coroutine directionFailMoveRoutine;

	private Vector3 initialGraphicOffset;

	private float colliderOffsetL;

	private float colliderOffsetR;

	private float colliderOffsetU;

	private float colliderOffsetD;

	private bool didFirstHit;

	public UnityEvent FirstHit;

	private void OnValidate()
	{
		if ((bool)tinker)
		{
			tinkers = new TinkEffect[1] { tinker };
			tinker = null;
		}
	}

	private void Awake()
	{
		OnValidate();
		foreach (HitSlidePlatformNode node in nodes)
		{
			if (node.transform.IsChildOf(base.transform))
			{
				node.transform.SetParent(null, worldPositionStays: true);
			}
		}
		TinkEffect[] array = tinkers;
		foreach (TinkEffect tinkEffect in array)
		{
			if ((bool)tinkEffect)
			{
				tinkEffect.HitInDirection += OnHit;
			}
		}
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out int value)
			{
				value = currentNodeIndex;
			};
			persistent.OnSetSaveState += delegate(int value)
			{
				if (value < 0 && (bool)initialNode)
				{
					SetAtNode(initialNode);
				}
				else
				{
					SetAtNode(value);
				}
			};
		}
		if ((bool)enemyKiller)
		{
			enemyKiller.gameObject.SetActive(value: false);
		}
		if ((bool)heroDamager)
		{
			heroDamageInitialPosition = heroDamager.transform.localPosition;
			heroDamager.gameObject.SetActive(value: false);
		}
		if ((bool)initialNode)
		{
			initialNode.transform.SetParent(null, worldPositionStays: true);
			currentNodeIndex = -1;
			SetAtNode(initialNode);
		}
		else
		{
			SetAtNode(-1);
		}
	}

	private void Start()
	{
		BoxCollider2D component = terrainCollider.GetComponent<BoxCollider2D>();
		Vector2 size = component.size;
		Vector2 offset = component.offset;
		float x = size.x;
		float y = size.y;
		float x2 = offset.x;
		float y2 = offset.y;
		float num = x / 2f;
		float num2 = y / 2f;
		colliderOffsetL = 0f - num + x2;
		colliderOffsetR = num + x2;
		colliderOffsetD = 0f - num2 + y2;
		colliderOffsetU = num2 + y2;
	}

	private void SetAtNode(int nodeIndex)
	{
		if (nodeIndex < 0)
		{
			float num = float.MaxValue;
			Vector2 a = base.transform.position;
			for (int i = 0; i < nodes.Count; i++)
			{
				HitSlidePlatformNode hitSlidePlatformNode = nodes[i];
				if ((bool)hitSlidePlatformNode && hitSlidePlatformNode.gameObject.activeSelf)
				{
					float num2 = Vector2.Distance(a, hitSlidePlatformNode.transform.position);
					if (num2 < num)
					{
						num = num2;
						nodeIndex = i;
					}
				}
			}
		}
		currentNodeIndex = nodeIndex;
		SetAtNode(nodes[currentNodeIndex]);
	}

	private void SetAtNode(HitSlidePlatformNode node)
	{
		currentNode = node;
		base.transform.SetPosition2D(currentNode.transform.position);
	}

	private void OnHit(GameObject source, HitInstance.HitDirection hitDirection)
	{
		if (moveRoutine != null)
		{
			return;
		}
		if ((bool)hitParticles)
		{
			hitParticles.PlayParticleSystems();
		}
		if ((bool)jitter)
		{
			jitter.StartTimedJitter();
		}
		if (currentNode.IsEndNode)
		{
			return;
		}
		if (directionFailMoveRoutine != null)
		{
			jitter.transform.localPosition = initialGraphicOffset;
			StopCoroutine(directionFailMoveRoutine);
			directionFailMoveRoutine = null;
		}
		Vector3 position = source.gameObject.transform.position;
		HitInstance.HitDirection validDirection = GetValidDirection(position, hitDirection);
		if (validDirection < HitInstance.HitDirection.Left)
		{
			validDirection = GetValidDirection(source.transform.root.position, hitDirection);
		}
		float angle = DirectionUtils.GetAngle(validDirection);
		Vector2 vector = new Vector2(Mathf.Cos(angle * (MathF.PI / 180f)), Mathf.Sin(angle * (MathF.PI / 180f)));
		HitSlidePlatformNode connectedNode = currentNode.GetConnectedNode(validDirection);
		if (!connectedNode)
		{
			directionFailMoveRoutine = StartCoroutine(DirectionFailMove(vector));
			return;
		}
		if ((bool)trailParticles)
		{
			trailParticles.Play(withChildren: true);
		}
		if ((bool)enemyKiller)
		{
			enemyKiller.direction = angle;
			enemyKiller.gameObject.SetActive(value: true);
		}
		if ((bool)heroDamager)
		{
			Vector2 vector2 = heroDamagerDirectionalOffset.MultiplyElements(vector);
			heroDamager.transform.SetLocalPosition2D(heroDamageInitialPosition + vector2);
			heroDamager.gameObject.SetActive(value: true);
		}
		if ((bool)audioEventPlayer)
		{
			hitSound.SpawnAndPlayOneShot(audioEventPlayer, base.transform.position);
		}
		if ((bool)slideLoopSource)
		{
			slideLoopSource.Play();
		}
		currentNodeIndex = nodes.IndexOf(connectedNode);
		currentNode = connectedNode;
		moveRoutine = null;
		moveStartPosition = base.transform.position;
		moveEndPosition = connectedNode.transform.position;
		moveDuration = Vector2.Distance(moveStartPosition, moveEndPosition) / moveSpeed;
		moveRoutine = this.StartTimerRoutine(0f, moveDuration, delegate(float t)
		{
			base.transform.SetPosition2D(Vector2.Lerp(moveStartPosition, moveEndPosition, t));
		}, null, OnMoveEnd);
		if (!didFirstHit)
		{
			if (FirstHit != null)
			{
				FirstHit.Invoke();
			}
			didFirstHit = true;
		}
	}

	private HitInstance.HitDirection GetValidDirection(Vector3 sourcePos, HitInstance.HitDirection hitDirection)
	{
		Vector3 position = base.transform.position;
		bool flag = sourcePos.y <= position.y + colliderOffsetD;
		bool flag2 = sourcePos.y >= position.y + colliderOffsetU;
		bool flag3 = sourcePos.x <= position.x + colliderOffsetL;
		bool flag4 = sourcePos.x >= position.x + colliderOffsetR;
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
			if (flag4)
			{
				return hitDirection;
			}
			break;
		case HitInstance.HitDirection.Right:
			if (flag3)
			{
				return hitDirection;
			}
			break;
		case HitInstance.HitDirection.Up:
			if (flag)
			{
				return hitDirection;
			}
			break;
		case HitInstance.HitDirection.Down:
			if (flag2)
			{
				return hitDirection;
			}
			break;
		}
		if (flag)
		{
			return HitInstance.HitDirection.Up;
		}
		if (flag2)
		{
			return HitInstance.HitDirection.Down;
		}
		if (flag3)
		{
			return HitInstance.HitDirection.Right;
		}
		if (flag4)
		{
			return HitInstance.HitDirection.Left;
		}
		Debug.LogError("Couldn't determine valid hit direction!");
		return (HitInstance.HitDirection)(-1);
	}

	private void OnMoveEnd()
	{
		moveRoutine = null;
		if ((bool)slamParticles)
		{
			slamParticles.PlayParticleSystems();
		}
		if ((bool)jitter)
		{
			jitter.StartTimedJitter();
		}
		if ((bool)trailParticles)
		{
			trailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		arriveShake.DoShake(this);
		if ((bool)enemyKiller)
		{
			enemyKiller.gameObject.SetActive(value: false);
		}
		if ((bool)heroDamager)
		{
			heroDamager.gameObject.SetActive(value: false);
		}
		arriveSound.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)slideLoopSource)
		{
			slideLoopSource.Stop();
		}
	}

	private IEnumerator DirectionFailMove(Vector2 hitDirectionVector)
	{
		Transform trans = jitter.transform;
		initialGraphicOffset = trans.localPosition;
		if ((bool)audioEventPlayer)
		{
			arriveSound.SpawnAndPlayOneShot(audioEventPlayer, base.transform.position);
		}
		for (float elapsed = 0f; elapsed < directionFailMoveDuration; elapsed += Time.deltaTime)
		{
			float time = elapsed / directionFailMoveDuration;
			Vector2 vector = hitDirectionVector * (directionFailMove.Evaluate(time) * directionFailMoveMagnitude);
			trans.localPosition = initialGraphicOffset + (Vector3)vector;
			yield return null;
		}
		trans.localPosition = initialGraphicOffset;
		directionFailMoveRoutine = null;
	}
}
