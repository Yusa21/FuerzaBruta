// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        //Solo funciona cuando pongo la ruta absoluta y nose porque
        string[] passwordDictionary = File.ReadAllLines("..\\..\\..\\passwords.txt");
        Console.WriteLine($"Diccionario cargado con {passwordDictionary.Length} contraseñas");
        
        //Elijo ! love you
        string targetHash = CalculateHash("! love you");

        Console.WriteLine($"Contraseña objetivo: ! love you");
        Console.WriteLine($"Hash objetivo: {targetHash}");
        
        Console.WriteLine("\nIniciando ataque de fuerza bruta...");

        bool found = false;
        int attempts = 0;

        foreach (string password in passwordDictionary)
        {
            attempts++;
            string currentHash = CalculateHash(password);

            if (currentHash == targetHash)
            {
                found = true;
            
                
                Console.WriteLine($"\n¡Contraseña encontrada!");
                Console.WriteLine($"Contraseña: {password}");
                Console.WriteLine($"Intentos realizados: {attempts:N0}");
                break;
            }
        }

        if (!found)
        {
            Console.WriteLine("\nNo se encontró la contraseña en el diccionario.");
        }
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