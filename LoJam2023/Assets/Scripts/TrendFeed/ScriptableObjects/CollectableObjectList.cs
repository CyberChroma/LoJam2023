using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CollectableObjectList : ScriptableObject
{
    [SerializeField]
    List<CollectableObjectInfo> collectableObjects;

    public List<CollectableObjectInfo> CollectableObjects { get { return collectableObjects; } }

    private void OnValidate()
    {
        if (collectableObjects == null)
            collectableObjects = new List<CollectableObjectInfo>();
    }
}
