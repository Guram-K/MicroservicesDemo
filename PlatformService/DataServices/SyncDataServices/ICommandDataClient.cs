using PlatformService.Dtos;
using System.Threading.Tasks;

namespace PlatformService.DataServices.SyncDataServices
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto plat);
    }
}
