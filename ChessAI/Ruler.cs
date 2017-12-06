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
            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = (int)board.GetMailbox().getSideToPlay();
            int xside = (int)board.GetMailbox().getSideToPlay();


            for (int i = 0; i < 64; ++i)
            { /* loop over all squares (no piece list) */
                if (color[i] == side)
                { /* looking for own pieces and pawns to move */
                    int p = piece[i];

                    if (p != (int)Piece.PAWN)
                    { /* piece or pawn */
                        for (int j = 0; j < Mailbox.offsets[p]; ++j)
                        { /* for all knight or ray directions */
                            for (int n = i; ;)
                            { /* starting with from square */
                                n = Mailbox.tab120[Mailbox.tabPos[n] + Mailbox.offset[p, j]]; /* next square along the ray j */
                                if (n == -1) break; /* outside board */

                                if (color[n] != (int)Color.NONE)
                                {
                                    if (color[n] == xside)
                                        mouvementPossible.Add(genMove(i, n, 1)); /* capture from i to n */
                                    break;
                                }
                                mouvementPossible.Add(genMove(i, n, 0)); /* quiet move from i to n */
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
                            //Double "saut"
                            if (Mailbox.tabPos[i] <= 88 && Mailbox.tabPos[i] >= 81)
                            {
                                if (color[i + Mailbox.MMPAWN[3]] == (int)Color.NONE)
                                {
                                    mouvementPossible.Add(genMove(i, i + Mailbox.MMPAWN[3], 0));
                                }
                            }

                            //Avancer
                            if (color[i + Mailbox.MMPAWN[0]] == (int)Color.NONE)
                            {
                                mouvementPossible.Add(genMove(i, i + Mailbox.MMPAWN[0], 0));
                            }

                            //Manger Gauche

                            if (color[i + Mailbox.MMPAWN[1]] == (int)Color.BLACK)
                            {
                                mouvementPossible.Add(genMove(i, i + Mailbox.MMPAWN[1], 1));
                            }

                            //Manger droite

                            if (color[i + Mailbox.MMPAWN[2]] == (int)Color.BLACK)
                            {
                                mouvementPossible.Add(genMove(i, i + Mailbox.MMPAWN[2], 1));
                            }

                        }
                        else //side == dark
                        {
                            //Double "saut"
                            if (Mailbox.tabPos[i] >= 31 && Mailbox.tabPos[i] <= 38)
                            {

                                if (color[i - Mailbox.MMPAWN[3]] == (int)Color.NONE)
                                {
                                    mouvementPossible.Add(genMove(i, i - Mailbox.MMPAWN[3], 0));
                                }
                            }

                            //Avancer

                            if (color[i - Mailbox.MMPAWN[0]] == (int)Color.NONE)
                            {
                                mouvementPossible.Add(genMove(i, i - Mailbox.MMPAWN[0], 0));
                            }

                            //Manger Gauche

                            if (color[i - Mailbox.MMPAWN[1]] == (int)Color.WHITE)
                            {
                                mouvementPossible.Add(genMove(i, i - Mailbox.MMPAWN[1], 1));
                            }

                            //Manger droite

                            if (color[i - Mailbox.MMPAWN[2]] == (int)Color.WHITE)
                            {
                                mouvementPossible.Add(genMove(i, i - Mailbox.MMPAWN[2], 1));
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
            string dp = Mailbox.tabCoord[i];
            string arr = Mailbox.tabCoord[n];

            return Ply.Position(new Case(dp), new Case(arr));

            //int valeur = evaluateBoard(LIGHT);

            //Console.WriteLine("Mouvement possible {2} : {0}, {1}", dp, arr, piece[i]);
        }
    }
}
