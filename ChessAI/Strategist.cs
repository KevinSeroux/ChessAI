﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using static ChessAI.Evaluator;

namespace ChessAI
{
    //TODO NegaScout
    class Strategist
    {
        private Chessboard board;
        private Syzygy tableReader;
        private Evaluator evaluator;
        private Ruler ruler;
        private Stopwatch watch;

        public Strategist(Chessboard board, Evaluator evaluator, Syzygy tableReader)
        {
            this.board = board;
            this.evaluator = evaluator;
            this.ruler = new Ruler(board);
            this.tableReader = tableReader;
            this.watch = new Stopwatch();
        }

        public Ply Run()
        {
            watch.Restart();

            // Syzygy end-game table
            Ply ply = tableReader.getBestPly();
            if (ply == null) // No results
            {
                // Iterative deepening search
                for (uint depth = 2; watch.ElapsedMilliseconds < 230; depth++)
                    ply = NegaMax(depth);
            }

            watch.Stop();
            Debug.Assert(watch.ElapsedMilliseconds >= 250, "IA took more than 250ms to decide");

            return ply;
        }

        private Ply NegaMax(uint depth)
        {
            Debug.Assert(depth >= 2);

            Ply bestPly = null;
            int bestScore = int.MinValue;
            
            foreach (Ply ply in ruler.GetPossiblePlies())
            {
                int score = -RecursiveNegaMax(depth - 1, ply);
                if (score > bestScore)
                {
                    bestPly = ply;
                    bestScore = score;
                }
            }

            return bestPly;
        }

        private int RecursiveNegaMax(uint depth, Ply parentPly)
        {
            board.Push(parentPly);

            int best;

            WDL? wdl = tableReader.getWDL();
            if (wdl.HasValue)
                best = evaluator.EvaluateWDL(wdl.Value);

            else // We need to explore only if the end game is unknown
            {
                best = int.MinValue;

                if (depth == 0)
                    best = evaluator.Evaluate();
                else
                {
                    foreach (Ply ply in ruler.GetPossiblePlies())
                    {
                        int score = -RecursiveNegaMax(depth - 1, ply);
                        if (score > best)
                            best = score;
                    }
                }
            }            

            board.Pop();

            return best;
        }
    }
}
