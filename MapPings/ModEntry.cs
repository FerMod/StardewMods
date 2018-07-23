
using MapPings.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapPings {

    public class ModEntry : Mod {

        private ModConfig modConfig;
        private MapOverlay mapOverlay;
        //private ModData modData;

        public static IMonitor ModMonitor { get; private set; }
        public static Logger ModLogger { get; private set; }

        public override void Entry(IModHelper helper) {

            ModMonitor = Monitor;
            ModLogger = new Logger(Monitor, Path.Combine(helper.DirectoryPath, "logfile.txt"), false) {
                LogToOutput = LogOutput.Console
            };

            this.Monitor.Log("Loading mod config...", LogLevel.Trace);
            this.modConfig = helper.ReadConfig<ModConfig>();

            SaveEvents.AfterLoad += this.OnAfterLoad;

        }


        /*********
		** Private methods
		*********/

        /// <summary>The method called after the player loads their save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAfterLoad(object sender, EventArgs e) {

#if DEBUG
            // Pause time and set it to 09:00
            Helper.ConsoleCommands.Trigger("world_freezetime", new string[] { "1" });
            Helper.ConsoleCommands.Trigger("world_settime", new string[] { "0900" });
#endif

            mapOverlay = new MapOverlay(this.Helper, this.modConfig);

        }

    }

}
