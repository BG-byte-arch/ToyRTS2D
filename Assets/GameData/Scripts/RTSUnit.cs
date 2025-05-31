using System.Collections;
using UnityEngine;

public class RTSUnit : MonoBehaviour
{
    [Header("Main Unit Settings")]
    public bool unitIsBeingSelected;
    public float unitMoveSpeed = 2f;

    private Vector3 unitTargetPosition;
    private SpriteRenderer unitSpriteRenderer;
    private bool unitIsMoving = false;
    private bool allowToDeselect = true;

    [Header("Selection Circle Creation")]
    [SerializeField] private Sprite selectionCircleSprite;
    [SerializeField] private Color selectionCircleColor;
    SpriteRenderer selectionCircleRenderer;

    void Start()
    {
        unitSpriteRenderer = GetComponent<SpriteRenderer>();
        unitIsBeingSelected = false;

        //Selection Circle Creation
        GameObject selectionCircle = new GameObject("Circle");
        selectionCircle.transform.parent = this.gameObject.transform;
        selectionCircle.transform.localScale = new Vector3(this.gameObject.transform.localScale.x / 2.857f, 0.15f, 0f);
        selectionCircle.transform.localPosition = new Vector3(0f, -0.35f, 0f);

        selectionCircleRenderer = selectionCircle.AddComponent<SpriteRenderer>();
        selectionCircleRenderer.enabled = false;
        selectionCircleRenderer.color = selectionCircleColor;
        selectionCircleRenderer.sprite = selectionCircleSprite;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<UnitSelectionManager>() != null)
        {
            SelectThisUnit();
            UnitSelectionManager manager = collision.gameObject.GetComponent<UnitSelectionManager>();
            if (!manager.selectedUnits.Contains(this))
            {
                manager.selectedUnits.Add(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!Input.GetMouseButtonUp(0) && collision.gameObject.GetComponent<UnitSelectionManager>() != null)
        {
            DeselectThisUnit();
            UnitSelectionManager manager = collision.gameObject.GetComponent<UnitSelectionManager>();
            if (manager.selectedUnits.Contains(this))
            {
                manager.selectedUnits.Remove(this);
            }
        }
    }

    public void SelectThisUnit()
    {
        allowToDeselect = false;
        selectionCircleRenderer.enabled = true;
        unitIsBeingSelected = true;
        StartCoroutine(AllowToDeselect());
    }

    public void DeselectThisUnit()
    {
        unitIsBeingSelected = false;
        selectionCircleRenderer.enabled = false;
    }

    IEnumerator AllowToDeselect()
    {
        yield return new WaitForSeconds(0.2f);
        allowToDeselect = true;
    }

    public void MoveToPosition(Vector3 position)
    {
        unitTargetPosition = position;
        unitIsMoving = true;
    }

    void Update()
    {
        unitSpriteRenderer.sortingOrder = (int)(transform.position.y * -100);
        selectionCircleRenderer.sortingOrder = (int)(transform.position.y * -100 - 10);
        
        if (unitIsBeingSelected)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetMouseButtonDown(0) && allowToDeselect)
                {
                    DeselectThisUnit();
                    UnitSelectionManager manager = FindObjectOfType<UnitSelectionManager>();
                    if (manager.selectedUnits.Contains(this))
                    {
                        manager.selectedUnits.Remove(this);
                    }
                }
            }
        }

        if (unitIsMoving)
        {
            if (unitMoveSpeed > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, unitTargetPosition, unitMoveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, unitTargetPosition) < 0.01f)
                {
                    unitIsMoving = false;
                }
            }
            else
            {
                transform.position = unitTargetPosition;
                unitIsMoving = false;
            }
        }
    }
}