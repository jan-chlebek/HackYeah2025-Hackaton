# MCP Server Setup Verification Script
# Run this script to verify your MCP server installation

Write-Host "================================" -ForegroundColor Cyan
Write-Host "MCP Server Setup Verification" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check 1: MCP Packages Installed
Write-Host "1. Checking MCP packages..." -ForegroundColor Yellow
$packages = @(
    "@modelcontextprotocol/server-filesystem",
    "@modelcontextprotocol/server-github",
    "@modelcontextprotocol/server-memory",
    "@modelcontextprotocol/server-puppeteer"
)

$installedPackages = npm list -g --depth=0 2>&1 | Out-String
$allInstalled = $true

foreach ($package in $packages) {
    if ($installedPackages -match [regex]::Escape($package)) {
        Write-Host "   ✅ $package" -ForegroundColor Green
    } else {
        Write-Host "   ❌ $package (NOT INSTALLED)" -ForegroundColor Red
        $allInstalled = $false
    }
}

if ($allInstalled) {
    Write-Host "   All MCP packages installed successfully!" -ForegroundColor Green
} else {
    Write-Host "   Some packages are missing. Run: npm install -g <package-name>" -ForegroundColor Red
}

Write-Host ""

# Check 2: Configuration Files
Write-Host "2. Checking configuration files..." -ForegroundColor Yellow

$configFiles = @{
    ".vscode\settings.json" = "VS Code MCP configuration"
    ".env.mcp.template" = "Environment template"
    ".gitignore" = "Git exclusion rules"
    "prompts.md" = "Prompt engineering log"
    "MCP_SETUP_GUIDE.md" = "Setup guide"
}

foreach ($file in $configFiles.Keys) {
    if (Test-Path $file) {
        Write-Host "   ✅ $file - $($configFiles[$file])" -ForegroundColor Green
    } else {
        Write-Host "   ❌ $file - Missing!" -ForegroundColor Red
    }
}

Write-Host ""

# Check 3: MCP Directories
Write-Host "3. Checking MCP directories..." -ForegroundColor Yellow

$mcpDirs = @{
    ".mcp" = "MCP root directory"
    ".mcp\memory" = "Memory storage"
    ".mcp\logs" = "Log storage"
}

foreach ($dir in $mcpDirs.Keys) {
    if (Test-Path $dir) {
        Write-Host "   ✅ $dir - $($mcpDirs[$dir])" -ForegroundColor Green
    } else {
        Write-Host "   ❌ $dir - Missing!" -ForegroundColor Red
    }
}

Write-Host ""

# Check 4: Environment Configuration
Write-Host "4. Checking environment configuration..." -ForegroundColor Yellow

if (Test-Path ".env.mcp") {
    Write-Host "   ✅ .env.mcp exists" -ForegroundColor Green
    
    # Check if GitHub token is configured
    $envContent = Get-Content ".env.mcp" -Raw
    if ($envContent -match "GITHUB_TOKEN=ghp_" -or $envContent -match "GITHUB_TOKEN=github_pat_") {
        Write-Host "   ✅ GitHub token appears to be configured" -ForegroundColor Green
    } elseif ($envContent -match "GITHUB_TOKEN=your_github_personal_access_token_here") {
        Write-Host "   ⚠️  GitHub token not configured (using placeholder)" -ForegroundColor Yellow
        Write-Host "      Optional: Set up token at https://github.com/settings/tokens" -ForegroundColor Gray
    } else {
        Write-Host "   ⚠️  GitHub token status unclear" -ForegroundColor Yellow
    }
} else {
    Write-Host "   ⚠️  .env.mcp not found (optional)" -ForegroundColor Yellow
    Write-Host "      Create from template: Copy-Item .env.mcp.template .env.mcp" -ForegroundColor Gray
}

Write-Host ""

# Check 5: .gitignore Verification
Write-Host "5. Checking .gitignore exclusions..." -ForegroundColor Yellow

if (Test-Path ".gitignore") {
    $gitignoreContent = Get-Content ".gitignore" -Raw
    $exclusions = @{
        ".mcp/" = "MCP directory"
        ".env.mcp" = "MCP environment file"
        "mcp-*.log" = "MCP log files"
    }
    
    foreach ($pattern in $exclusions.Keys) {
        if ($gitignoreContent -match [regex]::Escape($pattern)) {
            Write-Host "   ✅ $pattern excluded - $($exclusions[$pattern])" -ForegroundColor Green
        } else {
            Write-Host "   ❌ $pattern NOT excluded!" -ForegroundColor Red
        }
    }
} else {
    Write-Host "   ❌ .gitignore not found!" -ForegroundColor Red
}

Write-Host ""

# Check 6: VS Code Settings Validation
Write-Host "6. Validating VS Code settings..." -ForegroundColor Yellow

if (Test-Path ".vscode\settings.json") {
    try {
        $settings = Get-Content ".vscode\settings.json" -Raw | ConvertFrom-Json
        
        if ($settings.'mcp.servers') {
            $serverCount = ($settings.'mcp.servers' | Get-Member -MemberType NoteProperty).Count
            Write-Host "   ✅ MCP servers configured: $serverCount" -ForegroundColor Green
            
            # List configured servers
            foreach ($server in ($settings.'mcp.servers' | Get-Member -MemberType NoteProperty).Name) {
                Write-Host "      • $server" -ForegroundColor Gray
            }
        } else {
            Write-Host "   ⚠️  No MCP servers configured in settings.json" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "   ❌ settings.json is not valid JSON!" -ForegroundColor Red
    }
} else {
    Write-Host "   ❌ .vscode\settings.json not found!" -ForegroundColor Red
}

Write-Host ""

# Summary
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Verification Summary" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. If packages are missing, install them:" -ForegroundColor White
Write-Host "   npm install -g @modelcontextprotocol/server-<name>" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Set up GitHub token (optional):" -ForegroundColor White
Write-Host "   - Copy .env.mcp.template to .env.mcp" -ForegroundColor Gray
Write-Host "   - Get token: https://github.com/settings/tokens" -ForegroundColor Gray
Write-Host "   - Add token to .env.mcp" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Restart VS Code:" -ForegroundColor White
Write-Host "   - Close and reopen VS Code" -ForegroundColor Gray
Write-Host "   - Or use: Ctrl+Shift+P > 'Developer: Reload Window'" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Test MCP integration:" -ForegroundColor White
Write-Host "   - Try generating an Angular component with AI" -ForegroundColor Gray
Write-Host "   - Check .mcp/logs/mcp.log for activity" -ForegroundColor Gray
Write-Host ""
Write-Host "5. Document your prompts:" -ForegroundColor White
Write-Host "   - Update prompts.md with your experiences" -ForegroundColor Gray
Write-Host ""
Write-Host "📚 Documentation: MCP_SETUP_GUIDE.md" -ForegroundColor Cyan
Write-Host "🔒 Security: All sensitive files excluded from git" -ForegroundColor Green
Write-Host "✨ Ready for AI-assisted development!" -ForegroundColor Green
Write-Host ""
