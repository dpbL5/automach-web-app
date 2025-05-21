using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace automach_backend.Interfaces
{
    public interface IImageUrlRepository
    {
        public Task<List<string>> GetImageUrlsByGameId(int gameId);
        public Task<string> CreateImageUrl(string url, int gameId);
        public Task<bool> DeleteImageUrl(int id);
        public Task<bool> UpdateImageUrl(int id, string url);
        public Task<bool> DeleteImageUrlsByGameId(int gameId); 
    }
}