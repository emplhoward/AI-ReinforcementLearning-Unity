//Codes by Leo Howard
//Copyrighted 07-30-2021

using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ProceduralTerrain))] //Link editor to action script.
[CanEditMultipleObjects]

public class ProceduralTerrainEditor : Editor {
    //Fold outs ------------
    bool showGenerateHill = true;
    bool showSmoothing = true;
    bool showSizePosition = true;

    //Terrain size:
    SerializedProperty serialized_terrainLength;
    SerializedProperty serialized_terrainHeight;

    //Splat maps are texture maps:
    bool showSplatMaps = true;
    SerializedProperty serialized_tileSize;

    void OnEnable()
    {
        serialized_terrainLength = serializedObject.FindProperty("terrainLength");
        serialized_terrainHeight = serializedObject.FindProperty("terrainHeight");
        serialized_tileSize = serializedObject.FindProperty("tileSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); //Update on screen.
        ProceduralTerrain proceduralTerrain = (ProceduralTerrain) target; //Link action script.

        showSizePosition = EditorGUILayout.Foldout(showSizePosition, "SetSizePosition");

        //Button to set size and position:
        if (showSizePosition)
        {
            EditorGUILayout.PropertyField(serialized_terrainLength, true);

            EditorGUILayout.PropertyField(serialized_terrainHeight, true);

            if (GUILayout.Button("SetSizePosition"))
            {
                proceduralTerrain.SetSizePosition();
            }
        }

        showGenerateHill = EditorGUILayout.Foldout(showGenerateHill, "GenerateMountains");

        if (showGenerateHill)
        {          
            if (GUILayout.Button("Generate Mountains"))
            {
                proceduralTerrain.GenerateMountains();
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        showSmoothing = EditorGUILayout.Foldout(showSmoothing, "Smooth");

        if (showSmoothing)
        {
            if (GUILayout.Button("Smooth"))
            {
                proceduralTerrain.Smooth();
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Randomize Texture/Splat Map");

        if (showSplatMaps)
        {
            EditorGUILayout.PropertyField(serialized_tileSize, true);
          
            if (GUILayout.Button("Randomize Textures"))
            {
                proceduralTerrain.RandomizeTexture();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Reset Terrain"))
        {
            proceduralTerrain.ResetTerrain();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        serializedObject.ApplyModifiedProperties();

    }

}
