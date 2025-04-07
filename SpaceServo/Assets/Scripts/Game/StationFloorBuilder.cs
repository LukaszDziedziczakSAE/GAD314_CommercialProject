using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// controlls placing floors in the station

// TODO: walls, cost, input error checking

public class StationFloorBuilder : MonoBehaviour
{
    public enum EWallSide
    {
        NoWall,
        TopWall,
        BottomWall,
        LeftWall,
        RightWall
    }

    [System.Serializable]
    public struct FloorBuildData
    {
        public Vector3 firstTilePosition;
        public Vector2 size;

        public static FloorBuildData Empty
        {
            get
            {
                FloorBuildData floorBuildData;
                floorBuildData.firstTilePosition = Vector3.zero;
                floorBuildData.size = Vector2.zero;
                return floorBuildData;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return firstTilePosition == Vector3.zero && size == Vector2.zero;
            }
        }
    }



    [field: SerializeField] public Vector2 TileSize = new Vector2(5,5);
    [SerializeField] MeshRenderer plane; // this is something for raycast to hit
    [SerializeField] RoomObject roomPrefab;
    [SerializeField] bool autoDoorwayPlacement;
    [field: SerializeField] public Room[] RoomConfigs {  get; private set; }

    
    //bool placing;
    Vector3 topLeftPoint;
    Vector3 botttomRightPoint;
    [SerializeField, Header("DEBUG")] FloorTile firstTile;
    [SerializeField] List<FloorTile> currentPlacement = new List<FloorTile>();

    [SerializeField] RoomObject currentRoom;
    public bool IsPlacing => currentRoom != null;
    private bool placingDoor;
    private FloorTile possibleDoorTile;
    private FloorTile possibleOtherDoorTile; // TODO: preview doorway building iteration
    //[SerializeField] private Vector2 currentRoomSize;

    public RoomObject CurrentRoom => currentRoom;
    public int CostOfPlacement => costOfPlacement;
    public Vector2 CurrentRoomSize => currentBuildData.size;
    private bool inEditingMode;
    private int previousCost;
    private EWallSide selectedWallSide;
    [SerializeField] FloorBuildData currentBuildData;
    [SerializeField] FloorBuildData tempBuildData;
    [SerializeField] FloorBuildData previousBuildData;

    private void Update()
    {

        if (currentRoom == null || UI.MouseOverUI) return;

        if (placingDoor) PlacingDoorUpdate();

        else if (inEditingMode) EditFloorUpdate();

        else PlacingFloorUpdate();
    }

    private void PlacingDoorUpdate()
    {
        FloorTile floorTileUnderMouse = Station.TileAtLocation(RoundToNearestGrid(GroundLocationUnderMouse));

        if (floorTileUnderMouse != null && floorTileUnderMouse.Room == currentRoom)
        {
            if (floorTileUnderMouse != possibleDoorTile)
            {
                if (possibleDoorTile != null)
                {
                    possibleDoorTile.SwitchToBuitMaterial();
                    possibleDoorTile = null;
                }
                possibleDoorTile = floorTileUnderMouse;
                possibleDoorTile.SwitchToBuildingValidMaterial();
            }

            
        }
        else if (possibleDoorTile != null)
        {
            possibleDoorTile.SwitchToBuitMaterial();
            possibleDoorTile = null;
        }
    }

    private void PlacingFloorUpdate()
    {
        if (!Game.Input.PrimaryButtonDown) // player is indicating where the floor starts
        {
            topLeftPoint = RoundToNearestGrid(GroundLocationUnderMouse);
            currentBuildData.firstTilePosition = topLeftPoint;
            SpawnAndMoveFirstTile();
        }
        else // player has indicated where the floor starts already
        {
            DeterminRoomSize();
            SpawnCurrentPlacement();
        }
    }

