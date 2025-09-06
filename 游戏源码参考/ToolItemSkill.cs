using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Skill)")]
public class ToolItemSkill : ToolItemBasic
{
	[Header("Skill")]
	[SerializeField]
	private LocalisedString msgGetPrefix;

	[SerializeField]
	private Sprite promptSprite;

	[SerializeField]
	private Sprite promptGlowSprite;

	[SerializeField]
	private Sprite promptSilhouetteSprite;

	[SerializeField]
	private LocalisedString promptDescription;

	[Space]
	[SerializeField]
	private Sprite hudGlowSprite;

	[SerializeField]
	private Sprite invGlowSprite;

	public LocalisedString MsgGetPrefix => msgGetPrefix;

	public Sprite PromptSprite => promptSprite;

	public Sprite PromptGlowSprite => promptGlowSprite;

	public Sprite PromptSilhouetteSprite => promptSilhouetteSprite;

	public LocalisedString PromptDescription => promptDescription;

	public Sprite HudGlowSprite => hudGlowSprite;

	public Sprite InvGlowSprite => invGlowSprite;
}
