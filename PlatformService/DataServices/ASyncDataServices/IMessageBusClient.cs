using PlatformService.Dtos;

namespace PlatformService.DataServices.ASyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishDto platformPublishDto);
    }
}
