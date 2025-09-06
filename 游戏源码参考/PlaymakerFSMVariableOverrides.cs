using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

public class PlaymakerFSMVariableOverrides : MonoBehaviour
{
	public interface IDictionaryAccessable<T>
	{
		Dictionary<string, T> Dictionary { get; set; }

		void OnAfterDeserialize();

		void OnBeforeSerialize();
	}

	[Serializable]
	private class NamedOverrideFloat : SerializableNamedData<float>
	{
	}

	[Serializable]
	private class NamedOverrideFloats : SerializableNamedList<float, NamedOverrideFloat>, IDictionaryAccessable<float>
	{
		public Dictionary<string, float> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideInt : SerializableNamedData<int>
	{
	}

	[Serializable]
	private class NamedOverrideInts : SerializableNamedList<int, NamedOverrideInt>, IDictionaryAccessable<int>
	{
		public Dictionary<string, int> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideBool : SerializableNamedData<bool>
	{
	}

	[Serializable]
	private class NamedOverrideBools : SerializableNamedList<bool, NamedOverrideBool>, IDictionaryAccessable<bool>
	{
		public Dictionary<string, bool> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideGameObject : SerializableNamedData<GameObject>
	{
	}

	[Serializable]
	private class NamedOverrideGameObjects : SerializableNamedList<GameObject, NamedOverrideGameObject>, IDictionaryAccessable<GameObject>
	{
		public Dictionary<string, GameObject> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideVector2 : SerializableNamedData<Vector2>
	{
	}

	[Serializable]
	private class NamedOverrideVector2s : SerializableNamedList<Vector2, NamedOverrideVector2>, IDictionaryAccessable<Vector2>
	{
		public Dictionary<string, Vector2> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideVector3 : SerializableNamedData<Vector3>
	{
	}

	[Serializable]
	private class NamedOverrideVector3s : SerializableNamedList<Vector3, NamedOverrideVector3>, IDictionaryAccessable<Vector3>
	{
		public Dictionary<string, Vector3> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideString : SerializableNamedData<string>
	{
	}

	[Serializable]
	private class NamedOverrideStrings : SerializableNamedList<string, NamedOverrideString>, IDictionaryAccessable<string>
	{
		public Dictionary<string, string> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideColor : SerializableNamedData<Color>
	{
	}

	[Serializable]
	private class NamedOverrideColors : SerializableNamedList<Color, NamedOverrideColor>, IDictionaryAccessable<Color>
	{
		public Dictionary<string, Color> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideMaterial : SerializableNamedData<Material>
	{
	}

	[Serializable]
	private class NamedOverrideMaterials : SerializableNamedList<Material, NamedOverrideMaterial>, IDictionaryAccessable<Material>
	{
		public Dictionary<string, Material> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[Serializable]
	private class NamedOverrideObject : SerializableNamedData<UnityEngine.Object>
	{
	}

	[Serializable]
	private class NamedOverrideObjects : SerializableNamedList<UnityEngine.Object, NamedOverrideObject>, IDictionaryAccessable<UnityEngine.Object>
	{
		public Dictionary<string, UnityEngine.Object> Dictionary
		{
			get
			{
				return RuntimeData;
			}
			set
			{
				RuntimeData = value;
			}
		}
	}

	[SerializeField]
	private PlayMakerFSM fsm;

	[SerializeField]
	private bool overrideInternalVariables;

	[SerializeField]
	private NamedOverrideFloats floatOverrides;

	[SerializeField]
	private NamedOverrideInts intOverrides;

	[SerializeField]
	private NamedOverrideBools boolOverrides;

	[SerializeField]
	private NamedOverrideGameObjects gameObjectOverrides;

	[SerializeField]
	private NamedOverrideVector2s vector2Overrides;

	[SerializeField]
	private NamedOverrideVector3s vector3Overrides;

	[SerializeField]
	private NamedOverrideStrings stringOverrides;

	[SerializeField]
	private NamedOverrideColors colorOverrides;

	[SerializeField]
	private NamedOverrideMaterials materialOverrides;

	[SerializeField]
	private NamedOverrideObjects objectOverrides;

	private void Awake()
	{
		if (!fsm)
		{
			return;
		}
		OverrideVariables(fsm.FsmVariables.FloatVariables, floatOverrides);
		OverrideVariables(fsm.FsmVariables.IntVariables, intOverrides);
		OverrideVariables(fsm.FsmVariables.BoolVariables, boolOverrides);
		OverrideVariables(fsm.FsmVariables.GameObjectVariables, gameObjectOverrides);
		OverrideVariables(fsm.FsmVariables.Vector2Variables, vector2Overrides);
		OverrideVariables(fsm.FsmVariables.Vector3Variables, vector3Overrides);
		OverrideVariables(fsm.FsmVariables.StringVariables, stringOverrides);
		OverrideVariables(fsm.FsmVariables.ColorVariables, colorOverrides);
		OverrideVariables(fsm.FsmVariables.QuaternionVariables, materialOverrides);
		OverrideVariables(fsm.FsmVariables.ObjectVariables, objectOverrides);
		FsmEnum[] enumVariables = fsm.FsmVariables.EnumVariables;
		foreach (FsmEnum fsmEnum in enumVariables)
		{
			if ((fsmEnum.ShowInInspector || overrideInternalVariables) && intOverrides.Dictionary.ContainsKey(fsmEnum.Name))
			{
				fsmEnum.RawValue = Enum.ToObject(fsmEnum.EnumType, intOverrides.Dictionary[fsmEnum.Name]);
			}
		}
	}

