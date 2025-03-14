using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RazorPagesJavaLexerParserANTLR.ANTLR;
using System.Text.RegularExpressions;

namespace RazorPagesJavaLexerParserANTLR.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string InputExpression { get; set; } = "public class HelloWorld { \r\n   public static void main(String[] args) { \r\n      System.out.println(1);\r\n      int a = 2 + 2;\r\n   }\r\n}";

        public void OnPost()
        {
            string input = InputExpression;
            AntlrInputStream inputStream = new AntlrInputStream(input);
            JavaGrammarLexer lexer = new JavaGrammarLexer(inputStream);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            JavaGrammarParser parser = new JavaGrammarParser(tokenStream);

            var errorListener = new SyntaxErrorListener();
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            var parseTree = parser.compilationUnit();
            
            if (errorListener.Errors.Count > 0)
            {
                ViewData["SyntaxErrors"] = errorListener.Errors;
            }
            else
            {
                var treeNode = ConvertParseTreeToTreeNode(parseTree, parser);
                ViewData["ParseTree"] = JsonConvert.SerializeObject(treeNode);
            }

            var tokens = new List<object>();
            for (int i = 0; i < tokenStream.Size; i++)
            {
                var token = tokenStream.Get(i);
                tokens.Add(new
                {
                    Index = i,
                    TokenType = parser.Vocabulary.GetSymbolicName(token.Type),
                    Value = token.Text,
                    Position = $"Line {token.Line}, Column {token.Column}"
                });
            }

            ViewData["Tokens"] = tokens;
        }

        public void OnGet()
        {
            var inputStream = new AntlrInputStream(InputExpression);
            var lexer = new JavaGrammarLexer(inputStream);
            var tokens = lexer.GetAllTokens().ToList();

            // Формируем данные для таблицы
            var tokenData = tokens.Select((token, index) => new
            {
                Index = index + 1,
                TokenType = lexer.Vocabulary.GetSymbolicName(token.Type) ?? $"UNKNOWN_{token.Type}",
                Value = token.Text,
                Position = $"Строка {token.Line}, позиция {token.Column}"
            }).ToList();

            ViewData["Tokens"] = tokenData;

            var newInputStream = new AntlrInputStream(InputExpression);
            var newLexer = new JavaGrammarLexer(newInputStream);
            var tokenStream = new CommonTokenStream(newLexer);
            var parser = new JavaGrammarParser(tokenStream);
            var parseTree = parser.compilationUnit();

            // Преобразуем дерево синтаксического разбора в TreeNode
            var treeNode = ConvertParseTreeToTreeNode(parseTree, parser);
            ViewData["ParseTree"] = JsonConvert.SerializeObject(treeNode);
        }

        public class TreeNode
        {
            public string Text { get; set; }
            public List<TreeNode> Children { get; set; } = new List<TreeNode>();
        }

        public static TreeNode ConvertParseTreeToTreeNode(IParseTree parseTree, JavaGrammarParser parser)
        {
            if (parseTree == null)
            {
                return new TreeNode { Text = "null" };
            }

            try
            {
                string nodeText;

                if (parseTree is ITerminalNode terminalNode)
                {
                    string tokenText = terminalNode.Symbol.Text;
                    tokenText = Regex.Replace(tokenText, "\"", "\\\"");
                    nodeText = tokenText;
                }
                else
                {
                    int ruleIndex = parseTree switch
                    {
                        IRuleNode ruleNode => ruleNode.RuleContext.RuleIndex,
                        _ => 0
                    };
                    nodeText = parser.RuleNames[ruleIndex];
                }

                var node = new TreeNode { Text = nodeText };

                for (int i = 0; i < parseTree.ChildCount; i++)
                {
                    node.Children.Add(ConvertParseTreeToTreeNode(parseTree.GetChild(i), parser));
                }

                return node;
            }
            catch (Exception ex)
            {
                return new TreeNode { Text = $"Error: {ex.Message}" };
            }
        }
    }
}