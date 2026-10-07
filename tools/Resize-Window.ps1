# Resizes the app's main window (client width x height in physical px) with SetWindowPos.
# Sizes twice (target+8, then target) because a single large resize can leave stale pixels (runtime gotcha).
param(
    [Parameter(Mandatory = $true)][int]$Width,
    [Parameter(Mandatory = $true)][int]$Height,
    [string]$ProcessName = 'Vigil'
)
Add-Type -MemberDefinition @'
[DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hWnd, IntPtr after, int x, int y, int cx, int cy, uint flags);
'@ -Name Win32Size -Namespace Cap
$proc = Get-Process $ProcessName | Where-Object MainWindowHandle -ne 0 | Select-Object -First 1
if (-not $proc) { throw "No $ProcessName window." }
$flags = 0x0002 -bor 0x0004 -bor 0x0010   # NOMOVE | NOZORDER | NOACTIVATE
# Window frame adds ~16 x 39 px on Windows 11 at 100 %.
[void][Cap.Win32Size]::SetWindowPos($proc.MainWindowHandle, [IntPtr]::Zero, 0, 0, $Width + 16 + 8, $Height + 39 + 8, $flags)
Start-Sleep -Milliseconds 900
[void][Cap.Win32Size]::SetWindowPos($proc.MainWindowHandle, [IntPtr]::Zero, 0, 0, $Width + 16, $Height + 39, $flags)
Start-Sleep -Milliseconds 900
"Resized $($proc.Id) to ~${Width}x${Height}"
