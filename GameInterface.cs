using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ex2
{
    internal class GameInterface
    {        
        internal static void ShowWelcomeMessage()
        {            
            string welcomeMSG = "Welcome to" + Environment.NewLine
                            + "╔═╗┬ ┬┌─┐┌─┐┬┌─┌─┐┬─┐┌─┐" + Environment.NewLine
                            + "║  ├─┤├┤ │  ├┴┐├┤ ├┬┘└─┐" + Environment.NewLine
                            + "╚═╝┴ ┴└─┘└─┘┴ ┴└─┘┴└─└─┘ ";
            Console.WriteLine(welcomeMSG);
        }

        internal static void ShowRulesMessage()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            string msg =
@"The object of the game is to prevent the opponent from being able to move when it is his turn to do so. This is accomplished either by capturing all of the opponent's Checkers, or by blocking those that remain so that none of them can be moved. If neither player can accomplish this, the game is a draw.
----------------------------------------------------------------------
You can not eat backwords, and if you have an option to eat it is your only valid move.
On each turn, you will see the board, and the previous player move in the format COLrow>COLrow.
You will have to write your move in the same format (COLrow>COLrow.)
For example if you have a checker on square Af and you want to move it to Be your input should be Af>Be.
----------------------------------------------------------------------
Good Luck & Enjoy!
----------------------------------------------------------------------
                Please press Enter to start
----------------------------------------------------------------------";
            Console.WriteLine(msg);
            Console.ReadLine();
            Ex02.ConsoleUtils.Screen.Clear();
        }

        internal static void EndGameMessage()
        {
            Console.WriteLine("Thanks for playing. We hope you enjoyed. (Copyrights) Peleg & Guy");
        }

        internal static bool PlayAnotherRound()
        {
            bool playAnother = false;
            Console.WriteLine("Would you like to play another round? (Yes/No)");
            string userInput = Console.ReadLine();
            if(userInput.ToLower().Equals("yes"))
            {
                playAnother = true;
            }
            else if(userInput.ToLower().Equals("no"))
            {
                playAnother = false;
            }
            else
            {
                Console.WriteLine("Ilegal input, please enter Yes or No.");
                playAnother = PlayAnotherRound();
            }

            return playAnother;
        }

        internal static void PrintEndOfRoundWin(Game.eGameStatus i_GameStatus, Player i_WiningPlayer, Player i_LosingPlayer)
        {
            Ex02.ConsoleUtils.Screen.Clear();
            string msg = string.Format(
@"-----------------------------------------
    Congrats! The winner of this round is {0}!!
          Better luck next time {1}.
-----------------------------------------
-----------------------------------------
             Current Scores
-----------------------------------------
          {0}'s score is: {2}
          {1}'s score is: {3}
-----------------------------------------",
                    i_WiningPlayer.GetName(),
                    i_LosingPlayer.GetName(),
                    i_WiningPlayer.Score,
                    i_LosingPlayer.Score);
            Console.WriteLine(msg);
        }

        internal static void PrintEndOfRoundDraw(Game.eGameStatus i_GameStatus, Player i_PlayerOne, Player i_PlayerTwo)
        {
            Ex02.ConsoleUtils.Screen.Clear();
            string msg = string.Format(
@"-----------------------------------------
              It's a Draw!!
-----------------------------------------
-----------------------------------------
             Current Scores
-----------------------------------------
          {0}'s score is: {1}
          {2}'s score is: {3}
-----------------------------------------",
                    i_PlayerOne.GetName(),
                    i_PlayerOne.Score,
                    i_PlayerTwo.GetName(),
                    i_PlayerTwo.Score );
            Console.WriteLine(msg);
        }

        internal static void ShowStartTurnMessage(Board i_Board, Player i_CurrentPlayer)
        {
            i_Board.PrintBoard();
            Console.WriteLine(i_CurrentPlayer.GetName() + "'s turn:");
        }

        internal static void ShowRoundDetails(Board i_Board, Player i_CurrentPlayer, Player i_OtherPlayer, string i_LastMove)
        {
            Ex02.ConsoleUtils.Screen.Clear();
            i_Board.PrintBoard();
            string roundDetails = string.Format(
@"{0}'s move was ({1}): {2}
{3}'s Turn ({4}): ",
                    i_CurrentPlayer.GetName(),
                    (char)i_CurrentPlayer.Color,
                    i_LastMove,
                    i_OtherPlayer.GetName(),
                    (char)i_OtherPlayer.Color);
            Console.WriteLine(roundDetails);
        }

        internal static String InvalidMove(int i_BoardSize)
        {
            Console.WriteLine("Invalid Move. Please enter a valid move");
            return getMove(i_BoardSize);
        }

        internal static bool IsLegalWithdraw(string i_Input, Turn i_Turn)
        {
            bool legalWithdraw = false;
            if (i_Input.Equals("Q"))
            {
                legalWithdraw = i_Turn.IsWithdrawLegal();
                if (!legalWithdraw)
                { 
                    Console.WriteLine("Only the losing player can withdraw.");
                }
            }

            return legalWithdraw;
        }
        
        internal static String getMove(int i_BoardSize)
        {
            string userInput = Console.ReadLine();
            while (!isLegalMoveInput(userInput, i_BoardSize))
            {
                Console.WriteLine("Invalid input. Please enter a valid move in the format COLrow>COLrow");
                userInput = Console.ReadLine();
            }

            return userInput;
        }

        internal static Boolean isLegalMoveInput(string i_UserInput, int i_BoardSize)
        {
            bool output = false;
            if (i_UserInput.Equals("Q"))
            {
                output = true;
            }
            else
            {
                string Pattern = string.Empty;
                switch (i_BoardSize)
                {
                    case 6:
                        Pattern = @"[A-F][a-f]>[A-F][a-f]";
                        break;
                    case 8:
                        Pattern = @"[A-H][a-h]>[A-H][a-h]";
                        break;
                    case 10:
                        Pattern = @"[A-J][a-j]>[A-J][a-j]";
                        break;
                }

                Match matcher = Regex.Match(i_UserInput, Pattern);
                output = matcher.Success;
            }

            return output;
        }

        internal static Player getOpponent()
        {
            Console.WriteLine("Will you be playing against another user or a bot?(Enter \"user\" or \"bot\")");
            string OpponentType = Console.ReadLine();
            bool isLegalType = OpponentType.ToUpper().Equals("USER") || OpponentType.ToUpper().Equals("BOT");
            while (!isLegalType)
            {
                Console.WriteLine("Sorry, you can not play against this type here. Please enter a valid type (User / Bot)");
                OpponentType = Console.ReadLine();
                isLegalType = OpponentType.ToUpper().Equals("USER") || OpponentType.ToUpper().Equals("BOT");
            }

            string OpponentName;
            if (OpponentType.ToUpper().Equals("USER"))
            {
                OpponentName = getPlayerName();
            }
            else
            {
                OpponentName = "Bot";
            }

            return new Player(OpponentType, OpponentName, ePlayerColor.Black);
        }

        internal static String getPlayerName()
        {
            Console.WriteLine("Please enter your name. (spaceless and up to 20 characters)");
            string PlayerName = Console.ReadLine();
            bool isNameLegal = !PlayerName.Contains(" ") || PlayerName.Length <= 20;
            while (!isNameLegal)
            {
                Console.WriteLine("Illegal name, please enter a valid name containing up to 20 characters and no spaces");
                PlayerName = Console.ReadLine();
                isNameLegal = !PlayerName.Contains(" ") || PlayerName.Length <= 20;
            }

            return PlayerName;
        }

        internal static int getBoardSize()
        {
            Console.WriteLine("please enter your preffered board size (6,8 or 10):");
            int boardSize;
            bool legalBoardSize = int.TryParse(Console.ReadLine(), out boardSize);
            bool isLegal = legalBoardSize && (boardSize == 6 || boardSize == 8 || boardSize == 10);
            while (!isLegal)
            {
                Console.WriteLine("Illegal board size, please enter a valid Integer board size:(6, 8 or 10)");
                legalBoardSize = int.TryParse(Console.ReadLine(), out boardSize);
                isLegal = legalBoardSize && (boardSize == 6 || boardSize == 8 || boardSize == 10);
            }

            return boardSize;
        }
    }
}
