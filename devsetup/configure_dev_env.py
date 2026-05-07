#!/usr/bin/env python3
"""
configure_dev_env.py
Configure development environment after package installation.
This script sets up various development tools and configurations.
"""

import os
import sys
import subprocess
import json
from pathlib import Path
from datetime import datetime

def log_message(message, level="INFO"):
    """Log message with timestamp"""
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    log_entry = f"[{level}] {timestamp} - {message}"
    print(log_entry)
    
    try:
        with open("dev_env_config.log", "a") as f:
            f.write(log_entry + "\n")
    except:
        pass

def run_command(cmd, description="", check=True):
    """Run command with error handling"""
    if description:
        log_message(f"Running: {description}")
    
    try:
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True, check=check)
        if result.stdout:
            print(result.stdout)
        return True, result.stdout
    except subprocess.CalledProcessError as e:
        log_message(f"Command failed: {cmd}", "ERROR")
        if e.stderr:
            print(e.stderr, file=sys.stderr)
        return False, e.stderr
    except Exception as e:
        log_message(f"Unexpected error: {e}", "ERROR")
        return False, str(e)

def setup_git_config():
    """Setup Git configuration"""
    log_message("Setting up Git configuration...")
    
    git_config = {
        "pull.rebase": "false",
        "init.defaultBranch": "main",
        "core.autocrlf": "input",
        "core.editor": "nano",
        "merge.tool": "vimdiff",
        "diff.tool": "vimdiff"
    }
    
    for key, value in git_config.items():
        success, _ = run_command(f"git config --global {key} '{value}'", f"Set git config {key}")
        if success:
            log_message(f"Git config set: {key} = {value}", "SUCCESS")
    
    # Setup .gitignore template
    gitignore_content = """
# Build outputs
build/
dist/
*.pyc
__pycache__/
*.pyo
*.pyd
.Python

# Virtual environments
venv/
env/
ENV/
.venv/
.env/

# IDE files
.vscode/
.idea/
*.swp
*.swo

# OS files
.DS_Store
Thumbs.db

# Logs
*.log

# Node.js
node_modules/
npm-debug.log*
yarn-debug.log*
yarn-error.log*

# Package files
*.egg-info/
.eggs/
*.egg

# Coverage reports
htmlcov/
.coverage
.coverage.*
coverage.xml

# pytest
.pytest_cache/
test-results/

# mypy
.mypy_cache/
.dmypy.json
dmypy.json

# Environment variables
.env
.env.local
.env.development.local
.env.test.local
.env.production.local
"""
    
    try:
        home = Path.home()
        gitignore_path = home / ".gitignore_global"
        
        with open(gitignore_path, "w") as f:
            f.write(gitignore_content.strip())
        
        run_command("git config --global core.excludesfile '~/.gitignore_global'")
        log_message("Global .gitignore template created", "SUCCESS")
    except Exception as e:
        log_message(f"Failed to create global .gitignore: {e}", "ERROR")

def setup_shell_environment():
    """Setup shell environment (zsh)"""
    log_message("Setting up shell environment...")
    
    home = Path.home()
    zshrc_path = home / ".zshrc"
    
    # Check if zsh is installed
    success, _ = run_command("which zsh", "Check if zsh is installed", check=False)
    if not success:
        log_message("Zsh not found, skipping shell configuration", "WARNING")
        return
    
    # Create .zshrc configuration
    zshrc_content = """
# Zsh configuration
export EDITOR=nano
export VISUAL=nano

# Development paths
export PATH="$HOME/.local/bin:$PATH"
export PATH="$HOME/go/bin:$PATH"

# Python development
export PYTHONPATH="$PYTHONPATH:$HOME/.local/lib/python3.*/site-packages"

# Node.js development
export NPM_CONFIG_PREFIX="$HOME/.npm-global"
export PATH="$NPM_CONFIG_PREFIX/bin:$PATH"

# Go development
export GOPATH="$HOME/go"
export PATH="$GOPATH/bin:$PATH"

# Rust development (if installed)
export PATH="$HOME/.cargo/bin:$PATH"

# Docker
export DOCKER_BUILDKIT=1

# Custom aliases
alias ll='ls -la'
alias la='ls -la'
alias l='ls -l'
alias ..='cd ..'
alias ...='cd ../..'
alias grep='grep --color=auto'
alias mkdir='mkdir -pv'
alias wget='wget -c'

# Git aliases
alias gs='git status'
alias ga='git add'
alias gc='git commit'
alias gp='git push'
alias gl='git pull'
alias gd='git diff'

# Development aliases
alias py='python3'
alias pip='pip3'
alias node='nodejs'
alias npm='npm'

# Enable completion
autoload -Uz compinit
compinit

# History settings
HISTSIZE=10000
SAVEHIST=10000
setopt HIST_IGNORE_DUPS
setopt HIST_IGNORE_ALL_DUPS
setopt HIST_FIND_NO_DUPS
setopt HIST_SAVE_NO_DUPS
setopt HIST_REDUCE_BLANKS

# Prompt
autoload -Uz promptinit
promptinit
prompt redhat
"""
    
    try:
        if not zshrc_path.exists():
            with open(zshrc_path, "w") as f:
                f.write(zshrc_content.strip())
            log_message(".zshrc configuration created", "SUCCESS")
        else:
            log_message(".zshrc already exists, skipping", "INFO")
    except Exception as e:
        log_message(f"Failed to create .zshrc: {e}", "ERROR")

