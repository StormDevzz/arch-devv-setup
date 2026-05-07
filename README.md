# Arch Linux Development Setup

## Описание / Description

Этот проект содержит скрипты и утилиты для автоматической настройки среды разработки на Arch Linux. Включает в себя установку необходимых пакетов, настройку окружения и базовых инструментов для разработки.

This project contains scripts and utilities for automatic development environment setup on Arch Linux. It includes installation of necessary packages, environment configuration, and basic development tools.

## Системные требования / System Requirements

- Arch Linux (или основанные на нем дистрибутивы) / Arch Linux (or based distributions)
- Права администратора (sudo) / Administrator privileges (sudo)
- Подключение к интернету / Internet connection

## Установка / Installation

### Способ 1: Использование Python скрипта / Method 1: Using Python script

```bash
# Клонируйте репозиторий / Clone the repository
git clone https://github.com/StormDevzz/arch-devv-setup.git
cd arch-devv-setup/devsetup

# Запустите скрипт установки / Run the setup script
python3 setup.py
```

### Способ 2: Использование C# приложения / Method 2: Using C# application

```bash
# Убедитесь, что установлен .NET SDK / Make sure .NET SDK is installed
sudo pacman -S dotnet-sdk

# Перейдите в папку проекта / Navigate to project directory
cd devsetup

# Соберите и запустите приложение / Build and run the application
dotnet run
```

## Что устанавливается / What gets installed

### Базовые пакеты / Basic packages
- `git` - система контроля версий / version control system
- `vim` - текстовый редактор / text editor
- `curl` - утилита для работы с HTTP / HTTP utility
- `wget` - утилита для загрузки файлов / file download utility
- `tree` - отображение структуры директорий / directory structure display

### Инструменты разработки / Development tools
- `base-devel` - базовые инструменты для сборки / basic build tools
- `python` - интерпретатор Python / Python interpreter
- `nodejs` - среда выполнения JavaScript / JavaScript runtime
- `npm` - менеджер пакетов Node.js / Node.js package manager
- `docker` - платформа контейнеризации / containerization platform

### Дополнительные утилиты / Additional utilities
- `htop` - монитор процессов / process monitor
- `neofetch` - информация о системе / system information
- `zsh` - расширенная командная оболочка / enhanced command shell

## Использование / Usage

После установки вы можете / After installation, you can:

1. **Проверить установку** / **Check installation**:
   ```bash
   git --version
   python3 --version
   node --version
   docker --version
   ```

2. **Настроить Git** / **Configure Git**:
   ```bash
   git config --global user.name "Your Name"
   git config --global user.email "your@email.com"
   ```

3. **Запустить Docker** / **Start Docker**:
   ```bash
   sudo systemctl start docker
   sudo systemctl enable docker
   ```

## Структура проекта

```
devsetup/
├── Program.cs       # Основной файл C# приложения
├── Config.cs        # Конфигурация приложения
├── Logger.cs        # Логирование операций
├── Utils.cs         # Вспомогательные функции
├── setup.py         # Python скрипт установки
├── devsetup.csproj  # Файл проекта C#
└── README.md        # Этот файл
```

## Устранение проблем / Troubleshooting

### Проблема: Нет прав на установку пакетов / Problem: No package installation permissions

**Решение** / **Solution**: Убедитесь, что у вас есть права sudo / Make sure you have sudo privileges:
```bash
sudo pacman -Syu
```

### Проблема: .NET SDK не найден / Problem: .NET SDK not found

**Решение** / **Solution**: Установите .NET SDK / Install .NET SDK:
```bash
sudo pacman -S dotnet-sdk
```

### Проблема: Python скрипт не запускается / Problem: Python script doesn't run

**Решение** / **Solution**: Убедитесь, что установлен Python 3 / Make sure Python 3 is installed:
```bash
sudo pacman -S python
```

## Настройка после установки / Post-installation setup

1. **Перезагрузите систему** для применения всех изменений / **Reboot system** to apply all changes
2. **Настройте свою среду разработки** (VS Code, IntelliJ IDEA и т.д.) / **Configure your development environment** (VS Code, IntelliJ IDEA, etc.)
3. **Создайте SSH ключи** для работы с Git / **Create SSH keys** for Git operations:
   ```bash
   ssh-keygen -t ed25519 -C "your@email.com"
   ```

## Лицензия / License

Этот проект распространяется под лицензией MIT. Подробности см. в файле [LICENSE](LICENSE).

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
