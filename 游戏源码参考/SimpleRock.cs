using GlobalEnums;
using UnityEngine;

public class SimpleRock : MonoBehaviour
{
	private int stepCounter;

	private bool spun;

	private Rigidbody2D rb;

	private float setZ;

	private void Start()
	{
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, Random.Range(0, 360));
		rb = GetComponent<Rigidbody2D>();
		setZ = Random.Range(base.transform.position.z, base.transform.position.z + 0.0009999f);
		base.transform.SetPositionZ(setZ);
	}

	private void FixedUpdate()
	{
		if (!spun)
		{
			if (stepCounter >= 1)
			{
				float torque = rb.linearVelocity.x * -7.5f;
				rb.AddTorque(torque);
				spun = true;
			}
			else
			{
				stepCounter++;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		PhysLayers layer = (PhysLayers)other.gameObject.layer;
		if (layer == PhysLayers.ENEMIES || layer == PhysLayers.HERO_BOX)
		{
			Vector2 force = new Vector2(Random.Range(-100f, 100f), Random.Range(-0f, 40f));
			rb.AddForce(force);
			rb.AddTorque(Random.Range(-50f, 50f));
		}
	}
}
