using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class MailboxRepresentation
    {
        public const int LIGHT = 0;
        public const int DARK = 1;
        public const int EMPTY = 6;

        public const int PAWN = 0;
        public const int KNIGHT = 1;
        public const int BISHOP = 2;
        public const int ROOK = 3;
        public const int QUEEN = 4;
        public const int KING = 5;

        public readonly static int[] offsets = { 0, 8, 4, 4, 8, 8 }; // NB Direction pour chaque pièce

        public readonly static bool[] slide = { false, false, true, true, true, false };

        int[] color = {   1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        6, 6, 6, 6, 6, 6, 6, 6,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0
            };// = new int[64]; /* LIGHT, DARK, or EMPTY */


        int[] piece = {
                3, 1, 2, 4, 5, 2, 1, 3,
                0, 0, 0, 0, 0, 0, 0, 0,
                6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6,
                6, 6, 6, 6, 6, 6, 6, 6,
                0, 0, 0, 0, 0, 0, 0, 0,
                3, 1, 2, 4, 5, 2, 1, 3
            };// = new int[64]; /* PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING, or EMPTY */

        

        private uint castle;
        private uint ep;

        int side; // side to move
        int xside; //side to not move

        //representation Max
        public readonly static int[] MMPAWN = { -8, -9, -7, -16 }; //Pion
        /*int[] MMBISHOP = { 11, -11, -9, 9 }; // Fou ---------------- TODO ajouter toute les combinaisons ou faire un modulo
        int[] MMROOK = { -1, 1, -8, 8 }; // Tour
        int[] MMPKNIGHT = { -17, -15, -6, 10, 17, 15, 6, -10 };//Cavalier
        int[] MMQUEEN = { -1, 1, -8, 8, -11, 11, 9, -9 }; // Reine
        int[] MMKING = { -1, 1, -8, 8 }; //Roi*/

        public readonly static int[,] offset = {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
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

        public readonly static string[] tabCoord =  {  "a8","b8","c8","d8","e8","f8","g8","h8",
                                "a7","b7","c7","d7","e7","f7","g7","h7",
                                "a6","b6","c6","d6","e6","f6","g6","h6",
                                "a5","b5","c5","d5","e5","f5","g5","h5",
                                "a4","b4","c4","d4","e4","f4","g4","h4",
                                "a3","b3","c3","d3","e3","f3","g3","h3",
                                "a2","b2","c2","d2","e2","f2","g2","h2",
                                "a1","b1","c1","d1","e1","f1","g1","h1" };



        public MailboxRepresentation()
        {
            side = LIGHT;
            xside = DARK;
        }

        public void changementTour()
        {
            int tourprecedent = side;
            side = xside;
            xside = tourprecedent;
        }

        public int[] getColor()
        {
            return color;
        }

        public int[] getPiece()
        {
            return piece;
        }

        public int getSideToPlay()
        {
            return side;
        }

        public int getSideNotToPlay()
        {
            return xside;
        }

        public int ligne(int index)
        {
            return index >> 3;
        }

        public int colonne(int index)
        {
            return index & 7;
        }
    }
}
