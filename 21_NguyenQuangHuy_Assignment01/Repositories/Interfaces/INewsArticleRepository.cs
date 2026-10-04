using System;
using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories.Interfaces;

public interface INewsArticleRepository
{
    List<NewsArticle> GetAll();
    List<NewsArticle> GetActive();
    NewsArticle? GetById(string id);
    List<NewsArticle> GetByCreator(short creatorId);
    List<NewsArticle> GetByDateRange(DateTime start, DateTime end);
    void Add(NewsArticle news, List<int>? tagIds = null);
    void Update(NewsArticle news, List<int>? tagIds = null);
    void Delete(string id);
}
