using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController:ControllerBase
    {
        private readonly IPlatformRepo _repositary;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repositary,IMapper mapper ,ICommandDataClient commandDataClient)
        {
            _repositary=repositary;
            _mapper=mapper;
            _commandDataClient=commandDataClient;
        }   

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            Console.WriteLine("--> Geting All Platforms");
            var platformItem=_repositary.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }
        [HttpGet("{id}")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem=_repositary.GetPlatformById(id);
            if(platformItem!=null)
            {
            return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            return NotFound("Invalid id : "+id);
        }
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel=_mapper.Map<Platform>(platformCreateDto);
            _repositary.CreatePlatform(platformModel);
            _repositary.SaveChanges();
            var platformReadDto=_mapper.Map<PlatformReadDto>(platformModel);
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }catch(Exception ex)
            {
                Console.WriteLine($"-> Sync call to command service does not set up {ex.Message}");
            }
            return Ok(platformReadDto);
        }

    }
}