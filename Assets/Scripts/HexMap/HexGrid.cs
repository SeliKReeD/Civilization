using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    /// <summary>
    ///  Ширина сетки.
    /// </summary>
    public int width = 6;
    /// <summary>
    ///  Высота сетки.
    /// </summary>
    public int height = 6;
    /// <summary>
    ///  Цвет ячейки по умолчанию.
    /// </summary>
    public Color defaultColor = Color.white;
    /// <summary>
    ///  Префаб ячейки.
    /// </summary>
    public HexCell cellPrefab;
    /// <summary>
    ///  Префаб лейбла координат.
    /// </summary>
    public Text labelPrefab;

    Canvas canvas;
    HexMesh mesh;

    HexCell[] hexCells;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        mesh = GetComponentInChildren<HexMesh>();

        hexCells = new HexCell[width * height];

        int index = 0;
        for(int z = 0; z < width; z++)
        {
            for(int x = 0; x < height; x++)
            {
                CreateCell(x, z, index++);
            }
        }
    }

    private void Start()
    {
        mesh.Triangulate(hexCells);
    }

    /// <summary>
    ///  Метод возвращает ячейку в заданых координатах.
    /// </summary>
    /// <param name="point">Точка на сетке, полученая от рейкаста.</param>
    public HexCell GetCell(Vector3 point)
    {
        // Получаем точку на сетке.
        Vector3 correctPoint = transform.InverseTransformPoint(point);
        // Переводим точку в хексагональные координаты.
        HexCoordinates coordinates = HexCoordinates.FromPosition(correctPoint);
        // Определяем индекс ячейки на которую нажали.
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return hexCells[index];
    }

    /// <summary>
    /// Создание ячейки.
    /// </summary>
    /// <param name="x">X координата.</param>
    /// <param name="z">Z координата</param>
    /// <param name="index">Индексв масиве ячеек</param>
    public void CreateCell(int x, int z, int index)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2;
        position.y = 0f;
        position.z = z * HexMetrics.outerRadius * 1.5f;

        HexCell cell = hexCells[index] = Instantiate(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        if(x > 0)
        {
            cell.SetNeighbors(hexCells[index - 1], HexDirections.W);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbors(hexCells[index - width], HexDirections.SE);
                if (x > 0)
                {
                    cell.SetNeighbors(hexCells[index - width -1 ], HexDirections.SW);
                }
            }
            else
            {

                cell.SetNeighbors(hexCells[index - width], HexDirections.SW);
                if (x < width - 1)
                {
                    cell.SetNeighbors(hexCells[index - width + 1], HexDirections.SE);
                }
            }
        }

        Text label = Instantiate(labelPrefab);
        label.rectTransform.SetParent(canvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.GetCoordinatesString();

        cell.label = label.rectTransform;
    }

    public void Refresh()
    {
        mesh.Triangulate(hexCells);
    }
}
