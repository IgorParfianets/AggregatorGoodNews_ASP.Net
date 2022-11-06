﻿using AspNetArticle.Core;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace AspNetArticle.Business.Services
{
    public class CommentaryService : ICommentaryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentaryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<CommentDto> GetCommentByIdAsync(Guid id)
        {
            return _mapper.Map<CommentDto>(await _unitOfWork.Comments.GetByIdAsync(id));
        }

        public async Task<int> CreateCommentAsync(CommentDto dto)
        {
            var entity = _mapper.Map<Comment>(dto);

            if (entity == null) 
                throw new ArgumentException(nameof(entity));

            await _unitOfWork.Comments.AddAsync(entity);
            var result = await _unitOfWork.Commit();
            return result;
        }

        public async Task<int> UpdateCommentAsync(CommentDto dto)
        {
            var entity = _mapper.Map<Comment>(dto);

            if (entity != null)
                _unitOfWork.Comments.Update(entity);
            
            return await _unitOfWork.Commit();
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsByUserIdAsync(Guid id)
        {
            var userAllComments = await _unitOfWork.Comments
                .Get()
                .Where(user => 
                    user.UserId.Equals(id))
                .Select(cmt => 
                    _mapper.Map<CommentDto>(cmt))
                .ToListAsync();

            return userAllComments;
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsByArticleIdAsync(Guid id)
        {
            var articleAllComments = await _unitOfWork.Comments
                .Get()
                .Where(article =>
                    article.UserId.Equals(id))
                .Select(cmt =>
                    _mapper.Map<CommentDto>(cmt))
                .ToListAsync();

            return articleAllComments;
        }

        public async Task DeleteCommentById(Guid id)
        {
            var entity = await _unitOfWork.Comments.GetByIdAsync(id);

            if(entity != null)
                _unitOfWork.Comments.Remove(entity);

            await _unitOfWork.Commit();
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByUserIdAndArticleName(Guid? article, Guid? user)
        {
            var entities = _unitOfWork.Comments.Get();

            if (article != null && !Guid.Empty.Equals(article))
            {
                entities = entities.Where(ent => ent.ArticleId.Equals(article));
            }

            if (user != null && !Guid.Empty.Equals(user))
            {
                entities = entities.Where(ent => ent.UserId.Equals(user));
            }

            var result = (await entities.ToListAsync())
                .Select(ent => _mapper.Map<CommentDto>(ent))
                .ToList();

            return result;
        }
    }
}