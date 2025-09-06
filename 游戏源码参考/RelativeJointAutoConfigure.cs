using UnityEngine;

[RequireComponent(typeof(RelativeJoint2D))]
public class RelativeJointAutoConfigure : MonoBehaviour
{
	private RelativeJoint2D joint;

	private void Awake()
	{
		joint = GetComponent<RelativeJoint2D>();
		joint.autoConfigureOffset = true;
	}

	private void Start()
	{
		joint.autoConfigureOffset = false;
	}
}
