using System;
using System.Diagnostics;
using static ChessAI.Evaluator;

namespace ChessAI
{
    class SyzygyLoadException : Exception
    {
        public SyzygyLoadException(String msg) : base(msg) {}
    }

    class Syzygy
    {
        private Chessboard board;
        private Fathom impl;

        public Syzygy(Chessboard board, String path)
        {
            this.board = board;
            try { impl = new Fathom(path); }
            catch (Exception e) when (
                e is DllNotFoundException || e is SyzygyLoadException
            ) { System.Console.WriteLine(e.Message); }
        }
        
        public WDL? getWDL()
        {
            if (impl is null) return null;
            if (board.CountMen > impl.Largest) return null;

            Bitboard bitboard = board.ToBitboard();

            uint res = impl.ProbeWDL(
                bitboard.white,
                bitboard.black,
                bitboard.kings,
                bitboard.queens,
                bitboard.rooks,
                bitboard.bishops,
                bitboard.knights,
                bitboard.pawns,
                0, // 50-move clock
                bitboard.castling,
                bitboard.ep, // "En passant"
                bitboard.turn
            );

            switch(res)
            {
                case Fathom.TB_WIN:
                    return WDL.WIN;

                case Fathom.TB_CURSED_WIN:
                    return WDL.CURSED_WIN;

                case Fathom.TB_DRAW:
                    return WDL.DRAW;

                case Fathom.TB_BLESSED_LOSS:
                    return WDL.BLESSED_LOSS;

                case Fathom.TB_LOSS:
                    return WDL.LOSS;

                default:
                    return null;
            }
        }

        // Castling is not supported
        public Ply getBestPly()
        {
            if (impl is null) return null;
            if (board.CountMen > impl.Largest) return null;
            
            Bitboard bitboard = board.ToBitboard();

            uint res = impl.ProbeRoot(
                bitboard.white,
                bitboard.black,
                bitboard.kings,
                bitboard.queens,
                bitboard.rooks,
                bitboard.bishops,
                bitboard.knights,
                bitboard.pawns,
                0, // 50-move clock
                bitboard.castling,
                bitboard.ep, // "En passant"
                bitboard.turn,
                UIntPtr.Zero // No more results
            );

            if (res == Fathom.TB_RESULT_FAILED)
                return null;

            uint from = Fathom.GetFrom(res);
            uint to = Fathom.GetTo(res);
            uint promote = Fathom.GetPromotes(res);
            uint ep = Fathom.GetEP(res);

            Ply ply;

            switch (promote)
            {
                case Fathom.TB_PROMOTES_ROOK:
                    ply = Ply.PromoteRook(from, to);
                    break;

                case Fathom.TB_PROMOTES_KNIGHT:
                    ply = Ply.PromoteKnight(from, to);
                    break;

                case Fathom.TB_PROMOTES_BISHOP:
                    ply = Ply.PromoteBishop(from, to);
                    break;

                case Fathom.TB_PROMOTES_QUEEN:
                    ply = Ply.PromoteQueen(from, to);
                    break;

                default:
                    if (ep == 0)
                        ply = Ply.Position(from, to);
                    else
                        ply = Ply.EnPassant(from, to, ep,false); //TODO <----- Qu'est ce qu'il se passe la ? Si capture effective mettre true, si generation d'une case EP mettre false

                    break;
            }


            // From here, it is just printf debugging
            {
                String status;

                if (res == Fathom.TB_RESULT_CHECKMATE)
                    status = "Checkmate";

                else if (res == Fathom.TB_RESULT_STALEMATE)
                    status = "Stalemate";

                else if (res == Fathom.TB_RESULT_FAILED)
                    status = "Failed";

                else
                    status = "Normal";

                uint wdl = Fathom.GetWDL(res);
                String wdlStr;

                switch (wdl)
                {
                    case Fathom.TB_WIN:
                        wdlStr = "WIN";
                        break;

                    case Fathom.TB_CURSED_WIN:
                        wdlStr = "CURSED WIN";
                        break;

                    case Fathom.TB_DRAW:
                        wdlStr = "DRAW";
                        break;

                    case Fathom.TB_BLESSED_LOSS:
                        wdlStr = "BLESSED LOSS";
                        break;

                    case Fathom.TB_LOSS:
                        wdlStr = "LOSS";
                        break;

                    default:
                        wdlStr = "N/A";
                        break;
                }

                uint dtz = Fathom.GetDTZ(res);

                Console.Out.WriteLine(
                    "Status: " + status + "\n" +
                    "WDL: " + wdlStr + ", DTZ: " + dtz + "\n" +
                    "ply: " + ply.ToString() + "\n" +
                    "ep: " + ep + "\n"
                );
            }

            return ply;
        }
    }
}
