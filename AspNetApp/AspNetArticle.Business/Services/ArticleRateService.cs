﻿using AspNetArticle.Data.Abstractions;
using AutoMapper;
using System.Net.Http.Json;
using AspNetSample.Business.Models;
using AspNetSample.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using AspNetArticle.Core.Abstractions;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
//using System.Text.Json.Nodes;
//using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace AspNetArticle.Business.Services
{
    public class ArticleRateService : IArticleRateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ArticleRateService(IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }


        public async Task AddRateToArticlesAsync()
        {
            var articlesWithEmptyRateIds = _unitOfWork.Articles.Get()
                .Where(article => article.Rate == null && !string.IsNullOrEmpty(article.Text))
                .Select(article => article.Id)
                .ToList();

            foreach (var articleId in articlesWithEmptyRateIds)
            {
                string articleFixedText = await RemoveHtmlTagsFromArticleTestAsync(articleId);
                await RateArticleAsync(articleId, articleFixedText);

            }
        }

        private async Task<string> RemoveHtmlTagsFromArticleTestAsync(Guid articleId)
        {
            var text = (await _unitOfWork.Articles.GetByIdAsync(articleId))?.Text;

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"Text with id: {articleId} doesn't exists",
                    nameof(articleId));
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);

            var textWithoutHtml = htmlDoc.DocumentNode
                .SelectNodes("p")
                .Select(t => t.InnerText)
                .Aggregate((i, j) => i + " " + j);

            textWithoutHtml = Regex.Replace(textWithoutHtml, @"<[^>]+>|&nbsp|\n;", " ")
                .Trim()
                .ToLower();
            
            return textWithoutHtml;
        }

        private async Task RateArticleAsync(Guid articleId, string articleFixedText)
        {
            try
            {
                var article = await _unitOfWork.Articles.GetByIdAsync(articleId);

                if (article == null)
                {
                    throw new ArgumentException($"Article with id: {articleId} doesn't exists",
                        nameof(articleId));
                }

                using (var client = new HttpClient())
                {
                    var isprasUrl = _configuration["IsprasUrl"];
                    var affinPath = _configuration["AffinPath"];

                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(@isprasUrl));

                    httpRequest.Headers.Add("Accept", "application/json");
                    httpRequest.Content = JsonContent.Create(new[] { new TextRequestModel() { Text = articleFixedText } });

                    var response = await client.SendAsync(httpRequest);
                    var responseStr = await response.Content.ReadAsStreamAsync();

                    using (var sr = new StreamReader(responseStr))
                    {
                        var data = await sr.ReadToEndAsync();
                        
                        var isprassResponce = JsonConvert.DeserializeObject<IsprassResponseObject[]>(data);

                        var affinJsonText = await File.ReadAllTextAsync(@affinPath);


                        var affinJsonObject = Affin.FromJson(affinJsonText);
                        
                        if (isprassResponce != null && affinJsonObject.Any())
                        {
                            double overallRate = 0 , resultRate = 0;
                            int numberRecognizedWords = 0;

                            foreach (var lem in isprassResponce[0].Annotations.Lemma) 
                            { 
                                long? temp = 0;
                                affinJsonObject.TryGetValue(lem.Value, out temp);

                                if (temp != null)
                                {
                                    overallRate += (double)temp;
                                    numberRecognizedWords++;
                                }
                                    
                            }
                            //var countWords = isprassResponce[0].Annotations.Lemma.Count;
                            if(numberRecognizedWords > 0)
                                resultRate = overallRate / numberRecognizedWords;

                            await _unitOfWork.Articles.UpdateArticleRateAsync(articleId, resultRate);
                            await _unitOfWork.Commit();

                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
