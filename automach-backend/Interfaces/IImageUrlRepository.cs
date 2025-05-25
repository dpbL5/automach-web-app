using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IImageUrlRepository
    {
        Task<ICollection<ImageUrl>> GetImagesByGameIdAsync(int gameId);
        Task<ImageUrl?> GetByIdAsync(int id);
        Task<ImageUrl> CreateAsync(ImageUrl imageUrl);
        Task<ImageUrl?> DeleteAsync(int id);
    }
}