    private void SpawnAndMoveFirstTile()
    {
        Vector3 postion = currentBuildData.firstTilePosition;
        if (postion == Vector3.zero) return;

        if (postion.y >= 500) return;

        if (firstTile == null)
        {
            firstTile = Instantiate(currentRoom.Config.FloorTilePrefab, postion, Quaternion.identity);
            firstTile.SwitchToBuildingValidMaterial();
            
        }

        firstTile.transform.position = postion;
        //print("firstTile.transform.position = " + firstTile.transform.position.ToString());
    }

    private void DeterminRoomSize()
    {
        Vector3 currentGrid = RoundToNearestGrid(GroundLocationUnderMouse);
        if (currentGrid.y >= 500) return;
        if (currentGrid != botttomRightPoint)
        {
            botttomRightPoint = currentGrid;


            int columns;
            int rows;

            if (botttomRightPoint.x > topLeftPoint.x)
                columns = (int)((botttomRightPoint.x - topLeftPoint.x) / TileSize.x);
            else
                columns = (int)((topLeftPoint.x - botttomRightPoint.x) / TileSize.x);

            if (botttomRightPoint.z < topLeftPoint.z)
                rows = (int)((topLeftPoint.z - botttomRightPoint.z) / TileSize.y);
            else
                rows = (int)((botttomRightPoint.z - topLeftPoint.z) / TileSize.y);

            currentBuildData.size = new Vector2(columns + 1, rows + 1);

            if (botttomRightPoint.x < topLeftPoint.x) currentBuildData.firstTilePosition.x = botttomRightPoint.x;
            if (botttomRightPoint.z > topLeftPoint.z) currentBuildData.firstTilePosition.y = botttomRightPoint.z;

            if (firstTile.transform.position == currentBuildData.firstTilePosition) DestroyFirstTile();
        }
    }

    private void SpawnCurrentPlacement()
    {
        if (firstTile == null) SpawnAndMoveFirstTile();

        int columns = (int)currentBuildData.size.x - 1;
        int rows = (int)currentBuildData.size.y - 1;

        DestroyCurrentPlacement();
        for (int currentCol = 0; currentCol <= columns; currentCol++)
        {
            for (int currentRow = 0; currentRow <= rows; currentRow++)
            {
                if (currentCol == 0 && currentRow == 0) continue;

                Vector3 position = firstTile.transform.position;
                position.x += TileSize.x * currentCol;
                position.z -= TileSize.y * currentRow;

                FloorTile newTile = Instantiate(currentRoom.Config.FloorTilePrefab, position, Quaternion.identity);
                newTile.SwitchToBuildingValidMaterial();
                currentPlacement.Add(newTile);
            }
        }
        SetWallSideOnPlacement();

        if (PlacementIsValid)
        {
            firstTile.SwitchToBuildingValidMaterial();
            foreach (FloorTile tile in currentPlacement)
            {
                tile.SwitchToBuildingValidMaterial();
            }
        }
        else
        {
            firstTile.SwitchToBuildingInvalidMaterial();
            foreach (FloorTile tile in currentPlacement)
            {
                tile.SwitchToBuildingInvalidMaterial();
            }
        }
    }

    private void DestroyFirstTile()
    {
        if (firstTile != null)
        {
            Destroy(firstTile.gameObject);
            firstTile = null;
        }
    }

    private void DestroyCurrentPlacement()
    {
        foreach (FloorTile tile in currentPlacement.ToArray())
        {
            Destroy(tile.gameObject);
        }
        currentPlacement.Clear();
    }

