﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Diagnostics;

namespace ChessAI
{
    class Program
    {
        private static Chessboard board;
        private static Strategist strategist;

        static void Init()
        {
            // Dependencies injection
            board = new Chessboard();
            Syzygy tableReader = new Syzygy(board, "data");
            Evaluator evaluator = new Evaluator(board);
            strategist = new Strategist(board, evaluator, tableReader);
        }

        static void Main(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif

            if (args.Length != 1)
            {
                Console.WriteLine("Incorrect count of arguments.");
                Console.WriteLine("1 required and you have specified " + args.Length);
                Console.WriteLine("Did you forgot to specify 'white' or 'black' in the arguments?");
                Console.WriteLine("Press enter to exit");
                Console.ReadKey();
                return;
            }

            Chessboard.Turn turn;

            if (args[0] == "white")
                turn = Chessboard.Turn.WHITE;
            else if (args[0] == "black")
                turn = Chessboard.Turn.BLACK;
            else
            {
                Console.WriteLine("\'" + args[0] + "\' argument is unknown.");
                Console.WriteLine("Please choose between 'white' or 'black'");
                Console.WriteLine("Press enter to exit");
                Console.ReadKey();
                return;
            }

            try
            {
                Init();

                bool stop = false;
                int[] tabVal = new int[64];
                String value;
                String[] coord = new String[] { "", "", "" };
                String[] tabCoord = new string[] { "a8","b8","c8","d8","e8","f8","g8","h8",
                                                "a7","b7","c7","d7","e7","f7","g7","h7",
                                                "a6","b6","c6","d6","e6","f6","g6","h6",
                                                "a5","b5","c5","d5","e5","f5","g5","h5",
                                                "a4","b4","c4","d4","e4","f4","g4","h4",
                                                "a3","b3","c3","d3","e3","f3","g3","h3",
                                                "a2","b2","c2","d2","e2","f2","g2","h2",
                                                "a1","b1","c1","d1","e1","f1","g1","h1" };

                while (!stop)
                {
                    using (var mmf = MemoryMappedFile.OpenExisting("plateau"))
                    {
                        using (var mmf2 = MemoryMappedFile.OpenExisting("repAI2"))
                        {
                            Mutex mutexStartAI2 = Mutex.OpenExisting("mutexStartAI2");
                            Mutex mutexAI2 = Mutex.OpenExisting("mutexAI2");
                            mutexAI2.WaitOne();
                            mutexStartAI2.WaitOne();

                            using (var accessor = mmf.CreateViewAccessor())
                            {
                                ushort Size = accessor.ReadUInt16(0);
                                byte[] Buffer = new byte[Size];
                                accessor.ReadArray(0 + 2, Buffer, 0, Buffer.Length);

                                value = ASCIIEncoding.ASCII.GetString(Buffer);
                                if (value == "stop") stop = true;
                                else
                                {
                                    Console.WriteLine(value);
                                    String[] substrings = value.Split(',');
                                    for (int i = 0; i < substrings.Length; i++)
                                    {
                                        tabVal[i] = Convert.ToInt32(substrings[i]);
                                    }
                                }
                            }
                            if (!stop)
                            {
                                /******************************************************************************************************/
                                /***************************************** ECRIRE LE CODE DE L'IA *************************************/
                                /******************************************************************************************************/

                                board.SetFromPlatformRepresentation(tabVal);
                                Ply ply = strategist.Run();
                                String strPly = ply.ToString();

                                /********************************************************************************************************/
                                /********************************************************************************************************/
                                /********************************************************************************************************/

                                using (var accessor = mmf2.CreateViewAccessor())
                                {
                                    byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(strPly);
                                    accessor.Write(0, (ushort)Buffer.Length);
                                    accessor.WriteArray(0 + 2, Buffer, 0, Buffer.Length);
                                }
                            }
                            mutexAI2.ReleaseMutex();
                            mutexStartAI2.ReleaseMutex();
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist. Run Process A first.");
                Console.ReadLine();
            }
        }
    }
}
