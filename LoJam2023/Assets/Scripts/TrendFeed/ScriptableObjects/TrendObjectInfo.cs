using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TrendObjectInfo : ScriptableObject
{
    [SerializeField]
    string objectName = "";

    [SerializeField]
    int minNegativeScore = 0;

    [SerializeField]
    int maxNegativeScore = 0;

    [SerializeField]
    int minPositiveScore = 0;

    [SerializeField]
    int maxPositiveScore = 0;

    [SerializeField]
    Sprite objectSprite = null;

    public string ObjectName { get {return objectName; }}
    public int MinNegativeScore { get { return minNegativeScore; } }
    public int MaxNegativeScore { get { return maxNegativeScore; } }
    public int MinPositiveScore {get {return minPositiveScore; }}
    public int MaxPositiveScore { get { return maxPositiveScore; } }

    public Sprite ObjectSprite { get { return objectSprite; } }

    private void OnValidate()
    {
    }
}
