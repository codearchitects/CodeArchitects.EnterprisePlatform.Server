using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Analyzer.Tests;

public abstract class CodeFixProviderTest : CompilationTest
{
  protected abstract Type CodeFixProviderType { get; }

  public async Task<string> GetFixedCodeAsync(string code, string diagnosticId)
  {
    CompilationData data = await GetCompilationDataAsync(code);
    data.Deconstruct(out AdhocWorkspace workspace, out _, out _, out Document document, out ImmutableArray<Diagnostic> diagnostics);
    Diagnostic diagnostic = diagnostics.Single(x => x.Id == diagnosticId);

    CodeFixProvider codeFixProvider = (CodeFixProvider)Activator.CreateInstance(CodeFixProviderType)!;

    CodeAction? registeredCodeAction = null;
    CodeFixContext context = new CodeFixContext(document, diagnostic, (codeAction, diagnostics) =>
    {
      if (registeredCodeAction != null)
        throw new Exception("Code action was registered more than once");

      registeredCodeAction = codeAction;
    }, CancellationToken.None);

    await codeFixProvider.RegisterCodeFixesAsync(context);

    if (registeredCodeAction == null)
      throw new Exception("Code action was not registered");

    ImmutableArray<CodeActionOperation> operations = await registeredCodeAction.GetOperationsAsync(CancellationToken.None);
    foreach (CodeActionOperation operation in operations)
    {
      operation.Apply(workspace, CancellationToken.None);
    }

    return (await workspace.CurrentSolution.GetDocument(document.Id)!.GetTextAsync()).ToString();
  }
}
