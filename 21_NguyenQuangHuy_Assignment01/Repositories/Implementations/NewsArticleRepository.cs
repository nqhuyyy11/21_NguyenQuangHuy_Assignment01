using System;
using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class NewsArticleRepository : INewsArticleRepository
{
    public List<NewsArticle> GetAll() => NewsArticleDAO.Instance.GetAll();
    public List<NewsArticle> GetActive() => NewsArticleDAO.Instance.GetActive();
    public NewsArticle? GetById(string id) => NewsArticleDAO.Instance.GetById(id);
    public List<NewsArticle> GetByCreator(short creatorId) => NewsArticleDAO.Instance.GetByCreator(creatorId);
    public List<NewsArticle> GetByDateRange(DateTime start, DateTime end) => NewsArticleDAO.Instance.GetByDateRange(start, end);
    public void Add(NewsArticle news, List<int>? tagIds = null) => NewsArticleDAO.Instance.Add(news, tagIds);
    public void Update(NewsArticle news, List<int>? tagIds = null) => NewsArticleDAO.Instance.Update(news, tagIds);
    public void Delete(string id) => NewsArticleDAO.Instance.Delete(id);
}
