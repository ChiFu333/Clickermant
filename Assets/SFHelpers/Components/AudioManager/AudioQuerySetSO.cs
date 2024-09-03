using System.Collections.Generic;
using UnityEngine;

namespace SFH {
    [CreateAssetMenu(fileName = "Audio Query Set", menuName = "SFH/Audio Query Set", order = 1)]
    public class AudioQuerySetSO : ScriptableObject {
        public List<AudioQuerySO> audioQueries = new List<AudioQuerySO>();
    }
}