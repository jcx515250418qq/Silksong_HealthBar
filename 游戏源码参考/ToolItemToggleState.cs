using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Toggle State)")]
public class ToolItemToggleState : ToolItem
{
	[Serializable]
	private class State
	{
		public Sprite InventorySprite;

		public Sprite InventorySpritePoison;

		public Sprite HudSprite;

		public Sprite HudSpritePoison;

		[Space]
		public UsageOptions Usage;
	}

	[Header("Toggle State")]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string statePdBool;

	[Space]
	[SerializeField]
	private AudioEventRandom toggleAudio;

	[Space]
	[SerializeField]
	private State offState;

	[SerializeField]
	private State onState;

	public override LocalisedString DisplayName => displayName;

	public override LocalisedString Description => description;

	public override UsageOptions Usage => CurrentState.Usage;

	private State CurrentState
	{
		get
		{
			if (!Application.isPlaying)
			{
				return onState;
			}
			if (!PlayerData.instance.GetVariable<bool>(statePdBool))
			{
				return offState;
			}
			return onState;
		}
	}

	public override bool DisplayTogglePrompt => !string.IsNullOrEmpty(statePdBool);

	public override bool CanToggle => true;

	public override Sprite GetInventorySprite(IconVariants iconVariant)
	{
		State currentState = CurrentState;
		if (iconVariant == IconVariants.Poison)
		{
			return currentState.InventorySpritePoison ? currentState.InventorySpritePoison : currentState.InventorySprite;
		}
		return currentState.InventorySprite;
	}

	public override Sprite GetHudSprite(IconVariants iconVariant)
	{
		State currentState = CurrentState;
		if (iconVariant == IconVariants.Poison)
		{
			return currentState.HudSpritePoison ? currentState.HudSpritePoison : currentState.HudSprite;
		}
		return currentState.HudSprite;
	}

	public override bool DoToggle(out bool didChangeVisually)
	{
		if (string.IsNullOrEmpty(statePdBool))
		{
			return base.DoToggle(out didChangeVisually);
		}
		PlayerData instance = PlayerData.instance;
		VariableExtensions.SetVariable(value: !instance.GetVariable<bool>(statePdBool), obj: instance, fieldName: statePdBool);
		didChangeVisually = true;
		return true;
	}

	public override void PlayToggleAudio(AudioSource audioSource)
	{
		toggleAudio.PlayOnSource(audioSource);
	}
}
