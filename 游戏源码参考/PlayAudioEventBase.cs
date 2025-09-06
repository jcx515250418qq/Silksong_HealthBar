using System;
using HutongGames.PlayMaker;
using UnityEngine;

public abstract class PlayAudioEventBase : FsmStateAction
{
	public FsmFloat pitchMin;

	public FsmFloat pitchMax;

	public FsmFloat volume;

	[ObjectType(typeof(AudioSource))]
	public FsmObject audioPlayerPrefab;

	public FsmOwnerDefault spawnPoint;

	public FsmVector3 spawnPosition;

	[UIHint(UIHint.Variable)]
	public FsmGameObject SpawnedPlayerRef;

	protected AudioSource spawnedAudioSource;

	public override void Reset()
	{
		pitchMin = 1f;
		pitchMax = 1f;
		volume = 1f;
		audioPlayerPrefab = new FsmObject
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		Vector3 value = spawnPosition.Value;
		GameObject safe = spawnPoint.GetSafe(this);
		if ((bool)safe)
		{
			value += safe.transform.position;
		}
		Action onRecycle = null;
		if (!SpawnedPlayerRef.IsNone)
		{
			onRecycle = delegate
			{
				SpawnedPlayerRef.Value = null;
			};
		}
		spawnedAudioSource = SpawnAudioEvent(value, onRecycle);
		if ((bool)spawnedAudioSource && !SpawnedPlayerRef.IsNone)
		{
			SpawnedPlayerRef.Value = spawnedAudioSource.gameObject;
		}
		Finish();
	}

	public override void OnExit()
	{
		base.OnExit();
		spawnedAudioSource = null;
	}

	protected abstract AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle);
}
