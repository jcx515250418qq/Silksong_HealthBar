using System.Collections.Generic;
using UnityEngine;

public class GrimmEnemyRange : MonoBehaviour
{
	public List<GameObject> enemyList;

	private void OnEnable()
	{
		ClearEnemyList();
	}

	public bool IsEnemyInRange()
	{
		if (enemyList.Count == 0)
		{
			return false;
		}
		return true;
	}

	public GameObject GetTarget()
	{
		GameObject result = null;
		float num = 99999f;
		if (enemyList.Count > 0)
		{
			for (int num2 = enemyList.Count - 1; num2 > -1; num2--)
			{
				if (enemyList[num2] == null || !enemyList[num2].activeSelf)
				{
					enemyList.RemoveAt(num2);
				}
			}
			foreach (GameObject enemy in enemyList)
			{
				if (!Physics2D.Linecast(base.transform.position, enemy.transform.position, 256))
				{
					float sqrMagnitude = (base.transform.position - enemy.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						result = enemy;
						num = sqrMagnitude;
					}
				}
			}
		}
		return result;
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		enemyList.Add(otherCollider.gameObject);
	}

	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		enemyList.Remove(otherCollider.gameObject);
	}

	public void ClearEnemyList()
	{
		enemyList.Clear();
	}
}
