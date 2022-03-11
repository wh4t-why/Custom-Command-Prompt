using System; // for code
using System.IO; // for directories
using System.ComponentModel; // for win32exception (yes, really)
using System.Diagnostics; // for executables

/*Notes:
 
 */

namespace Interpreter
{
    class Program
    {
        public event ConsoleCancelEventHandler CancelActionKeyPressed; //somehow this line makes it so you can close the console in an emergency
        struct Data
        {
            public bool exit;
            public string dir;
            public Data(bool exit, string dir)
            {
                this.exit = exit;
                this.dir = dir;
            }
        }
        static Data Init() //Initialization function
        {
            string date = DateTime.Today.ToString();
            date = date.Substring(0, 5);
            if (date.Equals("01/04"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            _ = date;
            Console.WriteLine("{0}:Hello World!\n---------------------------------------------------------------------------", Directory.GetCurrentDirectory());
            return new Data(false, Directory.GetCurrentDirectory());
        }
        static void Main(string[] args)
        {
            Data data = Init(); //initializing

            do
            {
                Console.Write("{0}:$", data.dir);
                string input = Console.ReadLine();
                data = Interpret(input, data.dir);
            } while (!data.exit);
            return;
        }
        static Data Interpret(string input, string dir) // reads what the user input is
        {
            int i;
            string command = ""; //initializing the command

            for(i = 0; i < input.Length; i++) //getting the command from the user input
            {
                if(input[i] == ' ')
                {
                    command = input.Substring(0, i);
                    break;
                }
            }
            if (command.Length == 0) // checking whether it was a command with no arguments or switches
            {
                command = input;
            }
            switch (command) // interpreting the command
            {
                case "read": // epic function for reading the entire bee movie scrip
                    string filename = @"";
                    string readFileName = "";
                    try
                    {
                        if (input[command.Length + 1] == '-')
                        {
                            if (input[command.Length + 2] == 'f')
                            {
                                filename += input.Substring(command.Length + 4);
                                readFileName = input.Substring(command.Length + 4);
                                goto ReadTxt;
                            }
                            else
                            {
                                Console.WriteLine("{0}:Error -{1} is an invalid switch.", dir, input[command.Length + 2]);
                                return new Data(false, dir);
                            }
                        }
                        else
                        {
                            if (dir[dir.Length - 1] == '\\')
                            {
                                filename += dir + input.Substring(command.Length + 1);
                                readFileName = input.Substring(command.Length + 1);
                                goto ReadTxt;
                            }
                            else
                            {
                                filename += dir + "\\" + input.Substring(command.Length + 1);
                                readFileName = input.Substring(command.Length + 1);
                                goto ReadTxt;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine("{0}:Error - No .txt file given to read", dir);
                    }
                ReadTxt:
                    try
                    {
                        string text = File.ReadAllText(filename);
                        Console.WriteLine("\n\t\t------------------------ {0} ------------------------\n", readFileName);
                        Console.WriteLine(text + "\n");
                        _ = text;
                    }
                    catch(IOException e)
                    {
                        Console.WriteLine("{0}:{1}", dir, e.Message);
                    }
                    _ = filename;
                    _ = readFileName;
                    break;
                case "run":
                    string fileName = @"";
                    try
                    {
                        if (input[command.Length + 1] == '-')
                        {
                            if (input[command.Length + 2] == 'f')
                            {
                                fileName += input.Substring(command.Length + 4);
                                goto RunProg;
                            }
                            else
                            {
                                Console.WriteLine("{0}:Error -{1} is an invalid switch.", dir, input[command.Length + 2]);
                                return new Data(false, dir);
                            }
                        }
                        else
                        {
                            if (dir[dir.Length - 1] == '\\')
                            {
                                fileName += dir + input.Substring(command.Length + 1);
                                goto RunProg;
                            }
                            else
                            {
                                fileName += dir + "\\" + input.Substring(command.Length + 1);
                                goto RunProg;
                            }
                        }
                    }
                    catch(IndexOutOfRangeException e)
                    {
                        Console.WriteLine("{0}:Error - No executable given to run", dir);
                    }
                    RunProg:
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
                        return new Data(false, dir);
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
                    _ = input;
                    _ = command;
                    Console.WriteLine("\n\t\t\t--------- Interpreter ---------\n");
                    break;



                case "list": 
                    Console.WriteLine("\n{0} => \n", dir);
                    i = command.Length;
                    bool fin = false;
                    string[] outp;
                    if(command.Length == input.Length)
                    {
                        try
                        {
                            outp = GetAllInDirectory(dir);
                            foreach (string di in outp)
                            {
                                Console.WriteLine("\t" + di.Substring(dir.Length));
                            }
                            _ = outp;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0}:{1}", dir, e.Message);
                        }
                    }
                    else
                    {
                        if (input[i + 1] == '-' && !fin)
                        {
                            if (input[i + 2] == 's')
                            {
                                outp = GetAllInDirectory(dir);
                                string strt = "";
                                for (int k = command.Length + 4; k < input.Length; k++)
                                {
                                    if (input[k] == '-')
                                    {
                                        if (input[k + 1] == 'e')
                                        {
                                            bool or = false;
                                            if (input[input.Length - 1] == 't') { or = true; }
                                            string end = "";
                                            int j;
                                            for (j = command.Length + 4; j < input.Length; j++)
                                            {
                                                if (input[j] == ' ')
                                                {
                                                    strt = input.Substring(command.Length + 4, j - (command.Length + 4));
                                                    break;
                                                }
                                            }
                                            for (j = command.Length + strt.Length + 8; j < input.Length; j++)
                                            {
                                                if (input[j] == ' ')
                                                {
                                                    end = input.Substring(command.Length + 8 + strt.Length, j - (command.Length + 8 + strt.Length));
                                                    break;
                                                }
                                            }
                                            if (j == input.Length)
                                            {
                                                strt = input.Substring(command.Length + end.Length + 8);
                                            }
                                            _ = j;
                                            if (or)
                                            {
                                                foreach (string file in outp)
                                                {
                                                    if (file.StartsWith(dir + "\\" + strt) || file.EndsWith(end))
                                                    {
                                                        Console.WriteLine("\t" + file.Substring(dir.Length));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (string file in outp)
                                                {
                                                    if (file.StartsWith(dir + "\\" + strt) && file.EndsWith(end))
                                                    {
                                                        Console.WriteLine("\t" + file.Substring(dir.Length));
                                                    }
                                                }
                                            }
                                            _ = outp;
                                            fin = true;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: invalid switch for {1}.", dir, command);
                                            return new Data(false, dir);
                                        }
                                    }
                                }
                                if (!fin)
                                {
                                    for (int j = command.Length + 4; j < input.Length; j++)
                                    {
                                        if (input[j] == ' ' || j == input.Length - 1)
                                        {
                                            strt = input.Substring(command.Length + 4, j - command.Length - 3);
                                            break;
                                        }
                                    }
                                    foreach (string file in outp)
                                    {
                                        if (file.StartsWith(dir + "\\" + strt))
                                        {
                                            Console.WriteLine("\t" + file.Substring(dir.Length));
                                        }
                                    }
                                    _ = outp;
                                }
                            }
                            else if (input[i + 2] == 'e')
                            {
                                outp = GetAllInDirectory(dir);
                                string end = "";
                                for (int k = command.Length + 4; k < input.Length; k++)
                                {
                                    if (input[k] == '-')
                                    {
                                        if (input[k + 1] == 's')
                                        {
                                            bool or = false;
                                            if (input[input.Length - 1] == 't') { or = true; }
                                            string strt = "";
                                            int j;
                                            for (j = command.Length + 4; j < input.Length; j++)
                                            {
                                                if (input[j] == ' ')
                                                {
                                                    end = input.Substring(command.Length + 4, j - command.Length - 4);
                                                    break;
                                                }
                                            }
                                            for (j = command.Length + end.Length + 8; j < input.Length; j++)
                                            {
                                                if (input[j] == ' ')
                                                {
                                                    strt = input.Substring(command.Length + end.Length + 8, j - (command.Length + end.Length + 8));
                                                    break;
                                                }
                                            }
                                            if(j == input.Length)
                                            {
                                                strt = input.Substring(command.Length + end.Length + 8);
                                            }
                                            _ = j;
                                            //foreach (string file in outp)
                                            //{
                                            //    Console.WriteLine("${0}", file);
                                            //    if (or)
                                            //    {
                                            //        if (file.StartsWith(dir + strt) || file.EndsWith(end))
                                            //        {
                                            //            Console.WriteLine(file.Substring(dir.Length - 1));
                                            //        }
                                            //    }
                                            //    else if (file.StartsWith(dir + strt) && file.EndsWith(end))
                                            //    {
                                            //        Console.WriteLine(file.Substring(dir.Length - 1));
                                            //    }
                                            //}
                                            if (or)
                                            {
                                                foreach (string file in outp)
                                                {
                                                    if (file.StartsWith(dir + "\\" + strt) || file.EndsWith(end))
                                                    {
                                                        Console.WriteLine("\t" + file.Substring(dir.Length));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (string file in outp)
                                                {
                                                    if (file.StartsWith(dir + "\\" + strt) && file.EndsWith(end))
                                                    {
                                                        Console.WriteLine("\t" + file.Substring(dir.Length));
                                                    }
                                                }
                                            }
                                            _ = outp;
                                            _ = input;
                                            fin = true;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0}: invalid switch for {1}.", dir, command);
                                            _ = input;
                                            _ = command;
                                            return new Data(false, dir);
                                        }
                                    }
                                }
                                if (!fin)
                                {
                                    for (int j = command.Length + 4; j < input.Length; j++)
                                    {
                                        if (input[j] == ' ' || j == input.Length - 1)
                                        {
                                            end = input.Substring(command.Length + 4, j - (command.Length + 3));
                                            break;
                                        }
                                    }
                                    try
                                    {
                                        outp = GetAllInDirectory(dir);
                                        foreach (string di in outp)
                                        {
                                            if(di.EndsWith(end))
                                                Console.WriteLine("\t" + di.Substring(dir.Length));

                                        }
                                        _ = outp;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("{0}:{1}", dir, e.Message);
                                    }
                                }
                            }
                            else if(input[i + 2] == 'd')
                            {
                                try
                                {
                                    outp = Directory.GetDirectories(dir);
                                    foreach (string di in outp)
                                    {
                                        Console.WriteLine("\t" + di.Substring(dir.Length));
                                    }
                                    _ = outp;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("{0}:{1}", dir, e.Message);
                                }
                            }
                            else if(input[i + 2] == 'f')
                            {
                                try
                                {
                                    outp = Directory.GetFiles(dir);
                                    foreach (string di in outp)
                                    {
                                        Console.WriteLine("\t" + di.Substring(dir.Length));
                                    }
                                    _ = outp;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("{0}:{1}",dir,e.Message);
                                }                            }
                            else
                            {
                                Console.WriteLine("{0}: invalid switch for {1}.", dir, command);
                                _ = input;
                                _ = command;
                                return new Data(false, dir);
                            }
                        }
                    }
                    Console.WriteLine();
                    _ = input;
                    _ = command;
                    break;



                case "fld::":
                    string temp = @"" + dir;
                    if (dir[dir.Length - 1] == ':' || dir[dir.Length - 2] == ':')
                    {
                        Console.WriteLine("{0}:No Parent folder to go to.", dir);
                    }
                    else
                    {
                        DirectoryInfo D = Directory.GetParent(temp);
                        dir = D.FullName;
                        _ = D;
                    }
                    _ = input;
                    _ = command;
                    break;



                case "fld":
                    i = command.Length;
                    if (input[i+1] == '-' && input[i+2] == 'f')
                    {
                        
                        temp = @""; temp += input.Substring(i + 4);
                        if (!Directory.Exists(temp))
                        {
                            Console.WriteLine("The Directory \"{0}\" does not exist", temp);
                        }
                        else
                        {
                            dir = input.Substring(i + 4);
                        }
                    }
                    else
                    {
                        bool hasforwardslash = false;
                        if(dir[dir.Length - 1] == '\\')
                        {
                            temp = @""; temp += dir + input.Substring(i + 1);
                            hasforwardslash = true;
                        }
                        else
                        {
                            temp = @""; temp += dir + "\\" + input.Substring(i + 1);
                        }
                        
                        string[] dirs = Directory.GetDirectories(dir);
                        foreach(string di in dirs)
                        {
                            //Console.WriteLine(di);
                            if(temp == di)
                            {
                                if (hasforwardslash)
                                {
                                    dir += input.Substring(i + 1);
                                }
                                else
                                {
                                    dir += "\\" + input.Substring(i + 1);
                                }
                                _ = input;
                                _ = command;
                                return new Data(false, dir);
                            }
                        }
                        Console.WriteLine("The Directory \"{0}\" does not exist", dir + "\\" + input.Substring(i + 1));
                    }
                    _ = input;
                    _ = command;
                    return new Data(false, dir);



                case "customize":
                    i = command.Length;
                    if(command.Length == input.Length)
                    {
                        Console.WriteLine("---------------------------------------------------------------------------------------------------");
                        foreach(ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
                        {
                            Console.WriteLine(color);
                        }
                        Console.WriteLine("---------------------------------------------------------------------------------------------------");
                        _ = input;
                        _ = command;
                        return new Data(false, dir);
                    }
                    if(input[i + 1] == '-')
                    {
                        if (input[i + 2] == 'f')
                        {
                            string color = input.Substring(i + 4);
                            color = color.ToLower();
                            switch (color)
                            {
                                case "red":
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case "green":
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                case "yellow":
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                case "white":
                                    Console.ForegroundColor = ConsoleColor.White;
                                    break;
                                case "gray":
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    break;
                                case "black":
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    break;
                                case "blue":
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    break;
                                case "cyan":
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;
                                case "darkblue":
                                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                                    break;
                                case "darkcyan":
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                    break;
                                case "darkgray":
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    break;
                                case "darkgreen":
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    break;
                                case "darkmagenta":
                                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                    break;
                                case "darkred":
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    break;
                                case "darkyellow":
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    break;
                                case "magenta":
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    break;
                                default:
                                    Console.WriteLine("{0}:{1} is an invalid color.", dir, color);
                                    break;
                            }
                        }
                        else if (input[i + 2] == 'b')
                        {
                            string color = input.Substring(i + 4);
                            color = color.ToLower();
                            switch (color)
                            {
                                case "red":
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    break;
                                case "green":
                                    Console.BackgroundColor = ConsoleColor.Green;
                                    break;
                                case "yellow":
                                    Console.BackgroundColor = ConsoleColor.Yellow;
                                    break;
                                case "white":
                                    Console.BackgroundColor = ConsoleColor.White;
                                    break;
                                case "gray":
                                    Console.BackgroundColor = ConsoleColor.Gray;
                                    break;
                                case "black":
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    break;
                                case "blue":
                                    Console.BackgroundColor = ConsoleColor.Blue;
                                    break;
                                case "cyan":
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    break;
                                case "darkblue":
                                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                                    break;
                                case "darkcyan":
                                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                                    break;
                                case "darkgray":
                                    Console.BackgroundColor = ConsoleColor.DarkGray;
                                    break;
                                case "darkgreen":
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    break;
                                case "darkmagenta":
                                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                    break;
                                case "darkred":
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                    break;
                                case "darkyellow":
                                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                                    break;
                                case "magenta":
                                    Console.BackgroundColor = ConsoleColor.Magenta;
                                    break;
                                default:
                                    Console.WriteLine("{0}:{1} is an invalid color.", dir, color);
                                    break;
                            }
                        }
                        else if(input[i + 2] == 'd')
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            Console.WriteLine("{0}: invalid switch for {1}.", dir, command);
                            _ = input;
                            _ = command;
                            return new Data(false, dir);
                        }
                    }
                    _ = input;
                    _ = command;
                    break;



                case "controlarg":
                    for (i = command.Length; i < input.Length; i++)
                    {
                        if (input[i] == '-')
                        {
                            i++;
                            switch (input[i])
                            {
                                case 'n':
                                    i += 2;
                                    int num = 0;
                                    for (int k = i + 1; k > -1; k++)
                                    {
                                        bool end = false;
                                        if(k == input.Length)
                                        {
                                            k = i;
                                            end = true;
                                        }
                                        if (input[k] == ' ' || end)
                                        {
                                            char[] number;
                                            if (end)
                                                number = input.Substring(i).ToCharArray();
                                            else
                                                number = input.Substring(i, k).ToCharArray();
                                            for (int j = 0; j < number.Length; j++)
                                            {
                                                switch (number[j])
                                                {
                                                    case '0':
                                                        num += 0;
                                                        break;
                                                    case '1':
                                                        num += (int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '2':
                                                        num += 2*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '3':
                                                        num += 3*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '4':
                                                        num += 4*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '5':
                                                        num += 5*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '6':
                                                        num += 6*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '7':
                                                        num += 7*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '8':
                                                        num += 8*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    case '9':
                                                        num += 9*(int)Math.Pow(10, number.Length - (j + 1));
                                                        break;
                                                    default:
                                                        string argval = "";
                                                        for(j = 0; j < number.Length; j++)
                                                        {
                                                            argval += number[j];
                                                        }
                                                        Console.WriteLine("{0}:{1} is an invalid argument, -n can only accept numbers", dir, argval);
                                                        j = number.Length; i = input.Length;
                                                        goto endControlarg;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    Console.Write(dir + ":");
                                    for (num = num; num > 0; num--)
                                    {
                                        Console.Write("Yes");
                                    }
                                    Console.Write("\n");
                                    goto endControlarg;
                                case 'h':
                                    if(!(i + 3 > input.Length))
                                    {
                                        if (input[i + 1] == 'e' && input[i + 2] == 'l' && input[i + 3] == 'p')
                                        {
                                            Console.WriteLine("[insert command desription and options here]");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("{0}:-{1} is not a valid type of argument for {2}. Did you mean \"-help\"?", dir, input[i], command);
                                    }
                                    goto endControlarg;
                                default:
                                    Console.WriteLine("{0}:-{1} is not a valid type of argument for {2}", dir, input[i], command);
                                    i = input.Length;
                                    goto endControlarg;
                            }
                        }
                    }
                    Console.Write("Yes\n");
                    endControlarg:
                    _ = input;
                    _ = command;
                    break;



                case "control":
                    Console.WriteLine("{0}:hi", dir);
                    _ = input;
                    _ = command;
                    break;



                case "help":
                    Console.WriteLine("---------------------------------------------------------------------------------------------------\ncontrol\ncontrolarg (-n number of times)\ncustomize (-f / -b)\nfld (-f)\nfld::\nlist (-s -e / -d / -f, \"t\")\nrun (-f)\n read(-f)\nexit\n---------------------------------------------------------------------------------------------------");
                    _ = input;
                    _ = command;
                    break;



                case "exit":
                    _ = input;
                    _ = command;
                    return new Data(true, dir);



                case "clear":
                    _ = input;
                    _ = command;
                    Console.Clear();
                    break;



                case "":
                    _ = input;
                    _ = command;
                    break;



                default:
                    Console.WriteLine("{0}:{1} is not a valid command", dir, command);
                    _ = input;
                    _ = command;
                    break;
            }
            return new Data(false, dir); //return default data since this function ended
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
    }
}
