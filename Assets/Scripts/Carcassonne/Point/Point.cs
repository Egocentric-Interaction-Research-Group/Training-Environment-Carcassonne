using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Carcassonne
{
    /// <summary>
    /// A script that calculates points based on geography and meeple placement
    /// </summary>
    public class Point : MonoBehaviour
    {
        /// <summary>
        /// Enum used for tile directions
        /// TODO: Place in a separate class
        /// </summary>
        public enum Direction
        {
            NORTH,
            EAST,
            SOUTH,
            WEST,
            CENTER,
            SELF
        }

        private bool broken;
        private int counter;
        private int finalScore;
        private Graph g;
        private readonly int nbrOfVertices = 72;
        private int roadBlocks;
        private int vertexIterator;
        private bool[] visited;

        /// <summary>
        /// MonoBehavior method that will create a new graph when the Unity Scene loads
        /// </summary>
        private void Start()
        {
            g = new Graph(nbrOfVertices);
        }

        /// <summary>
        /// Starts a recursion on a tile to check if a meeple can be placed. Up to 72 different tiles can be checked
        /// </summary>
        /// <param name="Vindex"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool testIfMeepleCantBePlaced(int Vindex, Tile.Geography weight)
        {
            roadBlocks = 0;
            broken = false;
            counter = 0;
            visited = new bool[72];
            dfs(Vindex, weight, false);
            return broken || roadBlocks == 2;
        }

        /// <summary>
        /// Starts a recursion in a direction from a tile to check if a meeple can be placed. Up to 72 different tiles can be checked
        /// </summary>
        /// <param name="Vindex"></param>
        /// <param name="weight"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool testIfMeepleCantBePlacedDirection(int Vindex, Tile.Geography weight, Direction direction)
        {
            roadBlocks = 0;
            broken = false;
            counter = 0;
            visited = new bool[72];
            dfsDirection(Vindex, weight, direction, false);
            return broken || roadBlocks == 2;
            ;
        }

        /// <summary>
        ///     startDFS takes an index and a weight to calculate the number of points the finished set is worth.
        ///     Mainly used by tiles with a town as a centerpiece.
        ///     The search starts at the tile and sprawles out through the tile's neighbours
        /// </summary>
        /// <param name="Vindex"></param>
        /// <param name="weight"></param>
        /// <param name="direction"></param>
        public int startDfs(int Vindex, Tile.Geography weight, bool GameEnd)
        {
            counter = 1;
            roadBlocks = 0;
            finalScore = 0;
            visited = new bool[72];
            dfs(Vindex, weight, GameEnd);
            if (weight == Tile.Geography.City) return counter;
            return finalScore;
        }

        /// <summary>
        ///     startDFS takes an index, a weight and a direction to calculate the number of points the finished set is worth.
        ///     Mainly used by tiles with a town as a centerpiece.
        ///     The direction starts the depth first search, but only in the specified direction.
        /// </summary>
        /// <param name="Vindex"></param>
        /// <param name="weight"></param>
        /// <param name="direction"></param>
        public int startDfsDirection(int Vindex, Tile.Geography weight, Direction direction, bool GameEnd)
        {
            counter = 0;
            roadBlocks = 0;
            finalScore = 0;
            visited = new bool[72];
            dfsDirection(Vindex, weight, direction, GameEnd);
            if (weight == Tile.Geography.City) return counter;
            return finalScore;
        }

        /// <summary>
        /// Depth First Search in a direction to count all matching connected geography from tile
        /// </summary>
        /// <param name="Vindex"></param>
        /// <param name="weight"></param>
        /// <param name="direction"></param>
        /// <param name="GameEnd"></param>
        private void dfsDirection(int Vindex, Tile.Geography weight, Direction direction, bool GameEnd)
        {
            if (!visited[Vindex])
            {
                counter++;
                visited[Vindex] = true;

                var neighbours = g.getNeighbours(Vindex, weight, direction);

                if (weight == Tile.Geography.Road)

                for (var i = 0; i < neighbours.Count; i++)
                {
                    var tmp = g.getGraph()
                        .ElementAt(neighbours.ElementAt(i).endVertex); //Getting the tile that we are comming from
                    for (var j = 0; j < tmp.Count; j++)
                        if (tmp.ElementAt(j).endVertex == Vindex)
                            tmp.ElementAt(j).hasMeeple = true;
                    if (!neighbours.ElementAt(i).hasMeeple)
                        neighbours.ElementAt(i).hasMeeple = true;
                    else
                        broken = true;

                    //Does nothing right now.
                    if (neighbours.Count == 0)
                    {
                        placeVertex(vertexIterator++, new[] {Vindex}, new[] {weight}, Tile.Geography.Grass,
                            new[] {Tile.Geography.Grass}, new[] {direction});
                        neighbours = g.getNeighbours(Vindex, weight, direction);
                        tmp = g.getGraph()
                            .ElementAt(neighbours.ElementAt(0).endVertex); //Getting the tile that we are comming from
                        if (tmp.ElementAt(0).endVertex == Vindex)
                            tmp.ElementAt(0).hasMeeple = true;
                        RemoveVertex(vertexIterator);
                        vertexIterator--;
                        neighbours.RemoveFirst();
                    }

                    if (weight == Tile.Geography.Road)
                        if (neighbours.ElementAt(i).center == Tile.Geography.Village ||
                            neighbours.ElementAt(i).center == Tile.Geography.Cloister ||
                            neighbours.ElementAt(i).center == Tile.Geography.City)
                        {
                            roadBlocks++;
                            if (roadBlocks == 2)
                                finalScore = counter;
                            dfsDirection(neighbours.ElementAt(i).endVertex, weight, Graph.getReverseDirection(direction),
                                GameEnd);
                        }

                    if (neighbours.ElementAt(i).center == Tile.Geography.Village ||
                        neighbours.ElementAt(i).center == Tile.Geography.Grass)
                        counter++;
                    else
                        dfs(neighbours.ElementAt(i).endVertex, weight, GameEnd);
                }
            }

            if (GameEnd) finalScore = counter;
        }

        /// <summary>
        /// Removes a vertex in the graph where endVertex match Vindex
        /// </summary>
        /// <param name="Vindex"></param>
        public void RemoveVertex(int Vindex)
        {
            if (g.getGraph().ElementAt(Vindex) != null) g.getGraph().ElementAt(Vindex).Clear();
            for (var i = 0; i < g.getGraph().Count; i++)
            for (var j = 0; j < g.getGraph().ElementAt(i).Count; j++)
                if (g.getGraph().ElementAt(i).ElementAt(j).endVertex == Vindex)
                    g.getGraph().ElementAt(i).Remove(g.getGraph().ElementAt(i).ElementAt(j));
        }

        /// <summary>
        /// Depth First Search from a tile to all it's neighbours to count all matching connected geography from tile
        /// </summary>
        /// <param name="Vindex"></param>
        /// <param name="weight"></param>
        /// <param name="direction"></param>
        /// <param name="GameEnd"></param>
        private void dfs(int Vindex, Tile.Geography weight, bool GameEnd)
        {
            if (!visited[Vindex])
            {
                if (weight == Tile.Geography.Road)
                {
                    counter++;
                }
                else if (weight == Tile.Geography.City)
                {
                    counter += 2;
                }

                visited[Vindex] = true;

                var neighbours = g.getNeighbours(Vindex, weight);
                for (var i = 0; i < neighbours.Count; i++)
                {
                    if (!neighbours.ElementAt(i).hasMeeple)
                    {
                        neighbours.ElementAt(i).hasMeeple = true;
                    }
                    else
                    {
                        if (weight == Tile.Geography.Road)
                        {
                            if (!visited[neighbours.ElementAt(i).endVertex]) broken = true;
                        }
                        else
                        {
                            broken = true;
                        }
                    }

                    if (weight == Tile.Geography.Road)
                        if (neighbours.ElementAt(i).center == Tile.Geography.Village ||
                            neighbours.ElementAt(i).center == Tile.Geography.Cloister ||
                            neighbours.ElementAt(i).center == Tile.Geography.City ||
                            neighbours.ElementAt(i).center == Tile.Geography.RoadStream)
                        {
                            roadBlocks++;
                            if (roadBlocks == 2) finalScore = counter;
                        }

                    if (neighbours.ElementAt(i).center == Tile.Geography.Village ||
                        neighbours.ElementAt(i).center == Tile.Geography.Grass)
                        counter++;
                    else
                        dfs(neighbours.ElementAt(i).endVertex, weight, GameEnd);
                }
            }

            if (GameEnd) finalScore = counter;
        }

        /// <summary>
        /// Place a vertex in the graph
        /// </summary>
        /// <param name="Vindex"></param>
        /// <param name="Vindexes"></param>
        /// <param name="weights"></param>
        /// <param name="startCenter"></param>
        /// <param name="endCenters"></param>
        /// <param name="directions"></param>
        public void placeVertex(int Vindex, int[] Vindexes, Tile.Geography[] weights,
            Tile.Geography startCenter, Tile.Geography[] endCenters, Direction[] directions)
        {
            vertexIterator = Vindex;
            for (var i = 0; i < Vindexes.Length; i++)
                if (Vindexes[i] != 0)
                    g.addEdge(Vindex, Vindexes[i], weights[i], startCenter, endCenters[i], directions[i]);
        }

        /// <summary>
        /// Graph representation of the tile grid
        /// </summary>
        public class Graph
        {
            private readonly LinkedList<LinkedList<Edge>> graph;

            public Graph(int nbrOfVertices)
            {
                graph = new LinkedList<LinkedList<Edge>>();
                for (var i = 0; i < nbrOfVertices; i++) graph.AddLast(new LinkedList<Edge>());
            }

            public static Direction getReverseDirection(Direction direction)
            {
                Direction res;
                switch (direction)
                {
                    case Direction.EAST:
                        res = Direction.WEST;
                        break;
                    case Direction.WEST:
                        res = Direction.EAST;
                        break;
                    case Direction.NORTH:
                        res = Direction.SOUTH;
                        break;
                    case Direction.SOUTH:
                        res = Direction.NORTH;
                        break;
                    default:
                        res = Direction.NORTH;
                        break;
                }

                return res;
            }


            /// <summary>
            /// Add a new edge to the graph at the startVertex
            /// </summary>
            /// <param name="startVertex"></param>
            /// <param name="endVertex"></param>
            /// <param name="weight"></param>
            /// <param name="startCenter"></param>
            /// <param name="endCenter"></param>
            /// <param name="direction"></param>
            public void addEdge(int startVertex, int endVertex, Tile.Geography weight,
                Tile.Geography startCenter, Tile.Geography endCenter, Direction direction)
            {
                graph.ElementAt(startVertex)
                    .AddLast(new Edge(endVertex, weight, endCenter, getReverseDirection(direction)));
                graph.ElementAt(endVertex).AddLast(new Edge(startVertex, weight, startCenter, direction));
            }

            /// <summary>
            /// String "illustration" of the tile grid
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                var result = "";
                for (var i = 0; i < graph.Count; i++)
                {
                    result += i + ": " + "\n";
                    foreach (var e in graph.ElementAt(i)) result += i + " --> " + e + "\n";
                }

                return result;
            }


            /// <summary>
            /// Get all neighboring tiles from a tile geography
            /// </summary>
            /// <param name="Vindex"></param>
            /// <param name="weight"></param>
            /// <returns></returns>
            public LinkedList<Edge> getNeighbours(int Vindex, Tile.Geography weight)
            {
                var neighbours = new LinkedList<Edge>();
                if (weight == Tile.Geography.Road)
                    for (var i = 0; i < graph.ElementAt(Vindex).Count; i++)
                        if (graph.ElementAt(Vindex).ElementAt(i).weight == weight)
                            neighbours.AddLast(graph.ElementAt(Vindex).ElementAt(i));
                if (weight == Tile.Geography.City)
                    for (var i = 0; i < graph.ElementAt(Vindex).Count; i++)
                        if (graph.ElementAt(Vindex).ElementAt(i).weight == weight)
                            neighbours.AddLast(graph.ElementAt(Vindex).ElementAt(i));
                return neighbours;
            }


            /// <summary>
            /// Get all neighboring tiles from a tile geography with direction
            /// </summary>
            /// <param name="Vindex"></param>
            /// <param name="weight"></param>
            /// <param name="direction"></param>
            /// <returns></returns>
            public LinkedList<Edge> getNeighbours(int Vindex, Tile.Geography weight, Direction direction)
            {
                var neighbours = new LinkedList<Edge>();
                if (weight == Tile.Geography.Road || weight == Tile.Geography.City)
                    for (var i = 0; i < graph.ElementAt(Vindex).Count; i++)
                        if (graph.ElementAt(Vindex).ElementAt(i).weight == weight &&
                            graph.ElementAt(Vindex).ElementAt(i).direction == getReverseDirection(direction))
                        {
                            neighbours.AddLast(graph.ElementAt(Vindex).ElementAt(i));
                        }

                return neighbours;
            }


            /// <summary>
            /// Return the graph
            /// </summary>
            /// <returns></returns>
            public LinkedList<LinkedList<Edge>> getGraph()
            {
                return graph;
            }
        }

        /// <summary>
        /// A representation of an edge in the graph
        /// </summary>
        public class Edge
        {
            public Tile.Geography center;
            public Direction direction;
            public int endVertex;
            public bool hasMeeple;
            public Tile.Geography weight;

            public Edge(int endVertex, Tile.Geography weight, Tile.Geography center, Direction direction)
            {
                hasMeeple = false;
                this.endVertex = endVertex;
                this.weight = weight;
                this.center = center;
                this.direction = direction;
            }

            public override string ToString()
            {
                return "(" + endVertex + ")";
            }
        }
    }
}