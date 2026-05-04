# Orc.DynamicObjects

Orc.DynamicObjects is a .NET library that provides dynamic object support built on top of [Catel](https://github.com/catel/catel). It enables creation of objects whose members are not known at compile time, while still benefiting from Catel's property change notification, validation, and serialisation infrastructure.

The library exposes two primary base classes:

- **`DynamicModelBase`** — Extends Catel's `ModelBase` with `IDynamicMetaObjectProvider` support. Dynamic properties are registered at runtime and fully participate in Catel's property system.
- **`DynamicObservableObject`** — Extends Catel's `ObservableObject` with a lightweight dictionary-backed dynamic property store and `INotifyPropertyChanged` integration.

---

## Critical Rules (Read First)

These rules are **non-negotiable**. Violating them causes broken builds, crashes, or downstream breakage.

### 1. Never Edit Generated Files

Files matching `*.generated.cs` are auto-generated.

- **NEVER** manually edit these files

### 2. ABI / API Stability

This project maintains a stable public API. Breaking changes break downstream consumers.

| Allowed | Never |
|---------|-------|
| Add new overloads | Modify existing signatures |
| Add new methods | Remove public APIs |
| Add new classes | Change return types |

### 3. Tests Are Mandatory

**Building alone is NOT sufficient.** Run tests before claiming completion (see [Commands](#commands)).

### 4. Branch Protection (COMPLIANCE REQUIRED)

**Direct commits to protected branches are a policy violation.**

| Repository | Protected Branches |
|------------|-------------------|
| Orc.DynamicObjects | `master` |
| Orc.DynamicObjects | `develop` |

**Required workflow:**

1. **Create a feature branch FIRST** — Use naming convention: `feature/issue-NNNN-description`
2. **Make all commits on the feature branch** — Never commit directly to protected branches
3. **Submit a Pull Request** — Changes must be reviewed by a human before merging

```bash
# CORRECT — Always create a feature branch first
git checkout -b feature/issue-1234-fix-description

# NEVER DO THIS — Policy violation
git checkout develop && git commit  # FORBIDDEN

# NEVER DO THIS — Policy violation
git checkout master && git commit  # FORBIDDEN
```

---

## Commands

Single source of truth for all commands:

| Task | Command |
|------|---------|
| **Build** | `dotnet cake --target=build` |
| **Test** | `dotnet cake --target=test` |
| **Build and test** | `dotnet cake --target=buildandtest` |

---

## Architecture & Directories

### Project Overview

```
src/Orc.DynamicObjects             # Main library (net8.0, net9.0, net10.0)
src/Orc.DynamicObjects.Tests       # NUnit test project
src/Orc.DynamicObjects.Example     # Example / sample project
deployment/                        # Build and deployment scripts (Cake)
```

### Key Types

| Type | Description |
|------|-------------|
| `DynamicModelBase` | Catel `ModelBase` + `IDynamicMetaObjectProvider` |
| `DynamicModelBaseMetaObject` | `DynamicMetaObject` binder for `DynamicModelBase` |
| `DynamicObservableObject` | Catel `ObservableObject` + dictionary-backed dynamic properties |
| `DynamicObservableObjectMetaObject` | `DynamicMetaObject` binder for `DynamicObservableObject` |

### Directory Guide

| Directory / Pattern | Editable? | Notes |
|---------------------|-----------|-------|
| `*.generated.cs` | No | Auto-generated — leave as-is |
| `deployment/` | No | Build / deployment scripts |
| `src/Orc.DynamicObjects/` | Yes | Main library source |
| `src/Orc.DynamicObjects.Tests/` | Yes | Tests |

---

## Writing Code

### Anti-Patterns (Never Do This)

| Anti-Pattern | Why |
|-------------|-----|
| Modifying public method signatures | ABI breaking |
| Manual edits to `*.generated.cs` | Overwritten on regenerate |
| Using default parameters in public APIs | ABI breaking |
| **Skipping failing tests** | **Unacceptable — tests must pass** |

---

## Testing & Debugging

### Running Tests

```bash
dotnet cake --target=test
```

### Tests MUST Pass

> **NON-NEGOTIABLE:** Tests must PASS before claiming completion.
>
> - Do NOT skip failing tests
> - Do NOT claim completion if tests fail
> - Do NOT use `SkipException` to work around failures

### Writing Tests

1. Use NUnit to write tests
2. Create a `Facts` class for a feature (e.g. `DynamicModelBaseFacts`)
3. Nest inner `[TestFixture]` classes per scenario
4. Combine Pascal / Snake case for test methods (e.g. `CorrectlyReturns_TheRightValue`)

```csharp
public class DynamicModelBaseFacts
{
    public class DynamicModel : DynamicModelBase { }

    [TestFixture]
    public class The_GetValue_Properties
    {
        [TestCase]
        public void ReturnsDynamicPropertyValue()
        {
            dynamic model = new DynamicModel();

            model.SomeProperty = "test";

            Assert.That((string)model.SomeProperty, Is.EqualTo("test"));
        }
    }
}
```

**Philosophy:** Tests FAIL when wrong, never skip (except missing hardware).

### Debugging Methodology

1. **Establish baseline** — What's the known-good state?
2. **One change at a time** — Verify each change before proceeding
3. **Track changes in a table** — Log what you changed and the result
4. **Platform differences are signals** — If X works and Y fails, the difference IS the answer
5. **Revert if worse** — Don't pile fixes on top of failures

---

## Further Reading

| Topic | Document |
|-------|----------|
| Contributing guidelines | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Catel (base framework) | https://github.com/catel/catel |
| Documentation portal | http://opensource.wildgums.com |
