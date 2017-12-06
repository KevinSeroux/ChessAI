using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Ruler
    {
        private Chessboard board;

        public Ruler(Chessboard board)
        {
            this.board = board;
        }

        //TODO
        public ICollection<Ply> GetPossiblePlies()
        {
            List<Ply> mouvementPossible = new List<Ply>();
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
                    { /* piece or pawn */
                        for (int j = 0; j < MailboxRepresentation.offsets[p]; ++j)
                        { /* for all knight or ray directions */
                            for (int n = i; ;)
                            { /* starting with from square */
                                n = MailboxRepresentation.tab120[MailboxRepresentation.tabPos[n] + MailboxRepresentation.offset[p, j]]; /* next square along the ray j */
                                if (n == -1) break; /* outside board */
                                if (color[n] != MailboxRepresentation.EMPTY)
                                {
                                    if (color[n] == xside)
                                        mouvementPossible.Add(genMove(i, n, 1)); /* capture from i to n */
                                    break;
                                }
                                mouvementPossible.Add(genMove(i, n, 0)); /* quiet move from i to n */
                                if (!MailboxRepresentation.slide[p]) break; /* next direction */
                            }
                        }
                    }
                    else
                    {

                        // -------------------------------- Les "en passant" ne sont pas pris en charge encore ------------------------------
                        /* pawn moves */
                        if (side == MailboxRepresentation.LIGHT)
                        {
                            //Double "saut"
                            if (MailboxRepresentation.tabPos[i] <= 88 && MailboxRepresentation.tabPos[i] >= 81)
                            {
                                if (color[i + MailboxRepresentation.MMPAWN[3]] == MailboxRepresentation.EMPTY)
                                {
                                    mouvementPossible.Add(genMove(i, i + MailboxRepresentation.MMPAWN[3], 0));
                                }
                            }

                            //Avancer
                            if (color[i + MailboxRepresentation.MMPAWN[0]] == MailboxRepresentation.EMPTY)
                            {
                                mouvementPossible.Add(genMove(i, i + MailboxRepresentation.MMPAWN[0], 0));
                            }

                            //Manger Gauche
                            if (color[i + MailboxRepresentation.MMPAWN[1]] == MailboxRepresentation.DARK)
                            {
                                mouvementPossible.Add(genMove(i, i + MailboxRepresentation.MMPAWN[1], 1));
                            }

                            //Manger droite
                            if (color[i + MailboxRepresentation.MMPAWN[2]] == MailboxRepresentation.DARK)
                            {
                                mouvementPossible.Add(genMove(i, i + MailboxRepresentation.MMPAWN[2], 1));
                            }

                        }
                        else //side == dark
                        {
                            //Double "saut"
                            if (MailboxRepresentation.tabPos[i] >= 31 && MailboxRepresentation.tabPos[i] <= 38)
                            {
                                if (color[i - MailboxRepresentation.MMPAWN[3]] == MailboxRepresentation.EMPTY)
                                {
                                    mouvementPossible.Add(genMove(i, i - MailboxRepresentation.MMPAWN[3], 0));
                                }
                            }

                            //Avancer
                            if (color[i - MailboxRepresentation.MMPAWN[0]] == MailboxRepresentation.EMPTY)
                            {
                                mouvementPossible.Add(genMove(i, i - MailboxRepresentation.MMPAWN[0], 0));
                            }

                            //Manger Gauche
                            if (color[i - MailboxRepresentation.MMPAWN[1]] == MailboxRepresentation.LIGHT)
                            {
                                mouvementPossible.Add(genMove(i, i - MailboxRepresentation.MMPAWN[1], 1));
                            }

                            //Manger droite
                            if (color[i - MailboxRepresentation.MMPAWN[2]] == MailboxRepresentation.LIGHT)
                            {
                                mouvementPossible.Add(genMove(i, i - MailboxRepresentation.MMPAWN[2], 1));
                            }
                        }
                    }
                }
            }

            return mouvementPossible;
            //return new HashSet<Ply>();
        }

        private Ply genMove(int i, int n, int v)
        {
            string dp = MailboxRepresentation.tabCoord[i];
            string arr = MailboxRepresentation.tabCoord[n];

            return Ply.Position(new Case(dp), new Case(arr));

            //int valeur = evaluateBoard(LIGHT);

            //Console.WriteLine("Mouvement possible {2} : {0}, {1}", dp, arr, piece[i]);
        }
    }
}
