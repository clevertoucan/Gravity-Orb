using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshGenerator : MonoBehaviour {
    public SquareGrid squareGrid;
    public MeshFilter walls;
    public MeshFilter cave;
    SquareGrid oldGrid = null;

    List<Vector3> vertices;
    List<int> triangles;

    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();

    public float wallHeight = 5f;

    [Range(0, 1)]
    public float foregroundValue = .1f;
    [Range(0, 1)]
    public float wallValue = .1f;
    [Range(0, 1)]
    public float goalValue = .1f;

    bool playerColorSet = false, won;

    public Material playerMaterial;
    public Material wallMaterial;
    public Material foregroundMaterial;
    public Material goalMaterial;
    public ParticleSystem playerParticleSystem;
    public ParticleSystem envParticleSystem;
    public ParticleSystem goalParticleSystem;
    public MaskableGraphic[] uiElements;
    public static HashSet<Image> markers = new HashSet<Image>();

    public GameObject goal, player;
    public static MeshGenerator instance;

    private void OnDestroy() {
        markers.Clear();
    }

    private void Awake() {
        instance = this;
    }

    /*
    public override void SetColors() {
        if (!playerColorSet) {
            playerMaterial.color = themeColor;
        }
        wallMaterial.color = Complementary(playerMaterial.color, wallValue);
        foregroundMaterial.color = Complementary(playerMaterial.color, foregroundValue);
        goalMaterial.color = Complementary(playerMaterial.color, goalValue);
        Camera.main.backgroundColor = Complementary(playerMaterial.color, backgroundValue);
        var pmain = playerParticleSystem.main;
        var emain = envParticleSystem.main;
        var gmain = goalParticleSystem.main;
        pmain.startColor = playerMaterial.color;
        emain.startColor = goalMaterial.color;
        gmain.startColor = goalMaterial.color;
        foreach (MaskableGraphic i in uiElements) {
            float localH, localS, localV;
            Color.RGBToHSV(i.color, out localH, out localS, out localV);
            i.color = Color.HSVToRGB(startH, s, v);
        }
        foreach (Image i in markers) {
            i.color = themeColor;
        }
    }
    */

    public void GenerateMesh(int[,] map, float squareSize) {
        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();
        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();
        float timeStart = Time.time;
        for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        cave.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        
        CreateWallMesh();

        oldGrid = squareGrid;
   }

    void  CreateWallMesh() {

        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5;

        foreach (List<int> outline in outlines) {
            for (int i = 0; i < outline.Count - 1; i++) {
                int startIndex = wallVertices.Count;
                
                wallVertices.Add(vertices[outline[i]]); // left
                wallVertices.Add(vertices[outline[i + 1]]); // right
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
               
            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        
        walls.mesh = wallMesh;
        
        foreach (MeshCollider collider in walls.gameObject.GetComponents<MeshCollider>()) {
            Destroy(collider);
        }
        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        wallCollider.sharedMesh = wallMesh;
    }

    void TriangulateSquare(Square square) {
        switch (square.configuration) {
            case 0:
                break;

            // 1 points:
            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;

            // 2 points:
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 3 point:
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;

            // 4 point:
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }
    }

    void MeshFromPoints(params Node[] points) {
        AssignVertices(points);

        if(points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);
    }

    void AssignVertices(Node[] points) {
        for (int i = 0; i < points.Length; i++) {
            if(points[i].vertexIndex == -1) {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c) {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
        if (triangleDictionary.ContainsKey(vertexIndexKey)) {
            triangleDictionary[vertexIndexKey].Add(triangle);
        } else {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }

    void CalculateMeshOutlines() {
        for(int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
            if (!checkedVertices.Contains(vertexIndex)) {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if(newOutlineVertex != -1) {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex, int outlineIndex) {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1) {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }


    int GetConnectedOutlineVertex(int vertexIndex) {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for(int i = 0; i < trianglesContainingVertex.Count; i++) {
            Triangle triangle = trianglesContainingVertex[i];

            for(int j = 0; j < 3; j++) {
                int vertexB = triangle[j];
                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB) && IsOutlineEdge(vertexIndex, vertexB)) {
                    return vertexB;
                }
            }
        }

        return -1;
    }

    bool IsOutlineEdge(int vertexA, int vertexB) {
        List<Triangle> trianglesA = triangleDictionary[vertexA];
        int sharedTriangleCount = 0;
        for (int i = 0; i < trianglesA.Count; i++) {
            if (trianglesA[i].Contains(vertexB)) {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1) {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    struct Triangle {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;

        int[] vertices;

        public Triangle(int a, int b, int c) {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        public int this[int i] {
            get {
                return vertices[i];
            }
        }

        public bool Contains(int vertexIndex) {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    public class SquareGrid {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize) {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++) {
                for (int y = 0; y < nodeCountY; y++) {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++) {
                for (int y = 0; y < nodeCountY - 1; y++) {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }


    public class Square {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;
        public int configuration;

        public Square (ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;

            if (topLeft.active) {
                configuration += 8;
            } if (topRight.active){
                configuration += 4;
            } if (bottomRight.active) {
                configuration += 2;
            } if (bottomLeft.active) {
                configuration += 1;
            }
        }

        public override bool Equals(object obj) {
            var square = obj as Square;
            return square != null &&
                   EqualityComparer<ControlNode>.Default.Equals(topLeft, square.topLeft) &&
                   EqualityComparer<ControlNode>.Default.Equals(topRight, square.topRight) &&
                   EqualityComparer<ControlNode>.Default.Equals(bottomRight, square.bottomRight) &&
                   EqualityComparer<ControlNode>.Default.Equals(bottomLeft, square.bottomLeft) &&
                   EqualityComparer<Node>.Default.Equals(centerTop, square.centerTop) &&
                   EqualityComparer<Node>.Default.Equals(centerRight, square.centerRight) &&
                   EqualityComparer<Node>.Default.Equals(centerBottom, square.centerBottom) &&
                   EqualityComparer<Node>.Default.Equals(centerLeft, square.centerLeft) &&
                   configuration == square.configuration;
        }

        public override int GetHashCode() {
            var hashCode = 969445268;
            hashCode = hashCode * -1521134295 + EqualityComparer<ControlNode>.Default.GetHashCode(topLeft);
            hashCode = hashCode * -1521134295 + EqualityComparer<ControlNode>.Default.GetHashCode(topRight);
            hashCode = hashCode * -1521134295 + EqualityComparer<ControlNode>.Default.GetHashCode(bottomRight);
            hashCode = hashCode * -1521134295 + EqualityComparer<ControlNode>.Default.GetHashCode(bottomLeft);
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(centerTop);
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(centerRight);
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(centerBottom);
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(centerLeft);
            hashCode = hashCode * -1521134295 + configuration.GetHashCode();
            return hashCode;
        }
    }

    public class Node {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos) {
            position = _pos;
        }
        public override bool Equals(object obj) {
            var node = obj as Node;
            return node != null &&
                   EqualityComparer<Vector3>.Default.Equals(position, node.position) &&
                   vertexIndex == node.vertexIndex;
        }

        public override int GetHashCode() {
            var hashCode = 994972775;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(position);
            hashCode = hashCode * -1521134295 + vertexIndex.GetHashCode();
            return hashCode;
        }
    }

    public class ControlNode : Node {

        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }

        public override bool Equals(object obj) {
            var node = obj as ControlNode;
            return node != null &&
                   base.Equals(obj) &&
                   active == node.active &&
                   EqualityComparer<Node>.Default.Equals(above, node.above) &&
                   EqualityComparer<Node>.Default.Equals(right, node.right);
        }

        public override int GetHashCode() {
            var hashCode = -1016028898;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + active.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(above);
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(right);
            return hashCode;
        }
    }
}
