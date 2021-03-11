namespace CoronaDeployments.Core
{
    public record IISDeployTargetExtraInfo(string SiteName, int Port) : IDeployTargetExtraInfo
    {
        public static bool Validate(IISDeployTargetExtraInfo i)
        {
            if (i == default) return false;

            if (string.IsNullOrWhiteSpace(i.SiteName)) return false;

            if (i.Port <= 0) return false;

            return true;
        }
    }
}
