using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace RazorPagesJavaLexerParserANTLR.ANTLR
{
    public class SyntaxErrorListener : BaseErrorListener
    {
        public List<AnalysysError> Errors { get; } = new List<AnalysysError>();

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            Errors.Add(new AnalysysError
            {
                Line = line,
                CharPositionInLine = charPositionInLine,
                Message = msg
            });
        }
    }
}
