// SecurityChecker.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public static class SecurityChecker
{
    public class SecurityCheckResult
    {
        public string CheckName { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }
        public string Recommendation { get; set; }
        public Severity Severity { get; set; }
    }

    public enum Severity
    {
        Info,
        Warning,
        Critical
    }

    // Выполнить все проверки безопасности
    public static List<SecurityCheckResult> RunAllSecurityChecks()
    {
        Logger.Logger.Info("=== Security Checks / Проверки Безопасности ===");
        
        var results = new List<SecurityCheckResult>();
        
        // Проверка обновлений системы
        results.Add(CheckSystemUpdates());
        
        // Проверка файрвола
        results.Add(CheckFirewall());
        
        // Проверка SSH конфигурации
        results.Add(CheckSSHConfiguration());
        
        // Проверка пользователей с sudo правами
        results.Add(CheckSudoUsers());
        
        // Проверка открытых портов
        results.Add(CheckOpenPorts());
        
        // Проверка антивируса
        results.Add(CheckAntivirus());
        
        // Проверка прав на критические файлы
        results.Add(CheckCriticalFilePermissions());
        
        // Проверка парольной политики
        results.Add(CheckPasswordPolicy());
        
        // Проверка автоматического входа
        results.Add(CheckAutoLogin());
        
        // Проверка шифрования диска
        results.Add(CheckDiskEncryption());
        
        // Проверка безопасности сети
        results.Add(CheckNetworkSecurity());
        
        // Проверка логирования
        results.Add(CheckLoggingConfiguration());
        
        DisplayResults(results);
        return results;
    }
    
    private static SecurityCheckResult CheckSystemUpdates()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "System Updates / Обновления системы",
            Severity = Severity.Warning
        };
        
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/checkupdates",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            if (string.IsNullOrWhiteSpace(output))
            {
                result.Passed = true;
                result.Message = "System is up to date / Система обновлена";
                result.Recommendation = "Continue regular updates / Продолжайте регулярные обновления";
            }
            else
            {
                var updateCount = output.Split('\n').Count(line => !string.IsNullOrWhiteSpace(line));
                result.Passed = false;
                result.Message = $"{updateCount} updates available / Доступно {updateCount} обновлений";
                result.Recommendation = "Run 'sudo pacman -Syu' to update / Выполните 'sudo pacman -Syu' для обновления";
                result.Severity = updateCount > 10 ? Severity.Critical : Severity.Warning;
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check updates: {ex.Message}";
            result.Recommendation = "Run 'sudo pacman -Syu' manually / Выполните 'sudo pacman -Syu' вручную";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckFirewall()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Firewall Status / Статус файрвола",
            Severity = Severity.Critical
        };
        
        try
        {
            // Проверка UFW (если установлен)
            var ufwProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/ufw",
                    Arguments = "status",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            ufwProc.Start();
            string ufwOutput = ufwProc.StandardOutput.ReadToEnd();
            ufwProc.WaitForExit();
            
            if (ufwOutput.Contains("Status: active"))
            {
                result.Passed = true;
                result.Message = "UFW firewall is active / Файрвол UFW активен";
                result.Recommendation = "Firewall is properly configured / Файрвол настроен правильно";
            }
            else
            {
                // Проверка iptables
                var iptablesProc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/usr/sbin/iptables",
                        Arguments = "-L",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                iptablesProc.Start();
                iptablesProc.WaitForExit();
                
                if (iptablesProc.ExitCode == 0)
                {
                    result.Passed = true;
                    result.Message = "iptables rules exist / Правила iptables существуют";
                    result.Recommendation = "Consider using UFW for easier management / Рассмотрите UFW для простого управления";
                }
                else
                {
                    result.Passed = false;
                    result.Message = "No firewall detected / Файрвол не обнаружен";
                    result.Recommendation = "Install and configure UFW: 'sudo pacman -S ufw && sudo ufw enable' / Установите и настройте UFW";
                }
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check firewall: {ex.Message}";
            result.Recommendation = "Install UFW: 'sudo pacman -S ufw' / Установите UFW";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckSSHConfiguration()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "SSH Configuration / Конфигурация SSH",
            Severity = Severity.Warning
        };
        
        var sshConfig = "/etc/ssh/sshd_config";
        
        if (!File.Exists(sshConfig))
        {
            result.Passed = true;
            result.Message = "SSH server not installed / SSH сервер не установлен";
            result.Recommendation = "SSH is not installed, no action needed / SSH не установлен, действия не требуются";
            return result;
        }
        
        try
        {
            var configLines = File.ReadAllLines(sshConfig);
            var issues = new List<string>();
            
            // Проверка PermitRootLogin
            var rootLoginLine = configLines.FirstOrDefault(line => 
                line.TrimStart().StartsWith("PermitRootLogin") && !line.TrimStart().StartsWith("#"));
            
            if (rootLoginLine != null)
            {
                var value = rootLoginLine.Split().LastOrDefault();
                if (value == "yes")
                {
                    issues.Add("Root login is permitted / Вход root разрешен");
                }
            }
            
            // Проверка PasswordAuthentication
            var passwordAuthLine = configLines.FirstOrDefault(line => 
                line.TrimStart().StartsWith("PasswordAuthentication") && !line.TrimStart().StartsWith("#"));
            
            if (passwordAuthLine != null)
            {
                var value = passwordAuthLine.Split().LastOrDefault();
                if (value == "yes")
            {
                    issues.Add("Password authentication is enabled / Аутентификация по паролю включена");
                }
            }
            
            // Проверка порта
            var portLine = configLines.FirstOrDefault(line => 
                line.TrimStart().StartsWith("Port") && !line.TrimStart().StartsWith("#"));
            
            if (portLine != null)
            {
                var value = portLine.Split().LastOrDefault();
                if (value == "22")
                {
                    issues.Add("SSH is running on default port 22 / SSH работает на стандартном порту 22");
                }
            }
            
            if (issues.Count == 0)
            {
                result.Passed = true;
                result.Message = "SSH configuration appears secure / Конфигурация SSH выглядит безопасной";
                result.Recommendation = "SSH is properly configured / SSH настроен правильно";
            }
            else
            {
                result.Passed = false;
                result.Message = $"SSH security issues found: {string.Join(", ", issues)}";
                result.Recommendation = "Consider: disable root login, use key authentication, change default port / Рассмотрите: отключите вход root, используйте ключи, смените порт";
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check SSH config: {ex.Message}";
            result.Recommendation = "Manually review /etc/ssh/sshd_config / Проверьте /etc/ssh/sshd_config вручную";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckSudoUsers()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Sudo Users / Пользователи с sudo",
            Severity = Severity.Warning
        };
        
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/getent",
                    Arguments = "group sudo",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            if (proc.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
            {
                var parts = output.Split(':');
                if (parts.Length > 3)
                {
                    var users = parts[3].Split(',');
                    result.Passed = true;
                    result.Message = $"Users with sudo: {string.Join(", ", users)}";
                    result.Recommendation = "Review sudo users regularly / Регулярно проверяйте пользователей с sudo";
                    
                    if (users.Length > 3)
                    {
                        result.Severity = Severity.Warning;
                        result.Recommendation += ". Consider reducing sudo users. / Рассмотрите уменьшение количества пользователей с sudo.";
                    }
                }
                else
                {
                    result.Passed = true;
                    result.Message = "Could not parse sudo group / Не удалось разобрать группу sudo";
                    result.Recommendation = "Manually check sudo users / Проверьте пользователей с sudo вручную";
                }
            }
            else
            {
                result.Passed = false;
                result.Message = "Could not check sudo group / Не удалось проверить группу sudo";
                result.Recommendation = "Ensure sudo group exists / Убедитесь что группа sudo существует";
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check sudo users: {ex.Message}";
            result.Recommendation = "Check /etc/group for sudo users / Проверьте /etc/group для пользователей sudo";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckOpenPorts()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Open Ports / Открытые порты",
            Severity = Severity.Warning
        };
        
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/netstat",
                    Arguments = "-tuln",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            var lines = output.Split('\n');
            var listeningPorts = new List<string>();
            
            foreach (var line in lines)
            {
                if (line.Contains("LISTEN"))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        var address = parts[3];
                        listeningPorts.Add(address);
                    }
                }
            }
            
            if (listeningPorts.Count == 0)
            {
                result.Passed = true;
                result.Message = "No listening ports detected / Слушающих портов не обнаружено";
                result.Recommendation = "System appears secure / Система выглядит безопасной";
            }
            else
            {
                var suspiciousPorts = listeningPorts.Where(port => 
                    port.Contains(":22") || port.Contains(":23") || port.Contains(":80") || 
                    port.Contains(":443") || port.Contains(":3389")).ToList();
                
                if (suspiciousPorts.Count > 0)
                {
                    result.Passed = false;
                    result.Message = $"Suspicious ports open: {string.Join(", ", suspiciousPorts)}";
                    result.Recommendation = "Review open ports and close unnecessary ones / Проверьте открытые порты и закройте ненужные";
                    result.Severity = Severity.Warning;
                }
                else
                {
                    result.Passed = true;
                    result.Message = $"Open ports: {string.Join(", ", listeningPorts)}";
                    result.Recommendation = "Monitor open ports regularly / Регулярно мониторьте открытые порты";
                }
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check open ports: {ex.Message}";
            result.Recommendation = "Install net-tools: 'sudo pacman -S net-tools' / Установите net-tools";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckAntivirus()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Antivirus / Антивирус",
            Severity = Severity.Info
        };
        
        try
        {
            var antivirusTools = new[] { "clamav", "chkrootkit", "rkhunter", "lynis" };
            var installedTools = new List<string>();
            
            foreach (var tool in antivirusTools)
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/usr/bin/which",
                        Arguments = tool,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                proc.WaitForExit();
                
                if (proc.ExitCode == 0)
                {
                    installedTools.Add(tool);
                }
            }
            
            if (installedTools.Count > 0)
            {
                result.Passed = true;
                result.Message = $"Security tools installed: {string.Join(", ", installedTools)}";
                result.Recommendation = "Security tools are installed / Инструменты безопасности установлены";
            }
            else
            {
                result.Passed = false;
                result.Message = "No security tools found / Инструменты безопасности не найдены";
                result.Recommendation = "Consider installing: 'sudo pacman -S clamav chkrootkit' / Рассмотрите установку антивируса";
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check antivirus: {ex.Message}";
            result.Recommendation = "Install security tools manually / Установите инструменты безопасности вручную";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckCriticalFilePermissions()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Critical File Permissions / Права на критические файлы",
            Severity = Severity.Critical
        };
        
        var criticalFiles = new[]
        {
            "/etc/passwd", "/etc/shadow", "/etc/group", "/etc/gshadow",
            "/etc/sudoers", "/etc/ssh/sshd_config", "/boot/grub/grub.cfg"
        };
        
        var issues = new List<string>();
        
        foreach (var file in criticalFiles)
        {
            if (File.Exists(file))
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    var permissions = fileInfo.UnixFileMode;
                    
                    // Проверка на права записи для других пользователей
                    if ((permissions & UnixFileMode.GroupWrite) != 0 || (permissions & UnixFileMode.OtherWrite) != 0)
                    {
                        issues.Add($"{file} has write permissions for group/others");
                    }
                }
                catch
                {
                    issues.Add($"Could not check permissions for {file}");
                }
            }
        }
        
        if (issues.Count == 0)
        {
            result.Passed = true;
            result.Message = "Critical file permissions are secure / Права на критические файлы безопасны";
            result.Recommendation = "File permissions are properly set / Права на файлы настроены правильно";
        }
        else
        {
            result.Passed = false;
            result.Message = $"Permission issues: {string.Join(", ", issues)}";
            result.Recommendation = "Review and fix file permissions / Проверьте и исправьте права на файлы";
            result.Severity = Severity.Critical;
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckPasswordPolicy()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Password Policy / Парольная политика",
            Severity = Severity.Warning
        };
        
        var pwConfig = "/etc/pam.d/passwd";
        
        if (File.Exists(pwConfig))
        {
            try
            {
                var configLines = File.ReadAllLines(pwConfig);
                var hasPasswordCheck = configLines.Any(line => 
                    line.Contains("pam_pwquality.so") || line.Contains("pam_cracklib.so"));
                
                if (hasPasswordCheck)
                {
                    result.Passed = true;
                    result.Message = "Password policy is configured / Парольная политика настроена";
                    result.Recommendation = "Password quality checks are enabled / Проверки качества паролей включены";
                }
                else
                {
                    result.Passed = false;
                    result.Message = "No password quality checks / Нет проверок качества паролей";
                    result.Recommendation = "Install and configure pam_pwquality / Установите и настройте pam_pwquality";
                }
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Message = $"Could not check password policy: {ex.Message}";
                result.Recommendation = "Manually check /etc/pam.d/passwd / Проверьте /etc/pam.d/passwd вручную";
            }
        }
        else
        {
            result.Passed = false;
            result.Message = "Password policy file not found / Файл парольной политики не найден";
            result.Recommendation = "Install pam_pwquality: 'sudo pacman -S pam_pwquality' / Установите pam_pwquality";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckAutoLogin()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Auto-login / Автоматический вход",
            Severity = Severity.Warning
        };
        
        var autoLoginFiles = new[]
        {
            "/etc/systemd/system/getty@tty1.service.d/autologin.conf",
            "/etc/lightdm/lightdm.conf",
            "/etc/gdm/custom.conf",
            "/etc/sddm.conf"
        };
        
        var autoLoginEnabled = false;
        
        foreach (var file in autoLoginFiles)
        {
            if (File.Exists(file))
            {
                try
                {
                    var content = File.ReadAllText(file);
                    if (content.Contains("autologin") && !content.Contains("#autologin"))
                    {
                        autoLoginEnabled = true;
                        break;
                    }
                }
                catch
                {
                    // Ignore file read errors
                }
            }
        }
        
        if (autoLoginEnabled)
        {
            result.Passed = false;
            result.Message = "Auto-login is enabled / Автоматический вход включен";
            result.Recommendation = "Disable auto-login for better security / Отключите авто-вход для лучшей безопасности";
            result.Severity = Severity.Warning;
        }
        else
        {
            result.Passed = true;
            result.Message = "Auto-login is disabled / Автоматический вход отключен";
            result.Recommendation = "Auto-login is properly disabled / Авто-вход правильно отключен";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckDiskEncryption()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Disk Encryption / Шифрование диска",
            Severity = Severity.Info
        };
        
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/lsblk",
                    Arguments = "-f",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            
            if (output.Contains("crypto"))
            {
                result.Passed = true;
                result.Message = "Disk encryption detected / Обнаружено шифрование диска";
                result.Recommendation = "Disk encryption is active / Шифрование диска активно";
            }
            else
            {
                result.Passed = false;
                result.Message = "No disk encryption detected / Шифрование диска не обнаружено";
                result.Recommendation = "Consider full disk encryption for better security / Рассмотрите полное шифрование диска";
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check disk encryption: {ex.Message}";
            result.Recommendation = "Manually verify disk encryption / Проверьте шифрование диска вручную";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckNetworkSecurity()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Network Security / Безопасность сети",
            Severity = Severity.Warning
        };
        
        try
        {
            // Проверка IPv6
            var ipv6Proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/sysctl",
                    Arguments = "net.ipv6.conf.all.disable_ipv6",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            ipv6Proc.Start();
            string ipv6Output = ipv6Proc.StandardOutput.ReadToEnd();
            ipv6Proc.WaitForExit();
            
            var issues = new List<string>();
            
            if (!ipv6Output.Contains("= 1"))
            {
                issues.Add("IPv6 is enabled / IPv6 включен");
            }
            
            // Проверка IP forwarding
            var ipForwardProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/sysctl",
                    Arguments = "net.ipv4.ip_forward",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            ipForwardProc.Start();
            string ipForwardOutput = ipForwardProc.StandardOutput.ReadToEnd();
            ipForwardProc.WaitForExit();
            
            if (ipForwardOutput.Contains("= 1"))
            {
                issues.Add("IP forwarding is enabled / IP forwarding включен");
            }
            
            if (issues.Count == 0)
            {
                result.Passed = true;
                result.Message = "Network security settings are good / Настройки безопасности сети хорошие";
                result.Recommendation = "Network is properly configured / Сеть настроена правильно";
            }
            else
            {
                result.Passed = false;
                result.Message = $"Network security issues: {string.Join(", ", issues)}";
                result.Recommendation = "Review network security settings / Проверьте настройки безопасности сети";
            }
        }
        catch (Exception ex)
        {
            result.Passed = false;
            result.Message = $"Could not check network security: {ex.Message}";
            result.Recommendation = "Manually check network settings / Проверьте сетевые настройки вручную";
        }
        
        return result;
    }
    
    private static SecurityCheckResult CheckLoggingConfiguration()
    {
        var result = new SecurityCheckResult
        {
            CheckName = "Logging Configuration / Конфигурация логирования",
            Severity = Severity.Info
        };
        
        var logFiles = new[]
        {
            "/var/log/auth.log", "/var/log/secure", "/var/log/audit/audit.log"
        };
        
        var existingLogs = logFiles.Where(File.Exists).ToList();
        
        if (existingLogs.Count > 0)
        {
            result.Passed = true;
            result.Message = $"Security logs found: {string.Join(", ", existingLogs.Select(Path.GetFileName))}";
            result.Recommendation = "Security logging is configured / Логирование безопасности настроено";
        }
        else
        {
            result.Passed = false;
            result.Message = "No security logs found / Логи безопасности не найдены";
            result.Recommendation = "Ensure auditd is installed: 'sudo pacman -S auditd' / Убедитесь что auditd установлен";
        }
        
        return result;
    }
    
    private static void DisplayResults(List<SecurityCheckResult> results)
    {
        Logger.Logger.Info("=== Security Check Results / Результаты Проверок Безопасности ===");
        
        var criticalCount = results.Count(r => r.Severity == Severity.Critical && !r.Passed);
        var warningCount = results.Count(r => r.Severity == Severity.Warning && !r.Passed);
        var passedCount = results.Count(r => r.Passed);
        
        Console.WriteLine($"\n📊 Summary / Сводка:");
        Console.WriteLine($"   ✅ Passed: {passedCount}");
        Console.WriteLine($"   ⚠️  Warnings: {warningCount}");
        Console.WriteLine($"   🚨 Critical: {criticalCount}");
        
        Console.WriteLine($"\n📋 Detailed Results / Детальные Результаты:");
        
        foreach (var result in results.OrderBy(r => r.Severity).ThenBy(r => r.CheckName))
        {
            var icon = result.Passed ? "✅" : (result.Severity == Severity.Critical ? "🚨" : "⚠️");
            Console.WriteLine($"\n{icon} {result.CheckName}");
            Console.WriteLine($"   Status: {(result.Passed ? "PASS / ПРОЙДЕНО" : "FAIL / НЕ ПРОЙДЕНО")}");
            Console.WriteLine($"   Message: {result.Message}");
            Console.WriteLine($"   Recommendation: {result.Recommendation}");
        }
        
        if (criticalCount > 0)
        {
            Logger.Logger.Error($"Security assessment: CRITICAL ISSUES FOUND ({criticalCount})");
        }
        else if (warningCount > 0)
        {
            Logger.Logger.Warning($"Security assessment: WARNINGS FOUND ({warningCount})");
        }
        else
        {
            Logger.Logger.Success("Security assessment: GOOD / Оценка безопасности: ХОРОШО");
        }
    }
}
