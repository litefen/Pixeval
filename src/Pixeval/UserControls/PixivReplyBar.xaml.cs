﻿#region Copyright (c) Pixeval/Pixeval

// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2021 Pixeval/PixivReplyBar.xaml.cs
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
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Pixeval.Misc;
using Pixeval.Pages.IllustrationViewer;
using Pixeval.Util.UI;
using Pixeval.Utilities;

namespace Pixeval.UserControls;

public sealed partial class PixivReplyBar
{
    private EventHandler<SendButtonTappedEventArgs>? _sendButtonTapped;

    private EventHandler<StickerTappedEventArgs>? _stickerTapped;

    public PixivReplyBar()
    {
        InitializeComponent();
        _stickerTapped += (_, _) => EmojiButton.Flyout.Hide();
    }

    public event EventHandler<SendButtonTappedEventArgs> SendButtonTapped
    {
        add => _sendButtonTapped += value;
        remove => _sendButtonTapped -= value;
    }

    public event EventHandler<StickerTappedEventArgs> StickerTapped
    {
        add => _stickerTapped += value;
        remove => _stickerTapped -= value;
    }

    private void PixivReplyBar_OnLoaded(object sender, RoutedEventArgs e)
    {
        EmojiButtonFlyoutEmojiSectionNavigationViewItem.Tag = new NavigationViewTag(typeof(PixivReplyEmojiListPage), this);
        EmojiButtonFlyoutStickersSectionNavigationViewItem.Tag = new NavigationViewTag(typeof(PixivReplyStickerListPage), (Guid.NewGuid(), _stickerTapped) /* just to support the serialization, see https://docs.microsoft.com/en-us/windows/winui/api/microsoft.ui.xaml.controls.frame.navigate?view=winui-3.0#Microsoft_UI_Xaml_Controls_Frame_Navigate_Windows_UI_Xaml_Interop_TypeName_System_Object_Microsoft_UI_Xaml_Media_Animation_NavigationTransitionInfo_ */);
    }

    private void EmojiButtonFlyoutNavigationView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        EmojiButtonFlyoutFrame.NavigateByNavigationViewTag(sender, new SlideNavigationTransitionInfo
        {
            Effect = args.SelectedItemContainer == EmojiButtonFlyoutEmojiSectionNavigationViewItem
                ? SlideNavigationTransitionEffect.FromLeft
                : SlideNavigationTransitionEffect.FromRight
        });
    }

    private void SendButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        ReplyContentRichEditBox.Document.GetText(TextGetOptions.UseObjectText, out var content);
        if (content.Length is 0 or > 140)
        {
            MessageDialogBuilder.CreateAcknowledgement(this,
                PixivReplyBarResources.CommentIsTooShortOrTooLongToastTitle,
                PixivReplyBarResources.CommentIsTooShortOrTooLongToastContentFormatted.Format(content.Length));
            return;
        }

        _sendButtonTapped?.Invoke(this, new SendButtonTappedEventArgs(e, content));
        ReplyContentRichEditBox.ClearContent();
    }

    private void ReplyContentRichEditBox_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        // disable context menu
        e.Handled = true;
    }
}

public class SendButtonTappedEventArgs : EventArgs
{
    public SendButtonTappedEventArgs(TappedRoutedEventArgs tappedEventArgs, string replyContentRichEditBoxStringContent)
    {
        TappedEventArgs = tappedEventArgs;
        ReplyContentRichEditBoxStringContent = replyContentRichEditBoxStringContent;
    }

    public TappedRoutedEventArgs TappedEventArgs { get; }

    public string ReplyContentRichEditBoxStringContent { get; }
}

public class StickerTappedEventArgs : EventArgs
{
    public StickerTappedEventArgs(TappedRoutedEventArgs tappedEventArgs, PixivReplyStickerViewModel stickerViewModel)
    {
        TappedEventArgs = tappedEventArgs;
        StickerViewModel = stickerViewModel;
    }

    public TappedRoutedEventArgs TappedEventArgs { get; }

    public PixivReplyStickerViewModel StickerViewModel { get; }
}