﻿using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateArticleTextDevIoCommand : IRequest
    {
        public Guid ArticleId { get; set; }
        public string Text { get; set; }
    }
}
