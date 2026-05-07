# setup.py
"""Install common Python development packages via pip.
This script is intended to be invoked by the C# utility after the base
system packages have been installed.
"""
import subprocess
import sys

# List of pip packages to install
packages = [
    "ipython",
    "pylint",
    "black",
    "flake8",
    "mypy",
    "pytest",
    "requests",
]

def run(cmd: list[str]):
    result = subprocess.run(cmd, capture_output=True, text=True)
    print(result.stdout)
    if result.returncode != 0:
        print(result.stderr, file=sys.stderr)
        sys.exit(result.returncode)

def main():
    # Upgrade pip first
    run([sys.executable, "-m", "pip", "install", "--upgrade", "pip"])
    # Install each package
    for pkg in packages:
        run([sys.executable, "-m", "pip", "install", pkg])

if __name__ == "__main__":
    main()
