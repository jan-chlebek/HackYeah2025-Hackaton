# Prompt Engineering Log - HackYeah 2025 UKNF Platform

**Project**: UKNF Communication Platform  
**Architecture**: Angular 20 SPA + ASP.NET Core 8/9 REST API + PostgreSQL  
**Purpose**: Track all AI prompts, their effectiveness, and lessons learned

---

## 🎯 Prompt Effectiveness Rating System

- ⭐⭐⭐⭐⭐ (Excellent) - Generated production-ready code, minimal edits needed
- ⭐⭐⭐⭐ (Good) - Generated solid foundation, minor refinements required
- ⭐⭐⭐ (Acceptable) - Generated useful starting point, significant edits needed
- ⭐⭐ (Poor) - Generated code with major issues, extensive rework required
- ⭐ (Failed) - Did not generate useful output, manual implementation needed

---

## 📅 October 4, 2025

### 1. PostgreSQL Docker Container Setup

**Prompt**: 
```
I want to make a postgres container on docker. I've got an error: 
Database is uninitialized and superuser password is not specified.
```

**Context**: Setting up PostgreSQL database for UKNF governance platform

**AI Response Summary**:
- Explained the need for `POSTGRES_PASSWORD` environment variable
- Provided 3 solutions: docker run command, docker-compose, and .env file approach
- Emphasized security best practices for governance projects
- Recommended solution 2 (docker-compose) for project management

**Implementation**:
- Created PostgreSQL container with environment variables:
  - `POSTGRES_PASSWORD`: Secure password
  - `POSTGRES_USER`: admin
  - `POSTGRES_DB`: governance_db
  - Port mapping: 5432:5432
  - Volume: postgres_data for persistence

**Effectiveness**: ⭐⭐⭐⭐⭐ (Excellent)

**Key Learnings**:
- Always specify POSTGRES_PASSWORD when creating PostgreSQL containers
- Never use `POSTGRES_HOST_AUTH_METHOD=trust` in production/governance systems
- Use docker-compose for better configuration management
- Store credentials in .env files, never in code

**Security Notes**:
- Password strength: minimum 16 characters with mixed case, numbers, symbols
- Added .env files to .gitignore
- Limited port exposure considerations for production

---

### 2. MCP (Model Context Protocol) Server Setup

**Prompt**: 
```
I need mcp server for creating angular app. Search all md files in folder .requirements. 
Tell me shortly what mcp server are, and how can i use it in this project.
```

**Context**: Understanding and implementing MCP servers for AI-assisted Angular development

**AI Response Summary**:
- Explained MCP servers as middleware between AI assistants and development environment
- Identified key MCP servers for the project:
  - `@modelcontextprotocol/server-filesystem` - File operations
  - `@modelcontextprotocol/server-github` - Repository integration
  - `@modelcontextprotocol/server-memory` - Context persistence
  - `@modelcontextprotocol/server-puppeteer` - E2E testing automation

**Implementation Steps**:
1. ✅ Installed MCP server packages globally via npm
2. ✅ Created `.vscode/settings.json` with MCP configuration
3. ✅ Created `.env.mcp.template` for environment variables
4. ✅ Updated `.gitignore` to exclude MCP secrets
5. ✅ Created `.mcp/memory` and `.mcp/logs` directories
6. ✅ Documented setup in prompts.md

**MCP Servers Installed**:
```bash
npm install -g @modelcontextprotocol/server-filesystem
npm install -g @modelcontextprotocol/server-github
npm install -g @modelcontextprotocol/server-memory
npm install -g @modelcontextprotocol/server-puppeteer
```

**Note**: `@modelcontextprotocol/server-docker` does not exist in npm registry

**Configuration Created**:
- **Filesystem Server**: Read/write permissions with security restrictions
  - Read: Frontend, Backend, .requirements, prompts
  - Write: Frontend/src, Backend/src only
  - Deny: .env*, appsettings*.json, secrets files
  
- **GitHub Server**: Repository integration for audit trails
  - Repository: HackYeah2025-Hackaton
  - Owner: jan-chlebek
  - Token: Stored in environment variable
  
- **Memory Server**: Context persistence across sessions
  - Path: .mcp/memory
  - Max context: 100,000 tokens
  
- **Puppeteer Server**: E2E testing automation
  - Headless mode enabled
  - For WCAG 2.2 accessibility testing

