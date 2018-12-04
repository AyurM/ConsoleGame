using System;

namespace ConsoleGame {
    public class Program {

        static Random rnd = new Random();
        static int width = 30, height = 15;

        //Координаты объектов на уровне
        static int px = 2, py = 2;
        static int bonusx = -1, bonusy = -1;             
        static int ex = rnd.Next(3, width), ey = rnd.Next(3, height);   //позиция противника выбирается случайным образом

        //Боевые параметры
        static int hp = 100, kills = 0;
        static int ehp = 50;
        static int pChanceToHit = 75, eChanceToHit = 50;
        static int pdmgMin = 8, pdmgMax = 12;
        static int edmgMin = 7, edmgMax = 10;
        static int playerDmg = 0, enemyDmg = 0;
        
        //Управление игрой
        static int turn = 0, combatTurn = 0, pick = 0;
        static bool turnMade = false;
        static bool inCombat = false;

        static void Main(string[] args) {
            Console.CursorVisible = false;
            ShowLevel();

            while (true) {
                ConsoleKeyInfo key = Console.ReadKey(true);
                HandleInput(key.Key);               

                if (turnMade) {
                    GenerateBonus();
                    EnemyTurn();
                    ShowGameScreen();
                    turnMade = false;
                }
                if (IsGameOver()) {
                    break;
                }
            }
            ShowGameOver();
            while (true) {
            }
        }

        static void HandleInput(ConsoleKey k) {
            if (!inCombat) {
                MoveInput(k);       //обработка нажатий в режиме перемещения
                if (turnMade) {
                    turn++;     //счетчик ходов
                }
                if (px == ex && py == ey) {
                    inCombat = true;        //вход в боевой режим
                    ShowCombatStats();
                }
                if(px == bonusx && py == bonusy) {
                    PickUpBonus();      //подобрать бонус
                }
            } else {
                CombatInput(k);     //обработка нажатий в боевом режиме
            }            
        }

        static void MoveInput(ConsoleKey k) {
            switch (k) {
                case ConsoleKey.LeftArrow:
                    if (px > 2) {
                        px--;
                        turnMade = true;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (px < width - 1) {
                        px++;
                        turnMade = true;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (py > 2) {
                        py--;
                        turnMade = true;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (py < height - 1) {
                        py++;
                        turnMade = true;
                    }
                    break;
                default:
                    turnMade = false;
                    break;
            }
        }

        static void CombatInput(ConsoleKey k) {
            switch (k) {
                case ConsoleKey.Enter:
                    turnMade = true;
                    combatTurn++;                    
                    break;
            }
            if (turnMade && ehp > 0) {
                DealDamage();                                    
            }                            
        }

        static void PickUpBonus() {
            if(hp < 100) {
                hp = Math.Min(100, hp + 10);
                bonusx = -1;
                bonusy = -1;
                pick = turn;
            }           
        }

        static void GenerateBonus() {
            //координаты бонуса меняются через 20 ходов
            if((turn - pick) % 20 == 0 && (turn - pick) != 0) {
                bonusx = rnd.Next(2, width);
                bonusy = rnd.Next(2, height);
            }
        }

        static void DealDamage() {
            if (rnd.Next(1, 101) <= pChanceToHit) {
                playerDmg = rnd.Next(pdmgMin, pdmgMax + 1);
                ehp -= playerDmg;
            }
            else {
                playerDmg = 0;
            }
            if (rnd.Next(1, 101) <= eChanceToHit) {
                enemyDmg = rnd.Next(edmgMin, edmgMax + 1);
                hp -= enemyDmg;
            } else {
                enemyDmg = 0;
            }
        }

        static void EnemyTurn() {
            //противник ходит в 2 раза реже
            if(turn % 2 == 0) {
                //либо сходить в случайном направлении, либо остаться на месте
                int direction = rnd.Next(1, 6);
                switch (direction) {
                    case 1:
                        if (ex < width - 1)
                            ex++;
                        break;
                    case 2:
                        if (ex > 2)
                            ex--;
                        break;
                    case 3:
                        if (ey > 2)
                            ey--;
                        break;
                    case 4:
                        if (ey < height - 1)
                            ey++;
                        break;
                }
            }
        }

        static void ShowGameScreen() {
            if (inCombat)
                ShowCombat();
            else
                ShowLevel();
        }

        static void ShowLevel() {
            Console.Clear();
            for (int i = 1; i <= height; i++) {
                Console.Write("\t");
                for (int j = 1; j <= width; j++) {
                    if (i == 1 || i == height)
                        Console.Write("-");
                    else if (j == 1 || j == width)
                        Console.Write("|");
                    else if (i == py && j == px)
                        Console.Write("X");
                    else if (i == ey && j == ex)
                        Console.Write("O");
                    else if (i == bonusy && j == bonusx)
                        Console.Write("+");
                    else
                        Console.Write(" ");
                }
                ShowStats(i);
                Console.WriteLine();
            }
        }

        static void ShowCombat() {
            if(combatTurn > 0) {
                if(ehp > 0) {
                    RefreshCombatScreen();
                } else {
                    ResetAfterCombat();
                    ShowCombatVictory();                  
                }                
            }
        }

        static void RefreshCombatScreen() {
            Console.SetCursorPosition(18, 2);
            Console.Write(hp + " ");
            Console.SetCursorPosition(58, 2);
            Console.Write(ehp + " ");
            Console.SetCursorPosition(2, 6 + (combatTurn - 1) * 3);
            Console.WriteLine(playerDmg > 0 ? "Вы нанесли " + playerDmg + " урона!" : "Вы промахнулись!");
            Console.SetCursorPosition(2, 7 + (combatTurn - 1) * 3);
            Console.WriteLine(enemyDmg > 0 ? "Враг нанес Вам " + enemyDmg + " урона!" : "Враг промахнулся по Вам!");
        }

        static void ResetAfterCombat() {
            combatTurn = 0;
            ehp = 50;
            kills++;
            inCombat = false;
            ex = rnd.Next(3, width);
            ey = rnd.Next(3, height);
        }

        static void ShowCombatVictory() {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t\t\tПОБЕДА");
            Console.WriteLine();
            Console.WriteLine("\tЗдоровье: " + hp + "\tУбито ноликов: "+ kills);
        }

        static void ShowCombatStats() {
            Console.Clear();
            Console.WriteLine("\t\tX\t\t\t\t\tO");
            Console.WriteLine();
            Console.WriteLine("\tЗдоровье: " + hp + "\t\t\t\tЗдоровье: " + ehp);
            Console.WriteLine("\tМеткость: " + pChanceToHit + "%" + "\t\t\t\tМеткость: " + eChanceToHit + "%");
            Console.WriteLine("\tУрон: " + pdmgMin + " - " + pdmgMax + "\t\t\t\tУрон: " + edmgMin + " - " + edmgMax);
            Console.WriteLine();
        }

        static void ShowStats(int line) {
            if (line == height / 2)
                Console.Write("\tЗдоровье: " + hp);
            else if (line == height / 2 + 1)
                Console.Write("\tУбито ноликов: " + kills);
            else if (line == height / 2 + 2)
                Console.Write("\tХод: " + turn);
        }

        static void ShowGameOver() {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t\tYOU DIED");
            Console.WriteLine();
            Console.WriteLine("\tХодов: " + turn + "\tУбито ноликов: " + kills);
        }

        static bool IsGameOver() {
            return hp <= 0;
        }
    }
}