using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneAppearanceRegion : MonoBehaviour
{
	[Serializable]
	public struct DustMaterials
	{
		public Material Foreground;

		public Material Background;
	}

	[Header("Hero Only")]
	[SerializeField]
	private bool affectHeroLight = true;

	[SerializeField]
	[FormerlySerializedAs("color")]
	[ModifiableProperty]
	[Conditional("affectHeroLight", true, false, false)]
	private Color heroLightColor;

	[Space]
	[SerializeField]
	private bool affectAmbientLight;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectAmbientLight", true, false, false)]
	private Color ambientLightColor;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectAmbientLight", true, false, false)]
	private float ambientLightIntensity;

	[Space]
	[SerializeField]
	private bool affectBloom;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectBloom", true, false, false)]
	[MultiPropRange(0f, 1.5f)]
	private float bloomThreshold = 0.25f;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectBloom", true, false, false)]
	[MultiPropRange(0f, 2.5f)]
	private float bloomIntensity = 0.75f;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectBloom", true, false, false)]
	[MultiPropRange(0.25f, 5.5f)]
	private float bloomBlurSize = 1f;

	[Space]
	[SerializeField]
	private bool affectSaturation;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectSaturation", true, false, false)]
	private float saturation;

	[Header("All Characters")]
	[SerializeField]
	private bool affectCharacterTint;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectCharacterTint", true, false, false)]
	private Color characterTint;

	[Space]
	[SerializeField]
	private bool affectCharacterLightDust;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectCharacterLightDust", true, false, false)]
	private Color characterLightDustColor;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectCharacterLightDust", true, false, false)]
	private DustMaterials characterLightDustMaterials;

	[Header("Common")]
	[SerializeField]
	private float fadeDuration = 1f;

	[SerializeField]
	private OverrideFloat exitFadeDuration;

	private int layerMask = -1;

	private bool isHeroInPosition;

	private static readonly Collider2D[] _tempResults = new Collider2D[100];

	private readonly HashSet<GameObject> insideObjs = new HashSet<GameObject>();

	public Color HeroLightColor => heroLightColor;

	public bool AffectAmbientLight => affectAmbientLight;

	public Color AmbientLightColor => ambientLightColor;

	public float AmbientLightIntensity => ambientLightIntensity;

	public float BloomThreshold => bloomThreshold;

	public float BloomIntensity => bloomIntensity;

	public float BloomBlurSize => bloomBlurSize;

	public bool AffectSaturation => affectSaturation;

	public float Saturation => saturation;

	public Color CharacterTintColor => characterTint;

	public Color CharacterLightDustColor => characterLightDustColor;

	public DustMaterials CharacterLightDustMaterials => characterLightDustMaterials;

	public float FadeDuration => fadeDuration;

	public float ExitFadeDuration
	{
		get
		{
			if (!exitFadeDuration.IsEnabled)
			{
				return fadeDuration;
			}
			return exitFadeDuration.Value;
		}
	}

	private void OnEnable()
	{
		if (layerMask < 0)
		{
			layerMask = Helper.GetCollidingLayerMaskForLayer(base.gameObject.layer);
		}
		HeroController hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			HeroInPosition(isFromEvent: false);
		}
		else
		{
			hc.heroInPosition += Temp;
		}
		void Temp(bool _)
		{
			if ((bool)this)
			{
				HeroInPosition(isFromEvent: true);
			}
			hc.heroInPosition -= Temp;
		}
	}

	private void HeroInPosition(bool isFromEvent)
	{
		isHeroInPosition = true;
		Collider2D[] components = GetComponents<Collider2D>();
		for (int i = 0; i < components.Length; i++)
		{
			int num = components[i].Overlap(new ContactFilter2D
			{
				useTriggers = true,
				useLayerMask = true,
				layerMask = layerMask
			}, _tempResults);
			for (int j = 0; j < num; j++)
			{
				HandleTriggerEnter(_tempResults[j], isFromEvent);
			}
		}
	}

	private void OnDisable()
	{
		if (!GameManager.UnsafeInstance)
		{
			return;
		}
		foreach (GameObject insideObj in insideObjs)
		{
			RemoveInside(insideObj);
		}
		insideObjs.Clear();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		HandleTriggerEnter(collision, forceImmediate: false);
	}

	private void HandleTriggerEnter(Collider2D collision, bool forceImmediate)
	{
		if ((bool)collision.GetComponent<HeroController>() && !isHeroInPosition)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		insideObjs.Add(gameObject);
		HeroLight component = gameObject.GetComponent<HeroLight>();
		if ((bool)component)
		{
			if (affectHeroLight)
			{
				component.AddInside(this, forceImmediate);
			}
			GameManager.instance.sm.AddInsideAppearanceRegion(this, forceImmediate);
			if (affectBloom)
			{
				GameCameras.instance.cameraController.GetComponent<BloomOptimized>().AddInside(this, forceImmediate);
			}
		}
		if (affectCharacterTint && CharacterTint.CanAdd(collision.gameObject))
		{
			(gameObject.GetComponent<CharacterTint>() ?? collision.gameObject.AddComponent<CharacterTint>()).AddInside(this, forceImmediate);
		}
		if (affectCharacterLightDust)
		{
			CharacterLightDust component2 = gameObject.GetComponent<CharacterLightDust>();
			if ((bool)component2)
			{
				component2.AddInside(this, forceImmediate);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!collision.GetComponent<HeroController>() || isHeroInPosition)
		{
			GameObject gameObject = collision.gameObject;
			RemoveInside(gameObject);
			insideObjs.Remove(gameObject);
		}
	}

	private void RemoveInside(GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		GameCameras silentInstance = GameCameras.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		HeroLight component = obj.GetComponent<HeroLight>();
		if ((bool)component)
		{
			if (affectHeroLight)
			{
				component.RemoveInside(this);
			}
			GameManager silentInstance2 = GameManager.SilentInstance;
			if ((bool)silentInstance2)
			{
				CustomSceneManager sm = silentInstance2.sm;
				if ((bool)sm)
				{
					sm.RemoveInsideAppearanceRegion(this, forceImmediate: false);
				}
			}
			if (affectBloom)
			{
				silentInstance.cameraController.GetComponent<BloomOptimized>().RemoveInside(this, forceImmediate: false);
			}
		}
		if (affectCharacterTint)
		{
			CharacterTint component2 = obj.GetComponent<CharacterTint>();
			if ((bool)component2)
			{
				component2.RemoveInside(this, forceImmediate: false);
			}
		}
		if (affectCharacterLightDust)
		{
			CharacterLightDust component3 = obj.GetComponent<CharacterLightDust>();
			if ((bool)component3)
			{
				component3.RemoveInside(this, forceImmediate: false);
			}
		}
	}
}
