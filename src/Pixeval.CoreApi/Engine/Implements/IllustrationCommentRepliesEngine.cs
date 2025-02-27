﻿#region Copyright (c) Pixeval/Pixeval.CoreApi

// GPL v3 License
// 
// Pixeval/Pixeval.CoreApi
// Copyright (c) 2021 Pixeval.CoreApi/IllustrationCommentRepliesEngine.cs
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using System.Threading;
using Pixeval.CoreApi.Model;
using Pixeval.CoreApi.Net;

namespace Pixeval.CoreApi.Engine.Implements;

public class IllustrationCommentRepliesEngine : AbstractPixivFetchEngine<Comment>
{
    private readonly string _commentId;

    public IllustrationCommentRepliesEngine(string commentId, MakoClient makoClient, EngineHandle? engineHandle) : base(makoClient, engineHandle)
    {
        _commentId = commentId;
    }

    public override IAsyncEnumerator<Comment> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    {
        return RecursivePixivAsyncEnumerators.Comment<IllustrationCommentRepliesEngine>.WithInitialUrl(this, MakoApiKind.AppApi,
            engine => $"/v2/illust/comment/replies?comment_id={engine._commentId}")!;
    }
}