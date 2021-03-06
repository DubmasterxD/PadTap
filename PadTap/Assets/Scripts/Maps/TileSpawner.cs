﻿using NavyTap.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavyTap.Maps
{
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] Tile tilePrefab = null;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] float showTilesTimeInterval = 0.1f;
        const float originalTileSize = 2.56f;
        public float tileSize { get; private set; } = 0;
        const float mapSize = 10.24f;

        public List<Tile> tiles { get; private set; } = null;

        GameManager game = null;

        private void Awake()
        {
            game = FindObjectOfType<GameManager>();
            if (game == null)
            {
                Debug.LogError(Logger.NoComponentFound(typeof(GameManager)));
            }
        }

        private void OnEnable()
        {
            game.onPrepareSong += PrepareTiles;
        }

        private void OnDisable()
        {
            game.onPrepareSong -= PrepareTiles;
        }

        private void PrepareTiles(Map map)
        {
            StartCoroutine(PrepareTilesCoroutine(map));
        }

        private IEnumerator PrepareTilesCoroutine(Map map)
        {
            if (map != null)
            {
                yield return StartCoroutine(ShowTiles(map.tilesRows, map.tilesColumns));
                yield return new WaitForSeconds(1);
                SetPerfectScores(map.GetPerfectScore(), map.GetPerfectScoreAcceptableDifference());
                yield return new WaitForSeconds(1);
                game.StartSong();
            }
            else
            {
                Debug.LogError(typeof(Map) + " received is null");
            }
        }

        public IEnumerator ShowTiles(int rows, int columns)
        {
            CreateList(rows * columns);
            if (rows > 0 && columns > 0)
            {
                tileSize = mapSize / Mathf.Max(rows, columns);
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        int index = column + row * columns;
                        tiles[index].tileIndex = index;
                        tiles[index].gameObject.SetActive(true);
                        tiles[index].transform.localScale = new Vector3(tileSize / originalTileSize, tileSize / originalTileSize, tileSize / originalTileSize);
                        float posX = -mapSize / 2 + tileSize * column;
                        posX += mapSize / 2 * (1 - Mathf.Clamp01(columns / (float)rows));
                        float posY = mapSize / 2 - tileSize * row;
                        posY -= mapSize / 2 * (1 - Mathf.Clamp01(rows / (float)columns));
                        tiles[index].transform.localPosition = new Vector3(posX, posY);
                        yield return new WaitForSeconds(showTilesTimeInterval);
                    }
                }
            }
            else
            {
                Debug.LogError("Wrong number of rows and/or columns received");
            }
        }

        private void CreateList(int size)
        {
            if (tiles == null)
            {
                tiles = new List<Tile>();
            }
            if (tiles.Count < size)
            {
                if (tilePrefab != null)
                {
                    for (int i = tiles.Count; i < size; i++)
                    {
                        tiles.Add(Instantiate(tilePrefab, spawnPoint));
                    }
                }
                else
                {
                    Debug.LogError(Logger.NotAssigned(typeof(Tile), GetType(), name));
                }
            }
            foreach (Tile tile in tiles)
            {
                tile.gameObject.SetActive(false);
            }
            //for (int i = 0; i < size; i++)
            //{
            //    tiles[i].gameObject.SetActive(true);
            //}
        }

        private void SetPerfectScores(float perferctScore, float perfectScoreDifference)
        {
            if (tiles != null)
            {
                foreach (Tile tile in tiles)
                {
                    tile.SetPerfectScore(perferctScore, perfectScoreDifference);
                }
            }
        }
    }
}