using UnityEngine;

public class EnemyEdgeControl : MonoBehaviour
{
	public Transform edgeL;

	public Transform edgeR;

	public PlayMakerFSM notifyFSM;

	private Transform selfTransform;

	private Rigidbody2D rb;

	private float leftX;

	private float rightX;

	private bool sentMsg;

	private bool stopped;

	private const float decelerationRate = 0.85f;

	private void Start()
	{
		selfTransform = base.transform;
		_ = selfTransform.position;
		leftX = (edgeL ? edgeL.position.x : 0f);
		rightX = (edgeR ? edgeR.position.x : 9999f);
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (!stopped)
		{
			float x = selfTransform.position.x;
			if ((x < leftX - 1f && rb.linearVelocity.x < 0f) || (x > rightX + 1f && rb.linearVelocity.x > 0f))
			{
				Stop();
			}
		}
	}

	private void FixedUpdate()
	{
		if (stopped)
		{
			return;
		}
		float x = selfTransform.position.x;
		if (x < leftX)
		{
			if (rb.linearVelocity.x < 0f)
			{
				Decelerate();
			}
			if (!sentMsg)
			{
				if ((bool)notifyFSM)
				{
					notifyFSM.SendEvent("EDGE");
				}
				if ((bool)notifyFSM)
				{
					notifyFSM.SendEvent("EDGE L");
				}
				sentMsg = true;
			}
		}
		else if (x > rightX)
		{
			if (rb.linearVelocity.x > 0f)
			{
				Decelerate();
			}
			if (!sentMsg)
			{
				if ((bool)notifyFSM)
				{
					notifyFSM.SendEvent("EDGE");
				}
				if ((bool)notifyFSM)
				{
					notifyFSM.SendEvent("EDGE R");
				}
				sentMsg = true;
			}
		}
		else if (sentMsg)
		{
			sentMsg = false;
		}
	}

	private void Decelerate()
	{
		Vector2 linearVelocity = rb.linearVelocity;
		if (linearVelocity.x < 0f)
		{
			linearVelocity.x *= 0.85f;
			if (linearVelocity.x > 0f)
			{
				linearVelocity.x = 0f;
			}
		}
		else if (linearVelocity.x > 0f)
		{
			linearVelocity.x *= 0.85f;
			if (linearVelocity.x < 0f)
			{
				linearVelocity.x = 0f;
			}
		}
		if (linearVelocity.x < 0.001f && linearVelocity.x > -0.001f)
		{
			linearVelocity.x = 0f;
		}
		rb.linearVelocity = linearVelocity;
	}

	private void Stop()
	{
		rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
	}

	public void StopEdgeControl()
	{
		stopped = true;
	}
}
