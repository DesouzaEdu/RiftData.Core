using Genocs.CleanArchitecture.Template.Application.Boundaries.CloseAccount;
using Genocs.CleanArchitecture.Template.Application.Repositories;
using Genocs.CleanArchitecture.Template.Application.Services;

namespace Genocs.CleanArchitecture.Template.Application.UseCases;

public sealed class CloseAccount : IUseCase
{
    private readonly IOutputPort _outputHandler;

    public CloseAccount(
        IOutputPort outputHandler)
    {
        _outputHandler = outputHandler;
    }

    public async Task Execute(CloseAccountInput closeAccountInput)
    {
        var closeAccountOutput = new CloseAccountOutput(Guid.NewGuid());
        _outputHandler.Default(closeAccountOutput);
    }
}