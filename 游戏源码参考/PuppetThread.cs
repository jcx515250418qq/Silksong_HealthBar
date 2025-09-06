using UnityEngine;

public class PuppetThread : MonoBehaviour
{
	private tk2dSpriteAnimator animator;

	private float timer;

	private GameObject parent;

	private Transform parent_transform;

	private Rigidbody2D parent_rb;

	private bool parented;

	private float xOffset;

	private float yOffset;

	private const float tiltMax = 10f;

	private const float tiltFactor = -10f;

	private const float rotationSpeed = 45f;

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		timer = Random.Range(2f, 4f);
	}

	private void Update()
	{
		if (!parented)
		{
			return;
		}
		if (parent == null)
		{
			base.gameObject.SetActive(value: false);
		}
		if (timer < 0f)
		{
			float num = Random.Range(1, 90);
			if (num < 31f)
			{
				animator.PlayFromFrame("String 1", 0);
			}
			else if (num < 61f)
			{
				animator.PlayFromFrame("String 2", 0);
			}
			else
			{
				animator.PlayFromFrame("String 3", 0);
			}
			timer = Random.Range(2f, 4f);
		}
		else
		{
			timer -= Time.deltaTime;
		}
		DoTilt();
		DoPosition();
	}

	public void SetParent(GameObject newParent, float new_xOffset, float new_yOffset)
	{
		parent = newParent;
		parent_rb = newParent.GetComponent<Rigidbody2D>();
		parent_transform = newParent.GetComponent<Transform>();
		xOffset = new_xOffset;
		yOffset = new_yOffset;
		parented = true;
	}

	private void DoTilt()
	{
		if (!(parent_rb != null))
		{
			return;
		}
		float num = parent_rb.linearVelocity.x * -10f;
		if (num > 10f)
		{
			num = 10f;
		}
		if (num < -10f)
		{
			num = -10f;
		}
		float num2 = num - base.transform.localEulerAngles.z;
		if ((num2 < 0f) ? (num2 < -180f) : (!(num2 > 180f)))
		{
			base.transform.Rotate(0f, 0f, 45f * Time.deltaTime);
			if (base.transform.localEulerAngles.z > num)
			{
				base.transform.localEulerAngles = new Vector3(base.transform.rotation.x, base.transform.rotation.y, num);
			}
		}
		else
		{
			base.transform.Rotate(0f, 0f, -45f * Time.deltaTime);
			if (base.transform.localEulerAngles.z < num)
			{
				base.transform.localEulerAngles = new Vector3(base.transform.rotation.x, base.transform.rotation.y, num);
			}
		}
	}

	private void DoPosition()
	{
		if (parent_transform != null)
		{
			base.transform.position = new Vector3(parent_transform.position.x + xOffset, parent_transform.position.y + yOffset, 0.009123f);
		}
	}
}
