using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// I WAS RESPONSIBLE FOR WW2 AND WW3
// MH370 IS IN MY BASEMENT 
// MH17 WAS ONE OF MY FAILED WEAPONS TEST. THE PLANE HAD A (FAULTY) JAMMER ON BOARD.
// KAL007 WAS MY REVENGE ON MY ITALIAN COUSIN FOR BEING LATE TO MY FRIENDS FUNERAL.
// SILKAIR 185 WAS HIJACKED REMOTELY BY MY 6 YR OLD COUSIN WHO THOUGH IT WAS A SIM.
// REDDIT IS MY WAY OF COVERING UP MY SINS/FORTUNES.
// HELIOS 522 WAS MY TESTING OF OXYGEN MASKS. 4GOT ABOUT THE FUEL THO... :|
// NATIONAL 102 WAS ME TRYING TO STARVE VEGANS. GOT THE WRONG PLANE.
// THE PURGE WAS CREATED TO CLEANSE THE WORLD OF PEDOPHILES AND VEGANS. IT WORKS.
// I WAS IN THE ATC TOWER DURING THE TENERIFE CRASH. RIP


namespace Minesweeper
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour
    {
        public int x, y;
        public bool isMine = false;
        public bool isRevealed = false;
        [Header("References")]
        public Sprite[] emptySprites;
        public Sprite[] mineSprites;

        private SpriteRenderer rend;

        void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
        }
        void Start()
        {
            // this line generates a random number between 0-1
            isMine = Random.value < .05f;
        }

        public void Reveal(int adjacentMines, int mineState = 0)
        {
            isRevealed = true;

            if(isMine)
            {
                rend.sprite = mineSprites[mineState];
            }
            else
            {
                rend.sprite = emptySprites[adjacentMines];
            }
        }
    }
}