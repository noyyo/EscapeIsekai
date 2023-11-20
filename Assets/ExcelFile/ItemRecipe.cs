using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemRecipe
{
    [SerializeField] private int id;
    [SerializeField] private int craftingID;
    [SerializeField] private int craftingPrice;
    [SerializeField] private string materials_string;
    [SerializeField] private string materials_count_string;
    [SerializeField] private int availableCount;
    private int[] materials;
    private int[] materialsCount;
    private bool isMaterialsReset = true;
    private bool isMaterialCountReset = true;

    public int ID { get {  return id; } }
    public int CraftingID { get { return craftingID; } }
    public int CraftingPrice { get { return craftingPrice; } }
    public int AvailableCount { get { return availableCount; } }
    public int[] Materials 
    { 
        get 
        {
            if (isMaterialsReset)
            {
                materials = materials_string.Split(',').Select(s => int.Parse(s)).ToArray();
                isMaterialsReset = false;
            }
            return materials; 
        }
    }

    public int[] MaterialsCount
    {
        get
        {
            if (isMaterialCountReset)
            {
                materialsCount = materials_count_string.Split(',').Select(s => int.Parse(s)).ToArray();
                isMaterialCountReset = false;
            }
            return materialsCount;
        }
    }
}