def setup_vscode_extensions():
    """Setup VS Code extensions if available"""
    log_message("Setting up VS Code extensions...")
    
    # Check if VS Code is installed
    success, _ = run_command("which code", "Check if VS Code is installed", check=False)
    if not success:
        log_message("VS Code not found, skipping extension setup", "INFO")
        return
    
    # List of recommended extensions
    extensions = [
        "ms-python.python",
        "ms-vscode.cpptools",
        "redhat.vscode-yaml",
        "ms-vscode.vscode-json",
        "bradlc.vscode-tailwindcss",
        "esbenp.prettier-vscode",
        "dbaeumer.vscode-eslint",
        "ms-vscode.csharp",
        "ms-dotnettools.csharp",
        "ms-vscode-remote.remote-containers",
        "github.copilot",
        "ms-vscode.hexeditor",
        "formulahendry.code-runner",
        "ms-vscode.makefile-tools"
    ]
    
    installed_count = 0
    for ext in extensions:
        success, _ = run_command(f"code --install-extension {ext}", f"Install {ext}", check=False)
        if success:
            installed_count += 1
            log_message(f"VS Code extension installed: {ext}", "SUCCESS")
        else:
            log_message(f"Failed to install VS Code extension: {ext}", "WARNING")
    
    log_message(f"VS Code extensions: {installed_count}/{len(extensions)} installed", "INFO")

def setup_docker_configuration():
    """Setup Docker configuration"""
    log_message("Setting up Docker configuration...")
    
    # Check if Docker is installed
    success, _ = run_command("which docker", "Check if Docker is installed", check=False)
    if not success:
        log_message("Docker not found, skipping Docker configuration", "INFO")
        return
    
    # Create Docker daemon configuration
    docker_config = {
        "registry-mirrors": [],
        "experimental": false,
        "debug": false,
        "log-driver": "json-file",
        "log-opts": {
            "max-size": "10m",
            "max-file": "3"
        }
    }
    
    try:
        docker_dir = Path("/etc/docker")
        if docker_dir.exists():
            config_file = docker_dir / "daemon.json"
            
            # Only create if we have sudo privileges
            if os.geteuid() == 0:
                with open(config_file, "w") as f:
                    json.dump(docker_config, f, indent=2)
                log_message("Docker daemon configuration created", "SUCCESS")
            else:
                log_message("Need sudo privileges to configure Docker daemon", "WARNING")
        
        # Add user to docker group
        username = os.getenv("USER")
        if username:
            success, _ = run_command(f"sudo usermod -aG docker {username}", 
                                  f"Add {username} to docker group", check=False)
            if success:
                log_message(f"User {username} added to docker group", "SUCCESS")
                log_message("You may need to logout and login again for changes to take effect", "INFO")
    except Exception as e:
        log_message(f"Failed to configure Docker: {e}", "ERROR")

def create_development_directories():
    """Create common development directories"""
    log_message("Creating development directories...")
    
    home = Path.home()
    directories = [
        "Projects",
        "Projects/python",
        "Projects/javascript",
        "Projects/go",
        "Projects/docker",
        "Projects/scripts",
        ".local/bin",
        "go/bin",
        ".npm-global",
        "tmp"
    ]
    
    for dir_name in directories:
        dir_path = home / dir_name
        try:
            dir_path.mkdir(parents=True, exist_ok=True)
            log_message(f"Directory created: {dir_path}", "SUCCESS")
        except Exception as e:
            log_message(f"Failed to create directory {dir_path}: {e}", "ERROR")

def main():
    log_message("Starting development environment configuration...")
    
    # Create development directories
    create_development_directories()
    
    # Setup Git configuration
    setup_git_config()
    
    # Setup shell environment
    setup_shell_environment()
    
    # Setup VS Code extensions
    setup_vscode_extensions()
    
    # Setup Docker configuration
    setup_docker_configuration()
    
    log_message("Development environment configuration completed!", "SUCCESS")
    log_message("You may need to restart your shell or logout/login for all changes to take effect.", "INFO")

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        log_message("Configuration interrupted by user", "WARNING")
        sys.exit(1)
    except Exception as e:
        log_message(f"Unexpected error: {e}", "ERROR")
        sys.exit(1)
