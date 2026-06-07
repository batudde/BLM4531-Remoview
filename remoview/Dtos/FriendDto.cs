namespace remoview.Dtos
{
    public class FriendUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfileDescription { get; set; }
        public int FavoriteCount { get; set; }
        public int FriendCount { get; set; }
        public List<FilmListDto> FavoriteFilms { get; set; } = new();
    }

    public class FriendSearchResultDto : FriendUserDto
    {
        public string FriendshipStatus { get; set; } = "none";
    }

    public class FriendRequestDto
    {
        public int Id { get; set; }
        public FriendUserDto User { get; set; } = new();
        public DateTime CreatedAtUtc { get; set; }
    }
}
