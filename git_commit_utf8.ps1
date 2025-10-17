# UTF-8 Git 提交脚本
# 用法: .\git_commit_utf8.ps1 "你的提交消息"

param(
    [Parameter(Mandatory=$true)]
    [string]$Message
)

$messageFile = "temp_commit_message.txt"
$Message | Out-File -FilePath $messageFile -Encoding UTF8

try {
    git commit -F $messageFile
    Write-Host "提交成功！" -ForegroundColor Green
} finally {
    Remove-Item $messageFile -ErrorAction SilentlyContinue
}

