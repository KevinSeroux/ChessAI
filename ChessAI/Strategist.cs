using System;
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
        private const long timingMaxMs = 240; //TODO

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
                for (uint depth = 2; watch.ElapsedMilliseconds < timingMaxMs; depth++)
                {
                    ply = NegaScout(depth,int.MinValue,int.MaxValue);
                    Console.WriteLine("Depth: " + depth + ", time: " + watch.ElapsedMilliseconds);
                }
            }

            watch.Stop();
            Debug.Assert(
                watch.ElapsedMilliseconds < 250,
                "IA took " + watch.ElapsedMilliseconds.ToString() + " which is more than 250ms to decide"
            );

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
                        if (watch.ElapsedMilliseconds >= timingMaxMs)
                            break;

                        int score = -RecursiveNegaMax(depth - 1, ply);
                        if (score > best)
                            best = score;
                    }
                }
            }            

            board.Pop();

            return best;
        }

        private Ply NegaScout(uint depth, int alpha, int beta)
        {
            Debug.Assert(depth >= 2);

            Ply bestPly = null;
            int bestScore = int.MinValue;
            uint d = depth;

            foreach (Ply ply in ruler.GetPossiblePlies())
            {
                int score = -RecursiveNegaScout(d - 1, ply,alpha,beta);
                if (score > bestScore)
                {
                    bestPly = ply;
                    bestScore = score;
                }
            }

            return bestPly;
        }

        private int RecursiveNegaScout(uint depth, Ply parentPly, int alpha, int beta)
        {
            board.Push(parentPly);
            
            int best;
            int score2;

            WDL? wdl = tableReader.getWDL();
            if (wdl.HasValue)
                best = evaluator.EvaluateWDL(wdl.Value);

            else 
            {
                best = int.MinValue;
                
                if (depth == 0)
                    best = evaluator.Evaluate();
                else
                {
                    foreach (Ply ply in ruler.GetPossiblePlies())
                    {
                        if (watch.ElapsedMilliseconds >= timingMaxMs)
                            break;

                        int score = RecursiveNegaScout(depth - 1, ply, -beta, -alpha);
                        score2 = score;

                        if (score > alpha && score < beta && depth > 1)
                            score2 = RecursiveNegaScout(depth - 1, ply, -beta, -score);
                        
                        if (score >= score2)
                            best = score;
                        else
                            best = score2;
                    }
                }
            }

            board.Pop();

            return best;
        }
    }
}
