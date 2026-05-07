#!/usr/bin/env python3
"""
system_monitor.py
System monitoring and maintenance utilities for Arch Linux development environment.
Provides tools for system health checks, cleanup, and performance monitoring.
"""

import os
import sys
import subprocess
import json
import time
import psutil
from datetime import datetime, timedelta
from pathlib import Path

def log_message(message, level="INFO"):
    """Log message with timestamp"""
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    log_entry = f"[{level}] {timestamp} - {message}"
    print(log_entry)
    
    try:
        with open("system_monitor.log", "a") as f:
            f.write(log_entry + "\n")
    except:
        pass

def run_command(cmd, description="", check=True):
    """Run command with error handling"""
    if description:
        log_message(f"Running: {description}")
    
    try:
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True, check=check)
        return True, result.stdout, result.stderr
    except subprocess.CalledProcessError as e:
        log_message(f"Command failed: {cmd}", "ERROR")
        return False, e.stdout, e.stderr
    except Exception as e:
        log_message(f"Unexpected error: {e}", "ERROR")
        return False, "", str(e)

def get_system_stats():
    """Get comprehensive system statistics"""
    log_message("Collecting system statistics...")
    
    stats = {}
    
    # CPU information
    cpu_percent = psutil.cpu_percent(interval=1)
    cpu_count = psutil.cpu_count()
    cpu_freq = psutil.cpu_freq()
    
    stats['cpu'] = {
        'usage_percent': cpu_percent,
        'core_count': cpu_count,
        'frequency_mhz': cpu_freq.current if cpu_freq else 0
    }
    
    # Memory information
    memory = psutil.virtual_memory()
    swap = psutil.swap_memory()
    
    stats['memory'] = {
        'total_gb': round(memory.total / (1024**3), 2),
        'available_gb': round(memory.available / (1024**3), 2),
        'used_gb': round(memory.used / (1024**3), 2),
        'usage_percent': memory.percent,
        'swap_total_gb': round(swap.total / (1024**3), 2),
        'swap_used_gb': round(swap.used / (1024**3), 2),
        'swap_percent': swap.percent
    }
    
    # Disk information
    disk_usage = psutil.disk_usage('/')
    disk_io = psutil.disk_io_counters()
    
    stats['disk'] = {
        'total_gb': round(disk_usage.total / (1024**3), 2),
        'used_gb': round(disk_usage.used / (1024**3), 2),
        'free_gb': round(disk_usage.free / (1024**3), 2),
        'usage_percent': round((disk_usage.used / disk_usage.total) * 100, 2),
        'read_mb': round(disk_io.read_bytes / (1024**2), 2) if disk_io else 0,
        'write_mb': round(disk_io.write_bytes / (1024**2), 2) if disk_io else 0
    }
    
    # Network information
    net_io = psutil.net_io_counters()
    network = psutil.net_if_addrs()
    
    stats['network'] = {
        'bytes_sent_mb': round(net_io.bytes_sent / (1024**2), 2) if net_io else 0,
        'bytes_recv_mb': round(net_io.bytes_recv / (1024**2), 2) if net_io else 0,
        'interfaces': list(network.keys())
    }
    
    # Process information
    process_count = len(psutil.pids())
    stats['processes'] = {
        'total_count': process_count,
        'running_count': len([p for p in psutil.process_iter(['status']) 
                            if p.info['status'] == psutil.STATUS_RUNNING])
    }
    
    # Boot time
    boot_time = datetime.fromtimestamp(psutil.boot_time())
    uptime = datetime.now() - boot_time
    
    stats['system'] = {
        'boot_time': boot_time.strftime("%Y-%m-%d %H:%M:%S"),
        'uptime_hours': round(uptime.total_seconds() / 3600, 2),
        'uptime_days': round(uptime.total_seconds() / (24 * 3600), 2)
    }
    
    return stats

