﻿using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Test.AvaloniaDemo.Services;
using Kok.Toolkit.Avalonia.Dialogs;
using Kok.Toolkit.Avalonia.Mvvm;
using Kok.Toolkit.Avalonia.Navigation;
using Microsoft.Extensions.Configuration;
using System;

namespace Kok.Test.AvaloniaDemo.ViewModels
{
    public partial class MainWindowViewModel : ViewModel
    {
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogs;
        private readonly IConfiguration _configuration;
        private readonly ITestService _service;

        public MainWindowViewModel(IConfiguration configuration, INavigationService navigation, IDialogService dialogs, ITestService service)
        {
            _service = service;
            _configuration = configuration;
            _navigation = navigation;
            _dialogs = dialogs;
            WinTitle = App.GetStringArg(_configuration, CommandArgType.ConfigFile);
            _service.OnStatusChanged = (val) => IsRunning = val;
            IsRunning = _service.IsRunning;
        }

        [ObservableProperty] private bool _isRunning;

        [ObservableProperty]
        private string _winTitle;

        [ObservableProperty]
        private string greeting = "Welcome to Avalonia!";

        [RelayCommand]
        private async void OpenFirstWin()
        {
            if (await MessageBox.AskAsync("是否要显示弹窗?"))
            {
                _dialogs.Show<Views.FirstView>(DateTime.Now);
            }
        }

        [RelayCommand]
        private void Save() =>
            WindowMessenger.Send(this, new NotificationMessage(this, "内容1111", false));
    }
}
