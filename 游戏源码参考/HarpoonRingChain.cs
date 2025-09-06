using UnityEngine;

public class HarpoonRingChain : MonoBehaviour
{
	[SerializeField]
	private ChainPushReaction chainInteraction;

	[SerializeField]
	private Rigidbody2D ringBody;

	[SerializeField]
	private Vector2 needleConnectImpulse;

	[SerializeField]
	private Vector2 heroAttachImpulse;

	[SerializeField]
	private Vector2 heroLeaveImpulse;

	[SerializeField]
	private Vector2 needleHitImpulse;

	[Space]
	[SerializeField]
	private bool isSnowy;

	[SerializeField]
	private GameObject ringObject;

	[SerializeField]
	private MeshRenderer chainRenderer;

	[SerializeField]
	private Material chainMatDefault;

	[SerializeField]
	private Material chainMatSnowy;

	private void OnValidate()
	{
		UpdateRingDisplay();
		UpdateChainDisplay();
	}

	private void Awake()
	{
		ReplaceWithTemplate[] componentsInChildren = GetComponentsInChildren<ReplaceWithTemplate>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Awake();
		}
		if ((bool)ringBody)
		{
			DisableBacking(ringBody.transform);
		}
	}

	private void Start()
	{
		UpdateRingDisplay();
		UpdateChainDisplay();
		if (!isSnowy)
		{
			return;
		}
		IdleForceAnimator component = GetComponent<IdleForceAnimator>();
		if (!component)
		{
			return;
		}
		component.SpeedMultiplier *= 5f;
		Vector2 a = base.transform.position;
		float num = float.MaxValue;
		UmbrellaWindRegion umbrellaWindRegion = null;
		foreach (UmbrellaWindRegion item in UmbrellaWindRegion.EnumerateActiveRegions())
		{
			Vector2 b = item.transform.position;
			float num2 = Vector2.Distance(a, b);
			if (!(num2 > num))
			{
				num = num2;
				umbrellaWindRegion = item;
			}
		}
		if (umbrellaWindRegion != null)
		{
			component.ExtraHorizontalSpeed += umbrellaWindRegion.SpeedX * umbrellaWindRegion.SpeedMultiplier;
		}
	}

	private void UpdateRingDisplay()
	{
		if (!ringObject)
		{
			return;
		}
		string text = (isSnowy ? "Idle Snowy" : "Idle");
		tk2dSpriteAnimator componentInChildren = ringObject.GetComponentInChildren<tk2dSpriteAnimator>();
		if ((bool)componentInChildren && !(componentInChildren.DefaultClip.name == text))
		{
			int clipIdByName = componentInChildren.GetClipIdByName(text);
			if (clipIdByName >= 0)
			{
				componentInChildren.DefaultClipId = clipIdByName;
				tk2dSpriteAnimationClip[] clips = componentInChildren.Library.clips;
				componentInChildren.Sprite.SetSprite(clips[clipIdByName].frames[0].spriteCollection, clips[clipIdByName].frames[0].spriteId);
			}
		}
	}

	private void UpdateChainDisplay()
	{
		if ((bool)chainRenderer)
		{
			Material material = (isSnowy ? chainMatSnowy : chainMatDefault);
			if (!(chainRenderer.sharedMaterial == material))
			{
				chainRenderer.sharedMaterial = material;
			}
		}
	}

	private void DisableBacking(Transform parent)
	{
		foreach (Transform item in parent)
		{
			if (item.gameObject.name == "Backing")
			{
				item.gameObject.SetActive(value: false);
			}
			else
			{
				DisableBacking(item);
			}
		}
	}

	public void HeroNeedleConnect()
	{
		if ((bool)chainInteraction)
		{
			chainInteraction.DisableLinks(HeroController.instance.transform);
		}
		ApplyImpulse(needleConnectImpulse);
	}

	public void HeroOnRing()
	{
		ApplyImpulse(heroAttachImpulse);
	}

	public void HeroOffRing()
	{
		ApplyImpulse(heroLeaveImpulse);
	}

	public void DamageHit(float hitDirection)
	{
		ApplyImpulse(needleHitImpulse);
	}

	private void ApplyImpulse(Vector2 impulse)
	{
		if ((bool)ringBody)
		{
			float x = HeroController.instance.transform.position.x;
			float x2 = ringBody.position.x;
			if (x > x2)
			{
				impulse.x *= -1f;
			}
			ringBody.AddForce(impulse, ForceMode2D.Impulse);
		}
	}
}
