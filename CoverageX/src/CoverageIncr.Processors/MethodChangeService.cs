using System.Text;
using CoverageIncr.Shared;
using LibGit2Sharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoverageIncr.Processors;

public class MethodChangeService : IMethodChangeService
{
    public ReportGeneratorFilter GetDiffMethodDescriptions(Blob targetBlob, Blob sourceBlob, string sourceFilePath)
    {
        throw new NotImplementedException();
    }

    public HashSet<int> GetMethodLineNumbers(Blob blob)
    {
        var methodLineNumbers = new HashSet<int>();
        if (blob == null)
            return methodLineNumbers;

        var content = blob.GetContentText(Encoding.UTF8);
        var syntaxTree = CSharpSyntaxTree.ParseText(content);
        var classes = syntaxTree.GetRoot().DescendantNodes().Where(x => x.GetType() == typeof(ClassDeclarationSyntax) || x.GetType() == typeof(RecordDeclarationSyntax));
        if (classes == null || classes.Count() == 0)
            return methodLineNumbers;
            
        foreach (var typeDeclaration in classes)
        {
            var methods = typeDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var lineSpan = method.GetLocation().GetLineSpan();
                var lineNumber = lineSpan.StartLinePosition.Line + 1; // 行号是从0开始的，所以加1

                methodLineNumbers.Add(lineNumber);
            }
        }

        return methodLineNumbers;
    }
}