	public void PurgeBlankNames()
	{
		PurgeBlankName(floatOverrides);
		PurgeBlankName(intOverrides);
		PurgeBlankName(boolOverrides);
		PurgeBlankName(gameObjectOverrides);
		PurgeBlankName(vector2Overrides);
		PurgeBlankName(vector3Overrides);
		PurgeBlankName(stringOverrides);
		PurgeBlankName(colorOverrides);
		PurgeBlankName(materialOverrides);
		PurgeBlankName(objectOverrides);
	}

	private void OverrideVariables<T, U>(T[] variables, IDictionaryAccessable<U> dictionary) where T : NamedVariable
	{
		foreach (T val in variables)
		{
			if ((val.ShowInInspector || overrideInternalVariables) && dictionary.Dictionary.ContainsKey(val.Name))
			{
				val.RawValue = dictionary.Dictionary[val.Name];
			}
		}
	}

	public void AddOverride(VariableType type, string variableName)
	{
		if (!string.IsNullOrEmpty(variableName))
		{
			switch (type)
			{
			case VariableType.Float:
				floatOverrides.Dictionary[variableName] = 0f;
				break;
			case VariableType.Int:
			case VariableType.Enum:
				intOverrides.Dictionary[variableName] = 0;
				break;
			case VariableType.Bool:
				boolOverrides.Dictionary[variableName] = false;
				break;
			case VariableType.GameObject:
				gameObjectOverrides.Dictionary[variableName] = null;
				break;
			case VariableType.Vector2:
				vector2Overrides.Dictionary[variableName] = default(Vector2);
				break;
			case VariableType.Vector3:
				vector3Overrides.Dictionary[variableName] = default(Vector3);
				break;
			case VariableType.String:
				stringOverrides.Dictionary[variableName] = null;
				break;
			case VariableType.Color:
				colorOverrides.Dictionary[variableName] = default(Color);
				break;
			case VariableType.Material:
				materialOverrides.Dictionary[variableName] = null;
				break;
			case VariableType.Object:
				objectOverrides.Dictionary[variableName] = null;
				break;
			case VariableType.Rect:
			case VariableType.Texture:
			case VariableType.Quaternion:
			case VariableType.Array:
				break;
			}
		}
	}

	public bool HasOverride(VariableType type, string variableName)
	{
		if (string.IsNullOrEmpty(variableName))
		{
			return false;
		}
		switch (type)
		{
		case VariableType.Float:
			return floatOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Int:
		case VariableType.Enum:
			return intOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Bool:
			return boolOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.GameObject:
			return gameObjectOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Vector2:
			return vector2Overrides.Dictionary.ContainsKey(variableName);
		case VariableType.Vector3:
			return vector3Overrides.Dictionary.ContainsKey(variableName);
		case VariableType.String:
			return stringOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Color:
			return colorOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Material:
			return materialOverrides.Dictionary.ContainsKey(variableName);
		case VariableType.Object:
			return objectOverrides.Dictionary.ContainsKey(variableName);
		default:
			return false;
		}
	}

	private void PurgeBlankName<U>(IDictionaryAccessable<U> dictionary)
	{
		dictionary.OnAfterDeserialize();
		if (dictionary.Dictionary.ContainsKey(string.Empty))
		{
			dictionary.Dictionary.Remove(string.Empty);
		}
		dictionary.OnBeforeSerialize();
	}

	public NamedVariable[] GetFsmVariables()
	{
		if (!fsm)
		{
			return new NamedVariable[0];
		}
		if (overrideInternalVariables)
		{
			return fsm.FsmVariables.GetAllNamedVariablesSorted();
		}
		return (from var in fsm.FsmVariables.GetAllNamedVariablesSorted()
			where var.ShowInInspector
			select var).ToArray();
	}

	public Type GetOverriddenObjectType(string variableName)
	{
		if (!fsm)
		{
			return null;
		}
		return fsm.FsmVariables.ObjectVariables.FirstOrDefault((FsmObject obj) => obj.Name == variableName)?.ObjectType;
	}

	public Type GetEnumType(string variableName)
	{
		if (!fsm)
		{
			return null;
		}
		return fsm.FsmVariables.EnumVariables.FirstOrDefault((FsmEnum obj) => obj.Name == variableName)?.EnumType;
	}
}
