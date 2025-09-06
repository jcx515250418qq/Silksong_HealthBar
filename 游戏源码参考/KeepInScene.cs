using UnityEngine;

public class KeepInScene : MonoBehaviour
{
	private GameManager gm;

	private float minX;

	private float maxX;

	private float minY = -10f;

	private float maxY;

	private void Start()
	{
		gm = GameManager.instance;
		maxX = gm.sceneWidth;
		maxY = gm.sceneHeight;
	}

	private void OnEnable()
	{
		gm = GameManager.instance;
		maxX = gm.sceneWidth;
		maxY = gm.sceneHeight;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		bool flag = false;
		if (position.x < minX)
		{
			position.x = minX;
			flag = true;
		}
		else if (position.x > maxX)
		{
			position.x = maxX;
			flag = true;
		}
		if (position.y < minY)
		{
			position.y = minY;
			flag = true;
		}
		else if (position.y > maxY)
		{
			position.y = maxY;
			flag = true;
		}
		if (flag)
		{
			base.transform.position = position;
		}
	}
}
