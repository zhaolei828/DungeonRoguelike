# UTF-8 Git Commit Script
# Usage: .\git_commit_utf8.ps1 "Your commit message"

param(
    [Parameter(Mandatory=$true)]
    [string]$Message
)

$messageFile = "temp_commit_message.txt"
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText($messageFile, $Message, $utf8NoBom)

try {
    git commit -F $messageFile
    Write-Host "Commit successful!" -ForegroundColor Green
} finally {
    Remove-Item $messageFile -ErrorAction SilentlyContinue
}
