using System; // for code
using System.IO; // for directories
using System.ComponentModel; // for win32exception (yes, really)
using System.Diagnostics; // for executables
using System.Collections.Generic; // for lists
using System.Media; // mind your own buisness.
using System.Speech.Synthesis; //just don't mind about it.

/*Notes:
 */

namespace Interpreter
{
    class Program
    {
        private string dir;
        private bool exit;
        private Random R;
        private string date;
        enum MultiComType
        {
            Pipe = 124,
            Chain = 62
        }
        private void Init() //Initialization function
        {
            R = new Random();
            date = DateTime.Today.ToString();
            date = date.Substring(0, 5);
            if (date.Equals("01/04") || date.Equals("17/02"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.White;
            }
            else
            {
                string setdir = Directory.GetCurrentDirectory() + "\\settings.txt";
                string[] settings = File.ReadAllLines(setdir);
                for (int i = 0; i < settings.Length; i++)
                {
                    try
                    {
                        settings[i] = settings[i].Substring(0, settings[i].IndexOf('/') - 1); //get the actual settings and ignore the comments - Note: the comments must be seperated from the values by a space and at least one backslash
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        settings[i] = settings[i]; // grab the whole line if there isn't a comment or if the whole line is a comment
                    }
                }
                Console.WriteLine(HappyBDayMeooyan(int.Parse(settings[2])));
                try
                {
                    Console.ForegroundColor = StringToConsoleColor(settings[0]);
                    Console.BackgroundColor = StringToConsoleColor(settings[1]);
                }
                catch(Exception e)
                {
                    Console.WriteLine("\nThere was an error with the settings - Using Default Colors.\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }
            _ = date;
            string txt = File.ReadAllText(Directory.GetCurrentDirectory() + "\\ascii-art.txt");
            Console.WriteLine(txt);
            Console.WriteLine("---------------------------------------------------------------------------");
            _ = txt;
            exit = false;
        }
        protected void OnCancelActionKeyPressed(object sender, ConsoleCancelEventArgs args) // needs to be finished
        {
            Console.WriteLine("\n----------\nCanceling action...\n----------");
            LiftOff(dir);
            System.Threading.Thread.CurrentThread.Abort();
        }
        private void LiftOff(string dir)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelActionKeyPressed);
            Init();
            this.dir = dir;
            do // main runtime loop
            {
                Console.Write("{0}:$", this.dir);
                string input = Console.ReadLine();

                if(input.ToLower().Contains("lean"))//Important lean code
                {
                      Console.ForegroundColor = ConsoleColor.Magenta;
                      if(R.Next(0, 5) == 1)
                      {
                          Console.WriteLine(File.ReadAllText(dir + "\\lean.txt") + "\n\n");
                      }
                      else
                      {
                          Console.WriteLine(File.ReadAllText(dir + "\\lean!!!!!.txt") + "\n\n");
                      }
                }

                if (input.Contains((char)MultiComType.Chain))
                {
                    string[] coms = GetSeperateCommands(input, MultiComType.Chain);
                    for (int i = 0; i < coms.Length; i++)
                    {
                        Interpret(coms[i]);
                    }
                }
                else if (input.Contains((char)MultiComType.Pipe)) // need to make the commands in "Interpret" seperate methods.
                {
                    string[] coms = GetSeperateCommands(input, MultiComType.Pipe);
                    Interpret(coms, 0);
                }
                else
                {
                    Interpret(input);
                }
            } while (!exit);
        }
        static void Main(string[] args)
        {
            Program P = new Program();
            P.LiftOff(Directory.GetCurrentDirectory());
        }

        private Func<string, string> GetFunc(string command)
        {
            switch (command)
            {
                case "georgesay":
                    return AsciiSay;
                case "read":
                    return ReadFile;
                case "list":
                    return List;
                default:
                    return null;
            }
        }
        private Func<string> GetFuncNA(string command)
        {
            switch (command)
            {
                case "help":
                    return GetHelp;
                default:
                    return null;
            }
        }
        private Action<string> GetAction(string command)
        {
            switch (command)
            {
                case "say":
                    return Say;
                case "funnymonke":
                    return FunnyMammal;
                case "customize":
                    return Customize;
                case "run":
                    return RunProg;
                case "edit":
                    return edit;
                case "fld":
                    return Fld;
                default:
                    return null;
            }
        }

        private string Interpret(string[] coms, int i)
        {
            if (i == coms.Length - 1)
            {
                Func<string> F = GetFuncNA(coms[i].Substring(0, coms[i].IndexOf(' ')));
                Func<string, string> F2 = GetFunc(coms[i].Substring(0, coms[i].IndexOf(' ')));

                if (F2 == null && F == null)
                {
                    return null;
                }
                if (F == null)
                {
                    return F2(coms[i].Substring(coms[i].IndexOf(' ')));
                }
                return F();
            }
            else if (i == 0)
            {
                Action<string> A = GetAction(coms[i]);

                if(A == null)
                {
                    Console.WriteLine("{0} in index {1} is a function which does not recieve arguments", coms[i], i);
                    return null;
                }
                A(Interpret(coms, i + 1));
                return "";
            }
            else
            {
                Func<string, string> F = GetFunc(coms[i]);
                if(F == null)
                {
                    Console.WriteLine("{0} in index {1} is a function which does not recieve arguments", coms[i], i);
                    return null;
                }
                return F(Interpret(coms, i + 1));
            }
        }

        private void Interpret(string input) // reads what the user input is
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(this.OnCancelActionKeyPressed);
            int i;
            string command = ""; //initializing the command

            if(input == null)
            {
                return;
            }
            for (i = 0; i < input.Length && input[i] != ' '; i++) //getting the command from the user input
            {
                command += input[i];
            }
            switch (command) // interpreting the command
            {
                case "georgesay":
                    Console.WriteLine(AsciiSay(input));
                    break;
                    
                case "funnymonke":
                    FunnyMammal(input.Substring(10));
                    Console.WriteLine();
                    break;

                case "edit":
                    edit(input.Substring(command.Length + 1));
                    break;



                case "read": // epic function for reading the entire bee movie scrip
                    Console.WriteLine(ReadFile(input));
                    break;


                case "run":
                    RunProg(input);
                    break;



                case "list":
                    Console.Write(List(input));
                    break;



                case "fld::":
                    FldUp();
                    break;



                case "fld":
                    Fld(input);
                    break;



                case "customize":
                    Customize(input);
                    break;



                case "help":
                    Console.WriteLine(GetHelp());
                    break;



                case "exit":
                    exit = true;
                    break;



                case "clear":
                    Console.Clear();
                    break;



                case "bandicoot":
                    /*
                                       /\ }] /\
                                      /__\]}/__\
                                     \ (*)  (*) /
                                    /   \ \o/ /  \
                                   /  /\ \==/ /\  \
                                 /  /   \    /  \  \
                                /  /    |_*_|   \  \
                    */

                    System.Threading.Thread.CurrentThread.Abort();
                    break;



                case "":
                    break;

                case "say":
                    Say(input);
                    break;

                default:
                    Console.WriteLine("{0}:\"{1}\" is not a valid command", dir, command);
                    _ = input;
                    _ = command;
                    break;
            }
        }

        private void Say(string input)
        {
            System.Threading.Thread T = new System.Threading.Thread(() => SpeakThread(input));
            T.Start();
        }
        private void SpeakThread(string input)
        {
            var speaker = new SpeechSynthesizer();
            speaker.SetOutputToDefaultAudioDevice();
            speaker.Speak(input.Substring(4));
        }

        private string ReadFile(string input)
        {
            string filename = "";
            try
            {
                filename = GetDirFromString(input.Substring(5));
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("{0}:Error - No .txt file given to read", dir);
            }
            string[] settings = File.ReadAllLines(dir + "\\settings.txt");
            settings[4] = settings[4].Substring(0, settings[4].IndexOf('/') - 1);
            settings[5] = settings[5].Substring(0, settings[5].IndexOf('/') - 1);

            if (bool.Parse(settings[4]) && File.ReadAllBytes(filename).Length > int.Parse(settings[5]))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Warning!");
                settings[0] = settings[0].Substring(0, settings[0].IndexOf('/'));
                Console.ForegroundColor = StringToConsoleColor(settings[0]);
                Console.Write(" The following file is pretty large... are you sure you want to read it? (y / n)");

                ConsoleKeyInfo info = Console.ReadKey();

                if(info.KeyChar == 'n')
                {
                    return "";
                }
                while(info.KeyChar != 'y')
                {
                    info = Console.ReadKey();
                    if (info.KeyChar == 'n')
                    {
                        return "";
                    }
                }
            }
            try
            {
                string text = File.ReadAllText(filename);
                return "\n\t" + text + "\n";
            }
            catch (IOException e)
            {
                Console.WriteLine("{0}:{1}", dir, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("{0}:{1}", dir, e.Message);
            }
            return "";
        }

        private void RunProg(string input)
        {
            string fileName = "";
            try
            {
                fileName = GetDirFromString(input.Substring(4));
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("{0}:Error - No executable given to run", dir);
            }
            try
            {
                Console.WriteLine("\n\t\t\t---------{0}---------\n", fileName);
                // Prepare the process to run
                ProcessStartInfo start = new ProcessStartInfo();
                // Enter the executable to run, including the complete path
                start.FileName = fileName;
                // Do you want to show a console window?
                start.WindowStyle = ProcessWindowStyle.Normal;
                start.CreateNoWindow = false;

                // Run the external process & wait for it to finish
                using (Process proc = Process.Start(start))
                {
                    proc.WaitForExit();
                }
                Console.WriteLine("\n\t\t\t--------- Interpreter ---------\n");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("{0}:{1}", dir, e.Message);
                Console.WriteLine("\n\t\t\t--------- Interpreter ---------\n");
            }
            catch (Win32Exception e)
            {
                Console.WriteLine("{0}:{1}", dir, e.Message);
                Console.WriteLine("\n\t\t\t--------- Interpreter ---------\n");
            }
        }

        private string GetHelp()
        {
            return "---------------------------------------------------------------------------------------------------\ncustomize (-f / -b / -d)\nfld\nfld::\nlist (-s -e / -d / -f, \"t\")\nread\nedit\nexit\n---------------------------------------------------------------------------------------------------";
        }

        private void FldUp()
        {
            string temp = @"" + dir;
            if (dir[dir.Length - 1] == ':' || dir[dir.Length - 2] == ':')
            {
                Console.WriteLine("{0}:No Parent folder to go to.", dir);
            }
            else
            {
                DirectoryInfo D = Directory.GetParent(temp);
                dir = D.FullName;
            }
        }

        private string List(string input)
        {
            string ret = "";
            string[] outp;
            if (4 == input.Length)
            {
                ret += dir + ": =>\n";
                try
                {
                    outp = GetAllInDirectory(dir);
                    foreach (string di in outp)
                    {
                        ret += "\t" + di.Substring(dir.Length) + "\n";
                    }
                    return ret + "\n";
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}:{1}", dir, e.Message);
                }
            }
            else
            {
                if (input[4 + 1] == '-')
                {
                    if (input[4 + 2] == 's')
                    {
                        Console.WriteLine("\n{0} => \n", dir);
                        outp = GetAllInDirectory(dir);
                        string strt = "";
                        for (int k = 8; k < input.Length; k++)
                        {
                            if (input[k] == '-')
                            {
                                if (input[k + 1] == 'e')
                                {
                                    bool or = false;
                                    if (input[input.Length - 1] == 't') { or = true; }
                                    string end = "";
                                    int j;
                                    for (j = 8; j < input.Length; j++)
                                    {
                                        if (input[j] == ' ')
                                        {
                                            strt = input.Substring(8, j - 8);
                                            break;
                                        }
                                    }
                                    for (j = strt.Length + 12; j < input.Length; j++)
                                    {
                                        if (input[j] == ' ')
                                        {
                                            end = input.Substring(12 + strt.Length, j - (12 + strt.Length));
                                            break;
                                        }
                                    }
                                    if (j == input.Length)
                                    {
                                        strt = input.Substring(end.Length + 12);
                                    }
                                    if (or)
                                    {
                                        foreach (string file in outp)
                                        {
                                            if (file.StartsWith(dir + "\\" + strt) || file.EndsWith(end))
                                            {
                                                ret += "\t" + file.Substring(dir.Length) + "\n";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string file in outp)
                                        {
                                            if (file.StartsWith(dir + "\\" + strt) && file.EndsWith(end))
                                            {
                                                ret += "\t" + file.Substring(dir.Length) + "\n";
                                            }
                                        }
                                    }
                                    return ret + "\n";
                                }
                                else
                                {
                                    Console.WriteLine("{0}: invalid switch for {1}.", dir, "list");
                                    return "\n";
                                }
                            }
                        }
                        for (int j = 8; j < input.Length; j++)
                        {
                            if (input[j] == ' ' || j == input.Length - 1)
                            {
                                strt = input.Substring(8, j - 7);
                                break;
                            }
                        }
                        foreach (string file in outp)
                        {
                            if (file.StartsWith(dir + "\\" + strt))
                            {
                                ret += "\t" + file.Substring(dir.Length);
                            }
                        }
                        return ret + "\n";
                    }
                    else if (input[6] == 'e')
                    {
                        Console.WriteLine("\n{0} => \n", dir);
                        outp = GetAllInDirectory(dir);
                        string end = "";
                        for (int k = 8; k < input.Length; k++)
                        {
                            if (input[k] == '-')
                            {
                                if (input[k + 1] == 's')
                                {
                                    bool or = false;
                                    if (input[input.Length - 1] == 't') { or = true; }
                                    string strt = "";
                                    int j;
                                    for (j = 8; j < input.Length; j++)
                                    {
                                        if (input[j] == ' ')
                                        {
                                            end = input.Substring(8, j - 8);
                                            break;
                                        }
                                    }
                                    for (j = end.Length + 12; j < input.Length; j++)
                                    {
                                        if (input[j] == ' ')
                                        {
                                            strt = input.Substring(end.Length + 12, j - end.Length - 12);
                                            break;
                                        }
                                    }
                                    if (j == input.Length)
                                    {
                                        strt = input.Substring(end.Length + 12);
                                    }
                                    if (or)
                                    {
                                        foreach (string file in outp)
                                        {
                                            if (file.StartsWith(dir + "\\" + strt) || file.EndsWith(end))
                                            {
                                                ret += "\t" + file.Substring(dir.Length) + "\n";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string file in outp)
                                        {
                                            if (file.StartsWith(dir + "\\" + strt) && file.EndsWith(end))
                                            {
                                                ret += "\t" + file.Substring(dir.Length) + "\n";
                                            }
                                        }
                                    }
                                    return ret + "\n";
                                }
                                else
                                {
                                    Console.WriteLine("{0}: invalid switch for {1}.", dir, "list");
                                    return "\n";
                                }
                            }
                        }
                        for (int j = 8; j < input.Length; j++)
                        {
                            if (input[j] == ' ' || j == input.Length - 1)
                            {
                                end = input.Substring(8, j - 7);
                                break;
                            }
                        }
                        try
                        {
                            outp = GetAllInDirectory(dir);
                            foreach (string di in outp)
                            {
                                if (di.EndsWith(end))
                                    ret += "\t" + di.Substring(dir.Length) + "\n";
                            }
                            return ret + "\n";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0}:{1}", dir, e.Message);
                        }
                    }
                    else if (input[6] == 'd')
                    {
                        Console.WriteLine("\n{0} => \n", dir);
                        try
                        {
                            outp = Directory.GetDirectories(dir);
                            foreach (string di in outp)
                            {
                                ret += "\t" + di.Substring(dir.Length) + "\n";
                            }
                            return ret + "\n";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0}:{1}", dir, e.Message);
                        }
                    }
                    else if (input[6] == 'f')
                    {
                        Console.WriteLine("\n{0} => \n", dir);
                        try
                        {
                            outp = Directory.GetFiles(dir);
                            foreach (string di in outp)
                            {
                                ret += "\t" + di.Substring(dir.Length) + "\n";
                            }                            
                            return ret + "\n";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0}:{1}", dir, e.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0}: invalid switch for {1}.", dir, "list");
                        return "\n";
                    }
                }
            }
            return "\n";
        }

