using Carcassonne.State;
using Carcassonne;
using UnityEngine;

namespace Assets.Scripts.Carcassonne.AI
{
    public class AIWrapper : InterfaceAIWrapper
    {
        public GameController gc;
        public GameState gs; //Contains TileState, MeepleState, FeatureState, PlayerState and a GameLog.
        public Player player;

        public AIWrapper()
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
            gs = gc.gameState;
        }

        public bool IsAITurn()
        {
            return player.Id == gs.Players.Current.Id;
        }

        public int GetBoardSize()
        {
            return gs.Tiles.Played.GetLength(0);
        }

        public void PickUpTile()
        {
            gc.PickupTile();
        }

        public int GetCurrentTileId()
        {
            return gs.Tiles.Current.id;
        }

        public Phase GetGamePhase()
        {
            return gs.phase;
        }

        public int GetMeeplesLeft()
        {
            return player.AmountOfFreeMeeples();
        }

        public void EndTurn()
        {
            gc.EndTurn();
        }

        public void DrawMeeple()
        {
            gc.meepleController.DrawMeeple();
        }

        public void RotateTile()
        {
            gc.pcRotate = true;
            gc.RotateTile();
        }

        public void PlaceTile(int x, int z)
        {
            gc.iTileAimX = x;
            gc.iTileAimZ = z;
            gc.ConfirmPlacement();
        }

        public void PlaceMeeple(float x, float z)
        {
            
            gc.ConfirmPlacement();

        }

        public void FreeCurrentMeeple()
        {
            gc.meepleController.FreeMeeple(gs.Meeples.Current, gc);
        }

    }
}