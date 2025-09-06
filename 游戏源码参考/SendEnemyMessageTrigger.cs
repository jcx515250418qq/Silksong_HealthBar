using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class SendEnemyMessageTrigger : MonoBehaviour, ISceneLintUpgrader
{
	[UnityEngine.Tooltip("If there is an enemy_message FSM on this gameobject, this value will be gotten from it.")]
	public string eventName = "";

	private readonly List<GameObject> enteredEnemies = new List<GameObject>();

	private void Awake()
	{
		OnSceneLintUpgrade(doUpgrade: true);
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "enemy_message");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "enemy_message FSM needs upgrading to SendEnemyMessageTrigger script";
		}
		FsmString fsmString = playMakerFSM.FsmVariables.FindFsmString("Event");
		eventName = fsmString.Value;
		Object.DestroyImmediate(playMakerFSM);
		return "enemy_message FSM was upgraded to SendEnemyMessageTrigger script";
	}

	private void FixedUpdate()
	{
		enteredEnemies.Clear();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		Rigidbody2D attachedRigidbody = collision.attachedRigidbody;
		GameObject gameObject = collision.gameObject;
		GameObject gameObject2 = (attachedRigidbody ? attachedRigidbody.gameObject : gameObject);
		if (!enteredEnemies.Contains(gameObject2))
		{
			enteredEnemies.Add(gameObject2);
			SendEvent(gameObject);
			GameObject gameObject3 = gameObject.transform.root.gameObject;
			if (gameObject3 != null && gameObject != gameObject3)
			{
				SendEvent(gameObject3);
			}
			if (gameObject2 != gameObject)
			{
				SendEvent(gameObject2);
			}
			gameObject2.GetComponent<IEnemyMessageReceiver>()?.ReceiveEnemyMessage(eventName);
		}
	}

	private void SendEvent(GameObject obj)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return;
		}
		FSMUtility.SendEventToGameObject(obj, eventName);
		if (string.IsNullOrEmpty(eventName))
		{
			return;
		}
		string text = eventName;
		if (!(text == "GO LEFT"))
		{
			if (text == "GO RIGHT")
			{
				SendWalkerGoInDirection(obj, 1);
			}
		}
		else
		{
			SendWalkerGoInDirection(obj, -1);
		}
	}

	private static void SendWalkerGoInDirection(GameObject target, int facing)
	{
		Walker component = target.GetComponent<Walker>();
		if ((bool)component)
		{
			component.RecieveGoMessage(facing);
		}
		WalkerV2 component2 = target.GetComponent<WalkerV2>();
		if ((bool)component2)
		{
			component2.ForceDirection(facing);
		}
	}
}
