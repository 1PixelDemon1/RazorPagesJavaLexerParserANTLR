namespace RazorPagesJavaLexerParserANTLR.ANTLR
{
    public class TreeNode
    {
        public string Text { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
