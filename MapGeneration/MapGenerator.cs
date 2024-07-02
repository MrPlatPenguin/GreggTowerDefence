using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] float _cellSize = 5;
    [SerializeField] int _chunkSize = 8;
    [SerializeField] Material _material;
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] Structure _mainTower;
    [SerializeField] Biome[] _biomes;
    [SerializeField] int _mapSize;
    [SerializeField] Transform _resourceContainer;
    [SerializeField] Resource _resourcePrefab;
    [SerializeField] int _seed = 0;
    [SerializeField] int _resourceGenerationBuffer;
    [SerializeField] List<Resource> pooledResources = new List<Resource>();
    [SerializeField] List<Resource> activeResources = new List<Resource>();

    Grid<TileObject> _grid;
    GameObject[,] _chunkMeshs;
    VoronoiNoise _biomeNoise;
    Vector2 _mapCentre;

    #region Statics
    static MapGenerator Instance { get; set; }
    public static float CellSize { get => Instance._cellSize; }
    public static Vector2 MapCentre { get => Instance._mapCentre; }
    public static Grid<TileObject> Grid { get => Instance._grid; }
    public static int HomeSize { get => LandUpgrader.GetSize(); }
    public static VoronoiNoise BiomeNoise { get => Instance._biomeNoise; }
    public static int MapRadius { get => Instance._mapSize; }
    public static Biome[] Biomes { get => Instance._biomes; }
    public static GameObject[,] ChunkMeshs { get => Instance._chunkMeshs; }
    public static Transform ResourceContainer { get => Instance._resourceContainer; }
    public static Material BiomeMaterial { get => Instance._material; }
    public static int Seed { get => Instance._seed; set => Instance._seed = value; }
    public static int ResourceGenerationBuffer { get => Instance._resourceGenerationBuffer; }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateMap()
    {
        GameManager.OnNightStart += delegate { new Delay(this, Mathf.Lerp(0, GameManager.NightLength, 0.5f), () => StartCoroutine(RegenerateResources())); };

        _biomeNoise = new VoronoiNoise(MapRadius * 2, MapRadius * 2, 10, _cellSize, Seed);

        _grid = new Grid<TileObject>(_chunkSize, MapRadius * 2, MapRadius * 2, _cellSize, Vector3.zero, (Grid<TileObject> g, int x, int y) => new TileObject(g, x, y));
        SetGrid(_grid);
        _mapCentre = _grid.GetGridCentreWorldPosition(_grid.CentreX, _grid.CentreY);
        StartCoroutine(RegenerateResources());
        _grid.GetGridObject(_grid.CentreX, _grid.CentreY).CreateNewWorldObject(_mainTower);
        _mainTower = (Structure)_grid.GetGridObject(_grid.CentreX, _grid.CentreY).WorldObject;
        _mainTower.tile = _grid.GetGridObject(_grid.CentreX, _grid.CentreY);
        _grid.GetGridObject(_grid.CentreX, _grid.CentreY - 1).SetWorldObjectDirty(_mainTower);

        Instantiate(_playerPrefab, MapCentre, Quaternion.identity);
        MapGeneratorEvents.MapGenerated();
    }

    public void SetGrid(Grid<TileObject> grid)
    {
        this._grid = grid;

        CreateMeshs();
    }

    void Grid_OnGridValueChanged(object sender, Grid<TileObject>.OnGridValueChangedEventArgs e)
    {
        
    }

    void CreateMeshs()
    {
        int chunkIndex = 0;
        float worldChunkSize = _cellSize * _chunkSize;
        _chunkMeshs = new GameObject[_grid.Width / _chunkSize, _grid.Height / _chunkSize];

        for (int i = 0; i < _chunkMeshs.GetLength(0); i++)
        {
            for (int ii = 0; ii < _chunkMeshs.GetLength(1); ii++)
            {
                UpdateChunk(i, ii, worldChunkSize);

                chunkIndex++;
            }
        }
    }

    void UpdateChunk(int x, int y, float worldChunkSize)
    {
        // Removes old chunks if they exist
        if (_chunkMeshs[x,y] != null)
            Destroy(_chunkMeshs[x, y].gameObject);

        // Creates the new chunk
        GameObject newChunk = new GameObject("Chunk" + x + ", " + y);
        _chunkMeshs[x, y] = newChunk;
        newChunk.transform.parent = transform;
        newChunk.transform.localPosition = new Vector3(x * worldChunkSize, y * worldChunkSize);
        MeshFilter meshFilter = newChunk.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newChunk.AddComponent<MeshRenderer>();
        SortingGroup sortingGroup = newChunk.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerID = SortingLayer.NameToID("Background");
        meshRenderer.material = _material;
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        DrawChunk(mesh, x, y);
    }

    public static void UpdateChunk(int x, int y)
    {
        float worldChunkSize = Instance._cellSize * Instance._chunkSize;

        Instance.UpdateChunk(x, y, worldChunkSize);
    }

    void DrawChunk(Mesh mesh, int chunkX, int chunkY)
    {
        Vector3 quadSize = new Vector3(1, 1) * _grid.CellSize * 1.0001f;
        mesh.Clear();
        MeshUtils.CreateEmptyMeshArrays(_chunkSize * _chunkSize, out Vector3[] verticies, out Vector2[] uv, out int[] triangles);

        int index = 0;

        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                TileObject gridValue = _grid.GetGridObject(x + (chunkX * _chunkSize), y + (chunkY * _chunkSize));
                gridValue.GetBiomeTexture(out Vector2 UV00, out Vector2 UV11);
                MeshUtils.AddToMeshArrays(verticies, uv, triangles, index, _grid.GetWorldPosition(x, y) + (quadSize * 0.5f), 0f, quadSize, UV00, UV11);
                index++;                                                                                                                // ^^^^^^^^^^ This shows the texture on the quad
            }
        }
        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    IEnumerator RegenerateResources()
    {
        Resource.ClearResourcesPool();
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                _grid.GetGridObject(x, y).RandomizeResource(_resourcePrefab, ResourceContainer);
            }
            yield return null;
        }
    }

    public static void SetHomeSize(int radius)
    {
        TileObject centreTile = Grid.GetGridObject(MapCentre);
        int centerX = centreTile.x; // set the x coordinate of the center point
        int centerY = centreTile.y; // set the y coordinate of the center point

        for (int x = centerX - radius - ResourceGenerationBuffer; x <= centerX + radius + ResourceGenerationBuffer; x++)
        {
            for (int y = centerY - radius - ResourceGenerationBuffer; y <= centerY + radius + ResourceGenerationBuffer; y++)
            {
                float distanceFromCentre = (new Vector2(x, y) - new Vector2(Grid.CentreX, Grid.CentreY)).magnitude - 0.5f;
                if (distanceFromCentre < radius)
                {
                    TileObject tile = Grid.GetGridObject(x, y);
                    tile.SetBiomeManual(0);

                    if (tile.WorldObject is Resource)
                        tile.CreateNewWorldObject<WorldObject>(null);

                }
                else if (distanceFromCentre < radius + ResourceGenerationBuffer)
                {
                    TileObject tile = Grid.GetGridObject(x, y);
                    tile.SetBiomeManual(1);

                }
            }
        }
    }
}

