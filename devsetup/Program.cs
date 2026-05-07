// Program.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Logger;
using Utils;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Logger.Info("Запуск утилиты настройки разработки");
            Logger.Info("Starting development setup utility");
            
            // Проверка прав sudo / Check sudo privileges
            if (!Utils.HasSudoPrivileges())
            {
                Logger.Error("Требуются права sudo для установки пакетов");
                Logger.Error("Sudo privileges required for package installation");
                return;
            }
            
            // Обновление системы перед установкой / Update system before installation
            Logger.Info("Обновление системы...");
            Utils.RunCommand("sudo pacman -Syu --noconfirm");
            
            // Установка пакетов / Install packages
            var packages = Config.Packages;
            int installedCount = 0;
            
            foreach (var pkg in packages)
            {
                if (Utils.IsPackageInstalled(pkg))
                {
                    Logger.Info($"Пакет {pkg} уже установлен, пропуск. / Package {pkg} already installed, skipping.");
                }
                else
                {
                    Logger.Info($"Установка пакета: {pkg} / Installing package: {pkg}");
                    Utils.RunCommand($"sudo pacman -S --noconfirm {pkg}");
                    installedCount++;
                    await Task.Delay(1000); // Небольшая задержка для стабильности / Small delay for stability
                }
            }
            
            Logger.Info($"Установлено {installedCount} новых пакетов / Installed {installedCount} new packages");
            
            // Включение и запуск сервисов / Enable and start services
            var services = Config.Services;
            foreach (var svc in services)
            {
                if (Utils.IsServiceActive(svc))
                {
                    Logger.Info($"Сервис {svc} уже активен / Service {svc} is already active");
                }
                else
                {
                    Logger.Info($"Включение и запуск сервиса: {svc} / Enabling and starting service: {svc}");
                    Utils.RunCommand($"sudo systemctl enable --now {svc}.service");
                }
            }
            
            // Запуск Python скрипта для pip пакетов / Run Python script for pip packages
            Logger.Info("Установка Python пакетов... / Installing Python packages...");
            Utils.RunCommand("python3 setup.py");
            
            Logger.Info("Настройка завершена успешно! / Setup completed successfully!");
            Console.WriteLine("\n=== Setup Complete! ===");
            Console.WriteLine("Перезагрузите систему для применения всех изменений. / Reboot your system to apply all changes.");
        }
        catch (Exception ex)
        {
            Logger.Error($"Ошибка при выполнении установки: {ex.Message} / Error during installation: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
