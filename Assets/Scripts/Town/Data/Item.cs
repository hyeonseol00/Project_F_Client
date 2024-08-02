using System;

namespace Assets.Scripts.Town.Data
{
    [Serializable]
    public class Item
    {
        public int item_id;
        public string item_name;
        public string item_description;
        public string item_type;
        public float item_hp;
        public float item_mp;
        public float item_attack;
        public float item_defense;
        public float item_magic;
        public float item_speed;
        public int item_cost;
        public bool can_sell;
        public int require_level;
        public float item_avoidance;
        public float item_critical;
        public string created_at;
        public string updated_at;
    }
}