**Security Guardrails Applied**:
- ✅ Denied access to sensitive files (.env*, appsettings*.json)
- ✅ Manual approval required for Docker, database, migration operations
- ✅ Read-only access to requirements folder
- ✅ Write access restricted to src directories only
- ✅ GitHub token stored in environment variable (not hardcoded)
- ✅ All MCP files added to .gitignore

**Integration Points for UKNF Platform**:

1. **Communication Domain**:
   - MCP Filesystem: Manage file attachments, case folders, document library
   - MCP Memory: Track conversation context for messaging threads
   
2. **Identity & Access Domain**:
   - MCP GitHub: Audit trails for code changes
   - Security validation for OAuth2/OIDC flows
   
3. **Administration Domain**:
   - MCP Puppeteer: Automated health checks and E2E tests
   - Performance monitoring and metrics

**Effectiveness**: ⭐⭐⭐⭐⭐ (Excellent)

**Key Learnings**:
- MCP servers provide context-aware AI assistance for code generation
- Security restrictions are critical for governance/financial projects
- Some MCP packages are deprecated but still functional
- Always validate MCP package availability before configuration
- Document prompt engineering process is part of HackYeah deliverables

**Next Steps**:
- [ ] Set up GitHub personal access token in .env.mcp
- [ ] Test MCP filesystem operations with Angular component generation
- [ ] Configure Puppeteer for WCAG 2.2 accessibility testing
- [ ] Create first AI-assisted Angular component using MCP context
- [ ] Test GitHub integration for automated commit messages

**Usage Examples**:

**Example 1 - Generate Angular Component with Context**:
```
Prompt: "Create a podmiot-list component with table showing podmiot entities, 
pagination (20 items/page), filtering by name and regon, sticky header for 
accessibility, and high contrast theme support"

Expected: MCP filesystem reads existing patterns and generates consistent code
```

**Example 2 - E2E Test Generation**:
```
Prompt: "Create Puppeteer test for login flow with OAuth2 and verify JWT 
token storage, including keyboard navigation tests"

Expected: MCP Puppeteer generates accessibility-compliant test automation
```

**Example 3 - Repository Integration**:
```
Prompt: "Show recent commits affecting authentication module and suggest 
improvements for JWT validation"

Expected: MCP GitHub retrieves commit history and provides context-aware suggestions
```

---

## 📊 Summary Statistics

- **Total Prompts**: 2
- **Successful Implementations**: 2
- **Failed Attempts**: 0
- **Average Effectiveness**: ⭐⭐⭐⭐⭐ (5.0/5.0)

---

## 🔑 Best Practices Discovered

1. **Security-First Prompting**:
   - Always mention project context (governance/financial) in prompts
   - Request security considerations explicitly
   - Ask for .gitignore updates when handling secrets

2. **Incremental Setup**:
   - Break complex setups into smaller, testable steps
   - Validate each step before proceeding
   - Document failures and workarounds

3. **Context Provision**:
   - Provide architecture details (Angular 20 + ASP.NET Core)
   - Mention project requirements (WCAG 2.2, OAuth2, etc.)
   - Reference existing files/folders when relevant

4. **Validation Requests**:
   - Ask AI to verify package availability
   - Request alternative solutions when packages are deprecated
   - Get security review of generated configurations

---

## 🎓 Lessons Learned

### Do's:
✅ Specify project context (governance platform, financial regulations)  
✅ Request security best practices explicitly  
✅ Ask for multiple solution options  
✅ Validate package existence before installation  
✅ Document every prompt and its outcome  

### Don'ts:
❌ Assume all MCP packages exist in npm registry  
❌ Skip .gitignore updates when creating config files  
❌ Use deprecated packages without noting alternatives  
❌ Hardcode secrets in configuration files  
❌ Skip manual approval for infrastructure operations  

---

## 🔮 Future Prompt Ideas

1. **Angular Component Generation**:
   - "Generate podmiot CRUD components with Entity Framework integration"
   - "Create file upload component with chunking and virus scan placeholder"
   - "Build communication dashboard with real-time message updates via SignalR"

2. **ASP.NET Core API Development**:
   - "Create REST API for podmiot management with pagination and filtering"
   - "Implement OAuth2/OIDC authentication with JWT tokens"
   - "Generate Entity Framework migrations for podmiot and communication tables"

3. **Testing & Quality**:
   - "Create Puppeteer tests for WCAG 2.2 compliance"
   - "Generate unit tests for podmiot service with NSubstitute mocks"
   - "Build integration tests for authentication flow"

4. **DevOps & Infrastructure**:
   - "Create docker-compose for PostgreSQL, Redis, and API services"
   - "Generate GitHub Actions workflow for CI/CD pipeline"
   - "Build health check endpoints for Kubernetes readiness probes"

