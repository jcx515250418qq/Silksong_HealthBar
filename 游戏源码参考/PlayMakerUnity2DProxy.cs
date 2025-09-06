using System;
using HutongGames.PlayMaker;
using UnityEngine;

public class PlayMakerUnity2DProxy : MonoBehaviour
{
	public delegate void OnCollisionEnter2dDelegate(Collision2D collisionInfo);

	public delegate void OnCollisionStay2dDelegate(Collision2D collisionInfo);

	public delegate void OnCollisionExit2dDelegate(Collision2D collisionInfo);

	public delegate void OnTriggerEnter2dDelegate(Collider2D collisionInfo);

	public delegate void OnTriggerStay2dDelegate(Collider2D collisionInfo);

	public delegate void OnTriggerExit2dDelegate(Collider2D collisionInfo);

	public bool debug;

	[HideInInspector]
	public bool HandleCollisionEnter2D;

	[HideInInspector]
	public bool HandleCollisionExit2D;

	[HideInInspector]
	public bool HandleCollisionStay2D;

	[HideInInspector]
	public bool HandleTriggerEnter2D;

	[HideInInspector]
	public bool HandleTriggerExit2D;

	[HideInInspector]
	public bool HandleTriggerStay2D;

	[HideInInspector]
	public Collision2D lastCollision2DInfo;

	[HideInInspector]
	public Collider2D lastTrigger2DInfo;

	private OnCollisionEnter2dDelegate OnCollisionEnter2dDelegates;

	private OnCollisionStay2dDelegate OnCollisionStay2dDelegates;

	private OnCollisionExit2dDelegate OnCollisionExit2dDelegates;

	private OnTriggerEnter2dDelegate OnTriggerEnter2dDelegates;

	private OnTriggerStay2dDelegate OnTriggerStay2dDelegates;

	private OnTriggerExit2dDelegate OnTriggerExit2dDelegates;

	public void AddOnCollisionEnter2dDelegate(OnCollisionEnter2dDelegate del)
	{
		OnCollisionEnter2dDelegates = (OnCollisionEnter2dDelegate)Delegate.Combine(OnCollisionEnter2dDelegates, del);
	}

	public void RemoveOnCollisionEnter2dDelegate(OnCollisionEnter2dDelegate del)
	{
		OnCollisionEnter2dDelegates = (OnCollisionEnter2dDelegate)Delegate.Remove(OnCollisionEnter2dDelegates, del);
	}

	public void AddOnCollisionStay2dDelegate(OnCollisionStay2dDelegate del)
	{
		OnCollisionStay2dDelegates = (OnCollisionStay2dDelegate)Delegate.Combine(OnCollisionStay2dDelegates, del);
	}

	public void RemoveOnCollisionStay2dDelegate(OnCollisionStay2dDelegate del)
	{
		OnCollisionStay2dDelegates = (OnCollisionStay2dDelegate)Delegate.Remove(OnCollisionStay2dDelegates, del);
	}

	public void AddOnCollisionExit2dDelegate(OnCollisionExit2dDelegate del)
	{
		OnCollisionExit2dDelegates = (OnCollisionExit2dDelegate)Delegate.Combine(OnCollisionExit2dDelegates, del);
	}

	public void RemoveOnCollisionExit2dDelegate(OnCollisionExit2dDelegate del)
	{
		OnCollisionExit2dDelegates = (OnCollisionExit2dDelegate)Delegate.Remove(OnCollisionExit2dDelegates, del);
	}

	public void AddOnTriggerEnter2dDelegate(OnTriggerEnter2dDelegate del)
	{
		OnTriggerEnter2dDelegates = (OnTriggerEnter2dDelegate)Delegate.Combine(OnTriggerEnter2dDelegates, del);
	}

	public void RemoveOnTriggerEnter2dDelegate(OnTriggerEnter2dDelegate del)
	{
		OnTriggerEnter2dDelegates = (OnTriggerEnter2dDelegate)Delegate.Remove(OnTriggerEnter2dDelegates, del);
	}

	public void AddOnTriggerStay2dDelegate(OnTriggerStay2dDelegate del)
	{
		OnTriggerStay2dDelegates = (OnTriggerStay2dDelegate)Delegate.Combine(OnTriggerStay2dDelegates, del);
	}

	public void RemoveOnTriggerStay2dDelegate(OnTriggerStay2dDelegate del)
	{
		OnTriggerStay2dDelegates = (OnTriggerStay2dDelegate)Delegate.Remove(OnTriggerStay2dDelegates, del);
	}

	public void AddOnTriggerExit2dDelegate(OnTriggerExit2dDelegate del)
	{
		OnTriggerExit2dDelegates = (OnTriggerExit2dDelegate)Delegate.Combine(OnTriggerExit2dDelegates, del);
	}

