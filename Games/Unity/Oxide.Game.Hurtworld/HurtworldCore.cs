﻿using System.Collections.Generic;
using System.Text;

using UnityEngine;
using NetworkMessageInfo = uLink.NetworkMessageInfo;
using NetworkPlayer = uLink.NetworkPlayer;

using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Game.Hurtworld.Libraries;

namespace Oxide.Game.Hurtworld
{
    /// <summary>
    /// The core Hurtworld plugin
    /// </summary>
    public class HurtworldCore : CSPlugin
    {
        // The pluginmanager
        private readonly PluginManager pluginmanager = Interface.Oxide.RootPluginManager;

        // The permission library
        private readonly Permission permission = Interface.Oxide.GetLibrary<Permission>();
        private static readonly string[] DefaultGroups = { "default", "moderator", "admin" };

        // The command library
        private readonly Command cmdlib = Interface.Oxide.GetLibrary<Command>();

        // Track when the server has been initialized
        private bool serverInitialized;
        private bool loggingInitialized;

        /// <summary>
        /// Initializes a new instance of the HurtworldCore class
        /// </summary>
        public HurtworldCore()
        {
            // Set attributes
            Name = "hurtworldcore";
            Title = "Hurtworld Core";
            Author = "Oxide Team";
            Version = new VersionNumber(1, 0, 0);

            var plugins = Interface.Oxide.GetLibrary<Core.Libraries.Plugins>();
            if (plugins.Exists("unitycore")) InitializeLogging();
        }

        /// <summary>
        /// Called when the plugin is initializing
        /// </summary>
        [HookMethod("Init")]
        private void Init()
        {
            // Configure remote logging
            RemoteLogger.SetTag("game", "hurtworld");
            RemoteLogger.SetTag("version", GameManager.Instance.GetProtocolVersion().ToString());
        }

        /// <summary>
        /// Called when the server is first initialized
        /// </summary>
        [HookMethod("OnServerInitialized")]
        private void OnServerInitialized()
        {
            if (serverInitialized) return;
            serverInitialized = true;

            // Configure the hostname after it has been set
            RemoteLogger.SetTag("hostname", GameManager.Instance.ServerConfig.GameName);
        }

        /// <summary>
        /// Called when a plugin is loaded
        /// </summary>
        /// <param name="plugin"></param>
        [HookMethod("OnPluginLoaded")]
        private void OnPluginLoaded(Plugin plugin)
        {
            if (serverInitialized) plugin.CallHook("OnServerInitialized");
            if (!loggingInitialized && plugin.Name == "unitycore") InitializeLogging();
        }

        /// <summary>
        /// Starts the logging
        /// </summary>
        private void InitializeLogging()
        {
            loggingInitialized = true;
            CallHook("InitLogging", null);
        }

        /// <summary>
        /// Called when a console command was run
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HookMethod("OnRunCommand")]
        private object OnRunCommand(string arg) => arg == null || arg.Trim().Length == 0 ? null : cmdlib.HandleConsoleCommand(arg);

        /// <summary>
        /// Called when the player sends a message
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="info"></param>
        /// <param name="message"></param>
        [HookMethod("OnPlayerChat")]
        private object OnPlayerChat(PlayerIdentity identity, NetworkMessageInfo info, string message)
        {
            if (message.Trim().Length <= 1) return true;

            var str = message.Substring(0, 1);

            // Is it a chat command?
            if (!str.Equals("/") && !str.Equals("!")) return null;

            // Get the arg string
            var argstr = message.Substring(1);

            // Parse it
            string chatcmd;
            string[] args;
            ParseChatCommand(argstr, out chatcmd, out args);
            if (chatcmd == null) return null;

            // Handle it
            if (!cmdlib.HandleChatCommand(identity, info, chatcmd, args))
            {
                ChatManager.Instance.AppendChatboxServerSingle($"<color=#b8d7a3>Unknown command: {chatcmd}</color>", info.sender);
                return true;
            }

            return true;
        }

        /// <summary>
        /// Parses the specified chat command
        /// </summary>
        /// <param name="argstr"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        private void ParseChatCommand(string argstr, out string cmd, out string[] args)
        {
            var arglist = new List<string>();
            var sb = new StringBuilder();
            var inlongarg = false;

            foreach (var c in argstr)
            {
                if (c == '"')
                {
                    if (inlongarg)
                    {
                        var arg = sb.ToString().Trim();
                        if (!string.IsNullOrEmpty(arg)) arglist.Add(arg);
                        sb = new StringBuilder();
                        inlongarg = false;
                    }
                    else
                    {
                        inlongarg = true;
                    }
                }
                else if (char.IsWhiteSpace(c) && !inlongarg)
                {
                    var arg = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(arg)) arglist.Add(arg);
                    sb = new StringBuilder();
                }
                else
                {
                    sb.Append(c);
                }
            }
            if (sb.Length > 0)
            {
                var arg = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(arg)) arglist.Add(arg);
            }
            if (arglist.Count == 0)
            {
                cmd = null;
                args = null;
                return;
            }
            cmd = arglist[0];
            arglist.RemoveAt(0);
            args = arglist.ToArray();
        }

        /// <summary>
        /// Called when the player has connected
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="player"></param>
        [HookMethod("OnPlayerConnected")]
        private void OnPlayerConnected(PlayerIdentity identity, NetworkPlayer player)
        {
            // Let covalence know
            Libraries.Covalence.HurtworldCovalenceProvider.Instance.PlayerManager.NotifyPlayerConnect(player);

            // TODO: Do permission stuff
        }

        /// <summary>
        /// Called when the player has disconnected
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="player"></param>
        [HookMethod("OnPlayerDisconnected")]
        private void OnPlayerDisconnected(PlayerIdentity identity, NetworkPlayer player)
        {
            // Let covalence know
            Libraries.Covalence.HurtworldCovalenceProvider.Instance.PlayerManager.NotifyPlayerDisconnect(player);
        }

        /// <summary>
        /// Sends game logging to the Unity log
        /// </summary>
        [HookMethod("IOnServerLog")]
        private void IOnServerLog(string text) => Debug.Log(text);
    }
}