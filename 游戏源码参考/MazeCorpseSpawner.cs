using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeCorpseSpawner : MonoBehaviour
{
	[SerializeField]
	private MazeController readScenesFromController;

	private HeroController hc;

	private void Start()
	{
		PlayerData instance = PlayerData.instance;
		IReadOnlyCollection<string> sceneNames = readScenesFromController.SceneNames;
		if (readScenesFromController.IsCapScene || string.IsNullOrEmpty(instance.HeroCorpseScene) || !sceneNames.Contains(instance.HeroCorpseScene) || sceneNames.Contains(instance.PreviousMazeScene))
		{
			return;
		}
		hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			SpawnCorpse();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate
		{
			SpawnCorpse();
			hc.heroInPosition -= temp;
		};
		hc.heroInPosition += temp;
	}

	private void SpawnCorpse()
	{
		HeroCorpseMarker closest = HeroCorpseMarker.GetClosest(hc.transform.position);
		if (closest == null)
		{
			Debug.LogError("Could not find a HeroCorpseMarker", this);
			return;
		}
		Vector2 position = closest.Position;
		GameObject heroCorpsePrefab = GameManager.instance.sm.heroCorpsePrefab;
		GameObject gameObject = Object.Instantiate(heroCorpsePrefab, new Vector3(position.x, position.y, heroCorpsePrefab.transform.position.z), Quaternion.identity);
		RepositionFromWalls component = gameObject.GetComponent<RepositionFromWalls>();
		if ((bool)component)
		{
			component.enabled = false;
			gameObject.transform.position = position;
		}
		gameObject.transform.SetParent(base.transform, worldPositionStays: true);
		gameObject.transform.SetParent(null);
	}
}
