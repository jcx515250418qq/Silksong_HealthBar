using UnityEngine;
using UnityEngine.Events;

public class ConstrainPosition : MonoBehaviour
{
	public bool constrainX;

	public float xMin;

	public float xMax;

	public bool constrainY;

	public float yMin;

	public float yMax;

	public bool cutVelocityOnConstrain;

	public bool localSpace;

	private Rigidbody2D rb;

	[Space]
	public UnityEvent OnConstrained;

	private void Awake()
	{
		if (cutVelocityOnConstrain)
		{
			rb = GetComponent<Rigidbody2D>();
		}
	}

	private void LateUpdate()
	{
		Vector3 vector = ((!localSpace) ? base.transform.position : base.transform.localPosition);
		bool flag = false;
		if (constrainX)
		{
			if (vector.x < xMin)
			{
				vector.x = xMin;
				flag = true;
			}
			else if (vector.x > xMax)
			{
				vector.x = xMax;
				flag = true;
			}
		}
		if (constrainY)
		{
			if (vector.y < yMin)
			{
				vector.y = yMin;
				flag = true;
			}
			else if (vector.y > yMax)
			{
				vector.y = yMax;
				flag = true;
			}
		}
		if (flag && !(Time.timeScale <= Mathf.Epsilon))
		{
			if (localSpace)
			{
				base.transform.localPosition = vector;
			}
			else
			{
				base.transform.position = vector;
			}
			if (cutVelocityOnConstrain)
			{
				rb.linearVelocity = new Vector2(0f, 0f);
			}
			if (OnConstrained != null)
			{
				OnConstrained.Invoke();
			}
		}
	}

	public void SetXMin(float newValue)
	{
		xMin = newValue;
	}

	public void SetXMax(float newValue)
	{
		xMax = newValue;
	}

	public void SetYMin(float newValue)
	{
		yMin = newValue;
	}

	public void SetYMax(float newValue)
	{
		yMax = newValue;
	}

	public void StartConstrainX()
	{
		constrainX = true;
	}

	public void EndConstrainX()
	{
		constrainX = false;
	}

	public void StartConstrainY()
	{
		constrainY = true;
	}

	public void EndConstrainY()
	{
		constrainY = false;
	}

	public void EndConstrain()
	{
		constrainX = false;
		constrainY = false;
	}
}
