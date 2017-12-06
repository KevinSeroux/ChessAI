using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Case
    {
        uint rank;
        char file;

        public Case(uint pos)
        {
            rank = pos / 8 + 1;
            file = (char)('a' + (pos % 8));
        }

        public Case (String pos)
        {
            file = pos[0];
            rank = (uint)Char.GetNumericValue(pos[1]);
        }

        public override String ToString()
        {
            return file + rank.ToString();
        }
    }
}
