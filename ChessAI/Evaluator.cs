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

        //TODO
        public int Evaluate()
        {
            int valeurPiece = 0;
            int valeurCouverture = 0;
            int valeurProtection = 0;
            int valeurAttaque = 0;

            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = board.GetMailbox().getSideToPlay();
            int xside = board.GetMailbox().getSideToPlay();

            for (int i = 0; i < 64; ++i)
            { /* loop over all squares (no piece list) */
                if (color[i] == side)
                { /* looking for own pieces and pawns to move */
                    int p = piece[i];
                    if (p != Mailbox.PAWN)
                    {
                        switch (piece[i])
                        {
                            case Mailbox.KNIGHT:
                                valeurPiece += 3;
                                break;
                            case Mailbox.BISHOP:
                                valeurPiece += 3;
                                break;
                            case Mailbox.ROOK:
                                valeurPiece += 3;
                                break;
                            case Mailbox.QUEEN:
                                valeurPiece += 10;
                                break;

                        }
                        /* piece or pawn */
                        for (int j = 0; j < Mailbox.offsets[p]; ++j)
                        { /* for all knight or ray directions */
                            for (int n = i; ;)
                            { /* starting with from square */
                                n = Mailbox.tab120[Mailbox.tabPos[n] + Mailbox.offset[p, j]]; /* next square along the ray j */
                                if (n == -1) break; /* outside board */
                                if (color[n] != Mailbox.EMPTY)
                                {
                                    if (color[n] == xside)
                                        valeurAttaque++; /* capture from i to n */ //----------------------------------------------------- < attaque possible
                                    else
                                    {
                                        valeurProtection++;//--------------------------------------------------------------------------------------------------< protection
                                    }
                                    break;
                                }
                                valeurCouverture++; /* quiet move from i to n */ //------------------------------------------------------------< deplacement possible
                                if (!Mailbox.slide[p]) break; /* next direction */
                            }
                        }
                    }
                    else
                    {
                        valeurPiece += 3;
                        // -------------------------------- Les "en passant" ne sont pas pris en charge encore ------------------------------
                        /* pawn moves */
                        if (side == Mailbox.LIGHT)
                        {
                            //Double "saut"
                            if (Mailbox.tabPos[i] <= 88 && Mailbox.tabPos[i] >= 81)
                            {
                                if (color[i + Mailbox.MMPAWN[3]] == Mailbox.EMPTY)
                                {
                                    valeurCouverture++;
                                }
                            }

                            //Avancer
                            if (color[i + Mailbox.MMPAWN[0]] == Mailbox.EMPTY)
                            {
                                valeurCouverture++;
                            }


                            //Manger Gauche
                            if (color[i + Mailbox.MMPAWN[1]] == Mailbox.DARK)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i + Mailbox.MMPAWN[1]] == Mailbox.LIGHT)
                            {
                                valeurProtection++;
                            }

                            //Manger droite
                            if (color[i + Mailbox.MMPAWN[2]] == Mailbox.DARK)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i + Mailbox.MMPAWN[2]] == Mailbox.LIGHT)
                            {
                                valeurProtection++;
                            }

                        }
                        else //side == dark
                        {
                            //Double "saut"
                            if (Mailbox.tabPos[i] >= 31 && Mailbox.tabPos[i] <= 38)
                            {
                                if (color[i - Mailbox.MMPAWN[3]] == Mailbox.EMPTY)
                                {
                                    valeurCouverture++;
                                }
                            }

                            //Avancer
                            if (color[i - Mailbox.MMPAWN[0]] == Mailbox.EMPTY)
                            {
                                valeurCouverture++;
                            }

                            //Manger Gauche
                            if (color[i - Mailbox.MMPAWN[1]] == Mailbox.LIGHT)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i - Mailbox.MMPAWN[1]] == Mailbox.DARK)
                            {
                                valeurProtection++;
                            }

                            //Manger droite
                            if (color[i - Mailbox.MMPAWN[2]] == Mailbox.LIGHT)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i - Mailbox.MMPAWN[2]] == Mailbox.DARK)
                            {
                                valeurProtection++;
                            }
                        }
                    }
                }
            }

            return valeurAttaque + valeurCouverture + valeurPiece + valeurProtection;
            //return (new Random()).Next();
        }

        public int EvaluateWDL(WDL wdl)
        {
            return Convert.ToInt32(wdl);
        }
    }
}
