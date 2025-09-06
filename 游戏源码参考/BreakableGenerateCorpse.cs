using System;
using UnityEngine;

public class BreakableGenerateCorpse : MonoBehaviour
{
	[SerializeField]
	private Breakable breakable;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasCorpseObject", false, true, false)]
	private GameObject corpsePrefab;

	[SerializeField]
	private GameObject corpseObject;

	[SerializeField]
	private bool corpseFacesRight;

	[SerializeField]
	private Vector2 corpseOffset;

	[SerializeField]
	private float corpseFlingSpeed;

	[SerializeField]
	private GameObject placedCorpseObject;

	private bool HasCorpseObject()
	{
		return corpseObject;
	}

	private void Awake()
	{
		if (!corpseObject)
		{
			corpseObject = UnityEngine.Object.Instantiate(corpsePrefab);
		}
		ActiveCorpse component = corpseObject.GetComponent<ActiveCorpse>();
		if ((bool)component)
		{
			component.SetBlockAudio(blockAudio: true);
			corpseObject.SetActive(value: true);
			component.SetBlockAudio(blockAudio: false);
		}
		else
		{
			corpseObject.SetActive(value: true);
		}
		corpseObject.SetActive(value: false);
		if ((bool)placedCorpseObject)
		{
			placedCorpseObject.SetActive(value: true);
			placedCorpseObject.SetActive(value: false);
		}
		breakable.BrokenHit += FlingCorpse;
		breakable.AlreadyBroken += PlaceCorpse;
	}

	private void FlingCorpse(HitInstance hit)
	{
		Vector3 position = base.transform.TransformPoint(corpseOffset);
		position.z = UnityEngine.Random.Range(0.008f, 0.009f);
		corpseObject.transform.position = position;
		corpseObject.SetActive(value: true);
		float num = corpseFlingSpeed;
		float num2 = hit.GetMagnitudeMultForType(HitInstance.TargetType.Corpse);
		float num3 = corpseObject.transform.localScale.x * (corpseFacesRight ? 1f : (-1f));
		float num4 = 60f;
		float num5 = 120f;
		if (num2 > 1.25f)
		{
			num4 = 45f;
			num5 = 135f;
		}
		float num6;
		switch (hit.GetActualHitDirection(base.transform, HitInstance.TargetType.Corpse))
		{
		case HitInstance.HitDirection.Right:
			num6 = num4;
			corpseObject.transform.SetScaleX((0f - num3) * Mathf.Sign(base.transform.localScale.x));
			break;
		case HitInstance.HitDirection.Left:
			num6 = num5;
			corpseObject.transform.SetScaleX(num3 * Mathf.Sign(base.transform.localScale.x));
			break;
		case HitInstance.HitDirection.Down:
			num6 = 270f;
			break;
		case HitInstance.HitDirection.Up:
			num6 = UnityEngine.Random.Range(75f, 105f);
			num *= 1.3f;
			break;
		default:
			num6 = 90f;
			break;
		}
		if (num2 < 0.5f)
		{
			num2 = 0.5f;
		}
		corpseObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Cos(num6 * (MathF.PI / 180f)), Mathf.Sin(num6 * (MathF.PI / 180f))) * (num * num2);
	}

	private void PlaceCorpse()
	{
		if ((bool)placedCorpseObject)
		{
			placedCorpseObject.SetActive(value: true);
			return;
		}
		ActiveCorpse component = corpseObject.GetComponent<ActiveCorpse>();
		if ((bool)component)
		{
			component.SetBlockAudio(blockAudio: true);
			corpseObject.SetActive(value: true);
			component.SetOnGround();
			component.SetBlockAudio(blockAudio: false);
		}
		else
		{
			corpseObject.SetActive(value: true);
		}
	}
}
