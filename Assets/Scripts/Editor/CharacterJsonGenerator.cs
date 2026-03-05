using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CharacterJsonGenerator
{
    [MenuItem("Tools/Generate Characters JSON")]
    public static void Generate()
    {
        string basePath = "Assets/Resources/Characters";

        CharacterPrefabs db = new CharacterPrefabs();

        Dictionary<string, CharacterPrefab> characterMap = new();


        string[] prefabs = AssetDatabase.FindAssets("t:Prefab", new[] { basePath });


        foreach (string guid in prefabs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            string resourcePath = assetPath.Replace("Assets/Resources/", "").Replace(".prefab", "");

            string[] parts = resourcePath.Split('/');

            // Characters / Clothes-X / Color / CharacterName
            if (parts.Length < 4)
                continue;


            string clothes = parts[1];

            string color = parts[2];

            string characterName = prefab.name;


            if (!characterMap.ContainsKey(characterName))
            {
                characterMap[characterName] = new CharacterPrefab
                {
                    name = characterName
                };
            }


            CharacterPrefab character = characterMap[characterName];

            ClothesPrefab clothesData = character.clothes.Find(c => c.type == clothes);


            if (clothesData == null)
            {
                clothesData = new ClothesPrefab
                {
                    type = clothes
                };

                character.clothes.Add(clothesData);
            }


            clothesData.colors.Add(new ColorPrefab
            {
                color = color,
                path = resourcePath
            });
        }


        db.characters.AddRange(characterMap.Values);


        string json = JsonUtility.ToJson(db, true);

        string savePath = "Assets/Resources/Characters/characters.json";

        File.WriteAllText(savePath, json);

        AssetDatabase.Refresh();

        Debug.Log("Characters JSON generated!");
    }
}