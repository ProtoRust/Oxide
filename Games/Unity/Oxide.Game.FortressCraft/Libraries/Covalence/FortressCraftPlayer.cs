﻿using System;

using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Game.FortressCraft.Libraries.Covalence
{
    /// <summary>
    /// Represents a player, either connected or not
    /// </summary>
    public class FortressCraftPlayer : IPlayer, IEquatable<IPlayer>
    {
        private static Permission libPerms;
        private readonly Player player;

        internal FortressCraftPlayer(ulong id, string name)
        {
            // Get perms library
            if (libPerms == null) libPerms = Interface.Oxide.GetLibrary<Permission>();

            // Store user details
            Name = name;
            Id = id.ToString();
        }

        internal FortressCraftPlayer(Player player) : this(player.mUserID, player.mUserName)
        {
            // Store user object
            this.player = player;
        }

        #region Objects

        /// <summary>
        /// Gets the object that backs the user
        /// </summary>
        public object Object => player;

        /// <summary>
        /// Gets the user's last command type
        /// </summary>
        public CommandType LastCommand { get; set; }

        #endregion

        #region Information

        /// <summary>
        /// Gets the name for the player
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the ID for the player (unique within the current game)
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the user's IP address
        /// </summary>
        public string Address
        {
            get
            {
                return null; // TODO: Implement once possible
            }
        }

        /// <summary>
        /// Gets the user's average network ping
        /// </summary>
        public int Ping => 0; // TODO: Implement once possible

        /// <summary>
        /// Returns if the user is admin
        /// </summary>
        public bool IsAdmin => NetworkManager.instance.mAdminListManager.CheckAdminList(player.mUserID, player.mUserName);

        /// <summary>
        /// Gets if the user is banned
        /// </summary>
        public bool IsBanned => NetworkManager.instance.mBanListManager.CheckBan(player.mUserID);

        /// <summary>
        /// Returns if the user is connected
        /// </summary>
        public bool IsConnected => false; // TODO: Implement once possible

        /// <summary>
        /// Returns if the user is sleeping
        /// </summary>
        public bool IsSleeping => false;

        #endregion

        #region Administration

        /// <summary>
        /// Bans the user for the specified reason and duration
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="duration"></param>
        public void Ban(string reason, TimeSpan duration = default(TimeSpan))
        {
            // Check if already banned
            if (IsBanned) return;

            // Ban and kick user
            NetworkManager.instance.mBanListManager.UnbanPlayer(player.mUserID);
            NetworkManager.instance.mServerThread.KickPlayer(player.mConnection, reason);
        }

        /// <summary>
        /// Gets the amount of time remaining on the user's ban
        /// </summary>
        public TimeSpan BanTimeRemaining => TimeSpan.MaxValue;

        /// <summary>
        /// Heals the user's character by specified amount
        /// </summary>
        /// <param name="amount"></param>
        public void Heal(float amount)
        {
            // TODO: Implement once possible
        }

        /// <summary>
        /// Gets/sets the user's health
        /// </summary>
        public float Health
        {
            get
            {
                return 0; // TODO: Implement when possible
            }
            set
            {
                // TODO: Implement once possible
            }
        }

        /// <summary>
        /// Damages the user's character by specified amount
        /// </summary>
        /// <param name="amount"></param>
        public void Hurt(float amount)
        {
            // TODO: Implement when possible
        }

        /// <summary>
        /// Kicks the user from the game
        /// </summary>
        /// <param name="reason"></param>
        public void Kick(string reason) => NetworkManager.instance.mServerThread.KickPlayer(player.mConnection, reason);

        /// <summary>
        /// Causes the user's character to die
        /// </summary>
        public void Kill() => player.mGameObject.GetComponent<SurvivalPlayerScript>().Die("");

        /// <summary>
        /// Gets/sets the user's maximum health
        /// </summary>
        public float MaxHealth
        {
            get
            {
                return 100f; // TODO: Implement when possible
            }
            set
            {
                // TODO: Implement when possible
            }
        }

        /// <summary>
        /// Renames the user to specified name
        /// <param name="name"></param>
        /// </summary>
        public void Rename(string name)
        {
            // TODO: Implement when possible
        }

        /// <summary>
        /// Teleports the user's character to the specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Teleport(float x, float y, float z) => player.TeleportTo((long)x, (long)y, (long)z, player.mLook);

        /// <summary>
        /// Unbans the user
        /// </summary>
        public void Unban()
        {
            // Check not banned
            if (!IsBanned) return;

            // Set to unbanned
            NetworkManager.instance.mBanListManager.UnbanPlayer(player.mUserID);
        }

        #endregion

        #region Location

        /// <summary>
        /// Gets the position of the user
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Position(out float x, out float y, out float z)
        {
            var pos = player.mLook.normalized;
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        /// <summary>
        /// Gets the position of the user
        /// </summary>
        /// <returns></returns>
        public GenericPosition Position()
        {
            var pos = player.mLook.normalized;
            return new GenericPosition(pos.x, pos.y, pos.z);
        }

        #endregion

        #region Chat and Commands

        /// <summary>
        /// Sends the specified message to the user
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Message(string message, params object[] args)
        {
            // TODO: Implement once possible
        }

        /// <summary>
        /// Replies to the user with the specified message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Reply(string message, params object[] args) => Message(message, args);

        /// <summary>
        /// Runs the specified console command on the user
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public void Command(string command, params object[] args)
        {
            // TODO: Implement once possible
        }

        #endregion

        #region Permissions

        /// <summary>
        /// Gets if the player has the specified permission
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        public bool HasPermission(string perm) => libPerms.UserHasPermission(Id, perm);

        /// <summary>
        /// Grants the specified permission on this user
        /// </summary>
        /// <param name="perm"></param>
        public void GrantPermission(string perm) => libPerms.GrantUserPermission(Id, perm, null);

        /// <summary>
        /// Strips the specified permission from this user
        /// </summary>
        /// <param name="perm"></param>
        public void RevokePermission(string perm) => libPerms.RevokeUserPermission(Id, perm);

        /// <summary>
        /// Gets if the player belongs to the specified usergroup
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool BelongsToGroup(string group) => libPerms.UserHasGroup(Id, group);

        /// <summary>
        /// Adds the player to the specified usergroup
        /// </summary>
        /// <param name="group"></param>
        public void AddToGroup(string group) => libPerms.AddUserGroup(Id, group);

        /// <summary>
        /// Removes the player from the specified usergroup
        /// </summary>
        /// <param name="group"></param>
        public void RemoveFromGroup(string group) => libPerms.RemoveUserGroup(Id, group);

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Returns if player's unique ID is equal to another player's unique ID
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPlayer other) => Id == other?.Id;

        /// <summary>
        /// Returns if player's object is equal to another player's object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is IPlayer && Id == ((IPlayer)obj).Id;

        /// <summary>
        /// Gets the hash code of the player's unique ID
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Returns a human readable string representation of this IPlayer
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Covalence.FortressCraftPlayer[{Id}, {Name}]";

        #endregion
    }
}
