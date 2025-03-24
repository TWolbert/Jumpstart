using System;
using PapenChat.Framework.Database;

namespace PapenChat.Models
{
    public class Users : BaseModel<Users>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? ImageID { get; set; }
        public string Bio { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}

