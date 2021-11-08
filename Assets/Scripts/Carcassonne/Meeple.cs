
using UnityEngine;

namespace Carcassonne
{
    public class Meeple : MonoBehaviour
    {
        // Start is called before the first frame update
        public Point.Direction direction;
        public Tile.Geography geography;

        public int playerId;

        public bool free;

        public int x, z;
        private void Start()
        {
           
            x = 0;
            z = 0;
            // id = 1;
        }

        public void assignAttributes(int x, int z, Point.Direction direction, Tile.Geography geography)
        {
            this.direction = direction;
            this.geography = geography;

            this.x = x;
            this.z = z;
        }

    }
}