using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();

        IEnumerable<Platformm> GetAllPlatforms();
        Platformm GetPlatformById(int id);
        void CreatePlatform(Platformm plat);
    }
}