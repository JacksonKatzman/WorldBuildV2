using Game.Debug;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Profiling;
using UnityEngine;
public class GameProfiler : MonoBehaviour
{
    private class MinMax
	{
        public double min;
        public double max;
	}
    public enum ProfileFunctionType { UPDATE, DEPLOY, UTILITY };
    public static ProfilerMarker worldUpdateMarker = new ProfilerMarker("world.update");

    //public Dictionary<ProfilerRecorder, List<double>> times;
    private static Dictionary<string, ProfilerMarker> markers;
    private static Dictionary<ProfilerMarker, ProfilerRecorder> recorders;
    private static Dictionary<ProfilerRecorder, MinMax> timings;
    public static bool UpdateProfiler
	{
        get
		{
            return updateProfiler;
		}
		set
		{
            updateProfiler = value;
            if (updateProfiler == false)
            {
                PrintRanges();
            }
		}
	}

    string statsText;
    private static bool updateProfiler;
    //ProfilerRecorder systemMemoryRecorder;
    //ProfilerRecorder gcMemoryRecorder;
    //ProfilerRecorder mainThreadTimeRecorder;
    //ProfilerRecorder worldUpdateTimeRecorder;

    public TMPro.TextMeshProUGUI profilerStatsText;

    public static void BeginProfiling(string markerName, ProfileFunctionType profileFunctionType)
	{
        if (markers != null)
        {
            if (!markers.ContainsKey(markerName))
            {
                var m = new ProfilerMarker($"{markerName}.{profileFunctionType.ToString()}");
                markers.Add(markerName, m);
            }

            var marker = markers[markerName];
            if (!recorders.ContainsKey(marker))
            {
                var r = ProfilerRecorder.StartNew(marker);
                recorders.Add(marker, r);
            }

            marker.Begin();
        }
	}

    public static void EndProfiling(string markerName)
	{
        if (markers != null)
        {
            markers[markerName].End();
        }
	}

    double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;

        var samples = new List<ProfilerRecorderSample>(samplesCount);
        recorder.CopyTo(samples);
        if (samples.Count > 0)
        {
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
        }
        r /= samplesCount;

        if(!timings.ContainsKey(recorder))
		{
            timings.Add(recorder, new MinMax { min = r, max = r});
		}

        var minMax = timings[recorder];
        if(r < timings[recorder].min)
		{
            timings[recorder].min = r;
		}
        if(r > timings[recorder].max)
		{
            timings[recorder].max = r;
		}

        /*
        if(!times.ContainsKey(recorder))
		{
            times.Add(recorder, new List<double>());
		}
        times[recorder].Add(r);
        
        return GetAverage(times[recorder]);
        */

        return r;
    }

    double GetAverage(List<double> entries)
	{
        double total = 0;
        foreach(var entry in entries)
		{
            total += entry;
		}
        return total / entries.Count;
	}

	private void Awake()
	{
    }

	void OnEnable()
    {
        markers = new Dictionary<string, ProfilerMarker>();
        recorders = new Dictionary<ProfilerMarker, ProfilerRecorder>();
        timings = new Dictionary<ProfilerRecorder, MinMax>();
        //times = new Dictionary<ProfilerRecorder, List<double>>();
        //systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        //gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        //mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        //worldUpdateTimeRecorder = ProfilerRecorder.StartNew(worldUpdateMarker);
    }

    void OnDisable()
    {
        //systemMemoryRecorder.Dispose();
        //gcMemoryRecorder.Dispose();
        //mainThreadTimeRecorder.Dispose();
        //worldUpdateTimeRecorder.Dispose();
        foreach(var recorder in recorders)
		{
            recorder.Value.Dispose();
		}
        recorders.Clear();
    }

    void Update()
    {
        if (UpdateProfiler)
        {

            var sb = new StringBuilder(500);
            // sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
            //sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
            //sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
            //sb.AppendLine($"World Update Time: {GetRecorderFrameAverage(worldUpdateTimeRecorder) * (1e-6f):F1} ms");
            foreach (var pair in markers)
            {
                //sb.AppendLine($"{pair.Key} Time: {GetRecorderFrameAverage(pair.Value) * (1e-6f):F1} ms");
                sb.AppendLine($"{pair.Key} Time: {GetRecorderFrameAverage(recorders[pair.Value]) * (1e-6f):F1} ms");
                //sb.AppendLine($"{pair.Key} Min: {timings[recorders[pair.Value]].min * (1e-6f):F1} ms, Max: {timings[recorders[pair.Value]].max * (1e-6f):F1} ms");
            }
            //profilerStatsText.text = sb.ToString();
            
        }
    }

    private static void PrintRanges()
	{
        if (markers != null)
        {
            foreach (var pair in markers)
            {
                if(!recorders.ContainsKey(pair.Value))
				{
                    OutputLogger.LogError("No recorder found for marker: " + pair.Key);
                    continue;
				}
                if(!timings.ContainsKey(recorders[pair.Value]))
				{
                    OutputLogger.LogError("No timings found for marker: " + pair.Key);
                    continue;
                }
                OutputLogger.Log($"{pair.Key} Min: {timings[recorders[pair.Value]].min * (1e-6f):F1} ms, Max: {timings[recorders[pair.Value]].max * (1e-6f):F1} ms");
            }
        }
	}
}
