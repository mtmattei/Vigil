# Completes a native Save dialog opened by the app: sets the file name through UI Automation, then posts IDOK.
# UIA Invoke on the dialog's Save button is ignored (runtime gotcha), and synthesized keys cannot reach a
# background window, so the dialog gets WM_COMMAND IDOK directly.
param(
    [Parameter(Mandatory = $true)][string]$Path,
    [string]$ProcessName = 'Vigil',
    [int]$TimeoutSeconds = 15
)
Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes
Add-Type -MemberDefinition @'
[DllImport("user32.dll")] public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
'@ -Name Win32Msg -Namespace Cap

$proc = Get-Process $ProcessName | Select-Object -First 1
$root = [System.Windows.Automation.AutomationElement]::RootElement
$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
$dialog = $null
while (-not $dialog -and (Get-Date) -lt $deadline) {
    $cond = New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ClassNameProperty, '#32770')))
    $dialog = $root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cond)
    if (-not $dialog) { Start-Sleep -Milliseconds 300 }
}
if (-not $dialog) { throw "No save dialog appeared for $ProcessName within $TimeoutSeconds s." }

$editCond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)
$edit = $dialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $editCond)
if (-not $edit) { throw "Save dialog has no file name box." }
$edit.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern).SetValue($Path)
Start-Sleep -Milliseconds 300
$hwnd = [IntPtr]$dialog.Current.NativeWindowHandle
[void][Cap.Win32Msg]::PostMessage($hwnd, 0x0111, [IntPtr]1, [IntPtr]::Zero)   # WM_COMMAND, IDOK
"Posted IDOK to save dialog for $Path"
