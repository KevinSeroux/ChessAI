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
            int valeurPiece = 0;


            int valeurCouvertureW = 0;
            int valeurProtectionW = 0;
            int valeurAttaqueW = 0;

            int valeurCouvertureB = 0;
            int valeurProtectionB = 0;
            int valeurAttaqueB = 0;


            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = (int)board.GetMailbox().getSideToPlay();
            int xside = (int)board.GetMailbox().getSideToPlay();


            for (int i = 0; i < 64; ++i)
            { /* loop over all squares (no piece list) */
                    int p = piece[i];

                    if (p != (int)Piece.PAWN)
                    {
                        switch ((Piece)piece[i])
                        {
                            case Piece.KNIGHT:
                                valeurPiece += 3 * color[i];
                                break;
                            case Piece.BISHOP:
                                valeurPiece += 3 * color[i];
                                break;
                            case Piece.ROOK:
                                valeurPiece += 3 * color[i];
                                break;
                            case Piece.QUEEN:
                                valeurPiece += 10 * color[i];
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

                                if (color[n] != (int)Color.NONE)
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
                            //Double "saut"
                            if (Mailbox.tabPos[i] <= 88 && Mailbox.tabPos[i] >= 81)
                            {
                                if (color[i + Mailbox.MMPAWN[3]] == (int)Color.NONE)
                                {
                                    valeurCouvertureW++;
                                }
                            }

                            //Avancer
                            if (color[i + Mailbox.MMPAWN[0]] == (int)Color.NONE)
                        {
                            valeurCouvertureW++;
                            }


                            //Manger Gauche
                            if (color[i + Mailbox.MMPAWN[1]] == (int)Color.BLACK)
                            {
                                valeurAttaqueW++;
                            }
                            else if (color[i + Mailbox.MMPAWN[1]] == (int)Color.WHITE)
                            {
                                valeurProtectionW++;
                            }

                            //Manger droite
                            if (color[i + Mailbox.MMPAWN[2]] == (int)Color.BLACK)
                            {
                                valeurAttaqueW++;
                            }
                            else if (color[i + Mailbox.MMPAWN[2]] == (int)Color.WHITE)
                            {
                                valeurProtectionW++;
                            }

                        }
                        else //side == dark
                        {
                            //Double "saut"
                            if (Mailbox.tabPos[i] >= 31 && Mailbox.tabPos[i] <= 38)
                            {
                                if (color[i - Mailbox.MMPAWN[3]] == (int)Color.NONE)
                                {
                                    valeurCouvertureB++;
                                }
                            }

                            //Avancer
                            if (color[i - Mailbox.MMPAWN[0]] == (int)Color.NONE)
                            {
                                valeurCouvertureB++;
                            }

                            //Manger Gauche
                            if (color[i - Mailbox.MMPAWN[1]] == (int)Color.WHITE)
                            {
                                valeurAttaqueB++;
                            }
                            else if (color[i - Mailbox.MMPAWN[1]] == (int)Color.BLACK)
                            {
                                valeurProtectionB++;
                            }

                            //Manger droite
                            if (color[i - Mailbox.MMPAWN[2]] == (int)Color.WHITE)
                            {
                                valeurAttaqueB++;
                            }
                            else if (color[i - Mailbox.MMPAWN[2]] == (int)Color.BLACK)
                            {
                                valeurProtectionB++;
                            }
                        }
                    }
                
            }

            int mobilite = valeurCouvertureB - valeurCouvertureW; //TODO ajouter un poids ici
            int protection = valeurProtectionB - valeurProtectionW; //TODO same
            int attaque = valeurAttaqueB - valeurAttaqueW; //TODO same

            return (valeurPiece + mobilite + protection + attaque) * side;
            //return valeurAttaqueW + valeurCouvertureW + valeurPiece + valeurProtectionW;
            //return (new Random()).Next();
        }

        public int EvaluateWDL(WDL wdl)
        {
            return Convert.ToInt32(wdl);
        }



    }
}
