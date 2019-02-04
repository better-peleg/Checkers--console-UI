using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex2
{
    public class Game
    {
        internal enum eGameStatus
        {
            Playing,
            Draw,
            Win,
        }

        private int m_size;
        private Player m_PlayerOne;
        private Player m_PlayerTwo;
        private Player m_CurrentPlayerTurn; // Pointer, which player turn it is.
        private Player m_NextPlayerTurn;
        private Board m_Board;
        private eGameStatus m_GameStatus;

        public Game()
        {
            this.StartGame();
            while (m_GameStatus == eGameStatus.Playing)
            {
                this.PlayRound(m_size);
                if (GameInterface.PlayAnotherRound())
                {
                    m_GameStatus = eGameStatus.Playing;
                }

                Ex02.ConsoleUtils.Screen.Clear();
            }

            GameInterface.EndGameMessage();
        }

        private void StartGame()
        {
            GameInterface.ShowWelcomeMessage();
            string PlayerOneName = GameInterface.getPlayerName();
            m_size = GameInterface.getBoardSize();            
            m_PlayerOne = new Player("User", PlayerOneName, ePlayerColor.White); // Create first player
            m_PlayerTwo = GameInterface.getOpponent();
            m_GameStatus = eGameStatus.Playing;
            GameInterface.ShowRulesMessage();
        }

        private void PlayRound(int i_size)
        {
            m_Board = new Board(i_size);
            GameInterface.ShowStartTurnMessage(m_Board, m_PlayerOne);
            m_CurrentPlayerTurn = m_PlayerOne;
            m_NextPlayerTurn = m_PlayerTwo;
            this.playTurn();
            switch (m_GameStatus)
            {
                case eGameStatus.Win:
                    this.updateScore();
                    GameInterface.PrintEndOfRoundWin(m_GameStatus, m_NextPlayerTurn, m_CurrentPlayerTurn);
                    break;
                case eGameStatus.Draw:
                    GameInterface.PrintEndOfRoundDraw(m_GameStatus, m_PlayerOne, m_PlayerTwo);
                    break;
            }
        }

        private void playTurn()
        {
            bool isValidMove;
            bool isLegalWithdraw = false;
            string currentMoveString = string.Empty;
            Turn currentTurn = new Turn(m_Board, m_CurrentPlayerTurn);
            if (!checkForEndRound(currentTurn))
            {
                if (currentTurn.SwitchTurn)
                {
                    currentTurn = new Turn(m_Board, m_CurrentPlayerTurn);
                }

                switch (m_CurrentPlayerTurn.Type())
                {
                    case eType.User:
                        {
                            currentMoveString = GameInterface.getMove(m_Board.Size);
                            isValidMove = currentTurn.isValidMove(currentMoveString);
                            isLegalWithdraw = GameInterface.IsLegalWithdraw(currentMoveString, currentTurn);
                            while (!isValidMove && !isLegalWithdraw)
                            {
                                currentMoveString = GameInterface.InvalidMove(m_Board.Size);
                                isValidMove = currentTurn.isValidMove(currentMoveString);
                                isLegalWithdraw = GameInterface.IsLegalWithdraw(currentMoveString, currentTurn);
                            }

                            break;
                        }

                    case eType.Bot:
                        {
                            currentMoveString = currentTurn.PlayBotTurn();
                            isLegalWithdraw = false;
                            break;
                        }
                }

                        if (!isLegalWithdraw)
                        {
                            currentTurn.PlayTurn();
                            if (currentTurn.JustAte)
                            {
                            currentTurn.SwitchTurn = true; 
                            }

                            m_Board = currentTurn.getBoard;

                            if (currentTurn.SwitchTurn)
                            {
                                GameInterface.ShowRoundDetails(m_Board, m_CurrentPlayerTurn, m_NextPlayerTurn, currentMoveString);
                                switchPlayers();
                            }
                            else
                            {
                                GameInterface.ShowRoundDetails(m_Board, m_CurrentPlayerTurn, m_CurrentPlayerTurn, currentMoveString);
                            }

                            this.playTurn();
                        }
                        else
                        {
                            m_GameStatus = eGameStatus.Win;
                        }
                }
            }
    
        private bool checkForEndRound(Turn i_CurrentTurn)
        {
            bool endRound = false;
            if(i_CurrentTurn.CheckForNoMoreMoves(m_CurrentPlayerTurn) && i_CurrentTurn.CheckForNoMoreMoves(m_NextPlayerTurn))
            {                
                m_GameStatus = eGameStatus.Draw;
                endRound = true;
            }
            else if (i_CurrentTurn.CheckForNoMoreMoves(m_CurrentPlayerTurn))
            {                
                m_GameStatus = eGameStatus.Win;
                endRound = true;
            }

            return endRound;
        }       

        private void switchPlayers()
        {
            Player temp = m_CurrentPlayerTurn;
            m_CurrentPlayerTurn = m_NextPlayerTurn;
            m_NextPlayerTurn = temp;
        }
        
        private void updateScore()
        {
            foreach(Board.BoardSquare currentSquare in m_Board.Matrix)
            {
                if(currentSquare.GetShape == eShape.White)
                {
                    m_PlayerOne.Score++;
                }

                if (currentSquare.GetShape == eShape.WhiteKing)
                {
                    m_PlayerOne.Score += 4;
                }

                if (currentSquare.GetShape == eShape.Black)
                {
                    m_PlayerTwo.Score++;
                }

                if (currentSquare.GetShape == eShape.BlackKing)
                {
                    m_PlayerTwo.Score += 4;
                }
            }
        }
        
        private void printBoard()
        {
            this.m_Board.PrintBoard();
        }
    }
}
