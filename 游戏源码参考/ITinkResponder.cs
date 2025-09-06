using System;

public interface ITinkResponder
{
	[Serializable]
	[Flags]
	public enum TinkFlags
	{
		None = 0,
		Normal = 1,
		Projectile = 2,
		Unused = int.MinValue,
		All = 3
	}

	TinkFlags ResponderType { get; }

	void Tinked();
}
