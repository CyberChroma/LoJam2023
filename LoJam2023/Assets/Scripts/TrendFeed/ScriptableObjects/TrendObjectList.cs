using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TrendObjectList : ScriptableObject
{
    [SerializeField]
    List<TrendObjectInfo> trendObjects;

    public List<TrendObjectInfo> TrendObjects { get { return trendObjects; } }

    private void OnValidate()
    {
        if (trendObjects == null)
            trendObjects = new List<TrendObjectInfo>();
    }
}
