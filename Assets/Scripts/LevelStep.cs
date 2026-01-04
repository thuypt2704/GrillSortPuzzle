using UnityEngine;
using System.Collections.Generic;

    public static class LevelStep
    {
        public static List<LevelData> Levels = new List<LevelData>()
        {
            // ======================
            // LEVEL 1 – Tutorial
            // ======================
            new LevelData
            {
                allFood = 2,
                totalFood = 2,
                totalGrill = 3
            },

            // ======================
            // LEVEL 2 – Easy
            // ======================
            new LevelData
            {
                allFood = 3,
                totalFood = 3,
                totalGrill = 4
            },

            // ======================
            // LEVEL 3 – Medium
            // ======================
            new LevelData
            {
                allFood = 4,
                totalFood = 4,
                totalGrill = 5
            },

            // ======================
            // LEVEL 4 – Hard
            // ======================
            new LevelData
            {
                allFood = 5,
                totalFood = 5,
                totalGrill = 6
            }
        };
    }
