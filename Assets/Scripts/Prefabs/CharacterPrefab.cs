using System;
using System.Collections.Generic;



[Serializable]
public class CharacterPrefab
{
    public string name;

    public List<ClothesPrefab> clothes = new();
}