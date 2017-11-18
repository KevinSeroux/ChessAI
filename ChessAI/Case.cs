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

        public new String ToString => file + rank.ToString();
    }
}
