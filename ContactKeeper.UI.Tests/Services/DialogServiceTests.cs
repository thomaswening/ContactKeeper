using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoBogus;

using ContactKeeper.UI.Events;
using ContactKeeper.UI.Factories;
using ContactKeeper.UI.Services;
using ContactKeeper.UI.ViewModels;
using ContactKeeper.UI.Views;

using NSubstitute;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.Services;

[TestFixture]
internal class DialogServiceTests
{
    private class FakeModalDialogVm(string message) : ModalDialogVm(message)
    {
        public void SetDialogResult(bool result) => DialogResult = result;
        public void RequestClose() => InvokeCloseRequested();
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private IDialogHost dialogHost;
    private IModalDialogViewFactory viewFactory;
    private DialogService dialogService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [SetUp]
    public void Setup()
    {
        dialogHost = Substitute.For<IDialogHost>();
        viewFactory = Substitute.For<IModalDialogViewFactory>();
        dialogService = new DialogService(dialogHost, viewFactory);
    }

    [Test]
    public void ShowModalDialogWithReturnValueAsync_WhenArgsIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var message = AutoFaker.Generate<string>();
        var viewModel = new ModalDialogVm(message);
        AwaitableEventArgs<bool> args = null!;

        // Act & Assert
        Assert.That(async () => await dialogService.ShowModalDialogWithReturnValueAsync(viewModel, args),
                    Throws.ArgumentNullException);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ShowModalDialogWithReturnValueAsync_WhenDialogIsAcceptedOrDeclined_SetsArgsResultAccordingly(bool dialogResult)
    {
        // Arrange
        var message = AutoFaker.Generate<string>();
        var fakeViewModel = new FakeModalDialogVm(message);
        var args = new AwaitableEventArgs<bool>();

        fakeViewModel.SetDialogResult(dialogResult);

        // Act
        await dialogService.ShowModalDialogWithReturnValueAsync(fakeViewModel, args);
        var result = await args.Task;

        // Assert
        Assert.That(result, Is.EqualTo(dialogResult));
    }

    [Test]
    public void ShowModalDialogAsync_WhenViewModelIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ModalDialogVm viewModel = null!;

        // Act & Assert
        Assert.That(async () => await dialogService.ShowModalDialogAsync(viewModel),
                    Throws.ArgumentNullException);
    }

    [Test]
    public async Task ShowModalDialogAsync_Always_ShowsDialog()
    {
        // Arrange
        var message = AutoFaker.Generate<string>();
        var viewModel = new ModalDialogVm(message);

        // Act
        await dialogService.ShowModalDialogAsync(viewModel);

        // Assert
        await dialogHost.Received().ShowAsync(Arg.Any<IModalDialogView>());
    }

    [Test]
    public async Task ShowModalDialogAsync_WhenDialogIsClosed_ClosesDialog()
    {
        // Arrange
        var message = AutoFaker.Generate<string>();
        var fakeViewModel = new FakeModalDialogVm(message);

        // Act
        await dialogService.ShowModalDialogAsync(fakeViewModel);
        fakeViewModel.RequestClose();

        // Assert
        dialogHost.Received().Close();
    }
}
