# Contributing

## Branching

- Do **not** commit or push directly to `main`.
- Create a feature branch from `main`:
  ```bash
  git checkout main
  git pull
  git checkout -b feature/<short-description>
  ```

## Before pushing

Enable the repository's shared Git hooks (once per clone):

```bash
git config core.hooksPath .githooks
```

The `pre-push` hook blocks direct pushes to `main`.

## Pull requests

1. Push your feature branch:
   ```bash
   git push -u origin feature/<short-description>
   ```
2. Open a pull request against `main` on GitHub.
3. Ensure tests pass and address review feedback.
4. Merge via the GitHub UI (do not push to `main` directly).

## Running tests

```bash
dotnet test StudentManagement.Tests.UI/StudentManagement.Tests.UI.csproj
```

See `README.md` for screenshots/reporting output locations.