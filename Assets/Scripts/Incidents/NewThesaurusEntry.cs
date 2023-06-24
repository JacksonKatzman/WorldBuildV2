using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	[HideReferenceObjectPicker]
    public class NewThesaurusEntry
	{
        public string key;
        [HideReferenceObjectPicker]
        public List<string> entries;
        private Action<NewThesaurusEntry> onComplete;

        public NewThesaurusEntry(string key, ThesaurusEntry entry, Action<NewThesaurusEntry> onComplete)
		{
            this.key = key;
            this.entries = ThesaurusProvider.ConvertEntryToStringList(entry.SynonymsString);
            this.onComplete = onComplete;
		}

        [Button("Add Entry")]
        public void AddEntry()
		{
            if(ThesaurusProvider.AddEntry(key, entries))
			{
                onComplete.Invoke(this);
			}
		}
	}
}