using System;
using System.Text.RegularExpressions;
using HutongGames.PlayMaker;
using UnityEngine;

public class PlayMakerUtils
{
	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, bool includeChildren)
	{
		SendEventToGameObject(fromFsm, target, fsmEvent, includeChildren, null);
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent)
	{
		SendEventToGameObject(fromFsm, target, fsmEvent, includeChildren: false, null);
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, FsmEventData eventData)
	{
		SendEventToGameObject(fromFsm, target, fsmEvent, includeChildren: false, eventData);
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, bool includeChildren, FsmEventData eventData)
	{
		if (eventData != null)
		{
			Fsm.EventData = eventData;
		}
		if (!(fromFsm == null))
		{
			FsmEventTarget fsmEventTarget = new FsmEventTarget();
			fsmEventTarget.excludeSelf = false;
			FsmOwnerDefault fsmOwnerDefault = new FsmOwnerDefault();
			fsmOwnerDefault.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
			fsmOwnerDefault.GameObject = new FsmGameObject();
			fsmOwnerDefault.GameObject.Value = target;
			fsmEventTarget.gameObject = fsmOwnerDefault;
			fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
			fsmEventTarget.sendToChildren = includeChildren;
			fromFsm.Fsm.Event(fsmEventTarget, fsmEvent);
		}
	}

	public static bool DoesTargetImplementsEvent(FsmEventTarget target, string eventName)
	{
		if (target.target == FsmEventTarget.EventTarget.BroadcastAll)
		{
			return FsmEvent.IsEventGlobal(eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.FSMComponent)
		{
			return DoesFsmImplementsEvent(target.fsmComponent, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.GameObject)
		{
			return DoesGameObjectImplementsEvent(target.gameObject.GameObject.Value, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.GameObjectFSM)
		{
			return DoesGameObjectImplementsEvent(target.gameObject.GameObject.Value, target.fsmName.Value, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.Self)
		{
			Debug.LogError("Self target not supported yet");
		}
		if (target.target == FsmEventTarget.EventTarget.SubFSMs)
		{
			Debug.LogError("subFsms target not supported yet");
		}
		if (target.target == FsmEventTarget.EventTarget.HostFSM)
		{
			Debug.LogError("HostFSM target not supported yet");
		}
		return false;
	}

	public static bool DoesGameObjectImplementsEvent(GameObject go, string fsmEvent)
	{
		if (go == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
		for (int i = 0; i < components.Length; i++)
		{
			if (DoesFsmImplementsEvent(components[i], fsmEvent))
			{
				return true;
			}
		}
		return false;
	}

	public static bool DoesGameObjectImplementsEvent(GameObject go, string fsmName, string fsmEvent)
	{
		if (go == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		bool flag = !string.IsNullOrEmpty(fsmName);
		PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			if (flag && object.Equals(playMakerFSM, fsmName) && DoesFsmImplementsEvent(playMakerFSM, fsmEvent))
			{
				return true;
			}
		}
		return false;
	}

	public static bool DoesFsmImplementsEvent(PlayMakerFSM fsm, string fsmEvent)
	{
		if (fsm == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		FsmTransition[] fsmGlobalTransitions = fsm.FsmGlobalTransitions;
		for (int i = 0; i < fsmGlobalTransitions.Length; i++)
		{
			if (fsmGlobalTransitions[i].EventName.Equals(fsmEvent))
			{
				return true;
			}
		}
		FsmState[] fsmStates = fsm.FsmStates;
		for (int i = 0; i < fsmStates.Length; i++)
		{
			fsmGlobalTransitions = fsmStates[i].Transitions;
			for (int j = 0; j < fsmGlobalTransitions.Length; j++)
			{
				if (fsmGlobalTransitions[j].EventName.Equals(fsmEvent))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static PlayMakerFSM FindFsmOnGameObject(GameObject go, string fsmName)
	{
		if (go == null || string.IsNullOrEmpty(fsmName))
		{
			return null;
		}
		PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			if (string.Equals(playMakerFSM.FsmName, fsmName))
			{
				return playMakerFSM;
			}
		}
		return null;
	}

	public static void RefreshValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm != null && fsmVar != null && fsmVar.useVariable)
		{
			switch (fsmVar.Type)
			{
			case VariableType.Int:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmInt(fsmVar.variableName));
				break;
			case VariableType.Float:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmFloat(fsmVar.variableName));
				break;
			case VariableType.Bool:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmBool(fsmVar.variableName));
				break;
			case VariableType.Color:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmColor(fsmVar.variableName));
				break;
			case VariableType.Quaternion:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName));
				break;
			case VariableType.Rect:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmRect(fsmVar.variableName));
				break;
			case VariableType.Vector2:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector2(fsmVar.variableName));
				break;
			case VariableType.Vector3:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
				break;
			case VariableType.Texture:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
				break;
			case VariableType.Material:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmMaterial(fsmVar.variableName));
				break;
			case VariableType.String:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmString(fsmVar.variableName));
				break;
			case VariableType.GameObject:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmGameObject(fsmVar.variableName));
				break;
			}
		}
	}

	public static object GetValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm == null)
		{
			return null;
		}
		if (fsmVar == null)
		{
			return null;
		}
		if (fsmVar.useVariable)
		{
			string variableName = fsmVar.variableName;
			switch (fsmVar.Type)
			{
			case VariableType.Int:
				return fromFsm.Variables.GetFsmInt(variableName).Value;
			case VariableType.Float:
				return fromFsm.Variables.GetFsmFloat(variableName).Value;
			case VariableType.Bool:
				return fromFsm.Variables.GetFsmBool(variableName).Value;
			case VariableType.Color:
				return fromFsm.Variables.GetFsmColor(variableName).Value;
			case VariableType.Quaternion:
				return fromFsm.Variables.GetFsmQuaternion(variableName).Value;
			case VariableType.Rect:
				return fromFsm.Variables.GetFsmRect(variableName).Value;
			case VariableType.Vector2:
				return fromFsm.Variables.GetFsmVector2(variableName).Value;
			case VariableType.Vector3:
				return fromFsm.Variables.GetFsmVector3(variableName).Value;
			case VariableType.Texture:
				return fromFsm.Variables.GetFsmTexture(variableName).Value;
			case VariableType.Material:
				return fromFsm.Variables.GetFsmMaterial(variableName).Value;
			case VariableType.String:
				return fromFsm.Variables.GetFsmString(variableName).Value;
			case VariableType.GameObject:
				return fromFsm.Variables.GetFsmGameObject(variableName).Value;
			case VariableType.Object:
				return fromFsm.Variables.GetFsmObject(variableName).Value;
			}
		}
		else
		{
			switch (fsmVar.Type)
			{
			case VariableType.Int:
				return fsmVar.intValue;
			case VariableType.Float:
				return fsmVar.floatValue;
			case VariableType.Bool:
				return fsmVar.boolValue;
			case VariableType.Color:
				return fsmVar.colorValue;
			case VariableType.Quaternion:
				return fsmVar.quaternionValue;
			case VariableType.Rect:
				return fsmVar.rectValue;
			case VariableType.Vector2:
				return fsmVar.vector2Value;
			case VariableType.Vector3:
				return fsmVar.vector3Value;
			case VariableType.Texture:
				return fsmVar.textureValue;
			case VariableType.Material:
				return fsmVar.materialValue;
			case VariableType.String:
				return fsmVar.stringValue;
			case VariableType.GameObject:
				return fsmVar.gameObjectValue;
			case VariableType.Object:
				return fsmVar.objectReference;
			}
		}
		return null;
	}

	public static bool ApplyValueToFsmVar(Fsm fromFsm, FsmVar fsmVar, object value)
	{
		if (fromFsm == null)
		{
			return false;
		}
		if (fsmVar == null)
		{
			return false;
		}
		if (value == null)
		{
			if (fsmVar.Type == VariableType.Bool)
			{
				fromFsm.Variables.GetFsmBool(fsmVar.variableName).Value = false;
			}
			else if (fsmVar.Type == VariableType.Color)
			{
				fromFsm.Variables.GetFsmColor(fsmVar.variableName).Value = Color.black;
			}
			else if (fsmVar.Type == VariableType.Int)
			{
				fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = 0;
			}
			else if (fsmVar.Type == VariableType.Float)
			{
				fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = 0f;
			}
			else if (fsmVar.Type == VariableType.GameObject)
			{
				fromFsm.Variables.GetFsmGameObject(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Material)
			{
				fromFsm.Variables.GetFsmMaterial(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Object)
			{
				fromFsm.Variables.GetFsmObject(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Quaternion)
			{
				fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName).Value = Quaternion.identity;
			}
			else if (fsmVar.Type == VariableType.Rect)
			{
				fromFsm.Variables.GetFsmRect(fsmVar.variableName).Value = new Rect(0f, 0f, 0f, 0f);
			}
			else if (fsmVar.Type == VariableType.String)
			{
				fromFsm.Variables.GetFsmString(fsmVar.variableName).Value = "";
			}
			else if (fsmVar.Type == VariableType.String)
			{
				fromFsm.Variables.GetFsmTexture(fsmVar.variableName).Value = null;
			}
			else if (fsmVar.Type == VariableType.Vector2)
			{
				fromFsm.Variables.GetFsmVector2(fsmVar.variableName).Value = Vector2.zero;
			}
			else if (fsmVar.Type == VariableType.Vector3)
			{
				fromFsm.Variables.GetFsmVector3(fsmVar.variableName).Value = Vector3.zero;
			}
			return true;
		}
		Type type = value.GetType();
		Type type2 = null;
		switch (fsmVar.Type)
		{
		case VariableType.Int:
			type2 = typeof(int);
			break;
		case VariableType.Float:
			type2 = typeof(float);
			break;
		case VariableType.Bool:
			type2 = typeof(bool);
			break;
		case VariableType.Color:
			type2 = typeof(Color);
			break;
		case VariableType.GameObject:
			type2 = typeof(GameObject);
			break;
		case VariableType.Quaternion:
			type2 = typeof(Quaternion);
			break;
		case VariableType.Rect:
			type2 = typeof(Rect);
			break;
		case VariableType.String:
			type2 = typeof(string);
			break;
		case VariableType.Texture:
			type2 = typeof(Texture2D);
			break;
		case VariableType.Vector2:
			type2 = typeof(Vector2);
			break;
		case VariableType.Vector3:
			type2 = typeof(Vector3);
			break;
		case VariableType.Object:
			type2 = typeof(UnityEngine.Object);
			break;
		case VariableType.Material:
			type2 = typeof(Material);
			break;
		}
		bool flag = true;
		if (!type2.Equals(type))
		{
			flag = false;
			if (type2.Equals(typeof(UnityEngine.Object)))
			{
				flag = true;
			}
			if (!flag)
			{
				if (type.Equals(typeof(double)))
				{
					flag = true;
				}
				if (type.Equals(typeof(long)))
				{
					flag = true;
				}
				if (type.Equals(typeof(byte)))
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Debug.LogError("The fsmVar value <" + type2?.ToString() + "> doesn't match the value <" + type?.ToString() + ">");
			return false;
		}
		if (type == typeof(bool))
		{
			fromFsm.Variables.GetFsmBool(fsmVar.variableName).Value = (bool)value;
		}
		else if (type == typeof(Color))
		{
			fromFsm.Variables.GetFsmColor(fsmVar.variableName).Value = (Color)value;
		}
		else if (type == typeof(int))
		{
			fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = Convert.ToInt32(value);
		}
		else if (type == typeof(byte))
		{
			fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = Convert.ToInt32(value);
		}
		else if (type == typeof(long))
		{
			if (fsmVar.Type == VariableType.Int)
			{
				fromFsm.Variables.GetFsmInt(fsmVar.variableName).Value = Convert.ToInt32(value);
			}
			else if (fsmVar.Type == VariableType.Float)
			{
				fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = Convert.ToSingle(value);
			}
		}
		else if (type == typeof(float))
		{
			fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = (float)value;
		}
		else if (type == typeof(double))
		{
			fromFsm.Variables.GetFsmFloat(fsmVar.variableName).Value = Convert.ToSingle(value);
		}
		else if (type == typeof(GameObject))
		{
			fromFsm.Variables.GetFsmGameObject(fsmVar.variableName).Value = (GameObject)value;
		}
		else if (type == typeof(Material))
		{
			fromFsm.Variables.GetFsmMaterial(fsmVar.variableName).Value = (Material)value;
		}
		else if (type == typeof(UnityEngine.Object) || type2 == typeof(UnityEngine.Object))
		{
			fromFsm.Variables.GetFsmObject(fsmVar.variableName).Value = (UnityEngine.Object)value;
		}
		else if (type == typeof(Quaternion))
		{
			fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName).Value = (Quaternion)value;
		}
		else if (type == typeof(Rect))
		{
			fromFsm.Variables.GetFsmRect(fsmVar.variableName).Value = (Rect)value;
		}
		else if (type == typeof(string))
		{
			fromFsm.Variables.GetFsmString(fsmVar.variableName).Value = (string)value;
		}
		else if (type == typeof(Texture2D))
		{
			fromFsm.Variables.GetFsmTexture(fsmVar.variableName).Value = (Texture2D)value;
		}
		else if (type == typeof(Vector2))
		{
			fromFsm.Variables.GetFsmVector2(fsmVar.variableName).Value = (Vector2)value;
		}
		else if (type == typeof(Vector3))
		{
			fromFsm.Variables.GetFsmVector3(fsmVar.variableName).Value = (Vector3)value;
		}
		else
		{
			Debug.LogWarning("?!?!" + type);
		}
		return true;
	}

	public static float GetFloatFromObject(object _obj, VariableType targetType, bool fastProcessingIfPossible)
	{
		switch (targetType)
		{
		case VariableType.Float:
		case VariableType.Int:
			return Convert.ToSingle(_obj);
		case VariableType.Vector2:
		{
			Vector2 vector = (Vector2)_obj;
			if (vector != Vector2.zero)
			{
				if (!fastProcessingIfPossible)
				{
					return vector.magnitude;
				}
				return vector.sqrMagnitude;
			}
			break;
		}
		}
		if (targetType == VariableType.Vector3)
		{
			Vector3 vector2 = (Vector3)_obj;
			if (vector2 != Vector3.zero)
			{
				if (!fastProcessingIfPossible)
				{
					return vector2.magnitude;
				}
				return vector2.sqrMagnitude;
			}
		}
		if (targetType == VariableType.GameObject)
		{
			GameObject gameObject = (GameObject)_obj;
			if (gameObject != null)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					return component.bounds.size.x * component.bounds.size.y * component.bounds.size.z;
				}
			}
		}
		switch (targetType)
		{
		case VariableType.Rect:
		{
			Rect rect = (Rect)_obj;
			return rect.width * rect.height;
		}
		case VariableType.String:
		{
			string text = (string)_obj;
			if (text != null)
			{
				return float.Parse(text);
			}
			break;
		}
		}
		return 0f;
	}

	public static string ParseFsmVarToString(Fsm fsm, FsmVar fsmVar)
	{
		if (fsmVar == null)
		{
			return "";
		}
		object valueFromFsmVar = GetValueFromFsmVar(fsm, fsmVar);
		if (valueFromFsmVar == null)
		{
			return "";
		}
		if (fsmVar.Type == VariableType.String)
		{
			return (string)valueFromFsmVar;
		}
		if (fsmVar.Type == VariableType.Bool)
		{
			if (!(bool)valueFromFsmVar)
			{
				return "0";
			}
			return "1";
		}
		if (fsmVar.Type == VariableType.Float)
		{
			return float.Parse(valueFromFsmVar.ToString()).ToString();
		}
		if (fsmVar.Type == VariableType.Int)
		{
			return int.Parse(valueFromFsmVar.ToString()).ToString();
		}
		if (fsmVar.Type == VariableType.Vector2)
		{
			Vector2 vector = (Vector2)valueFromFsmVar;
			return vector.x + "," + vector.y;
		}
		if (fsmVar.Type == VariableType.Vector3)
		{
			Vector3 vector2 = (Vector3)valueFromFsmVar;
			return vector2.x + "," + vector2.y + "," + vector2.z;
		}
		if (fsmVar.Type == VariableType.Quaternion)
		{
			Quaternion quaternion = (Quaternion)valueFromFsmVar;
			return quaternion.x + "," + quaternion.y + "," + quaternion.z + "," + quaternion.w;
		}
		if (fsmVar.Type == VariableType.Rect)
		{
			Rect rect = (Rect)valueFromFsmVar;
			return rect.x + "," + rect.y + "," + rect.width + "," + rect.height;
		}
		if (fsmVar.Type == VariableType.Color)
		{
			Color color = (Color)valueFromFsmVar;
			return color.r + "," + color.g + "," + color.b + "," + color.a;
		}
		if (fsmVar.Type == VariableType.GameObject)
		{
			return ((GameObject)valueFromFsmVar).name;
		}
		if (fsmVar.Type == VariableType.Material)
		{
			return ((Material)valueFromFsmVar).name;
		}
		if (fsmVar.Type == VariableType.Texture)
		{
			return ((Texture2D)valueFromFsmVar).name;
		}
		Debug.LogWarning("ParseValueToString type not supported " + valueFromFsmVar.GetType());
		return "<" + fsmVar.Type.ToString() + "> not supported";
	}

	public static string ParseValueToString(object item, bool useBytes)
	{
		return "";
	}

	public static string ParseValueToString(object item)
	{
		if (item.GetType() == typeof(string))
		{
			return "string(" + item.ToString() + ")";
		}
		if (item.GetType() == typeof(bool))
		{
			return "bool(" + (((bool)item) ? 1 : 0) + ")";
		}
		if (item.GetType() == typeof(float))
		{
			return "float(" + float.Parse(item.ToString()) + ")";
		}
		if (item.GetType() == typeof(int))
		{
			return "int(" + int.Parse(item.ToString()) + ")";
		}
		if (item.GetType() == typeof(Vector2))
		{
			Vector2 vector = (Vector2)item;
			return "vector2(" + vector.x + "," + vector.y + ")";
		}
		if (item.GetType() == typeof(Vector3))
		{
			Vector3 vector2 = (Vector3)item;
			return "vector3(" + vector2.x + "," + vector2.y + "," + vector2.z + ")";
		}
		if (item.GetType() == typeof(Vector4))
		{
			Vector4 vector3 = (Vector4)item;
			return "vector4(" + vector3.x + "," + vector3.y + "," + vector3.z + "," + vector3.w + ")";
		}
		if (item.GetType() == typeof(Quaternion))
		{
			Quaternion quaternion = (Quaternion)item;
			return "quaternion(" + quaternion.x + "," + quaternion.y + "," + quaternion.z + "," + quaternion.w + ")";
		}
		if (item.GetType() == typeof(Rect))
		{
			Rect rect = (Rect)item;
			return "rect(" + rect.x + "," + rect.y + "," + rect.width + "," + rect.height + ")";
		}
		if (item.GetType() == typeof(Color))
		{
			Color color = (Color)item;
			return "color(" + color.r + "," + color.g + "," + color.b + "," + color.a + ")";
		}
		if (item.GetType() == typeof(Texture2D))
		{
			byte[] inArray = ((Texture2D)item).EncodeToPNG();
			return "texture(" + Convert.ToBase64String(inArray) + ")";
		}
		if (item.GetType() == typeof(GameObject))
		{
			GameObject gameObject = (GameObject)item;
			return "gameObject(" + gameObject.name + ")";
		}
		Debug.LogWarning("ParseValueToString type not supported " + item.GetType());
		return "<" + item.GetType()?.ToString() + "> not supported";
	}

	public static object ParseValueFromString(string source, bool useBytes)
	{
		return null;
	}

	public static object ParseValueFromString(string source, VariableType type)
	{
		Type typeFromHandle = typeof(string);
		switch (type)
		{
		case VariableType.Bool:
			typeFromHandle = typeof(bool);
			break;
		case VariableType.Color:
			typeFromHandle = typeof(Color);
			break;
		case VariableType.Float:
			typeFromHandle = typeof(float);
			break;
		case VariableType.GameObject:
			typeFromHandle = typeof(GameObject);
			break;
		case VariableType.Int:
			typeFromHandle = typeof(int);
			break;
		case VariableType.Quaternion:
			typeFromHandle = typeof(Quaternion);
			break;
		case VariableType.Rect:
			typeFromHandle = typeof(Rect);
			break;
		case VariableType.Vector2:
			typeFromHandle = typeof(Vector2);
			break;
		case VariableType.Vector3:
			typeFromHandle = typeof(Vector3);
			break;
		case VariableType.Unknown:
			return ParseValueFromString(source);
		}
		return ParseValueFromString(source, typeFromHandle);
	}

	public static object ParseValueFromString(string source, Type type)
	{
		if (source == null)
		{
			return null;
		}
		if (type == typeof(string))
		{
			return source;
		}
		if (type == typeof(bool))
		{
			if (string.Equals(source, "true", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.Equals(source, "false", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			return (int.Parse(source) != 0) ? true : false;
		}
		if (type == typeof(int))
		{
			return int.Parse(source);
		}
		if (type == typeof(float))
		{
			return float.Parse(source);
		}
		if (type == typeof(Vector2))
		{
			string text = "vector2\\([x],[y]\\)";
			string text2 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text = text.Replace("[x]", "(?<x>" + text2 + ")");
			text = text.Replace("[y]", "(?<y>" + text2 + ")");
			text = "^\\s*" + text;
			Match match = new Regex(text).Match(source);
			if (match.Groups["x"].Value != "" && match.Groups["y"].Value != "")
			{
				return new Vector2(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value));
			}
			return Vector2.zero;
		}
		if (type == typeof(Vector3))
		{
			string text3 = "vector3\\([x],[y],[z]\\)";
			string text4 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text3 = text3.Replace("[x]", "(?<x>" + text4 + ")");
			text3 = text3.Replace("[y]", "(?<y>" + text4 + ")");
			text3 = text3.Replace("[z]", "(?<z>" + text4 + ")");
			text3 = "^\\s*" + text3;
			Match match2 = new Regex(text3).Match(source);
			if (match2.Groups["x"].Value != "" && match2.Groups["y"].Value != "" && match2.Groups["z"].Value != "")
			{
				return new Vector3(float.Parse(match2.Groups["x"].Value), float.Parse(match2.Groups["y"].Value), float.Parse(match2.Groups["z"].Value));
			}
			return Vector3.zero;
		}
		if (type == typeof(Vector4))
		{
			string text5 = "vector4\\([x],[y],[z],[w]\\)";
			string text6 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text5 = text5.Replace("[x]", "(?<x>" + text6 + ")");
			text5 = text5.Replace("[y]", "(?<y>" + text6 + ")");
			text5 = text5.Replace("[z]", "(?<z>" + text6 + ")");
			text5 = text5.Replace("[w]", "(?<w>" + text6 + ")");
			text5 = "^\\s*" + text5;
			Match match3 = new Regex(text5).Match(source);
			if (match3.Groups["x"].Value != "" && match3.Groups["y"].Value != "" && match3.Groups["z"].Value != "" && match3.Groups["z"].Value != "")
			{
				return new Vector4(float.Parse(match3.Groups["x"].Value), float.Parse(match3.Groups["y"].Value), float.Parse(match3.Groups["z"].Value), float.Parse(match3.Groups["w"].Value));
			}
			return Vector4.zero;
		}
		if (type == typeof(Rect))
		{
			string text7 = "rect\\([x],[y],[w],[h]\\)";
			string text8 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text7 = text7.Replace("[x]", "(?<x>" + text8 + ")");
			text7 = text7.Replace("[y]", "(?<y>" + text8 + ")");
			text7 = text7.Replace("[w]", "(?<w>" + text8 + ")");
			text7 = text7.Replace("[h]", "(?<h>" + text8 + ")");
			text7 = "^\\s*" + text7;
			Match match4 = new Regex(text7).Match(source);
			if (match4.Groups["x"].Value != "" && match4.Groups["y"].Value != "" && match4.Groups["w"].Value != "" && match4.Groups["h"].Value != "")
			{
				return new Rect(float.Parse(match4.Groups["x"].Value), float.Parse(match4.Groups["y"].Value), float.Parse(match4.Groups["w"].Value), float.Parse(match4.Groups["h"].Value));
			}
			return new Rect(0f, 0f, 0f, 0f);
		}
		if (type == typeof(Quaternion))
		{
			string text9 = "quaternion\\([x],[y],[z],[w]\\)";
			string text10 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text9 = text9.Replace("[x]", "(?<x>" + text10 + ")");
			text9 = text9.Replace("[y]", "(?<y>" + text10 + ")");
			text9 = text9.Replace("[z]", "(?<z>" + text10 + ")");
			text9 = text9.Replace("[w]", "(?<w>" + text10 + ")");
			text9 = "^\\s*" + text9;
			Match match5 = new Regex(text9).Match(source);
			if (match5.Groups["x"].Value != "" && match5.Groups["y"].Value != "" && match5.Groups["z"].Value != "" && match5.Groups["z"].Value != "")
			{
				return new Quaternion(float.Parse(match5.Groups["x"].Value), float.Parse(match5.Groups["y"].Value), float.Parse(match5.Groups["z"].Value), float.Parse(match5.Groups["w"].Value));
			}
			return Quaternion.identity;
		}
		if (type == typeof(Color))
		{
			string text11 = "color\\([r],[g],[b],[a]\\)";
			string text12 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text11 = text11.Replace("[r]", "(?<r>" + text12 + ")");
			text11 = text11.Replace("[g]", "(?<g>" + text12 + ")");
			text11 = text11.Replace("[b]", "(?<b>" + text12 + ")");
			text11 = text11.Replace("[a]", "(?<a>" + text12 + ")");
			text11 = "^\\s*" + text11;
			Match match6 = new Regex(text11).Match(source);
			if (match6.Groups["r"].Value != "" && match6.Groups["g"].Value != "" && match6.Groups["b"].Value != "" && match6.Groups["a"].Value != "")
			{
				return new Color(float.Parse(match6.Groups["r"].Value), float.Parse(match6.Groups["g"].Value), float.Parse(match6.Groups["b"].Value), float.Parse(match6.Groups["a"].Value));
			}
			return Color.black;
		}
		if (type == typeof(GameObject))
		{
			source = source.Substring(11, source.Length - 12);
			return GameObject.Find(source);
		}
		Debug.LogWarning("ParseValueFromString failed for " + source);
		return null;
	}

	public static object ParseValueFromString(string source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.StartsWith("string("))
		{
			source = source.Substring(7, source.Length - 8);
			return source;
		}
		if (source.StartsWith("bool("))
		{
			source = source.Substring(5, source.Length - 6);
			return int.Parse(source) == 1;
		}
		if (source.StartsWith("int("))
		{
			source = source.Substring(4, source.Length - 5);
			return int.Parse(source);
		}
		if (source.StartsWith("float("))
		{
			source = source.Substring(6, source.Length - 7);
			return float.Parse(source);
		}
		if (source.StartsWith("vector2("))
		{
			string text = "vector2\\([x],[y]\\)";
			string text2 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text = text.Replace("[x]", "(?<x>" + text2 + ")");
			text = text.Replace("[y]", "(?<y>" + text2 + ")");
			text = "^\\s*" + text;
			Match match = new Regex(text).Match(source);
			if (match.Groups["x"].Value != "" && match.Groups["y"].Value != "")
			{
				return new Vector2(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value));
			}
			return Vector2.zero;
		}
		if (source.StartsWith("vector3("))
		{
			string text3 = "vector3\\([x],[y],[z]\\)";
			string text4 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text3 = text3.Replace("[x]", "(?<x>" + text4 + ")");
			text3 = text3.Replace("[y]", "(?<y>" + text4 + ")");
			text3 = text3.Replace("[z]", "(?<z>" + text4 + ")");
			text3 = "^\\s*" + text3;
			Match match2 = new Regex(text3).Match(source);
			if (match2.Groups["x"].Value != "" && match2.Groups["y"].Value != "" && match2.Groups["z"].Value != "")
			{
				return new Vector3(float.Parse(match2.Groups["x"].Value), float.Parse(match2.Groups["y"].Value), float.Parse(match2.Groups["z"].Value));
			}
			return Vector3.zero;
		}
		if (source.StartsWith("vector4("))
		{
			string text5 = "vector4\\([x],[y],[z],[w]\\)";
			string text6 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text5 = text5.Replace("[x]", "(?<x>" + text6 + ")");
			text5 = text5.Replace("[y]", "(?<y>" + text6 + ")");
			text5 = text5.Replace("[z]", "(?<z>" + text6 + ")");
			text5 = text5.Replace("[w]", "(?<w>" + text6 + ")");
			text5 = "^\\s*" + text5;
			Match match3 = new Regex(text5).Match(source);
			if (match3.Groups["x"].Value != "" && match3.Groups["y"].Value != "" && match3.Groups["z"].Value != "" && match3.Groups["z"].Value != "")
			{
				return new Vector4(float.Parse(match3.Groups["x"].Value), float.Parse(match3.Groups["y"].Value), float.Parse(match3.Groups["z"].Value), float.Parse(match3.Groups["w"].Value));
			}
			return Vector4.zero;
		}
		if (source.StartsWith("rect("))
		{
			string text7 = "rect\\([x],[y],[w],[h]\\)";
			string text8 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text7 = text7.Replace("[x]", "(?<x>" + text8 + ")");
			text7 = text7.Replace("[y]", "(?<y>" + text8 + ")");
			text7 = text7.Replace("[w]", "(?<w>" + text8 + ")");
			text7 = text7.Replace("[h]", "(?<h>" + text8 + ")");
			text7 = "^\\s*" + text7;
			Match match4 = new Regex(text7).Match(source);
			if (match4.Groups["x"].Value != "" && match4.Groups["y"].Value != "" && match4.Groups["w"].Value != "" && match4.Groups["h"].Value != "")
			{
				return new Rect(float.Parse(match4.Groups["x"].Value), float.Parse(match4.Groups["y"].Value), float.Parse(match4.Groups["w"].Value), float.Parse(match4.Groups["h"].Value));
			}
			return new Rect(0f, 0f, 0f, 0f);
		}
		if (source.StartsWith("quaternion("))
		{
			string text9 = "quaternion\\([x],[y],[z],[w]\\)";
			string text10 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text9 = text9.Replace("[x]", "(?<x>" + text10 + ")");
			text9 = text9.Replace("[y]", "(?<y>" + text10 + ")");
			text9 = text9.Replace("[z]", "(?<z>" + text10 + ")");
			text9 = text9.Replace("[w]", "(?<w>" + text10 + ")");
			text9 = "^\\s*" + text9;
			Match match5 = new Regex(text9).Match(source);
			if (match5.Groups["x"].Value != "" && match5.Groups["y"].Value != "" && match5.Groups["z"].Value != "" && match5.Groups["z"].Value != "")
			{
				return new Quaternion(float.Parse(match5.Groups["x"].Value), float.Parse(match5.Groups["y"].Value), float.Parse(match5.Groups["z"].Value), float.Parse(match5.Groups["w"].Value));
			}
			return Quaternion.identity;
		}
		if (source.StartsWith("color("))
		{
			string text11 = "color\\([r],[g],[b],[a]\\)";
			string text12 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text11 = text11.Replace("[r]", "(?<r>" + text12 + ")");
			text11 = text11.Replace("[g]", "(?<g>" + text12 + ")");
			text11 = text11.Replace("[b]", "(?<b>" + text12 + ")");
			text11 = text11.Replace("[a]", "(?<a>" + text12 + ")");
			text11 = "^\\s*" + text11;
			Match match6 = new Regex(text11).Match(source);
			if (match6.Groups["r"].Value != "" && match6.Groups["g"].Value != "" && match6.Groups["b"].Value != "" && match6.Groups["a"].Value != "")
			{
				return new Color(float.Parse(match6.Groups["r"].Value), float.Parse(match6.Groups["g"].Value), float.Parse(match6.Groups["b"].Value), float.Parse(match6.Groups["a"].Value));
			}
			return Color.black;
		}
		if (source.StartsWith("texture("))
		{
			source = source.Substring(8, source.Length - 9);
			byte[] data = Convert.FromBase64String(source);
			Texture2D texture2D = new Texture2D(16, 16);
			texture2D.LoadImage(data);
			return texture2D;
		}
		if (source.StartsWith("gameObject("))
		{
			source = source.Substring(11, source.Length - 12);
			return GameObject.Find(source);
		}
		Debug.LogWarning("ParseValueFromString failed for " + source);
		return null;
	}
}