def display_system_stats(stats):
    """Display system statistics in a formatted way"""
    print("\n" + "="*60)
    print("SYSTEM STATISTICS / СИСТЕМНАЯ СТАТИСТИКА")
    print("="*60)
    
    # CPU
    print(f"\n🖥️  CPU:")
    print(f"   Usage: {stats['cpu']['usage_percent']}%")
    print(f"   Cores: {stats['cpu']['core_count']}")
    print(f"   Frequency: {stats['cpu']['frequency_mhz']:.0f} MHz")
    
    # Memory
    print(f"\n💾 Memory:")
    print(f"   Total: {stats['memory']['total_gb']} GB")
    print(f"   Used: {stats['memory']['used_gb']} GB ({stats['memory']['usage_percent']}%)")
    print(f"   Available: {stats['memory']['available_gb']} GB")
    if stats['memory']['swap_total_gb'] > 0:
        print(f"   Swap: {stats['memory']['swap_used_gb']} GB / {stats['memory']['swap_total_gb']} GB ({stats['memory']['swap_percent']}%)")
    
    # Disk
    print(f"\n💽 Disk:")
    print(f"   Total: {stats['disk']['total_gb']} GB")
    print(f"   Used: {stats['disk']['used_gb']} GB ({stats['disk']['usage_percent']}%)")
    print(f"   Free: {stats['disk']['free_gb']} GB")
    print(f"   I/O: {stats['disk']['read_mb']} MB read, {stats['disk']['write_mb']} MB write")
    
    # Network
    print(f"\n🌐 Network:")
    print(f"   Sent: {stats['network']['bytes_sent_mb']} MB")
    print(f"   Received: {stats['network']['bytes_recv_mb']} MB")
    print(f"   Interfaces: {', '.join(stats['network']['interfaces'][:5])}")
    
    # Processes
    print(f"\n⚙️  Processes:")
    print(f"   Total: {stats['processes']['total_count']}")
    print(f"   Running: {stats['processes']['running_count']}")
    
    # System
    print(f"\n🕐 System:")
    print(f"   Boot time: {stats['system']['boot_time']}")
    print(f"   Uptime: {stats['system']['uptime_days']} days ({stats['system']['uptime_hours']} hours)")
    
    print("\n" + "="*60)

def check_system_health():
    """Check system health and report issues"""
    log_message("Performing system health check...")
    
    issues = []
    warnings = []
    
    # Check CPU usage
    cpu_percent = psutil.cpu_percent(interval=1)
    if cpu_percent > 90:
        issues.append(f"High CPU usage: {cpu_percent}%")
    elif cpu_percent > 70:
        warnings.append(f"Elevated CPU usage: {cpu_percent}%")
    
    # Check memory usage
    memory = psutil.virtual_memory()
    if memory.percent > 90:
        issues.append(f"High memory usage: {memory.percent}%")
    elif memory.percent > 80:
        warnings.append(f"Elevated memory usage: {memory.percent}%")
    
    # Check disk usage
    disk = psutil.disk_usage('/')
    disk_percent = (disk.used / disk.total) * 100
    if disk_percent > 90:
        issues.append(f"Low disk space: {disk_percent:.1f}% used")
    elif disk_percent > 80:
        warnings.append(f"Moderate disk usage: {disk_percent:.1f}% used")
    
    # Check swap usage
    swap = psutil.swap_memory()
    if swap.percent > 50:
        warnings.append(f"High swap usage: {swap.percent}%")
    
    # Check for zombie processes
    zombie_count = 0
    for proc in psutil.process_iter(['status']):
        try:
            if proc.info['status'] == psutil.STATUS_ZOMBIE:
                zombie_count += 1
        except (psutil.NoSuchProcess, psutil.AccessDenied):
            continue
    
    if zombie_count > 0:
        warnings.append(f"Found {zombie_count} zombie processes")
    
    # Report results
    if issues:
        log_message("🚨 SYSTEM ISSUES FOUND:", "ERROR")
        for issue in issues:
            log_message(f"  ❌ {issue}", "ERROR")
    
    if warnings:
        log_message("⚠️  SYSTEM WARNINGS:", "WARNING")
        for warning in warnings:
            log_message(f"  ⚠️  {warning}", "WARNING")
    
    if not issues and not warnings:
        log_message("✅ System health check passed!", "SUCCESS")
    
    return len(issues) == 0

