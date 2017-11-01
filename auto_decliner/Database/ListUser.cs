namespace LatinAutoDecline.Database
{
    public class ListUser
    {
        public string UserId { get; set; }
        public int ListId { get; set; }
        public bool IsOwner { get; set; }
        public bool IsContributor { get; set; }
        public bool IsLearning { get; set; }
        public List List { get; set; }
    }
}
