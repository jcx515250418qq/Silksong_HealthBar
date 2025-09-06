using UnityEngine;

public class SceneImporter : MonoBehaviour
{
	public string xml_folder = "./Assets/_Porting/Scene XML/";

	public string prefabs_folder = "Prefabs/";

	public string xml_doc_filename;

	public string level_name;

	public int tile_size;

	public int scene_width;

	public int scene_height;

	public int layer_count;

	public GameObject placeholder_prefab;

	public int importMode = 1;

	public bool lookForPrefabsFirst = true;

	public bool debug_output;
}
