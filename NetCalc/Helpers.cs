using System;
using Microsoft.Phone.Marketplace;

namespace NetCalc.Helpers
{
	
	public static class LicenseInfo
	{
		public static bool IsTrial
		{
			get
			{
#if DEBUG
				return true;	// or FALSE it's up to you & your testing  ;)
#else
				var license = new Microsoft.Phone.Marketplace.LicenseInformation();
				return license.IsTrial();
#endif
			}
		}
	}
}

