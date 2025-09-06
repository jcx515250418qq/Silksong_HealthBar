namespace InControl
{
	public interface IInputControl
	{
		bool HasChanged { get; }

		bool IsPressed { get; }

		bool WasPressed { get; }

		bool WasReleased { get; }

		void ClearInputState();
	}
}
