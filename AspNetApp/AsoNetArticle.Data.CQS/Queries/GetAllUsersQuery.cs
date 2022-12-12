﻿using AspNetArticle.Database.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<User>?>
    {
    }
}