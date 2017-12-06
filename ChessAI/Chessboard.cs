using System.Collections.Generic;

namespace ChessAI
{
    // TODO: Reset the chessboard when resetting all the positions
    class Chessboard
    {
        // TODO: Replace both "object" by chessboard representation
        private object pos;
        private Stack<object> stack;

        public Chessboard()
        {
            stack = new Stack<object>();
        }

        public uint CountMen
        {
            get { return 5; }
        }

        public void Push(Ply ply)
        {
            // Backup the current chessboard
            stack.Push(pos);

            //TODO; Here apply the ply
            //...
        }

        // Cancel the previous ply
        public void Pop()
        {
            pos = stack.Pop();
        }

        // Restore the chessboard to the initial state
        public void PopAll()
        {
            while (stack.Count >= 2) stack.Pop();
            Pop();
        }

        public Bitboard ToBitboard()
        {
            Bitboard board = new Bitboard();

            //Test FEM: 8/8/8/8/1p2P3/4P3/1k6/3K4 w - - 0 1
            board.white = 0x0000000010100008;
            board.black = 0x0000000002000200;
            board.pawns = 0x0000000012100000;
            board.rooks = 0x0000000000000000;
            board.knights = 0x0000000000000000;
            board.bishops = 0x0000000000000000;
            board.queens = 0x0000000000000000;
            board.kings = 0x0000000000000208;
            board.castling = 0x00000000; // Always at 0
            board.turn = true;

            return board;
        }
    }
}
