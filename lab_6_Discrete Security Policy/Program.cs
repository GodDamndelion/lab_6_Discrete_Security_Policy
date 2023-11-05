using System.Security.Cryptography;

namespace lab_6_Discrete_Security_Policy;

internal class Program
{
    public enum RightsSet { Ban, Read, Write, RAndW, Full };
    public enum Right { Read, Write, Grant };

    static Dictionary<RightsSet, Dictionary<Right, bool>> rightsSetsRight = new Dictionary<RightsSet, Dictionary<Right, bool>>();

    public class User
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public User(string name, string login, string password)
        {
            Name = name;
            Login = login;
            Password = password;
        }
    }

    static List<User> users = new List<User>();

    public class File
    {
        public string Name { get; set; }
        public string Information { get; set; }

        public File(string name, string information)
        {
            Name = name;
            Information = information;
        }
    }

    static List<File> files = new List<File>();

    static RightsSet[,] accessMatrix = new RightsSet[6,5];

    static public void Initialization()
    {
        rightsSetsRight[RightsSet.Ban] = new Dictionary<Right, bool>();
        rightsSetsRight[RightsSet.Read] = new Dictionary<Right, bool>();
        rightsSetsRight[RightsSet.Write] = new Dictionary<Right, bool>();
        rightsSetsRight[RightsSet.RAndW] = new Dictionary<Right, bool>();
        rightsSetsRight[RightsSet.Full] = new Dictionary<Right, bool>();

        rightsSetsRight[RightsSet.Ban][Right.Read] = false;
        rightsSetsRight[RightsSet.Ban][Right.Write] = false;
        rightsSetsRight[RightsSet.Ban][Right.Grant] = false;

        rightsSetsRight[RightsSet.Read][Right.Read] = true;
        rightsSetsRight[RightsSet.Read][Right.Write] = false;
        rightsSetsRight[RightsSet.Read][Right.Grant] = false;

        rightsSetsRight[RightsSet.Write][Right.Read] = false;
        rightsSetsRight[RightsSet.Write][Right.Write] = true;
        rightsSetsRight[RightsSet.Write][Right.Grant] = false;

        rightsSetsRight[RightsSet.RAndW][Right.Read] = true;
        rightsSetsRight[RightsSet.RAndW][Right.Write] = true;
        rightsSetsRight[RightsSet.RAndW][Right.Grant] = false;

        rightsSetsRight[RightsSet.Full][Right.Read] = true;
        rightsSetsRight[RightsSet.Full][Right.Write] = true;
        rightsSetsRight[RightsSet.Full][Right.Grant] = true;


        users.Add(new User("Admin", "best_admin_2007", "admin password"));
        users.Add(new User("User 1", "user1", "user1password"));
        users.Add(new User("User 2", "user2", "user2password"));
        users.Add(new User("User 3", "user3", "user3password"));
        users.Add(new User("User 4", "user4", "user4password"));
        users.Add(new User("User 5", "user5", "user5password"));

        files.Add(new File("File 1", "Information from file 1"));
        files.Add(new File("File 2", "Information from file 2"));
        files.Add(new File("File 3", "Information from file 3"));
        files.Add(new File("File 4", "Information from file 4"));
        files.Add(new File("File 5", "Information from file 5"));

        for (int j = 0; j < files.Count; ++j)
        {
            accessMatrix[0, j] = RightsSet.Full;
        }

        for (int i = 1; i < users.Count; ++i)
        {
            for (int j = 0; j < files.Count; ++j)
            {
                accessMatrix[i, j] = (RightsSet)RandomNumberGenerator.GetInt32(Enum.GetValues(typeof(RightsSet)).Length);
            }
        }
    }

    static void Print(RightsSet[,] am)
    {
        Console.Write("\t\t");
        for (int i = 0; i < files.Count; ++i)
        {
            Console.Write(files[i].Name + "\t\t");
        }
        Console.WriteLine();

        for (int i = 0; i < am.GetLength(0); ++i)
        {
            Console.Write(users[i].Name + "\t\t");
            for (int j = 0; j < am.GetLength(1); ++j)
            {
                Console.Write(am[i, j].ToString() + "\t\t");
            }
            Console.WriteLine();
        }
    }

    static void Main()
    {
        Initialization();
        Print(accessMatrix);

        while (true)
        {
            Console.WriteLine();
            string login;
            string password;
            Console.WriteLine("Для входа в систему введите логин и пароль:");
            User? currentUser = null;

            while (currentUser == null)
            {
                login = Console.ReadLine()!;
                password = Console.ReadLine()!;
                Console.WriteLine();

                currentUser = users.FirstOrDefault(x => x.Login == login && x.Password == password);
                
                if (currentUser == null)
                    Console.WriteLine("Пользователь с введёнными логином и паролем не найден! Попробуйте ещё раз!");
            }

            Console.WriteLine($"Привет, {currentUser.Name}!");
            Console.WriteLine();

            bool quit = false;
            string message;
            string[] commands;
            string filename;
            File? currentFile;
            while (!quit)
            {
                Console.Write($"{currentUser.Name}> ");
                message = Console.ReadLine()!;
                commands = message.Split(' ');
                switch (message)
                {
                    case "":
                        break;

                    case "quit":
                        quit = true;
                        break;

                    case "cls":
                        Console.Clear();
                        Print(accessMatrix);
                        Console.WriteLine();
                        break;

                    default:
                        switch (commands[0])
                        {
                            case "read":
                                filename = message.Substring(5);
                                currentFile = files.FirstOrDefault(x => x.Name == filename);
                                if (currentFile == null)
                                {
                                    Console.WriteLine($"Файл с именем {filename} не найден!");
                                }
                                else
                                {
                                    if (rightsSetsRight[accessMatrix[users.IndexOf(currentUser), files.IndexOf(currentFile)]][Right.Read] == false)
                                    {
                                        Console.WriteLine($"Права на чтение файла {filename} отсутствуют!");
                                    }
                                    else
                                    {
                                        Console.WriteLine(currentFile.Information);
                                    }
                                }
                                break;

                            case "write":
                                filename = message.Substring(6);
                                currentFile = files.FirstOrDefault(x => x.Name == filename);
                                if (currentFile == null)
                                {
                                    Console.WriteLine($"Файл с именем {filename} не найден!");
                                }
                                else
                                {
                                    if (rightsSetsRight[accessMatrix[users.IndexOf(currentUser), files.IndexOf(currentFile)]][Right.Write] == false)
                                    {
                                        Console.WriteLine($"Права на запись в файл {filename} отсутствуют!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Введите новую информацию для файла:");
                                        currentFile.Information = Console.ReadLine()!;
                                    }
                                }
                                break;

                            case "grant":
                                filename = message.Substring(6);
                                currentFile = files.FirstOrDefault(x => x.Name == filename);
                                if (currentFile == null)
                                {
                                    Console.WriteLine($"Файл с именем {filename} не найден!");
                                }
                                else
                                {
                                    if (rightsSetsRight[accessMatrix[users.IndexOf(currentUser), files.IndexOf(currentFile)]][Right.Grant] == false)
                                    {
                                        Console.WriteLine($"Права на изменение уровня доступа к файлу {filename} отсутствуют!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Введите уровень доступа и имя пользователя:");
                                        message = Console.ReadLine()!;
                                        commands = message.Split(' ');
                                        string userName;
                                        User? user = null;
                                        switch (commands[0])
                                        {
                                            case "Ban":
                                                userName = message.Substring(4);
                                                user = users.FirstOrDefault(x => x.Name == userName);
                                                if (user == null)
                                                {
                                                    Console.WriteLine($"Пользователь с именем {userName} не найден!");
                                                }
                                                else
                                                {
                                                    accessMatrix[users.IndexOf(user), files.IndexOf(currentFile)] = RightsSet.Ban;
                                                    Console.WriteLine($"Уровень доступа к файлу {filename} пользователя {userName} изменён на {RightsSet.Ban}.");
                                                    Console.WriteLine();
                                                    Print(accessMatrix);
                                                    Console.WriteLine();
                                                }
                                                break;

                                            case "Read":
                                                userName = message.Substring(5);
                                                user = users.FirstOrDefault(x => x.Name == userName);
                                                if (user == null)
                                                {
                                                    Console.WriteLine($"Пользователь с именем {userName} не найден!");
                                                }
                                                else
                                                {
                                                    accessMatrix[users.IndexOf(user), files.IndexOf(currentFile)] = RightsSet.Read;
                                                    Console.WriteLine($"Уровень доступа к файлу {filename} пользователя {userName} изменён на {RightsSet.Read}.");
                                                    Console.WriteLine();
                                                    Print(accessMatrix);
                                                    Console.WriteLine();
                                                }
                                                break;

                                            case "Write":
                                                userName = message.Substring(6);
                                                user = users.FirstOrDefault(x => x.Name == userName);
                                                if (user == null)
                                                {
                                                    Console.WriteLine($"Пользователь с именем {userName} не найден!");
                                                }
                                                else
                                                {
                                                    accessMatrix[users.IndexOf(user), files.IndexOf(currentFile)] = RightsSet.Write;
                                                    Console.WriteLine($"Уровень доступа к файлу {filename} пользователя {userName} изменён на {RightsSet.Write}.");
                                                    Console.WriteLine();
                                                    Print(accessMatrix);
                                                    Console.WriteLine();
                                                }
                                                break;

                                            case "RAndW":
                                                userName = message.Substring(6);
                                                user = users.FirstOrDefault(x => x.Name == userName);
                                                if (user == null)
                                                {
                                                    Console.WriteLine($"Пользователь с именем {userName} не найден!");
                                                }
                                                else
                                                {
                                                    accessMatrix[users.IndexOf(user), files.IndexOf(currentFile)] = RightsSet.RAndW;
                                                    Console.WriteLine($"Уровень доступа к файлу {filename} пользователя {userName} изменён на {RightsSet.RAndW}.");
                                                    Console.WriteLine();
                                                    Print(accessMatrix);
                                                    Console.WriteLine();
                                                }
                                                break;

                                            case "Full":
                                                userName = message.Substring(5);
                                                user = users.FirstOrDefault(x => x.Name == userName);
                                                if (user == null)
                                                {
                                                    Console.WriteLine($"Пользователь с именем {userName} не найден!");
                                                }
                                                else
                                                {
                                                    accessMatrix[users.IndexOf(user), files.IndexOf(currentFile)] = RightsSet.Full;
                                                    Console.WriteLine($"Уровень доступа к файлу {filename} пользователя {userName} изменён на {RightsSet.Full}.");
                                                    Console.WriteLine();
                                                    Print(accessMatrix);
                                                    Console.WriteLine();
                                                }
                                                break;

                                            default:
                                                Console.WriteLine($"Уровень доступа {commands[0]} не существует!");
                                                break;
                                        }
                                    }
                                }
                                break;

                            default:
                                Console.WriteLine(message);
                                break;
                        }
                        break;
                }
            }
        }
    }
}