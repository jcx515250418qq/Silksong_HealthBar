using System.Diagnostics;

namespace TeamCherry.GameCore
{
	public sealed class DeltaTimeCalculator
	{
		private Stopwatch _stopwatch = new Stopwatch();

		private long _lastElapsedMilliseconds;

		public void Start()
		{
			_stopwatch.Start();
			_lastElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
		}

		public long GetDeltaTimeMillis()
		{
			long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
			long result = elapsedMilliseconds - _lastElapsedMilliseconds;
			_lastElapsedMilliseconds = elapsedMilliseconds;
			return result;
		}

		public float GetDeltaTimeSeconds()
		{
			return (float)GetDeltaTimeMillis() / 1000f;
		}

		public void Reset()
		{
			_stopwatch.Reset();
			_lastElapsedMilliseconds = 0L;
		}
	}
}
