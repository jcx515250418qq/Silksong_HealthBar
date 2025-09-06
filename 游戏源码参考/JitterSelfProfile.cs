using UnityEngine;

[CreateAssetMenu(fileName = "New Jitter Profile", menuName = "Profiles/Jitter Config")]
public class JitterSelfProfile : ScriptableObject
{
	[SerializeField]
	private JitterSelfConfig config;

	public JitterSelfConfig Config => config;
}
