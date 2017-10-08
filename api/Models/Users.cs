using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class Users
    {
        public Users()
        {
            UserLearntWords = new HashSet<UserLearntWords>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }

        public ICollection<UserLearntWords> UserLearntWords { get; set; }
    }
}
