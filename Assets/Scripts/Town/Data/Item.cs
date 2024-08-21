namespace Assets.Scripts.Town.Data
{
    public class Item
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
        public string item_type { get; set; }
        public int item_hp { get; set; }
        public int item_mp { get; set; }
        public int item_attack { get; set; }
        public int item_defense { get; set; }
        public int item_magic { get; set; }
        public int item_speed { get; set; }
        public int item_cost { get; set; }
        public bool can_sell { get; set; }
        public int require_level { get; set; }
        public float item_avoidance { get; set; }
        public float item_critical { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
