using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerationController : MonoBehaviour
{
    public static LevelGenerationController Current;
    public int DestroyDistance;

    public Vector2 SectionSize;

    public GameObject LevelGrid;
    
    public List<GameObject> SectionPrefabs;
    
    [SerializeField]
    private Dictionary<Vector2, GameObject> ChunksInPlay;

    private const string WEST_COLLIDER_TAG = "WestCollider";
    private const string EAST_COLLIDER_TAG = "EastCollider";
    private const string NORTH_COLLIDER_TAG = "NorthCollider";
    private const string SOUTH_COLLIDER_TAG = "SouthCollider";

    private void Awake()
    {
        Current = this;
        ChunksInPlay = new Dictionary<Vector2, GameObject>();
    }

    private void OnEnable()
    {
        LoadFirstChunk();
    }

    private void LoadFirstChunk()
    {
        GenerateChunk(new Vector2(0,0));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject parent = other.transform.parent.gameObject;
        //TODO: Refactor
        switch (other.tag)
        {
            case NORTH_COLLIDER_TAG:
                LoadNextChunk(parent, DirectionType.North);
                break;
            case EAST_COLLIDER_TAG:
                LoadNextChunk(parent, DirectionType.East);
                break;
            case SOUTH_COLLIDER_TAG:
                LoadNextChunk(parent, DirectionType.South);
                break;
            case WEST_COLLIDER_TAG:
                LoadNextChunk(parent, DirectionType.West);
                break;
        }
    }

    void LoadNextChunk(GameObject colliderParent, DirectionType direction)
    {
        Vector2 chunkLocation = GetNextChunkLocation(colliderParent, direction);
        
        if (ChunksInPlay.ContainsKey(chunkLocation))
        {
            if (ChunksInPlay[chunkLocation] == null)
            {
                GenerateChunk(chunkLocation);

            }
        }
        else
        {
            GenerateChunk(chunkLocation);
        }

        ChunkCleanup();
    }

    private void GenerateChunk(Vector2 chunkLocation)
    {
        GameObject newChunk = Instantiate(GetRandomChunkPrefab(), LevelGrid.transform);
        CacheChunk(chunkLocation, newChunk);
        SetChunkPosition(chunkLocation, newChunk);
    }


    private Vector2 GetNextChunkLocation(GameObject colliderParent, DirectionType direction)
    {
        KeyValuePair<Vector2, GameObject> currentChunkLocation = ChunksInPlay.Where(x => x.Value == colliderParent).FirstOrDefault();

        Vector2 chunkLocation = new Vector2();
        
        //TODO: Refactor
        switch (direction)
        {
            case DirectionType.North:
                chunkLocation = currentChunkLocation.Key + new Vector2(0, 1);
                break;
            case DirectionType.East:
                chunkLocation = currentChunkLocation.Key + new Vector2(1,0);
                break;
            case DirectionType.South:
                chunkLocation = currentChunkLocation.Key + new Vector2(0, -1);
                break;
            case DirectionType.West:
                chunkLocation = currentChunkLocation.Key + new Vector2(-1,0);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
        
        return chunkLocation;
    }

    private void SetChunkPosition(Vector2 gridLocation, GameObject newSection)
    {
        newSection.transform.position = new Vector3(gridLocation.x * SectionSize.x, gridLocation.y * SectionSize.y);
    }

    private void CacheChunk(Vector2 gridLocation, GameObject newSection)
    {
        if (!ChunksInPlay.ContainsKey(gridLocation))
        {
            ChunksInPlay.Add(gridLocation, newSection);
        }
        else
        {
            ChunksInPlay[gridLocation] = newSection;
        }
    }

    void ChunkCleanup()
    {
        foreach (KeyValuePair<Vector2, GameObject> section in ChunksInPlay)
        {
            if (section.Value != null)
            {
                if (Vector3.Distance(section.Value.transform.position, MovementController.Current.transform.position) >
                    DestroyDistance)
                {
                    Destroy(section.Value);
                }
            }
        }
    }

    GameObject GetRandomChunkPrefab()
    {
        int x = SectionPrefabs.Count -1;
        int rand = Random.Range(0, x);
        return SectionPrefabs[rand];
    }
}

enum DirectionType
{
    North = 1,
    East = 2,
    South = 3,
    West = 4
}