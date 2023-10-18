
using System;
using System.Collections.Generic;

//this class stores the state of the game.
namespace Snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Direction { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Direction> directionChanges = new LinkedList<Direction>(); 
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        // first element = head and last element = tail

        private readonly Random random = new Random();  // used to figure out where the food should spawn

        public GameState(int rows, int column)
        {
            Rows = rows;
            Columns = column;
            Grid = new GridValue[rows, column];
            Direction = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int midRow = Rows / 2;

            for (int i = 1; i <= 3; i++)
            {
                Grid[midRow, i] = GridValue.Snake;
                snakePositions.AddFirst(new Position(midRow, i));
            }
        }

        public IEnumerable<Position> EmptyPositions()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Grid[i, j] == GridValue.Empty)
                    {
                        yield return new Position(i, j);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position position = empty[random.Next(empty.Count)];
            Grid[position.Row, position.Column] = GridValue.Food;
        }

        // helper methods

        public Position HeadPosition()  // used to add the eyes
        {
            return snakePositions.First.Value;   // as the head of the list is the head of the snake
        }

        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePosition()
        {
            return snakePositions;
        }

        private void AddHead(Position position)
        {
            // adds the given position at the front of the snakePosition list to make it the new head

            snakePositions.AddFirst(position);
            Grid[position.Row, position.Column] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if (directionChanges.Count == 0)
            {
                return Direction;
            }
            return directionChanges.Last.Value;
        }

        private bool CanChangeDirections(Direction newDirection)
        {
            if (directionChanges.Count == 2)
            {
                return false;
            }

            Direction lastDirection = GetLastDirection();
            return lastDirection != newDirection && newDirection != lastDirection.Opposite();

        }
        public void ChangeDirections(Direction direction)
        {   //here we do not directly change the direction because of 2 cases:
            // 1. There might be multiple keys placed in one single move
            // 2. Pressing keys in the opposite direction of the current direction should not be allowed

            if (CanChangeDirections(direction))
            {
                directionChanges.AddLast(direction);
            }

        }

        private bool OutsideGrid(Position position)
        {
            return position.Row < 0 || position.Column < 0 || position.Row >= Rows || position.Column >= Columns;
        }

        private GridValue WillHit(Position position)
        {
            if (OutsideGrid(position))
            {
                return GridValue.Outside;
            }
            if (position == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[position.Row, position.Column];
        }

        public void Move()
        {
            if (directionChanges.Count > 0)
            {
                Direction = directionChanges.First.Value;
                directionChanges.RemoveFirst();
            }

            Position newHeadPosition = HeadPosition().Translate(Direction);
            GridValue Hit = WillHit(newHeadPosition);

            if (Hit == GridValue.Outside || Hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (Hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPosition);
            }
            else if (Hit == GridValue.Food)
            {
                AddHead(newHeadPosition);
                Score++;
                AddFood();
            }
        }
    }
}
