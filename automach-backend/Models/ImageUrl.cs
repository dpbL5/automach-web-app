using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace automach_backend.Models
{
    public class ImageUrl
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int GameId { get; set; }
        public Game? Game { get; set; }
    }
}