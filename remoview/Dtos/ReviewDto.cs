namespace remoview.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        // public string UserName { get; set; } // İleride kullanıcının adını göstermek için
    }
}