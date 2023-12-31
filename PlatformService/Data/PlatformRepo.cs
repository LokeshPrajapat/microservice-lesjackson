using PlatformService.Models;
using System;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;
        public PlatformRepo(AppDbContext context)
        {
            _context=context;
        }   
        public void CreatePlatform(Platform platform)
        {
           if(platform==null)
           {
                throw new Exception("null");
                
           }
           _context.Platforms.Add(platform);
           _context.SaveChanges();

        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
           return _context.Platforms.FirstOrDefault(p=>p.Id==id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >=0);
        }
    }
}