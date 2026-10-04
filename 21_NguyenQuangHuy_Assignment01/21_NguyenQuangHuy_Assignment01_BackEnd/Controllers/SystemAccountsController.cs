using System;
using System.Linq;
using _21_NguyenQuangHuy_Assignment01_BackEnd.DTOs;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;

namespace _21_NguyenQuangHuy_Assignment01_BackEnd.Controllers;

[Authorize(Roles = "Admin")]
public class SystemAccountsController : ODataController
{
    private readonly IAccountRepository _accountRepo;
    private readonly IConfiguration _config;

    public SystemAccountsController(IAccountRepository accountRepo, IConfiguration config)
    {
        _accountRepo = accountRepo;
        _config = config;
    }

    [EnableQuery]
    [HttpGet("odata/SystemAccounts")]
    public IActionResult Get()
    {
        return Ok(_accountRepo.GetAll().AsQueryable());
    }

    [EnableQuery]
    [HttpGet("odata/SystemAccounts({key})")]
    public IActionResult Get([FromRoute] short key)
    {
        var account = _accountRepo.GetById(key);
        if (account == null)
        {
            return NotFound(new { message = $"Account with ID {key} not found." });
        }
        return Ok(account);
    }

    [HttpPost("api/SystemAccounts")]
    public IActionResult Create([FromBody] AccountCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var adminEmail = _config["AdminAccount:Email"];
        if (!string.IsNullOrEmpty(adminEmail) && dto.AccountEmail.Trim().Equals(adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Cannot use the Administrator email for a system account." });
        }

        var existingWithEmail = _accountRepo.GetByEmail(dto.AccountEmail);
        if (existingWithEmail != null)
        {
            return BadRequest(new { message = "An account with this email already exists." });
        }

        var account = new SystemAccount
        {
            AccountName = dto.AccountName.Trim(),
            AccountEmail = dto.AccountEmail.Trim(),
            AccountRole = dto.AccountRole,
            AccountPassword = dto.AccountPassword
        };

        _accountRepo.Add(account);
        return Ok(account);
    }

    [HttpPut("api/SystemAccounts/{id}")]
    public IActionResult Update(short id, [FromBody] AccountUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var account = _accountRepo.GetById(id);
        if (account == null)
        {
            return NotFound(new { message = $"Account with ID {id} not found." });
        }

        var adminEmail = _config["AdminAccount:Email"];
        if (!string.IsNullOrEmpty(adminEmail) && dto.AccountEmail.Trim().Equals(adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Cannot use the Administrator email for a system account." });
        }

        var existingWithEmail = _accountRepo.GetByEmail(dto.AccountEmail);
        if (existingWithEmail != null && existingWithEmail.AccountId != id)
        {
            return BadRequest(new { message = "An account with this email already exists." });
        }

        account.AccountName = dto.AccountName.Trim();
        account.AccountEmail = dto.AccountEmail.Trim();
        account.AccountRole = dto.AccountRole;
        if (!string.IsNullOrWhiteSpace(dto.AccountPassword))
        {
            account.AccountPassword = dto.AccountPassword;
        }

        _accountRepo.Update(account);
        return NoContent();
    }

    [HttpDelete("api/SystemAccounts/{id}")]
    public IActionResult Delete(short id)
    {
        var account = _accountRepo.GetById(id);
        if (account == null)
        {
            return NotFound(new { message = $"Account with ID {id} not found." });
        }

        if (!_accountRepo.CanDelete(id))
        {
            return BadRequest(new { message = "This account has created news articles and cannot be deleted." });
        }

        _accountRepo.Delete(id);
        return NoContent();
    }
}
