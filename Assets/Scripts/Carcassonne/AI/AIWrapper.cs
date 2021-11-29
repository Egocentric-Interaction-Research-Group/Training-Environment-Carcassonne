using Carcassonne;
using UnityEngine;
using static Carcassonne.Point;

namespace Assets.Scripts.Carcassonne.AI
{
    public class AIWrapper : InterfaceAIWrapper
    {
        public GameController controller;
        public GameState state;
        public Player player;
        public int totalTiles;

        public bool IsAITurn()
        {
            return player.id == controller.currentPlayer.id;
        }

        public int GetMaxBoardSize()
        {
            return state.tiles.Played.GetLength(0);
        }

        public void PickUpTile()
        {
            controller.PickupTile();
        }

        public int GetCurrentTileId()
        {
            return state.tiles.Current.id;
        }

        public Phase GetGamePhase()
        {
            return state.phase;
        }

        public int GetMeeplesLeft()
        {
            return player.AmountOfFreeMeeples();
        }

        public void EndTurn()
        {
            controller.EndTurn();
        }

        public void DrawMeeple()
        {
            controller.meepleController.DrawMeeple();
        }

        public void RotateTile()
        {
            controller.pcRotate = true;
            controller.RotateTile();
        }

        public void PlaceTile(int x, int z)
        {
            controller.iTileAimX = x;
            controller.iTileAimZ = z;
            controller.ConfirmPlacement();
        }

        public void PlaceMeeple(Direction meepleDirection)
        {
            //if-statement below is how this is done in real code, there are probably easier solutions in this project.
            /*
            float meepleX = 0;
            float meepleZ = 0;
            if (meepleDirection == Direction.NORTH || meepleDirection == Direction.SOUTH || meepleDirection == Direction.CENTER)
            {
                meepleX = 0.000f;
            }
            else if (meepleDirection == Direction.EAST)
            {
                meepleX = 0.011f;
            }
            else if (meepleDirection == Direction.WEST)
            {
                meepleX = -0.011f;
            }

            if (meepleDirection == Direction.WEST || meepleDirection == Direction.EAST || meepleDirection == Direction.CENTER)
            {
                meepleZ = 0.000f;
            }
            else if (meepleDirection == Direction.NORTH)
            {
                meepleZ = 0.011f;
            }
            else if (meepleDirection == Direction.SOUTH)
            {
                meepleZ = -0.011f;
            }*/

            //Needs some form of placement of meeple based on direction and/or coordinates (like the code above)
            controller.ConfirmPlacement();

        }

        public void FreeCurrentMeeple()
        {
            controller.meepleController.FreeMeeple(state.meeples.Current, controller);
        }

        public void Reset()
        {
            controller.state.phase = Phase.GameOver;
        }

        public int GetMaxMeeples()
        {
            return player.meeples.Count;
        }

        public int GetMaxTileId()
        {
            return 33; //I have no clue how to get this in a more error safe manner at the moment.
        }

        public float[,] GetPlacedTiles()
        {
            return state.tiles.PlayedId;
        }

        public Tile[,] GetTiles()
        {
            return state.tiles.Played;
        }
        
        public int GetNumberOfPlacedTiles()
        {
            return totalTiles - state.tiles.Remaining.Count;
        }

        public int GetTotalTiles() {
            return totalTiles;
        }
    }
}