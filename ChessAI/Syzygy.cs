using System;

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
            uint ep = Fathom.GetEP(res);
            uint promote = Fathom.GetPromotes(res);

            uint from = Fathom.GetFrom(res);
            uint to = Fathom.GetTo(res);
            Ply ply = new Ply(from, to);

            String promoteStr;
            switch(promote)
            {
                case Fathom.TB_PROMOTES_NONE:
                    promoteStr = "nope";
                    break;

                case Fathom.TB_PROMOTES_ROOK:
                    promoteStr = "ROOK";
                    break;

                case Fathom.TB_PROMOTES_KNIGHT:
                    promoteStr = "KNIGHT";
                    break;

                case Fathom.TB_PROMOTES_BISHOP:
                    promoteStr = "BISHOP";
                    break;

                case Fathom.TB_PROMOTES_QUEEN:
                    promoteStr = "QUEEN";
                    break;

                default:
                    promoteStr = "N/A";
                    break;
            }

            Console.Out.WriteLine(
                "Status: " + status + "\n" +
                "WDL: " + wdlStr + ", DTZ: " + dtz + "\n" +
                "ply: " + ply.ToString + "\n" +
                "promote: " + promoteStr + "\n" +
                "ep: " + ep + "\n"
            );

            return ply;
        }
    }
}
