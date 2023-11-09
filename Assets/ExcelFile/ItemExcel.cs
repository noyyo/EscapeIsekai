using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class ItemExcel : ScriptableObject
{
	public List<ItemData_Test> ItemDatas; // Replace 'EntityType' to an actual type that is serializable.
	public List<ItemStats> Stats; // Replace 'EntityType' to an actual type that is serializable.
	public List<ItemRecipe> Recipe; // Replace 'EntityType' to an actual type that is serializable.
}
