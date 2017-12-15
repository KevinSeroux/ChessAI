using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
        //Première version, il faudrait regarder la methode "attaque" -> lien https://stackoverflow.com/questions/16803893/chess-getting-all-legal-chess-moves
        //Refaire avec un unply plutot qu'avec une copie de la MailBox
        public ICollection<Ply> GetPossiblePlies()
        {
            List<Ply> legalMove = new List<Ply>();
            if (board.GetMailbox().endGame)
                return legalMove;

            Mailbox temp = new Mailbox(board.GetMailbox());
            Mailbox.Check c;
            foreach(Ply pseudo in generatePeudoLegalsMoves())
            {
                temp.testPly(pseudo);
                c = temp.testCheck(board.GetTurn);
                if (c == Mailbox.Check.NONE)
                {
                    legalMove.Add(pseudo);
                }else if(c == Mailbox.Check.CHECKMATE)
                {
                    pseudo.lastPly = true;
                    legalMove.Add(pseudo);
                }
                temp.testUnPly();
            }
            

            return legalMove;
            //return new HashSet<Ply>();
        }

        private ICollection<Ply> generatePeudoLegalsMoves()
        {
            List<Ply> mouvementPossible = new List<Ply>();
            int[] color = board.GetMailbox().getColor();
            int[] piece = board.GetMailbox().getPiece();
            int side = (int)board.GetTurn;
            int xside = (int)(board.GetTurn == Color.WHITE ? Color.BLACK : Color.WHITE);
            int countPieceNoir = board.GetMailbox().countPieceNoir;
            int countPieceBlanche = board.GetMailbox().countPieceBlanche;

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
                                    int caseEnPassant = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 0]];
                                    if ((color[n] == (int)Color.NONE) && (color[caseEnPassant] == (int)Color.NONE))
                                    {
                                        mouvementPossible.Add(genMove(i, n, 0, caseEnPassant));
                                    }
                                }
                            }
                            //Avancer
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if(n<=7 && n>=0) //Dernière ligne
                                {
                                    if (color[n] == (int)Color.NONE)
                                        if (countPieceBlanche != Mailbox.initialCount)
                                            mouvementPossible.Add(genMovePromotion(i, n, Color.WHITE));
                                }
                                else if (color[n] == (int)Color.NONE || color[n] == (int)Color.PAWN_EN_PASSANT)
                                {
                                    mouvementPossible.Add(genMove(i, n, 0, -1));
                                }
                            }

                            //Manger Gauche
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (n <= 7 && n >= 0) //Dernière ligne
                                {
                                    if (color[n] == (int)Color.BLACK )
                                        if (countPieceBlanche != Mailbox.initialCount)
                                            mouvementPossible.Add(genMovePromotion(i, n, Color.WHITE));
                                }
                                else
                                if (color[n] == (int)Color.BLACK || (color[n] == (int)Color.PAWN_EN_PASSANT && n < 40))
                                {
                                    mouvementPossible.Add(genMove(i, n, 1, -1));
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (n <= 7 && n >= 0) //Dernière ligne
                                {
                                    if (color[n] == (int)Color.BLACK)
                                        if (countPieceBlanche != Mailbox.initialCount)
                                            mouvementPossible.Add(genMovePromotion(i, n, Color.WHITE));
                                }
                                else
                                if (color[n] == (int)Color.BLACK || (color[n] == (int)Color.PAWN_EN_PASSANT && n < 40))
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
                                    int caseEnPassant = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 0]];
                                    if (color[n] == (int)Color.NONE && (color[caseEnPassant]== (int)Color.NONE))
                                    {
                                        mouvementPossible.Add(genMove(i, n, 0, caseEnPassant));
                                    }
                                }
                            }

                            //Avancer
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 0]];
                            if (n != -1)
                            {
                                if (n <= 63 && n >= 56) //Dernière ligne
                                {
                                    if (color[n] == (int)Color.NONE)
                                    {
                                        if (countPieceNoir != Mailbox.initialCount)
                                            mouvementPossible.Add(genMovePromotion(i, n, Color.BLACK));
                                    }
                                }
                                else
                                   if (color[n] == (int)Color.NONE || color[n] == (int)Color.PAWN_EN_PASSANT)
                                {
                                    mouvementPossible.Add(genMove(i, n, 0, -1));
                                }
                            }

                            //Manger Gauche
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 1]];
                            if (n != -1)
                            {
                                if (n <= 63 && n >= 56) //Dernière ligne
                                {
                                    if (color[n] == (int)Color.WHITE)
                                    {
                                        if (countPieceNoir != Mailbox.initialCount)
                                            mouvementPossible.Add(genMovePromotion(i, n,Color.BLACK));
                                    }
                                }
                                else
                                if (color[n] == (int)Color.WHITE || (color[n] == (int)Color.PAWN_EN_PASSANT && n > 23))
                                {
                                    mouvementPossible.Add(genMove(i, n, 1, -1));
                                }
                            }

                            //Manger droite
                            n = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 2]];
                            if (n != -1)
                            {
                                if (n <= 63 && n >= 56) //Dernière ligne
                                {
                                    if (color[n] == (int)Color.WHITE)
                                    {
                                        if (countPieceNoir != Mailbox.initialCount)
                                            mouvementPossible.Add(genMovePromotion(i, n, Color.BLACK));
                                    }
                                }
                                else
                                if (color[n] == (int)Color.WHITE || (color[n] == (int)Color.PAWN_EN_PASSANT && n > 23))
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
            if (enPassant != -1)
            {
                if (v == 0)
                {
                    string ep = Mailbox.tabCoord[enPassant];
                    p = Ply.EnPassant(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr), Case.BuildCaseMailBox(enPassant, ep), false);
                    return p;
                }
                //On mange un pion via la case en Passant
                else if (v == 1 && board.GetMailbox().getColor()[arrivee] == (int)Color.PAWN_EN_PASSANT)
                {
                    string ep = Mailbox.tabCoord[enPassant];
                    p = Ply.EnPassant(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr), Case.BuildCaseMailBox(enPassant, ep), true);
                    return p;
                }
            }
            //Generation d'un mouvement normal
            
             p = Ply.Position(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr));
            

            return p;
           

            //return Ply.Position(Case.BuildCaseMailBox(depart, dp), Case.BuildCaseMailBox(arrivee, arr));
            //int valeur = evaluateBoard(LIGHT);

            //Console.WriteLine("Mouvement possible {2} : {0}, {1}", dp, arr, piece[i]);
        }

        private Ply genMovePromotion(int depart, int arrivee, Color c)
        {
            string dp = Mailbox.tabCoord[depart];
            Case from = Case.BuildCaseMailBox(depart, dp);
            string arr = Mailbox.tabCoord[arrivee];
            Case to = Case.BuildCaseMailBox(depart, arr);

            return Ply.PromoteQueen(from, to);
        }
    }
}
