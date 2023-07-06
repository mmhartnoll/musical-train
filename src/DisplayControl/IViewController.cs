using DisplayControl.ViewModels;
using DomainModel.Entities;
using System;

namespace DisplayControl
{
    internal interface IViewController
    {
        void CloseViewModel(ViewModel viewModel);

        void DisplayEditProfileViewModel(DisplayProfile displayProfile, Action onSuccess);

        void DisplayWaitToSwitchProfileViewModel(DisplayProfile displayProfile, Action onSuccess);
    }
}