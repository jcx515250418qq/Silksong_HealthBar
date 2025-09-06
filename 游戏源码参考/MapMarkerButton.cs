using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class MapMarkerButton : MonoBehaviour
{
	public enum DisableType
	{
		GameObject = 0,
		SpriteRenderer = 1,
		NestedFadeGroup = 2
	}

	public int neededMarkerTypes = 2;

	public DisableType disable;

	public bool keepDisabled;

	private bool shouldDisable;

	private SpriteRenderer spriteRenderer;

	private NestedFadeGroupBase fade;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		fade = GetComponent<NestedFadeGroupBase>();
	}

	private void OnEnable()
	{
		PlayerData playerData = GameManager.instance.playerData;
		if (playerData != null)
		{
			if ((bool)fade)
			{
				fade.AlphaSelf = 1f;
			}
			if ((playerData.hasMarker_a ? 1 : 0) + (playerData.hasMarker_b ? 1 : 0) + (playerData.hasMarker_c ? 1 : 0) + (playerData.hasMarker_d ? 1 : 0) + (playerData.hasMarker_e ? 1 : 0) < neededMarkerTypes)
			{
				DoDisable();
				shouldDisable = true;
			}
			else
			{
				shouldDisable = false;
			}
		}
	}

	private void Update()
	{
		if (keepDisabled && shouldDisable)
		{
			DoDisable();
		}
	}

	private void DoDisable()
	{
		switch (disable)
		{
		case DisableType.GameObject:
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
			break;
		case DisableType.SpriteRenderer:
			if ((bool)spriteRenderer && spriteRenderer.enabled)
			{
				spriteRenderer.enabled = false;
			}
			break;
		case DisableType.NestedFadeGroup:
			if ((bool)fade)
			{
				fade.AlphaSelf = 0f;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
