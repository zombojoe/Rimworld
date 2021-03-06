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

namespace FishIndustry
{
    /// <summary>
    /// FishingPier custom PlaceWorker class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public class PlaceWorker_FishingPierSpawner : PlaceWorker
    {
        public const int minDistanceBetweenTwoFishingPiers = 15;

        /// <summary>
        /// Check if a new fishing pier can be built at this location.
        /// - the fishing pier bank cell must be on a bank.
        /// - the rest of the fishing pier and the fishing spot must be on water.
        /// - must not be too near from another fishing pier.
        /// </summary>
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot)
        {
            // Check fishing pier bank cell is on a "solid" terrain.
            if (IsAquaticTerrain(loc))
            {
                return new AcceptanceReport("Fishing pier must touch a bank.");
            }
            // Check fishing pier middle and river cells are on water.
            if ((IsAquaticTerrain(loc + new IntVec3(0, 0, 1).RotatedBy(rot)) == false)
                || (IsAquaticTerrain(loc + new IntVec3(0, 0, 2).RotatedBy(rot)) == false))
            {
                return new AcceptanceReport("Fishing pier must be placed on water.");
            }
            // Check fishing zone is on water.
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = 3; yOffset <= 5; yOffset++)
                {
                    if (IsAquaticTerrain(loc + new IntVec3(xOffset, 0, yOffset).RotatedBy(rot)) == false)
                    {
                        return new AcceptanceReport("Fishing zone must be placed on water.");
                    }
                }
            }

            // Check if another fishing pier is not too close (mind the test on "fishing pier" def and "fishing pier spawner" blueprint and frame defs.
            List<Thing> fishingPierList = Find.ListerThings.ThingsOfDef(Util_FishIndustry.FishingPierDef);
            List<Thing> fishingPierSpawnerBlueprintList = Find.ListerThings.ThingsOfDef(Util_FishIndustry.FishingPierSpawnerDef.blueprintDef);
            List<Thing> fishingPierSpawnerFrameList = Find.ListerThings.ThingsOfDef(Util_FishIndustry.FishingPierSpawnerDef.frameDef);

            if (fishingPierList != null)
            {
                IEnumerable<Thing> fishingPierInTheArea = fishingPierList.Where(building => loc.InHorDistOf(building.Position, minDistanceBetweenTwoFishingPiers));
                if (fishingPierInTheArea.Count() > 0)
                {
                    return new AcceptanceReport("An other fishing pier is too close.");
                }
            }
            if (fishingPierSpawnerBlueprintList != null)
            {
                IEnumerable<Thing> fishingPierBlueprintInTheArea = fishingPierSpawnerBlueprintList.Where(building => loc.InHorDistOf(building.Position, minDistanceBetweenTwoFishingPiers));
                if (fishingPierBlueprintInTheArea.Count() > 0)
                {
                    return new AcceptanceReport("An other fishing pier blueprint is too close.");
                }
            }
            if (fishingPierSpawnerFrameList != null)
            {
                IEnumerable<Thing> fishingPierFrameInTheArea = fishingPierSpawnerFrameList.Where(building => loc.InHorDistOf(building.Position, minDistanceBetweenTwoFishingPiers));
                if (fishingPierFrameInTheArea.Count() > 0)
                {
                    return new AcceptanceReport("An other fishing pier frame is too close.");
                }
            }

            return true;
        }

        public static bool IsAquaticTerrain(IntVec3 position)
        {
            TerrainDef terrainDef = Find.TerrainGrid.TerrainAt(position);
            if ((terrainDef == TerrainDef.Named("WaterShallow"))
                || (terrainDef == TerrainDef.Named("WaterDeep"))
                || (terrainDef == TerrainDef.Named("Marsh")))
            {
                return true;
            }
            return false;
        }
    }
}
