using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace automach_backend.Helpers
{
    public class QueryObject
    {
        public string? Title { get; set; } = null!;
        public string? GameTag { get; set; } = null!;
        public List<string>? GameTags { get; set; } = null!;
        public bool? IsFeatured { get; set; } = null!;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public float? MaxPrice { get; set; } = null!;
    }
}