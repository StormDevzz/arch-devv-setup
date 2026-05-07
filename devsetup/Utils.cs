// Utils.cs
using System;
using System.Diagnostics;

public static class Utils
{
    // Выполняет команду Bash и выводит результат
    public static void RunCommand(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string err = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Console.WriteLine(output);
        if (!string.IsNullOrWhiteSpace(err))
            Console.Error.WriteLine(err);
    }

    // Проверяет, установлен ли пакет через pacman
    public static bool IsPackageInstalled(string pkg)
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/pacman",
                Arguments = $"-Qi {pkg}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        proc.Start();
        proc.WaitForExit();
        return proc.ExitCode == 0;
    }

    // Проверяет, активен ли сервис (systemd)
    public static bool IsServiceActive(string serviceName)
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/systemctl",
                Arguments = $"is-active --quiet {serviceName}.service",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        proc.Start();
        proc.WaitForExit();
        return proc.ExitCode == 0;
    }
}
