using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    public List<SystemAccount> GetAll() => SystemAccountDAO.Instance.GetAll();
    public SystemAccount? GetById(short id) => SystemAccountDAO.Instance.GetById(id);
    public SystemAccount? GetByEmail(string email) => SystemAccountDAO.Instance.GetByEmail(email);
    public SystemAccount? Login(string email, string password) => SystemAccountDAO.Instance.Login(email, password);
    public void Add(SystemAccount account) => SystemAccountDAO.Instance.Add(account);
    public void Update(SystemAccount account) => SystemAccountDAO.Instance.Update(account);
    public bool CanDelete(short id) => !SystemAccountDAO.Instance.HasCreatedNews(id);
    public void Delete(short id) => SystemAccountDAO.Instance.Delete(id);
}
