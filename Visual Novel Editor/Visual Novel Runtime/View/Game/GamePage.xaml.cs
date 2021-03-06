﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vnrt.Runtime.Common;
using Vnrt.Utilities;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Vnrt.Runtime
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage : Page, INotifyPropertyChanged
    {
        private NavigationHelper navigationHelper;
        private string game;

        public string GetCurrentGame()
        {
            return game;
        }

        private async void SetCurrentGame(string theGame)
        {
            game = theGame;
            await GamePageView.InvokeScriptAsync("loadGame", new string[] { game });
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public GamePage()
        {
            this.InitializeComponent();
            DataContext = this;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            GamePageView.ScriptNotify += GamePageView_ScriptNotify;
        }

        private void GamePageView_ScriptNotify(object sender, NotifyEventArgs e)
        {
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
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
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        ///
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                string gameDir = e.Parameter.ToString();
                GamePageView.Navigate(new Uri("ms-appdata:///local/" + gameDir + "/index.html"));
                GamePageView.NavigationCompleted += async (ob, ev) =>
                {
                    SetCurrentGame(await FileUtil.LoadGameAsJson(gameDir));
                };
            }
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion NavigationHelper registration

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberNameAttribute] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }

        private async void AppBarButton_Home_Click(object sender, RoutedEventArgs e)
        {
            string confirmMsg = ResourceLoader.GetForCurrentView().GetString("GameQuitConfirmMsg");
            string yesMsg = ResourceLoader.GetForCurrentView().GetString("YesMsg");
            string noMsg = ResourceLoader.GetForCurrentView().GetString("NoMsg");

            MessageDialog confirmDialog = new MessageDialog(confirmMsg);
            confirmDialog.Commands.Add(new UICommand(yesMsg, (cmd) => Frame.Navigate(typeof(MainPage))));
            confirmDialog.Commands.Add(new UICommand(noMsg, (cmd) => { }));

            await confirmDialog.ShowAsync();
        }
    }
}