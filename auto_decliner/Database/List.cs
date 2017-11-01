using System.Collections.Generic;

namespace LatinAutoDecline.Database
{
    public class List
    {
        public List()
        {
            Users = new HashSet<ListUser>();
            Sections = new HashSet<Section>();
        }

        public int ListId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsPrivate { get; set; }
        public ICollection<ListUser> Users { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}
