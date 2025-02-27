﻿#region Copyright (c) Pixeval/Pixeval.CoreApi

// GPL v3 License
// 
// Pixeval/Pixeval.CoreApi
// Copyright (c) 2021 Pixeval.CoreApi/IEngineHandle.cs
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

using JetBrains.Annotations;

namespace Pixeval.CoreApi.Engine;

/// <summary>
///     Represents a class that is capable of tracking its own lifetime, any class that
///     implements <see cref="IEngineHandleSource" /> must exposes an <see cref="EngineHandle" />
///     that can be used to cancel itself or report the completion
/// </summary>
[PublicAPI]
public interface IEngineHandleSource
{
    EngineHandle EngineHandle { get; }
}