using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Ply
    {
        // TODO: Not sure for some promotion
        public abstract class Promotion
        {
            public abstract override String ToString();
        }

        public class QueenPromotion : Promotion
        {
            public override string ToString()
            {
                return "D";
            }
        }

        public class KnightPromotion : Promotion
        {
            public override string ToString()
            {
                return "C";
            }
        }

        public class RookPromotion : Promotion
        {
            public override string ToString()
            {
                return "T";
            }
        }

        public class BishopPromotion : Promotion
        {
            public override string ToString()
            {
                return "F";
            }
        }

        private bool castlingKingSide;
        private bool castlingQueenSide;
        private Case from;
        private Case to;
        private Promotion promotion;
        private Case captureEnPassant;

        private Ply()
        {
            castlingKingSide = false;
            castlingQueenSide = false;
            from = null;
            to = null;
            promotion = null;
        }

        public override String ToString()
        {
            String str;

            if (castlingQueenSide)
                str = "grand roque";

            else if (castlingKingSide)
                str = "petit roque";

            else
            {
                str = from.ToString() + ',' + to.ToString();
                if (promotion != null)
                    str += "," + promotion.ToString();
            }

            return str;
        }

        static public Ply Position(uint from, uint to)
        {
            return Position(new Case(from), new Case(to));
        }

        static public Ply Position(Case from, Case to)
        {
            Ply newPly = new Ply();
            newPly.from = from;
            newPly.to = to;
            return newPly;
        }

        static public Ply CastlingKingSide()
        {
            Ply newPly = new Ply();
            newPly.castlingKingSide = true;
            return newPly;
        }

        static public Ply CastlingQueenSide()
        {
            Ply newPly = new Ply();
            newPly.castlingQueenSide = true;
            return newPly;
        }

        static public Ply PromoteQueen(uint from, uint to)
        {
            return PromoteQueen(new Case(from), new Case(to));
        }

        static public Ply PromoteQueen(Case from, Case to)
        {
            Ply newPly = Position(from, to);
            newPly.promotion = new QueenPromotion();
            return newPly;
        }

        static public Ply PromoteKnight(uint from, uint to)
        {
            return PromoteKnight(new Case(from), new Case(to));
        }

        static public Ply PromoteKnight(Case from, Case to)
        {
            Ply newPly = Position(from, to);
            newPly.promotion = new KnightPromotion();
            return newPly;
        }

        static public Ply PromoteRook(uint from, uint to)
        {
            return PromoteRook(new Case(from), new Case(to));
        }

        static public Ply PromoteRook(Case from, Case to)
        {
            Ply newPly = Position(from, to);
            newPly.promotion = new RookPromotion();
            return newPly;
        }

        static public Ply PromoteBishop(uint from, uint to)
        {
            return PromoteBishop(new Case(from), new Case(to));
        }

        static public Ply PromoteBishop(Case from, Case to)
        {
            Ply newPly = Position(from, to);
            newPly.promotion = new BishopPromotion();
            return newPly;
        }

        static public Ply EnPassant(uint from, uint to, uint capture)
        {
            return EnPassant(new Case(from), new Case(to), new Case(capture));
        }

        static public Ply EnPassant(Case from, Case to, Case capture)
        {
            Ply newPly = Position(from, to);
            newPly.captureEnPassant = capture;
            return newPly;
        }
    }
}
