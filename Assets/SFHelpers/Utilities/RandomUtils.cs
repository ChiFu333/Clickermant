using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace SFH {
    public static class RandomUtils {

        public static T WeightedRandomItem<T>(int[] weights, T[] items) {
            return items[WeightedRandomIndex(weights)];
        }

        public static T WeightedRandomItem<T>(List<int> weights, T[] items) {
            return items[WeightedRandomIndex(weights)];
        }

        public static int WeightedRandomIndex(int[] weights) {
            int weightTotal = weights.Sum();
            int result = 0, total = 0;
            int randValue = Random.Range(0,weightTotal);
            for (result = 0; result < weights.Count(); result++) {
                total += weights[result];
                if (total >= randValue) break;
            }
            return result;
        }

        public static int WeightedRandomIndex(List<int> weights) {
            return WeightedRandomIndex(weights.ToArray());
        }
    }
}