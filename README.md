# MicroMLAst

A simple .NET web app that parses **MicroML** code and visualizes its Abstract Syntax Tree (AST) using [Mermaid](https://mermaid-js.github.io/).

## Project Structure

```
MicroMLAst/
├── MicroMLAst.Core/      # Core library with Lexer, Parser, AST definitions, and Mermaid visitor
├── MicroMLAst.Web/       # ASP.NET Core minimal API serving the SPA
├── LICENSE               # MIT License
├── README.md             # This file
└── .gitignore            # Excludes build artifacts and Scratch harness
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- A modern web browser

### Build & Run

1. Clone the repo:
   ```bash
   git clone https://github.com/BenjaminDanker/MicroMLAst.git
   cd MicroMLAst
   ```
2. Restore and build:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Run the web app:
   ```bash
   dotnet run --project MicroMLAst.Web
   ```
4. Open your browser at `http://localhost:5209`.

## Usage

1. Enter MicroML code into the text box, e.g.:
   ```ml
   let x = 1 in x + 2 end
   ```
2. Click **Draw AST**.
3. View the rendered AST diagram inline.

### API Endpoint

- `POST /api/parse`  
  Request body: `{ "code": "<MicroML code>" }`  
  Response: `{ "mermaid": "graph TD ..." }`

## Screenshot

![AST Screenshot](screenshots/ast1.png)
![AST Screenshot](screenshots/ast2.png)
![AST Screenshot](screenshots/ast3.png)

## Contributing

1. Fork the repository  
2. Create a feature branch: `git checkout -b feature/YourFeature`  
3. Commit changes: `git commit -m "feat: add ..."`  
4. Push branch: `git push origin feature/YourFeature`  
5. Open a Pull Request

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
