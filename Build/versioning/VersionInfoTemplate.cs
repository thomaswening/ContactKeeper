/// <summary>
/// Auto-generated Version Info Class.
/// </summary>
namespace {namespace};
public static class VersionInfo
{{
    /// <summary>
    /// Version of the application, used in assembly version and file version. Format: Major.Minor.Patch
    /// </summary>
    public static string Version => "{version}";

    /// <summary>
    /// SHA of the latest commit.
    /// </summary>
    public static string CommitSha => "{commit_sha}";

    /// <summary>
    /// DateTime of the latest commit.
    /// </summary>
    public static string CommitDateTime => "{build_date}";

    /// <summary>
    /// Name of the current branch.
    /// </summary>
    public static string Branch => "{branch}";

    /// <summary>
    /// Returns a string of the version info in the following format. 
    /// 'Version: {{Version}}\nBranch: {{Branch}}\nCommit: {{CommitSha}}\nDate: {{CommitDateTime}}'
    /// </summary>
    public static string AsPrettyString()
    {{
        return $"Version: {{Version}}\nBranch: {{Branch}}\nCommit: {{CommitSha}}\nCommit Date: {{CommitDateTime}}";
    }}
}}