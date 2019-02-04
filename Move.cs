using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex2
{
    internal class Move
    {
        private Board.BoardSquare m_Origin;
        private Board.BoardSquare m_Dest;
        private bool m_IsEatMove;

        internal Move(Board.BoardSquare i_Origin, Board.BoardSquare i_Dest, bool i_IsEatMove)
        {
            m_IsEatMove = i_IsEatMove;
            m_Origin = i_Origin;
            m_Dest = i_Dest;
        }

        internal Board.BoardSquare Origin
        {
            get { return m_Origin; }

            set { m_Origin = value; }
        }

        internal Board.BoardSquare Destination
        {
            get { return m_Dest; }

            set { m_Dest = value; }
        }

        internal bool IsEatMove
        {
            get { return m_IsEatMove; }
            set { m_IsEatMove = value; }
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append((char)(Origin.Coordinates.ColIndex + 'A'));
            output.Append((char)(Origin.Coordinates.RowIndex + 'a'));
            output.Append('>');
            output.Append((char)(Destination.Coordinates.ColIndex + 'A'));
            output.Append((char)(Destination.Coordinates.RowIndex + 'a'));
            return output.ToString();
        }
    }
}