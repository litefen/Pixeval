﻿#region Copyright (c) Pixeval/Pixeval
// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2021 Pixeval/KnownFolders.cs
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
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Pixeval.Util.IO;

namespace Pixeval.AppManagement;

public class AppKnownFolders
{
    public static AppKnownFolders Local = new(ApplicationData.Current.LocalFolder, _ => ApplicationData.Current.ClearAsync(ApplicationDataLocality.Local).AsTask());

    public static AppKnownFolders LoginProxy = null!;

    public static AppKnownFolders SavedWallPaper = null!;

    public static AppKnownFolders Temporary = new(ApplicationData.Current.TemporaryFolder, _ => ApplicationData.Current.ClearAsync(ApplicationDataLocality.Temporary).AsTask());

    private static async Task<AppKnownFolders> GetOrCreate(AppKnownFolders folder, string subfolderName)
    {
        return new AppKnownFolders(await folder.Self.GetOrCreateFolderAsync(subfolderName));
    }

    public static async Task InitializeAsync()
    {
        LoginProxy = await GetOrCreate(Local, "LoginProxy");
        SavedWallPaper = await GetOrCreate(Local, "Wallpapers");
    }

    public static IAsyncOperation<StorageFile> CreateTemporaryFileWithRandomNameAsync(string? extension = null)
    {
        return Temporary.CreateFileAsync($"{Guid.NewGuid()}.{extension ?? "temp"}");
    } 

    public static IAsyncOperation<StorageFile> CreateTemporaryFileWithNameAsync(string name, string? extension = null)
    {
        return Temporary.CreateFileAsync($"{name}.{extension ?? "temp"}");
    }

    private readonly Func<StorageFolder, Task>? _deleter;

    public StorageFolder Self { get; }

    public AppKnownFolders(StorageFolder self)
    {
        Self = self;
    }

    private AppKnownFolders(StorageFolder self, Func<StorageFolder, Task> deleter) : this(self)
    {
        _deleter = deleter;
    }

    public IAsyncOperation<StorageFile> GetFileAsync(string name)
    {
        return Self.GetFileAsync(name);
    }

    public IAsyncOperation<StorageFolder> GetFolderAsync(string name)
    {
        return Self.GetFolderAsync(name);
    }

    public async Task<StorageFolder?> TryGetFolderRelativeToSelfAsync(string pathWithoutSlash)
    {
        return await Self.TryGetItemAsync(pathWithoutSlash) as StorageFolder;
    }

    public async Task<StorageFile?> TryGetFileRelativeToSelfAsync(string pathWithoutSlash)
    {
        return await Self.TryGetItemAsync(pathWithoutSlash) as StorageFile;
    }

    public IAsyncOperation<StorageFile> CreateFileAsync(string name, CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting)
    {
        return Self.CreateFileAsync(name, collisionOption);
    }

    public IAsyncOperation<StorageFolder> CreateFolderAsync(string name, CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting)
    {
        return Self.CreateFolderAsync(name, collisionOption);
    }

    public Task<StorageFile> GetOrCreateFileAsync(string name)
    {
        return Self.GetOrCreateFileAsync(name);
    }

    public Task<StorageFolder> GetOrCreateFolderAsync(string name)
    {
        return Self.GetOrCreateFolderAsync(name);
    }

    public Task ClearAsync()
    {
        return _deleter is not null ? _deleter(Self) : Self.ClearDirectoryAsync();
    }

    public string Resolve(string path)
    {
        return Path.Combine(Self.Path, path);
    }
}