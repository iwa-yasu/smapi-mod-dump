﻿
/*

Provides a few options to make Journey of the Prairie King minigame in Stardew
Valley easier

Copyright(C) 2020  shanks3042

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.

*/


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardewValleyEasyPrairieKing
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>

        private ModConfig config_;
        private const int INFINITE = 99;
        private Dictionary<string, bool> values_set_ = new Dictionary<string, bool>();
        //private IReflectedField<int> waveTimerCount;
        /// 

        public override void Entry(IModHelper helper)
        {
            this.config_ = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += eventUpdateTicks;
            this.values_set_.Add("lives", false);
            this.values_set_.Add("coins", false);
            this.config_.waveTimer *= 1000;

        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 
        /*
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }
        */
        private void eventUpdateTicks(object sender, EventArgs e)
        {
            if (Game1.currentMinigame == null || !"AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                return;
            }

            Type minigameType = Game1.currentMinigame.GetType();

            if (this.config_.lives_ > INFINITE)
            {
                minigameType.GetField("lives").SetValue(Game1.currentMinigame, INFINITE);
            }
            else if(this.config_.lives_ != 0 && !values_set_["lives"])
            {
                minigameType.GetField("lives").SetValue(Game1.currentMinigame, this.config_.lives_);
                values_set_["lives"] = true;
            }

            if (this.config_.coins_ > INFINITE)
            {
                minigameType.GetField("coins").SetValue(Game1.currentMinigame, INFINITE);
            }
            else if(this.config_.coins_ > INFINITE)
            {
                minigameType.GetField("coins").SetValue(Game1.currentMinigame, this.config_.coins_);
                values_set_["coins"] = true;
            }

            if(this.config_.ammo_level_ != 0 )
            {
                minigameType.GetField("ammoLevel").SetValue(Game1.currentMinigame, this.config_.ammo_level_);
            }

            if (this.config_.bullet_damage_ != 0)
            {
                minigameType.GetField("bulletDamage").SetValue(Game1.currentMinigame, this.config_.bullet_damage_);
            }


            if (this.config_.fire_speed_level_ != 0)
            {
                minigameType.GetField("shootingDelay").SetValue(Game1.currentMinigame, this.config_.fire_speed_level_);
            }

            if (this.config_.run_speed_level_ != 0)
            {
                minigameType.GetField("runSpeedLevel").SetValue(Game1.currentMinigame, this.config_.run_speed_level_);
            }

            if (this.config_.spread_pistol_)
            {
                minigameType.GetField("spreadPistol").SetValue(Game1.currentMinigame, this.config_.spread_pistol_);
            }

            if (this.config_.always_invincible_)
            {
                minigameType.GetField("playerInvincibleTimer").SetValue(Game1.currentMinigame, 5000);
            }

            if(this.config_.waveTimer != 0 && (int)minigameType.GetField("waveTimer").GetValue(Game1.currentMinigame) > this.config_.waveTimer )
            {
                minigameType.GetField("waveTimer").SetValue(Game1.currentMinigame, this.config_.waveTimer);
            }


           // String string1 = String.Format("shootOutLevel: {0}", minigameType.GetField("shootoutLevel").GetValue(Game1.currentMinigame));
           // this.Monitor.Log(string1, LogLevel.Info);



        }

    }
}