// Программа.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Logger;
using Utils;
class Program
{
    static void Main(string[] args)
    {
        Logger.Info("Запуск утилиты настройки разработки");
        // Список пакетов для установки
        var packages = Config.Packages;
        foreach (var pkg in packages)
        {
            if (Utils.IsPackageInstalled(pkg))
                Logger.Info($"Пакет {pkg} уже установлен, пропуск.");
            else
                Utils.RunCommand($"sudo pacman -S --noconfirm {pkg}");
        }
        // Включаем и запускаем сервисы
        var services = Config.Services;
        foreach (var svc in services)
        {
            RunCommand($"sudo systemctl enable --now {svc}.service");
        }
        // Запускаем дополнительный Python‑скрипт для pip‑пакетов
        RunCommand("python setup.py");
        Console.WriteLine("Setup complete!");
    }

    static void RunCommand(string command)
    {
        var process = new Process();
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{command}\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string err = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Console.WriteLine(output);
        if (!string.IsNullOrWhiteSpace(err))
        {
            Console.Error.WriteLine(err);
        }
    }
}
