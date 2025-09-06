using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class CrestSocketUnlockInventoryDescription : MonoBehaviour
{
	[SerializeField]
	private NestedFadeGroupSpriteRenderer slotIcon;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer leftLock;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer leftLockGlow;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer rightLock;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer rightLockGlow;

	[Space]
	[SerializeField]
	private float lockMoveX;

	[SerializeField]
	private AnimationCurve lockMoveXCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float lockJitterMagnitude;

	[Space]
	[SerializeField]
	private AudioEvent consumeAudio;

	private AudioSource spawnedConsumeAudio;

	private Vector2 leftLockInitialPosition;

	private Vector2 rightLockInitialPosition;

	private void Awake()
	{
		leftLockInitialPosition = leftLock.transform.localPosition;
		rightLockInitialPosition = rightLock.transform.localPosition;
	}

	public void SetSlotSprite(Sprite sprite, Color color)
	{
		slotIcon.Sprite = sprite;
		slotIcon.Color = color;
		leftLock.Color = color;
		leftLockGlow.BaseColor = color;
		rightLock.Color = color;
		rightLockGlow.BaseColor = color;
		SetConsumeShakeAmount(0f);
	}

	public void StartConsume()
	{
		CancelConsume();
		AudioSource spawnedSource = null;
		spawnedSource = (spawnedConsumeAudio = consumeAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, delegate
		{
			if (!(spawnedConsumeAudio != spawnedSource))
			{
				spawnedConsumeAudio = null;
			}
		}));
	}

	public void CancelConsume()
	{
		if ((bool)spawnedConsumeAudio)
		{
			spawnedConsumeAudio.Stop();
			spawnedConsumeAudio = null;
		}
	}

	public void ConsumeCompleted()
	{
		spawnedConsumeAudio = null;
	}

	public void SetConsumeShakeAmount(float t)
	{
		leftLockGlow.AlphaSelf = t;
		rightLockGlow.AlphaSelf = t;
		Vector2 vector = Random.insideUnitCircle * (t * lockJitterMagnitude);
		float num = lockMoveXCurve.Evaluate(t) * lockMoveX;
		leftLock.transform.SetLocalPosition2D(leftLockInitialPosition + vector + new Vector2(0f - num, 0f));
		rightLock.transform.SetLocalPosition2D(rightLockInitialPosition + vector + new Vector2(num, 0f));
	}
}
