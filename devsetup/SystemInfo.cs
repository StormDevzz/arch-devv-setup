// SystemInfo.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

public static class SystemInfo
{
    // Получение информации о системе
    public static void DisplaySystemInfo()
    {
        Logger.Logger.Info("=== System Information / Системная информация ===");
        
        // Операционная система
        Logger.Logger.Info($"Operating System: {RuntimeInformation.OSDescription}");
        
        // Архитектура
        Logger.Logger.Info($"Architecture: {RuntimeInformation.OSArchitecture}");
        
        // Версия .NET
        Logger.Logger.Info($".NET Version: {Environment.Version}");
        
        // Информация о процессоре
        try
        {
            var cpuInfo = GetCpuInfo();
            Logger.Logger.Info($"CPU: {cpuInfo}");
        }
        catch (Exception ex)
        {
            Logger.Logger.Warning($"Could not get CPU info: {ex.Message}");
        }
        
        // Информация о памяти
        try
        {
            var memoryInfo = GetMemoryInfo();
            Logger.Logger.Info($"Memory: {memoryInfo}");
        }
        catch (Exception ex)
        {
            Logger.Logger.Warning($"Could not get memory info: {ex.Message}");
        }
        
        // Дисковое пространство
        try
        {
            var diskInfo = GetDiskInfo();
            Logger.Logger.Info($"Disk Space: {diskInfo}");
        }
        catch (Exception ex)
        {
            Logger.Logger.Warning($"Could not get disk info: {ex.Message}");
        }
        
        // Сетевые интерфейсы
        try
        {
            var networkInfo = GetNetworkInfo();
            Logger.Logger.Info($"Network Interfaces: {networkInfo}");
        }
        catch (Exception ex)
        {
            Logger.Logger.Warning($"Could not get network info: {ex.Message}");
        }
        
        Logger.Logger.Info("=== End System Information / Конец системной информации ===");
    }
    
    private static string GetCpuInfo()
    {
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/lscpu",
                    Arguments = "",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            var lines = output.Split('\n');
            var modelLine = Array.Find(lines, line => line.StartsWith("Model name:"));
            var coresLine = Array.Find(lines, line => line.StartsWith("CPU(s):"));
            
            string model = modelLine?.Split(':')[1]?.Trim() ?? "Unknown";
            string cores = coresLine?.Split(':')[1]?.Trim() ?? "Unknown";
            
            return $"{model} ({cores} cores)";
        }
        catch
        {
            return "Unable to get CPU info";
        }
    }
    
    private static string GetMemoryInfo()
    {
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/free",
                    Arguments = "-h",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            var lines = output.Split('\n');
            var memLine = Array.Find(lines, line => line.StartsWith("Mem:"));
            
            if (memLine != null)
            {
                var parts = memLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    return $"{parts[1]} total, {parts[2]} used, {parts[3]} free";
                }
            }
            
            return "Unable to parse memory info";
        }
        catch
        {
            return "Unable to get memory info";
        }
    }
    
    private static string GetDiskInfo()
    {
        try
        {
            var drive = new DriveInfo("/");
            long totalSpace = drive.TotalSize;
            long freeSpace = drive.AvailableFreeSpace;
            
            string totalSize = FormatBytes(totalSpace);
            string freeSize = FormatBytes(freeSpace);
            string usedSize = FormatBytes(totalSpace - freeSpace);
            
            return $"{usedSize} used / {totalSize} total ({freeSize} free)";
        }
        catch
        {
            return "Unable to get disk info";
        }
    }
    
    private static string GetNetworkInfo()
    {
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/ip",
                    Arguments = "addr show",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            var lines = output.Split('\n');
            var interfaces = new List<string>();
            
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("inet ") && !line.Contains("127.0.0.1"))
                {
                    var parts = line.Trim().Split();
                    if (parts.Length > 1)
                    {
                        interfaces.Add(parts[1]);
                    }
                }
            }
            
            return interfaces.Count > 0 ? string.Join(", ", interfaces) : "No active interfaces";
        }
        catch
        {
            return "Unable to get network info";
        }
    }
    
    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
    
    // Проверка системных требований
    public static bool CheckSystemRequirements()
    {
        Logger.Logger.Info("Checking system requirements...");
        
        bool allRequirementsMet = true;
        
        // Проверка Arch Linux
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/pacman",
                    Arguments = "-Qi archlinux-keyring",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            
            if (proc.ExitCode == 0)
            {
                Logger.Logger.Success("Arch Linux detected / Arch Linux обнаружен");
            }
            else
            {
                Logger.Logger.Warning("This may not be Arch Linux / Это может быть не Arch Linux");
                allRequirementsMet = false;
            }
        }
        catch
        {
            Logger.Logger.Error("Could not verify Arch Linux / Не удалось проверить Arch Linux");
            allRequirementsMet = false;
        }
        
        // Проверка доступного места на диске (минимум 5GB)
        try
        {
            var drive = new DriveInfo("/");
            long freeSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
            
            if (freeSpaceGB >= 5)
            {
                Logger.Logger.Success($"Sufficient disk space: {freeSpaceGB}GB available / Достаточно места на диске: {freeSpaceGB}GB доступно");
            }
            else
            {
                Logger.Logger.Warning($"Low disk space: only {freeSpaceGB}GB available / Мало места на диске: только {freeSpaceGB}GB доступно");
                allRequirementsMet = false;
            }
        }
        catch
        {
            Logger.Logger.Error("Could not check disk space / Не удалось проверить место на диске");
            allRequirementsMet = false;
        }
        
        // Проверка интернет-соединения
        if (Utils.Utils.HasInternetConnection())
        {
            Logger.Logger.Success("Internet connection available / Интернет-соединение доступно");
        }
        else
        {
            Logger.Logger.Error("No internet connection / Нет интернет-соединения");
            allRequirementsMet = false;
        }
        
        return allRequirementsMet;
    }
}
