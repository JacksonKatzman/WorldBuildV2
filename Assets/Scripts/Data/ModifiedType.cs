using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiedType<T>
{
    public T original;
    public T modified;

	public ModifiedType(T original)
	{
		this.original = original;
		this.modified = original;
	}
}
