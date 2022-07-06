using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUINode : MonoBehaviour
{
    #region Data
    private LogicNode logicNode;
    private DataNode dataNode;
    private DisplayNode displayNode;
    private Map map;

    [SerializeField] private Camera mainCamera;
    [SerializeField] [Range(0.01f, 0.1f)] private float scrollSensitivity;
    #endregion Data


    #region Methods
    public void Initialize(LogicNode logicNode, DataNode dataNode, DisplayNode displayNode)
    {
        this.logicNode = logicNode;
        this.dataNode = dataNode;
        this.displayNode = displayNode;
    }
    public void SetMap(Map map)
    {
        this.map = map;
    }


    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int adjustedX = (int)((worldPosition.x - (worldPosition.x % displayNode.GridCellSizeX)) / displayNode.GridCellSizeX);

            Vector2Int gridPosition = new Vector2Int(adjustedX, Mathf.FloorToInt(worldPosition.y));

            MapCell cell = map.CellAtPosition(gridPosition);
            if (cell != null)
            {
                if (cell.Visible)
                {
                    if (cell.HasAgent)
                    {
                        Debug.Log(cell.Agent.Name);
                    }
                    else if (cell.HasStructure)
                    {
                        Debug.Log(cell.Structure.FullName);
                    }
                    else if (cell.HasItem)
                    {
                        Debug.Log($"{cell.Item.Item.FullName}, {cell.Item.Amount}");
                    }
                    else if (cell.HasPlant)
                    {
                        Debug.Log($"{cell.Plant.Name}, {cell.Plant.Stage}");
                    }
                }
                else
                {
                    Debug.Log(cell.UppermostName);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int adjustedX = (int)((worldPosition.x - (worldPosition.x % displayNode.GridCellSizeX)) / displayNode.GridCellSizeX);

            Vector2Int gridPosition = new Vector2Int(adjustedX, Mathf.FloorToInt(worldPosition.y));

            MapCell cell = map.CellAtPosition(gridPosition);

            if (cell.HasStructure && cell.Structure.HasInventory)
            {
                Debug.Log($"{cell.Structure.FullName} ({cell.Structure.Inventory.StoredItems.Count}/{cell.Structure.Inventory.ItemSlots}) contains: ");
                Structure structure = cell.Structure;

                foreach (ItemSlot itemSlot in structure.Inventory.StoredItems)
                {
                    Debug.Log($"{itemSlot.Item.FullName}, {itemSlot.Amount}");
                }
            }
            if (cell.HasCorpse)
            {
                Debug.Log($"{cell.Corpse.Name} contains: ");
                Corpse corpse = cell.Corpse;

                foreach (ItemSlot itemSlot in corpse.Inventory.StoredItems)
                {
                    Debug.Log($"{itemSlot.Item.FullName}, {itemSlot.Amount}");
                }
            }
            if (cell.HasPlant)
            {
                cell.Plant.TakeDamage(5000);
            }
            if (cell.HasAgent)
            {
                cell.Agent.TakeDamage(5000);
            }
            if (cell.HasStructure)
            {
                cell.Structure.TakeDamage(50000);
            }
        }
    }
    private void KeyboardInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.y += scrollSensitivity;
            mainCamera.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.y -= scrollSensitivity;
            mainCamera.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.x -= scrollSensitivity;
            mainCamera.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.x += scrollSensitivity;
            mainCamera.transform.position = newPos;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            logicNode.TogglePause();
        }
    }
    #endregion Methods

    #region Behaviour
    void Start()
    {
        
    }

    void Update()
    {
        MouseInput();
        KeyboardInput();
    }
    #endregion Behaviour
}