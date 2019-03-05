using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public RectTransform label;
    public Color color;

    [SerializeField]
    public HexCell[] neighbors;

    int elevation;

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 newPosition = transform.localPosition;
            newPosition.y = value * HexMetrics.elevationStep;
            transform.localPosition = newPosition;

            Vector3 newLabelPosition = label.localPosition;
            newLabelPosition.z = value * -HexMetrics.elevationStep;
            label.localPosition = newLabelPosition;
        }
    }

    public HexCell GetNeighbors(HexDirections direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbors(HexCell cell, HexDirections direction)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposide()] = this;
    }
}
