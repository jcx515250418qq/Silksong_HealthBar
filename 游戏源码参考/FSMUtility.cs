using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public static class FSMUtility
{
	public abstract class CheckFsmStateAction : FsmStateAction
	{
		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		public abstract bool IsTrue { get; }

		public override void Reset()
		{
			trueEvent = null;
			falseEvent = null;
		}

		public override void OnEnter()
		{
			bool isTrue = IsTrue;
			storeValue.Value = isTrue;
			if (isTrue)
			{
				base.Fsm.Event(trueEvent);
			}
			else
			{
				base.Fsm.Event(falseEvent);
			}
			Finish();
		}
	}

	public abstract class CheckFsmStateEveryFrameAction : FsmStateAction
	{
		public FsmEvent TrueEvent;

		public FsmEvent FalseEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public bool EveryFrame;

		public abstract bool IsTrue { get; }

		public override void Reset()
		{
			TrueEvent = null;
			FalseEvent = null;
			StoreValue = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			bool isTrue = IsTrue;
			StoreValue.Value = isTrue;
			if (isTrue)
			{
				base.Fsm.Event(TrueEvent);
			}
			else
			{
				base.Fsm.Event(FalseEvent);
			}
		}
	}

	public abstract class GetIntFsmStateAction : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;

		public abstract int IntValue { get; }

		public override void Reset()
		{
			storeValue = null;
		}

		public override void OnEnter()
		{
			if (!storeValue.IsNone)
			{
				storeValue.Value = IntValue;
			}
			Finish();
		}
	}

	public abstract class SetBoolFsmStateAction : FsmStateAction
	{
		public FsmBool setValue;

		public abstract bool BoolValue { set; }

		public override void Reset()
		{
			setValue = null;
		}

		public override void OnEnter()
		{
			if (!setValue.IsNone)
			{
				BoolValue = setValue.Value;
			}
			Finish();
		}
	}

	public abstract class GetComponentFsmStateAction<T> : FsmStateAction where T : Component
	{
		public FsmOwnerDefault Target;

		protected virtual bool AutoFinish => true;

		protected virtual bool LogMissingComponent => false;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				T component = safe.GetComponent<T>();
				if (component != null)
				{
					DoAction(component);
				}
				else
				{
					DoActionNoComponent(safe);
				}
			}
			if (AutoFinish)
			{
				Finish();
			}
		}

		protected abstract void DoAction(T component);

		protected virtual void DoActionNoComponent(GameObject target)
		{
		}
	}

	private const int FsmListPoolSizeMax = 20;

	private static List<List<PlayMakerFSM>> fsmListPool = new List<List<PlayMakerFSM>>();

	private static List<PlayMakerFSM> ObtainFsmList()
	{
		if (fsmListPool.Count > 0)
		{
			List<PlayMakerFSM> result = fsmListPool[fsmListPool.Count - 1];
			fsmListPool.RemoveAt(fsmListPool.Count - 1);
			return result;
		}
		return new List<PlayMakerFSM>();
	}

	private static void ReleaseFsmList(List<PlayMakerFSM> fsmList)
	{
		fsmList.Clear();
		if (fsmListPool.Count < 20)
		{
			fsmListPool.Add(fsmList);
		}
	}

	public static bool ContainsFSM(GameObject go, string fsmName)
	{
		if (go == null)
		{
			return false;
		}
		List<PlayMakerFSM> list = ObtainFsmList();
		go.GetComponents(list);
		bool result = false;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].FsmName == fsmName)
			{
				result = true;
				break;
			}
		}
		ReleaseFsmList(list);
		return result;
	}

	public static PlayMakerFSM LocateFSM(GameObject go, string fsmName)
	{
		if (go == null)
		{
			return null;
		}
		List<PlayMakerFSM> list = ObtainFsmList();
		go.GetComponents(list);
		PlayMakerFSM result = null;
		for (int i = 0; i < list.Count; i++)
		{
			PlayMakerFSM playMakerFSM = list[i];
			if (playMakerFSM.FsmName == fsmName)
			{
				result = playMakerFSM;
				break;
			}
		}
		ReleaseFsmList(list);
		return result;
	}

	public static PlayMakerFSM LocateMyFSM(this GameObject go, string fsmName)
	{
		return LocateFSM(go, fsmName);
	}

	public static PlayMakerFSM GetFSM(GameObject go)
	{
		return go.GetComponent<PlayMakerFSM>();
	}

	public static void SendEventToGameObject(GameObject go, string eventName, bool isRecursive = false)
	{
		if (go != null)
		{
			SendEventToGameObject(go, FsmEvent.FindEvent(eventName), isRecursive);
		}
	}

	public static void SendEventToGameObject(GameObject go, FsmEvent ev, bool isRecursive = false)
	{
		if (!(go != null))
		{
			return;
		}
		List<PlayMakerFSM> list = ObtainFsmList();
		go.GetComponents(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Fsm.Event(ev);
		}
		ReleaseFsmList(list);
		if (isRecursive)
		{
			Transform transform = go.transform;
			for (int j = 0; j < transform.childCount; j++)
			{
				SendEventToGameObject(transform.GetChild(j).gameObject, ev, isRecursive);
			}
		}
	}

	public static void SendEventUpwards(GameObject go, string eventName)
	{
		if (go != null)
		{
			SendEventUpwards(go, FsmEvent.FindEvent(eventName));
		}
	}

	public static void SendEventUpwards(GameObject go, FsmEvent ev)
	{
		if (!(go != null))
		{
			return;
		}
		List<PlayMakerFSM> list = ObtainFsmList();
		go.GetComponents(list);
		foreach (PlayMakerFSM item in list)
		{
			item.Fsm.Event(ev);
		}
		ReleaseFsmList(list);
		if ((bool)go.transform.parent)
		{
			SendEventUpwards(go.transform.parent.gameObject, ev);
		}
	}

	public static GameObject GetSafe(this FsmOwnerDefault ownerDefault, FsmStateAction stateAction)
	{
		if (ownerDefault.OwnerOption == OwnerDefaultOption.UseOwner)
		{
			return stateAction.Owner;
		}
		return ownerDefault.GameObject.Value;
	}

	public static T GetSafe<T>(this FsmOwnerDefault ownerDefault, FsmStateAction stateAction) where T : Component
	{
		GameObject safe = ownerDefault.GetSafe(stateAction);
		if (safe != null)
		{
			return safe.GetComponent<T>();
		}
		return null;
	}

	public static T GetSafe<T>(this FsmGameObject ownerDefault) where T : Component
	{
		GameObject value = ownerDefault.Value;
		if (value != null)
		{
			return value.GetComponent<T>();
		}
		return null;
	}

	public static bool GetBool(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmBool(variableName).Value;
	}

	public static int GetInt(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmInt(variableName).Value;
	}

	public static float GetFloat(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmFloat(variableName).Value;
	}

	public static string GetString(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmString(variableName).Value;
	}

	public static Vector3 GetVector3(PlayMakerFSM fsm, string variableName)
	{
		return fsm.FsmVariables.FindFsmVector3(variableName).Value;
	}

	public static void SetBool(PlayMakerFSM fsm, string variableName, bool value)
	{
		fsm.FsmVariables.GetFsmBool(variableName).Value = value;
	}

	public static void SetBoolOnGameObjectFSMs(GameObject go, string variableName, bool value)
	{
		if (go == null)
		{
			return;
		}
		List<PlayMakerFSM> list = ObtainFsmList();
		go.GetComponents(list);
		foreach (PlayMakerFSM item in list)
		{
			SetBool(item, variableName, value);
		}
		ReleaseFsmList(list);
	}

	public static void SetInt(PlayMakerFSM fsm, string variableName, int value)
	{
		fsm.FsmVariables.GetFsmInt(variableName).Value = value;
	}

	public static void SetFloat(PlayMakerFSM fsm, string variableName, float value)
	{
		fsm.FsmVariables.GetFsmFloat(variableName).Value = value;
	}

	public static void SetString(PlayMakerFSM fsm, string variableName, string value)
	{
		fsm.FsmVariables.GetFsmString(variableName).Value = value;
	}

	public static PlayMakerFSM FindFSMWithPersistentBool(PlayMakerFSM[] fsmArray)
	{
		for (int i = 0; i < fsmArray.Length; i++)
		{
			if (fsmArray[i].FsmVariables.FindFsmBool("Activated") != null)
			{
				return fsmArray[i];
			}
		}
		return null;
	}

	public static PlayMakerFSM FindFSMWithPersistentInt(PlayMakerFSM[] fsmArray)
	{
		for (int i = 0; i < fsmArray.Length; i++)
		{
			if (fsmArray[i].FsmVariables.FindFsmInt("Value") != null)
			{
				return fsmArray[i];
			}
		}
		return null;
	}

	public static void SendEventToGlobalGameObject(string gameObjectName, string eventName)
	{
		FsmGameObject fsmGameObject = FsmVariables.GlobalVariables.GetFsmGameObject(gameObjectName);
		if (fsmGameObject != null && fsmGameObject.Value != null)
		{
			SendEventToGameObject(fsmGameObject.Value, eventName);
		}
		else
		{
			Debug.LogError($"Could not send {eventName} to {gameObjectName}, GameObject could not be found in global variables!");
		}
	}
}
