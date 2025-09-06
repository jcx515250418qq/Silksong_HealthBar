using System.Collections.Generic;
using UnityEngine;

public class FrostRegion : TrackTriggerObjects
{
	public const float BASE_WARMTH = 100f;

	public const float FROST_SPEED_MULT = 1f;

	public const float CLOAKLESS_FROST_MULT = 2f;

	public const float FEATHERED_CLOAK_CAP = 0.035f;

	public const float HERO_DAMAGE_TIME = 1.75f;

	public const float FIRE_IMBUEMENT_ADD_FROST = -25f;

	public const float FIRE_IMBUEMENT_MULTIPLIER = 0.7f;

	public const float WARRIOR_RAGE_ADD_FROST = -15f;

	public const float WARRIOR_RAGE_MULTIPLIER = 0.9f;

	public const float WISP_LANTERN_MULTIPLIER = 0.8f;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingProfile", false, true, false)]
	private float frostSpeed;

	[SerializeField]
	[AssetPickerDropdown]
	private FrostSpeedProfile frostSpeedProfile;

	[SerializeField]
	[Range(-100f, 100f)]
	private float addOnEnter;

	private static readonly List<FrostRegion> _activeRegions = new List<FrostRegion>();

	public float FrostSpeed
	{
		get
		{
			if (!IsUsingProfile())
			{
				return frostSpeed;
			}
			return frostSpeedProfile.FrostSpeed;
		}
	}

	public static List<FrostRegion> FrostRegions => _activeRegions;

	private bool IsUsingProfile()
	{
		return frostSpeedProfile;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		_activeRegions.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_activeRegions.Remove(this);
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		if (isInside && Mathf.Abs(addOnEnter) > Mathf.Epsilon)
		{
			HeroController.instance.AddFrost(addOnEnter);
		}
	}

	public static IEnumerable<FrostRegion> EnumerateInsideRegions()
	{
		foreach (FrostRegion activeRegion in _activeRegions)
		{
			if (activeRegion.IsInside)
			{
				yield return activeRegion;
			}
		}
	}
}
