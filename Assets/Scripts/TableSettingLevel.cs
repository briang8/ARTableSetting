using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDefinition
{
    public string itemType;
    public string addressKey;
    public string slotId;
}

[CreateAssetMenu(fileName = "NewLevel", menuName = "Table Setting/Level")]
public class TableSettingLevel : ScriptableObject
{
    public string levelName;
    public float timeLimit = 120f;
    public int passMark = 70;
    public string scoringType = "Basic"; // "Basic" or "TimeBonus"
    public List<ItemDefinition> items = new List<ItemDefinition>();
}