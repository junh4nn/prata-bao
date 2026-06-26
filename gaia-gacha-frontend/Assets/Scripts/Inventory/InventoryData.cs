using System;

[Serializable]
public class InventoryItemRow
{
    public int    itemId;
    public string name;
    public string rarity;
    public string type;
    public int    count;
    public string firstObtainedAt;
}

[Serializable]
public class InventoryResponse
{
    public InventoryItemRow[] items;
}
