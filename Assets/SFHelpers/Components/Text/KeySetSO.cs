//using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFH {
    [CreateAssetMenu(fileName = "Key Set", menuName = "SFH/Key Set", order = 2)]
    public class KeySetSO : SerializedScriptableObject {
        public Dictionary<KeyCode, string> keyCodeLookup = new Dictionary<KeyCode, string>();

        //Type_Descriptor_Variant
        //key_0_white
        //mouse_left_red

        public string GetKeyTag(KeyCode code, bool isWhiteVariant = true, Color color = new Color()) {
            if (!keyCodeLookup.ContainsKey(code)) throw new System.NotImplementedException($"Icon for {code} is not implemented");
            return GetKeyTag(keyCodeLookup[code], isWhiteVariant, color);
        }

        public string GetKeyTag(string literal, bool isWhiteVariant = true, Color color = new Color()) {
            //StringBuilder builder = new StringBuilder();
            string colorPart = isWhiteVariant ? "_white\"" : "_black\"";
            string colorEnding = "color=#"+ColorUtility.ToHtmlStringRGB(color)+">";
            string tag = "<sprite name=\"key_" + literal + colorPart + colorEnding;
            //builder.Append();
            return tag;
        }
    }
}