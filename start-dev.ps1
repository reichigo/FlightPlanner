# Start FlightPlanner in Development Mode
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  FlightPlanner Development Server" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check if MongoDB is running
Write-Host "Checking MongoDB..." -ForegroundColor Yellow
$mongoRunning = docker ps --filter "name=mypethere-mongodb" --filter "status=running" --format "{{.Names}}"
if ($mongoRunning) {
    Write-Host "MongoDB is running" -ForegroundColor Green
} else {
    Write-Host "MongoDB is not running!" -ForegroundColor Red
    Write-Host "Please start MongoDB first" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Starting services..." -ForegroundColor Yellow
Write-Host ""

# Start API
Write-Host "[1/2] Starting API..." -ForegroundColor Cyan
$apiScript = "Write-Host 'FlightPlanner API' -ForegroundColor Green; Set-Location '$PSScriptRoot'; dotnet run --project FlightPlanner.Api"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $apiScript

# Wait for API to start
Write-Host "Waiting for API to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Start Web
Write-Host "[2/2] Starting Web..." -ForegroundColor Cyan
$webScript = "Write-Host 'FlightPlanner Web' -ForegroundColor Green; Set-Location '$PSScriptRoot'; dotnet run --project FlightPlanner.Web"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $webScript

Write-Host ""
Write-Host "======================================" -ForegroundColor Green
Write-Host "  Services Started Successfully!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "API:  http://localhost:5062" -ForegroundColor Cyan
Write-Host "Web:  http://localhost:5181" -ForegroundColor Cyan
Write-Host ""
Write-Host "To stop services, close the PowerShell windows" -ForegroundColor Yellow
Write-Host ""

