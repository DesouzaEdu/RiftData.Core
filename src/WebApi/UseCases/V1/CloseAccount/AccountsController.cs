using Asp.Versioning;
using Genocs.CleanArchitecture.Template.Application.Boundaries.CloseAccount;
using Microsoft.AspNetCore.Mvc;

namespace Genocs.CleanArchitecture.Template.WebApi.UseCases.V1.CloseAccount;

[ApiVersion("1.0")]
[Route("api/v1/[controller]")]
[ApiController]
public sealed class AccountsController : ControllerBase
{
    private readonly IUseCase _closeAccountUseCase;
    private readonly CloseAccountPresenter _presenter;

    public AccountsController(
        IUseCase closeAccountUseCase,
        CloseAccountPresenter presenter)
    {
        _closeAccountUseCase = closeAccountUseCase;
        _presenter = presenter;
    }

    /// <summary>
    /// Close an Account
    /// </summary>
    /// <response code="200">The closed account id.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Error.</response>
    /// <param name="request">The request to Close an Account.</param>
    /// <returns>The account id.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CloseAccountResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        //var closeAccountInput = new CloseAccountInput(request.AccountId);
        //await _closeAccountUseCase.Execute(closeAccountInput);
        return _presenter.ViewModel;
    }
}