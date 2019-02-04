using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex2
{
    public enum eType
    {
        User,
        Bot
    }

    public enum ePlayerColor
    {
        White = 'X', // (X)
        Black = 'O', // (O)
        Blank = ' '// (' ')
    }

    public class Player
    {
        private eType m_Type;
        private string m_Name;
        private int m_Score;
        private ePlayerColor m_Color; 
 
        // Constructor
        internal Player(string i_Type, string i_Name, ePlayerColor i_Color)
        {
            this.m_Type = getTypeFromString(i_Type);
            this.m_Name = i_Name;
            this.m_Score = 0;
            this.m_Color = i_Color;       
        }

        internal ePlayerColor Color
        {
            get { return m_Color; }
        }

        internal int Score
        {
            get { return m_Score; }

            set { m_Score = value; }
        }

        // Returens the player type - User or Computer
        internal eType Type()
        {
            return m_Type;
        }

        // Returns Player name
        internal string GetName()
        {
            return m_Name;
        }

        internal eType getTypeFromString(string i_Type)
        {
            eType output;
            if (i_Type.ToUpper().Equals("USER"))
            {
                output = eType.User;
            }
            else
            {
                output = eType.Bot;
            }

            return output;
        }
    }
}
