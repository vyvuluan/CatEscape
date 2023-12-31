using UnityEngine;

namespace Entry
{
    public class EntryModel : MonoBehaviour
    {
        [Header("Appsflyer")]
        [SerializeField] private string appsFlyerDevKey = "DEB_KEY";
        [SerializeField] private string appsFlyerAppIdIos = "APP_ID";
        [SerializeField] private string appsFlyerAppIdAndroid = "APP_ID";

        [Header("IAP")]
        [SerializeField] private string iAPRemoveAdsKey = "";

        [Header("Game Service")]
        [SerializeField] private string tosURL = "url";
        [SerializeField] private string privacyURL = "url";
        [SerializeField] private string rateURL = "url";

        [Header("Ads")]
        [SerializeField] private string maxSDKKey = "";

        [Header("Android")]
        [SerializeField] private string interstitialIdAndroid = "";
        [SerializeField] private string bannerIdAndroid = "";
        [SerializeField] private string rewardedIdAndroid = "";
		[SerializeField] private string mrecIdAndroid = "";
        [SerializeField] private string appOpenAdsIdAndroid = "";

        [Header("IOs")]
        [SerializeField] private string interstitialIdIOS = "";
        [SerializeField] private string bannerIdIOS = "";
        [SerializeField] private string rewardedIdIOS = "";
		[SerializeField] private string mrecIdIos = "";
		[SerializeField] private string appOpenAdsIdIos = "";

        public string AppsFlyerDevKey => appsFlyerDevKey;
        public string AppsFlyerAppIdIos => appsFlyerAppIdIos;
        public string AppFlyerAppIdAndroid => appsFlyerAppIdAndroid;
        public string IAPRemoveAdsKey => iAPRemoveAdsKey;
        public string TOSURL => tosURL;
        public string PrivacyURL => privacyURL;
        public string RateURL => rateURL;
        public string MaxSDKKey => maxSDKKey;
        public string IntersIdAndroid => interstitialIdAndroid;
        public string BannerIdAndroid => bannerIdAndroid;
        public string RewardedIdAndroid => rewardedIdAndroid;
		public string MrecIdAndroid => mrecIdAndroid;
        public string AOAIdAndroid => appOpenAdsIdAndroid;
        public string IntersIdIOS => interstitialIdIOS;
        public string BannerIdIOS => bannerIdIOS;
        public string RewardedIdIOS => rewardedIdIOS;
		public string MrecIdIos => mrecIdIos;
        public string AOAIdIOS => appOpenAdsIdIos;
    }
}
