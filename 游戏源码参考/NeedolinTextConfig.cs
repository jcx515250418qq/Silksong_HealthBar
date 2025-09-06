using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Localised Text Collection Config")]
public class NeedolinTextConfig : ScriptableObject
{
	[SerializeField]
	private float holdDuration = 0.6f;

	[SerializeField]
	private float speedMultiplier = 1f;

	[Space]
	[SerializeField]
	private int emptyStartGap;

	[SerializeField]
	private MinMaxInt emptyGap;

	public float HoldDuration => holdDuration;

	public float SpeedMultiplier => speedMultiplier;

	public int EmptyStartGap => emptyStartGap;

	public MinMaxInt EmptyGap => emptyGap;
}
