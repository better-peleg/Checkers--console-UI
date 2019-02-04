using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex2
{
   public enum eShape
    {
     Blank = ' ',
     White = 'X',
     Black = 'O',
     WhiteKing = 'K',
     BlackKing = 'U',
    }

    internal struct Location
    {
        private int m_Row;
        private int m_Col;

        internal Location(int i_Row, int i_Col)
        {
            m_Col = i_Col;
            m_Row = i_Row;
        }

        internal int RowIndex
        {
            get
            {
                return m_Row;
            }

            set
            {
                m_Row = value;
            }
        }

        internal int ColIndex
        {
            get
            {
                return m_Col;
            }

            set
            {
                m_Col = value;
            }
        }
        
        public static bool operator ==(Location i_Loc1, Location i_Loc2)
        {
            return i_Loc1.ColIndex == i_Loc2.ColIndex && i_Loc1.RowIndex == i_Loc2.RowIndex;
        }

        public static bool operator !=(Location i_Loc1, Location i_Loc2)
        {
            return i_Loc1.ColIndex != i_Loc2.ColIndex || i_Loc1.RowIndex != i_Loc2.RowIndex;
        }

        public override bool Equals(object i_Loc)
        {
            return i_Loc is Location && this == (Location)i_Loc;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class Piece
    {
        private Location m_Location;
        private eShape m_Shape;
        private ePlayerColor m_Color;

        internal Piece(Location i_Location, eShape i_Shape, ePlayerColor i_Color)
        {
            m_Shape = i_Shape;
            m_Color = i_Color;
            m_Location = i_Location;           
        }

        internal ePlayerColor Color
        {
            get { return m_Color; }
        }

        internal eShape Shape
        {
            get { return m_Shape; }

            set { m_Shape = value; }
        }

        public static bool operator ==(Piece i_Piece1, Piece i_Piece2)
        {
            bool output = false;
            if (object.ReferenceEquals(i_Piece1, i_Piece2))
            {
                output = true;
            }            
            else if ((object)i_Piece1 == null || (object)i_Piece2 == null)
            {
                output = false;
            }
            else if (i_Piece1.Color == i_Piece2.Color)
            {
                output = true;
            }

            return output;
        }

        public static bool operator !=(Piece i_Piece1, Piece i_Piece2) => !(i_Piece1 == i_Piece2);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object i_Piece)
        {
            return this == (Piece)i_Piece;
        }
    }
}
