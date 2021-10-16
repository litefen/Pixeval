﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.System;
using Microsoft.UI.Xaml.Input;
using Pixeval.Popups;
using Pixeval.Popups.IllustrationResultFilter;
using Pixeval.UserControls.TokenInput;
using Pixeval.Utilities;
using Pixeval.Util;
using Pixeval.Util.Generic;
using Pixeval.Util.UI;
using Pixeval.ViewModel;

namespace Pixeval.UserControls
{
    public sealed partial class IllustrationGridCommandBar
    {
        private readonly IEnumerable<Control> _defaultCommands;

        public IllustrationGridCommandBar()
        {
            InitializeComponent();
            var defaultCommands = new List<ICommandBarElement>(CommandBar.PrimaryCommands);
            defaultCommands.AddRange(CommandBar.SecondaryCommands);
            _defaultCommands = defaultCommands.Where(e => e is AppBarButton).Cast<Control>();
            CommandBarElements = new ObservableCollection<UIElement>();
            CommandBarElements.CollectionChanged += (_, args) =>
            {
                switch (args)
                {
                    case { Action: NotifyCollectionChangedAction.Add }:
                        if (args is { NewItems: not null })
                        {
                            foreach (UIElement argsNewItem in args.NewItems)
                            {
                                ExtraCommandsBar.Children.Add(argsNewItem);
                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        public static DependencyProperty PrimaryCommandSupplementsProperty = DependencyProperty.Register(
            nameof(PrimaryCommandsSupplements),
            typeof(ObservableCollection<ICommandBarElement>),
            typeof(IllustrationGridCommandBar),
            PropertyMetadata.Create(
                new ObservableCollection<ICommandBarElement>(),
                (o, args) => AddCommandCallback(args, ((IllustrationGridCommandBar) o).CommandBar.PrimaryCommands)));

        public static DependencyProperty SecondaryCommandsSupplementsProperty = DependencyProperty.Register(
            nameof(SecondaryCommandsSupplements),
            typeof(ObservableCollection<ICommandBarElement>),
            typeof(IllustrationGridCommandBar),
            PropertyMetadata.Create(
                new ObservableCollection<ICommandBarElement>(),
                (o, args) => AddCommandCallback(args, ((IllustrationGridCommandBar) o).CommandBar.SecondaryCommands)));

        public static DependencyProperty IsDefaultCommandsEnabledProperty = DependencyProperty.Register(
            nameof(IsDefaultCommandsEnabled),
            typeof(bool),
            typeof(IllustrationGridCommandBar),
            PropertyMetadata.Create(true,
                (o, args) => ((IllustrationGridCommandBar) o)._defaultCommands.Where(c => c != ((IllustrationGridCommandBar) o).SelectAllButton).ForEach(c => c.IsEnabled = (bool) args.NewValue)));

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(IllustrationGridViewModel),
            typeof(IllustrationGridCommandBar),
            PropertyMetadata.Create(new IllustrationGridViewModel()));

        public ObservableCollection<ICommandBarElement> PrimaryCommandsSupplements
        {
            get => (ObservableCollection<ICommandBarElement>) GetValue(PrimaryCommandSupplementsProperty);
            set => SetValue(PrimaryCommandSupplementsProperty, value);
        }

        public ObservableCollection<ICommandBarElement> SecondaryCommandsSupplements
        {
            get => (ObservableCollection<ICommandBarElement>) GetValue(SecondaryCommandsSupplementsProperty);
            set => SetValue(SecondaryCommandsSupplementsProperty, value);
        }

        public bool IsDefaultCommandsEnabled
        {
            get => (bool) GetValue(IsDefaultCommandsEnabledProperty);
            set => SetValue(IsDefaultCommandsEnabledProperty, value);
        }

        public ObservableCollection<UIElement> CommandBarElements { get; }

        public IllustrationGridViewModel ViewModel
        {
            get => (IllustrationGridViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private static void AddCommandCallback(DependencyPropertyChangedEventArgs e, ICollection<ICommandBarElement> commands)
        {
            if (e.NewValue is ObservableCollection<ICommandBarElement> collection)
            {
                collection.CollectionChanged += (_, args) =>
                {
                    switch (args)
                    {
                        case { Action: NotifyCollectionChangedAction.Add }:
                            foreach (var argsNewItem in args.NewItems ?? Array.Empty<ICommandBarElement>())
                            {
                                commands.Add((ICommandBarElement) argsNewItem);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(e), @"This collection does not support operations except the Add");
                    }
                };
            }
        }

        private void SelectAllToggleButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IllustrationsView.OfType<IllustrationViewModel>().ForEach(v => v.IsSelected = true);
        }

        private async void AddAllToBookmarkButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            // TODO
            var notBookmarked = ViewModel.SelectedIllustrations.Where(i => !i.IsBookmarked);
            var viewModelSelectedIllustrations = notBookmarked as IllustrationViewModel[] ?? notBookmarked.ToArray();
            if (viewModelSelectedIllustrations.Length > 5 && await MessageDialogBuilder.CreateOkCancel(
                    this,
                    IllustrationGridCommandBarResources.SelectedTooManyItemsForBookmarkTitle,
                    IllustrationGridCommandBarResources.SelectedTooManyItemsForBookmarkContent)
                .ShowAsync() != ContentDialogResult.Primary)
            {
                return;
            }

            foreach (var viewModelSelectedIllustration in viewModelSelectedIllustrations)
            {
                _ = viewModelSelectedIllustration.PostPublicBookmarkAsync(); // discard the result
            }

            if (viewModelSelectedIllustrations.Length is var c and > 0)
            {
                UIHelper.ShowTextToastNotification(
                    IllustrationGridCommandBarResources.AddAllToBookmarkToastTitle,
                    IllustrationGridCommandBarResources.AddAllToBookmarkToastContentFormatted.Format(c),
                    AppContext.AppLogoNoCaptionUri);
            }
        }

        private void SaveAllButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OpenAllInBrowserButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.SelectedIllustrations is { Count: var count } selected)
            {
                if (count > 15 && await MessageDialogBuilder.CreateOkCancel(
                        this,
                        IllustrationGridCommandBarResources.SelectedTooManyItemsTitle,
                        IllustrationGridCommandBarResources.SelectedTooManyItemsForOpenInBrowserContent)
                    .ShowAsync() != ContentDialogResult.Primary)
                {
                    return;
                }

                foreach (var illustrationViewModel in selected)
                {
                    await Launcher.LaunchUriAsync(MakoHelper.GetIllustrationWebUri(illustrationViewModel.Id));
                }
            }
        }

        private void ShareButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            // TODO
            throw new NotImplementedException();
        }

        private void CancelSelectionButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.Illustrations.ForEach(v => v.IsSelected = false);
        }

