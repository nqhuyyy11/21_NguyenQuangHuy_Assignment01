using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class TagDAO
{
    private static TagDAO? _instance;
    private static readonly object _lock = new();

    private TagDAO() { }

    public static TagDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new TagDAO();
            }
        }
    }

    public List<Tag> GetAll()
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.Tags.AsNoTracking().ToList();
    }

    public Tag? GetById(int id)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.Tags.AsNoTracking().FirstOrDefault(t => t.TagId == id);
    }

    public List<Tag> GetByIds(List<int> ids)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.Tags.Where(t => ids.Contains(t.TagId)).AsNoTracking().ToList();
    }

    public int GetNextId()
    {
        using var ctx = new FUNewsManagementContext();
        var maxId = ctx.Tags.Max(t => (int?)t.TagId) ?? 0;
        return maxId + 1;
    }

    public void Add(Tag tag)
    {
        using var ctx = new FUNewsManagementContext();
        if (tag.TagId <= 0)
        {
            tag.TagId = GetNextId();
        }
        ctx.Tags.Add(tag);
        ctx.SaveChanges();
    }

    public void Update(Tag tag)
    {
        using var ctx = new FUNewsManagementContext();
        var existing = ctx.Tags.Find(tag.TagId);
        if (existing != null)
        {
            existing.TagName = tag.TagName;
            existing.Note = tag.Note;
            ctx.SaveChanges();
        }
    }
}
