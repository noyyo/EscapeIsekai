using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class ItemExcel : ScriptableObject
{
	public List<ItemData> ItemDatas; // Replace 'EntityType' to an actual type that is serializable.
}
