using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SFH;

public class GridRegions : SerializableMonoSingleton<GridRegions> {
    private static readonly Dictionary<RegionType, Color> regionColors = new Dictionary<RegionType, Color>(){
        { RegionType.Nothing, Color.gray},
        { RegionType.Forest, Color.green},
        { RegionType.Stone, Color.white},
    };
    [TableMatrix(HorizontalTitle = "Region matrix", DrawElementMethod = "DrawCell")]
    public RegionType[,] region = new RegionType[32, 20];

    public enum RegionType {
        Nothing,
        Forest,
        Stone
    }

    private static RegionType GetNextType(RegionType type) {
        switch (type) {
            case RegionType.Nothing:
                return RegionType.Forest;
            case RegionType.Forest:
                return RegionType.Stone;
            case RegionType.Stone:
                return RegionType.Nothing;
        }
        return RegionType.Nothing;
    }
    #if UNITY_EDITOR 
    private static RegionType DrawCell(Rect rect, RegionType type) {
        if (Event.current.type == EventType.MouseDown &&
            rect.Contains(Event.current.mousePosition)) {
            //Change type
            type = GetNextType(type);
            GUI.changed = true;
            Event.current.Use();
        }

            EditorGUI.DrawRect(rect.Padding(1), regionColors[type]);
        return type;
    }
    #endif
}