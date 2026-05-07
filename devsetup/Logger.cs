// Logger.cs
using System;

public static class Logger
{
    // Вывод сообщения с меткой времени
    public static void Info(string message)
    {
        Console.WriteLine($"[INFO] {DateTime.Now:HH:mm:ss} - {message}");
    }

    public static void Error(string message)
    {
        Console.Error.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}");
    }
}
