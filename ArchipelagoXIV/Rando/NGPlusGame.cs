using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchipelagoXIV.Rando
{
    internal class NGPlusGame : BaseGame
    {
        public NGPlusGame(ApState apState) : base(apState)
        {
            this.GoalCount = 50;
        }

        private readonly string[] Jobs = ["PLD", "WAR", "DRK", "GNB", "WHM", "SCH", "AST", "SGE", "MNK", "DRG", "NIN", "SAM", "RPR", "BRD", "MCH", "DNC", "BLM", "SMN", "RDM", "BLU"];
        private long GoalCount;
        private long McGuffinCount;

        public override string Name => "Manual_FFXIV_Silasary";

        public override int MaxLevel() => Jobs.Max(MaxLevel);

        public override int MaxLevel(string job) => apState.Items.Count(i => i == $"5 {job} Levels") * 5;

        internal override void ProcessItem(ItemInfo item, string itemName)
        {
            base.ProcessItem(item, itemName);
            if (itemName.EndsWith("Levels"))
            {
                var words = itemName.Split(' ');
                var job = Data.ClassJobs.First(j => j.Abbreviation == words[1]);
                Levels[job] = MaxLevel(job);
            }
            else if (itemName == "Memory of a Distant World")
            {
                if ((McGuffinCount= apState.Items.Count(i => i == itemName)) >= GoalCount)
                {
                    var statusUpdatePacket = new StatusUpdatePacket();
                    statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
                    apState.session.Socket.SendPacket(statusUpdatePacket);
                }
            }
        }

        internal override void HandleSlotData(Dictionary<string, object> slotData)
        {
            base.HandleSlotData(slotData);
            this.GoalCount = (long)slotData["mcguffins_needed"];
            DalamudApi.Echo($"Goal is {GoalCount} Memories.");
        }

        internal override string GoalString()
        {
            if (Goal == 0)
            {
                return $"{McGuffinCount}/{GoalCount} Memories of a Distant World recovered";
            }
            return "";
        }
    }
}
