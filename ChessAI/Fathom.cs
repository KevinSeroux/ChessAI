using System;
using System.Runtime.InteropServices;

namespace ChessAI
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    class FathomLoadException : SyzygyLoadException {
        public FathomLoadException() : base(
            "Fathom DLL loaded but unable to init"
        ) {}
    }
    class FathomNoTableException : SyzygyLoadException {
        public FathomNoTableException(String path) : base(
            "Fathom DLL loaded, init done but no tables found in folder: " + path
        ) { }
    }

    class Fathom
    {
        public const UInt32 TB_MAX_MOVES = 192 + 1;

        public const UInt32 TB_CASTLING_K = 0x1; /* White king-side. */
        public const UInt32 TB_CASTLING_Q = 0x2; /* White queen-side. */
        public const UInt32 TB_CASTLING_k = 0x4; /* Black king-side. */
        public const UInt32 TB_CASTLING_q = 0x8; /* Black queen-side. */


        public const UInt32 TB_LOSS = 0;         /* LOSS */
        public const UInt32 TB_BLESSED_LOSS = 1; /* LOSS but 50-move draw */
        public const UInt32 TB_DRAW = 2;         /* DRAW */
        public const UInt32 TB_CURSED_WIN = 3;   /* WIN but 50-move draw  */
        public const UInt32 TB_WIN = 4;          /* WIN  */

        public const UInt32 TB_PROMOTES_NONE = 0;
        public const UInt32 TB_PROMOTES_QUEEN = 1;
        public const UInt32 TB_PROMOTES_ROOK = 2;
        public const UInt32 TB_PROMOTES_BISHOP = 3;
        public const UInt32 TB_PROMOTES_KNIGHT = 4;

        public static UInt32 TB_RESULT_CHECKMATE = SetWDL(0, TB_WIN);
        public static UInt32 TB_RESULT_STALEMATE = SetWDL(0, TB_DRAW);
        public static UInt32 TB_RESULT_FAILED = 0xFFFFFFFF;

        private static UInt32 TB_RESULT_WDL_MASK = 0x0000000F;
        private static UInt32 TB_RESULT_TO_MASK = 0x000003F0;
        private static UInt32 TB_RESULT_FROM_MASK = 0x0000FC00;
        private static UInt32 TB_RESULT_PROMOTES_MASK = 0x00070000;
        private static UInt32 TB_RESULT_EP_MASK = 0x00080000;
        private static UInt32 TB_RESULT_DTZ_MASK = 0xFFF00000;
        private static int TB_RESULT_WDL_SHIFT = 0;
        private static int TB_RESULT_TO_SHIFT = 4;
        private static int TB_RESULT_FROM_SHIFT = 10;
        private static int TB_RESULT_PROMOTES_SHIFT = 16;
        private static int TB_RESULT_EP_SHIFT = 19;
        private static int TB_RESULT_DTZ_SHIFT = 20;

        private static UInt32 GetSomething(UInt32 res, UInt32 mask, int shift)
        {
            return (((res) & mask) >> shift);
        }

        public static UInt32 GetWDL(UInt32 res)
        {
            return GetSomething(res, TB_RESULT_WDL_MASK, TB_RESULT_WDL_SHIFT);
        }

        public static UInt32 GetTo(UInt32 res)
        {
            return GetSomething(res, TB_RESULT_TO_MASK, TB_RESULT_TO_SHIFT);
        }

        public static UInt32 GetFrom(UInt32 res)
        {
            return GetSomething(res, TB_RESULT_FROM_MASK, TB_RESULT_FROM_SHIFT);
        }

        public static UInt32 GetPromotes(UInt32 res)
        {
            return GetSomething(res, TB_RESULT_PROMOTES_MASK, TB_RESULT_PROMOTES_SHIFT);
        }

        public static UInt32 GetEP(UInt32 res)
        {
            return GetSomething(res, TB_RESULT_EP_MASK, TB_RESULT_EP_SHIFT);
        }

        public static UInt32 GetDTZ(UInt32 res)
        {
            return GetSomething(res, TB_RESULT_DTZ_MASK, TB_RESULT_DTZ_SHIFT);
        }

        private static UInt32 SetSomething(UInt32 res, UInt32 wdl, UInt32 mask, int shift)
        {
            return
                (((res) & ~mask) |
                (((wdl) << shift) & mask));
        }

        private static UInt32 SetWDL(UInt32 res, UInt32 wdl)
        {
            return SetSomething(res, wdl, TB_RESULT_WDL_MASK, TB_RESULT_WDL_SHIFT);
        }

        private static UInt32 SetTo(UInt32 res, UInt32 wdl)
        {
            return SetSomething(res, wdl, TB_RESULT_TO_MASK, TB_RESULT_TO_SHIFT);
        }

        private static UInt32 SetFrom(UInt32 res, UInt32 wdl)
        {
            return SetSomething(res, wdl, TB_RESULT_FROM_MASK, TB_RESULT_FROM_SHIFT);
        }

        private static UInt32 SetPromotes(UInt32 res, UInt32 wdl)
        {
            return SetSomething(res, wdl, TB_RESULT_PROMOTES_MASK, TB_RESULT_PROMOTES_SHIFT);
        }

        private static UInt32 SetEP(UInt32 res, UInt32 wdl)
        {
            return SetSomething(res, wdl, TB_RESULT_EP_MASK, TB_RESULT_EP_SHIFT);
        }

        private static UInt32 SetDTZ(UInt32 res, UInt32 wdl)
        {
            return SetSomething(res, wdl, TB_RESULT_DTZ_MASK, TB_RESULT_DTZ_SHIFT);
        }


        private IntPtr dll;
        private IntPtr ptr_TB_LARGEST;

        /*
        * Initialize the tablebase.
        *
        * PARAMETERS:
        * - path:
        *   The tablebase PATH string.
        *
        * RETURN:
        * - The TB_LARGEST global will also be initialized. 
        *   If no tablebase files are found, TB_LARGEST is set to zero.
        */
        public Fathom(String path)
        {
            // Get a reference to TB_LARGEST global
            dll = NativeMethods.LoadLibrary("fathom.dll");
            ptr_TB_LARGEST = NativeMethods.GetProcAddress(dll, "TB_LARGEST");
            NativeMethods.FreeLibrary(dll);

            if (!tb_init(path))
                throw new FathomLoadException();
            if (Largest == 0)
                throw new FathomNoTableException(path);
        }

        [DllImport("fathom.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool tb_init(String path);
        
        /*
        * The tablebase can be probed for any position where #pieces <= TB_LARGEST.
        */
        public UInt32 Largest
        {
            get { return (UInt32)Marshal.ReadInt32(ptr_TB_LARGEST); }
        }

        /*
         * Probe the Win-Draw-Loss (WDL) table.
         *
         * PARAMETERS:
         * - white, black, kings, queens, rooks, bishops, knights, pawns:
         *   The current position (bitboards).
         * - rule50:
         *   The 50-move half-move clock.
         * - castling:
         *   Castling rights.  Set to zero if no castling is possible.
         * - ep:
         *   The en passant square (if exists).  Set to zero if there is no en passant
         *   square.
         * - turn:
         *   true=white, false=black
         *
         * RETURN:
         * - One of {TB_LOSS, TB_BLESSED_LOSS, TB_DRAW, TB_CURSED_WIN, TB_WIN}.
         *   Otherwise returns TB_RESULT_FAILED if the probe failed.
         *
         * NOTES:
         * - Engines should use this function during search.
         * - This function is thread safe assuming TB_NO_THREADS is disabled.
         */
        public UInt32 ProbeWDL(
            UInt64 white,
            UInt64 black,
            UInt64 kings,
            UInt64 queens,
            UInt64 rooks,
            UInt64 bishops,
            UInt64 knights,
            UInt64 pawns,
            UInt32 rule50,
            UInt32 castling,
            UInt32 ep,
            bool turn,
            UIntPtr results
        )
        {
            return tb_probe_wdl(
                white,
                black,
                kings,
                queens,
                rooks,
                bishops,
                knights,
                pawns,
                rule50,
                castling,
                ep,
                turn
            );
        }

        [DllImport("fathom.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 tb_probe_wdl(
            UInt64 white,
            UInt64 black,
            UInt64 kings,
            UInt64 queens,
            UInt64 rooks,
            UInt64 bishops,
            UInt64 knights,
            UInt64 pawns,
            UInt32 rule50,
            UInt32 castling,
            UInt32 ep,
            bool turn
        );

        /*
        * Probe the Distance-To-Zero (DTZ) table.
        *
        * PARAMETERS:
        * - white, black, kings, queens, rooks, bishops, knights, pawns:
        *   The current position (bitboards).
        * - rule50:
        *   The 50-move half-move clock.
        * - castling:
        *   Castling rights.  Set to zero if no castling is possible.
        * - ep:
        *   The en passant square (if exists).  Set to zero if there is no en passant
        *   square.
        * - turn:
        *   true=white, false=black
        * - results (OPTIONAL):
        *   Alternative results, one for each possible legal move.  The passed array
        *   must be TB_MAX_MOVES in size.
        *   If alternative results are not desired then set results=NULL.
        *
        * RETURN:
        * - A TB_RESULT value comprising:
        *   1) The WDL value (TB_GET_WDL)
        *   2) The suggested move (TB_GET_FROM, TB_GET_TO, TB_GET_PROMOTES, TB_GET_EP)
        *   3) The DTZ value (TB_GET_DTZ)
        *   The suggested move is guaranteed to preserved the WDL value.
        *
        *   Otherwise:
        *   1) TB_RESULT_STALEMATE is returned if the position is in stalemate.
        *   2) TB_RESULT_CHECKMATE is returned if the position is in checkmate.
        *   3) TB_RESULT_FAILED is returned if the probe failed.
        *
        *   If results!=NULL, then a TB_RESULT for each legal move will be generated
        *   and stored in the results array.  The results array will be terminated
        *   by TB_RESULT_FAILED.
        *
        * NOTES:
        * - Engines can use this function to probe at the root.  This function should
        *   not be used during search.
        * - DTZ tablebases can suggest unnatural moves, especially for losing
        *   positions.  Engines may prefer to traditional search combined with WDL
        *   move filtering using the alternative results array.
        * - This function is NOT thread safe.  For engines this function should only
        *   be called once at the root per search.
        */
        public UInt32 ProbeRoot(
            UInt64 white,
            UInt64 black,
            UInt64 kings,
            UInt64 queens,
            UInt64 rooks,
            UInt64 bishops,
            UInt64 knights,
            UInt64 pawns,
            UInt32 rule50,
            UInt32 castling,
            UInt32 ep,
            bool turn,
            UIntPtr results
        )
        {
            return tb_probe_root(
                white,
                black,
                kings,
                queens,
                rooks,
                bishops,
                knights,
                pawns,
                rule50,
                castling,
                ep,
                turn,
                results
            );
        }

        [DllImport("fathom.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 tb_probe_root(
            UInt64 white,
            UInt64 black,
            UInt64 kings,
            UInt64 queens,
            UInt64 rooks,
            UInt64 bishops,
            UInt64 knights,
            UInt64 pawns,
            UInt32 rule50,
            UInt32 castling,
            UInt32 ep,
            bool turn,
            UIntPtr results
        );
    }
}
