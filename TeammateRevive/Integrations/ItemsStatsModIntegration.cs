﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ItemStats;
using ItemStats.Stat;
using ItemStats.ValueFormatters;
using RoR2;
using TeammateRevive.Logging;
using TeammateRevive.Resources;
using TeammateRevive.Revive.Rules;

namespace TeammateRevive.Integrations
{
    /// <summary>
    /// Adds description for added items.
    /// </summary>
    public class ItemsStatsModIntegration
    {
        private readonly ReviveRulesCalculator rules;

        public ItemsStatsModIntegration(ReviveRulesCalculator rules)
        {
            this.rules = rules;
            RoR2Application.onLoad += () =>
            {
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.ontrigger.itemstats"))
                {
                    AddToItemStats();
                }
            };
        }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        void AddToItemStats()
        {
            if (!ItemsAndBuffs.InitedIndexes || ItemsAndBuffs.ReviveItemIndex.ToString() == "None")
            {
                Log.Warn("ItemStats integration: Cannot add - items weren't loaded at application start!");
                return;
            }

            ItemStatsMod.AddCustomItemStatDef(ItemsAndBuffs.ReviveItemIndex, new ItemStatDef
            {
                Stats = new List<ItemStat>
                {
                    new(
                        (itemCount, ctx) => this.rules.GetReviveIncrease((int)itemCount),
                        (value, ctx) => $"Revive speed increased by {value.FormatPercentage(signed: true, decimalPlaces: 1)}"
                    ),
                    new(
                        (itemCount, ctx) => this.rules.GetReviveTime((int)itemCount),
                        (value, ctx) => $"Time to revive alone: {value.FormatInt(postfix: "s", decimals: 1)}"
                    ),
                    new(
                        (itemCount, ctx) => this.rules.CalculateSkullRadius((int)itemCount, 1),
                        (value, ctx) => $"Revive circle range: {value.FormatInt(postfix: "m", decimals: 1)}"
                    ),
                    new(
                        (itemCount, ctx) => this.rules.GetReviveReduceDamageFactor((int)itemCount) - 1,
                        (value, ctx) => $"Damage from your circle: {value.FormatPercentage(decimalPlaces: 1, signed: true)}"
                    )
                }
            });
            
            Log.Info($"ItemStats integration: Added! Idx {ItemsAndBuffs.ReviveItemIndex}");
        }
    }
}