def cleanup_system():
    """Perform system cleanup operations"""
    log_message("Starting system cleanup...")
    
    cleanup_operations = []
    
    # Clean package cache
    success, stdout, stderr = run_command("sudo pacman -Scc --noconfirm", "Clean package cache")
    if success:
        cleanup_operations.append("Package cache cleaned")
        log_message("Package cache cleaned", "SUCCESS")
    else:
        cleanup_operations.append("Package cache cleanup failed")
        log_message("Package cache cleanup failed", "ERROR")
    
    # Remove orphan packages
    success, stdout, stderr = run_command("sudo pacman -Rns $(pacman -Qtdq) --noconfirm", "Remove orphan packages", check=False)
    if success or "target not found" in stderr.lower():
        cleanup_operations.append("Orphan packages removed")
        log_message("Orphan packages removed", "SUCCESS")
    else:
        cleanup_operations.append("No orphan packages found or cleanup failed")
        log_message("No orphan packages found or cleanup failed", "INFO")
    
    # Clean temporary files
    temp_dirs = ["/tmp", "/var/tmp"]
    cleaned_temp = 0
    
    for temp_dir in temp_dirs:
        if os.path.exists(temp_dir):
            try:
                # Count files before cleanup
                file_count = len([f for f in Path(temp_dir).glob('*') if f.is_file()])
                
                # Clean files older than 1 day
                success, stdout, stderr = run_command(f"sudo find {temp_dir} -type f -mtime +1 -delete", 
                                                   f"Clean {temp_dir}")
                if success:
                    cleaned_temp += file_count
            except Exception as e:
                log_message(f"Failed to clean {temp_dir}: {e}", "WARNING")
    
    if cleaned_temp > 0:
        cleanup_operations.append(f"Cleaned {cleaned_temp} temporary files")
        log_message(f"Cleaned {cleaned_temp} temporary files", "SUCCESS")
    
    # Clean journal logs
    success, stdout, stderr = run_command("sudo journalctl --vacuum-time=7d", "Clean journal logs")
    if success:
        cleanup_operations.append("Journal logs cleaned (7 days)")
        log_message("Journal logs cleaned (7 days)", "SUCCESS")
    
    # Clean Docker if available
    if os.path.exists("/usr/bin/docker"):
        success, stdout, stderr = run_command("sudo docker system prune -f --volumes", "Clean Docker system")
        if success:
            cleanup_operations.append("Docker system cleaned")
            log_message("Docker system cleaned", "SUCCESS")
    
    log_message(f"Cleanup completed. Operations performed: {len(cleanup_operations)}")
    for op in cleanup_operations:
        log_message(f"  ✓ {op}")

def monitor_resources(duration=60, interval=5):
    """Monitor system resources for a specified duration"""
    log_message(f"Starting resource monitoring for {duration} seconds (interval: {interval}s)...")
    
    end_time = time.time() + duration
    samples = []
    
    try:
        while time.time() < end_time:
            sample = {
                'timestamp': datetime.now().strftime("%H:%M:%S"),
                'cpu_percent': psutil.cpu_percent(),
                'memory_percent': psutil.virtual_memory().percent,
                'disk_percent': (psutil.disk_usage('/').used / psutil.disk_usage('/').total) * 100
            }
            samples.append(sample)
            
            print(f"\r{sample['timestamp']} | CPU: {sample['cpu_percent']:5.1f}% | "
                  f"Memory: {sample['memory_percent']:5.1f}% | "
                  f"Disk: {sample['disk_percent']:5.1f}%", end="", flush=True)
            
            time.sleep(interval)
        
        print("\n")  # New line after monitoring
        
        # Calculate statistics
        if samples:
            avg_cpu = sum(s['cpu_percent'] for s in samples) / len(samples)
            avg_memory = sum(s['memory_percent'] for s in samples) / len(samples)
            max_cpu = max(s['cpu_percent'] for s in samples)
            max_memory = max(s['memory_percent'] for s in samples)
            
            log_message(f"Monitoring completed. Samples: {len(samples)}")
            log_message(f"Average CPU: {avg_cpu:.1f}% (Max: {max_cpu:.1f}%)")
            log_message(f"Average Memory: {avg_memory:.1f}% (Max: {max_memory:.1f}%)")
    
    except KeyboardInterrupt:
        log_message("Monitoring interrupted by user", "WARNING")

