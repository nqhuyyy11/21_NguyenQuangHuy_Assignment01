using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface ITagRepository
{
    List<Tag> GetAll();
    Tag? GetById(int id);
    List<Tag> GetByIds(List<int> ids);
    void Add(Tag tag);
    void Update(Tag tag);
}
