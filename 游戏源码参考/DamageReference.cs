using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Reference", menuName = "Hornet/Damage Reference")]
public class DamageReference : IntReference
{
	public enum Targets
	{
		DamageEnemies = 0,
		DamageHero = 1
	}

	public const string NEW_ASSET_LOC = "Data Assets/Damages";

	[SerializeField]
	private Targets target;

	public Targets Target => target;
}
