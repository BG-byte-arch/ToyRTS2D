using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    private LineRenderer lineRend;
    private Vector2 firstMousePosition, currentMousePosition;
    private BoxCollider2D boxColl;
    
    public List<RTSUnit> selectedUnits = new List<RTSUnit>();
    [SerializeField] private float formationSpacing = 1f; // Adjustable spacing between units

    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Clear previous selection if not holding shift
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                foreach (RTSUnit unit in selectedUnits)
                {
                    unit.DeselectThisUnit();
                }
                selectedUnits.Clear();
            }

            lineRend.positionCount = 4;
            firstMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRend.SetPosition(0, new Vector2(firstMousePosition.x, firstMousePosition.y));
            lineRend.SetPosition(1, new Vector2(firstMousePosition.x, firstMousePosition.y));
            lineRend.SetPosition(2, new Vector2(firstMousePosition.x, firstMousePosition.y));
            lineRend.SetPosition(3, new Vector2(firstMousePosition.x, firstMousePosition.y));

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("Entity_Unit"))
            {
                RTSUnit rtsUnit = hit.collider.gameObject.GetComponent<RTSUnit>();
                rtsUnit.SelectThisUnit();
                if (!selectedUnits.Contains(rtsUnit))
                {
                    selectedUnits.Add(rtsUnit);
                }
            }

            boxColl = gameObject.AddComponent<BoxCollider2D>();
            boxColl.isTrigger = true;
            boxColl.offset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        if (Input.GetMouseButton(0))
        {
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            lineRend.SetPosition(0, new Vector2(firstMousePosition.x, firstMousePosition.y));
            lineRend.SetPosition(1, new Vector2(firstMousePosition.x, currentMousePosition.y));
            lineRend.SetPosition(2, new Vector2(currentMousePosition.x, currentMousePosition.y));
            lineRend.SetPosition(3, new Vector2(currentMousePosition.x, firstMousePosition.y));

            transform.position = (currentMousePosition + firstMousePosition) / 2;

            boxColl.size = new Vector2(
                Mathf.Abs(firstMousePosition.x - currentMousePosition.x), 
                Mathf.Abs(firstMousePosition.y - currentMousePosition.y));
        }

        if (Input.GetMouseButtonUp(0))
        { 
            lineRend.positionCount = 0;
            Destroy(boxColl);
            transform.position = Vector3.zero;
        }
        
        // Handle square formation movement on right click
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            
            // Calculate square formation positions
            int unitsPerRow = Mathf.CeilToInt(Mathf.Sqrt(selectedUnits.Count));
            int row = 0;
            int col = 0;
            
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                // Calculate offset for square formation
                float xOffset = (col - (unitsPerRow - 1) * 0.5f) * formationSpacing;
                float yOffset = (row - (unitsPerRow - 1) * 0.5f) * formationSpacing;
                Debug.Log("xOffset: " + xOffset + " yOffset: " + yOffset);
                
                Vector3 unitOffset = new Vector3(xOffset, yOffset, 0);
                selectedUnits[i].MoveToPosition(targetPosition + unitOffset);
                
                // Update row/col counters
                col++;
                if (col >= unitsPerRow)
                {
                    col = 0;
                    row++;
                }
            }
        }
    }
}