        private void Fld(string input)
        {
            string temp = GetDirFromString(input.Substring(4));
            if (!Directory.Exists(temp))
            {
                Console.WriteLine("The Directory \"{0}\" does not exist.", temp);
            }
            else
            {
                dir = temp;
            }
        }

        /* 
         * the function below gets a string and a directory, and detects if the string is a full directory.
         * if it is - it returns the string, otherwise - it returns the directory with the string at the end.
         */
        private string GetDirFromString(string str)
        {
            int i = str.IndexOf(':');
            if (i == -1)
            {
                if (dir[dir.Length - 1] == '\\')
                {
                    return @"" + dir + str;
                }
                return @"" + dir + "\\" + str;
            }
            return str;
        }
        /*
         * The function below gets a string which contains at least one '|' character,
         * and returns the strings within it in order of piping.
         */
        static string[] GetSeperateCommands(string input, MultiComType type)
        {
            List<int> indecises = new List<int>(0);
            for(int i = 0; i < input.Length; i++)
            {
                if(input[i] == (char)type)
                {
                    indecises.Add(i);
                }
            }
            string[] coms = new string[indecises.Count + 1];
            int ind = 0;
            for (int i = 0; i < indecises.Count; i++)
            {
                coms[i] = input.Substring(ind, indecises[i] - 1 - ind);
                ind = indecises[i] + 2;
            }
            coms[indecises.Count] = input.Substring(ind);
            return coms;
        }

