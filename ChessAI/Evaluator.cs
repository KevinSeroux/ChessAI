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

            int[] color = board.GetMailboxRepresentation().getColor();
            int[] piece = board.GetMailboxRepresentation().getPiece();
            int side = board.GetMailboxRepresentation().getSideToPlay();
            int xside = board.GetMailboxRepresentation().getSideToPlay();

            for (int i = 0; i < 64; ++i)
            { /* loop over all squares (no piece list) */
                if (color[i] == side)
                { /* looking for own pieces and pawns to move */
                    int p = piece[i];
                    if (p != MailboxRepresentation.PAWN)
                    {
                        switch (piece[i])
                        {
                            case MailboxRepresentation.KNIGHT:
                                valeurPiece += 3;
                                break;
                            case MailboxRepresentation.BISHOP:
                                valeurPiece += 3;
                                break;
                            case MailboxRepresentation.ROOK:
                                valeurPiece += 3;
                                break;
                            case MailboxRepresentation.QUEEN:
                                valeurPiece += 10;
                                break;

                        }
                        /* piece or pawn */
                        for (int j = 0; j < MailboxRepresentation.offsets[p]; ++j)
                        { /* for all knight or ray directions */
                            for (int n = i; ;)
                            { /* starting with from square */
                                n = MailboxRepresentation.tab120[MailboxRepresentation.tabPos[n] + MailboxRepresentation.offset[p, j]]; /* next square along the ray j */
                                if (n == -1) break; /* outside board */
                                if (color[n] != MailboxRepresentation.EMPTY)
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
                                if (!MailboxRepresentation.slide[p]) break; /* next direction */
                            }
                        }
                    }
                    else
                    {
                        valeurPiece += 3;
                        // -------------------------------- Les "en passant" ne sont pas pris en charge encore ------------------------------
                        /* pawn moves */
                        if (side == MailboxRepresentation.LIGHT)
                        {
                            //Double "saut"
                            if (MailboxRepresentation.tabPos[i] <= 88 && MailboxRepresentation.tabPos[i] >= 81)
                            {
                                if (color[i + MailboxRepresentation.MMPAWN[3]] == MailboxRepresentation.EMPTY)
                                {
                                    valeurCouverture++;
                                }
                            }

                            //Avancer
                            if (color[i + MailboxRepresentation.MMPAWN[0]] == MailboxRepresentation.EMPTY)
                            {
                                valeurCouverture++;
                            }


                            //Manger Gauche
                            if (color[i + MailboxRepresentation.MMPAWN[1]] == MailboxRepresentation.DARK)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i + MailboxRepresentation.MMPAWN[1]] == MailboxRepresentation.LIGHT)
                            {
                                valeurProtection++;
                            }

                            //Manger droite
                            if (color[i + MailboxRepresentation.MMPAWN[2]] == MailboxRepresentation.DARK)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i + MailboxRepresentation.MMPAWN[2]] == MailboxRepresentation.LIGHT)
                            {
                                valeurProtection++;
                            }

                        }
                        else //side == dark
                        {
                            //Double "saut"
                            if (MailboxRepresentation.tabPos[i] >= 31 && MailboxRepresentation.tabPos[i] <= 38)
                            {
                                if (color[i - MailboxRepresentation.MMPAWN[3]] == MailboxRepresentation.EMPTY)
                                {
                                    valeurCouverture++;
                                }
                            }

                            //Avancer
                            if (color[i - MailboxRepresentation.MMPAWN[0]] == MailboxRepresentation.EMPTY)
                            {
                                valeurCouverture++;
                            }

                            //Manger Gauche
                            if (color[i - MailboxRepresentation.MMPAWN[1]] == MailboxRepresentation.LIGHT)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i - MailboxRepresentation.MMPAWN[1]] == MailboxRepresentation.DARK)
                            {
                                valeurProtection++;
                            }

                            //Manger droite
                            if (color[i - MailboxRepresentation.MMPAWN[2]] == MailboxRepresentation.LIGHT)
                            {
                                valeurAttaque++;
                            }
                            else if (color[i - MailboxRepresentation.MMPAWN[2]] == MailboxRepresentation.DARK)
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
