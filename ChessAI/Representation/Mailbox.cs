using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Mailbox
    {

        public readonly static int[] offsets = { 4, 8, 4, 4, 8, 8 }; // NB Direction pour chaque pièce

        public readonly static bool[] slide = { false, false, true, true, true, false };

        int[] color;
        // = new int[64]; /* LIGHT, DARK, or EMPTY */


        int[] piece;
        // = new int[64]; /* PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING, or EMPTY */

        

        private uint castle;

        private int? ep;
        


        //representation Max
        public readonly static int[] MMPAWN = { -8, -9, -7, -16 }; //Pion
        /*int[] MMBISHOP = { 11, -11, -9, 9 }; // Fou ---------------- TODO ajouter toute les combinaisons ou faire un modulo
        int[] MMROOK = { -1, 1, -8, 8 }; // Tour
        int[] MMPKNIGHT = { -17, -15, -6, 10, 17, 15, 6, -10 };//Cavalier
        int[] MMQUEEN = { -1, 1, -8, 8, -11, 11, 9, -9 }; // Reine
        int[] MMKING = { -1, 1, -8, 8 }; //Roi*/

        public readonly static int[,] offset = {
                { -10, -11, -9, -20, 0, 0, 0, 0 },
                { -21, -19, -12, -8, 8, 12, 19, 21 },
                { -11, -9, 9, 11, 0, 0, 0, 0 },
                { -10, -1, 1, 10, 0, 0, 0, 0 },
                { -11, -10, -9, -1, 1, 9, 10, 11 },
                { -11, -10, -9, -1, 1, 9, 10, 11 }
        };

        //Tableau permettant de calculer les déplacements avec les vecteurs de déplacement des pièces
        public readonly static int[] tabPos = {   21, 22, 23, 24, 25, 26, 27, 28,
                            31, 32, 33, 34, 35, 36, 37, 38,
                            41, 42, 43, 44, 45, 46, 47, 48,
                            51, 52, 53, 54, 55, 56, 57, 58,
                            61, 62, 63, 64, 65, 66, 67, 68,
                            71, 72, 73, 74, 75, 76, 77, 78,
                            81, 82, 83, 84, 85, 86, 87, 88,
                            91, 92, 93, 94, 95, 96, 97, 98 };

        //Permet de calculer les déplacement en évitant les sorties de tableau
        public readonly static int[] tab120 =  {  - 1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1,  0,  1,  2,  3,  4,  5,  6,  7, -1,
                            -1,  8,  9, 10, 11, 12, 13, 14, 15, -1,
                            -1, 16, 17, 18, 19, 20, 21, 22, 23, -1,
                            -1, 24, 25, 26, 27, 28, 29, 30, 31, -1,
                            -1, 32, 33, 34, 35, 36, 37, 38, 39, -1,
                            -1, 40, 41, 42, 43, 44, 45, 46, 47, -1,
                            -1, 48, 49, 50, 51, 52, 53, 54, 55, -1,
                            -1, 56, 57, 58, 59, 60, 61, 62, 63, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        public readonly static string[] tabCoord =  {
                                "a8","b8","c8","d8","e8","f8","g8","h8",
                                "a7","b7","c7","d7","e7","f7","g7","h7",
                                "a6","b6","c6","d6","e6","f6","g6","h6",
                                "a5","b5","c5","d5","e5","f5","g5","h5",
                                "a4","b4","c4","d4","e4","f4","g4","h4",
                                "a3","b3","c3","d3","e3","f3","g3","h3",
                                "a2","b2","c2","d2","e2","f2","g2","h2",
                                "a1","b1","c1","d1","e1","f1","g1","h1" };



        public Mailbox()
        {
            piece = new int[]{
                3, 1, 2, 4, 5, 2, 1, 3,
                0, 0, 0, 0, 0, 0, 0, 0,
                6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6,
                0, 0, 0, 0, 0, 0, 0, 0,
                3, 1, 2, 4, 5, 2, 1, 3
            };

            color = new int[] {
                        1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        -1, -1, -1, -1, -1, -1, -1, -1,
                        -1, -1, -1, -1, -1, -1, -1, -1
            };

            ep = null;
        }

        public uint CountMen
        {
            get
            {
                uint count = 0;

                for (uint i = 0; i < piece.Length; i++)
                    if (piece[i] != 6)
                        ++count;

                return count;
            }
        }

        public int[] getColor()
        {
            return color;
        }

        public int[] getPiece()
        {
            return piece;
        }

        public int ligne(int index)
        {
            return index >> 3;
        }

        public int colonne(int index)
        {
            return index & 7;
        }

        public Mailbox(Mailbox m)
        {
            this.color = new int[64];
            this.piece = new int[64];

            Array.Copy(m.piece, piece,64);
            Array.Copy(m.color, color, 64);

            this.ep = m.ep;

        }

        public Mailbox(int[] tabVal)
        {
            this.piece = new int[64];
            this.color = new int[64];

            for (int i = 0; i < tabVal.Length; i++)
            {
                int val = tabVal[i];

                int col;
                if (val == 10 || val == -10)
                    col = (int)Color.PAWN_EN_PASSANT;
                else if (val < 0)
                    col = (int)Color.BLACK;
                else if (val == 0)
                    col = (int)Color.NONE;
                else
                    col = (int)Color.WHITE;
                color[i] = col;

                int curPiece;
                switch (Math.Abs(val))
                {
                    case 1:
                        curPiece = (int)Piece.PAWN;
                        break;

                    case 10: // TODO En passant
                        //curPiece = (int)Color.PAWN_EN_PASSANT; //TODO Rename to Piece
                        curPiece = (int)Color.NONE;

                        Debug.Assert(ep.HasValue, "Mailbox conversion : Il y a plusieurs pion en passant sur le board! IMPOSSIBLE NON ?");
                        //Il faut trouver le pion associé
                        if(tabPos[i] >= 41 && tabPos[i] <= 48)
                        {
                            //Noir
                            this.ep = Mailbox.tab120[Mailbox.tabPos[i] - Mailbox.offset[0, 0]];
                            break;
                        }
                        if(tabPos[i] >=71 && tabPos[i] <= 78)
                        {
                            //blanc
                            this.ep = Mailbox.tab120[Mailbox.tabPos[i] + Mailbox.offset[0, 0]];
                            break;
                        }

                        break;

                    case 21:
                    case 22:
                        curPiece = (int)Piece.ROOK;
                        break;

                    case 31:
                    case 32:
                        curPiece = (int)Piece.KNIGHT;
                        break;

                    case 4:
                        curPiece = (int)Piece.BISHOP;
                        break;

                    case 5:
                        curPiece = (int)Piece.QUEEN;
                        break;

                    case 6:
                        curPiece = (int)Piece.KING;
                        break;

                    default:
                        Debug.Assert(true, val + " unknown as piece type");
                        curPiece = (int)Color.NONE;
                        break;
                }
                piece[i] = curPiece;
            }
        }

        /*
         * private bool castlingKingSide;
        private bool castlingQueenSide;
        private Case from;
        private Case to;
        private Promotion promotion;
        private Case captureEnPassant;
        */
        public void ply(Ply p)
        {
            int? testdep = p.from.getPosMailBox();
            int? testarr = p.to.getPosMailBox();

            int dep = 0;
            int arr = 0;

            if(testdep.HasValue && testarr.HasValue)
            {
                dep = p.from.getPosMailBox();
                arr = p.to.getPosMailBox();
            }
            else
            {
                for (int i = 0; i < tabCoord.Length; i++)
                {
                    if (p.from.ToString().Equals(tabCoord[i])) dep = i;
                    if (p.to.ToString().Equals(tabCoord[i])) arr = i;
                }
            }
            

            color[arr] = color[dep];
            color[dep] = (int)Color.NONE;

            piece[arr] = piece[dep];
            piece[dep] = (int)Color.NONE;

            //TODO
            //C'est un double saut qui genere une case en passant
            if(p.captureEnPassant != null && !p.captureEP)
            {
                //Si EP est deja set, c'est celui du tour d'avant donc on peut l'ecraser
                int caseEnPassant = p.captureEnPassant.getPosMailBox();
                color[caseEnPassant] = (int)Color.PAWN_EN_PASSANT;
                ep = arr;
            } else if(p.captureEnPassant != null && p.captureEP)
            {
                Debug.Assert(ep.HasValue, "On mange un pion via EP mais aucuns pions associe à la piece");
                //On mange un pion via la case en passant
                color[ep.Value] = (int)Color.NONE;
                piece[ep.Value] = (int)Color.NONE;
                ep = null;

            }

            if(p.captureEnPassant == null && this.ep.HasValue)
            {
                //Si on fait un mouvement et qu'il y a un EP alors on reset le EP (il est valable qu'un tour)
                int? caseEnPassant = null;
                //Il faut trouver le pion associé
                if (tabPos[ep.Value] >= 41 && tabPos[ep.Value] <= 48)
                {
                    //Noir
                    caseEnPassant = Mailbox.tab120[Mailbox.tabPos[ep.Value] + Mailbox.offset[0, 0]];
                }
                else if (tabPos[ep.Value] >= 71 && tabPos[ep.Value] <= 78)
                {
                    //blanc
                    caseEnPassant = Mailbox.tab120[Mailbox.tabPos[ep.Value] - Mailbox.offset[0, 0]];
                }

                Debug.Assert(caseEnPassant.HasValue, "Il y a une case en passant qui n'est pas au bon endroit " + tabCoord[ep.Value]);

                color[caseEnPassant.Value] = (int)Color.NONE;

                ep = null;

            }


            if(p.promotion != null)
            {
                piece[arr] = p.promotion.piece;
            }

            if (p.castlingKingSide)
            {
                //TODO -- qqch à faire la ? Vu qu'on a depart et arrivé ?
            }
            if (p.castlingQueenSide)
            {
                //TODO -- qqch à faire la ? Vu qu'on a depart et arrivé ?
            }


        }
    }
}
