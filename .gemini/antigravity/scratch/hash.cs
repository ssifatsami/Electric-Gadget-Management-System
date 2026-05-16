using System;
using System.Security.Cryptography;
using System.Text;

public class Program {
    public static void Main() {
        string password = "superadmin123";
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        Console.WriteLine(sb.ToString());
    }
}
