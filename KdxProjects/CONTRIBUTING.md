# Contributing to Kdx Class Libraries

Thank you for your interest in contributing to the Kdx project! This document provides a quick overview of our contribution process.

## 📚 Full Documentation

For detailed guidelines, please refer to our comprehensive documentation:
- **日本語**: [貢献ガイド (Japanese)](./docs/contribution-guide.md)
- **English**: See below for quick reference

## Quick Start

### 1. Create an Issue

Before starting work, create an issue to discuss:
- Bug reports
- Feature requests
- Improvements

### 2. Fork & Branch

```bash
# Fork the repository on GitHub
# Clone your fork
git clone https://github.com/YOUR_USERNAME/kdxprojects.git

# Create a feature branch
git checkout -b feature/123-your-feature
```

### 3. Make Changes

- Follow our [coding standards](#coding-standards)
- Write tests for new features
- Update documentation as needed

### 4. Commit

Use [Conventional Commits](https://www.conventionalcommits.org/):

```bash
git commit -m "feat(contracts): add Company CRUD methods

- Add GetCompanyByIdAsync
- Add AddCompanyAsync
- Add UpdateCompanyAsync
- Add DeleteCompanyAsync

Closes #123"
```

### 5. Push & Create Pull Request

```bash
git push origin feature/123-your-feature
```

Then create a Pull Request on GitHub.

## Branch Naming Convention

- `feature/<issue>-<description>` - New features
- `fix/<issue>-<description>` - Bug fixes
- `docs/<issue>-<description>` - Documentation
- `refactor/<issue>-<description>` - Code refactoring

## Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation only
- `style`: Formatting changes
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding tests
- `chore`: Build/tooling changes

## Coding Standards

### C# Style

```csharp
// Class names: PascalCase
public class SupabaseRepository { }

// Interface names: IPascalCase
public interface ISupabaseRepository { }

// Methods: PascalCase with Async suffix
public async Task<Company> GetCompanyByIdAsync(int id) { }

// Private fields: _camelCase
private readonly Client _supabaseClient;

// Local variables: camelCase
var companyData = await GetData();
```

### XML Documentation

```csharp
/// <summary>
/// Gets a company by its ID
/// </summary>
/// <param name="id">The company ID</param>
/// <returns>The company, or null if not found</returns>
public async Task<Company?> GetCompanyByIdAsync(int id)
{
    // Implementation
}
```

## Pull Request Checklist

- [ ] Code follows coding standards
- [ ] Self-reviewed the code
- [ ] Added/updated tests
- [ ] All tests pass
- [ ] Updated documentation
- [ ] No new warnings
- [ ] Build succeeds

## Testing

```bash
# Run tests
dotnet test

# Check coverage (target: 80%+)
dotnet test /p:CollectCoverage=true
```

## Versioning

We use [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

Example: `2.1.0-alpha` → `2.1.0` → `2.1.1`

## Code Review Process

1. Submit Pull Request
2. Automated checks run (CI)
3. Code review by maintainers
4. Address feedback
5. Approval & merge

**Review response time:** Within 48 hours

## Issue Labels

- `feature` - New feature
- `bug` - Bug fix
- `enhancement` - Improvement
- `docs` - Documentation
- `priority: high` - High priority
- `good first issue` - Good for newcomers
- `help wanted` - Help needed

## Release Process

1. Update version in `.csproj`
2. Update `CHANGELOG.md`
3. Create release branch
4. Build & test
5. Create Pull Request
6. Merge to master
7. Create Git tag
8. Publish to NuGet

## Getting Help

- **Questions**: GitHub Discussions
- **Bugs**: GitHub Issues
- **Urgent**: Contact maintainers

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on the code, not the person
- Help others learn and grow

## License

By contributing, you agree that your contributions will be licensed under the project's license.

## Resources

- [Full Contribution Guide (Japanese)](./docs/contribution-guide.md)
- [Web Template Guide](./docs/web-template-guide.md)
- [Class Library Benefits](./docs/class-library-benefits.md)

## Thank You!

Your contributions make this project better for everyone. We appreciate your time and effort! 🙏