    private void EditFloorUpdate()
    {
        if (selectedWallSide == EWallSide.NoWall) return;

        Vector3 currentGrid = RoundToNearestGrid(GroundLocationUnderMouse);
        if (currentGrid.y >= 500) return;
        if (currentGrid != botttomRightPoint)
        {
            botttomRightPoint = currentGrid;

            float xDif = (botttomRightPoint.x - topLeftPoint.x) / TileSize.x;
            float zDif = (topLeftPoint.z - botttomRightPoint.z) / TileSize.y;

            switch (selectedWallSide)
            {
                case EWallSide.RightWall:
                    currentBuildData.size.x = tempBuildData.size.x + xDif;
                    break;

                case EWallSide.LeftWall:
                    xDif *= -1;
                    currentBuildData.size.x = tempBuildData.size.x + xDif;
                    currentBuildData.firstTilePosition.x = tempBuildData.firstTilePosition.x - (xDif * TileSize.x);
                    break;

                case EWallSide.BottomWall:
                    currentBuildData.size.y = tempBuildData.size.y + zDif;
                    break;

                case EWallSide.TopWall:
                    currentBuildData.size.y = tempBuildData.size.y - zDif;
                    currentBuildData.firstTilePosition.z = tempBuildData.firstTilePosition.z - (zDif * TileSize.y);
                    break;
            }

            DestroyAllPlacement();
            SpawnCurrentPlacement();
        }
    }

    public void StartPlacingFloor(Room config)
    {
        if (Game.PlaceableBuilder.IsPlacing) return;
        Game.Input.OnPrimaryPress += PlaceFirstTile;
        Game.Input.OnSecondaryPress += CancelPlacement;

        Game.Selection.DeselectRoom();

        previousCost = 0;

        currentRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
        currentRoom.Initialize(config);

        if (Game.Tutorial.ListentingForFloorBuildStart) Game.Tutorial.RoomBuiltStarted(config);

        currentBuildData = FloorBuildData.Empty;
        previousBuildData = FloorBuildData.Empty;

        UI.ShowFloorPlacementInfo();
    }

    private Vector3 GroundLocationUnderMouse
    {
        get
        {
            if (Game.CameraController == null)
            {
                Debug.LogError("Game is missing Camera Controller reference");
                return Vector3.zero;
            }

            Ray ray = Game.CameraController.Camera.ScreenPointToRay(Game.Input.MousePosition);
            float rayCastDistance = Game.CameraController.DistanceToGround + 50;

            if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance, Game.StationLayer))
            {
                return hit.point;
            }

