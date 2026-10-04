using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class NewsArticleDAO
{
    private static NewsArticleDAO? _instance;
    private static readonly object _lock = new();

    private NewsArticleDAO() { }

    public static NewsArticleDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new NewsArticleDAO();
            }
        }
    }

    public List<NewsArticle> GetAll()
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public List<NewsArticle> GetActive()
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.NewsArticles
            .Where(n => n.NewsStatus == true)
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public NewsArticle? GetById(string id)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .AsNoTracking()
            .FirstOrDefault(n => n.NewsArticleId == id);
    }

    public List<NewsArticle> GetByCreator(short creatorId)
    {
        using var ctx = new FUNewsManagementContext();
        return ctx.NewsArticles
            .Where(n => n.CreatedById == creatorId)
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public List<NewsArticle> GetByDateRange(DateTime start, DateTime end)
    {
        using var ctx = new FUNewsManagementContext();
        var startDate = start.Date;
        var endDateExclusive = end.Date.AddDays(1);
        return ctx.NewsArticles
            .Where(n => n.CreatedDate >= startDate && n.CreatedDate < endDateExclusive)
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public string GetNextId()
    {
        using var ctx = new FUNewsManagementContext();
        var maxId = ctx.NewsArticles
            .Select(n => n.NewsArticleId)
            .AsEnumerable()
            .Select(s => int.TryParse(s, out var v) ? v : 0)
            .DefaultIfEmpty(0)
            .Max();
        return (maxId + 1).ToString();
    }

    public void Add(NewsArticle news, List<int>? tagIds = null)
    {
        using var ctx = new FUNewsManagementContext();
        if (string.IsNullOrWhiteSpace(news.NewsArticleId))
        {
            news.NewsArticleId = GetNextId();
        }
        if (tagIds != null && tagIds.Count > 0)
        {
            news.Tags = ctx.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
        }
        ctx.NewsArticles.Add(news);
        ctx.SaveChanges();
    }

    public void Update(NewsArticle news, List<int>? tagIds = null)
    {
        using var ctx = new FUNewsManagementContext();
        var existing = ctx.NewsArticles
            .Include(n => n.Tags)
            .FirstOrDefault(n => n.NewsArticleId == news.NewsArticleId);

        if (existing != null)
        {
            existing.NewsTitle = news.NewsTitle;
            existing.Headline = news.Headline;
            existing.NewsContent = news.NewsContent;
            existing.NewsSource = news.NewsSource;
            existing.CategoryId = news.CategoryId;
            existing.NewsStatus = news.NewsStatus;
            existing.UpdatedById = news.UpdatedById;
            existing.ModifiedDate = news.ModifiedDate;

            // Update Tags
            existing.Tags.Clear();
            if (tagIds != null && tagIds.Count > 0)
            {
                var tagsToAssign = ctx.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                foreach (var tag in tagsToAssign)
                {
                    existing.Tags.Add(tag);
                }
            }
            ctx.SaveChanges();
        }
    }

    public void Delete(string id)
    {
        using var ctx = new FUNewsManagementContext();
        var existing = ctx.NewsArticles
            .Include(n => n.Tags)
            .FirstOrDefault(n => n.NewsArticleId == id);

        if (existing != null)
        {
            existing.Tags.Clear();
            ctx.NewsArticles.Remove(existing);
            ctx.SaveChanges();
        }
    }
}
