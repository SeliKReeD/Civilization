using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;

    public Color[] colors;

    public Color activeColor;

    public Slider elevationSlider; 

    public int activeElevation;

    private void Start()
    {
        activeColor = colors[0];
        elevationSlider.onValueChanged.AddListener(SetElevation);
    }

    private void Update()
    {
        if(Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    public void HandleInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if(Physics.Raycast(ray, out rayHit))
        {
            EditCell(hexGrid.GetCell(rayHit.point));
        }
    }

    public void EditCell(HexCell cell)
    {
        cell.color = activeColor;
        cell.Elevation = activeElevation;
        hexGrid.Refresh();
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    public void SetElevation(float elevation)
    {
        Debug.Log(elevation);
        activeElevation = (int)elevation;
    }
}
