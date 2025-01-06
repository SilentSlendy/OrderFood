using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace POS.ViewModels
{
    /// <summary>
    /// View model cho Settings
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// View model cho Payment
        /// </summary>
        private PaymentViewModel _paymentViewModel;
        /// <summary>
        /// Local settings
        /// </summary>
        private readonly ApplicationDataContainer _localSettings;

        /// <summary>
        /// Giá trị mặc định
        /// </summary>
        private readonly float defaultVAT = 10.0f;
        private readonly string defaultAccessKey = "F8BBA842ECF85";
        private readonly string defaultSecretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
        private readonly string defaultIpnUrl = "https://webhook.site/4cb43743-df24-494e-839d-c6cc184d872c";

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsViewModel()
        {
            _localSettings = ApplicationData.Current.LocalSettings;

            // Khởi tạo giá trị từ LocalSettings hoặc dùng mặc định
            VAT = GetSetting(nameof(VAT), defaultVAT);
            AccessKey = GetSetting(nameof(AccessKey), defaultAccessKey);
            SecretKey = GetSetting(nameof(SecretKey), defaultSecretKey);
            IpnUrl = GetSetting(nameof(IpnUrl), defaultIpnUrl);
        }

        /// <summary>
        /// Thuộc tính có thông báo thay đổi
        /// </summary>
        private float _vat;
        public float VAT
        {
            get => _vat;
            set
            {
                _vat = value;
                OnPropertyChanged();
                SaveToLocalSetting(nameof(VAT), _vat);
            }
        }

        /// <summary>
        /// Access key
        /// </summary>
        private string _accessKey;
        public string AccessKey
        {
            get => _accessKey;
            set
            {
                _accessKey = value;
                OnPropertyChanged();
                SaveToLocalSetting(nameof(AccessKey), _accessKey);
            }
        }

        /// <summary>
        /// Secret key
        /// </summary>
        private string _secretKey;
        public string SecretKey
        {
            get => _secretKey;
            set
            {
                _secretKey = value;
                OnPropertyChanged();
                SaveToLocalSetting(nameof(SecretKey), _secretKey);
            }
        }

        /// <summary>
        /// Ipn Url webhook
        /// </summary>
        private string _ipnUrl;
        public string IpnUrl
        {
            get => _ipnUrl;
            set
            {
                _ipnUrl = value;
                OnPropertyChanged();
                SaveToLocalSetting(nameof(IpnUrl), _ipnUrl);
            }
        }

        /// <summary>
        /// Lấy giá trị từ LocalSettings hoặc giá trị mặc định
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T GetSetting<T>(string key, T defaultValue)
        {
            if (_localSettings.Values[key] is T value)
                return value;

            return defaultValue;
        }

        /// <summary>
        /// Lưu giá trị vào LocalSettings
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SaveToLocalSetting(string key, object value)
        {
            _localSettings.Values[key] = value;
        }

        /// <summary>
        /// Load local settings
        /// </summary>
        public void PaymentLoadLocalSettings()
        {
            _paymentViewModel = new PaymentViewModel();
            _paymentViewModel.LoadLocalSettings();
        }

        /// <summary>
        /// Sự kiện PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
