using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerationController : MonoBehaviour
{
    public static LevelGenerationController Current;
    
    public GameObject LevelGrid;

    public GameObject[] SectionPrefabs;

    private void Start()
    {
        Current = this;
    }

    private void Update()
    {
        if()
    }

    void LoadNextSection()
    {
        Instantiate(GetRandomSection(), LevelGrid.transform);
    }

    GameObject GetRandomSection()
    {
        int x = SectionPrefabs.Length -1;
        int rand = Random.Range(0, x);
        return SectionPrefabs[rand];
    }
}