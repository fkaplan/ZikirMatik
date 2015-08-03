using App3.Common;
using App3.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Phone.Devices.Notification;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DuaSayfasi : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public List<Dualar> dualar { get; set; }
        int sayfaID;
        
        public DuaSayfasi()
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


        private async void arttır_Click(object sender, RoutedEventArgs e)
        {
            
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var sayac = await conn.Table<Dualar>().Where(x => x.Id == sayfaID).FirstOrDefaultAsync();
            if (sayac != null)
            {
                sayac.kacKezOkundu += 1;
                await conn.UpdateAsync(sayac);
            }

            var allusers = await conn.QueryAsync<Dualar>("SELECT kacKezOkundu FROM DuaList WHERE Id = ?" , sayfaID);
            foreach (var user in allusers)
            {
                //txtDuaSayaci.Text = user.kacKezOkundu.ToString();
                txtDuaSayisi.Text = user.kacKezOkundu.ToString();


                //Eğer titreşim modu aktifse hedefe ulaşıldığında telefon titresin
                if (Int32.Parse(txtDuaSayisi.Text) == Int32.Parse(sayac.kacKezOkunmali.ToString()) && ToggleTitresim.IsOn == true)
                {
                    VibrationDevice testVibration = VibrationDevice.GetDefault();
                    testVibration.Vibrate(TimeSpan.FromSeconds(2));
                }
            }

            
           
        }

        private async void sifirla_Click(object sender, RoutedEventArgs e)
        {
            // SayfaID'sinden dualistesinde istenen duayı bul, kacKezOkundu değerini sıfırla
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var sayac = await conn.Table<Dualar>().Where(x => x.Id == sayfaID).FirstOrDefaultAsync();
            if (sayac != null)
            {
                sayac.kacKezOkundu = 0;
                await conn.UpdateAsync(sayac);
            }

            var allusers = await conn.QueryAsync<Dualar>("SELECT kacKezOkundu FROM DuaList WHERE Id = ?",sayfaID);
            foreach (var user in allusers)
            {
                //txtDuaSayaci.Text = user.kacKezOkundu.ToString();
                txtDuaSayisi.Text = user.kacKezOkundu.ToString();
            }
        }

        private void guncelle_Click(object sender, RoutedEventArgs e)
        {
            if (sayfaID != null)
            {
                //Frame.Navigate(typeof(DuaSayfasi), ((Dualar)DuaListesi.SelectedItem).Id);
                Frame.Navigate(typeof(DuaGuncelle), sayfaID);               
            }
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

            var title = e.Parameter;
            sayfaID =Int32.Parse(title.ToString());

            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var allusers = await conn.QueryAsync<Dualar>("SELECT * FROM DuaList WHERE Id = ?", title);
            var duaKacKezOkundu = await conn.QueryAsync<Dualar>("SELECT kacKezOkundu FROM DuaList WHERE Id = ?", title);

            Duaİcerik.ItemsSource = allusers;

            foreach (var user in duaKacKezOkundu)
            {
                //txtDuaSayaci.Text = user.kacKezOkundu.ToString();
                txtDuaSayisi.Text = user.kacKezOkundu.ToString();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        
    }
}