        static string[] GetAllInDirectory(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            string[] folders = Directory.GetDirectories(dir);
            string[] outp = new string[files.Length + folders.Length];

            for(int i = 0; i < files.Length; i++)
            {
                outp[i] = files[i];
            }
            for(int i = 0, k = files.Length; i < folders.Length; i++)
            {
                outp[k + i] = folders[i];
            }
            _ = files;
            _ = folders;
            return outp;
        }

        static ConsoleColor StringToConsoleColor(string color)
        {
            switch (color)
            {
                case "red":
                    return ConsoleColor.Red;
                case "green":
                    return ConsoleColor.Green;
                case "yellow":
                    return ConsoleColor.Yellow;
                case "white":
                    return ConsoleColor.White;
                case "gray":
                    return ConsoleColor.Gray;
                case "black":
                    return ConsoleColor.Black;
                case "blue":
                    return ConsoleColor.Blue;
                case "cyan":
                    return ConsoleColor.Cyan;
                case "darkblue":
                    return ConsoleColor.DarkBlue;
                case "darkcyan":
                    return ConsoleColor.DarkCyan;
                case "darkgray":
                    return ConsoleColor.DarkGray;
                case "darkgreen":
                    return ConsoleColor.DarkGreen;
                case "darkmagenta":
                    return ConsoleColor.DarkMagenta;
                case "darkred":
                    return ConsoleColor.DarkRed;
                case "darkyellow":
                    return ConsoleColor.DarkYellow;
                case "magenta":
                    return ConsoleColor.Magenta;
                default:
                    Console.WriteLine("\n{1} is an invalid color.\n", color);
                    return 0;
            }
        }

