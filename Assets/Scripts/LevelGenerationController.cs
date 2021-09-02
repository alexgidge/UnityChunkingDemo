using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        DirectionType direction = GetDirectionFromTag(other.tag);

        
        LoadNextChunk(parent, direction);
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
        Vector2 chunkLocation = new Vector2();
        KeyValuePair<Vector2, GameObject> currentChunkLocation =
            ChunksInPlay.Where(x => x.Value == colliderParent).FirstOrDefault();
        Vector2 movement = GetMovementByDirection(direction);
        chunkLocation = currentChunkLocation.Key + movement;

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
    
    private DirectionType GetDirectionFromTag(string otherTag)
    {
        switch (otherTag)
        {
            case NORTH_COLLIDER_TAG:
                return DirectionType.North;
            case EAST_COLLIDER_TAG:
                return DirectionType.East;
            case SOUTH_COLLIDER_TAG:
                return DirectionType.South;
            case WEST_COLLIDER_TAG:
                return DirectionType.West;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Vector2 GetMovementByDirection(DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.North:
                return new Vector2(0, 1);
            case DirectionType.East:
                return new Vector2(1,0);
            case DirectionType.South:
                return new Vector2(0, -1);
            case DirectionType.West:
                return new Vector2(-1,0);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

}

enum DirectionType
{
    North = 1,
    East = 2,
    South = 3,
    West = 4
}