using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class CollectableObjectList : ScriptableObject
{
    [SerializeField]
    List<CollectableObjectInfo> collectableObjects;

    public List<CollectableObjectInfo> CollectableObjects => collectableObjects;

    private void OnValidate()
    {
        if (collectableObjects == null)
            collectableObjects = new List<CollectableObjectInfo>();

        HashSet<string> objectNames = new();

        foreach(CollectableObjectInfo obj in collectableObjects)
        {


            objectNames.Add(obj.name);
        }
    }
}
