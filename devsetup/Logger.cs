// Logger.cs
using System;
using System.IO;

public static class Logger
{
    private static readonly string LogFile = "devsetup.log";
    
    // Вывод сообщения с меткой времени
    public static void Info(string message)
    {
        Log("INFO", message);
    }

    public static void Error(string message)
    {
        Log("ERROR", message);
    }
    
    public static void Warning(string message)
    {
        Log("WARNING", message);
    }
    
    public static void Success(string message)
    {
        Log("SUCCESS", message);
    }
    
    private static void Log(string level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"[{level}] {timestamp} - {message}";
        
        // Вывод в консоль / Console output
        if (level == "ERROR")
        {
            Console.Error.WriteLine(logEntry);
        }
        else if (level == "SUCCESS")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(logEntry);
            Console.ResetColor();
        }
        else if (level == "WARNING")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(logEntry);
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine(logEntry);
        }
        
        // Запись в файл / Write to file
        try
        {
            File.AppendAllText(LogFile, logEntry + Environment.NewLine);
        }
        catch
        {
            // Игнорируем ошибки логирования / Ignore logging errors
        }
    }
}
