﻿using PadTap.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PadTap.Maps
{
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab = null;
        private float originalTileSize = 2.5f;
        private float tileSize = 0;
        private float mapSize = 10;

        public List<Tile> tiles { get; private set; } = null;

        private Game game = null;

        private void Awake()
        {
            game = FindObjectOfType<Game>();
        }

        private void OnEnable()
        {
            game.onGameStart += StartGame;
        }

        private void OnDisable()
        {
            game.onGameStart -= StartGame;
        }

        private void StartGame(Map map)
        {
            ShowTiles(map.tilesRows, map.tilesColumns);
            SetThresholds(map.threshold);
        }

        public void ShowTiles(int rows, int columns)
        {
            CreateList(rows * columns);
            tileSize = mapSize / Mathf.Max(rows, columns);
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    int index = column + row * columns;
                    tiles[index].gameObject.SetActive(true);
                    tiles[index].transform.localScale = new Vector3(tileSize / originalTileSize, tileSize / originalTileSize, tileSize / originalTileSize);
                    float posX = -mapSize / 2 + tileSize * column;
                    posX += mapSize / 2 * (1 - Mathf.Clamp01(columns / (float)rows));
                    float posY = mapSize / 2 - tileSize * row;
                    posY -= mapSize / 2 * (1 - Mathf.Clamp01(rows / (float)columns));
                    tiles[index].transform.position = new Vector3(posX, posY);
                }
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
                for (int i = tiles.Count; i < size; i++)
                {
                    tiles.Add(Instantiate(tilePrefab, transform));
                }
            }
            foreach (Tile tile in tiles)
            {
                tile.gameObject.SetActive(false);
            }
            for (int i = 0; i < size; i++)
            {
                tiles[i].gameObject.SetActive(true);
            }
        }

        private void SetThresholds(float threshold)
        {
            foreach (Tile tile in tiles)
            {
                tile.SetThreshold(threshold);
            }
        }
    }
}