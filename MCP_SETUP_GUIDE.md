# MCP Server Setup Guide - Quick Start

## ✅ What Has Been Set Up

All MCP (Model Context Protocol) servers have been installed and configured for your UKNF governance platform project.

### Installed Packages:
- ✅ `@modelcontextprotocol/server-filesystem` - File operations
- ✅ `@modelcontextprotocol/server-github` - GitHub integration
- ✅ `@modelcontextprotocol/server-memory` - Context persistence
- ✅ `@modelcontextprotocol/server-puppeteer` - E2E testing

### Created Files:
- ✅ `.vscode/settings.json` - MCP server configuration
- ✅ `.env.mcp.template` - Environment variables template
- ✅ `.mcp/memory/` - Memory storage directory
- ✅ `.mcp/logs/` - Logs storage directory
- ✅ `prompts.md` - Prompt engineering documentation
- ✅ Updated `.gitignore` - MCP files excluded from git

---

## 🚀 Quick Start (3 Steps)

### Step 1: Set Up GitHub Token (Optional but Recommended)

1. Go to GitHub Settings → Developer settings → Personal access tokens
   - URL: https://github.com/settings/tokens
   
2. Click "Generate new token (classic)"

3. Select scopes:
   - ✅ `repo` (Full control of private repositories)
   - ✅ `read:org` (Read organization data)
   - ✅ `read:user` (Read user profile data)

4. Generate and copy your token

5. Create `.env.mcp` file (copy from template):
   ```bash
   Copy-Item .env.mcp.template .env.mcp
   ```

6. Edit `.env.mcp` and add your token:
   ```env
   GITHUB_TOKEN=ghp_your_actual_token_here
   ```

### Step 2: Restart VS Code

**Important**: VS Code needs to reload to activate MCP servers.

**Option A - Restart Extension Host** (Faster):
1. Press `Ctrl+Shift+P` (Windows) or `Cmd+Shift+P` (Mac)
2. Type "Developer: Reload Window"
3. Press Enter

**Option B - Restart VS Code** (Complete):
1. Close VS Code completely
2. Reopen your project

### Step 3: Test MCP Integration

Try this AI-assisted prompt:
```
"Create a simple Angular component called 'test-component' with MCP filesystem context"
```

The MCP filesystem server should automatically:
- Read your existing Angular patterns
- Generate files in the correct location
- Apply consistent styling and structure

---

## 🔧 Configuration Details

### MCP Servers Active:

#### 1. **Filesystem Server**
- **Purpose**: Secure file read/write operations
- **Read Access**: Frontend, Backend, .requirements, prompts
- **Write Access**: Frontend/src, Backend/src (protected)
- **Denied**: .env*, appsettings*.json, secrets

#### 2. **GitHub Server** (Requires token)
- **Purpose**: Repository integration, commit analysis
- **Repository**: HackYeah2025-Hackaton
- **Owner**: jan-chlebek
- **Token**: Environment variable (never hardcoded)

#### 3. **Memory Server**
- **Purpose**: Context persistence across sessions
- **Storage**: `.mcp/memory/`
- **Max Context**: 100,000 tokens
- **Logs**: `.mcp/logs/mcp.log`

#### 4. **Puppeteer Server**
- **Purpose**: E2E testing, accessibility checks
- **Mode**: Headless
- **Use Cases**: WCAG 2.2 testing, browser automation

---

## 🔒 Security Features

### Automatic Protections:
- ✅ **Secrets Protection**: .env*, appsettings*.json cannot be read/written
- ✅ **Write Restrictions**: Only src directories can be modified
- ✅ **Manual Approval**: Docker, database, migration operations require approval
- ✅ **Git Exclusion**: All MCP files (.env.mcp, .mcp/) excluded from commits

### Best Practices Enforced:
- Strong passwords required (documented in prompts.md)
- HTTPS-only assumptions for production
- Audit logging enabled
- Context-aware security suggestions

---

## 📚 How to Use MCP Servers

### Example 1: Generate Angular Component
```
Prompt: "Create a podmiot-list component with:
- DataTable showing podmiot entities
- Server-side pagination (20 items/page)
- Filtering by name and regon
- Sticky table header
- High contrast theme support
- WCAG 2.2 compliant"
```

**What MCP Does**:
- Reads existing Angular components for patterns
- Generates consistent file structure
- Applies project coding standards
- Suggests accessibility improvements

### Example 2: ASP.NET Core API Generation
```
Prompt: "Create a PodmiotController with:
- GET /api/v1/podmiots (paginated)
- GET /api/v1/podmiots/{id}
- POST /api/v1/podmiots
- PUT /api/v1/podmiots/{id}
- DELETE /api/v1/podmiots/{id}
- OAuth2 authentication required
- Entity Framework integration"
```

**What MCP Does**:
- Reads existing controller patterns
- Applies consistent REST conventions
- Suggests security best practices
- Generates Entity Framework queries

### Example 3: E2E Testing with Puppeteer
```
Prompt: "Create Puppeteer test for:
- Login flow with OAuth2
- JWT token validation
- Keyboard navigation (Tab, Enter, Escape)
- High contrast theme switching
- Screen reader compatibility"
```

**What MCP Does**:
- Generates browser automation script
- Implements WCAG 2.2 tests
- Validates accessibility requirements
- Suggests edge cases

---

## 🐛 Troubleshooting

### MCP Servers Not Working?

**Check 1: VS Code Restarted?**
- Restart VS Code after configuration changes
- Use "Developer: Reload Window" command

**Check 2: Packages Installed?**
```bash
npm list -g | Select-String "modelcontextprotocol"
```

Should show:
- @modelcontextprotocol/server-filesystem
- @modelcontextprotocol/server-github
- @modelcontextprotocol/server-memory
- @modelcontextprotocol/server-puppeteer

**Check 3: Settings File Valid?**
- Open `.vscode/settings.json`
- Verify JSON is valid (no syntax errors)

**Check 4: GitHub Token (Optional)**
- If GitHub integration fails, check .env.mcp
- Verify token has correct scopes
- Ensure GITHUB_TOKEN environment variable is set

### Common Warnings (Safe to Ignore):

```
npm warn deprecated @modelcontextprotocol/server-github
npm warn deprecated @modelcontextprotocol/server-puppeteer
```

These packages are deprecated but still functional. Future updates may provide replacements.

---

## 📖 Documentation

- **Prompt Engineering Log**: `prompts.md`
- **Environment Template**: `.env.mcp.template`
- **VS Code Configuration**: `.vscode/settings.json`
- **Project Requirements**: `.requirements/` folder

---

## 🎯 Next Steps

1. **[ ] Set up GitHub token** (optional but recommended)
2. **[ ] Restart VS Code** to activate MCP servers
3. **[ ] Test with Angular component generation**
4. **[ ] Document your prompts** in prompts.md
5. **[ ] Explore Puppeteer testing** for accessibility

---

## 🆘 Need Help?

### Resources:
- Model Context Protocol Docs: https://modelcontextprotocol.io/
- VS Code Extension API: https://code.visualstudio.com/api
- Project Requirements: `.requirements/DETAILS_UKNF_Prompt2Code2.md`

### Support:
- Check `prompts.md` for successful prompt patterns
- Review `.vscode/settings.json` for configuration
- Verify `.gitignore` excludes sensitive files

---

**✨ You're all set!** MCP servers are configured and ready to enhance your AI-assisted development for the UKNF governance platform.

**Remember**: Document all prompts in `prompts.md` - it's part of your HackYeah 2025 deliverables! 🏆

---

*Last Updated: October 4, 2025*  
*Project: HackYeah 2025 - UKNF Communication Platform*
