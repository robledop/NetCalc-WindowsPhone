using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NetCalc.Model;
using NetCalc.Resources;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.Linq;

namespace NetCalc.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }


        private Dictionary<int, string> masksList;
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        bool toastCheck = true;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            //if (Helpers.LicenseInfo.IsTrial == false)
            //{
            //    ContentPanel.Children.Remove(adControl);
            //    ContentPanel.Children.Remove(AdDuplex);
            //}

            //AdControl adControl = new AdControl("58eb0733-0b25-4cdb-b777-de6035c6522e", "141657", true);

            ////AdControl adControl = new AdControl("test_client", "Image480_80" , true);
            //adControl.SetValue(Grid.RowProperty, 2);
            //adControl.Height = 50;
            //adControl.Width = 320;
            //ContentPanel.Children.Add(adControl);

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();

            masksList = new Dictionary<int, string>();


            masksList.Add(1, "128.0.0.0");
            masksList.Add(2, "192.0.0.0");
            masksList.Add(3, "224.0.0.0");
            masksList.Add(4, "240.0.0.0");
            masksList.Add(5, "248.0.0.0");
            masksList.Add(6, "252.0.0.0");
            masksList.Add(7, "254.0.0.0");
            masksList.Add(8, "255.0.0.0");
            masksList.Add(9, "255.128.0.0");
            masksList.Add(10, "255.192.0.0");
            masksList.Add(11, "255.224.0.0");
            masksList.Add(12, "255.240.0.0");
            masksList.Add(13, "255.248.0.0");
            masksList.Add(14, "255.252.0.0");
            masksList.Add(15, "255.254.0.0");
            masksList.Add(16, "255.255.0.0");
            masksList.Add(17, "255.255.128.0");
            masksList.Add(18, "255.255.192.0");
            masksList.Add(19, "255.255.224.0");
            masksList.Add(20, "255.255.240.0");
            masksList.Add(21, "255.255.248.0");
            masksList.Add(22, "255.255.252.0");
            masksList.Add(23, "255.255.254.0");
            masksList.Add(24, "255.255.255.0");
            masksList.Add(25, "255.255.255.128");
            masksList.Add(26, "255.255.255.192");
            masksList.Add(27, "255.255.255.224");
            masksList.Add(28, "255.255.255.240");
            masksList.Add(29, "255.255.255.248");
            masksList.Add(30, "255.255.255.252");
            masksList.Add(31, "255.255.255.254 - (RFC 3021)");
            masksList.Add(32, "255.255.255.255");

            lstNetBits.ItemsSource = masksList;

            if (settings.Contains("AddressBlock"))
            {
                txtIP.Text = settings["AddressBlock"].ToString();
            }
            else
            {
                txtIP.Text = "10.0.0.0";
            }

            if (settings.Contains("Mask"))
            {
                lstNetBits.SelectedIndex = (int)settings["Mask"];
            }
            else
            {
                lstNetBits.SelectedIndex = 23;
            }

            if (settings.Contains("Subnets"))
            {
                lstSubnets.SelectedIndex = (int)settings["Subnets"];
            }
            else
            {
                lstSubnets.SelectedIndex = 0;
            }
        }

        private void Calculate()
        {
            try
            {
                KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)lstNetBits.SelectedItem;
                byte netBits = Convert.ToByte(selectedItem.Key);

                txtIP.Text = txtIP.Text.Replace(",", ".");

                IPSegment ipNetwork = new IPSegment(txtIP.Text, netBits);

                KeyValuePair<uint, uint> selectedSubnets = (KeyValuePair<uint, uint>)lstSubnets.SelectedItem;

                // número de opções de números de subnets
                double additionalBitsToTheSubnetMask = 0;
                //if ((uint)lstSubnets.SelectedItem != 1)
                //{
                    additionalBitsToTheSubnetMask = Math.Round(Math.Log(Convert.ToDouble(selectedSubnets.Key), 2));
                //}

                byte subnetting = (byte)(netBits + additionalBitsToTheSubnetMask);

                IPSegmentCollection ipNetCollection = new IPSegmentCollection(ipNetwork, subnetting);

                txtCIDR.Text = ipNetCollection.Current.Cidr.ToString();
                txtHosts.Text = ipNetCollection.Current.NumberOfHosts.ToString();

                if (ipNetCollection.Current.Rfc3021)
                {
                    txtRFC3021.Text = AppResources.True;
                }
                else
                {
                    txtRFC3021.Text = AppResources.False;
                }

                Subnets.ItemsSource = ipNetCollection;

                settings["AddressBlock"] = txtIP.Text;
                settings["Mask"] = lstNetBits.SelectedIndex;
                settings["Subnets"] = lstSubnets.SelectedIndex;

                settings.Save();
            }
            catch (Exception)
            {
                MessageBox.Show(AppResources.InvalidIP);
            }
            finally
            {
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = System.Windows.Visibility.Visible;

            this.Dispatcher.BeginInvoke(delegate
            {
                if (toastCheck)
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Title = AppResources.Tip;
                    toast.Message = AppResources.Toast;
                    toast.TextWrapping = TextWrapping.Wrap;
                    toast.MillisecondsUntilHidden = 5000;
                    toast.Show();

                    toastCheck = false;
                }
            });

            Subnets.ItemsSource = null;
            Subnets.Items.Clear();

            this.Dispatcher.BeginInvoke(delegate
            {
                Calculate();

                
            });
        }

        private void TxtNetBitsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lstNetBits.SelectedItem != null)
                {
                    KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)lstNetBits.SelectedItem;

                    IPSegment ipNetwork = new IPSegment(txtIP.Text, Convert.ToByte(selectedItem.Key));
                    IPSegmentCollection ipNetCollection = new IPSegmentCollection(ipNetwork, 32);

                    uint maxSubnets = Convert.ToUInt32(ipNetCollection.Count);
                    List<uint> sub = new List<uint>();

                    do
                    {
                        sub.Add(maxSubnets);
                        maxSubnets = maxSubnets / 2;
                    } while (maxSubnets >= 1);

                    sub.Reverse();

                    List<uint> numberOfHosts = new List<uint>();

                    for (int i = 0; i < sub.Count; i++)
                    {
                        IPSegment ipSegment = new IPSegment(txtIP.Text, Convert.ToByte(selectedItem.Key + i));
                        numberOfHosts.Add(ipSegment.NumberOfHosts);
                    }

                    Dictionary<uint, uint> dict = sub.ToDictionary(x => x, x => numberOfHosts[sub.IndexOf(x)]);

                    lstSubnets.ItemsSource = dict;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(AppResources.InvalidIP);
            }
        }

        private void ApplicationBarAboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutPrompt about = new AboutPrompt();
            about.VersionNumber = "v1.4";
            about.Show("Robledo Pazotto", "", "robledo@gmail.com");
        }

        void AppBarRateMenuItemClick(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarAboutMenuItem = new ApplicationBarMenuItem(AppResources.About);
            appBarAboutMenuItem.Click += ApplicationBarAboutMenuItem_Click;

            ApplicationBarMenuItem appBarRateMenuItem = new ApplicationBarMenuItem(AppResources.RateAndReview);
            appBarRateMenuItem.Click += AppBarRateMenuItemClick;

            ApplicationBarMenuItem appBarBuyMenuItem = new ApplicationBarMenuItem(AppResources.BuyToRemoveAdvertisements);
            appBarBuyMenuItem.Click += AppBarBuyMenuItemClick;

            ApplicationBar.MenuItems.Add(appBarAboutMenuItem);
            ApplicationBar.MenuItems.Add(appBarRateMenuItem);

            //if (Helpers.LicenseInfo.IsTrial == true)
            //{
                ApplicationBar.MenuItems.Add(appBarBuyMenuItem);
            //}
        }

        void AppBarBuyMenuItemClick(object sender, EventArgs e)
        {
            MarketplaceDetailTask buyTask = new MarketplaceDetailTask();
            buyTask.ContentIdentifier = "cdb1ecb1-bca5-4afa-97ff-55c5e6682ff5";
            buyTask.Show();
        }

        private void Subnets_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IPSegment subnet = Subnets.SelectedItem as IPSegment;

            if (subnet != null)
            {
                SubnetDetailsControl subnetDetails = new SubnetDetailsControl();

                subnetDetails.txtNetworkID.Text = subnet.NetworkAddress.ToIpString();
                subnetDetails.txtBroadcast.Text = subnet.BroadcastAddress.ToIpString();
                subnetDetails.txtCIDR.Text = subnet.Cidr.ToString();
                subnetDetails.txtFirstUsable.Text = subnet.FirstUsable.ToIpString();
                subnetDetails.txtLastUsable.Text = subnet.LastUsable.ToIpString();
                subnetDetails.txtNumberOfHosts.Text = subnet.NumberOfHosts.ToString();
                subnetDetails.txtSubnetMask.Text = subnet.SubnetMask.ToString();
                subnetDetails.txtWildCardSubnetMask.Text = subnet.WildCardSubnetMask;

                if (subnet.Cidr == 31)
                {
                    subnetDetails.txtRFC3021.Text = AppResources.True;
                }
                else
                {
                    subnetDetails.txtRFC3021.Text = AppResources.False;
                }

                MessagePrompt messagePrompt = new MessagePrompt
                {
                    Title = AppResources.SubnetDetails,
                    Body = subnetDetails
                };

                messagePrompt.Show();
            }
        }

        private void AdControlErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            //MessageBox.Show(e.Error.Message);
            adControl.Visibility = System.Windows.Visibility.Collapsed;

            //smaato.Visibility = System.Windows.Visibility.Visible;
            //mobFox.Visibility = System.Windows.Visibility.Visible;
            AdDuplex.Visibility = System.Windows.Visibility.Visible;
        }

        private void AdDuplex_AdLoadingError(object sender, AdDuplex.AdLoadingErrorEventArgs e)
        {
            AdDuplex.Visibility = System.Windows.Visibility.Collapsed;
        }

       

 

        //private void mobFox_NoAd(object sender, MobFox.Ads.NoAdEventArgs args)
        //{
        //    //mobFox.Visibility = System.Windows.Visibility.Collapsed;
        //    AdDuplex.Visibility = System.Windows.Visibility.Visible;
        //}

     
    }
}