using UnityEngine;

public class HitSlidePlatformNode : MonoBehaviour
{
	[SerializeField]
	[ArrayForEnum(typeof(HitInstance.HitDirection))]
	private HitSlidePlatformNode[] connectedNodes;

	[Space]
	[SerializeField]
	private bool isEndNode;

	public bool IsEndNode => isEndNode;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref connectedNodes, typeof(HitInstance.HitDirection));
	}

	private void OnDrawGizmos()
	{
		Vector2 vector = base.transform.position;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(vector, 0.15f);
		HitSlidePlatformNode[] array = connectedNodes;
		foreach (HitSlidePlatformNode hitSlidePlatformNode in array)
		{
			if ((bool)hitSlidePlatformNode)
			{
				Vector2 vector2 = hitSlidePlatformNode.transform.position;
				Gizmos.color = Color.white;
				Gizmos.DrawLine(vector, vector2);
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(vector, 0.3f);
			}
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	public HitSlidePlatformNode GetConnectedNode(HitInstance.HitDirection hitDirection)
	{
		return connectedNodes[(int)hitDirection];
	}
}
