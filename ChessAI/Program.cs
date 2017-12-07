using System;
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

            Color agentColor;
            String fileSuffix;

            if (args[0] == "white")
            {
                fileSuffix = "AI1";
                agentColor = Color.WHITE;
            }
            else if (args[0] == "black")
            {
                fileSuffix = "AI2";
                agentColor = Color.BLACK;
            }
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

                while (!stop)
                {
                    using (var mmf = MemoryMappedFile.OpenExisting("plateau"))
                    {
                        using (var mmf2 = MemoryMappedFile.OpenExisting("rep" + fileSuffix))
                        {
                            Mutex mutexStartAI = Mutex.OpenExisting("mutexStart" + fileSuffix);
                            Mutex mutexAI = Mutex.OpenExisting("mutex" + fileSuffix);
                            mutexAI.WaitOne();
                            mutexStartAI.WaitOne();

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
                                
                                board.ResetFromPlatformRepresentation(tabVal, agentColor);
                                Ply ply = strategist.Run();
                                value = ply.ToString();

                                /********************************************************************************************************/
                                /********************************************************************************************************/
                                /********************************************************************************************************/

                                using (var accessor = mmf2.CreateViewAccessor())
                                {
                                    byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(value);
                                    accessor.Write(0, (ushort)Buffer.Length);
                                    accessor.WriteArray(0 + 2, Buffer, 0, Buffer.Length);
                                }
                            }
                            mutexAI.ReleaseMutex();
                            mutexStartAI.ReleaseMutex();
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
