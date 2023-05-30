using NetCalc.Resources;

/// <summary>
/// Provides access to string resources.
/// </summary>
namespace NetCalc
{
	public class LocalizedStrings
	{
		private static AppResources _localizedResources = new AppResources();
 
		public AppResources LocalizedResources { get { return _localizedResources; } }
	}
}