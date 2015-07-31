using App3.Common;
using App3.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        public List<Dualar> dualar { get; set; }
        public List<Zikirmatik> sayac { get; set; }
        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            //Sayfa yüklenmeden önce veritabanı kontrol ediliyor
            bool dbExist = await CheckDbAsync("duaDB.db");

            //Eğer veritabanı yoksa oluşturuluyor
            if (!dbExist)
            {
                await CreateDatabaseAsync();
                await AddUsersAsync();
            }

            // Veritabanı içerisindeki dualar alınıyor
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var query = conn.Table<Dualar>();
            dualar = await query.ToListAsync();

            // Show users         
            DuaListesi.ItemsSource = dualar;


            //Zikirmatik sayacı son kaldığı değere atanıyor
            var allusers = await conn.QueryAsync<Zikirmatik>("SELECT Counter FROM ZikirmatikSayac WHERE Id = 1");
            foreach (var user in allusers)
            {
                Sayac.Text = user.Counter.ToString();
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region SQLite utils
        private async Task<bool> CheckDbAsync(string dbName)
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(dbName);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }
        private async Task CreateDatabaseAsync()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            await conn.CreateTableAsync<Dualar>();
            await conn.CreateTableAsync<Zikirmatik>();
        }
        private async Task AddUsersAsync()
        {
            // Create a users list
            var duaList = new List<Dualar>()
            {
                new Dualar()
                {
                    DuaName = "Allahu Ekber",
                    Dua = "Allahu Ekber",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                },
                new Dualar()
                {
                    DuaName = "La ilahe İllallah",
                    Dua = "La ilahe İllallah",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                },
                new Dualar()
                {
                    DuaName = "Subhanallah",
                    Dua = "Subhanallah",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                },
                new Dualar()
                {
                    DuaName = "Elhamdulillah",
                    Dua = "Elhamdulillah",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                },
                new Dualar()
                {
                    DuaName = "Tövbe Estağfurullah",
                    Dua = "Tövbe Estağfurullah",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                },
                new Dualar()
                {
                    DuaName = "Kelime-i Tevhid",
                    Dua = "La ilahe İllallah, Muhammedün Resulullah",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                },
                new Dualar()
                {
                    DuaName="Kelime-i Şehadet",
                    Dua ="Eşhedü en la ilahe illallah ve eşhedü enne Muhammeden abdühü ve resulühü",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                },
                new Dualar()
                {
                    DuaName ="Salavat-ı Şerif",
                    Dua = "Allahümme Salli Ala Seyyidina Muhammedin ve Ala Ali Seyyidina Muhammed",
                    kacKezOkunmali=100,
                    kacKezOkundu=0
                }
            };
            var sayacList = new List<Zikirmatik>()
            {
                new Zikirmatik()
                {
                    Counter=0
                }
            };

            // Add rows to the User Table
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            await conn.InsertAllAsync(duaList);
            await conn.InsertAllAsync(sayacList);

            //Zikirmatiğin sayacı son kaldığı değere setleniyor
            var allusers = await conn.QueryAsync<Zikirmatik>("SELECT Counter FROM ZikirmatikSayac WHERE Id = 1");
            foreach (var user in allusers)
            {
                Sayac.Text = user.Counter.ToString();
            }
        }

        #endregion SQLite utils



        private async void arttır_Click(object sender, RoutedEventArgs e)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var sayac = await conn.Table<Zikirmatik>().Where(x => x.Id == 1).FirstOrDefaultAsync();
            if(sayac != null)
            {
                sayac.Counter += 1;
                await conn.UpdateAsync(sayac);
            }

            var allusers = await conn.QueryAsync<Zikirmatik>("SELECT Counter FROM ZikirmatikSayac WHERE Id = 1");
            foreach (var user in allusers)
            {
                Sayac.Text = user.Counter.ToString();
            }
        }

        private async void sifirla_Click(object sender, RoutedEventArgs e)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var sayac = await conn.Table<Zikirmatik>().Where(x => x.Id == 1).FirstOrDefaultAsync();
            if (sayac != null)
            {
                sayac.Counter =0;
                await conn.UpdateAsync(sayac);
            }

            var allusers = await conn.QueryAsync<Zikirmatik>("SELECT Counter FROM ZikirmatikSayac WHERE Id = 1");
            foreach (var user in allusers)
            {
                Sayac.Text = user.Counter.ToString();
            }
        }

        private void DuaListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DuaListesi.SelectedItem != null)
            {
                //Frame.Navigate(typeof(DuaSayfasi), ((Dualar)DuaListesi.SelectedItem).Id);
                Frame.Navigate(typeof(DuaSayfasi), ((Dualar)DuaListesi.SelectedItem).Id);
                DuaListesi.SelectedItem = null;
            }
        }





        private void DuaEkle_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DuaEkle));
        }





        private void rateButton_Click(object sender, RoutedEventArgs e)
        {
            AskForAppRating();
        }
        private async Task AskForAppRating()
        {
            if (!CheckInternetConnectivity())
                return;

            MessageDialog msg;
            bool retry = false;
            msg = new MessageDialog("Uygulamayı mağazada değerlendirerek geliştirme sürecine destek olmak ister misiniz?");
            msg.Commands.Add(new UICommand("Oyla", new UICommandInvokedHandler(this.OpenStoreRating)));
            msg.Commands.Add(new UICommand("Şimdi Değil"));
            msg.DefaultCommandIndex = 0;
            msg.CancelCommandIndex = 1;
            await msg.ShowAsync();

            while (retry)
            {
                try { await msg.ShowAsync(); }
                catch (Exception ex) { }
            }
        }
        public static bool CheckInternetConnectivity()
        {
            var internetProfile = NetworkInformation.GetInternetConnectionProfile();
            if (internetProfile == null)
                return false;

            return (internetProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }
        private async void OpenStoreRating(IUICommand command)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=c27c60c5-a949-4a8a-bdd5-41daf7c9809d"));
        }


        private void hakkinda_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(hakkinda));
        }

    }
}
