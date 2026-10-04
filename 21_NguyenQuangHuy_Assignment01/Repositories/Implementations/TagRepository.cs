using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class TagRepository : ITagRepository
{
    public List<Tag> GetAll() => TagDAO.Instance.GetAll();
    public Tag? GetById(int id) => TagDAO.Instance.GetById(id);
    public List<Tag> GetByIds(List<int> ids) => TagDAO.Instance.GetByIds(ids);
    public void Add(Tag tag) => TagDAO.Instance.Add(tag);
    public void Update(Tag tag) => TagDAO.Instance.Update(tag);
}
