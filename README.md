
# FilesBundlerCLI

A command-line tool to collect, sort, and filter files from a folder (including subfolders) by programming languages and combine them into a single text file.

---

## Key Features

- Collect all files from a folder (including nested subfolders).
- Filter files by programming languages (e.g., C#, Python, JavaScript).
- Sort files by name (optional).
- Remove empty lines from files (optional).
- Add a comment with the original file path in the output (optional).
- Save the combined output into a single text file.
- Support running with a Response File containing parameters.

---

## Requirements

- Node.js installed on your machine.

---

## Usage

Run the tool from the command line:

```bash
FilesBundlerCLI --language csharp python --output combined.txt
````

### Main Parameters

* `--language` (or `-l`): List of programming languages to filter by. Examples: `csharp`, `python`, `js`.
* `--output` (or `-o`): The output file name to save the combined files.
* `--note` (or `-n`): If specified, adds a comment with the original file path for each file in the output.
* `--sort` (or `-s`): If specified, sorts files by name.
* `--remove-empty-lines` (or `-r`): If specified, removes empty lines from the files.

---

## Using a Response File

You can save the parameters in a text file, e.g. `params.txt`:

```
--language
csharp
python
--output
combined.txt
--note
--sort
```

And run the tool like this:

```bash
FilesBundlerCLI @params.txt
```

---

## Full Example Command

```bash
FilesBundlerCLI --language csharp python --output combined.txt --note --sort --remove-empty-lines
```

---

## Support and Issues

If you encounter problems or have questions, please open an Issue on GitHub or contact me directly.

---

Thank you for using my tool!

