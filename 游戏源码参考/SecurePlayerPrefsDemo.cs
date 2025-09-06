using SecPlayerPrefs;
using UnityEngine;

public class SecurePlayerPrefsDemo : MonoBehaviour
{
	private void Start()
	{
		SecurePlayerPrefs.SetFloat("float", 0.1f);
		SecurePlayerPrefs.SetBool("bool", Value: true);
		SecurePlayerPrefs.SetInt("int", 100);
		SecurePlayerPrefs.SetString("string", "amazing!");
		Debug.Log(SecurePlayerPrefs.GetFloat("float"));
		Debug.Log(SecurePlayerPrefs.GetBool("bool"));
		Debug.Log(SecurePlayerPrefs.GetInt("int"));
		Debug.Log(SecurePlayerPrefs.GetString("string"));
		SecureplayerPrefsDemoClass secureplayerPrefsDemoClass = new SecureplayerPrefsDemoClass();
		SecureDataManager<SecureplayerPrefsDemoClass> secureDataManager = new SecureDataManager<SecureplayerPrefsDemoClass>("name");
		secureplayerPrefsDemoClass.incremental = true;
		secureplayerPrefsDemoClass.playID = "tester";
		secureplayerPrefsDemoClass.type = 10;
		secureDataManager.Save(secureplayerPrefsDemoClass);
		secureplayerPrefsDemoClass = new SecureDataManager<SecureplayerPrefsDemoClass>("name").Get();
		Debug.Log(secureplayerPrefsDemoClass.incremental);
		Debug.Log(secureplayerPrefsDemoClass.type);
		Debug.Log(secureplayerPrefsDemoClass.playID);
	}
}
