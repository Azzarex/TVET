using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Useful Hotkeys
 * - CTRl + K + D : Cleans code
 * - CTRL + M + O : Folds code
 * - CTRL + M + P : UnFolds code
 */

namespace Minesweeper
{
    public class Grid : MonoBehaviour
    {
        public GameObject tilePrefab;
        public int width = 10, height = 10;
        public float spacing = .155f;

        private Tile[,] tiles;
        private float y;

        Tile SpawnTile(Vector3 pos)
        {

            GameObject clone = Instantiate(tilePrefab);

            clone.transform.position = pos;
            Tile currentTile = clone.GetComponent<Tile>();

            return currentTile;

        }
        void GenerateTiles()
        {

            tiles = new Tile[width, height];

            for (int x = 0; x < width; x++)
            {
                Vector2 halfSize = new Vector2(width * 0.5f,
                            height * 0.5f);
                Vector2 pos = new Vector2(x - halfSize.x,
                    y - halfSize.y);

                pos *= spacing; // says how far apart the grid is
                // imgay
               


            }





        }
    }
}