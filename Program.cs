
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            bool whilePlaying = false;
            ConsoleKey input = ConsoleKey.Z;
            do
            {
                whilePlaying = false;
                Grid grid = new Grid();
                grid.PlayGame();
                Console.WriteLine("Would you like to play another round? Enter Y or N");
                while (!whilePlaying)
                {
                    input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.Y || input == ConsoleKey.N)
                        whilePlaying = true;
                }
            } while (input == ConsoleKey.Y);

            Console.Clear();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    class Ship
    {
        public enum ShipType { battleship, submarine, minesweeper, carrier, cruiser }

        public ShipType Type { get; set; }
        public List<Point> Coordinates { get; set; }
        public int shipSize { get; set; }
        public bool goodAim { get; set; }
        public bool IsSunk
        {
            get
            {
                bool attacked = false;
                foreach (Point point in Coordinates)
                {
                    if (point.Status == Point.PointStatus.Hit)
                        attacked = true;
                    else
                    {
                        attacked = false;
                        break;
                    }
                }
                if (attacked)
                {
                    if (!goodAim)
                    {
                        Console.WriteLine("\n The {0} is down! Good job!", this.Type);
                        Console.WriteLine("Press any key to continue . . .");
                        Console.ReadKey();
                        goodAim = true;
                    }
                }
                return attacked;
            }
        }

        public Ship(ShipType typeOfShip)
        {
            this.Coordinates = new List<Point>();
            this.Type = typeOfShip;

            switch (this.Type)
            {
                case ShipType.carrier:
                    this.shipSize = 5;
                    break;
                case ShipType.battleship:
                    this.shipSize = 4;
                    break;
                case ShipType.cruiser:
                    this.shipSize = 3;
                    break;
                case ShipType.submarine:
                    this.shipSize = 3;
                    break;
                case ShipType.minesweeper:
                    this.shipSize = 2;
                    break;
                default:
                    break;
            }

            goodAim = false;
        }
    }
    class Point
    {
        public enum PointStatus { Empty, Ship, Hit, Miss }

        public int X { get; set; }
        public int Y { get; set; }
        public PointStatus Status { get; set; }

        public Point(int x, int y, PointStatus z)
        {
            this.X = x;
            this.Y = y;
            this.Status = z;
        }
    }

    class Grid
    {
        public enum PlaceShipDirection { xCoordinate, yCoordinate }

        public Point[,] Ocean { get; set; }
        public List<Ship> shipTypes { get; set; }
        private bool allshipsSunk;
        public bool AllShipsSunk
        {
            get
            {
                bool allShipsSunk = false;
                foreach (Ship ship in shipTypes)
                {
                    if (ship.IsSunk)
                        allShipsSunk = true;
                    else
                    {
                        allShipsSunk = false;
                        break;
                    }
                }

                return allShipsSunk;
            }
        }
        public int Round { get; set; }

        public Grid()
        {
            this.Ocean = new Point[10, 10];

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                    this.Ocean[x, y] = new Point(x, y, Point.PointStatus.Empty);
            }

            this.shipTypes = new List<Ship>();
            this.shipTypes.Add(new Ship(Ship.ShipType.carrier));
            this.shipTypes.Add(new Ship(Ship.ShipType.battleship));
            this.shipTypes.Add(new Ship(Ship.ShipType.cruiser));
            this.shipTypes.Add(new Ship(Ship.ShipType.submarine));
            this.shipTypes.Add(new Ship(Ship.ShipType.minesweeper));

            Random rng = new Random();
            foreach (Ship ship in shipTypes)
            {
                bool validPlacement = false;
                PlaceShipDirection direction = (PlaceShipDirection)rng.Next(0, 2);
                int startX = 0;
                int startY = 0;
                int xToCheck = 0;
                int yToCheck = 0;
                int check = 0;
                while (!validPlacement)
                {
                    direction = (PlaceShipDirection)rng.Next(0, 2);
                    startX = rng.Next(0, 10);
                    startY = rng.Next(0, 10);
                    xToCheck = startX;
                    yToCheck = startY;

                    switch (direction)
                    {
                        case PlaceShipDirection.xCoordinate:
                            while (ship.shipSize > (9 - startX))
                            {
                                startX = rng.Next(0, 10);
                                xToCheck = startX;
                            }
                            break;
                        case PlaceShipDirection.yCoordinate:
                            while (ship.shipSize > (9 - startY))
                            {
                                startY = rng.Next(0, 10);
                                yToCheck = startY;
                            }
                            break;
                        default:
                            break;
                    }
                    while (Ocean[xToCheck, yToCheck].Status == Point.PointStatus.Empty && check < ship.shipSize)
                    {
                        switch (direction)
                        {
                            case PlaceShipDirection.xCoordinate:
                                xToCheck++;
                                check++;
                                break;
                            case PlaceShipDirection.yCoordinate:
                                yToCheck++;
                                check++;
                                break;
                            default:
                                break;
                        }
                    }
                    if (Ocean[xToCheck, yToCheck].Status == Point.PointStatus.Empty)
                    {
                        PlaceShip(ship, direction, startX, startY);
                        validPlacement = true;
                    }
                }
            }
        }


        void PlaceShip(Ship shipPlacement, PlaceShipDirection direction, int startX, int startY)
        {
            for (int i = 0; i < shipPlacement.shipSize; i++)
            {
                this.Ocean[startX, startY].Status = Point.PointStatus.Ship;
                shipPlacement.Coordinates.Add(this.Ocean[startX, startY]);

                switch (direction)
                {
                    case PlaceShipDirection.xCoordinate:
                        startX++;
                        break;
                    case PlaceShipDirection.yCoordinate:
                        startY++;
                        break;
                    default:
                        break;
                }
            }
        }

        void DrawOcean()
        {
            Console.WriteLine("   0  1  2  3  4  5  6  7  8  9");
            for (int y = 0; y < 10; y++)
            {
                Console.Write("{0} ", y);
                for (int x = 0; x < 10; x++)
                {
                    switch (Ocean[x, y].Status)
                    {
                        case Point.PointStatus.Empty:
                            Console.Write("[ ]");
                            break;
                        case Point.PointStatus.Ship:
                            Console.Write("[ ]");
                            break;
                        case Point.PointStatus.Hit:
                            Console.Write("[X]");
                            break;
                        case Point.PointStatus.Miss:
                            Console.Write("[O]");
                            break;
                        default:
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n Number of hits: {0}", Round);
            Console.ResetColor();
        }

        bool attackCoordinates(int x, int y)
        {
            int numberOfShipsDestroyed = shipTypes.Where(z => z.IsSunk).Count();
            if (Ocean[x, y].Status == Point.PointStatus.Ship)
            {
                Ocean[x, y].Status = Point.PointStatus.Hit;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n Good Aim! You hit!! :)");
                Console.ResetColor();
                Console.WriteLine("\n\nPress any key to continue . . .");
                Console.ReadKey();

            }
            else if (Ocean[x, y].Status == Point.PointStatus.Empty)
            {
                Ocean[x, y].Status = Point.PointStatus.Miss;
                Console.WriteLine("\nYou MISSED!! :(");
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\nYou've already attacked that coordinate!");
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey();
            }

            int numberOfShipsDestroyedNow = shipTypes.Where(z => z.IsSunk).Count();

            if (numberOfShipsDestroyedNow > numberOfShipsDestroyed)
                return true;
            else
                return false;
        }

        public void PlayGame()
        {
            int x = 0;
            int y = 0;

            while (!AllShipsSunk)
            {
                bool validInput = false;
                Console.Clear();
                DrawOcean();
                while (!validInput)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n Please enter x-coordinate: ");
                    Console.ResetColor();
                    bool xIsNumber = int.TryParse(Console.ReadLine(), out x);
                    if (!xIsNumber)
                    {
                        Console.WriteLine("invalid input!");
                        System.Threading.Thread.Sleep(900);
                        Console.Clear();
                        DrawOcean();

                    }
                    else if (x < 0 || x > 9)
                    {
                        Console.WriteLine("invalid input!");
                        System.Threading.Thread.Sleep(900);
                        Console.Clear();
                        DrawOcean();


                    }
                    else
                        validInput = true;
                }
                validInput = false;
                while (!validInput)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nPlease enter y-coordinate: ");
                    Console.ResetColor();
                    bool yIsNumber = int.TryParse(Console.ReadLine(), out y);

                    if (!yIsNumber)
                    {
                        Console.WriteLine("invalid input!");
                        System.Threading.Thread.Sleep(900);
                        Console.Clear();
                    }
                    else if (y < 0 || y > 9)
                    {
                        Console.WriteLine("invalid input!");
                        System.Threading.Thread.Sleep(900);
                        Console.Clear();
                    }
                    else
                        validInput = true;
                }

                attackCoordinates(x, y);
                Round++;
            }

            Console.Clear();

            DrawOcean();

            Console.WriteLine("\nVictory is yours! It took you {0} hits.", Round);
        }
    }
}