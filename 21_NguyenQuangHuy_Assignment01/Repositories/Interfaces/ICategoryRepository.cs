using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface ICategoryRepository
{
    List<Category> GetAll();
    List<Category> GetActive();
    Category? GetById(short id);
    void Add(Category category);
    void Update(Category category);
    bool CanDelete(short id);
    void Delete(short id);
}
