using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CollectableObjectInfo : ScriptableObject
{
    [SerializeField]
    string objectName;

    [SerializeField]
    int minNegativeScore;

    [SerializeField]
    int maxNegativeScore;

    [SerializeField]
    int minPositiveScore;

    [SerializeField]
    int maxPositiveScore;

    public string ObjectName => objectName;
    public int MinNegativeScore => minNegativeScore;
    public int MaxNegativeScore => maxNegativeScore;
    public int MinPositiveScore => minPositiveScore;
    public int MaxPositiveScore => maxPositiveScore;

    private void OnValidate()
    {
        minNegativeScore = Mathf.Min(Mathf.Max(maxNegativeScore, minNegativeScore), -1);
        minPositiveScore = Mathf.Max(Mathf.Min(maxPositiveScore, minPositiveScore), 0);
    }
}
