using Carcassonne;
using UnityEngine;

namespace Assets.Scripts.Carcassonne.AI
{
    public class AIWrapper : InterfaceAIWrapper
    {
        public GameController controller;
        public GameState state;
        public Player player;

        public AIWrapper(Player player)
        {
            this.player = player;
        }

        public bool IsAITurn()
        {
            return player.id == controller.currentPlayer.id;
        }

        public int GetBoardSize()
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

        public void PlaceMeeple(float x, float z)
        {
            
            controller.ConfirmPlacement();

        }

        public void FreeCurrentMeeple()
        {
            controller.meepleController.FreeMeeple(state.meeples.Current, controller);
        }

        public void Reset()
        {
            controller.startGame = true;
        }

    }
}