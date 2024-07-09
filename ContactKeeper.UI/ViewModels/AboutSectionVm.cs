using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ContactKeeper.UI.ViewModels;

internal partial class AboutSectionVm : ObservableObject
{
    public static string ApplicationDescription => "ContactKeeper is a contact management application for Windows Desktop, " +
        "designed to help users store and manage their contacts efficiently.";

    public static string Version => "1.0.0";

    public static string Developer => "Thomas Wening";

    public static string Contact => "thomaswening94@gmail.com";

    public static string GithubLink => "https://github.com/thomaswening/ContactKeeper";

    public static string License => "The ContactKeeper project is licensed under the GNU General Public License (GPL) version 3.";

    public static string AcknowledgedDependencies => string.Join(", ", acknowledgedDependencies);

    private static IEnumerable<string> acknowledgedDependencies =>
        [
            "CommunityToolkit.Mvvm",
            "MaterialDesignThemes",
            "Serilog",
            "AutoBogus",
            "NSubstitute",
            "NUnit",
            "NUnit3TestAdapter"
        ];
}
