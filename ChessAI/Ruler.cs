using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Ruler
    {
        private Chessboard board;

        public Ruler(Chessboard board)
        {
            this.board = board;
        }

        //TODO
        public ICollection<Ply> GetPossiblePlies()
        {
            return new HashSet<Ply>();
        }
    }
}
