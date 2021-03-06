﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;   // Always needed
//using VerseBase;   // Material/Graphics handling functions are found here
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
//using Verse.AI;    // Needed when you do something with the AI
//using Verse.Sound; // Needed when you do something with the Sound

namespace OutpostGenerator
{
    /// <summary>
    /// OG_RuinEffects class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public static class OG_RuinEffects
    {
        public static void GenerateRuinEffects(ref OG_OutpostData outpostData)
        {
            if (outpostData.isInhabited)
            {
                return;
            }
            GenerateRuinDamage(ref outpostData);
            GenerateRuinFilth(ref outpostData);
        }

        private static void GenerateRuinDamage(ref OG_OutpostData outpostData)
        {
            float minHitPointsFactor = 0.10f;
            float maxHitPointsFactor = 1.0f;
            float damageDensity = 0.5f;
            if (outpostData.isRuined)
            {
                // Ruined outpost.
                minHitPointsFactor = 0.2f;
                maxHitPointsFactor = 0.6f;
                damageDensity = 0.3f;
            }
            else
            {
                // Only rusty outpost.
                minHitPointsFactor = 0.8f;
                maxHitPointsFactor = 1f;
                damageDensity = 0.05f;
            }
            foreach (Thing thing in outpostData.outpostThingList)
            {
                if (Rand.Value < damageDensity)
                {
                    thing.HitPoints = (int)((float)thing.MaxHitPoints * Rand.Range(minHitPointsFactor, maxHitPointsFactor));
                }
            }
        }

        private static void GenerateRuinFilth(ref OG_OutpostData outpostData)
        {
            const float dustDensity = 0.3f;
            const float slagDensity = 0.1f;
            if (outpostData.isRuined)
            {
                int areaSideLength = 0;
                if (outpostData.size == OG_OutpostSize.SmallOutpost)
                {
                    areaSideLength = OG_SmallOutpost.areaSideLength;
                }
                else
                {
                    areaSideLength = OG_BigOutpost.areaSideLength;
                }
                CellRect areaRect = new CellRect(outpostData.areaSouthWestOrigin.x, outpostData.areaSouthWestOrigin.z, areaSideLength, areaSideLength);
                foreach (IntVec3 cell in areaRect)
                {
                    if (cell.GetEdifice() != null)
                    {
                        continue;
                    }

                    if (Rand.Value < dustDensity)
                    {
                        GenSpawn.Spawn(ThingDefOf.FilthDirt, cell);
                    }
                    if (Rand.Value < slagDensity)
                    {
                        float slagSelector = Rand.Value;
                        if (slagSelector < 0.33f)
                        {
                            GenSpawn.Spawn(ThingDef.Named("RockRubble"), cell);
                        }
                        else if (slagSelector < 0.66f)
                        {
                            GenSpawn.Spawn(ThingDef.Named("BuildingRubble"), cell);
                        }
                        else
                        {
                            GenSpawn.Spawn(ThingDef.Named("SandbagRubble"), cell);
                        }
                    }
                }
            }
        }
    }
}