[System.Serializable]
public class TileObject
{
    protected Grid<TileObject> grid;
    public int x { get; protected set; }
    public int y { get; protected set; }
    public Biome biome { get; private set; }
    WorldObject _worldObject;
    public WorldObject WorldObject { get => _worldObject;}
    bool withinResourceRange;

    public TileObject(Grid<TileObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        Vector2Int pos = new Vector2Int(x - grid.CentreX, y - grid.CentreY);
        withinResourceRange = pos.magnitude <= (MapGenerator.MapRadius + 2);
        SetBiome();
    }

    /// <summary>
    /// Sets the object inside a tile and automatically creates the instance of that object in the world
    /// </summary>
    /// <typeparam name="TWorldObject"></typeparam>
    /// <param name="newWorldObject"></param>
    /// <returns></returns>
    public TWorldObject CreateNewWorldObject<TWorldObject>(TWorldObject newWorldObject) where TWorldObject : WorldObject
    {
        if (WorldObject != null)
        {
            GameObject.Destroy(WorldObject.gameObject);
            _worldObject = null;
        }


        TWorldObject worldObjectInstance = null;
        if (newWorldObject != null)
        {
            worldObjectInstance = GameObject.Instantiate(newWorldObject, grid.GetGridCentreWorldPosition(x, y), Quaternion.identity);
            _worldObject = worldObjectInstance;
        }
        return worldObjectInstance;
    }

