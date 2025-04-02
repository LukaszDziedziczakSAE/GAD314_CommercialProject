using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// this is the physical placeable object, this contains functionality of the placeable object, use this as the mother class for the different placeable objects, this script itself just holds the functionality to place it in the world first how it works after placement should be in a child script

public class PlaceableObject : MonoBehaviour
{
    [field: SerializeField] public Placeable Config { get; private set; }
    [SerializeField] Material validPlacementMat;
    [SerializeField] Material invalidPlacementMat;
    [SerializeField] Material placedMat;
    [SerializeField] MeshRenderer[] meshes;
    [SerializeField] BoxCollider boxCollider;

    public bool IsPlaced;

    public bool HasValidPlacement;
    public RoomObject Room { get; private set; }
    private List<GameObject> objectsInColider = new List<GameObject>();

    private Vector3 down => -transform.up;

    private void Awake()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
    }

    protected virtual void Start()
    {
        SetInvalidPlacementMaterial();
    }

    protected virtual void Update()
    {
        if (!IsPlaced)
        {
            if (ValidPlacement) SetValidPlacementMaterial();
            else SetInvalidPlacementMaterial();
        }
    }

    public void SetValidPlacementMaterial()
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material = validPlacementMat;
        }
        HasValidPlacement = true;
    }

    public void SetInvalidPlacementMaterial()
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material = invalidPlacementMat;
        }
        HasValidPlacement = false;
    }

    public virtual void SetPlaced()
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material = placedMat;
        }
        IsPlaced = true;
        Station.AddPlaceable(this);
        Station.NavMeshSurface.BuildNavMesh();

        Vector3 origin = transform.position;
        origin.y += 5;
        if (Physics.Raycast(origin, new Vector3(0, -1, 0), out RaycastHit hit , 10, Game.StationFloorLayer))
        {
            if (hit.collider.TryGetComponent<FloorTile>(out FloorTile tile))
            {
                Room = tile.Room;
                Room.AddPlaceable(this);
            }
            else
            {
                Debug.LogWarning("Room detection hit " + hit.collider.name + " instead of floor tile");
            }
        }
        else
        {
            Debug.LogWarning(Config.Name + " did not detect what room it was placed in");
        }

        UI.UpdateRoomInfo();
    }

    private bool ValidPlacement
    {
        get
        {
            if (!triggerClear) return false;

            if (boxCollider != null)
            {
                bool topLeftValid = false;
                foreach (RaycastHit hit in Physics.RaycastAll(topLeftOrigin, down, 10))
                {
                    FloorTile floorTile = hit.collider.GetComponent<FloorTile>();
                    if (floorTile != null)
                    {
                        if ((Game.Selection.Room == null && floorTile.Room.Config.Placeables.Contains(Config))
                            || (Game.Selection.Room == floorTile.Room && !floorTile.IsEdgePiece))
                            topLeftValid = true;
                    }
                }

                bool topRightValid = false;
                foreach (RaycastHit hit in Physics.RaycastAll(topRightOrigin, down, 10))
                {
                    FloorTile floorTile = hit.collider.GetComponent<FloorTile>();
                    if (floorTile != null)
                    {
                        if ((Game.Selection.Room == null && floorTile.Room.Config.Placeables.Contains(Config))
                            || Game.Selection.Room == (floorTile.Room && !floorTile.IsEdgePiece))
                            topRightValid = true;
                    }
                }

                bool bottomLeftValid = false;
                foreach (RaycastHit hit in Physics.RaycastAll(bottomLeftOrigin, down, 10))
                {
                    FloorTile floorTile = hit.collider.GetComponent<FloorTile>();
                    if (floorTile != null)
                    {
                        if ((Game.Selection.Room == null && floorTile.Room.Config.Placeables.Contains(Config))
                            || Game.Selection.Room == (floorTile.Room && !floorTile.IsEdgePiece))
                            bottomLeftValid = true;
                    }
                }

                bool bottomRightValid = false;
                foreach (RaycastHit hit in Physics.RaycastAll(bottomRightOrigin, down, 10))
                {
                    FloorTile floorTile = hit.collider.GetComponent<FloorTile>();
                    if (floorTile != null)
                    {
                        if ((Game.Selection.Room == null && floorTile.Room.Config.Placeables.Contains(Config))
                            || Game.Selection.Room == (floorTile.Room && !floorTile.IsEdgePiece))
                            bottomRightValid = true;
                    }
                }

                return topLeftValid && topRightValid && bottomLeftValid && bottomRightValid;
            }
            else
            {
                Vector3 origin = transform.position;
                origin.y += 5;
                RaycastHit[] hits = Physics.RaycastAll(origin, down, 10);
                foreach (RaycastHit hit in hits)
                {
                    //print("hit " + hit.collider.name);
                    FloorTile floorTile = hit.collider.GetComponent<FloorTile>();
                    if (floorTile != null)
                    {
                        if (Game.Selection.Room == null
                            || Game.Selection.Room == floorTile.Room)
                            return true;
                    }
                }
            }

            return false;
        }
    }

    private Vector3 topLeftOrigin
    {
        get
        {
            Vector3 position = transform.position;
            position.y += 5;
            position.x -= boxCollider.size.x / 2;
            return position;
        }
    }

    private Vector3 topRightOrigin
    {
        get
        {
            Vector3 position = transform.position;
            position.y += 5;
            position.x += boxCollider.size.x / 2;
            return position;
        }
    }

    private Vector3 bottomLeftOrigin
    {
        get
        {
            Vector3 position = transform.position;
            position.y += 5;
            position.z -= boxCollider.size.y / 2;
            return position;
        }
    }

    private Vector3 bottomRightOrigin
    {
        get
        {
            Vector3 position = transform.position;
            position.y += 5;
            position.z += boxCollider.size.y / 2;
            return position;
        }
    }

    protected virtual void OnDestroy()
    {
        if (Room != null) Room.RemovePlaceable(this);
        Station.RemovePlaceable(this);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        objectsInColider.Add(other.gameObject);

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectsInColider.Contains(other.gameObject))
            objectsInColider.Remove(other.gameObject);
    }

    private bool triggerClear => objectsInColider.Count == 0;

    public void CorrectConfig(Placeable config)
    {
        Config = config;
        Debug.LogWarning("Corrected config on " + name);
    }
}
