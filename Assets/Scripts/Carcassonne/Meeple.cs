using Carcassonne;
    public class Meeple
    {
        // Start is called before the first frame update
        public Point.Direction direction;
        public NewTile.Geography geography;

        public int playerId;

        public bool free;

        public int x = 0, z = 0;


        public void assignAttributes(int x, int z, Point.Direction direction, NewTile.Geography geography)
        {
            this.direction = direction;
            this.geography = geography;

            this.x = x;
            this.z = z;
            free = false;
        }
    }

