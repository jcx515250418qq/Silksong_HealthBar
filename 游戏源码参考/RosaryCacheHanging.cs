using UnityEngine;
using UnityEngine.Serialization;

public class RosaryCacheHanging : RosaryCache
{
	[SerializeField]
	[FormerlySerializedAs("lowestBody")]
	private Rigidbody2D reactToHit;

	[SerializeField]
	private float hitForce = 10f;

	[SerializeField]
	private Vector2 forcePositionOffset;

	protected override void RespondToHit(HitInstance damageInstance, Vector2 hitPos)
	{
		Vector2 force = damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular) * hitForce;
		if ((bool)reactToHit)
		{
			reactToHit.AddForceAtPosition(force, reactToHit.transform.TransformPoint(forcePositionOffset), ForceMode2D.Impulse);
		}
	}
}
