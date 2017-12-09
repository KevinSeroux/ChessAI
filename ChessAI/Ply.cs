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
            public int piece;
            public abstract override String ToString();
        }

        public class QueenPromotion : Promotion
        {

            public QueenPromotion() { piece = (int)Piece.QUEEN; }
            public override string ToString()
            {
                return "D";
            }
        }

        public class KnightPromotion : Promotion
        {

            public KnightPromotion() { piece = (int)Piece.KNIGHT; }
            public override string ToString()
            {
                return "C";
            }
        }

        public class RookPromotion : Promotion
        {
            public RookPromotion() { piece = (int)Piece.ROOK; }
            public override string ToString()
            {
                return "T";
            }
        }

        public class BishopPromotion : Promotion
        {

            public BishopPromotion() { piece = (int)Piece.BISHOP; }
            public override string ToString()
            {
                return "F";
            }
        }


        public bool castlingKingSide { get; private set; }
        public bool castlingQueenSide { get; private set; }
        public Case from { get; private set; }
        public Case to { get; private set; }
        public Promotion promotion { get; private set; }
        public Case captureEnPassant { get; private set; }
        //public Case captureEnPassantCapture { get; private set; }
        public bool lastPly { get; set; }

        public bool captureEP { get; private set; }

        private Ply()
        {
            castlingKingSide = false;
            castlingQueenSide = false;
            from = null;
            to = null;
            promotion = null;
            lastPly = false;
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
                str = from.ToString() + ',' + to.ToString() + ',';
                if (promotion != null)
                    str += promotion.ToString();
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

        static public Ply EatKing(uint from, uint to)
        {
            Ply newPly = Position(from, to);
            newPly.lastPly = true;
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

        static public Ply EnPassant(uint from, uint to, uint capturePossible, bool captureEffective)
        {
            return EnPassant(new Case(from), new Case(to), new Case(capturePossible), captureEffective);
        }

        static public Ply EnPassant(Case from, Case to, Case capturePossible, bool captureEffective)
        {
            Ply newPly = Position(from, to);
            newPly.captureEnPassant = capturePossible;
            newPly.captureEP = captureEffective;
            return newPly;
        }
    }
}
