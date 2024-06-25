using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactKeeper.UI.ViewModels;

using NUnit.Framework;

namespace ContactKeeper.UI.Tests.ViewModels;

[TestFixture]
internal class DialogVmTests
{
    [Test]
    public void OkCommand_WhenExecutedWithOkAction_InvokesOkActionAndSetsDialogResultToTrueAndCloseDialog()
    {
        // Arrange
        var isCloseRequestedInvoked = false;
        var isCustomActionInvoked = false;
        var dialogVm = new DialogVm("Test", okAction: () => isCustomActionInvoked = true);
        dialogVm.CloseRequested += (sender, args) => isCloseRequestedInvoked = true;

        // Act
        dialogVm.OkCommand.Execute(null);

        // Assert
        Assert.That(isCustomActionInvoked, Is.True);
        Assert.That(dialogVm.DialogResult, Is.True);
        Assert.That(isCloseRequestedInvoked, Is.True);
    }

    [Test]
    public void OkCommand_WhenExecutedWithoutOkAction_SetsDialogResultToTrueAndClosesDialog()
    {
        // Arrange
        var isCloseRequestedInvoked = false;
        var dialogVm = new DialogVm("Test");
        dialogVm.CloseRequested += (sender, args) => isCloseRequestedInvoked = true;

        // Act
        dialogVm.OkCommand.Execute(null);

        // Assert
        Assert.That(dialogVm.DialogResult, Is.True);
        Assert.That(isCloseRequestedInvoked, Is.True);
    }

    [Test]
    public void CancelCommand_WhenExecutedWithCancelAction_InvokesCancelActionAndSetsDialogResultToFalseAndClosesDialog()
    {
        // Arrange
        var isCustomActionInvoked = false;
        var isCloseRequestedInvoked = false;
        var dialogVm = new DialogVm("Test", cancelAction: () => isCustomActionInvoked = true);
        dialogVm.CloseRequested += (sender, args) => isCloseRequestedInvoked = true;

        // Act
        dialogVm.CancelCommand.Execute(null);

        // Assert
        Assert.That(isCustomActionInvoked, Is.True);
        Assert.That(dialogVm.DialogResult, Is.False);
        Assert.That(isCloseRequestedInvoked, Is.True);
    }

    [Test]
    public void CancelCommand_WhenExecutedWithoutCancelAction_SetsDialogResultToFalseAndClosesDialog()
    {
        // Arrange
        var isCloseRequestedInvoked = false;
        var dialogVm = new DialogVm("Test");
        dialogVm.CloseRequested += (sender, args) => isCloseRequestedInvoked = true;

        // Act
        dialogVm.CancelCommand.Execute(null);

        // Assert
        Assert.That(dialogVm.DialogResult, Is.False);
        Assert.That(isCloseRequestedInvoked, Is.True);
    }
}
