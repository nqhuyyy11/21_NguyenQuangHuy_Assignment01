using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class SystemAccountDAO
{
    private static SystemAccountDAO? _instance;
    private static readonly object _lock = new();

    private SystemAccountDAO() { }

    public static SystemAccountDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new SystemAccountDAO();
            }
        }
    }

    public List<SystemAccount> GetAll()
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.SystemAccounts.AsNoTracking().ToList();
    }

    public SystemAccount? GetById(short id)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountId == id);
    }

    public SystemAccount? GetByEmail(string email)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.SystemAccounts.AsNoTracking()
            .FirstOrDefault(a => a.AccountEmail != null && a.AccountEmail.ToLower() == email.Trim().ToLower());
    }

    public SystemAccount? Login(string email, string password)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.SystemAccounts.AsNoTracking()
            .FirstOrDefault(a => a.AccountEmail != null 
                              && a.AccountEmail.ToLower() == email.Trim().ToLower() 
                              && a.AccountPassword == password);
    }

    public short GetNextId()
    {
        using var ctx = new FUNewsManagementContext();
        var maxId = ctx.SystemAccounts.Max(a => (short?)a.AccountId) ?? 0;
        return (short)(maxId + 1);
    }

    public void Add(SystemAccount account)
    {
        using var ctx = new FUNewsManagementContext();
        if (account.AccountId <= 0)
        {
            account.AccountId = GetNextId();
        }
        ctx.SystemAccounts.Add(account);
        ctx.SaveChanges();
    }

    public void Update(SystemAccount account)
    {
        using var ctx = new FUNewsManagementContext();
        var existing = ctx.SystemAccounts.Find(account.AccountId);
        if (existing != null)
        {
            existing.AccountName = account.AccountName;
            existing.AccountEmail = account.AccountEmail;
            if (account.AccountRole.HasValue)
            {
                existing.AccountRole = account.AccountRole;
            }
            if (!string.IsNullOrEmpty(account.AccountPassword))
            {
                existing.AccountPassword = account.AccountPassword;
            }
            ctx.SaveChanges();
        }
    }

    public bool HasCreatedNews(short accountId)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.NewsArticles.Any(n => n.CreatedById == accountId || n.UpdatedById == accountId);
    }

    public void Delete(short id)
    {
        using var ctx = new FUNewsManagementContext();
        var account = ctx.SystemAccounts.Find(id);
        if (account != null)
        {
            ctx.SystemAccounts.Remove(account);
            ctx.SaveChanges();
        }
    }
}