def generate_system_report():
    """Generate comprehensive system report"""
    log_message("Generating system report...")
    
    stats = get_system_stats()
    health_ok = check_system_health()
    
    report = {
        'timestamp': datetime.now().isoformat(),
        'hostname': os.uname().nodename,
        'platform': os.uname().sysname,
        'platform_release': os.uname().release,
        'platform_version': os.uname().version,
        'architecture': os.uname().machine,
        'stats': stats,
        'health_status': 'HEALTHY' if health_ok else 'ISSUES_FOUND'
    }
    
    # Save report to file
    report_file = f"system_report_{datetime.now().strftime('%Y%m%d_%H%M%S')}.json"
    
    try:
        with open(report_file, 'w') as f:
            json.dump(report, f, indent=2, default=str)
        log_message(f"System report saved to: {report_file}", "SUCCESS")
    except Exception as e:
        log_message(f"Failed to save report: {e}", "ERROR")
    
    return report_file

def main():
    """Main function with menu"""
    print("🖥️  Arch Linux System Monitor / Арх Linux Системный Монитор")
    print("=" * 60)
    
    if len(sys.argv) > 1:
        command = sys.argv[1].lower()
        
        if command == "stats":
            stats = get_system_stats()
            display_system_stats(stats)
        
        elif command == "health":
            check_system_health()
        
        elif command == "cleanup":
            cleanup_system()
        
        elif command == "monitor":
            duration = int(sys.argv[2]) if len(sys.argv) > 2 else 60
            monitor_resources(duration)
        
        elif command == "report":
            generate_system_report()
        
        else:
            print(f"Unknown command: {command}")
            print_usage()
    else:
        # Interactive mode
        while True:
            print("\nOptions:")
            print("1. Show system statistics")
            print("2. Check system health")
            print("3. Cleanup system")
            print("4. Monitor resources")
            print("5. Generate system report")
            print("6. Exit")
            
            try:
                choice = input("\nSelect option (1-6): ").strip()
                
                if choice == "1":
                    stats = get_system_stats()
                    display_system_stats(stats)
                
                elif choice == "2":
                    check_system_health()
                
                elif choice == "3":
                    cleanup_system()
                
                elif choice == "4":
                    duration = input("Monitor duration in seconds (default 60): ").strip()
                    duration = int(duration) if duration.isdigit() else 60
                    monitor_resources(duration)
                
                elif choice == "5":
                    generate_system_report()
                
                elif choice == "6":
                    log_message("Exiting system monitor", "INFO")
                    break
                
                else:
                    print("Invalid option. Please try again.")
            
            except KeyboardInterrupt:
                log_message("Interrupted by user", "WARNING")
                break
            except Exception as e:
                log_message(f"Error: {e}", "ERROR")

def print_usage():
    """Print usage information"""
    print("\nUsage:")
    print("  python3 system_monitor.py [command] [options]")
    print("\nCommands:")
    print("  stats     - Show system statistics")
    print("  health    - Check system health")
    print("  cleanup   - Perform system cleanup")
    print("  monitor   - Monitor resources (default 60s)")
    print("  report    - Generate system report")
    print("\nExamples:")
    print("  python3 system_monitor.py stats")
    print("  python3 system_monitor.py monitor 120")
    print("  python3 system_monitor.py cleanup")

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        log_message("Interrupted by user", "WARNING")
        sys.exit(1)
    except Exception as e:
        log_message(f"Unexpected error: {e}", "ERROR")
        sys.exit(1)
