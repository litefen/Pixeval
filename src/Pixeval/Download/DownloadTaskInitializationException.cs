﻿#region Copyright (c) Pixeval/Pixeval

// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2021 Pixeval/DownloadTaskInitializationException.cs
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

using System;
using System.Runtime.Serialization;

namespace Pixeval.Download;

public class DownloadTaskInitializationException : Exception
{
    public DownloadTaskInitializationException()
    {
    }

    protected DownloadTaskInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DownloadTaskInitializationException(string? message) : base(message)
    {
    }

    public DownloadTaskInitializationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}