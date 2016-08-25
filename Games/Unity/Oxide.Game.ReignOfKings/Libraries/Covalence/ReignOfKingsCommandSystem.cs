﻿using System.Collections.Generic;

using CodeHatch.Engine.Core.Commands;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;

namespace Oxide.Game.ReignOfKings.Libraries.Covalence
{
    /// <summary>
    /// Represents a binding to a generic command system
    /// </summary>
    public class ReignOfKingsCommandSystem : ICommandSystem
    { 
        // Default constructor
        public ReignOfKingsCommandSystem()
        {
            Initialize();
        }

        // Chat command handler
        private ChatCommandHandler commandHandler;

        // All registered commands
        private IDictionary<string, CommandCallback> registeredCommands;

        /// <summary>
        /// Initializes the command system provider
        /// </summary>
        private void Initialize()
        {
            registeredCommands = new Dictionary<string, CommandCallback>();
            commandHandler = new ChatCommandHandler(ChatCommandCallback, registeredCommands.ContainsKey);
        }

        private bool ChatCommandCallback(IPlayer caller, string command, string[] args)
        {
            CommandCallback callback;
            return registeredCommands.TryGetValue(command, out callback) && callback(caller, command, args);
        }

        /// <summary>
        /// Registers the specified command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        public void RegisterCommand(string command, Plugin plugin, CommandCallback callback)
        {
            // No console command support so no need to register the command as console command
            // Register the command as a chat command
            // Convert to lowercase
            var commandName = command.ToLowerInvariant();

            // Check if it already exists
            if (CommandManager.RegisteredCommands.ContainsKey(commandName) || registeredCommands.ContainsKey(commandName))
                throw new CommandAlreadyExistsException(commandName);

            registeredCommands.Add(commandName, callback);
        }

        /// <summary>
        /// Unregisters the specified command
        /// </summary>
        /// <param name="command"></param>
        public void UnregisterCommand(string command, Plugin plugin) => CommandManager.RegisteredCommands.Remove(command);

        /// <summary>
        /// Handles a chat message
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HandleChatMessage(IPlayer player, string message) => commandHandler.HandleChatMessage(player, message);
    }
}
