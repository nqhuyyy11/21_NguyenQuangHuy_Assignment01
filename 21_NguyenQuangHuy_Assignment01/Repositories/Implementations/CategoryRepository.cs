using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    public List<Category> GetAll() => CategoryDAO.Instance.GetAll();
    public List<Category> GetActive() => CategoryDAO.Instance.GetActive();
    public Category? GetById(short id) => CategoryDAO.Instance.GetById(id);
    public void Add(Category category) => CategoryDAO.Instance.Add(category);
    public void Update(Category category) => CategoryDAO.Instance.Update(category);
    public bool CanDelete(short id) => !CategoryDAO.Instance.IsUsedByNews(id) && !CategoryDAO.Instance.HasChildren(id);
    public void Delete(short id) => CategoryDAO.Instance.Delete(id);
}
