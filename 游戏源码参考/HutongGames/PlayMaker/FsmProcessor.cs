using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace HutongGames.PlayMaker
{
	[Preserve]
	public class FsmProcessor
	{
		public static void OnPreprocess(PlayMakerFSM fsm)
		{
			if (fsm.Fsm.HandleLegacyNetworking && !AddEventHandlerComponent(fsm, ReflectionUtils.GetGlobalType("PlayMakerLegacyNetworking")))
			{
				Debug.LogError("Could not add PlayMakerLegacyNetworking proxy!");
			}
			if (fsm.Fsm.HandleUiEvents != 0)
			{
				HandleUiEvent<PlayMakerUiClickEvent>(fsm, UiEvents.Click);
				HandleUiEvent<PlayMakerUiDragEvents>(fsm, UiEvents.DragEvents);
				HandleUiEvent<PlayMakerUiDropEvent>(fsm, UiEvents.Drop);
				HandleUiEvent<PlayMakerUiPointerEvents>(fsm, UiEvents.PointerEvents);
				HandleUiEvent<PlayMakerUiBoolValueChangedEvent>(fsm, UiEvents.BoolValueChanged);
				HandleUiEvent<PlayMakerUiFloatValueChangedEvent>(fsm, UiEvents.FloatValueChanged);
				HandleUiEvent<PlayMakerUiIntValueChangedEvent>(fsm, UiEvents.IntValueChanged);
				HandleUiEvent<PlayMakerUiVector2ValueChangedEvent>(fsm, UiEvents.Vector2ValueChanged);
				HandleUiEvent<PlayMakerUiEndEditEvent>(fsm, UiEvents.EndEdit);
			}
		}

		private static void HandleUiEvent<T>(PlayMakerFSM fsm, UiEvents uiEvent) where T : PlayMakerUiEventBase
		{
			if ((fsm.Fsm.HandleUiEvents & uiEvent) != 0)
			{
				AddUiEventHandler<T>(fsm);
			}
		}

		private static void AddUiEventHandler<T>(PlayMakerFSM fsm) where T : PlayMakerUiEventBase
		{
			T val = fsm.GetComponent<T>();
			if (val == null)
			{
				val = fsm.gameObject.AddComponent<T>();
				if (!PlayMakerPrefs.ShowEventHandlerComponents)
				{
					val.hideFlags = HideFlags.HideInInspector;
				}
			}
			val.AddTargetFsm(fsm);
		}

		private static bool AddEventHandlerComponent(PlayMakerFSM fsm, Type type)
		{
			if (type == null)
			{
				return false;
			}
			PlayMakerProxyBase eventHandlerComponent = GetEventHandlerComponent(fsm.gameObject, type);
			if (eventHandlerComponent == null)
			{
				return false;
			}
			eventHandlerComponent.AddTarget(fsm);
			if (!PlayMakerGlobals.IsEditor)
			{
				_ = PlayMakerPrefs.LogPerformanceWarnings;
			}
			return true;
		}

		public static PlayMakerProxyBase GetEventHandlerComponent(GameObject go, Type type)
		{
			if (go == null)
			{
				return null;
			}
			Component component = go.GetComponent(type);
			if (component == null)
			{
				component = go.AddComponent(type);
				if (!PlayMakerPrefs.ShowEventHandlerComponents)
				{
					component.hideFlags = HideFlags.HideInInspector;
				}
			}
			return component as PlayMakerProxyBase;
		}
	}
}
