using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Evaluator
    {
        private const int WDL_WIN_VALUE = int.MaxValue;
        private const int WDL_CURSED_WIN_VALUE = int.MaxValue / 2; //TODO
        private const int WDL_DRAW_VALUE = 0;
        private const int WDL_LOSS_VALUE = int.MinValue / 2; //TODO
        private const int WDL_LOSS = int.MinValue;

        private Chessboard board;

        public Evaluator(Chessboard board)
        {
            this.board = board;
        }

        //TODO
        public int Evaluate()
        {
            return (new Random()).Next();
        }

        public int? EvaluateWDL(WDL? wdl)
        {
            switch(wdl)
            {
                case WDL.WIN:
                    return WDL_WIN_VALUE;

                case WDL.CURSED_WIN:
                    return WDL_CURSED_WIN_VALUE;

                case WDL.DRAW:
                    return WDL_DRAW_VALUE;

                case WDL.BLESSED_LOSS:
                    return WDL_LOSS_VALUE;

                case WDL.LOSS:
                    return WDL_LOSS;

                default:
                    return null;
            }
        }
    }
}
