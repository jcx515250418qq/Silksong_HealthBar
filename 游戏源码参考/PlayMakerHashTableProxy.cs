using System.Collections;

public class PlayMakerHashTableProxy : PlayMakerCollectionProxy
{
	public Hashtable _hashTable;

	private Hashtable _snapShot;

	public Hashtable hashTable => _hashTable;

	public void Awake()
	{
		_hashTable = new Hashtable();
		PreFillHashTable();
		TakeSnapShot();
	}

	public bool isCollectionDefined()
	{
		return hashTable != null;
	}

	public void TakeSnapShot()
	{
		_snapShot = new Hashtable();
		foreach (object key in _hashTable.Keys)
		{
			_snapShot[key] = _hashTable[key];
		}
	}

	public void RevertToSnapShot()
	{
		_hashTable = new Hashtable();
		foreach (object key in _snapShot.Keys)
		{
			_hashTable[key] = _snapShot[key];
		}
	}

	public void InspectorEdit(int index)
	{
		dispatchEvent(setEvent, index, "int");
	}

	private void PreFillHashTable()
	{
		for (int i = 0; i < preFillKeyList.Count; i++)
		{
			switch (preFillType)
			{
			case VariableEnum.Bool:
				hashTable[preFillKeyList[i]] = preFillBoolList[i];
				break;
			case VariableEnum.Color:
				hashTable[preFillKeyList[i]] = preFillColorList[i];
				break;
			case VariableEnum.Float:
				hashTable[preFillKeyList[i]] = preFillFloatList[i];
				break;
			case VariableEnum.GameObject:
				hashTable[preFillKeyList[i]] = preFillGameObjectList[i];
				break;
			case VariableEnum.Int:
				hashTable[preFillKeyList[i]] = preFillIntList[i];
				break;
			case VariableEnum.Material:
				hashTable[preFillKeyList[i]] = preFillMaterialList[i];
				break;
			case VariableEnum.Quaternion:
				hashTable[preFillKeyList[i]] = preFillQuaternionList[i];
				break;
			case VariableEnum.Rect:
				hashTable[preFillKeyList[i]] = preFillRectList[i];
				break;
			case VariableEnum.String:
				hashTable[preFillKeyList[i]] = preFillStringList[i];
				break;
			case VariableEnum.Texture:
				hashTable[preFillKeyList[i]] = preFillTextureList[i];
				break;
			case VariableEnum.Vector2:
				hashTable[preFillKeyList[i]] = preFillVector2List[i];
				break;
			case VariableEnum.Vector3:
				hashTable[preFillKeyList[i]] = preFillVector3List[i];
				break;
			case VariableEnum.AudioClip:
				hashTable[preFillKeyList[i]] = preFillAudioClipList[i];
				break;
			}
		}
	}
}
