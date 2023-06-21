using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Game.Incidents
{
	static public class ThesaurusEntryRetriever
	{
		public static ThesaurusObjectRoot GetSynonyms(string word)
		{
			var url = string.Format("http://thesaurus.altervista.org/thesaurus/v1?word={0}&language=en_US&key={1}&output=json", word, "dB0XpkI8WTt8ZZ06IONW");
			var httpClientHandler = new HttpClientHandler();
			var httpClient = new HttpClient(httpClientHandler)
			{
				BaseAddress = new Uri(url)
			};
			using (var response = httpClient.GetAsync(url))
			{
				string responseBody = response.Result.Content.ReadAsStringAsync().Result;
				var item = JsonConvert.DeserializeObject<ThesaurusObjectRoot>(responseBody);
				return item;
				/*
				if(item.Containers.Count > 0)
				{
					ThesaurusProvider.AddEntry(word.ToUpper(), item.Containers[0].Entry);
				}
				*/
			}
		}

		public class ThesaurusObjectRoot
		{
			[JsonProperty("response")]
			public List<ThesaurusEntryContainer> Containers { get; set; }
		}

		public class ThesaurusEntryContainer
		{
			[JsonProperty("list")]
			public ThesaurusEntry Entry { get; set; }
		}
	}

	public class ThesaurusEntry
	{
		[JsonProperty("category")]
		public string Category { get; set; }
		[JsonProperty("synonyms")]
		public string SynonymsString { get; set; }
	}
}