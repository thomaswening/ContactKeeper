/// <summary>
/// Auto-generated Version Info Class.
/// </summary>
namespace ContactKeeper.Infrastructure.Utilities;
public static class VersionInfo
{
    /// <summary>
    /// Version of the application, used in assembly version and file version. Format: Major.Minor.Patch
    /// </summary>
    public static string Version => "0.0.0";

    /// <summary>
    /// SHA of the latest commit.
    /// </summary>
    public static string CommitSha => "998a77dd64a6f8662575cbc4fba2d12398b11190";

    /// <summary>
    /// DateTime of the latest commit.
    /// </summary>
    public static string CommitDateTime => "15.08.2024-18:34:47+0200";

    /// <summary>
    /// Name of the current branch.
    /// </summary>
    public static string Branch => "features/7-automatic-version-number-in-about-section";

    /// <summary>
    /// Returns a string of the version info in the following format. 
    /// 'Version: {Version}\nBranch: {Branch}\nCommit: {CommitSha}\nDate: {CommitDateTime}'
    /// </summary>
    public static string AsPrettyString()
    {
        return $"Version: {Version}\nBranch: {Branch}\nCommit: {CommitSha}\nCommit Date: {CommitDateTime}";
    }
}