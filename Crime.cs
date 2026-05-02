using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Crime
{
    public string crimeType;
    public string location;
    public string spawnTime;
    public string unitNeeded;
    public bool isActive = false;
    public List<GameObject> crimeGameObjects = new List<GameObject>();
    
}
