using UnityEngine;

[CreateAssetMenu(fileName = "PointsData", menuName = "ScriptableObjects/PointsData")]
public class PointsData : ScriptableObject
{
    public int points = 0;

    public void ModifyPoints(int inputPoints)
    {
        points += inputPoints;
    }
}
