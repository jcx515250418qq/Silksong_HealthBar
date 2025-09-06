using UnityEngine;

public class MoveOffsetFollowLooped : MonoBehaviour
{
	[SerializeField]
	private Transform followLocal;

	[SerializeField]
	private Vector2 minPos;

	[SerializeField]
	private Vector2 maxPos;

	private Vector2 previousFollowPos;

	private void Start()
	{
		if ((bool)followLocal)
		{
			previousFollowPos = followLocal.localPosition;
		}
	}

	private void Update()
	{
		if ((bool)followLocal)
		{
			Vector2 vector = followLocal.localPosition;
			Vector2 vector2 = vector - previousFollowPos;
			Vector2 position = (Vector2)base.transform.localPosition + vector2;
			if (position.x > maxPos.x)
			{
				position.x = minPos.x;
			}
			if (position.y > maxPos.y)
			{
				position.y = minPos.y;
			}
			if (position.x < minPos.x)
			{
				position.x = maxPos.x;
			}
			if (position.y < minPos.y)
			{
				position.y = maxPos.y;
			}
			base.transform.SetLocalPosition2D(position);
			previousFollowPos = vector;
		}
	}
}