            if (UI.MouseOverUI) Debug.LogWarning("Mouse over UI");
            //else Debug.LogWarning("RayCast fail, no ground position under mouse");
            return new Vector3(0, 500, 0);
        }
    }

    public Vector3 RoundToNearestGrid(Vector3 location)
    {
        float x = Mathf.Round(location.x / TileSize.x) * TileSize.x;
        float z = Mathf.Round(location.z / TileSize.y) * TileSize.y;
        return new Vector3(x, location.y, z);
    }

    public Vector3 RoundToNearestHalfGrid(Vector3 location)
    {
        float x = Mathf.Round(location.x / (TileSize.x / 2)) * (TileSize.x/2);
        float z = Mathf.Round(location.z / (TileSize.y / 2)) * (TileSize.y/2);
        return new Vector3(x, location.y, z);
    }

    private void PlaceFirstTile()
    {
        currentBuildData.firstTilePosition = firstTile.transform.position;
        //print("PlaceFirstTile() firstTilePosition = " + currentBuildData.firstTilePosition.ToString());
        currentBuildData.size = new Vector2(1, 1);
        Game.Input.OnPrimaryPress -= PlaceFirstTile;
        Game.Input.OnPrimaryRelease += PlacementFinished;
        botttomRightPoint = topLeftPoint;
    }

    public void BeginEditMode(RoomObject roomToEdit)
    {
        inEditingMode = true;
        currentRoom = roomToEdit;
        previousCost = CostOfPlacement;
        currentBuildData.firstTilePosition = currentRoom.Floor[0].transform.position;
        currentBuildData.size = currentRoom.RoomSize;
        previousBuildData = currentBuildData;

        Station.RemoveRoom(currentRoom);
        currentRoom.RemoveWalls();
        currentRoom.ClearFloor();
        DestroyAllPlacement();
        SpawnCurrentPlacement();


        Game.Input.OnPrimaryPress += TrySelectWall;

        UI.ShowFloorPlacementInfo();
    }

    private void PlacementFinished()
    {
        Game.Input.OnPrimaryRelease -= PlacementFinished;

        if (PlacementIsValid && CanAffordPlacement)
        {
            inEditingMode = true;
            currentRoom.SetRoomSize(CurrentRoomSize);

            Game.Input.OnPrimaryPress += TrySelectWall;
        }

        else
        {
            DestroyAllPlacement();
            Game.Input.OnPrimaryPress += PlaceFirstTile;
        }
    }

    public void CompletePlacement()
    {
        //Game.Input.OnPrimaryRelease -= CompletePlacement;


        if (PlacementIsValid && CanAffordPlacement)
        {
            Game.Input.OnSecondaryPress -= CancelPlacement;
            Game.Input.OnPrimaryPress -= TrySelectWall;

            Station.Money.Remove(costOfPlacement);

            if (Game.Debug.RoomCosts.ContainsKey(currentRoom))
                Game.Debug.RoomCosts[CurrentRoom] = costOfPlacement;
            else
                Game.Debug.RoomCosts.Add(currentRoom, costOfPlacement);

            currentRoom.SetRoomSize(CurrentRoomSize);
            currentRoom.AddFloorTile(firstTile);
            firstTile = null;
            foreach (FloorTile tile in currentPlacement)
                currentRoom.AddFloorTile(tile);
            currentPlacement.Clear();

            Station.AddRoom(currentRoom);
            currentRoom.ShowSelectedMaterial(true);
            currentRoom.AddWalls();
            if (Station.Rooms.Count > 1)
            {
                if (autoDoorwayPlacement)
                {
                    AutoDoorwayPlacement();
                    BuildNavMesh();
                    /*if (Game.Debug.RoomsBuilt.ContainsKey(currentRoom.Config))
                        Game.Debug.RoomsBuilt[currentRoom.Config]++;
                    else
                        Game.Debug.RoomsBuilt.Add(currentRoom.Config, 1);*/
                    Game.Selection.SelectRoom(currentRoom);
                    currentRoom = null;
                    currentBuildData = FloorBuildData.Empty;
                    tempBuildData = FloorBuildData.Empty;
                    previousBuildData = FloorBuildData.Empty;
                    UI.Sound.PlayPlaceFloorSound();
                    Game.Input.OnSecondaryPress -= CancelPlacement;
                }
                else
                {
                    placingDoor = true;
                    Game.Input.OnPrimaryPress += AddDoorToPlacement;
                }
            }
            else
            {
                BuildNavMesh();
                /*if (Game.Debug.RoomsBuilt.ContainsKey(currentRoom.Config))
                    Game.Debug.RoomsBuilt[currentRoom.Config]++;
                else
                    Game.Debug.RoomsBuilt.Add(currentRoom.Config, 1);*/
                Game.Selection.SelectRoom(currentRoom);
                currentRoom = null;
                currentBuildData = FloorBuildData.Empty;
                tempBuildData = FloorBuildData.Empty;
                previousBuildData = FloorBuildData.Empty;
                UI.Sound.PlayPlaceFloorSound();
                Game.Input.OnSecondaryPress -= CancelPlacement;
            }

            //AddWallsToCurrentRoom();
            inEditingMode = false;
        }

        else
        {
            DestroyAllPlacement();
            Game.Input.OnPrimaryPress += PlaceFirstTile;
        }
        
    }

    private void DestroyAllPlacement()
    {
        DestroyFirstTile();
        DestroyCurrentPlacement();
    }

    private void AutoDoorwayPlacement()
    {
        List<RoomObject> roomsToConnect = new List<RoomObject>();

        if (currentRoom.Config.Type == Room.EType.Hallway)
        {
            roomsToConnect = RoomsTouchingRoom(currentRoom);
        }
        else
        {
            roomsToConnect = RoomsTouchingRoom(currentRoom);

            foreach(RoomObject roomObject in roomsToConnect.ToArray())
                if (roomObject.Config.Type != Room.EType.Hallway)
                    roomsToConnect.Remove(roomObject);
        }

        foreach(RoomObject room in roomsToConnect)
        {
            ConnectRooms(currentRoom, room);
        }
    }

    private void AddDoorToPlacement()
    {
        FloorTile otherRoomTile = possibleDoorTile.OtherRoomTile();
        if (otherRoomTile != null)
        {
            possibleDoorTile.MakeDoorTile();
            otherRoomTile.MakeDoorTile();
            possibleDoorTile.AddWalls();
            possibleDoorTile.SwitchToBuitMaterial();
            possibleDoorTile = null;

            placingDoor = false;
            Game.Input.OnPrimaryPress -= AddDoorToPlacement;

            
            BuildNavMesh();
            if (Game.Debug.RoomsBuilt.ContainsKey(currentRoom.Config))
                Game.Debug.RoomsBuilt[currentRoom.Config]++;
            else
                Game.Debug.RoomsBuilt.Add(currentRoom.Config, 1);
            Game.Selection.SelectRoom(currentRoom);
            currentRoom = null;
            UI.Sound.PlayPlaceFloorSound();
            Game.Input.OnSecondaryPress -= CancelPlacement;
        }
    }

    private void BuildNavMesh()
    {
        if (Station.NavMeshSurface == null)
        {
            //firstTile.NavMeshSurface.enabled = true;
            currentRoom.Floor[0].NavMeshSurface.enabled = true;
            Station.SetNavMeshSurface(currentRoom.Floor[0].NavMeshSurface);
            Station.NavMeshSurface.BuildNavMesh();
        }
        else
            Station.NavMeshSurface.BuildNavMesh();
    }

    public void CancelPlacement()
    {
        Game.Input.OnSecondaryPress -= CancelPlacement;
        
        if (Game.Input.PrimaryButtonDown)
            Game.Input.OnPrimaryRelease -= PlacementFinished;
        else if (firstTile != null)
            Game.Input.OnPrimaryPress -= PlaceFirstTile;

        DestroyAllPlacement();

        if (previousBuildData.IsEmpty)
        {
            Destroy(currentRoom.gameObject);
        }
        else
        {
            currentBuildData = previousBuildData;
            SpawnCurrentPlacement();
            currentRoom.AddFloorTile(firstTile);
            firstTile = null;
            foreach (FloorTile tile in currentPlacement)
                currentRoom.AddFloorTile(tile);
            currentPlacement.Clear();
            Station.AddRoom(currentRoom);
            Game.Selection.SelectRoom(currentRoom);
        }

        Game.Input.OnPrimaryPress -= TrySelectWall;
        UI.Sound.PlayButtonCancelSound();
        currentRoom = null;
        currentBuildData = FloorBuildData.Empty;
        tempBuildData = FloorBuildData.Empty;
        previousBuildData = FloorBuildData.Empty;
        inEditingMode = false;
    }

    private List<RoomObject> RoomsTouchingTile(FloorTile tile)
    {
        List<RoomObject> list = new List<RoomObject>();

        Vector3 up = tile.transform.position;
        up.z += TileSize.y;
        FloorTile upTile = Station.TileAtLocation(up);
        if (upTile != null && upTile.Room != null)
            list.Add(upTile.Room);

        Vector3 down = tile.transform.position;
        down.z -= TileSize.y;
        FloorTile downTile = Station.TileAtLocation(down);
        if (downTile != null && downTile.Room != null)
            list.Add(downTile.Room);

        Vector3 left = tile.transform.position;
        left.x -= TileSize.x;
        FloorTile leftTile = Station.TileAtLocation(left);
        if (leftTile != null && leftTile.Room != null)
            list.Add(leftTile.Room);

        Vector3 right = tile.transform.position;
        right.x += TileSize.x;
        FloorTile rightTile = Station.TileAtLocation(right);
        if (rightTile != null && rightTile.Room != null)
            list.Add(rightTile.Room);


        return list;
    }

    private List<RoomObject> RoomsTouchingRoom(RoomObject room)
    {
        List<RoomObject> list = new List<RoomObject>();

        foreach(FloorTile tile in room.Floor)
        {
            foreach (RoomObject otherRoom in RoomsTouchingTile(tile))
            {
                if (!list.Contains(otherRoom)) list.Add(otherRoom);
            }
        }

        if (list.Contains(room)) list.Remove(room);
        return list;
    }

    private List<RoomObject> roomsTouchingPlacement
    {
        get
        {
            List<RoomObject> list = new List<RoomObject>();

            foreach (RoomObject room in RoomsTouchingTile(firstTile).ToArray())
            {
                if (!list.Contains(room)) list.Add(room);
            }

            foreach (FloorTile tile in currentPlacement)
            {
                foreach (RoomObject room in RoomsTouchingTile(tile).ToArray())
                {
                    if (!list.Contains(room)) list.Add(room);
                }
            }

            return list;
        }
    }

    public bool PlacementIsValid
    {
        get
        {
            if (firstTile == null || currentPlacement.Count == 0) return false;

            if (CurrentRoomSize.x < currentRoom.Config.MinimumSize.x)
            {
                //Debug.LogWarning("Room width too small");
                return false;
            }

            if (CurrentRoomSize.y < currentRoom.Config.MinimumSize.y)
            {
                //Debug.LogWarning("Room height too small");
                return false;
            }

            // check current Placement is not overlapping
            if (Station.TileAtLocation(firstTile.transform.position))
            {
                //Debug.LogWarning("First tile overlapping");
                return false;
            }

            foreach (FloorTile tile in currentPlacement.ToArray())
            {
                if (Station.TileAtLocation(tile.transform.position))
                {
                    //Debug.LogWarning("Placement overlapping");
                    return false;
                }
                    
            }


            // check that room is touching hallway
            if (Station.HasRooms) // if the station has rooms check that current placement is touching a hallway or the currently-being-placed hallway is touching another room
            {
                if (currentRoom.Config.Type != Room.EType.Hallway) // not placing a hallway
                {
                    bool containsAHallway = false;

                    foreach (RoomObject room in roomsTouchingPlacement)
                    {
                        if (room.Config.Type == Room.EType.Hallway)
                        {
                            containsAHallway = true;
                        }
                    }

                    if (!containsAHallway)
                    {
                        Debug.LogWarning("Placement not touching hallway");
                        return false;
                    }
                }
                else // placing a hallway
                {
                    bool touchingSomeOtherRoom = false;

                    foreach (RoomObject room in roomsTouchingPlacement)
                    {
                        if (room.Config.Type != Room.EType.Hallway)
                            touchingSomeOtherRoom = true;
                    }

                    if (!touchingSomeOtherRoom)
                    {
                        Debug.LogWarning("Hallway not touching any rooms");
                        return false;
                    }
                }
            }

            return true;
        }
    }

    private void AddWallsToCurrentRoom()
    {
        foreach (FloorTile tile in currentRoom.Floor)
        {
            AddWallsToTile(tile);
        }

        /*foreach (RoomObject room in RoomsTouchingRoom(currentRoom))
        {
            foreach (FloorTile tile in room.Floor)
            {
                AddWallsToTile(tile);
            }
        }*/
    }

    private void AddWallsToTile(FloorTile tile)
    {
        tile.RemoveWalls();

        Vector3 up = tile.transform.position;
        up.z += TileSize.y;
        FloorTile upTile = Station.TileAtLocation(up);
        if (upTile == null)
            tile.AddTopWall();
        else AddWallsToTileSingle(upTile);

            Vector3 down = tile.transform.position;
        down.z -= TileSize.y;
        FloorTile downTile = Station.TileAtLocation(down);
        if (downTile == null)
            tile.AddBottomWall();
        else AddWallsToTileSingle(downTile);

            Vector3 left = tile.transform.position;
        left.x -= TileSize.x;
        FloorTile leftTile = Station.TileAtLocation(left);
        if (leftTile == null)
            tile.AddLeftWall();
        else AddWallsToTileSingle(leftTile);

        Vector3 right = tile.transform.position;
        right.x += TileSize.x;
        FloorTile rightTile = Station.TileAtLocation(right);
        if (rightTile == null)
            tile.AddRightWall();
        else AddWallsToTileSingle(rightTile);

        tile.ApplyPillers();
    }

    private void AddWallsToTileSingle(FloorTile tile)
    {
        tile.RemoveWalls();

        Vector3 up = tile.transform.position;
        up.z += TileSize.y;
        FloorTile upTile = Station.TileAtLocation(up);
        if (upTile == null)
            tile.AddTopWall();

        Vector3 down = tile.transform.position;
        down.z -= TileSize.y;
        FloorTile downTile = Station.TileAtLocation(down);
        if (downTile == null)
            tile.AddBottomWall();

        Vector3 left = tile.transform.position;
        left.x -= TileSize.x;
        FloorTile leftTile = Station.TileAtLocation(left);
        if (leftTile == null)
            tile.AddLeftWall();

        Vector3 right = tile.transform.position;
        right.x += TileSize.x;
        FloorTile rightTile = Station.TileAtLocation(right);
        if (rightTile == null)
            tile.AddRightWall();

        tile.ApplyPillers();
    }

    public bool CanAffordPlacement
    {
        get
        {
            if (currentRoom == null || firstTile == null) return false;
            return Station.Money.CanAfford(costOfPlacement);
        }
    }

    private int costOfPlacement
    {
        get
        {
            if (firstTile == null) return 0;
            return currentRoom.Config.Cost(1 + currentPlacement.Count)/* - previousCost*/;
        }
    }

    public void ConnectRooms(RoomObject room1, RoomObject room2)
    {
        List<FloorTile> tileWhichConnect = new List<FloorTile>();

        foreach (FloorTile tile in room1.Floor)
        {
            if (tile.TouchingRooms.Contains(room2)) tileWhichConnect.Add(tile);
        }

        if (tileWhichConnect.Count == 1)
        {
            tileWhichConnect[0].MakeDoorTile();
        }

        else
        {
            FloorTile lowestTile = null;
            FloorTile hightestTile = null;

            if (tileWhichConnect[0].x == tileWhichConnect[tileWhichConnect.Count-1].x)
            {
                float highest = float.NegativeInfinity;
                float lowest = float.PositiveInfinity;
                

                foreach (FloorTile tile in tileWhichConnect)
                {
                    if (tile.z > highest)
                    {
                        highest = tile.z;
                        hightestTile = tile;
                    }

                    if (tile.z < lowest)
                    {
                        lowest = tile.z;
                        lowestTile = tile;
                    }
                }
            }

            else if (tileWhichConnect[0].z == tileWhichConnect[tileWhichConnect.Count - 1].z)
            {
                float highest = float.NegativeInfinity;
                float lowest = float.PositiveInfinity;

                foreach (FloorTile tile in tileWhichConnect)
                {
                    if (tile.x > highest)
                    {
                        highest = tile.x;
                        hightestTile = tile;
                    }

                    if (tile.x < lowest)
                    {
                        lowest = tile.x;
                        lowestTile = tile;
                    }
                }
            }

            if (hightestTile != null && lowestTile != null)
            {
                float xDif = hightestTile.x - lowestTile.x;
                float zDif = hightestTile.z - lowestTile.z;
                Vector3 centerPoint = new Vector3(lowestTile.x + (xDif/2), lowestTile.transform.position.y, lowestTile.z + (zDif/2));

                float closestDistance = float.PositiveInfinity;
                FloorTile closestTile = null;

                foreach (FloorTile tile in tileWhichConnect)
                {
                    float distance = Vector3.Distance(tile.transform.position, centerPoint);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTile = tile;
                    }
                }

                if (closestTile != null)
                {
                    closestTile.MakeDoorTile();
                }
            }
        }
    }

    private void TrySelectWall()
    {
        if (UI.MouseOverUI == true)
        {
            Debug.LogWarning("TrySelectWall UI.MouseOverUI == true");
            return;
        }

        Vector3 clickLocation = RoundToNearestGrid(GroundLocationUnderMouse);
        //Debug.Log("TrySelectWall at = " + clickLocation.ToString());
        FloorTile tileUnderMouse = TileAtLocation(clickLocation);

        if (tileUnderMouse != null && tileUnderMouse.IsEdgeTile)
        {
            if (tileUnderMouse.TopEdge) selectedWallSide = EWallSide.TopWall;
            else if (tileUnderMouse.BottomEdge) selectedWallSide= EWallSide.BottomWall;
            else if (tileUnderMouse.LeftEdge) selectedWallSide = EWallSide.LeftWall;
            else if (tileUnderMouse.RightEdge) selectedWallSide = EWallSide.RightWall;

            Game.Input.OnPrimaryRelease += StopHoldingWall;

            topLeftPoint = tileUnderMouse.transform.position;
            botttomRightPoint = topLeftPoint;
            tempBuildData = currentBuildData;
        }

        else if (tileUnderMouse != null)
        {
            Debug.Log("Tile is not edge");
        }
    }

    private void StopHoldingWall()
    {
        Game.Input.OnPrimaryRelease -= StopHoldingWall;
        selectedWallSide = EWallSide.NoWall;
        tempBuildData = currentBuildData;
    }

    private FloorTile TileAtLocation(Vector3 location)
    {
        if (firstTile == null)
        {
            Debug.LogWarning("First tile is null");
            return null;
        }
        if (currentPlacement.Count == 0)
        {
            Debug.LogWarning("currentPlacement.Count == 0");
            return null;
        }

        if (firstTile.transform.position == location) return firstTile;

        foreach (FloorTile tile in currentPlacement)
        {
            if (tile.transform.position == location)
            {
                return tile;
            }
        }

        return null;
    }

    private Vector2 LocalTilePosition(FloorTile tile)
    {
        return new Vector2(tile.x - firstTile.x, tile.z - firstTile.z);
    }

    private EWallSide TileWallSide(FloorTile tile)
    {
        Vector2 localPosition = LocalTilePosition(tile);

        if (localPosition.x == 0) return EWallSide.LeftWall;
        else if (localPosition.x == CurrentRoomSize.x) return EWallSide.RightWall;
        else if (localPosition.y == CurrentRoomSize.y) return EWallSide.BottomWall;
        else if (localPosition.y == 0) return EWallSide.TopWall;

        return EWallSide.NoWall;
    }

    List<FloorTile> currentPlacementAll
    {
        get
        {
            List<FloorTile> list = new List<FloorTile>();

            list.Add(firstTile);

            foreach (FloorTile tile in currentPlacement)
            {
                list.Add(tile);
            }

            return list;
        }
    }

    private void SetWallSideOnPlacement()
    {
        if (currentPlacementAll.Count == 0) return;

        float lowestX = float.MaxValue;
        float highestX = float.MinValue;
        float lowestZ = float.MaxValue;
        float highestZ = float.MinValue;

        foreach (FloorTile tile in currentPlacementAll)
        {
            if (tile.x < lowestX) lowestX = tile.x;
            else if (tile.x > highestX) highestX = tile.x;

            if (tile.z < lowestZ) lowestZ = tile.z;
            else if (tile.z > highestZ) highestZ = tile.z;
        }

        foreach (FloorTile tile in currentPlacementAll)
        {
            tile.ClearEdges();

            if (tile.x == lowestX)
            {
                tile.LeftEdge = true;
            }
            else if (tile.x == highestX)
            {
                tile.RightEdge = true;
            }

            if (tile.z == lowestZ)
            {
                tile.BottomEdge = true;
            }
            else if (tile.z == highestZ)
            {
                tile.TopEdge = true;
            }
        }
    }

    private void SelectTopEdge()
    {

    }

    private void SelectBottomEdge()
    {

    }

    private void SelectLeftEdge()
    {

    }

    private void SelectRightEdge()
    {

    }
}
