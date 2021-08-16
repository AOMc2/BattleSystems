﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossSceneManagement : MonoBehaviour
{
    [HideInInspector]public Database database;
    [HideInInspector]public PointHolder pointHolder;
    private static int total = 0;
    [SerializeField] private string previousSceneName;
    [HideInInspector]public int previousBattleSceneLevel;
    [SerializeField]private bool forcedThrowDice = false;

    private void Awake()
    {
        if (total == 0)
        {
            total++;
            DontDestroyOnLoad(gameObject);

            database = transform.GetChild(0).GetComponent<Database>();
            database.SetUp();
            pointHolder = transform.GetChild(1).GetComponent<PointHolder>();
            pointHolder.SetUp();
        }
        else
        {
            Destroy(gameObject);
        }


    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) // On Level Finished Loading Second
    {

        SetObjectActivation(scene.name);
    }

    public void LoadScene(string sceneName) // Load Scene First
    {
        previousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void SetObjectActivation(string sceneName)
    {
        switch (sceneName)
        {
            case "BigMap":
                pointHolder.gameObject.SetActive(true);
                if (previousSceneName == "BattleScene")
                {
                    if (database.allyDetails.Count == 0)
                    {
                        pointHolder.avaliableLevel.Clear();
                        pointHolder.avaliableLevel.Add(0);
                        pointHolder.beatedLevel.Clear();
                    }
                    else
                    {
                        forcedThrowDice = true;
                        pointHolder.ExpandLevel();
                    }
                }
                pointHolder.Initialize();
                break;
            case "BattleScene":
                database.Initialize();
                pointHolder.gameObject.SetActive(false);
                break;
            case "Shop":
                pointHolder.gameObject.SetActive(false);
                if (previousSceneName == "BattleScene")
                {
                    if (database.allyDetails.Count == 0)
                    {
                        pointHolder.avaliableLevel.Clear();
                        pointHolder.avaliableLevel.Add(0);
                        pointHolder.beatedLevel.Clear();
                    }
                    else
                    {
                        pointHolder.ExpandLevel();
                    }
                }
                break;
        }

        if (forcedThrowDice == true)
        {
            forcedThrowDice = false;
            database.GetAllyNPC();
        }

        if (SceneManager.GetActiveScene().name == "Shop")
        {
            if (previousSceneName == "BattleScene")
            {
                forcedThrowDice = true;
            }
        }
    }
}
