using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class CategoryDAO
{
    private static CategoryDAO? _instance;
    private static readonly object _lock = new();

    private CategoryDAO() { }

    public static CategoryDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new CategoryDAO();
            }
        }
    }

    public List<Category> GetAll()
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.Categories
            .Include(c => c.ParentCategory)
            .AsNoTracking()
            .ToList();
    }

    public List<Category> GetActive()
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.Categories
            .Where(c => c.IsActive == true)
            .Include(c => c.ParentCategory)
            .AsNoTracking()
            .ToList();
    }

    public Category? GetById(short id)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.Categories
            .Include(c => c.ParentCategory)
            .AsNoTracking()
            .FirstOrDefault(c => c.CategoryId == id);
    }

    public void Add(Category category)
    {
        using var ctx = new FUNewsManagementContext();
        // CategoryId is IDENTITY in database
        category.CategoryId = 0;
        ctx.Categories.Add(category);
        ctx.SaveChanges();
    }

    public void Update(Category category)
    {
        using var ctx = new FUNewsManagementContext();
        var existing = ctx.Categories.Find(category.CategoryId);
        if (existing != null)
        {
            existing.CategoryName = category.CategoryName;
            existing.CategoryDesciption = category.CategoryDesciption;
            existing.ParentCategoryId = category.ParentCategoryId;
            existing.IsActive = category.IsActive;
            ctx.SaveChanges();
        }
    }

    public bool IsUsedByNews(short id)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.NewsArticles.Any(n => n.CategoryId == id);
    }

    public bool HasChildren(short id)
    {
        using var ctx = new FUNewsManagementContext();
        // In sample data, root categories have ParentCategoryID = CategoryID. Exclude itself!
        return ctx.Categories.Any(c => c.ParentCategoryId == id && c.CategoryId != id);
    }

    public void Delete(short id)
    {
        using var ctx = new FUNewsManagementContext();
        ctx.Categories.Where(c => c.CategoryId == id).ExecuteDelete();
    }
}
