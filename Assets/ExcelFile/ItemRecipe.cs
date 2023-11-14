using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemRecipe
{
    [SerializeField] private int _id;
    [SerializeField] private int _craftingID;
    [SerializeField] private int _craftingPrice;
    [SerializeField] private string _materials_string;
    [SerializeField] private string _materials_count_string;
    [SerializeField] private int _availableCount;
    private int[] _materials;
    private int[] _materialsCount;
    private bool isMaterialsReset = true;
    private bool isMaterialCountReset = true;

    public int ID { get {  return _id; } }
    public int CraftingID { get { return _craftingID; } }
    public int CraftingPrice { get { return _craftingPrice; } }
    public int AvailableCount { get { return _availableCount; } }
    public int[] Materials 
    { 
        get 
        {
            if (isMaterialsReset)
            {
                _materials = _materials_string.Split(',').Select(s => int.Parse(s)).ToArray();
                isMaterialsReset = false;
            }
            return _materials; 
        }
    }

    public int[] MaterialsCount
    {
        get
        {
            if (isMaterialCountReset)
            {
                _materialsCount = _materials_count_string.Split(',').Select(s => int.Parse(s)).ToArray();
                isMaterialCountReset = false;
            }
            return _materialsCount;
        }
    }
}
