using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Evaluator
    {
        public enum WDL
        {
            WIN = int.MaxValue,
            CURSED_WIN = int.MaxValue / 2, //TODO
            DRAW = 0,
            BLESSED_LOSS = int.MinValue / 2, //TODO
            LOSS = int.MinValue
        }

        private Chessboard board;

        public Evaluator(Chessboard board)
        {
            this.board = board;
        }

        //Valeur des pièces Pawn/Knight/Bichop/Rook/Queen/King
        private readonly static double[] piecesValues = { 1.0, 3.0, 3.0, 5.0, 8.0, 20.0 };
        //TODO Ameliorer en prenant en compte les pawn doubled/blocked/isolated
        //TODO Ameliorer en prenant en comptre le nombre de coups possibles ?
        public int Evaluate()
        {

            //Les rois sont-ils tj présents sur le plateau ?
            if (board.GetMailbox().endGame)
            {
                //TODO verifier si ca s'est bon
                //Si on arrive la c'est qu'on est le joueur qui a plus de roi
                //Sinon faire une evaluation normale (car elle prend en compte le poids des pièce
                //et que le roi à un gros poids normalement
                return int.MinValue+1;
            }

            return evaluateVersion3();
        }

        private int evaluateVersion1()
        {
            double valeurPiece = 0;


            double valeurCouvertureW = 0;
            double valeurProtectionW = 0;
            double valeurAttaqueW = 0;

            double valeurCouvertureB = 0;
            double valeurProtectionB = 0;
            double valeurAttaqueB = 0;


            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = (int)board.GetTurn;
            int xside = (int)(board.GetTurn == Color.WHITE ? Color.BLACK : Color.WHITE);


            for (int i = 0; i < 64; ++i)
            { /* loop over all squares (no piece list) */

                int p = piece[i];
                if (p != (int)Color.NONE && p != (int)Color.PAWN_EN_PASSANT)
                {
                    valeurPiece += piecesValues[p] * color[i];

                    if (p != (int)Piece.PAWN)
                    {
                        /* piece or pawn */
                        for (int j = 0; j < Mailbox.offsets[p]; ++j)
                        { /* for all knight or ray directions */
                            for (int n = i; ;)
                            { /* starting with from square */
                                n = Mailbox.tab120[Mailbox.tabPos[n] + Mailbox.offset[p, j]]; /* next square along the ray j */
                                if (n == -1) break; /* outside board */

                                if (color[n] != (int)Color.NONE && color[n] != (int)Color.PAWN_EN_PASSANT)
                                {
                                    if (color[n] == xside && xside == (int)Color.BLACK)
                                        valeurAttaqueW++; /* capture from i to n */ //----------------------------------------------------- < attaque possible
                                    else if (color[n] == xside && xside == (int)Color.WHITE)
                                        valeurAttaqueB++;
                                    if (color[n] == side && side == (int)Color.WHITE)
                                        valeurProtectionW++;
                                    else if (color[n] == side && side == (int)Color.BLACK)
                                        valeurProtectionB++;


                                    break;
                                }

                                if (color[i] == (int)Color.WHITE)
                                    valeurCouvertureW++; /* quiet move from i to n */ //------------------------------------------------------------< deplacement possible
                                else valeurCouvertureB++;

                                if (!Mailbox.slide[p]) break; /* next direction */
                            }
                        }
                    }
                    else
                    {
                        // -------------------------------- Les "en passant" ne sont pas pris en charge encore ------------------------------
                        /* pawn moves */
                        if (side == (int)Color.WHITE)
                        {
                            int n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 3]];
                            if (n != -1)
                            {
                                //Double "saut"
                                if (Mailbox.tabPos[i] <= 88 && Mailbox.tabPos[i] >= 81)
                                {
                                    if (color[n] == (int)Color.NONE && (Mailbox.tabPos[n + Mailbox.offset[0, 0]] == (int)Color.NONE))
                                    {
                                        valeurCouvertureW++;
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (Mailbox.tabPos[n] == (int)Color.NONE)
                                {
                                    valeurCouvertureW++;
                                }
                            }


                            //Manger Gauche
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    valeurAttaqueW++;
                                }
                                else if (color[n] == (int)Color.WHITE)
                                {
                                    valeurProtectionW++;
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    valeurAttaqueW++;
                                }
                                else if (color[n] == (int)Color.WHITE)
                                {
                                    valeurProtectionW++;
                                }
                            }

                        }
                        else //side == dark
                        {
                            int n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 3]];
                            if (n != -1)
                            {
                                if (Mailbox.tabPos[i] >= 31 && Mailbox.tabPos[i] <= 38)
                                {

                                    if (color[n] == (int)Color.NONE && (Mailbox.tabPos[n + Mailbox.offset[0, 0]] == (int)Color.NONE))
                                    {
                                        valeurCouvertureB++;
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (color[i - Mailbox.MMPAWN[0]] == (int)Color.NONE)
                                {
                                    valeurCouvertureB++;
                                }
                            }

                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE)
                                {
                                    valeurAttaqueB++;
                                }
                                else if (color[n] == (int)Color.BLACK)
                                {
                                    valeurProtectionB++;
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE)
                                {
                                    valeurAttaqueB++;
                                }
                                else if (color[n] == (int)Color.BLACK)
                                {
                                    valeurProtectionB++;
                                }
                            }
                        }
                    }
                }
            }

            double mobilite = valeurCouvertureB - valeurCouvertureW; //TODO ajouter un poids ici
            double protection = valeurProtectionB - valeurProtectionW; //TODO same
            double attaque = valeurAttaqueB - valeurAttaqueW; //TODO same

            int eval = (int)((valeurPiece + 0.5 * mobilite + 0.6 * protection + 0.6 * attaque)) * side;

            //Console.WriteLine("Evaluation {0} : {1}", side, eval);
            return eval;
            //return valeurAttaqueW + valeurCouvertureW + valeurPiece + valeurProtectionW;
            //return (new Random()).Next();
        }


        private int evaluateVersion2()
        {
            double valeurPiece = 0;
            double valeurCouvertureW = 0;
            double valeurProtectionW = 0;
            double valeurAttaqueW = 0;
            double valeurCouvertureB = 0;
            double valeurProtectionB = 0;
            double valeurAttaqueB = 0;


            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = (int)board.GetTurn;
            int xside = (int)(board.GetTurn == Color.WHITE ? Color.BLACK : Color.WHITE);


            for (int i = 0; i < 64; ++i)
            { /* loop over all squares (no piece list) */

                int p = piece[i];
                if (p != (int)Color.NONE && p != (int)Color.PAWN_EN_PASSANT)
                {
                    valeurPiece += piecesValues[p]* color[i];

                    if (p != (int)Piece.PAWN)
                    {
                        /* piece or pawn */
                        for (int j = 0; j < Mailbox.offsets[p]; ++j)
                        { /* for all knight or ray directions */
                            for (int n = i; ;)
                            { /* starting with from square */
                                n = Mailbox.tab120[Mailbox.tabPos[n] + Mailbox.offset[p, j]]; /* next square along the ray j */
                                if (n == -1) break; /* outside board */

                                if (color[n] == (int)Color.WHITE || color[n] == (int)Color.BLACK)
                                {
                                    if (color[n] == xside)
                                    {
                                        if (xside == (int)Color.BLACK)
                                            valeurAttaqueW++;
                                        else
                                            valeurAttaqueB++;
                                    }
                                    else
                                    {
                                        if (side == (int)Color.WHITE)
                                            valeurProtectionW++;
                                        else
                                            valeurProtectionB++;
                                    }
                                    break;
                                }

                                if (color[i] == (int)Color.WHITE)
                                    valeurCouvertureW++; /* quiet move from i to n */ //------------------------------------------------------------< deplacement possible
                                else valeurCouvertureB++;

                                if (!Mailbox.slide[p]) break; /* next direction */
                            }
                        }
                    }
                    else
                    {
                        // -------------------------------- Les "en passant" ne sont pas pris en charge encore ------------------------------
                        int n;
                        int valeurPosition = Mailbox.tabPos[i];
                        /* pawn moves */
                        if (side == (int)Color.WHITE)
                        {
                            //Double "saut"
                            if (valeurPosition <= 88 && valeurPosition >= 81)
                            {
                                n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 3]];
                                if (n != -1)
                                {
                                
                                    if (color[n] == (int)Color.NONE && (Mailbox.tabPos[n + Mailbox.offset[0, 0]] == (int)Color.NONE))
                                    {
                                        valeurCouvertureW++;
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (Mailbox.tabPos[n] == (int)Color.NONE)
                                {
                                    valeurCouvertureW++;
                                }
                            }


                            //Manger Gauche
                            n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    valeurAttaqueW++;
                                }
                                else if (color[n] == (int)Color.WHITE)
                                {
                                    valeurProtectionW++;
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    valeurAttaqueW++;
                                }
                                else if (color[n] == (int)Color.WHITE)
                                {
                                    valeurProtectionW++;
                                }
                            }

                        }
                        else //side == dark
                        {
                            if (valeurPosition >= 31 && valeurPosition <= 38)
                            {
                                n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 3]];
                                if (n != -1)
                                {
                                    if (color[n] == (int)Color.NONE && (Mailbox.tabPos[n + Mailbox.offset[0, 0]] == (int)Color.NONE))
                                    {
                                        valeurCouvertureB++;
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (color[i - Mailbox.MMPAWN[0]] == (int)Color.NONE)
                                {
                                    valeurCouvertureB++;
                                }
                            }

                            n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE)
                                {
                                    valeurAttaqueB++;
                                }
                                else if (color[n] == (int)Color.BLACK)
                                {
                                    valeurProtectionB++;
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE)
                                {
                                    valeurAttaqueB++;
                                }
                                else if (color[n] == (int)Color.BLACK)
                                {
                                    valeurProtectionB++;
                                }
                            }
                        }
                    }
                }
            }

            double mobilite = valeurCouvertureB - valeurCouvertureW; //TODO ajouter un poids ici
            double protection = valeurProtectionB - valeurProtectionW; //TODO same
            double attaque = valeurAttaqueB - valeurAttaqueW; //TODO same

            int eval = (int)((valeurPiece + 0.5 * mobilite + 0.6 * protection + 0.6 * attaque)) * side;

            //Console.WriteLine("Evaluation {0} : {1}", side, eval);
            return eval;
            //return valeurAttaqueW + valeurCouvertureW + valeurPiece + valeurProtectionW;
            //return (new Random()).Next();
        }


        private int evaluateVersion3()
        {
            double valeurPiece = 0;
            double valeurCouverture = 0;
            double valeurProtection = 0;
            double valeurAttaque = 0;


            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = (int)board.GetTurn;


            for (int i = 0; i < 64; ++i)
            { /* loop over all squares (no piece list) */

                int p = piece[i];
                int c = color[i];
                if (p != (int)Color.NONE && p != (int)Color.PAWN_EN_PASSANT)
                {
                    valeurPiece += piecesValues[p] * color[i];

                    if (p != (int)Piece.PAWN)
                    {
                        /* piece or pawn */
                        for (int j = 0; j < Mailbox.offsets[p]; ++j)
                        { /* for all knight or ray directions */
                            for (int n = i; ;)
                            { /* starting with from square */
                                n = Mailbox.tab120[Mailbox.tabPos[n] + Mailbox.offset[p, j]]; /* next square along the ray j */
                                if (n == -1) break; /* outside board */

                                if (color[n] == (int)Color.WHITE || color[n] == (int)Color.BLACK)
                                {
                                    if (color[n] != c)
                                    {
                                        valeurAttaque += piecesValues[p]*c;//(c * piecesValues[piece[n]]);
                                    }
                                    else
                                    {
                                        valeurProtection += piecesValues[p]*c;// (c * piecesValues[piece[n]]);
                                    }
                                    break;
                                }
                                valeurCouverture+= c; /* quiet move from i to n */ //------------------------------------------------------------< deplacement possible
                              

                                if (!Mailbox.slide[p]) break; /* next direction */
                            }
                        }
                    }
                    else
                    {
                        // -------------------------------- Les "en passant" ne sont pas pris en charge encore ------------------------------
                        int n;
                        int valeurPosition = Mailbox.tabPos[i];
                        
                        /* pawn moves */
                        if (color[i] == (int)Color.WHITE)
                        {
                            //Double "saut"
                            if (valeurPosition <= 88 && valeurPosition >= 81)
                            {
                                n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 3]];
                                if (n != -1)
                                {
                                    int caseEntre = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 0]];
                                    if (color[n] == (int)Color.NONE && (color[caseEntre] == (int)Color.NONE))
                                    {
                                        valeurCouverture+= c;
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.NONE)
                                {
                                    valeurCouverture+= c;
                                }
                            }


                            //Manger Gauche
                            n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    valeurAttaque += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                                else if (color[n] == (int)Color.WHITE)
                                {
                                    valeurProtection += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[valeurPosition + Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    valeurAttaque += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                                else if (color[n] == (int)Color.WHITE)
                                {
                                    valeurProtection += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                            }

                        }
                        else //side == dark
                        {
                            if (valeurPosition >= 31 && valeurPosition <= 38)
                            {
                                n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 3]];
                                if (n != -1)
                                {
                                    int caseEntre = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 0]];
                                    if (color[n] == (int)Color.NONE && (color[caseEntre] == (int)Color.NONE))
                                    {
                                        valeurCouverture+= c;
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.NONE)
                                {
                                    valeurCouverture+= c;
                                }
                            }

                            n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE)
                                {
                                    valeurAttaque += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                                else if (color[n] == (int)Color.BLACK)
                                {
                                    valeurProtection += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[valeurPosition - Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE)
                                {
                                    valeurAttaque += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                                else if (color[n] == (int)Color.BLACK)
                                {
                                    valeurProtection += piecesValues[p]*c;// (c * piecesValues[piece[n]]); ;
                                }
                            }
                        }
                    }
                }
            }



            int eval = (int)((10 * valeurPiece + 2 * valeurCouverture + 0.2 * valeurProtection + 4 * valeurAttaque)) * side;

            Console.WriteLine("Evaluation {1}: {0} , piece = {2}, couverture = {3}, protection = {4}, attaque = {5}", eval,side,valeurPiece,valeurCouverture,valeurProtection,valeurAttaque);
            return eval;
            //return valeurAttaqueW + valeurCouvertureW + valeurPiece + valeurProtectionW;
            //return (new Random()).Next();
        }

        private int evaluateVersion4()
        {
            return (new Random()).Next(500);
        }
        public int EvaluateWDL(WDL wdl)
        {
            return Convert.ToInt32(wdl);
        }



    }
}
