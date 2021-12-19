﻿using CommandsService.Dtos;
using CommandsService.Models;
using System.Collections.Generic;

namespace CommandsService.Repositories
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        //Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExists(int platformId);
        bool ExternalPlatformExists(int externalPlatformId);

        //Commands
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}
