# setup.py
"""Install common Python development packages via pip.
This script is intended to be invoked by the C# utility after the base
system packages have been installed.
"""
import subprocess
import sys
import os
from datetime import datetime

# List of pip packages to install
packages = [
    "ipython",
    "jupyter",
    "pylint",
    "black",
    "flake8",
    "mypy",
    "pytest",
    "requests",
    "numpy",
    "pandas",
    "matplotlib",
    "seaborn",
    "virtualenv",
    "pipenv",
    "python-dotenv",
    "pre-commit"
]

def log_message(message, level="INFO"):
    """Log message with timestamp"""
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    log_entry = f"[{level}] {timestamp} - {message}"
    print(log_entry)
    
    # Also write to log file
    try:
        with open("python_setup.log", "a") as f:
            f.write(log_entry + "\n")
    except:
        pass

def run(cmd, description=""):
    """Run command with error handling"""
    if description:
        log_message(f"Running: {description}")
    
    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        if result.stdout:
            print(result.stdout)
        return True
    except subprocess.CalledProcessError as e:
        log_message(f"Command failed: {' '.join(cmd)}", "ERROR")
        if e.stdout:
            print(e.stdout)
        if e.stderr:
            print(e.stderr, file=sys.stderr)
        return False
    except Exception as e:
        log_message(f"Unexpected error: {e}", "ERROR")
        return False

def check_python_version():
    """Check Python version compatibility"""
    version = sys.version_info
    log_message(f"Python version: {version.major}.{version.minor}.{version.micro}")
    
    if version.major < 3 or (version.major == 3 and version.minor < 8):
        log_message("Python 3.8+ is recommended", "WARNING")
    return True

def main():
    log_message("Starting Python packages setup...")
    
    # Check Python version
    if not check_python_version():
        log_message("Python version check failed", "ERROR")
        sys.exit(1)
    
    # Upgrade pip first
    log_message("Upgrading pip...")
    if not run([sys.executable, "-m", "pip", "install", "--upgrade", "pip"], "Upgrade pip"):
        log_message("Failed to upgrade pip", "WARNING")
    
    # Install each package
    success_count = 0
    failed_packages = []
    
    for pkg in packages:
        log_message(f"Installing {pkg}...")
        if run([sys.executable, "-m", "pip", "install", pkg], f"Install {pkg}"):
            success_count += 1
            log_message(f"Successfully installed {pkg}", "SUCCESS")
        else:
            failed_packages.append(pkg)
            log_message(f"Failed to install {pkg}", "WARNING")
    
    # Summary
    log_message(f"Installation complete: {success_count}/{len(packages)} packages installed")
    
    if failed_packages:
        log_message(f"Failed packages: {', '.join(failed_packages)}", "WARNING")
    
    # Install additional development tools
    log_message("Installing additional development tools...")
    
    # Install git pre-commit hook configuration
    if run([sys.executable, "-m", "pre_commit", "install"], "Install pre-commit hooks"):
        log_message("Pre-commit hooks installed", "SUCCESS")
    
    log_message("Python setup completed!")

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        log_message("Setup interrupted by user", "WARNING")
        sys.exit(1)
    except Exception as e:
        log_message(f"Unexpected error: {e}", "ERROR")
        sys.exit(1)
