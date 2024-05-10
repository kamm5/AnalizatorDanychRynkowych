using HtmlAgilityPack;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

class Program
{
    struct taskinfo
    {
        public string name;
        public int frequency;
        public string url;
        public string xPath;
        public List<string> valueTags;
        public List<int> cursorShifts;
        public List<string> lastIndexTags;
    }

    static void WriterLine(string comment)
    {
        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " + comment);
        File.AppendAllText("log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " + comment + Environment.NewLine);
    }
    static void Writer(string comment)
    {
        Console.Write($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " + comment);
        File.AppendAllText("log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " + comment);
    }
    static string ReaderLine()
    {
        string value = Console.ReadLine();
        File.AppendAllText("log.txt", value + Environment.NewLine);
        return value;
    }
    static string DataCoordinator(char mode)
    {
        if (mode == 's')
        {
            string value = ReaderLine();
            while (string.IsNullOrEmpty(value))
            {
                Writer("Wartość została błędnie podana, proszę podać ją jeszcze raz: ");
                value = ReaderLine();
            }
            return value;
        }
        else if (mode == 'i')
        {
            int value;
            while (!int.TryParse(ReaderLine(), out value))
            {
                Writer("Wartość nie jest liczbą całkowitą, proszę podać ją jeszcze raz: ");
            }
            return value.ToString();
        }
        else
        {
            WriterLine("Wybrano błędny tryb w DataCoordinator!");
            return "0";
        }
    }
    static void Enter_decision(out ConsoleKeyInfo keyInfo, string key1, string key2)
    {
        do
        {
            keyInfo = Console.ReadKey();
            Console.Write("\b \b");
        } while ((keyInfo.Key.ToString() != key1) && (keyInfo.Key.ToString() != key2));
    }
    static void TimersInfo(List<DateTime> clock, List<taskinfo> importTasks)
    {
        DateTime actualClock = DateTime.Now;
        DateTime temp;
        WriterLine("Czasy do wykonania zadań: ");
        WriterLine("----------");
        for (int i = 0; i < importTasks.Count; i++)
        {
            temp = clock[i].AddMinutes(importTasks[i].frequency);
            TimeSpan difTime = temp.Subtract(actualClock);
            WriterLine(importTasks[i].name + " " + (int)difTime.TotalSeconds / 60 + " Minut " + (int)difTime.TotalSeconds % 60 + " Sekund ");
        }
        WriterLine("----------");
    }
    static void SnapShotHtml()
    {
        string url = "";

        Writer("Podaj url strony: ");
        do
        {
            url += DataCoordinator('s');
        } while ((url.Length % 254) == 0);

        HtmlWeb web = new HtmlWeb();
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            HtmlDocument doc = web.Load(url);
            HtmlNode targetElement = doc.DocumentNode;

            if (targetElement != null)
            {
                File.WriteAllText("snapshot.html", targetElement.OuterHtml);
                WriterLine("Migawka została prawidłowo wykonana");
            }
            else
            {
                WriterLine("Nie udało się wykonać migawki");
            }
        }
        else
        {
            WriterLine("Nie udało się wykonać migawki z powodu błędnego url");
        }
    }
    static void ImpFunction(ref List<taskinfo> importTasks)
    {
        if (File.Exists("tasks.txt"))
        {
            string[] tasks = File.ReadAllLines("tasks.txt");
            if (tasks.Length == 0)
            {
                WriterLine("Brak zadeklarowanych zadań");
            }
            else
            {
                foreach (string task in tasks)
                {
                    string[] parameters = task.Split(';');
                    taskinfo task_temp = new taskinfo()
                    {
                        name = parameters[0],
                        frequency = int.Parse(parameters[1]),
                        url = parameters[2],
                        xPath = parameters[3],
                        valueTags = new List<string>(),
                        cursorShifts = new List<int>(),
                        lastIndexTags = new List<string>()
                    };
                    for (int i = 4; i < parameters.Length; i += 3)
                    {
                        task_temp.valueTags.Add(parameters[i]);
                    }
                    for (int i = 5; i < parameters.Length; i += 3)
                    {
                        task_temp.cursorShifts.Add(int.Parse(parameters[i]));
                    }
                    for (int i = 6; i < parameters.Length; i += 3)
                    {
                        task_temp.lastIndexTags.Add(parameters[i]);
                    }
                    importTasks.Add(task_temp);
                }
            }
        }
        else
        {
            WriterLine("Brak zadeklarowanych zadań");
        }
    }
    static void ExpFunction(ref List<taskinfo> importTasks)
    {
        taskinfo task_temp = new taskinfo();
        string tasktextfile;
        ConsoleKeyInfo keyInfo;

        using (StreamWriter taskFile = new StreamWriter("tasks.txt", true))
        {
            Writer("Podaj nazwę zadania: ");
            task_temp.name = DataCoordinator('s');
            tasktextfile = task_temp.name + ";";
            Writer("Podaj częstotliwość wykonywania zadania w minutach: ");
            task_temp.frequency = int.Parse(DataCoordinator('i'));
            tasktextfile += task_temp.frequency + ";";
            task_temp.url = "";
            Writer("Podaj url: ");
            do
            {
                task_temp.url += DataCoordinator('s');
            } while ((task_temp.url.Length % 254) == 0);
            tasktextfile += task_temp.url + ";";
            Writer("Podaj xPath: ");
            task_temp.xPath = DataCoordinator('s');
            tasktextfile += task_temp.xPath + ";";
            task_temp.valueTags = new List<string>();
            task_temp.cursorShifts = new List<int>();
            task_temp.lastIndexTags = new List<string>();
            Writer("Podaj nazwę do odczytu: ");
            task_temp.valueTags.Add(DataCoordinator('s'));
            tasktextfile += task_temp.valueTags[0] + ";";
            Writer("Podaj przesunięcie kursora odczytu nazwy: ");
            task_temp.cursorShifts.Add(int.Parse(DataCoordinator('i')));
            tasktextfile += task_temp.cursorShifts[0] + ";";
            Writer("Podaj znak kończący: ");
            task_temp.lastIndexTags.Add(DataCoordinator('s'));
            tasktextfile += task_temp.lastIndexTags[0] + ";";

            WriterLine("Czy chcesz podać kolejną wartość do odczytania (t/n): ");
            Enter_decision(out keyInfo, "T", "N");
            int i = 1;
            while (keyInfo.Key == ConsoleKey.T)
            {
                Writer("Podaj wartość do odczytu: ");
                task_temp.valueTags.Add(DataCoordinator('s'));
                tasktextfile += task_temp.valueTags[i] + ";";
                Writer("Podaj przesunięcie kursora odczytu wartości: ");
                task_temp.cursorShifts.Add(int.Parse(DataCoordinator('i')));
                tasktextfile += task_temp.cursorShifts[i] + ";";
                Writer("Podaj znak kończący: ");
                task_temp.lastIndexTags.Add(DataCoordinator('s'));
                tasktextfile += task_temp.lastIndexTags[i] + ";";
                WriterLine("Czy chcesz podać kolejną wartość do odczytania (t/n): ");
                Enter_decision(out keyInfo, "T", "N");
                i++;
            }
            WriterLine("Czy chcesz zapisać zadanie (t/n): ");
            Enter_decision(out keyInfo, "T", "N");
            if (keyInfo.Key == ConsoleKey.T)
            {
                taskFile.WriteLine(tasktextfile);
                importTasks.Add(task_temp);
                WriterLine("Zadanie '" + task_temp.name + "' zostało zapisane");
            }
            else
            {
                WriterLine("Zadanie '" + task_temp.name + "' nie zostało zapisane");
            }
            taskFile.Close();
        }
    }
    static void DelFunction(ref List<taskinfo> importTasks, ref List<Timer> timers, ref List<DateTime> clock)
    {
        ConsoleKeyInfo keyInfo;
        Writer("Podaj nazwę zadania do usunięcia: ");
        string delTaskName = DataCoordinator('s');
        string[] lines = File.ReadAllLines("tasks.txt");
        int lineToRemove = Array.FindIndex(lines, line => line.Split(';')[0] == delTaskName);
        if (lineToRemove >= 0)
        {
            lines = lines.Where((line, index) => index != lineToRemove).ToArray();
            WriterLine("Czy na pewno chcesz usunąć zadanie '" + delTaskName + "' (t/n)");
            Enter_decision(out keyInfo, "T", "N");
            if (keyInfo.Key == ConsoleKey.T)
            {
                int index = importTasks.FindIndex(task => task.name == delTaskName);
                timers[index].Dispose();
                timers.RemoveAt(index);
                clock.RemoveAt(index);
                importTasks.RemoveAll(task => task.name == delTaskName);
                File.WriteAllLines("tasks.txt", lines);
                WriterLine("Zadanie '" + delTaskName + "' zostało pomyślnie usunięte");
            }
            else
            {
                WriterLine("Anulowano usunięcie zadania '" + delTaskName + "'");
            }
        }
        else
        {
            WriterLine("Nie znaleziono zadania o nazwie '" + delTaskName + "'");
        }
    }
    static void Download_content(object state, ref List<DateTime> clock)
    {
        var data = (Tuple<taskinfo, int>)state;
        taskinfo task = data.Item1;
        int index = data.Item2;
        string file_save;
        int returnamount = 0;
        clock[index] = DateTime.Now;

        HtmlWeb web = new HtmlWeb();
        if (Uri.IsWellFormedUriString(task.url, UriKind.Absolute))
        {
            while (returnamount != 5)
            {
                HtmlDocument doc = web.Load(task.url);
                HtmlNode targetElement = doc.DocumentNode.SelectSingleNode(task.xPath);

                if (targetElement != null)
                {
                    string elementText = targetElement.OuterHtml;
                    string name;
                    string amount;
                    int startIndex = 0;
                    int lastIndex;
                    int dataamount = 0;
                    while ((startIndex = elementText.IndexOf(task.valueTags[0], startIndex)) != -1)
                    {
                        lastIndex = elementText.IndexOf(task.lastIndexTags[0], startIndex + task.valueTags[0].Length);
                        name = elementText.Substring(startIndex + task.valueTags[0].Length + task.cursorShifts[0], lastIndex - startIndex - task.valueTags[0].Length - task.cursorShifts[0]);
                        for (int i = 1; i < task.cursorShifts.Count; i++)
                        {
                            startIndex = elementText.IndexOf(task.valueTags[i], startIndex);
                            file_save = name.Replace(" |", "") + ".txt";
                            lastIndex = elementText.IndexOf(task.lastIndexTags[i], startIndex + task.valueTags[i].Length);
                            amount = elementText.Substring(startIndex + task.valueTags[i].Length + task.cursorShifts[i], lastIndex - startIndex - task.valueTags[i].Length - +task.cursorShifts[i]);
                            if (!Directory.Exists(task.name))
                            {
                                Directory.CreateDirectory(task.name);
                            }
                            if (task.cursorShifts.Count == 2)
                            {
                                File.AppendAllText(Path.Combine(task.name, file_save), $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " + amount + Environment.NewLine);
                            }
                            else if (i == 1)
                            {
                                File.AppendAllText(Path.Combine(task.name, file_save), $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " + amount + " ");
                            }
                            else if (i == task.cursorShifts.Count - 1)
                            {
                                File.AppendAllText(Path.Combine(task.name, file_save), amount + Environment.NewLine);
                            }
                            else
                            {
                                File.AppendAllText(Path.Combine(task.name, file_save), amount + " ");
                            }
                            //File.AppendAllText(Path.Combine(task.name, file_save), $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " + amount + Environment.NewLine);
                        }
                        dataamount++;
                    }
                    if (dataamount != 0)
                    {
                        WriterLine("Dane zadania '" + task.name + "' zostały zapisane, ilość danych wynosi: " + dataamount);
                        returnamount = 5;
                    }
                    else
                    {
                        returnamount++;
                        if (returnamount == 5)
                        {
                            WriterLine("Wystąpił problem z odczytaniem danych zadania '" + task.name + "'");
                        }
                    }
                }
                else
                {
                    returnamount++;
                    if (returnamount == 5)
                    {
                        WriterLine("Wystąpił problem z odczytaniem strony zadania '" + task.name + "'");
                    }
                }
            }
        }
        else
        {
            WriterLine("Wystąpił problem z odczytaniem strony zadania '" + task.name + "', ponieważ url jest nie poprawny");
        }
    }
    static void Main()
    {
        List<taskinfo> importTasks = new List<taskinfo>();
        List<Timer> timers = new List<Timer>();
        List<DateTime> clock = new List<DateTime>();
        bool duration = true;

        ImpFunction(ref importTasks);

        for (int i = 0; i < importTasks.Count; i++)
        {
            clock.Add(DateTime.Now);
            Tuple<taskinfo, int> data = new Tuple<taskinfo, int>(importTasks[i], i);
            timers.Add(new Timer(state => Download_content(state, ref clock), data, 0, 60 * importTasks[i].frequency * 1000));
        }
        WriterLine("-----------------------------------------------");
        WriterLine("Program do Analizy Danych Rynkowych");
        WriterLine("Aby wyświetlić menu kliknij 'M'");
        WriterLine("Aby zakończyć działanie programu naciśnij 'Enter'");
        WriterLine("-----------------------------------------------");
        while (duration)
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                Console.Write("\b \b");
            } while ((keyInfo.Key != ConsoleKey.M) && (keyInfo.Key != ConsoleKey.Enter) && (keyInfo.Key != ConsoleKey.D1) && (keyInfo.Key != ConsoleKey.D2) && (keyInfo.Key != ConsoleKey.D3) && (keyInfo.Key != ConsoleKey.D4));
            switch (keyInfo.Key)
            {
                case ConsoleKey.M:
                    Console.Clear();
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine("Kliknij 1 aby dodać zadanie");
                    Console.WriteLine("Kliknij 2 aby usunąć zadanie");
                    Console.WriteLine("Kliknij 3 aby wykonać migawkę strony");
                    Console.WriteLine("Kliknij 4 aby zobaczyć czasy zadań");
                    Console.WriteLine("Kliknij enter aby zakonczyc dzialanie programu");
                    Console.WriteLine("-----------------------------------------------");
                    break;
                case ConsoleKey.D1:
                    ExpFunction(ref importTasks);
                    int count = importTasks.Count - 1;
                    clock.Add(DateTime.Now);
                    Tuple<taskinfo, int> data = new Tuple<taskinfo, int>(importTasks[count], count);
                    timers.Add(new Timer(state => Download_content(state, ref clock), data, 0, 60 * importTasks[count].frequency * 1000));
                    break;
                case ConsoleKey.D2:
                    DelFunction(ref importTasks, ref timers, ref clock);
                    break;
                case ConsoleKey.D3:
                    SnapShotHtml();
                    break;
                case ConsoleKey.D4:
                    TimersInfo(clock, importTasks);
                    break;
                default:
                    WriterLine("Zakończono działanie programu!");
                    duration = false;
                    break;
            }
        }
    }
}