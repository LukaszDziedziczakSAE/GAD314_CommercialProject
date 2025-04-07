using NUnit.Framework;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FloorTile : MonoBehaviour
{
    [SerializeField] Material buildingValid; // the material used while the player is placing the floor
    [SerializeField] Material buildingInvalid;
    [SerializeField] Material built; // the material once the player has confirmed placement, what it normally looks like
    [SerializeField] Material selected;
    [SerializeField] MeshRenderer[] meshes;
    [SerializeField] GameObject wallTop;
    [SerializeField] GameObject wallBottom;
    [SerializeField] GameObject wallLeft;
    [SerializeField] GameObject wallRight;
    [SerializeField] GameObject pillerTopLeft;
    [SerializeField] GameObject pillerTopRight;
    [SerializeField] GameObject pillerBottomLeft;
    [SerializeField] GameObject pillerBottomRight;
    [SerializeField] GameObject doorwayTop;
    [SerializeField] GameObject doorwayBottom;
    [SerializeField] GameObject doorwayLeft;
    [SerializeField] GameObject doorwayRight;

    [field: SerializeField] public NavMeshSurface NavMeshSurface {  get; private set; }
    public RoomObject Room {  get; private set; }

    public bool IsDoorTile => Room.DoorTiles.Contains(this);
    public bool IsSelectedRoom => Game.Selection.Room == Room;

    public float x => transform.position.x;
    public float z => transform.position.z;
    public bool TopEdge;
    public bool BottomEdge;
    public bool LeftEdge;
    public bool RightEdge;

    public void SetRoom(RoomObject room)
    {
        this.Room = room;
    }


    public void SwitchToBuildingValidMaterial()
    {
        foreach (MeshRenderer meshRenderer in meshes)
        {
            meshRenderer.material = buildingValid;
        }
    }
    public void SwitchToBuildingInvalidMaterial()
    {
        foreach (MeshRenderer meshRenderer in meshes)
        {
            meshRenderer.material = buildingInvalid;
        }
    }

    public void SwitchToBuitMaterial()
    {
        foreach (MeshRenderer meshRenderer in meshes)
        {
            meshRenderer.material = built;
        }
    }
    public void SwitchToSelectedMaterial()
    {
        foreach (MeshRenderer meshRenderer in meshes)
        {
            meshRenderer.material = selected;
        }
    }

    private void OnDestroy()
    {
        Room?.RemoveTile(this);
    }

    public void RemoveWalls()
    {
        wallTop.SetActive(false);
        wallBottom.SetActive(false);
        wallLeft.SetActive(false);
        wallRight.SetActive(false);

        pillerTopLeft.SetActive(false);
        pillerBottomLeft.SetActive(false);
        pillerBottomRight.SetActive(false);
        pillerTopRight.SetActive(false);

        doorwayTop.SetActive(false);
        doorwayBottom.SetActive(false);
        doorwayLeft.SetActive(false);
        doorwayRight.SetActive(false);
    }

    public void AddTopWall()
    {
        wallTop.SetActive(true);
    }

    public void AddBottomWall()
    {
        wallBottom.SetActive(true);
    }

    public void AddLeftWall()
    {
        wallLeft.SetActive(true);
    }

    public void AddRightWall()
    {
        wallRight.SetActive(true);
    }

    public void AddTopDoorway()
    {
        doorwayTop.SetActive(true);
    }

    public void AddDownDoorway()
    {
        doorwayBottom.SetActive(true);
    }

    public void AddLeftDoorway()
    {
        doorwayLeft.SetActive(true);
    }

    public void AddRightDoorway()
    {
        doorwayRight.SetActive(true);
    }

    public void ApplyPillers()
    {
        if (wallTop.activeSelf && wallLeft.activeSelf)
        {
            pillerTopLeft.SetActive(true);
        }

        if (wallBottom.activeSelf && wallLeft.activeSelf)
        {
            pillerBottomLeft.SetActive(true);
        }

        if (wallBottom.activeSelf && wallRight.activeSelf)
        {
            pillerBottomRight.SetActive(true);
        }

        if (wallTop.activeSelf && wallRight.activeSelf)
        {
            pillerTopRight.SetActive(true);
        }
    }

    public bool IsEdgePiece
    {
        get
        {
            if (!Game.PlaceableBuilder.DontPlaceOnEdge) return false;
            return wallTop.activeSelf || wallBottom.activeSelf || wallRight.activeSelf || wallLeft.activeSelf;
        }
    }

    public void AddWalls()
    {
        bool isDoor = IsDoorTile;

        RemoveWalls();

        Vector3 up = transform.position;
        up.z += Game.FloorBuilder.TileSize.y;
        FloorTile upTile = Station.TileAtLocation(up);
        if (upTile == null || (upTile.Room != Room && !upTile.IsDoorTile))
            AddTopWall();
        else if (upTile != null && upTile.Room != Room && isDoor && upTile.IsDoorTile)
            AddTopDoorway();

        Vector3 down = transform.position;
        down.z -= Game.FloorBuilder.TileSize.y;
        FloorTile downTile = Station.TileAtLocation(down);
        if (downTile == null || (downTile.Room != Room && !downTile.IsDoorTile))
            AddBottomWall();
        else if (downTile != null && downTile.Room != Room && isDoor && downTile.IsDoorTile) 
            AddDownDoorway();

        Vector3 left = transform.position;
        left.x -= Game.FloorBuilder.TileSize.x;
        FloorTile leftTile = Station.TileAtLocation(left);
        if (leftTile == null || (leftTile.Room != Room && !leftTile.IsDoorTile))
            AddLeftWall();
        else if (leftTile != null && leftTile.Room != Room && isDoor && leftTile.IsDoorTile) 
            AddLeftDoorway();

        Vector3 right = transform.position;
        right.x += Game.FloorBuilder.TileSize.x;
        FloorTile rightTile = Station.TileAtLocation(right);
        if (rightTile == null || (rightTile.Room != Room && !rightTile.IsDoorTile))
            AddRightWall();
        else if (rightTile != null && rightTile.Room != Room && isDoor && rightTile.IsDoorTile) 
            AddRightDoorway();

        ApplyPillers();
    }

    public FloorTile OtherRoomTile()
    {
        Vector3 up = transform.position;
        up.z += Game.FloorBuilder.TileSize.y;
        FloorTile upTile = Station.TileAtLocation(up);
        if (upTile != null && upTile.Room != Room)
            return upTile;

        Vector3 down = transform.position;
        down.z -= Game.FloorBuilder.TileSize.y;
        FloorTile downTile = Station.TileAtLocation(down);
        if (downTile != null && downTile.Room != Room)
            return downTile;

        Vector3 left = transform.position;
        left.x -= Game.FloorBuilder.TileSize.x;
        FloorTile leftTile = Station.TileAtLocation(left);
        if (leftTile != null && leftTile.Room != Room)
            return leftTile;

        Vector3 right = transform.position;
        right.x += Game.FloorBuilder.TileSize.x;
        FloorTile rightTile = Station.TileAtLocation(right);
        if (rightTile != null && rightTile.Room != Room)
            return rightTile;

        print("didn't find other room tile");
        return null;
    }

    public List<RoomObject> TouchingRooms
    {
        get
        {
            List<RoomObject> otherTiles = new List<RoomObject>();

            Vector3 up = transform.position;
            up.z += Game.FloorBuilder.TileSize.y;
            FloorTile upTile = Station.TileAtLocation(up);
            if (upTile != null && upTile.Room != Room && !otherTiles.Contains(upTile.Room))
                otherTiles.Add(upTile.Room);

            Vector3 down = transform.position;
            down.z -= Game.FloorBuilder.TileSize.y;
            FloorTile downTile = Station.TileAtLocation(down);
            if (downTile != null && downTile.Room != Room && !otherTiles.Contains(downTile.Room))
                otherTiles.Add(downTile.Room);

            Vector3 left = transform.position;
            left.x -= Game.FloorBuilder.TileSize.x;
            FloorTile leftTile = Station.TileAtLocation(left);
            if (leftTile != null && leftTile.Room != Room && !otherTiles.Contains(leftTile.Room))
                otherTiles.Add(leftTile.Room);

            Vector3 right = transform.position;
            right.x += Game.FloorBuilder.TileSize.x;
            FloorTile rightTile = Station.TileAtLocation(right);
            if (rightTile != null && rightTile.Room != Room && !otherTiles.Contains(rightTile.Room))
                otherTiles.Add(rightTile.Room);

            return otherTiles;
        }
    }

    public void MakeDoorTile(bool isOtherTile = false)
    {
        Room.AddDoorTile(this);
        if (!isOtherTile) OtherRoomTile().MakeDoorTile(true);
        AddWalls();
    }

    public void ClearEdges()
    {
        TopEdge = false;
        LeftEdge = false;
        RightEdge = false;
        BottomEdge = false;
    }

    public bool IsEdgeTile => TopEdge || LeftEdge || RightEdge || BottomEdge;

}
