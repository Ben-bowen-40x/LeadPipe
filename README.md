To turn this solution into a reusable template, create a `.template.config` folder at the root and add a `template.json` file. Set `"sourceName": "Template"` in that file to ensure all occurrences of `Template` in file names, folders, and code (like namespaces) are replaced when generating new projects. Use `"shortName"` to define the CLI command (e.g., `dotnet new clean-arch-template`).

To install the template locally, run the following from the root folder (where `.template.config` is located):

```bash
dotnet new install .
```

To use the template to generate a new project:

```bash
dotnet new clean-arch-template -n MyProjectName
```

If you update files in the template (e.g., `Template.sln`, code files, etc.), reinstall it to reflect those changes:

```bash
dotnet new uninstall clean-arch-template
dotnet new install .
```

---

Let me know if you want to include notes about publishing the template as a NuGet package too.
