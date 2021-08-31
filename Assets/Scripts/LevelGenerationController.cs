using System;
using System.Collections.Generic;
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
    private List<GameObject> SectionsInPlay;

    private void Start()
    {
        Current = this;
    }

    private void Update()
    {
        if (MovementController.Current.transform.position.x > 10 && SectionsInPlay.Count < 3)
        {
            LoadNextSection();
        }
    }

    void LoadNextSection()
    {
        GameObject newSection = Instantiate(GetRandomSection(), LevelGrid.transform);
        SectionsInPlay.Add(newSection); 
        SectionCleanup();
    }

    void SectionCleanup()
    {
        foreach (GameObject section in SectionsInPlay)
        {
            if (section != null)
            {
                if (Vector3.Distance(section.transform.position, MovementController.Current.transform.position) >
                    DestroyDistance)
                {
                    SectionsInPlay.Remove(section);
                    Destroy(section);
                }
            }
        }
    }

    GameObject GetRandomSection()
    {
        int x = SectionPrefabs.Count -1;
        int rand = Random.Range(0, x);
        return SectionPrefabs[rand];
    }
}