    public TWorldObject SetWorldObject<TWorldObject>(TWorldObject newWorldObject) where TWorldObject : WorldObject
    {
        if (newWorldObject == null)
        {
            _worldObject = null;
            return null;
        }

        if (WorldObject != null)
        {
            GameObject.Destroy(WorldObject.gameObject);
            _worldObject = null;
        }

        TWorldObject worldObjectInstance = null;
        if (newWorldObject != null)
        {
            worldObjectInstance = newWorldObject;
            worldObjectInstance.transform.position = grid.GetGridCentreWorldPosition(x, y);
            _worldObject = worldObjectInstance;
        }
        return worldObjectInstance;
    }

    /// <summary>
    /// Sets the object inside a tile to a world object without creating the object
    /// </summary>
    /// <typeparam name="TWorldObject"></typeparam>
    /// <param name="newWorldObject"></param>
    /// <returns></returns>
    public TWorldObject SetWorldObjectDirty<TWorldObject>(TWorldObject newWorldObject) where TWorldObject : WorldObject
    {
        _worldObject = newWorldObject;
        return newWorldObject;
    }

    void SetBiome()
    {
        float distanceFromCentre = (new Vector2(x, y) - new Vector2(grid.CentreX, grid.CentreY)).magnitude - 0.5f;
        if (distanceFromCentre > MapGenerator.MapRadius)
        {
            biome = MapGenerator.Biomes[MapGenerator.Biomes.Length - 1];
            return;
        }
        else if (distanceFromCentre < MapGenerator.HomeSize)
        {
            biome = MapGenerator.Biomes[0];
            return;
        }
        else if (distanceFromCentre < MapGenerator.HomeSize + MapGenerator.ResourceGenerationBuffer)
        {
            biome = MapGenerator.Biomes[1];
            return;
        }
        biome = MapGenerator.Biomes[MapGenerator.BiomeNoise.GetValue(grid.GetGridCentreWorldPosition(x, y))];
    }

    public void SetBiomeManual(int biome)
    {
        this.biome = MapGenerator.Biomes[biome];
        GetChunkXY(out int chunkX, out int chunkY);
        MapGenerator.UpdateChunk(chunkX, chunkY);
    }

    public GameObject GetChunk()
    {
        return MapGenerator.ChunkMeshs[x / grid.ChunkSize, y / grid.ChunkSize];
    }

    void GetChunkXY(out int x, out int y)
    {
        x = Mathf.FloorToInt((float)this.x / (float)grid.ChunkSize);
        y = Mathf.FloorToInt((float)this.y / (float)grid.ChunkSize);
    }

    public void RandomizeResource(Resource prefab, Transform parent)
    {
        if (biome.Name == "Home" || biome.Name == "ResourceGenerationBuffer"|| !withinResourceRange)
            return;

        ResourceSO resourceChosen = biome.ChooseResource();

        if (resourceChosen != null)
        {
            CreateNewWorldObject(prefab);

            ((Resource)WorldObject).SetResource(resourceChosen, this);
            WorldObject.transform.parent = parent;
        }
    }

    public void GetBiomeTexture(out Vector2 UV00, out Vector2 UV11)
    {
        float pixelHeight = 1f / MapGenerator.BiomeMaterial.mainTexture.height;
        float pixelWidth = 1f / MapGenerator.BiomeMaterial.mainTexture.width;

        float offsetWidth = pixelWidth * 1;
        float offsetHeight = pixelHeight * 1;

        int numOfTextuersWidth = MapGenerator.BiomeMaterial.mainTexture.width / 41;
        float raitoWidth = 1f / numOfTextuersWidth;
        int numOfTextuersHeight = MapGenerator.BiomeMaterial.mainTexture.height / 41;
        float raitoHeight = 1f / numOfTextuersHeight;
 
        int randomTextureIndexX = Random.Range(0, biome.TextureIndexX + 1);
        UV00 = new Vector2(raitoWidth * randomTextureIndexX, raitoHeight * biome.TextureIndexY);
        UV11 = new Vector2((raitoWidth * (randomTextureIndexX + 1)) - offsetWidth, (raitoHeight  * (biome.TextureIndexY + 1))-offsetHeight) ;
    }

    public void OnClick()
    {
        
    }

    public override string ToString()
    {
        return WorldObject == null ? "Null" : WorldObject.name.ToString();
    }
}