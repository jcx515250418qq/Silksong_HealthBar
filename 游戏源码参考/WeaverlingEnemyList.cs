using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class WeaverlingEnemyList : MonoBehaviour
{
	[ActionCategory("Hollow Knight")]
	public class GetEnemyTarget : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault listHolder;

		public FsmGameObject storeTarget;

		public override void Reset()
		{
			listHolder = new FsmOwnerDefault();
			storeTarget = new FsmGameObject();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((listHolder.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : listHolder.GameObject.Value);
			if (gameObject != null)
			{
				WeaverlingEnemyList component = gameObject.GetComponent<WeaverlingEnemyList>();
				if (component != null)
				{
					storeTarget.Value = component.GetTarget();
				}
			}
			Finish();
		}
	}

	public List<GameObject> enemyList;

	private void OnEnable()
	{
		enemyList.Clear();
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		enemyList.Add(otherCollider.gameObject);
	}

	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		enemyList.Remove(otherCollider.gameObject);
	}

	public GameObject GetTarget()
	{
		if (enemyList.Count > 0)
		{
			return enemyList[Random.Range(0, enemyList.Count)];
		}
		return null;
	}
}
