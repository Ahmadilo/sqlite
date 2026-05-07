# 🗃️ SQLite CLI

A lightweight command-line tool for quickly reading SQLite tables directly from the terminal without opening database browsers or repeatedly writing SQL queries.

Built with C# and `System.CommandLine`.

---

# ✨ Why This Tool Exists

Working with SQLite databases is common in:

- 🖥️ Local applications
- 🧪 Testing environments
- 🎮 Game tools
- ⚡ Prototypes
- 🛠️ Small desktop utilities

But reading tables usually means:

- Opening GUI database browsers
- Writing repetitive SQL queries
- Navigating unnecessary menus

This tool simplifies the workflow into a single terminal command.

---

# 🚀 Features

- 📖 Read SQLite tables directly from terminal
- 📐 Automatic table formatting
- 🔍 Automatic `sqlite.config` discovery
- 📂 Relative database path support
- ⚡ Lightweight and fast
- 🧩 Simple command-line interface

---

# 📦 Installation

## Clone Repository

```bash 
git clone https://github.com/ahmadaden/sqlite.git
cd sqlite
```

## Build Project

```bash
dotnet build
```

## Publish Standalone Release

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

#### Example output:

```bash
Id  Name     Age
--  -------  ---
1   Ahmad    21
2   Sarah    19
3   John     30
```

# Using sqlite.config
Instead of writing `--path` every time, create a configuration file named:

```bash
sqlite.config
```
Example:

```txt
path=./database/app.db
```

Now you can simply run:

```bash
sqlite from users
```

 The tool will automatically:
1. 🔎 Search for `sqlite.config`
2. 📄 Read the database path
3. 🗃️ Load the SQLite database

# Project Structure

```
sqlite
├── Program.cs
├── sqlite.csproj
└── sqlite.config
```

# 💡 Example Workflow

```bash
sqlite from products
sqlite from users
sqlite from logs
```

Useful for:
- 🐞 Quick debugging
- 🔍 Inspecting local databases
- ⚙️ Development workflows
- 🧪 Testing environments

## 🧰 Technologies

C#
- .NET
- Microsoft.Data.Sqlite
- System.CommandLine

## 📌 Current Status

This project is currently in early development.

Planned future improvements:

- 🛡️ Better error handling
- 🧠 Custom query support
- 📤 Export options
- 📋 Table listing command
- 🎨 Colored terminal output

## 📄 License

MIT License