---

### 9. Database Schema Creation

**Prompt**: 
```
Create directory "database" in which it will be located script creating tables in postgres based on .requirements
```

**Context**: Need to create PostgreSQL database schema based on UKNF requirements documents

**AI Response Summary**:
- Analyzed `.requirements/DETAILS_UKNF_Prompt2Code2.md` for data model requirements
- Identified three main modules: Communication, Authentication & Authorization, Administration
- Created comprehensive schema with 40+ tables covering all functional requirements
- Included proper indexes, foreign keys, enums, and triggers

**Implementation**:
Created three files in `database/` directory:
1. **init-schema.sql** (650+ lines):
   - Core entities table (Podmioty Nadzorowane) with full field set from requirements
   - User management with role-based access control
   - Access requests workflow
   - Reporting module with validation status tracking
   - Messaging system with attachments
   - File library with permissions
   - Administrative cases with history
   - Announcements with read confirmations
   - Contact groups and addressees
   - FAQ system with tags and ratings
   - Audit logging and history tracking
   - Password policy management
   - Comprehensive indexes and triggers

2. **seed-data.sql** (300+ lines):
   - Sample entities (5 financial institutions)
   - User accounts (3 UKNF employees, 5 external users)
   - User-entity permissions
   - Report registries and sample reports
   - Contact groups and contacts
   - Library files (templates and documentation)
   - Announcements with recipients
   - FAQ questions and answers
   - Administrative cases
   - Messages and notifications

3. **README.md**:
   - Complete documentation of schema structure
   - Usage instructions for different environments
   - Data model highlights with ASCII diagrams
   - Maintenance and backup procedures

**Effectiveness**: ⭐⭐⭐⭐⭐ (Excellent)

**Key Learnings**:
- Requirements documents contain detailed data structure specifications (e.g., entity fields on p. 11)
- PostgreSQL ENUMs provide type safety for status fields
- Comprehensive indexes are crucial for performance (especially on foreign keys and status fields)
- Triggers for automatic timestamp updates reduce boilerplate
- Seed data is essential for development and testing
- History/audit tables enable versioning and compliance requirements

**Database Design Decisions**:
- Used BIGSERIAL for primary keys (scalability for government system)
- VARCHAR lengths aligned with requirements (e.g., entity_name: 500, matching spec)
- Separate tables for history/versioning (entity_history, case_history, etc.)
- Status fields as ENUMs for data integrity
- Comprehensive foreign key relationships with CASCADE deletes where appropriate
- Separate internal/external user handling via is_internal flag

**Security Notes**:
- Password hashes stored (never plaintext)
- Password policy table for configurable security settings
- Password history tracking to prevent reuse
- Audit log for all critical operations
- User status tracking (active, blocked, pending)
- PESEL numbers masked (only last 4 digits visible)

**Requirements Mapping**:
- Entity fields match specification exactly (p. 11 of DETAILS document)
- Report statuses align with validation workflow (p. 7)
- Case categories and statuses from requirements (p. 10)
- Message contexts and statuses per specification (p. 8)
- Access request workflow per requirements (pp. 13-14)

**Follow-up Tasks**:
- Create Entity Framework Core migrations from this schema
- Generate TypeScript interfaces for frontend
- Build API DTOs matching database structure
- Implement repository pattern in backend

---

## 📝 Template for New Prompt Entries

```markdown
### [Number]. [Feature/Task Name]

**Prompt**: 
```
[Exact prompt text]
```

**Context**: [What were you trying to achieve?]

**AI Response Summary**: [Key points from AI response]

**Implementation**: [What was actually built/configured]

**Effectiveness**: ⭐⭐⭐⭐⭐ ([Rating explanation])

**Key Learnings**: 
- [Learning 1]
- [Learning 2]

**Security Notes**: [Any security considerations]

**Follow-up Prompts**: [Prompts that built on this one]

---
```

## 🔒 Security Reminders

This prompts.md file is part of the project deliverables for HackYeah 2025. It demonstrates:
- **Systematic approach** to AI-assisted development
- **Security-first mindset** for governance applications
- **Transparency** in development process
- **Knowledge sharing** for team collaboration

**⚠️ WARNING**: Never include actual credentials, API keys, or sensitive data in this file. Use placeholders and reference .env files.

---

*Last Updated: October 4, 2025*  
*Project: HackYeah 2025 - UKNF Communication Platform*  
*Team: Clouds On Mars*