	public void RemoveOnTriggerExit2dDelegate(OnTriggerExit2dDelegate del)
	{
		OnTriggerExit2dDelegates = (OnTriggerExit2dDelegate)Delegate.Remove(OnTriggerExit2dDelegates, del);
	}

	[ContextMenu("Help")]
	public void help()
	{
		Application.OpenURL("https://hutonggames.fogbugz.com/default.asp?W1150");
	}

	public void Start()
	{
		if (!PlayMakerUnity2d.isAvailable())
		{
			Debug.LogError("PlayMakerUnity2DProxy requires the 'PlayMaker Unity 2D' Prefab in the Scene.\nUse the menu 'PlayMaker/Addons/Unity 2D/Components/Add PlayMakerUnity2D to Scene' to correct the situation", this);
			base.enabled = false;
		}
		else
		{
			RefreshImplementation();
		}
	}

	public void RefreshImplementation()
	{
		CheckGameObjectEventsImplementation();
	}

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (HandleCollisionEnter2D)
		{
			lastCollision2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnCollisionEnter2DEvent);
		}
		if (OnCollisionEnter2dDelegates != null)
		{
			OnCollisionEnter2dDelegates(coll);
		}
	}

	private void OnCollisionStay2D(Collision2D coll)
	{
		if (debug)
		{
			Debug.Log("OnCollisionStay2D " + HandleCollisionStay2D, base.gameObject);
		}
		if (HandleCollisionStay2D)
		{
			lastCollision2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnCollisionStay2DEvent);
		}
		if (OnCollisionStay2dDelegates != null)
		{
			OnCollisionStay2dDelegates(coll);
		}
	}

	private void OnCollisionExit2D(Collision2D coll)
	{
		if (debug)
		{
			Debug.Log("OnCollisionExit2D " + HandleCollisionExit2D, base.gameObject);
		}
		if (HandleCollisionExit2D)
		{
			lastCollision2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnCollisionExit2DEvent);
		}
		if (OnCollisionExit2dDelegates != null)
		{
			OnCollisionExit2dDelegates(coll);
		}
	}

	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (debug)
		{
			Debug.Log(base.gameObject.name + " OnTriggerEnter2D " + coll.gameObject.name, base.gameObject);
		}
		if (HandleTriggerEnter2D)
		{
			lastTrigger2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnTriggerEnter2DEvent);
		}
		if (OnTriggerEnter2dDelegates != null)
		{
			OnTriggerEnter2dDelegates(coll);
		}
	}

	private void OnTriggerStay2D(Collider2D coll)
	{
		if (debug)
		{
			Debug.Log(base.gameObject.name + " OnTriggerStay2D " + coll.gameObject.name, base.gameObject);
		}
		if (HandleTriggerStay2D)
		{
			lastTrigger2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnTriggerStay2DEvent);
		}
		if (OnTriggerStay2dDelegates != null)
		{
			OnTriggerStay2dDelegates(coll);
		}
	}

	private void OnTriggerExit2D(Collider2D coll)
	{
		if (debug)
		{
			Debug.Log(base.gameObject.name + " OnTriggerExit2D " + coll.gameObject.name, base.gameObject);
		}
		if (HandleTriggerExit2D)
		{
			lastTrigger2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnTriggerExit2DEvent);
		}
		if (OnTriggerExit2dDelegates != null)
		{
			OnTriggerExit2dDelegates(coll);
		}
	}

	private void CheckGameObjectEventsImplementation()
	{
		PlayMakerFSM[] components = GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM fsm in components)
		{
			CheckFsmEventsImplementation(fsm);
		}
	}

	private void CheckFsmEventsImplementation(PlayMakerFSM fsm)
	{
		FsmTransition[] fsmGlobalTransitions = fsm.FsmGlobalTransitions;
		foreach (FsmTransition fsmTransition in fsmGlobalTransitions)
		{
			CheckTransition(fsmTransition.EventName);
		}
		FsmState[] fsmStates = fsm.FsmStates;
		for (int i = 0; i < fsmStates.Length; i++)
		{
			fsmGlobalTransitions = fsmStates[i].Transitions;
			foreach (FsmTransition fsmTransition2 in fsmGlobalTransitions)
			{
				CheckTransition(fsmTransition2.EventName);
			}
		}
	}

	private void CheckTransition(string transitionName)
	{
		if (transitionName.Equals(PlayMakerUnity2d.OnCollisionEnter2DEvent))
		{
			HandleCollisionEnter2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnCollisionExit2DEvent))
		{
			HandleCollisionExit2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnCollisionStay2DEvent))
		{
			HandleCollisionStay2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnTriggerEnter2DEvent))
		{
			HandleTriggerEnter2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnTriggerExit2DEvent))
		{
			HandleTriggerExit2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnTriggerStay2DEvent))
		{
			HandleTriggerStay2D = true;
		}
	}
}
