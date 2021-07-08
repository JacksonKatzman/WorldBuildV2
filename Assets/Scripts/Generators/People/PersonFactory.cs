using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonFactory
{
	private List<Person> storage;
	public PersonFactory()
	{
		storage = new List<Person>();
	}
}
