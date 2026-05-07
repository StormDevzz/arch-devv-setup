// Config.cs
using System.Collections.Generic;

public static class Config
{
    // Список пакетов для установки через pacman
    public static readonly List<string> Packages = new List<string>
    {
        "git",
        "base-devel",
        "vim",
        "neovim",
        "tmux",
        "zsh",
        "curl",
        "wget",
        "openssh",
        "dotnet-sdk",
        "python",
        "python-pip",
        "nodejs",
        "npm",
        "postgresql",
        "redis",
        "docker",
        "docker-compose"
    };

    // Список сервисов для включения и запуска
    public static readonly List<string> Services = new List<string>
    {
        "docker",
        "postgresql",
        "redis"
    };
}
