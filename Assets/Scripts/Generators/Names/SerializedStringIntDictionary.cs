using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerializedStringIntDictionary : UnitySerializedDictionary<string, int>
{
	public static SerializedStringIntDictionary Merge(SerializedStringIntDictionary a, SerializedStringIntDictionary b)
	{
		var result = new SerializedStringIntDictionary();

		result = (SerializedStringIntDictionary)a.Union(b).ToDictionary(pair => pair.Key, pair => pair.Value);

		return result;
	}
}
