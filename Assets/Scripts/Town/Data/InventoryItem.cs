using Assets.Scripts.Town.Data;

public class InventoryItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public Item ItemData { get; set; }
}

public class EquippedItems
{
    public Item Weapon;
    public Item Armor;
    public Item Gloves;
    public Item Shoes;
    public Item Accessory;


    public EquippedItems(Item weapon, Item armor, Item gloves, Item shoes, Item accessory )
    {
        Weapon = weapon;
        Armor = armor;
        Gloves = gloves;
        Shoes = shoes;
        Accessory = accessory;
    }
}