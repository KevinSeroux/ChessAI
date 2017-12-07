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

        //TODO Ameliorer en prenant en compte les pawn doubled/blocked/isolated
        //TODO Ameliorer en prenant en comptre le nombre de coups possibles ?
        public int Evaluate()
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
                if (p != (int)Color.NONE && p!=(int)Color.PAWN_EN_PASSANT)
                {
                    if (p != (int)Piece.PAWN)
                    {
                        switch ((Piece)piece[i])
                        {
                            case Piece.KNIGHT:
                                valeurPiece += 7 * color[i];
                                break;
                            case Piece.BISHOP:
                                valeurPiece += 35 * color[i];
                                break;
                            case Piece.ROOK:
                                valeurPiece += 35 * color[i];
                                break;
                            case Piece.QUEEN:
                                valeurPiece += 100 * color[i];
                                break;
                            case Piece.KING:
                                valeurPiece += 200 * color[i];
                                break;

                        }
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
                        valeurPiece += 3 * color[i];
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

            int eval = (int)((valeurPiece + 0.5 * mobilite + 0.6 * protection + 0.6 * attaque) * side);

            Console.WriteLine("Evaluation {0} : {1}", side, eval);
            return eval;
            //return valeurAttaqueW + valeurCouvertureW + valeurPiece + valeurProtectionW;
            //return (new Random()).Next();
        }

        public int EvaluateWDL(WDL wdl)
        {
            return Convert.ToInt32(wdl);
        }



    }
}
