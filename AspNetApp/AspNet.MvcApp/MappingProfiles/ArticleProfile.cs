﻿using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AspNetArticle.MvcApp.Models.ArticleModels;
using AutoMapper;

namespace AspNetArticle.MvcApp.MappingProfiles;

public class ArticleProfile : Profile 
{
    public ArticleProfile()
    {
        // For Entity -> Dto & Dto -> Entity
        CreateMap<Article, ArticleDto>()
            .ForMember(dto => dto.Id,
                opt
                    => opt.MapFrom(article => article.Id))
            .ForMember(dto => dto.Title,
                opt
                    => opt.MapFrom(article => article.Title))
            .ForMember(dto => dto.ShortDescription,
                opt
                    => opt.MapFrom(article => article.ShortDescription))
            .ForMember(dto => dto.Text,
                opt
                    => opt.MapFrom(article => article.Text))
            .ForMember(dto => dto.PublicationDate,
                opt
                    => opt.MapFrom(article => article.PublicationDate))
            .ForMember(dto => dto.Comments,
                opt
                    => opt.MapFrom(article => article.Comments))
            .ForMember(dto => dto.ImageUrl,
                opt
                    => opt.MapFrom(article => article.ImageUrl))
            .ForMember(dto => dto.Rate,
            opt
                => opt.MapFrom(article => article.Rate));

        CreateMap<ArticleDto, Article>()
            .ForMember(article => article.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(article => article.Title, opt => opt.MapFrom(dto => dto.Title))
            .ForMember(article => article.ShortDescription, opt => opt.MapFrom(dto => dto.ShortDescription))
            .ForMember(article => article.Text, opt => opt.MapFrom(dto => dto.Text))
            .ForMember(article => article.PublicationDate, opt => opt.MapFrom(dto => dto.PublicationDate))
            .ForMember(article => article.Comments, opt => opt.MapFrom(dto => dto.Comments));

        // For ArticleDto -> ArticleModel 

        CreateMap<ArticleDto, ArticleModel>()
            .ForMember(article => article.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(article => article.Title, opt => opt.MapFrom(dto => dto.Title))
            .ForMember(article => article.ShortDescription, opt => opt.MapFrom(dto => dto.ShortDescription))
            .ForMember(article => article.Text, opt => opt.MapFrom(dto => dto.Text))
            .ForMember(article => article.PublicationDate, opt => opt.MapFrom(dto => dto.PublicationDate));

        CreateMap<ArticleModel,ArticleDto>()
            .ForMember(article => article.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(article => article.Title, opt => opt.MapFrom(dto => dto.Title))
            .ForMember(article => article.ShortDescription, opt => opt.MapFrom(dto => dto.ShortDescription))
            .ForMember(article => article.Text, opt => opt.MapFrom(dto => dto.Text))
            .ForMember(article => article.PublicationDate, opt => opt.MapFrom(dto => DateTime.Now));

    }
}
