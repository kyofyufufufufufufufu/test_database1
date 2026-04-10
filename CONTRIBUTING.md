# Contributing Guide

## Code Quality

Before submitting code, ensure it follows standard C# conventions.

- **Format:** Ensure your IDE is set to use the EditorConfig settings included in the repo
- **Build check:** Run a clean build to ensure no new warnings or errors are introduced:
```bash
dotnet build
```

---

## Contribution Workflow

### 1. Branching

Always create a new branch for your work. Do not commit directly to `main`.

Examples:
- `feature/minigame-name`
- `fix/bug-description`
- `docs/update-csv-template`

### 2. Opening a Pull Request

When your feature is ready, open a PR against `main`. Your PR description should include:

- What was changed or added
- A screenshot of the UI if applicable
- Anything that caused changes to the JSON database structure

### 3. Definition of Done

A task is considered done only when:

- The code compiles without errors or null warnings
- Images upload correctly to the `/images` folder on GitHub
- The locations bitwise encoding logic is verified for the specific module
- `jsonTest.json` is successfully updated via the app

### 4. Code Review Expectations

- Expect feedback within 24–48 hours
- Approve only if you have verified the JSON output is valid and the app runs without errors

---

## Reporting Bugs & Requests

Open an [Issue](https://github.com/kyofyufufufufufufufu/test_database1/issues) in this repository and include:

- Steps to reproduce the bug
- The CSV file used (if it was a bulk upload error)
- The specific error message received in the MessageBox

---

## Support & Handoff

For installation help, technical issues, or feature requests, submit a request via the [PharmacyGO Support & Feedback Form](https://forms.gle/NajcPYnEa8jS3CWN6). Submissions are monitored by the project manager and will be routed to the appropriate team or future development group.

If you run into issues with the GitHub API or the WinForms designer, refer to the `DataModels.cs` comments for bitwise logic explanations, or contact geigerta@oregonstate.edu.
