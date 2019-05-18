using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Logging;

namespace Main
{
    static class Program
    {
        private static MainForm _main;
        private static Thread _engineThread;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Config.Setup();

            //Environment.OSVersion.Platform
            AppDomain.CurrentDomain.UnhandledException
                += delegate(object sender, UnhandledExceptionEventArgs args)
                {
                    var exception = (Exception) args.ExceptionObject;

                    string logFile = Path.Combine(Logger.GetPath(), "Crash Log.txt");

                    using (TextWriter tw = new StreamWriter(logFile, true))
                    {
                        tw.WriteLine("");
                        tw.WriteLine("{0} {1}", Application.ProductName, Application.ProductVersion);
                        tw.WriteLine("{0}", DateTime.Now);
                        tw.WriteLine("Unhandled exception: " + exception);
                    }

                    MessageBox.Show($"Unexpected Error, please send '{logFile}' to simeon.pilgrim@gmail.com",
                        "Unexpected Error");
                    Environment.Exit(1);
                };


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.SetExitFunc(engine.seg043.print_and_exit);

            _main = new MainForm();

            ThreadStart threadDelegate = EngineThread;
            _engineThread = new Thread(threadDelegate) {Name = "Engine"};
            _engineThread.Start();


            Application.Run(_main);
        }

        private delegate void VoidDelegate();

        private static void EngineStopped()
        {
            VoidDelegate d = Application.Exit;
            _main.Invoke(d);
        }

        private static void EngineThread()
        {
            engine.seg001.__SystemInit(EngineStopped);
            engine.seg001.PROGRAM();

            EngineStopped();
        }
    }
}