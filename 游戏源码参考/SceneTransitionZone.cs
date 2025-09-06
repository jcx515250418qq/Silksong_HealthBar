using UnityEngine;

public class SceneTransitionZone : SceneTransitionZoneBase
{
	[SerializeField]
	private string targetScene;

	[SerializeField]
	private string targetGate;

	protected override string TargetScene => targetScene;

	protected override string TargetGate => targetGate;
}
