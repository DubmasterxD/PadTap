﻿using PadTap.Core;
using System.Collections;
using UnityEngine;

namespace PadTap.Maps
{
    public class IndicatorSpawner : MonoBehaviour
    {
        [SerializeField] private Indicator indicatorPrefab = null;

        public Map map { get; private set; } = null;

        private Game game = null;
        private TileSpawner tileSpawner = null;
        private Coroutine spawning = null;

        private void Awake()
        {
            game = FindObjectOfType<Game>();
            tileSpawner = GetComponent<TileSpawner>();
        }

        private void OnEnable()
        {
            game.onGameStart += StartGame;
        }

        private void OnDisable()
        {
            game.onGameStart -= StartGame;
        }

        public void StartGame(Map map)
        {
            spawning = StartCoroutine(SpawnContinuously());
            StartCoroutine(PlaySong(map.song));
        }

        public void GameOver()
        {
            StopCoroutine(spawning);
            game.GameOver();
        }

        private IEnumerator SpawnContinuously()
        {
            int index = 0;
            while (index<map.points.Count)
            {
                float previousTime = 0;
                if (index != 0)
                { 
                    previousTime = map.points[index - 1].time;
                    yield return new WaitForSeconds(map.points[index].time - previousTime);
                }
                else
                {
                    if (GetFirstIndicatorSpawnTime() > 0)
                    {
                        yield return new WaitForSeconds(GetFirstIndicatorSpawnTime());
                    }
                }
                tileSpawner.tiles[map.points[index].tileIndex].Spawn(indicatorPrefab, map.indicatorLifespan);
                index++;
            }
        }

        private IEnumerator PlaySong(AudioClip song)
        {
            if (GetFirstIndicatorSpawnTime() < 0)
            {
                yield return new WaitForSeconds(-GetFirstIndicatorSpawnTime());
            }
            FindObjectOfType<Audio>().Play(song);
            yield return new WaitForSeconds(map.song.length);
            GameOver();
        }

        private float GetFirstIndicatorSpawnTime()
        {
            return map.points[0].time - map.GetPerfectScore() * map.indicatorLifespan;
        }
    }
}