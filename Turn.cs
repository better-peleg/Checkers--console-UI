using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Ex2
{
    public class Turn
    {
        private bool m_JustAte;
        private bool m_SwitchTurn;
        private Move m_CurrentMove;
        private List<Move> m_LegalMoves;
        private Board m_Board;
        private Player m_CurPlayer;

        internal static bool CheckBoundaries(int i_ColIndex, int i_RowIndex, int i_BoardSize)
        {
            return (i_ColIndex < i_BoardSize && i_ColIndex >= 0) && (i_RowIndex < i_BoardSize && i_RowIndex >= 0);
        }

        internal Turn(Board i_Board, Player i_CurTurn)
        {
            m_JustAte = false;
            m_CurPlayer = i_CurTurn;
            m_Board = i_Board;
            m_LegalMoves = LegalMoves(i_Board, i_CurTurn);
        }

        internal bool JustAte
        {
            get { return m_JustAte; }

            set { m_JustAte = value; }
        }

        internal bool SwitchTurn
        {
            get { return m_SwitchTurn; }

            set { m_SwitchTurn = value; }
        }

        internal bool isValidMove(string i_UserInput)
        {
            bool output = false;
            if (!i_UserInput.Equals("Q"))
            {
                Move userInput = GetMoveFromString(i_UserInput);

                foreach (Move legalMove in m_LegalMoves)
                {
                    if (userInput.Origin == legalMove.Origin && userInput.Destination == legalMove.Destination)
                    {
                        m_CurrentMove = legalMove;
                        output = true;
                        break;
                    }
                }
            }

            return output;
        }

        internal string PlayBotTurn()
        {
            List<Move> botMoves = LegalMoves(m_Board, m_CurPlayer);
            Random random = new Random();
            int randomBotMove = random.Next(0, botMoves.Count);
            Move botRandomMove = botMoves.ElementAt(randomBotMove);
            m_CurrentMove = botRandomMove;
            return botRandomMove.ToString();
        }

        private List<Move> UpdateEdibleMoves(List<Move> i_legalMoves)
        {
            List<Move> onlyEatMoves = new List<Move>();
            bool foundEatingMove = false;
            foreach (Move move in i_legalMoves)
            {
                if (move.IsEatMove)
                {
                    foundEatingMove = true;
                    onlyEatMoves.Add(move);
                    continue;
                }
            }

            List<Move> output;
            if (m_JustAte)
            {
                output = onlyEatMoves;
            }
            else
            {
                output = foundEatingMove ? onlyEatMoves : i_legalMoves;
            }

            return output;
        }

        internal Board getBoard
        {
            get { return m_Board; }
        }

        internal void PlayTurn()
        {
            int originRowIndex = m_CurrentMove.Origin.Coordinates.RowIndex;
            int originColIndex = m_CurrentMove.Origin.Coordinates.ColIndex;
            int destRowIndex = m_CurrentMove.Destination.Coordinates.RowIndex;
            int destColIndex = m_CurrentMove.Destination.Coordinates.ColIndex;
            Board.BoardSquare origin = m_Board.getBoardSquare(originRowIndex, originColIndex);
            Board.BoardSquare destination = m_Board.getBoardSquare(destColIndex, destRowIndex);
            m_Board.setBoardSquare(originRowIndex, originColIndex, eShape.Blank, ePlayerColor.Blank);

            if (m_CurPlayer.Color == ePlayerColor.White && destRowIndex == 0)
            {
                m_Board.setBoardSquare(destRowIndex, destColIndex, eShape.WhiteKing, m_CurPlayer.Color);
            }
            else if (m_CurPlayer.Color == ePlayerColor.Black && destRowIndex == m_Board.Size - 1)
            {
                m_Board.setBoardSquare(destRowIndex, destColIndex, eShape.BlackKing, m_CurPlayer.Color);
            }
            else
            {
                m_Board.setBoardSquare(destRowIndex, destColIndex, origin.GetShape, m_CurPlayer.Color);
            }

            if (m_CurrentMove.IsEatMove)
            {
                int toEatRowIndex = (originRowIndex > destRowIndex) ? (originRowIndex - 1) : (originRowIndex + 1);
                int toEatColIndex = (originColIndex > destColIndex) ? (originColIndex - 1) : (originColIndex + 1);
                m_Board.setBoardSquare(toEatRowIndex, toEatColIndex, eShape.Blank, ePlayerColor.Blank);
                m_JustAte = true;
                CheckForExtraEat(m_Board.getBoardSquare(destRowIndex, destColIndex));
            }          
        }

        internal bool IsWithdrawLegal()
        {
            int whitePlayerScore = 0;
            int blackPlayerScore = 0;
            ePlayerColor currentPlayerColor = m_CurPlayer.Color;
            bool legalWithdraw = false;
            foreach (Board.BoardSquare square in m_Board.Matrix)
            {
                if (square.GetShape != eShape.Blank)
                {
                    switch (square.GetShape)
                    {
                        case eShape.Black:
                            blackPlayerScore++;
                            break;
                        case eShape.BlackKing:
                            blackPlayerScore += 4;
                            break;
                        case eShape.White:
                            whitePlayerScore++;
                            break;
                        case eShape.WhiteKing:
                            whitePlayerScore += 4;
                            break;
                    }
                }
            }

            switch (currentPlayerColor)
            {
                case ePlayerColor.White:
                    legalWithdraw = whitePlayerScore <= blackPlayerScore;
                    break;
                case ePlayerColor.Black:
                    legalWithdraw = whitePlayerScore >= blackPlayerScore;
                    break;
            }

            return legalWithdraw;
        }

        internal bool CheckForNoMoreMoves(Player i_CurPlayer)
        {
            List<Move> moves = LegalMoves(m_Board, i_CurPlayer);
            return moves.Count == 0;
        }

        private void CheckForExtraEat(Board.BoardSquare i_Origin)
        {
            List<Move> legalMoves = SquareLegalMove(i_Origin);
            legalMoves = UpdateEdibleMoves(legalMoves);
            if (legalMoves.Count > 0)
            {
                m_SwitchTurn = false;
                m_LegalMoves = legalMoves;
            }
            else
            {
                m_SwitchTurn = true;
            }
        }

        private Move GetMoveFromString(string i_UserInput)
        {
            string origin = i_UserInput.Substring(0, 2);
            string dest = i_UserInput.Substring(3);
            Board.BoardSquare originSquare = m_Board.getBoardSquare(origin[1] - 'a', origin[0] - 'A');
            Board.BoardSquare destSquare = m_Board.getBoardSquare(dest[1] - 'a', dest[0] - 'A');
            return new Move(originSquare, destSquare, false);
        }

        private List<Move> LegalMoves(Board i_CurBoard, Player i_CurTurn)
        {
            List<Move> legalMoves = new List<Move>();
            int size = i_CurBoard.Size;
            foreach (Board.BoardSquare square in i_CurBoard.Matrix)
            {
                if (square.BoardSquareChecker != null)
                {
                    if (square.BoardSquareChecker.Color == i_CurTurn.Color)
                    {
                        legalMoves.AddRange(SquareLegalMove(square));
                    }
                }
            }

            return UpdateEdibleMoves(legalMoves);
        }

        private List<Move> SquareLegalMove(Board.BoardSquare i_BoardSquare)
        {
            List<Move> output = new List<Move>();
            Location currentLocation = i_BoardSquare.Coordinates;
            int colIndex = currentLocation.ColIndex;
            int rowIndex = currentLocation.RowIndex;
            switch (i_BoardSquare.GetShape)
            {
                case eShape.White:
                    // Checks available right upwards move
                    if (CheckBoundaries(rowIndex - 1, colIndex + 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 1, colIndex + 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.Black) ||
                            (m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.BlackKing))
                        {
                            if (CheckBoundaries(rowIndex - 2, colIndex + 2, m_Board.Size)
                                 && (m_Board.getBoardSquare(rowIndex - 2, colIndex + 2).GetShape == eShape.Blank))
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 2, colIndex + 2), true));
                            }
                        }
                    }

                    // Checks available left upwards move
                    if (CheckBoundaries(rowIndex - 1, colIndex - 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 1, colIndex - 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.Black) || (m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.BlackKing))
                        {
                            if (CheckBoundaries(rowIndex - 2, colIndex - 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex - 2, colIndex - 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 2, colIndex - 2), true));
                            }
                        }
                    }

                    break;
                case eShape.WhiteKing:
                    // Checks available right upwards move
                    if (CheckBoundaries(rowIndex - 1, colIndex + 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 1, colIndex + 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.Black) || (m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.BlackKing))
                        {
                            if (CheckBoundaries(rowIndex - 2, colIndex + 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex - 2, colIndex + 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 2, colIndex + 2), true));
                            }
                        }
                    }

                    // Checks available left upwards move
                    if (CheckBoundaries(rowIndex - 1, colIndex - 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 1, colIndex - 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.Black) || (m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.BlackKing))
                        {
                            if (CheckBoundaries(rowIndex - 2, colIndex - 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex - 2, colIndex - 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 2, colIndex - 2), true));
                            }
                        }
                    }

                    // Checks available right downwards move
                    if (CheckBoundaries(rowIndex + 1, colIndex + 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 1, colIndex + 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.Black) || (m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.BlackKing))
                        {
                            if (CheckBoundaries(rowIndex + 2, colIndex + 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex + 2, colIndex + 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 2, colIndex + 2), true));
                            }
                        }
                    }

                    // Checks available left downwards move
                    if (CheckBoundaries(rowIndex + 1, colIndex - 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 1, colIndex - 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.Black) || (m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.BlackKing))
                        {
                            if (CheckBoundaries(rowIndex + 2, colIndex - 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex + 2, colIndex - 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 2, colIndex - 2), true));
                            }
                        }
                    }

                    break;
                case eShape.Black:
                    // Checks available right downwards move
                    if (CheckBoundaries(rowIndex + 1, colIndex + 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 1, colIndex + 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.White) || (m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.WhiteKing))
                        {
                            if (CheckBoundaries(rowIndex + 2, colIndex + 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex + 2, colIndex + 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 2, colIndex + 2), true));
                            }
                        }
                    }

                    // Checks available left downwards move
                    if (CheckBoundaries(rowIndex + 1, colIndex - 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 1, colIndex - 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.White) || (m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.WhiteKing))
                        {
                            if (CheckBoundaries(rowIndex + 2, colIndex - 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex + 2, colIndex - 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 2, colIndex - 2), true));
                            }
                        }
                    }

                    break;
                case eShape.BlackKing:
                    // Checks available right upwards move
                    if (CheckBoundaries(rowIndex - 1, colIndex + 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 1, colIndex + 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.White) || (m_Board.getBoardSquare(rowIndex - 1, colIndex + 1).GetShape == eShape.WhiteKing))
                        {
                            if (CheckBoundaries(rowIndex - 2, colIndex + 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex - 2, colIndex + 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 2, colIndex + 2), true));
                            }
                        }
                    }

                    // Checks available left upwards move
                    if (CheckBoundaries(rowIndex - 1, colIndex - 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 1, colIndex - 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.White) || (m_Board.getBoardSquare(rowIndex - 1, colIndex - 1).GetShape == eShape.WhiteKing))
                        {
                            if (CheckBoundaries(rowIndex - 2, colIndex - 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex - 2, colIndex - 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex - 2, colIndex - 2), true));
                            }
                        }
                    }

                    // Checks available right downwards move
                    if (CheckBoundaries(rowIndex + 1, colIndex + 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 1, colIndex + 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.White) || (m_Board.getBoardSquare(rowIndex + 1, colIndex + 1).GetShape == eShape.WhiteKing))
                        {
                            if (CheckBoundaries(rowIndex + 2, colIndex + 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex + 2, colIndex + 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 2, colIndex + 2), true));
                            }
                        }
                    }

                    // Checks available left downwards move
                    if (CheckBoundaries(rowIndex + 1, colIndex - 1, m_Board.Size))
                    {
                        if (m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.Blank)
                        {
                            output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 1, colIndex - 1), false));
                        }
                        else if ((m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.White) || (m_Board.getBoardSquare(rowIndex + 1, colIndex - 1).GetShape == eShape.WhiteKing))
                        {
                            if (CheckBoundaries(rowIndex + 2, colIndex - 2, m_Board.Size) && m_Board.getBoardSquare(rowIndex + 2, colIndex - 2).GetShape == eShape.Blank)
                            {
                                output.Add(new Move(m_Board.getBoardSquare(rowIndex, colIndex), m_Board.getBoardSquare(rowIndex + 2, colIndex - 2), true));
                            }
                        }
                    }

                    break;
            }

            return output;
        }       
    }
}