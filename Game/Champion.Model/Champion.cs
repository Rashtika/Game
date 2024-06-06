namespace Game.Model
{
    public class Champion
    {
        public Guid Id { get; set; }
        public string? Name { set; get; }
        public bool IsActive { get; set; }
        public Guid InventoryId { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public Inventory? Inventory { get; set; }
        
    }
}
