using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Debug
{
	public class TimingProfiler
	{
		Dictionary<string, List<float>> timeDictionary;

		private float timeStarted;

		public TimingProfiler()
		{
			timeDictionary = new Dictionary<string, List<float>>();
		}

		public void Tic()
		{
			timeStarted = Time.realtimeSinceStartup;
		}

		public void Toc(string source)
		{
			if (!timeDictionary.ContainsKey(source))
			{
				timeDictionary.Add(source, new List<float>());
			}
			timeDictionary[source].Add(Time.realtimeSinceStartup - timeStarted);
		}

		public void PrintFindings()
		{
			foreach(var pair in timeDictionary)
			{
				var total = 0.0f;
				foreach(float time in pair.Value)
				{
					total += time;
				}
				var average = total / pair.Value.Count;

				OutputLogger.LogFormat("{0} took an average of {1} seconds to complete.", Enums.LogSource.PROFILE, pair.Key, average);
			}

			timeDictionary.Clear();
		}
	}
}