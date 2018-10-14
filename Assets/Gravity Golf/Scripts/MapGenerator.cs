using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour {
    public static Vector3 startPlayerPosition;
    public int publicWidth, publicHeight, smoothing, borderSize = 5, minRegionSize = 50, passageSize = 3;
    int width, height;
    public string seed;
    public bool useRandomSeed, godMode;
    public GameObject[] enemies;
    public HashSet<EnemyController> spawnedEnemies = new HashSet<EnemyController>(); 
    public GameObject radar;
    public Image RestartButton;
    GameObject player, goal;
    GameManager manager;
    public delegate void OnMapGeneratedAction();
    public static event OnMapGeneratedAction OnMapGenerated;

    [Range(0,100)]
    public int randomFillPercent;

    int[,] map;
    int[,] borderedMap;

    MeshGenerator mg;

    Room playerSpawnRoom, goalSpawnRoom;
    List<Room> rooms;

    public static MapGenerator instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        mg = GetComponent<MeshGenerator>();
        manager = GameManager.instance;
        GameManager.OnReset += OnReset;
        player = manager.player;
        goal = manager.goal;
        GenerateMap();
        if (OnMapGenerated != null) {
            OnMapGenerated();
        }
    }

    private void Update() {
        /*
        if (Input.GetMouseButtonDown(0)) {
            DestroyCircle(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y)), 10);
        }
        */
    }

    void OnReset() {
        player.SetActive(true);
        player.transform.position = playerSpawnPoint;
    }

    public void GenerateMap() {
        width = publicWidth;
        height = publicHeight;
        map = new int[width, height];
        RandomFillMap();
        Room.largestRoom = new Room();

        for (int i = 0; i < smoothing; i++) {
            SmoothMap();
        }

        ProcessMap();
        SpawnPlayer();
        SpawnGoal();
        SpawnEnemies();
        borderedMap = new int[width + borderSize * 2, height + borderSize * 2];
        for (int x = 0; x < borderedMap.GetLength(0); x++) {
            for (int y = 0; y < borderedMap.GetLength(1); y++) {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                } else {
                    borderedMap[x, y] = 1;
                }
            }
        }
        width += borderSize * 2;
        height += borderSize * 2;
        map = borderedMap;
        
        mg.GenerateMesh(map, 1);
    }

    public int GetFill(Coord coord) {
        return IsInMapRange(coord.tileX, coord.tileY)? map[coord.tileX, coord.tileY]: -1;
    }

    Vector3 playerSpawnPoint;
    void SpawnPlayer() {
        playerSpawnRoom = Room.largestRoom;
        Coord spawn = playerSpawnRoom.middle;
        float distance = Mathf.Infinity;
        foreach (Coord c in playerSpawnRoom.tiles) {
            float currentDistance = Magnitude(c, playerSpawnRoom.middle);
            if (currentDistance < distance && map[c.tileX, c.tileY] == 0) {
                spawn = c;
                distance = currentDistance;
            }
        }
        DrawCircle(spawn, 5, 0);
        playerSpawnPoint = new Vector3(.5f, -4.5f, 0) + CoordToWorldPoint(spawn);
        player.transform.position = playerSpawnPoint;
        startPlayerPosition = player.transform.position;
        Camera.main.transform.position = new Vector3(0, 8, 0) + CoordToWorldPoint(spawn);
    }

    void SpawnGoal() {
        goalSpawnRoom = null;
        float distance = 0;
        foreach (Room r in rooms) {
            float d = (CoordToWorldPoint(r.middle) - CoordToWorldPoint(playerSpawnRoom.middle)).sqrMagnitude;
            if (d > distance) {
                goalSpawnRoom = r;
                distance = d;
            }
        }
        DrawCircle(goalSpawnRoom.middle, 5, 0);
        goal.transform.position = new Vector3(.5f, -4.5f, 0) + CoordToWorldPoint(goalSpawnRoom.middle);
        goal.SetActive(true);
    }

    void SpawnEnemies() {
        foreach (Room r in rooms) {
            if (r != playerSpawnRoom && r != goalSpawnRoom && enemies.Length > 0) {
                GameObject e = Instantiate(enemies[UnityEngine.Random.Range(0, enemies.Length)], new Vector3(.5f, -4.5f, 0) + CoordToWorldPoint(r.middle), Quaternion.Euler(Vector3.zero));
                DrawCircle(r.middle, 3, 0);
                spawnedEnemies.Add(e.GetComponentInChildren<EnemyController>());
            }
        }
    }


    public void DestroyCircle(Vector3 pos, int radius) {
        DrawCircle(WorldToCoordPoint(pos), radius, 0);
        mg.GenerateMesh(map, 1);
    }

    void ProcessMap() {
        List<List<Coord>> wallRegions = GetRegions(1);

        foreach (List<Coord> wallRegion in wallRegions) {
            if (wallRegion.Count < minRegionSize) {
                foreach (Coord tile in wallRegion) {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions) {
            if (roomRegion.Count < minRegionSize) {
                foreach (Coord tile in roomRegion) {
                    map[tile.tileX, tile.tileY] = 1;
                }
            } else {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }

        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;
        ConnectClosestRooms(survivingRooms);
        rooms = survivingRooms;
    }
    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibility = false) {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibility) {
            foreach(Room room in allRooms) {
                if (room.isAccessibleFromMainRoom) {
                    roomListB.Add(room);
                } else {
                    roomListA.Add(room);
                }
            }
        } else {
            roomListA = allRooms;
            roomListB = allRooms;
        }
        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA) {
            if (!forceAccessibility) {
                possibleConnectionFound = false;
                if(roomA.connectedRooms.Count > 0) {
                    continue;
                }
            }
            foreach (Room roomB in roomListB) {
                if (roomA == roomB || roomA.IsConnected(roomB)) {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibility) {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibility) {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibility) {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach(Coord c in line) {
            DrawCircle(c, passageSize, 0);
        }
    }

    void DrawCircle(Coord c, int r, int fill) {
        for(int x = -r; x <= r; x++) { 
            for(int y = -r; y<=r; y++) {
                if(x*x + y*y <= r * r) {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if(IsInMapRange(drawX, drawY)) {
                        map[drawX, drawY] = fill;
                    }
                }
            }
        }
    }

    List<Coord> GetLine(Coord from, Coord to) {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;
        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest) {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for(int i = 0; i < longest; i++) {
            line.Add(new Coord(x, y));

            if (inverted) {
                y += step;
            } else {
                x += step;
            }

            gradientAccumulation += shortest;
            if(gradientAccumulation >= longest) {
                if (inverted) {
                    x += gradientStep;
                } else {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    public Vector3 CoordToWorldPoint(Coord tile) {
        float x = tile.tileX - ( ( width ) / 2 - .5f );
        float y = 2;
        float z = tile.tileY - ( ( height ) / 2 - .5f );
        return new Vector3(x, y, z);
    }

    public Coord WorldToCoordPoint(Vector3 position) {
        int x = (int)( (( width ) / 2 - .5f) + position.x );
        int y = (int)( ( height ) / 2 - .5f + position.z );
        Coord coord = new Coord(x, y);

        return coord;
    }

    List<List<Coord>> GetRegions(int tileType) {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType) {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion) {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX)) {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType) {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    bool IsInMapRange(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void RandomFillMap() {
        if (useRandomSeed) {
            seed = DateTime.Now.Millisecond.ToString();
        }

        System.Random rng = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 1;
                } else {
                    map[x, y] = rng.Next(0, 100) < randomFillPercent ? 1 : 0;
                }
            }
        }
        
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighborWallTiles = GetSurroundingWallCount(x, y);

                if (neighborWallTiles > 4) {
                    map[x, y] = 1;
                } else if (neighborWallTiles < 4) { 
                    map[x, y] = 0;
                }
            }
        }
   }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for(int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX ++) {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++) {
                if (neighborX >= 0 && neighborX < width && neighborY >= 9 && neighborY < height) {
                    if (neighborX != gridX || neighborY != gridY) {
                        wallCount += map[neighborX, neighborY];
                    }
                } else {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    float Magnitude(Coord c1, Coord c2) {
        return Mathf.Sqrt(Mathf.Pow((c1.tileX - c2.tileX), 2) + Mathf.Pow(( c1.tileY - c2.tileY ), 2));
    }

    public struct Coord {
        public int tileX;
        public int tileY;

        public Coord(int x, int y) {
            tileX = x;
            tileY = y;
        }
    }

    class Room : IComparable<Room>{
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public static Room largestRoom = new Room();
        public int roomSize, startX = int.MaxValue, startY = int.MaxValue, endX = -1, endY = -1;
        public bool isAccessibleFromMainRoom = false, isMainRoom = false;
        public Coord middle;

        public Room() {
            roomSize = -1;
        }

        public Room(List<Coord> roomTiles, int[,] map) {
            tiles = roomTiles;
            roomSize = tiles.Count;
            if(roomSize > largestRoom.roomSize) {
                largestRoom = this;
            }
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles) {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                        if (x == tile.tileX || y == tile.tileY) {
                            if (map[x, y] == 1) {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
                startX = tile.tileX < startX ? tile.tileX : startX;
                endX = tile.tileX > endX ? tile.tileX : endX;
                startY = tile.tileY < startY ? tile.tileY : startY;
                endY = tile.tileY > endY ? tile.tileY : endY;
            }
            middle = new Coord(endX - Mathf.Abs(endX - startX) / 2, endY - Mathf.Abs(endY - startY) / 2);
        }

        public void SetAccessibleFromMainRoom() {
            if(!isAccessibleFromMainRoom) {
                isAccessibleFromMainRoom = true;
                foreach(Room connectedRoom in connectedRooms) {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB) {
            if (roomA.isAccessibleFromMainRoom) {
                roomB.SetAccessibleFromMainRoom();
            } else if (roomB.isAccessibleFromMainRoom) {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom) {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom) {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }

    private void OnDestroy() {
        OnMapGenerated = null;
    }
}
