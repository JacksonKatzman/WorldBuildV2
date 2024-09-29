using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	[Serializable, HideReferenceObjectPicker]
    public class AdventureComponentTextField
    {
		public AdventureComponentTextFieldType fieldType;

		[TextArea(1, 60), PropertyOrder(0), SerializeField, ShowIf("@this.fieldType == AdventureComponentTextFieldType.TEXT")]
		private string text;

		[SerializeField, ListDrawerSettings(Expanded = true, CustomAddFunction = "AddBullet"), ShowIf("@this.fieldType == AdventureComponentTextFieldType.BULLETLIST")]
		private List<AdventureComponentTextArea> bulletList = new List<AdventureComponentTextArea>();

		public string Text => CreateText();
		public AdventureComponentTextField() { }
		public AdventureComponentTextField(string defaultText)
        {
			text = defaultText;
        }

		public void UpdateIDs(int oldID, int newID)
		{
			text = text.Replace($":{oldID}}}", $":{newID}}}");
		}

		public string CreateText()
        {
			if(fieldType == AdventureComponentTextFieldType.BULLETLIST)
            {
				var bullets = string.Empty;
				foreach(var bullet in bulletList)
                {
					bullets += $"\u2022 {bullet.text} \n";
                }
				return bullets.Trim('\n');
            }
			else
            {
				return text;
            }
        }

		public void SetSingleText(string text)
        {
			this.text = text;
			fieldType = AdventureComponentTextFieldType.TEXT;
        }

		private void AddBullet()
        {
			bulletList.Add(new AdventureComponentTextArea());
        }
	}

	public enum AdventureComponentTextFieldType
    {
		TEXT = 0,
		BULLETLIST = 1
    }

	[HideReferenceObjectPicker]
	public class AdventureComponentTextArea
    {
		[TextArea(1, 30)]
		public string text;
    }
}
