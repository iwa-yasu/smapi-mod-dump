﻿using Newtonsoft.Json.Linq;
using QuestFramework.Framework.Store;
using QuestFramework.Hooks;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace QuestFramework.Quests
{
    /// <summary>
    /// Custom quest definition
    /// </summary>
    public class CustomQuest
    {
        private int customTypeId;
        private string trigger;

        internal int id = -1;
        public string Name { get; set; }
        public string OwnedByModUid { get; internal set; }
        public QuestType BaseType { get; set; } = QuestType.Basic;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Objective { get; set; }
        public List<string> NextQuests { get; set; }
        public int Reward { get; set; }
        public string RewardDescription { get; set; }
        public bool Cancelable { get; set; }
        public string ReactionText { get; set; }
        public int DaysLeft { get; set; } = 0;
        public List<Hook> Hooks { get; set; }
        
        public string Trigger 
        {
            get => this.trigger;
            set
            {
                if (this is ITriggerLoader triggerLoader)
                {
                    triggerLoader.LoadTrigger(value);
                }

                this.trigger = value;
            }
        }

        public bool IsDailyQuest()
        {
            return this.DaysLeft > 0;
        }

        public int CustomTypeId 
        { 
            get => this.BaseType == QuestType.Custom ? this.customTypeId : -1; 
            set => this.customTypeId = value >= 0 ? value : 0; 
        }

        internal protected static IModHelper Helper => QuestFrameworkMod.Instance.Helper;
        internal protected static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;

        public CustomQuest()
        {
            this.BaseType = QuestType.Custom;
            this.NextQuests = new List<string>();
            this.Hooks = new List<Hook>();
        }

        public CustomQuest(string name) : this()
        {
            this.Name = name;
        }


        /// <summary>
        /// Get full quest name in format {questName}@{ownerModUniqueId}
        /// </summary>
        public string GetFullName()
        {
            return $"{this.Name}@{this.OwnedByModUid}";
        }

        /// <summary>
        /// Cast this custom quest as statefull custom quest if this quest contains state.
        /// </summary>
        /// <returns>A statefull custom quest or null if this quest not contains or hasn't defined type of state</returns>
        public IStatefull AsStatefull()
        {
            return this as IStatefull;
        }

        /// <summary>
        /// Cast this custom quest as statefull custom quest with specific state type.
        /// </summary>
        /// <typeparam name="TState">Type of state</typeparam>
        /// <returns>A statefull custom quest with specific type of state or null when quest has no state or doesn't match state type.</returns>
        public CustomQuest<TState> AsStatefull<TState>() where TState : class, new()
        {
            return this as CustomQuest<TState>;
        }

        internal string NormalizeName(string name)
        {
            if (name.Contains("@"))
                return name;

            return $"{name}@{this.OwnedByModUid}";
        }
    }

    /// <summary>
    /// Custom quest definition with custom local state
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class CustomQuest<TState> : CustomQuest, IStatefull<TState>, IStateRestorable where TState : class, new()
    {
        /// <summary>
        /// A state data
        /// </summary>
        public TState State { get; private set; }

        /// <summary>
        /// Create custom quest with state (Statefull quest)
        /// </summary>
        public CustomQuest() : base()
        {
            this.State = new TState();
        }

        /// <summary>
        /// Create custom quest with state (Statefull quest)
        /// </summary>
        /// <param name="name">Name of the quest</param>
        public CustomQuest(string name) : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Sync quest state with the store (singleplayer or mainplayer)
        /// or with host (server) via network in multiplayer game.
        /// 
        /// CALL THIS METHOD EVER WHEN YOU CHANGED QUEST STATE!
        /// </summary>
        public void Sync()
        {
            var payload = new StatePayload(
                questName: this.GetFullName(),
                farmerId: Game1.player.UniqueMultiplayerID,
                stateData: JToken.FromObject(this.State)
             );

            if (Context.IsMainPlayer)
            {
                QuestFrameworkMod.Instance.QuestStateStore.Commit(payload);
            } else
            {
                Helper.Multiplayer.SendMessage(
                    payload, "SyncState", new[] { QuestFrameworkMod.Instance.ModManifest.UniqueID });
                Monitor.Log($"Payload `{payload.QuestName}/{payload.FarmerId}` type `{payload.StateData.Type}` sent to sync to host.");
            }
        }
        
        void IStateRestorable.RestoreState(StatePayload payload)
        {
            if (payload.StateData == null)
            {
                this.State = new TState();
                return;
            }

            this.State = payload.StateData.ToObject<TState>();
        }

        /// <summary>
        /// Reset quest state to default state data
        /// </summary>
        public void ResetState()
        {
            this.State = new TState();
            this.Sync();
        }
    }
}
