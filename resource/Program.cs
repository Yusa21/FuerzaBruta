using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

class Program
{
    private static readonly object consoleLock = new object();
    private static volatile bool passwordFound = false;
    private static string foundPassword = string.Empty;

    static void Main()
    {
        string[] passwordDictionary = File.ReadAllLines("..\\..\\..\\2151220-passwords.txt");
        Console.WriteLine($"Diccionario cargado con {passwordDictionary.Length} contraseñas");

        string targetPassword = "~~zhou075278";
        string targetHash = CalculateHash(targetPassword);

        Console.WriteLine($"Contraseña objetivo: {targetPassword}");
        Console.WriteLine($"Hash objetivo: {targetHash}");

        // Saca la cantidad de procesadores de mi ordenador
        int processorCount = Environment.ProcessorCount;
       

        Console.WriteLine($"\nUsando {processorCount} threads");
        Console.WriteLine("\nIniciando ataque de fuerza bruta...");

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Parte el diccionario
        var chunks = SplitList(passwordDictionary, processorCount);
        var tasks = new List<Task>();

        //Una task por cada parte
        foreach (var chunk in chunks)
        {
            tasks.Add(Task.Run(() => ProcessChunk(chunk, targetHash)));
        }

        // Espera a que una encuentre la contrasena o que todas acaben
        Task.WaitAll(tasks.ToArray());

        stopwatch.Stop();

        if (passwordFound)
        {
            Console.WriteLine($"\n¡Contraseña encontrada!");
            Console.WriteLine($"Contraseña: {foundPassword}");
            Console.WriteLine($"Tiempo total: {stopwatch.ElapsedMilliseconds / 1000.0:F2} segundos");
        }
        else
        {
            Console.WriteLine("\nNo se encontró la contraseña en el diccionario.");
        }
    }

    static void ProcessChunk(IEnumerable<string> passwords, string targetHash)
    {
        foreach (string password in passwords)
        {
            if (passwordFound) return; // Acabe antes si se encuntra en otro hilo
            
            string currentHash = CalculateHash(password);

            if (currentHash == targetHash)
            {
                passwordFound = true;
                foundPassword = password;
                return;
            }
        }
    }

    static List<List<T>> SplitList<T>(IList<T> source, int chunks)
    {
        var result = new List<List<T>>();
        int chunkSize = (int)Math.Ceiling(source.Count / (double)chunks);

        for (int i = 0; i < source.Count; i += chunkSize)
        {
            result.Add(source.Skip(i).Take(chunkSize).ToList());
        }

        return result;
    }

    static string CalculateHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}