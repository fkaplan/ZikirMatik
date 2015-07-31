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
    public sealed partial class DuaGuncelle : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        int sayfaID;
        public DuaGuncelle()
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

            var title = e.Parameter;
            sayfaID = Int32.Parse(title.ToString());

            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var allusers = await conn.QueryAsync<Dualar>("SELECT * FROM DuaList WHERE Id = ?", title);
            //var duaKacKezOkundu = await conn.QueryAsync<Dualar>("SELECT kacKezOkundu FROM DuaList WHERE Id = ?", title);
            
            foreach (var user in allusers)
            {
                TxtDuaName.Text = user.DuaName;
                TxtDua.Text = user.Dua;
                TxtZikirSayisi.Text = user.kacKezOkunmali.ToString();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Guncelle_Clik(object sender, RoutedEventArgs e)
        {
            if(TxtDua.Text != "" && TxtDuaName.Text != "" && TxtZikirSayisi.Text != "")
            {
                Guncelle();
            }
            else
            {
                MessageDialog msg;
                msg = new MessageDialog("Lütfen Tüm Alanları Doldurun");
                msg.Commands.Add(new UICommand("Kapat"));
                await msg.ShowAsync();
            }            
        }

        public async void Guncelle()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("duaDB.db");
            var guncellenecekDua = await conn.Table<Dualar>().Where(x => x.Id == sayfaID).FirstOrDefaultAsync();
            if (guncellenecekDua != null)
            {
                guncellenecekDua.DuaName = TxtDuaName.Text;
                guncellenecekDua.Dua = TxtDua.Text;
                guncellenecekDua.kacKezOkunmali = Int32.Parse(TxtZikirSayisi.Text);

                await conn.UpdateAsync(guncellenecekDua);
            }

            MessageDialog msg;
            bool retry = false;
            msg = new MessageDialog("Duanız Başarıyla Güncellendi");
            msg.Commands.Add(new UICommand("Kapat"));
            await msg.ShowAsync();

            Frame.Navigate(typeof(MainPage));
        }
    }
}
