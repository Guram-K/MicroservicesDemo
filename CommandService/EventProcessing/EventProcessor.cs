using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using CommandsService.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformAdd:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string eventMessage)
        {
            Console.WriteLine("--> Determining Event type");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(eventMessage);

            switch (eventType.Event)
            {
                case "platform-add":
                    return EventType.PlatformAdd;
                default:
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishDto);

                    if (!repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();

                        Console.WriteLine($"--> Platform Added");
                    }
                    else
                    {
                        Console.WriteLine($"--> Platform already exists!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform to DB");
                }
            }
        }
    }

    enum EventType 
    { 
        PlatformAdd,
        Undetermined
    }
}
