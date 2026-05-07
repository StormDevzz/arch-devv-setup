// Utils.cs
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public static class Utils
{
    // Выполняет команду Bash и выводит результат
    public static void RunCommand(string command)
    {
        try
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
            
            if (!string.IsNullOrWhiteSpace(output))
                Console.WriteLine(output);
            if (!string.IsNullOrWhiteSpace(err))
                Console.Error.WriteLine(err);
                
            if (process.ExitCode != 0)
            {
                Logger.Logger.Error($"Команда завершилась с ошибкой (код {process.ExitCode}): {command}");
                Logger.Logger.Error($"Command failed with exit code {process.ExitCode}: {command}");
            }
        }
        catch (Exception ex)
        {
            Logger.Logger.Error($"Ошибка выполнения команды: {ex.Message} / Command execution error: {ex.Message}");
            throw;
        }
    }

    // Проверяет, установлен ли пакет через pacman
    public static bool IsPackageInstalled(string pkg)
    {
        try
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
        catch
        {
            return false;
        }
    }

    // Проверяет, активен ли сервис (systemd)
    public static bool IsServiceActive(string serviceName)
    {
        try
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
        catch
        {
            return false;
        }
    }
    
    // Проверяет наличие прав sudo
    public static bool HasSudoPrivileges()
    {
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/sudo",
                    Arguments = "-n true",
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
        catch
        {
            return false;
        }
    }
    
    // Проверяет подключение к интернету
    public static bool HasInternetConnection()
    {
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/ping",
                    Arguments = "-c 1 -W 3 8.8.8.8",
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
        catch
        {
            return false;
        }
    }
}
