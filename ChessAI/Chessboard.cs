using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChessAI
{

    enum Piece
    {
        PAWN = 0,
        KNIGHT = 1,
        BISHOP = 2,
        ROOK = 3,
        QUEEN = 4,
        KING = 5
        //PAWN_EN_PASSANT = 6 //TODO
    }

    enum Color
    {
        WHITE = -1,
        BLACK = 1,
        NONE = 6,
        PAWN_EN_PASSANT = 7
    }
    // TODO: Reset the chessboard when resetting all the positions
    class Chessboard
    {
        private Mailbox pos;
        private Stack<Mailbox> stack;
        private Color turn;

        public Chessboard()
        {
            stack = new Stack<Mailbox>();
            turn = Color.WHITE;
        }

        public uint CountMen
        {
            get
            {
                return 5; //TODO pos.CountMen
            }
        }

        public void Push(Ply ply)
        {
            // Backup the current chessboard
            stack.Push(pos);

            //Copy of pos
            Mailbox mailbox = new Mailbox(pos);
            mailbox.ply(ply);
            switchTurn();
            pos = mailbox;
        }

        // Cancel the previous ply
        public void Pop()
        {
            pos = stack.Pop();
            switchTurn();
        }
        
        public Bitboard ToBitboard()
        {
            Bitboard board = new Bitboard();

            board.white = board.black = 0;
            board.pawns = board.rooks = board.knights = board.bishops = board.queens = board.kings = 0;
            board.castling = 0; // Always at 0
            board.turn = (turn == Color.WHITE);

            int[] colors = pos.getColor();
            int[] pieces = pos.getPiece();

            for (uint rank = 0; rank < 8; ++rank)
                for (uint file = 0; file < 8; ++file)
                {
                    uint mailBoxIndex = rank * 8 + file;
                    uint bitboardIndex = (7 - rank) * 8 + file;
                    UInt64 bitboardValue = (UInt64)Math.Pow(2, bitboardIndex);

                    switch ((Color)colors[mailBoxIndex])
                    {
                        case Color.WHITE:
                            board.white += bitboardValue;
                            break;

                        case Color.BLACK:
                            board.black += bitboardValue;
                            break;

                        case Color.PAWN_EN_PASSANT:
                            board.ep = bitboardIndex;
                            break;
                    }

                    switch((Piece)pieces[mailBoxIndex])
                    {
                        case Piece.PAWN:
                            board.pawns += bitboardValue;
                            break;

                        case Piece.ROOK:
                            board.rooks += bitboardValue;
                            break;

                        case Piece.BISHOP:
                            board.bishops += bitboardValue;
                            break;

                        case Piece.KNIGHT:
                            board.knights += bitboardValue;
                            break;

                        case Piece.QUEEN:
                            board.queens += bitboardValue;
                            break;

                        case Piece.KING:
                            board.kings += bitboardValue;
                            break;
                    }
                }


            //Test FEM: 8/8/8/8/1p2P3/4P3/1k6/3K4 w - - 0 1
            /*board.white = 0x0000000010100008;
            board.black = 0x0000000002000200;
            board.pawns = 0x0000000012100000;
            board.rooks = 0x0000000000000000;
            board.knights = 0x0000000000000000;
            board.bishops = 0x0000000000000000;
            board.queens = 0x0000000000000000;
            board.kings = 0x0000000000000208;
            board.castling = 0x00000000; // Always at 0
            board.turn = true;*/

            return board;
        }

        public Mailbox GetMailbox()
        {
            return pos;
        }

        public Color GetTurn
        {
            get { return turn; }
        }
        
        public void ResetFromPlatformRepresentation(int[] tabVal, Color agentColor)
        {
            pos = new Mailbox(tabVal);
            stack.Clear();
            turn = agentColor;
        }

        private void switchTurn()
        {
            turn = (turn == Color.WHITE ? Color.BLACK : Color.WHITE);
        }
    }
}
