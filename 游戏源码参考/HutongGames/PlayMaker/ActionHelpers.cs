using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	public static class ActionHelpers
	{
		public static RaycastHit mousePickInfo;

		private static float mousePickRaycastTime;

		private static float mousePickDistanceUsed;

		private static int mousePickLayerMaskUsed;

		public const string colon = ": ";

		public static Texture2D WhiteTexture => Texture2D.whiteTexture;

		public static Color BlendColor(ColorBlendMode blendMode, Color c1, Color c2)
		{
			switch (blendMode)
			{
			case ColorBlendMode.Normal:
				return Color.Lerp(c1, c2, c2.a);
			case ColorBlendMode.Multiply:
				return Color.Lerp(c1, c1 * c2, c2.a);
			case ColorBlendMode.Screen:
			{
				Color b = Color.white - (Color.white - c1) * (Color.white - c2);
				return Color.Lerp(c1, b, c2.a);
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static bool IsVisible(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			Renderer component = go.GetComponent<Renderer>();
			if (component != null)
			{
				return component.isVisible;
			}
			return false;
		}

		public static bool IsVisible(GameObject go, Camera camera, bool useBounds)
		{
			if (go == null || !camera)
			{
				return false;
			}
			Renderer component = go.GetComponent<Renderer>();
			if (component == null)
			{
				return false;
			}
			if (useBounds)
			{
				return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), component.bounds);
			}
			Vector3 vector = camera.WorldToViewportPoint(go.transform.position);
			if (vector.z > 0f && vector.x > 0f && vector.x < 1f && vector.y > 0f)
			{
				return vector.y < 1f;
			}
			return false;
		}

		public static GameObject GetOwnerDefault(FsmStateAction action, FsmOwnerDefault ownerDefault)
		{
			return action.Fsm.GetOwnerDefaultTarget(ownerDefault);
		}

		public static PlayMakerFSM GetGameObjectFsm(GameObject go, string fsmName)
		{
			if (!string.IsNullOrEmpty(fsmName))
			{
				PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
				foreach (PlayMakerFSM playMakerFSM in components)
				{
					if (playMakerFSM.FsmName == fsmName)
					{
						return playMakerFSM;
					}
				}
			}
			return go.GetComponent<PlayMakerFSM>();
		}

		public static int GetRandomWeightedIndex(FsmFloat[] weights)
		{
			float num = 0f;
			foreach (FsmFloat fsmFloat in weights)
			{
				num += fsmFloat.Value;
			}
			float num2 = UnityEngine.Random.Range(0f, num);
			for (int j = 0; j < weights.Length; j++)
			{
				if (num2 < weights[j].Value)
				{
					return j;
				}
				num2 -= weights[j].Value;
			}
			return -1;
		}

		public static void AddAnimationClip(GameObject go, AnimationClip animClip)
		{
			if (!(animClip == null))
			{
				Animation component = go.GetComponent<Animation>();
				if (component != null)
				{
					component.AddClip(animClip, animClip.name);
				}
			}
		}

		public static bool HasAnimationFinished(AnimationState anim, float prevTime, float currentTime)
		{
			if (anim.wrapMode == WrapMode.Loop || anim.wrapMode == WrapMode.PingPong)
			{
				return false;
			}
			if ((anim.wrapMode == WrapMode.Default || anim.wrapMode == WrapMode.Once) && prevTime > 0f && currentTime.Equals(0f))
			{
				return true;
			}
			if (prevTime < anim.length)
			{
				return currentTime >= anim.length;
			}
			return false;
		}

		public static Vector3 GetPosition(FsmGameObject fsmGameObject, FsmVector3 fsmVector3)
		{
			if (fsmGameObject.Value != null)
			{
				return (!fsmVector3.IsNone) ? fsmGameObject.Value.transform.TransformPoint(fsmVector3.Value) : fsmGameObject.Value.transform.position;
			}
			return fsmVector3.Value;
		}

		public static Vector3 GetDeviceAcceleration()
		{
			return Input.acceleration;
		}

		public static Vector3 GetMousePosition()
		{
			return Input.mousePosition;
		}

		public static bool AnyKeyDown()
		{
			return Input.anyKeyDown;
		}

		public static bool IsMouseOver(GameObject gameObject, float distance, int layerMask)
		{
			if (gameObject == null)
			{
				return false;
			}
			return gameObject == MouseOver(distance, layerMask);
		}

		public static RaycastHit MousePick(float distance, int layerMask)
		{
			if (!mousePickRaycastTime.Equals(Time.frameCount) || mousePickDistanceUsed < distance || mousePickLayerMaskUsed != layerMask)
			{
				DoMousePick(distance, layerMask);
			}
			return mousePickInfo;
		}

		public static GameObject MouseOver(float distance, int layerMask)
		{
			if (!mousePickRaycastTime.Equals(Time.frameCount) || mousePickDistanceUsed < distance || mousePickLayerMaskUsed != layerMask)
			{
				DoMousePick(distance, layerMask);
			}
			if (mousePickInfo.collider != null && mousePickInfo.distance < distance)
			{
				return mousePickInfo.collider.gameObject;
			}
			return null;
		}

		private static void DoMousePick(float distance, int layerMask)
		{
			if (!(Camera.main == null))
			{
				Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePickInfo, distance, layerMask);
				mousePickLayerMaskUsed = layerMask;
				mousePickDistanceUsed = distance;
				mousePickRaycastTime = Time.frameCount;
			}
		}

		public static int LayerArrayToLayerMask(FsmInt[] layers, bool invert)
		{
			int num = 0;
			foreach (FsmInt fsmInt in layers)
			{
				num |= 1 << fsmInt.Value;
			}
			if (invert)
			{
				num = ~num;
			}
			if (num != 0)
			{
				return num;
			}
			return -5;
		}

		public static bool IsLoopingWrapMode(WrapMode wrapMode)
		{
			if (wrapMode != WrapMode.Loop)
			{
				return wrapMode == WrapMode.PingPong;
			}
			return true;
		}

		public static string CheckRayDistance(float rayDistance)
		{
			if (!(rayDistance <= 0f))
			{
				return "";
			}
			return "Ray Distance should be greater than zero!\n";
		}

		public static string CheckForValidEvent(FsmState state, string eventName)
		{
			if (state == null)
			{
				return "Invalid State!";
			}
			if (string.IsNullOrEmpty(eventName))
			{
				return "";
			}
			FsmTransition[] globalTransitions = state.Fsm.GlobalTransitions;
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				if (globalTransitions[i].EventName == eventName)
				{
					return "";
				}
			}
			globalTransitions = state.Transitions;
			for (int i = 0; i < globalTransitions.Length; i++)
			{
				if (globalTransitions[i].EventName == eventName)
				{
					return "";
				}
			}
			return "Fsm will not respond to Event: " + eventName;
		}

		public static string CheckPhysicsSetup(FsmOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "";
			}
			return CheckPhysicsSetup(ownerDefault.GameObject.Value);
		}

		public static string CheckOwnerPhysicsSetup(GameObject gameObject)
		{
			return CheckPhysicsSetup(gameObject);
		}

		public static string CheckPhysicsSetup(GameObject gameObject)
		{
			string text = string.Empty;
			if (gameObject != null && gameObject.GetComponent<Collider>() == null && gameObject.GetComponent<Rigidbody>() == null)
			{
				text += "GameObject requires RigidBody/Collider!\n";
			}
			return text;
		}

		public static string CheckPhysics2dSetup(FsmOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "";
			}
			return CheckPhysics2dSetup(ownerDefault.GameObject.Value);
		}

		public static string CheckOwnerPhysics2dSetup(GameObject gameObject)
		{
			return CheckPhysics2dSetup(gameObject);
		}

		public static string CheckPhysics2dSetup(GameObject gameObject)
		{
			string text = string.Empty;
			if (gameObject != null && gameObject.GetComponent<Collider2D>() == null && gameObject.GetComponent<Rigidbody2D>() == null)
			{
				text += "GameObject requires a RigidBody2D or Collider2D component!\n";
			}
			return text;
		}

		public static void DebugLog(Fsm fsm, LogLevel logLevel, string text, bool sendToUnityLog = false)
		{
		}

		public static void LogError(string text)
		{
			DebugLog(FsmExecutionStack.ExecutingFsm, LogLevel.Error, text, sendToUnityLog: true);
		}

		public static void LogWarning(string text)
		{
			DebugLog(FsmExecutionStack.ExecutingFsm, LogLevel.Warning, text, sendToUnityLog: true);
		}

		public static string FormatUnityLogString(string text)
		{
			if (FsmExecutionStack.ExecutingFsm == null)
			{
				return text;
			}
			string text2 = Fsm.GetFullFsmLabel(FsmExecutionStack.ExecutingFsm);
			if (FsmExecutionStack.ExecutingState != null)
			{
				text2 = text2 + " : " + FsmExecutionStack.ExecutingStateName;
			}
			if (FsmExecutionStack.ExecutingAction != null)
			{
				text2 += FsmExecutionStack.ExecutingAction.Name;
			}
			return text2 + " : " + text;
		}

		public static string StripTags(string textWithTags)
		{
			textWithTags = textWithTags.Replace("<i>", "");
			textWithTags = textWithTags.Replace("</i>", "");
			textWithTags = textWithTags.Replace("<b>", "");
			textWithTags = textWithTags.Replace("</b>", "");
			return textWithTags;
		}

		public static string GetValueLabel(INamedVariable variable)
		{
			return "";
		}

		public static string GetValueLabel(Fsm fsm, FsmOwnerDefault ownerDefault)
		{
			if (ownerDefault == null)
			{
				return "[null]";
			}
			if (ownerDefault.OwnerOption == OwnerDefaultOption.UseOwner)
			{
				return "Owner";
			}
			return GetValueLabel(ownerDefault.GameObject);
		}

		public static string AutoName(FsmStateAction action, params INamedVariable[] exposedFields)
		{
			if (action != null)
			{
				return AutoName(action.GetType().Name, exposedFields);
			}
			return null;
		}

		public static string AutoName(FsmStateAction action, Fsm fsm, FsmOwnerDefault ownerDefault)
		{
			if (action != null)
			{
				return AutoName(action.GetType().Name, GetValueLabel(fsm, ownerDefault));
			}
			return null;
		}

		public static string AutoName(FsmStateAction action, params string[] labels)
		{
			if (action != null)
			{
				return AutoName(action.GetType().Name, labels);
			}
			return null;
		}

		public static string AutoName(FsmStateAction action, FsmEvent fsmEvent)
		{
			if (action != null)
			{
				return AutoName(action.GetType().Name, (fsmEvent != null) ? fsmEvent.Name : "None");
			}
			return null;
		}

		public static string AutoName(string actionName, params INamedVariable[] exposedFields)
		{
			string text = actionName + ": ";
			foreach (INamedVariable variable in exposedFields)
			{
				text = text + GetValueLabel(variable) + " ";
			}
			return text;
		}

		public static string AutoName(string actionName, params string[] labels)
		{
			string text = actionName + ": ";
			foreach (string text2 in labels)
			{
				text = text + text2 + " ";
			}
			return text;
		}

		public static string AutoName(FsmStateAction action, Fsm fsm, FsmOwnerDefault target, params INamedVariable[] exposedFields)
		{
			if (action != null)
			{
				return AutoName(action.GetType().Name, fsm, target, exposedFields);
			}
			return null;
		}

		public static string AutoName(string actionName, Fsm fsm, FsmOwnerDefault target, params INamedVariable[] exposedFields)
		{
			string text = actionName + ": " + GetValueLabel(fsm, target) + " ";
			foreach (INamedVariable variable in exposedFields)
			{
				text = text + GetValueLabel(variable) + " ";
			}
			return text;
		}

		public static string AutoNameRange(FsmStateAction action, NamedVariable min, NamedVariable max)
		{
			if (action != null)
			{
				return AutoNameRange(action.GetType().Name, min, max);
			}
			return null;
		}

		public static string AutoNameRange(string actionName, NamedVariable min, NamedVariable max)
		{
			return actionName + ": " + GetValueLabel(min) + " - " + GetValueLabel(max);
		}

		public static string AutoNameSetVar(FsmStateAction action, NamedVariable var, NamedVariable value)
		{
			if (action != null)
			{
				return AutoNameSetVar(action.GetType().Name, var, value);
			}
			return null;
		}

		public static string AutoNameSetVar(string actionName, NamedVariable var, NamedVariable value)
		{
			return actionName + ": " + GetValueLabel(var) + " = " + GetValueLabel(value);
		}

		public static string AutoNameConvert(FsmStateAction action, NamedVariable fromVariable, NamedVariable toVariable)
		{
			if (action != null)
			{
				return AutoNameConvert(action.GetType().Name, fromVariable, toVariable);
			}
			return null;
		}

		public static string AutoNameConvert(string actionName, NamedVariable fromVariable, NamedVariable toVariable)
		{
			return actionName.Replace("Convert", "") + ": " + GetValueLabel(fromVariable) + " to " + GetValueLabel(toVariable);
		}

		public static string AutoNameGetProperty(FsmStateAction action, NamedVariable property, NamedVariable store)
		{
			if (action != null)
			{
				return AutoNameGetProperty(action.GetType().Name, property, store);
			}
			return null;
		}

		public static string AutoNameGetProperty(string actionName, NamedVariable property, NamedVariable store)
		{
			return actionName + ": " + GetValueLabel(property) + " -> " + GetValueLabel(store);
		}

		[Obsolete("Use LogError instead.")]
		public static void RuntimeError(FsmStateAction action, string error)
		{
			action.LogError(action?.ToString() + ": " + error);
		}
	}
}
