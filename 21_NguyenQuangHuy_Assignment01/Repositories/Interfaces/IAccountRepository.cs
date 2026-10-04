using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface IAccountRepository
{
    List<SystemAccount> GetAll();
    SystemAccount? GetById(short id);
    SystemAccount? GetByEmail(string email);
    SystemAccount? Login(string email, string password);
    void Add(SystemAccount account);
    void Update(SystemAccount account);
    bool CanDelete(short id);
    void Delete(short id);
}