        private readonly IllustrationResultFilterPopupViewModel _filterPopupViewModel = new();

        private void OpenConditionDialogButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var content = new IllustrationResultFilterPopupContent(_filterPopupViewModel);
            content.ResetButtonTapped += (_, _) => ViewModel.IllustrationsView.Filter = null;
            var popup = PopupManager.CreatePopup(content, 550, heightMargin: 100, lightDismiss: false, closing: ConditionPopupClosing);
            content.CloseButtonTapped += (_, _) =>
            {
                content.Cleanup();
                PopupManager.ClosePopup(popup);
            };
            PopupManager.ShowPopup(popup);
        }

        private void ConditionPopupClosing(IAppPopupContent popup, object? arg)
        {
            OpenConditionDialogButton.IsChecked = false;
            if (arg is FilterSettings(
                    var includeTags,
                    var excludeTags,
                    var leastBookmark,
                    var maximumBookmark,
                    _, // TODO user group name
                    var illustratorName,
                    var illustratorId,
                    var illustrationName,
                    var illustrationId,
                    var publishDateStart,
                    var publishDateEnd)
                && popup is IllustrationResultFilterPopupContent { IsReset: false })
            {
                ViewModel.IllustrationsView.Filter = null;
                ViewModel.IllustrationsView.Filter += o =>
                {
                    if (o is IllustrationViewModel vm)
                    {
                        var stringTags = vm.Illustration.Tags?.Select(t => t.Name).WhereNotNull().ToArray() ?? Array.Empty<string>();
                        var result = ExamineExcludeTags(stringTags, excludeTags)
                                     && ExamineIncludeTags(stringTags, includeTags)
                                     && vm.Bookmark >= leastBookmark
                                     && vm.Bookmark <= maximumBookmark
                                     && illustrationName.Match(vm.Illustration.Title)
                                     && illustratorName.Match(vm.Illustration.User?.Name)
                                     && (illustratorId.IsNullOrEmpty() || illustratorId == vm.Illustration.User?.Id.ToString())
                                     && (illustrationId.IsNullOrEmpty() || illustrationId == vm.Id)
                                     && vm.PublishDate >= publishDateStart
                                     && vm.PublishDate <= publishDateEnd;
                        return result;
                    }

                    return false;
                };
            } 
            else if (popup is IllustrationResultFilterPopupContent { IsReset: true })
            {
                ViewModel.IllustrationsView.ResetView();
            }

            static bool ExamineExcludeTags(IEnumerable<string> tags, IEnumerable<Token> predicates)
            {
                return predicates.Aggregate(true, (acc, token) => acc && tags.None(token.Match));
            }

            static bool ExamineIncludeTags(IEnumerable<string> tags, IEnumerable<Token> predicates)
            {
                var tArr = tags.ToArray();
                return !tArr.Any() || predicates.Aggregate(true, (acc, token) => acc && tArr.Any(token.Match));
            }
        }
    }
}