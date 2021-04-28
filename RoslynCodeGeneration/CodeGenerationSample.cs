using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RoslynCodeGeneration
{
    internal class CodeGenerationSample
    {
        private readonly CodeGenOptions _options;
        private readonly ILogger _logger;
        
        private const string CodeOutput = @"
namespace MyNamespace
{
    public class MyClass
    {
        public string MyProperty { get; set }
    }
}";

        public CodeGenerationSample(
            IOptions<CodeGenOptions> options,
            ILogger<CodeGenerationSample> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        internal async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var targetFile = $"{_options.TargetDirectory}/test.cs";
            var node = CSharpSyntaxTree.ParseText(CodeOutput, cancellationToken: cancellationToken).GetRoot();
            await WriteToFileAsync(node, targetFile).ConfigureAwait(false);
            _logger.LogInformation("Test CS file created: '{0}'", targetFile);
        }

        private static async Task WriteToFileAsync(SyntaxNode syntaxNode, string fileName)
        {
            using (var writer = File.CreateText(fileName))
            {
                await writer.WriteAsync(syntaxNode.ToFullString()).ConfigureAwait(false);
            }
        }
    }
}
