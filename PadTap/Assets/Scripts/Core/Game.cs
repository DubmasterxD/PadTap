﻿using System.Collections;
using UnityEngine;

namespace PadTap.Core
{
    public class Game : MonoBehaviour
    {
        public delegate void OnGameStart(Map map);
        public event OnGameStart onGameStart;

        private void Awake()
        {
            if (FindObjectsOfType<Game>().Length == 1)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartGame(Map map, float timeToStart)
        {
            StartCoroutine(StartGameAfter(map, timeToStart));
        }

        public IEnumerator StartGameAfter(Map map, float timeToStart)
        {
            yield return new WaitForSeconds(timeToStart);
            onGameStart(map);
        }

        public void GameOver()
        {
            Scene scene = FindObjectOfType<Scene>();
            try
            {
                scene.LoadMenu();
            }
            catch (System.Exception e)
            {
                Debug.LogError("No object with " + typeof(Scene) + " component found!\n" + e);
            }
        }
    }
}