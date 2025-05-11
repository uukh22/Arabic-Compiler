using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FinalProjectOfCompiler_Arabic_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text;

            // Scanner
            Scanner scanner = new Scanner();
            var tokens = scanner.Tokenize(input);

            // عرض الرموز كما هي في الترتيب
            lstTokens.Items.Clear();
            foreach (var token in tokens)
            {
                lstTokens.Items.Add($"{token.TokenType}: {token.Value.ToString()}");
            }

            // Implement parser here
            try
            {
                lstParsers.Items.Clear();
                Parser parser = new Parser(tokens);
                parser.ParseProgram();

                // Display parse tree structure
                var parseTree = parser.GetParseTree();
                foreach (var entry in parseTree)
                {
                    lstParsers.Items.Add(entry);
                }

                MessageBox.Show("The input code is syntactically valid.");
            }
            catch (Exception ex)
            {
                lstParsers.Items.Add($"Error: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class Scanner
    {
        private static readonly Dictionary<string, string> TokenPatterns = new()
        {
            { "KEYWORD", @"\b(اذا|طالما|متغير|الا)\b" },
            { "LBRACE", @"\{" },
            { "LPAREN", @"\(" },
            { "IDENT", @"[a-zA-Z_]\w*" },
            { "ASSIGN", @"=" },
            { "NUM", @"\b\d+\b" },
            { "OPERATOR", @"[+\-*/]|==|!=|<=|>=|<|>" },
            { "SEMICOLON", @";" },
            { "RPAREN", @"\)" },
            { "RBRACE", @"\}" },
            { "WS", @"\s+" }
        };

        public List<(string TokenType, string Value)> Tokenize(string input)
        {
            var tokens = new List<(string TokenType, string Value)>();
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                int currentIndex = 0;

                while (currentIndex < line.Length)
                {
                    bool matchFound = false;

                    foreach (var pattern in TokenPatterns)
                    {
                        var regex = new Regex(pattern.Value);
                        var match = regex.Match(line, currentIndex);

                        if (match.Success && currentIndex == match.Index)
                        {
                            if (pattern.Key != "WS") tokens.Add((pattern.Key, match.Value.ToString()));
                            currentIndex = match.Index + match.Length;
                            matchFound = true;
                            break;
                        }
                    }

                    if (!matchFound)
                    {
                        throw new InvalidOperationException($"Invalid character at position {currentIndex} in line: {line}");
                    }
                }
            }
            return tokens;
        }
    }

    public class Parser
    {
        private List<(string TokenType, string Value)> tokens;
        private int currentIndex;
        private List<string> parseTree; // To store the parse tree structure
        private int indentLevel; // To manage indentation

        public Parser(List<(string TokenType, string Value)> tokens)
        {
            this.tokens = tokens;
            this.currentIndex = 0;
            this.parseTree = new List<string>(); // Initialize the parse tree list
            this.indentLevel = 0; // Initialize indentation level
        }

        private (string TokenType, string Value) CurrentToken =>
            currentIndex < tokens.Count ? tokens[currentIndex] : (null, null);

        private void Consume(string expectedTokenType)
        {
            if (CurrentToken.TokenType == expectedTokenType)
            {
                currentIndex++;
            }
            else
            {
                throw new ParserException($"Expected {expectedTokenType} but found {CurrentToken.TokenType} at index {currentIndex}");
            }
        }

        public ASTNode ParseProgram()
        {
            var statements = new List<ASTNode>();
            parseTree.Add($"{GetIndentation()}<program> → <statement_list>"); // Add program entry
            indentLevel++;
            while (CurrentToken.TokenType != null)
            {
                statements.Add(ParseStatement());
            }
            indentLevel--;
            return new ProgramNode(statements);
        }

        private ASTNode ParseStatement()
        {
            if (CurrentToken.TokenType == "KEYWORD" && CurrentToken.Value == "متغير")
            {
                return ParseDeclaration();
            }
            else if (CurrentToken.TokenType == "KEYWORD" && CurrentToken.Value == "طالما")
            {
                return ParseWhileStatement();
            }
            else if (CurrentToken.TokenType == "KEYWORD" && CurrentToken.Value == "اذا")
            {
                return ParseIfStatement();
            }
            else if (CurrentToken.TokenType == "IDENT")
            {
                return ParseAssignmentOrFunctionCall();
            }
            else
            {
                throw new ParserException($"Unexpected token: {CurrentToken.TokenType} at index {currentIndex}");
            }
        }

        private ASTNode ParseDeclaration()
        {
            Consume("KEYWORD"); // Consume "متغير"
            var identifier = CurrentToken.Value;
            Consume("IDENT");   // Consume identifier
            Consume("ASSIGN");  // Consume "="
            var value = ParseExpression(); // Parse the expression
            Consume("SEMICOLON"); // Consume ";"

            // Add to parse tree
            parseTree.Add($"{GetIndentation()}<declaration> → متغير {identifier.ToString()} = {value.ToString()};");
            return new DeclarationNode(identifier, value);
        }

        private ASTNode ParseWhileStatement()
        {
            Consume("KEYWORD"); // Consume "طالما"
            Consume("LPAREN");   // Consume "("
            var condition = ParseExpression(); // Parse the condition
            Consume("RPAREN");   // Consume ")"
            Consume("LBRACE");   // Consume "{"
            var body = ParseStatementList(); // Parse the statement list inside the while
            Consume("RBRACE");   // Consume "}"

            // Add to parse tree
            parseTree.Add($"{GetIndentation()}<while_statement> → طالما ( {condition.ToString()} ) {{ {string.Join(" ", body)} }}");
            return new WhileNode(condition, body);
        }

        private ASTNode ParseIfStatement()
        {
            Consume("KEYWORD"); // Consume "اذا"
            Consume("LPAREN");   // Consume "("
            var condition = ParseExpression(); // Parse the condition
            Consume("RPAREN");   // Consume ")"
            Consume("LBRACE");   // Consume "{"
            var thenBody = ParseStatementList(); // Parse the statement list inside the if
            Consume("RBRACE");   // Consume "}"

            List<ASTNode> elseBody = null; // Initialize as null
            if (CurrentToken.TokenType == "KEYWORD" && CurrentToken.Value == "الا")
            {
                Consume("KEYWORD"); // Consume "الا"
                Consume("LBRACE");   // Consume "{"
                elseBody = ParseStatementList(); // Parse the statement list inside the else
                Consume("RBRACE");   // Consume "}"
            }

            // Add to parse tree
            parseTree.Add($"{GetIndentation()}<if_statement> → اذا ( {condition.ToString()} ) {{ {string.Join(" ", thenBody)} }}");
            if (elseBody != null)
            {
                parseTree.Add($"{GetIndentation()}<else_statement> → الا {{ {string.Join(" ", elseBody)} }}");
            }
            return new IfNode(condition, thenBody, elseBody);
        }

        private ASTNode ParseAssignmentOrFunctionCall()
        {
            var identifier = CurrentToken.Value;
            Consume("IDENT"); // Consume identifier
            if (CurrentToken.TokenType == "ASSIGN")
            {
                Consume("ASSIGN"); // Consume "="
                var value = ParseExpression(); // Parse the expression
                Consume("SEMICOLON"); // Consume ";"
                                      // Add to parse tree
                parseTree.Add($"{GetIndentation()}<assignment> → {identifier} = {value.ToString()};");
                return new AssignmentNode(identifier, value);
            }
            else if (CurrentToken.TokenType == "LPAREN")
            {
                Consume("LPAREN"); // Consume "("
                var arguments = ParseFunctionArguments();
                Consume("RPAREN"); // Consume ")"
                Consume("SEMICOLON"); // Consume ";"
                                      // Add to parse tree
                parseTree.Add($"{GetIndentation()}<function_call> → {identifier}({string.Join(", ", arguments)})");
                return new FunctionCallNode(identifier, arguments);
            }
            else
            {
                throw new ParserException($"Unexpected token after identifier: {CurrentToken.TokenType} at index {currentIndex}");
            }
        }

        private List<ASTNode> ParseFunctionArguments()
        {
            var arguments = new List<ASTNode>();

            // Parse the first argument
            arguments.Add(ParseExpression());

            // Parse additional arguments if they exist
            while (CurrentToken.TokenType == "OPERATOR" && CurrentToken.Value == ",")
            {
                Consume("OPERATOR"); // Consume the comma
                arguments.Add(ParseExpression()); // Parse the next argument
            }

            return arguments;
        }

        private List<ASTNode> ParseStatementList()
        {
            var statements = new List<ASTNode>();
            while (CurrentToken.TokenType != null && CurrentToken.TokenType != "RBRACE")
            {
                statements.Add(ParseStatement());
            }
            return statements;
        }

        private ASTNode ParseExpression()
        {
            var left = ParseTerm();
            while (CurrentToken.TokenType == "OPERATOR")
            {
                var operatorToken = CurrentToken.TokenType;
                var operatorValue = CurrentToken.Value;
                Consume(operatorToken); // Consume operator
                var right = ParseTerm(); // Parse the next term
                left = new BinaryOperationNode(left, operatorValue, right);
            }
            return left;
        }

        private ASTNode ParseTerm()
        {
            var left = ParseFactor();
            while (CurrentToken.TokenType == "OPERATOR")
            {
                var operatorToken = CurrentToken.TokenType;
                var operatorValue = CurrentToken.Value;
                Consume(operatorToken); // Consume operator
                var right = ParseFactor(); // Parse the next factor
                left = new BinaryOperationNode(left, operatorValue, right);
            }
            return left;
        }

        private ASTNode ParseFactor()
        {
            if (CurrentToken.TokenType == "NUM")
            {
                var value = CurrentToken.Value;
                Consume("NUM"); // Consume number
                return new NumberNode(value);
            }
            else if (CurrentToken.TokenType == "IDENT")
            {
                var identifier = CurrentToken.Value;
                Consume("IDENT"); // Consume identifier
                return new IdentifierNode(identifier);
            }
            else if (CurrentToken.TokenType == "LPAREN")
            {
                Consume("LPAREN"); // Consume "("
                var expression = ParseExpression(); // Parse the nested expression
                Consume("RPAREN"); // Consume ")"
                return expression;
            }
            else
            {
                throw new ParserException($"Unexpected token in expression: {CurrentToken.TokenType} at index {currentIndex}");
            }
        }

        public List<string> GetParseTree()
        {
            return parseTree; // Return the parse tree structure
        }

        private string GetIndentation()
        {
            return new string('\t', indentLevel);
        }
    }

    public abstract class ASTNode
    {
        // Base class for all AST nodes
        public abstract override string ToString();
    }

    public class ProgramNode : ASTNode
    {
        public List<ASTNode> Statements { get; }

        public ProgramNode(List<ASTNode> statements)
        {
            Statements = statements;
        }

        public override string ToString()
        {
            return "<program> → " + string.Join(" ", Statements);
        }
    }

    public class DeclarationNode : ASTNode
    {
        public string Identifier { get; }
        public ASTNode Value { get; }

        public DeclarationNode(string identifier, ASTNode value)
        {
            Identifier = identifier;
            Value = value;
        }

        public override string ToString()
        {
            return $"<declaration> → متغير {Identifier} = {Value.ToString()}";
        }
    }

    public class WhileNode : ASTNode
    {
        public ASTNode Condition { get; }
        public List<ASTNode> Body { get; }

        public WhileNode(ASTNode condition, List<ASTNode> body)
        {
            Condition = condition;
            Body = body;
        }

        public override string ToString()
        {
            return $"<while_statement> → طالما ( {Condition.ToString()} ) {{ {string.Join(" ", Body)} }}";
        }
    }

    public class IfNode : ASTNode
    {
        public ASTNode Condition { get; }
        public List<ASTNode> ThenBody { get; }
        public List<ASTNode> ElseBody { get; }

        public IfNode(ASTNode condition, List<ASTNode> thenBody, List<ASTNode> elseBody)
        {
            Condition = condition;
            ThenBody = thenBody;
            ElseBody = elseBody;
        }

        public override string ToString()
        {
            var elsePart = ElseBody != null ? $" else {{ {string.Join(" ", ElseBody)} }}" : "";
            return $"<if_statement> → اذا ( {Condition.ToString()} ) {{ {string.Join(" ", ThenBody)} }}{elsePart}";
        }
    }

    public class AssignmentNode : ASTNode
    {
        public string Identifier { get; }
        public ASTNode Value { get; }

        public AssignmentNode(string identifier, ASTNode value)
        {
            Identifier = identifier;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Identifier} = {Value.ToString()}";
        }
    }

    public class FunctionCallNode : ASTNode
    {
        public string Identifier { get; }
        public List<ASTNode> Arguments { get; }

        public FunctionCallNode(string identifier, List<ASTNode> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"<function_call> → {Identifier}({string.Join(", ", Arguments)})";
        }
    }

    public class BinaryOperationNode : ASTNode
    {
        public ASTNode Left { get; }
        public string Operator { get; }
        public ASTNode Right { get; }

        public BinaryOperationNode(ASTNode left, string operatorToken, ASTNode right)
        {
            Left = left;
            Operator = operatorToken;
            Right = right;
        }

        public override string ToString()
        {
            return $"{Left.ToString()} {Operator} {Right.ToString()}";
        }
    }

    public class NumberNode : ASTNode
    {
        public string Value { get; }

        public NumberNode(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value; // Return the actual value of the number
        }
    }

    public class IdentifierNode : ASTNode
    {
        public string Name { get; }

        public IdentifierNode(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name; // Return the actual name of the identifier
        }
    }

    public class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }
    }
}