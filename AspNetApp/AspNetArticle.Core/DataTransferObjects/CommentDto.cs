﻿namespace AspNetArticle.Core.DataTransferObjects;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTime PublicationDate { get; set; }
    public string ArticleName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
    public bool IsEdited { get; set; }
}
