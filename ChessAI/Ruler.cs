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
            if (board.GetMailbox().check)
            {
                return withCheckPly();
            }
            return withoutCheckPly();
            //return new HashSet<Ply>();
        }

        private ICollection<Ply> withCheckPly()
        {

        }

        private ICollection<Ply> withoutCheckPly()
        {
            List<Ply> mouvementPossible = new List<Ply>();
            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = (int)board.GetTurn;
            int xside = (int)(board.GetTurn == Color.WHITE ? Color.BLACK : Color.WHITE);


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

                                if (color[n] != (int)Color.NONE && color[n] != (int)Color.PAWN_EN_PASSANT)
                                //if (color[n] != (int)Color.NONE)
                                {
                                    if (color[n] == xside)
                                        mouvementPossible.Add(genMove(i, n, 1, -1)); /* capture from i to n */
                                    break;
                                }
                                mouvementPossible.Add(genMove(i, n, 0, -1)); /* quiet move from i to n */
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

                                    if ((color[n] == (int)Color.NONE) && (Mailbox.tabPos[n + Mailbox.offset[0, 0]] == (int)Color.NONE))
                                    {
                                        int caseEnPassant = Mailbox.tabPos[n + Mailbox.offset[0, 0]];
                                        mouvementPossible.Add(genMove(i, n, 0, caseEnPassant));
                                    }
                                }
                            }
                            //Avancer
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (Mailbox.tabPos[n] == (int)Color.NONE)
                                {
                                    mouvementPossible.Add(genMove(i, n, 0, -1));
                                }
                            }

                            //Manger Gauche
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    mouvementPossible.Add(genMove(i, n, 1, -1));
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.BLACK)
                                {
                                    mouvementPossible.Add(genMove(i, n, 1, -1));
                                }
                            }

                        }
                        else //side == dark
                        {
                            //Double "saut"
                            int n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 3]];
                            if (n != -1)
                            {
                                if (Mailbox.tabPos[i] >= 31 && Mailbox.tabPos[i] <= 38)
                                {

                                    if (color[n] == (int)Color.NONE && (Mailbox.tabPos[n - Mailbox.offset[0, 0]] == (int)Color.NONE))
                                    {
                                        int caseEnPassant = Mailbox.tabPos[n - Mailbox.offset[0, 0]];
                                        mouvementPossible.Add(genMove(i, n, 0, caseEnPassant));
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.NONE)
                                {
                                    mouvementPossible.Add(genMove(i, n, 0, -1));
                                }
                            }

                            //Manger Gauche
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE || color[n] == (int)Color.PAWN_EN_PASSANT)
                                {
                                    mouvementPossible.Add(genMove(i, n, 1, -1));
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (color[n] == (int)Color.WHITE || color[n] == (int)Color.PAWN_EN_PASSANT)
                                {
                                    mouvementPossible.Add(genMove(i, n, 1, -1));
                                }
                            }
                        }
                    }
                }
            }


            return mouvementPossible;
        }

        private Ply genMove(int depart, int arrivee, int v, int enPassant)
        {
            string dp = Mailbox.tabCoord[depart];
            string arr = Mailbox.tabCoord[arrivee];


            Ply p = null;

            //Generation d'une case EN_PASSANT car double saut
            if(v == 0 && enPassant != -1)
            {
                string ep = Mailbox.tabCoord[enPassant];
                p = Ply.EnPassant(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr), Case.BuildCaseMailBox(enPassant, ep), false);
            }
            //On mange un pion via la case en Passant
            else if(v == 1 && board.GetMailbox().getColor()[arrivee] == (int)Color.PAWN_EN_PASSANT)
            {
                string ep = Mailbox.tabCoord[enPassant];
                p = Ply.EnPassant(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr), Case.BuildCaseMailBox(enPassant, ep), true);
            }
            //Generation d'un mouvement normal
            else
            {
                p = Ply.Position(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr));
            } 

            return p;
           

            //return Ply.Position(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr));
            //int valeur = evaluateBoard(LIGHT);

            //Console.WriteLine("Mouvement possible {2} : {0}, {1}", dp, arr, piece[i]);
        }
    }
}
