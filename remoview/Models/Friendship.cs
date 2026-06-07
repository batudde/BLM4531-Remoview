namespace remoview.Models
{
    public enum FriendshipStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }

    public class Friendship
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int AddresseeId { get; set; }
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAtUtc { get; set; }

        public virtual User Requester { get; set; } = null!;
        public virtual User Addressee { get; set; } = null!;
    }
}
