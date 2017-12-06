using System;

namespace ChessAI
{
    struct Bitboard
    {
        // MSB at h8, LSB at a1
        // Exemple: king: 0x0000000000000010 means a king at e1
        public UInt64 white;
        public UInt64 black;
        public UInt64 kings;
        public UInt64 queens;
        public UInt64 rooks;
        public UInt64 bishops;
        public UInt64 knights;
        public UInt64 pawns;

        public UInt32 castling;
        public UInt32 ep; // "En Passant"
        public bool turn; // true: white, false: black
    }
}
