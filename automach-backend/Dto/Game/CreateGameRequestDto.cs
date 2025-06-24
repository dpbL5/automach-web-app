using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace automach_backend.Dto.Game
{
    public class CreateGameRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public float Price { get; set; }
        public string GameInfo { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string Developer { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }

    }

    
}