        static string[] GetStringSeperated(string str, char c)
        {
            List<int> indecises = new List<int>(0);
            int cnt = 1;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == c)
                {
                    indecises.Add(i);
                    cnt++;
                }
            }
            string[] coms = new string[cnt];
            int ind = 0;
            for (int i = 0; i < cnt - 1; i++)
            {
                coms[i] = str.Substring(ind, indecises[i] - ind);
                ind = indecises[i] + 1;
            }
            coms[cnt - 1] = str.Substring(ind);
            return coms;
        }

        static string ReadLine(string Default) // needs to be modified to work with editing multiple lines
        {
            Console.WriteLine();
            int zer = Console.CursorLeft;
            int row = Console.CursorTop;
            int ind = Default.Length - 1;
            Console.Write(Default);
            ConsoleKeyInfo info;
            List<char> chars = new List<char>();
            if (!string.IsNullOrEmpty(Default))
            {
                chars.AddRange(Default.ToCharArray());
            }
            string[] lns = GetStringSeperated(Default, '\n');
            List<int> lnsln = new List<int>();
            for(int i = 0; i < lns.Length; i++)
            {
                lnsln.Insert(i, lns[i].Length);
            }
            _ = lns;

            while (true)
            {
                info = Console.ReadKey(true);
                if (char.IsLetterOrDigit(info.KeyChar))
                {
                    ind++;
                    Console.Write(info.KeyChar);
                    chars.Insert(ind, info.KeyChar);
                    lnsln[Console.CursorTop - row]++;
                }
                else
                {
                    switch (info.Key)
                    {
                        case ConsoleKey.Escape:
                            //control key
                            break;
                        case ConsoleKey.Home:
                            Console.CursorTop = row;
                            Console.CursorLeft = zer;
                            ind = 0;
                            break;
                        case ConsoleKey.Enter:
                            if (info.Modifiers == ConsoleModifiers.Control)
                            {
                                Console.CursorLeft = 0; Console.CursorTop = row + GetNumberOfApperances(chars, '\n'); goto exitfunc;
                            }
                            else
                            {
                                chars.Insert(ind + 1, '\n');
                                int temprow = Console.CursorTop + 1;
                                ind++;
                                int j = lnsln[Console.CursorTop - row] - Console.CursorLeft + zer;
                                lnsln.Insert(Console.CursorTop - row + 1, j);
                                lnsln[Console.CursorTop - row] -= j;
                                _ = j;
                                for(int i = Console.CursorTop - row + 1; i < lnsln.Count; i++)
                                {
                                    for(int k = 0; k < lnsln[i]; k++)
                                    {
                                        Console.Write(' ');
                                    }
                                    Console.WriteLine();
                                }
                                Console.CursorTop = temprow;
                                for(int i = ind + 1; i < chars.Count; i++)
                                {
                                    Console.Write(chars[i]);
                                }
                                Console.CursorTop = temprow;
                                Console.CursorLeft = zer;
                            }
                            break;
                        case ConsoleKey.Backspace:
                            if (Console.CursorLeft > zer)
                            {
                                chars.RemoveAt(ind);
                                Console.CursorLeft -= 1;
                                Console.Write(' ');
                                Console.CursorLeft -= 1;
                                ind--;
                                lnsln[Console.CursorTop - row]--;
                            }
                            else if (Console.CursorLeft == zer && Console.CursorTop > row)
                            {
                                chars.RemoveAt(ind - 1);
                                ind--;
                                int temprow = Console.CursorTop - 1;
                                for(int i = Console.CursorTop - row; i < lnsln.Count; i++)
                                {
                                    for(int k = 0; k < lnsln[i]; k++)
                                    {
                                        Console.Write(' ');
                                    }
                                    Console.CursorTop++;
                                    Console.CursorLeft = zer;
                                }
                                Console.CursorTop = temprow;
                                Console.CursorLeft = lnsln[Console.CursorTop - row] + zer;
                                for (int i = ind + 1; i < chars.Count; i++)
                                {
                                    Console.Write(chars[i]);
                                }
                                Console.CursorTop = temprow;
                                ind += lnsln[Console.CursorTop - row + 1];
                                lnsln[Console.CursorTop - row] += lnsln[Console.CursorTop - row + 1];
                                lnsln.RemoveAt(Console.CursorTop - row + 1);
                                Console.CursorLeft = zer + lnsln[Console.CursorTop - row];
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (Console.CursorTop > row)
                            {
                                ind -= 1 + Console.CursorLeft - zer;
                                Console.CursorTop--;
                                Console.CursorLeft = zer + lnsln[Console.CursorTop - row];
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (Console.CursorTop - row < lnsln.Count - 1)
                            {
                                ind = ind - Console.CursorLeft + zer + lnsln[Console.CursorTop - row];
                                Console.CursorTop++;
                                Console.CursorLeft = zer + lnsln[Console.CursorTop - row];
                                ind = ind + 1 + Console.CursorLeft;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if (Console.CursorLeft > zer)
                            {
                                Console.CursorLeft--;
                                ind--;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (Console.CursorLeft - zer < lnsln[Console.CursorTop - row])
                            {
                                Console.CursorLeft++;
                                ind++;
                            }
                            break;
                        default:
                            try
                            {
                                ind++;
                                Console.Write(info.KeyChar);
                                chars.Insert(ind, info.KeyChar);
                                lnsln[Console.CursorTop - row]++;
                            }
                            catch(Exception e) { }
                            break;
                    }
                }
            }
        exitfunc:
            Console.CursorTop = lnsln.Count + row;
            Console.CursorLeft = zer;
            Console.WriteLine();
            return new string(chars.ToArray());
        }

        private void FunnyMammal(string text)
        {
            ConsoleColor[] rainbow = { ConsoleColor.Magenta, ConsoleColor.DarkMagenta, ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.DarkGreen, ConsoleColor.Yellow, ConsoleColor.DarkYellow, ConsoleColor.Red, ConsoleColor.DarkRed};

            string[] settings = File.ReadAllLines(dir + "\\settings.txt");
            settings[3] = settings[3].Substring(0, settings[3].IndexOf('/') - 1);
            if (R.Next(0, int.Parse(settings[3])) == 0)
            {
                SoundPlayer s = new SoundPlayer();
                s.SoundLocation = @"" + dir + "\\" + "funni_monkey.WAV";
                s.Play();
            } // Halal function

            string[] lines = GetStringSeperated(text, '\n');
            Console.WriteLine();
            for(int i = 0; i < lines.Length; i++)
            {
                for(int j = 0; j < lines[i].Length; j++)
                {
                    if(i + j >= rainbow.Length)
                    {
                        int num = (i + j) / rainbow.Length;
                        Console.ForegroundColor = rainbow[i + j - (num * rainbow.Length)];
                    }
                    else
                    {
                        Console.ForegroundColor = rainbow[i + j];
                    }
                    Console.Write(lines[i][j]);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        private void edit(string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    if (dir[dir.Length - 1] == '\\')
                    {
                        File.Create(dir + file);
                    }
                    else
                    {
                        File.Create(dir + "\\" + file);
                    }
                }
                string contents = File.ReadAllText(file);
                contents = ReadLine(contents);
                File.WriteAllText(file, contents);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}:{1}", dir, e.Message);
            }
        }
        private void Customize(string input)
        {
            date = DateTime.Today.ToString();
            date = date.Substring(0, 5);
            if(date.Equals("01/04"))
            {
                return;
            }
            if (9 == input.Length)
            {
                Console.WriteLine("---------------------------------------------------------------------------------------------------");
                foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
                {
                    Console.WriteLine(color);
                }
                Console.WriteLine("---------------------------------------------------------------------------------------------------");
            }
            else if (input[9 + 1] == '-')
            {
                if (input[9 + 2] == 'f')
                {
                    string color = input.Substring(9 + 4);
                    color = color.ToLower();
                    try
                    {
                        Console.ForegroundColor = StringToConsoleColor(color);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0}:Using Default Colors.\n", e.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
                else if (input[9 + 2] == 'b')
                {
                    string color = input.Substring(9 + 4);
                    color = color.ToLower();
                    try
                    {
                        Console.BackgroundColor = StringToConsoleColor(color);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0}:Using Default Colors.\n", e.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
                else if (input[9 + 2] == 'd')
                {
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("{0}: invalid switch for {1}.", dir, "customize");
                }
            }
        }

        private string AsciiSay(string input)
        {
            string path = @"" + dir + "\\c" + R.Next(1,6).ToString() + ".txt";
            
            input = input.Substring(input.IndexOf(' ') + 1);
            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);
            }
            catch(IndexOutOfRangeException e)
            {
                return "Character not found";
            }
            if(lines.Length < 6)
            {
                lines[0] += "    " + input;
            }
            else
            {
                int biggestLine = 0;
                foreach(string line in lines)
                {
                    if(line.Length > biggestLine)
                    {
                        biggestLine = line.Length;
                    }
                }
                
                int distance = biggestLine + 3;
                
                for(int j = 3; j < 6; j++)
                {
                    for (int i = 0; i < (distance - lines[j].Length); i++)
                    {
                        lines[j] += " ";
                    }
                }

                /*
                ....__....__
                .../..word..\
                ..<___....__/
                */

                for (int i = 0; i < 4; i++)
                {
                    lines[3] += " ";
                }
                for(int i = 0; i < (input.Length+4); i++)
                {
                    lines[3] += "_";
                }

                for (int i = 0; i < 3; i++)
                {
                    lines[4] += " ";
                }
                lines[4] += "/  " + input + "  \\";

                for (int i = 0; i < 2; i++)
                {
                    lines[5] += " ";
                }
                lines[5] += "<";
                for (int i = 0; i < (input.Length + 6); i++)
                {
                    lines[5] += "_";
                }
                lines[5] += "/";
            }
            string output = "";
            foreach(string line in lines)
            {
                output += line + "\n";
            }
            return output;
        }

        static int GetNumberOfApperances(List<char> chars, char ch)
        {
            int cnt = 0;
            for(int i = 0; i < chars.Count; i++)
            {
                if(chars[i] == ch)
                {
                    cnt++;
                }
            }
            return cnt;
        }

        public string HappyBDayMeooyan(int num)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(this.OnCancelActionKeyPressed);
            /*
                            ___________
                  /\\     /  Happ B day \\
                 /  \\   |   Mooyan     |
                /____\\ < ____________ /
               / 0 ^ 0\\
               \\  3  /
                \\   /
                 \\ /
                  \\/
            */
            string ou = "";
            for (int i = 0; i < num; i++)
            {
                ou += "                ___________\n" + "      /\\     /  Happ B day \\\n" + "     /  \\   |   Mooyan     |\n" + "    /____\\ < ____________ /\n" + "   / 0 ^ 0\\\n" + "   \\   3  /\n" + "    \\    /\n" + "     \\  /\n" + "      \\/\n\n";
                ou += "                ___________\n" + "      /\\     /  Happ B day \\\n" + "     /  \\   |   Mooyan     |\n" + "    /____\\ < ____________ /\n" + "   / 0 ^ 0\\\n" + "   \\   O  /\n" + "    \\    /\n" + "     \\  /\n" + "      \\/\n\n";            
            }
            return ou;
        }
    }
}