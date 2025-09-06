using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMakerUnity2d : MonoBehaviour
{
	public enum Collision2DType
	{
		OnCollisionEnter2D = 0,
		OnCollisionStay2D = 1,
		OnCollisionExit2D = 2
	}

	public enum Trigger2DType
	{
		OnTriggerEnter2D = 0,
		OnTriggerStay2D = 1,
		OnTriggerExit2D = 2
	}

	private static PlayMakerFSM fsmProxy;

	public static string PlayMakerUnity2dProxyName = "PlayMaker Unity 2D";

	private static FsmOwnerDefault goTarget;

	public static string OnCollisionEnter2DEvent = "COLLISION ENTER 2D";

	public static string OnCollisionExit2DEvent = "COLLISION EXIT 2D";

	public static string OnCollisionStay2DEvent = "COLLISION STAY 2D";

	public static string OnTriggerEnter2DEvent = "TRIGGER ENTER 2D";

	public static string OnTriggerExit2DEvent = "TRIGGER EXIT 2D";

	public static string OnTriggerStay2DEvent = "TRIGGER STAY 2D";

	private static Dictionary<Fsm, RaycastHit2D> lastRaycastHit2DInfoLUT;

	public static void RecordLastRaycastHitInfo(Fsm fsm, RaycastHit2D info)
	{
		if (lastRaycastHit2DInfoLUT == null)
		{
			lastRaycastHit2DInfoLUT = new Dictionary<Fsm, RaycastHit2D>();
		}
		lastRaycastHit2DInfoLUT[fsm] = info;
	}

	public static RaycastHit2D GetLastRaycastHitInfo(Fsm fsm)
	{
		if (lastRaycastHit2DInfoLUT == null)
		{
			lastRaycastHit2DInfoLUT[fsm] = default(RaycastHit2D);
			return lastRaycastHit2DInfoLUT[fsm];
		}
		return lastRaycastHit2DInfoLUT[fsm];
	}

	private void Awake()
	{
		fsmProxy = GetComponent<PlayMakerFSM>();
		if (fsmProxy == null)
		{
			Debug.LogError("'PlayMaker Unity 2D' is missing.", this);
		}
		goTarget = new FsmOwnerDefault();
		goTarget.GameObject = new FsmGameObject();
		goTarget.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
		new FsmEventTarget
		{
			excludeSelf = false,
			target = FsmEventTarget.EventTarget.GameObject,
			gameObject = goTarget,
			sendToChildren = false
		};
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
		if (lastRaycastHit2DInfoLUT != null)
		{
			lastRaycastHit2DInfoLUT.Clear();
		}
	}

	public static bool isAvailable()
	{
		return fsmProxy != null;
	}

	public static void ForwardEventToGameObject(GameObject target, string eventName)
	{
		goTarget.GameObject.Value = target;
		FsmEventTarget fsmEventTarget = new FsmEventTarget();
		fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
		fsmEventTarget.gameObject = goTarget;
		FsmEvent fsmEvent = new FsmEvent(eventName);
		fsmProxy.Fsm.Event(fsmEventTarget, fsmEvent.Name);
	}

	public static void ForwardCollisionToCurrentState(GameObject target, Collision2DType type, Collision2D CollisionInfo)
	{
		PlayMakerFSM[] components = target.GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			FsmState fsmState = null;
			FsmState[] fsmStates = playMakerFSM.FsmStates;
			foreach (FsmState fsmState2 in fsmStates)
			{
				if (fsmState2.Name.Equals(playMakerFSM.ActiveStateName))
				{
					fsmState = fsmState2;
					break;
				}
			}
			if (fsmState == null)
			{
				continue;
			}
			FsmStateAction[] actions = fsmState.Actions;
			for (int j = 0; j < actions.Length; j++)
			{
				IFsmCollider2DStateAction fsmCollider2DStateAction = (IFsmCollider2DStateAction)actions[j];
				if (type == Collision2DType.OnCollisionEnter2D)
				{
					fsmCollider2DStateAction.DoCollisionEnter2D(CollisionInfo);
				}
			}
		}
	}
}
