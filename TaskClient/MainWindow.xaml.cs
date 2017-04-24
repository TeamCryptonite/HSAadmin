using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// The following using statements were added for this sample.
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;
using System.Configuration;
using Microsoft.Identity.Client;

namespace TaskClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient httpClient = new HttpClient();
        private PublicClientApplication pca = null;
        private BizzaroClient client = null;

        UserDTO myObj = new UserDTO("", "", "", false, true);

        public MainWindow()
        {
            InitializeComponent();
        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            pca = new PublicClientApplication(Globals.clientId)
            {
                UserTokenCache = new FileCache(),
            };

            AuthenticationResult result = null;
            try
            {
                // If the user has has a token cached with any policy, we'll display them as signed-in.
                TokenCacheItem tci = pca.UserTokenCache.ReadItems(Globals.clientId).Where(i => i.Scope.Contains(Globals.clientId) && !string.IsNullOrEmpty(i.Policy)).FirstOrDefault();
                string existingPolicy = tci == null ? null : tci.Policy;
                result = await pca.AcquireTokenSilentAsync(new string[] { Globals.clientId }, string.Empty, Globals.authority, existingPolicy, false);

                SignInButton.Visibility = Visibility.Collapsed;
                //SignUpButton.Visibility = Visibility.Collapsed;
                EditProfileButton.Visibility = Visibility.Visible;
                SignOutButton.Visibility = Visibility.Visible;
                UsernameLabel.Content = result.User.Name;
                client = new BizzaroClient(result);
                GetTodoList();
            }
            catch (MsalException ex)
            {
                if (ex.ErrorCode == "failed_to_acquire_token_silently")
                {
                    // There are no tokens in the cache. 
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }
                    MessageBox.Show(message);
                }
                return;
            }
        }

        private async void SignIn(object sender = null, RoutedEventArgs args = null)
        {
            AuthenticationResult result = null;
            try
            {
                result = await pca.AcquireTokenAsync(new string[] { Globals.clientId },
                    string.Empty, UiOptions.ForceLogin, null, null, Globals.authority,
                    Globals.signInPolicy);

                SignInButton.Visibility = Visibility.Collapsed;
                //SignUpButton.Visibility = Visibility.Collapsed;
                EditProfileButton.Visibility = Visibility.Visible;
                SignOutButton.Visibility = Visibility.Visible;
                UsernameLabel.Content = result.User.Name;
                GetTodoList();
            }
            catch (MsalException ex)
            {
                if (ex.ErrorCode != "authentication_canceled")
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }

                return;
            }
        }
        

        private async void EditProfile(object sender, RoutedEventArgs e)
        {
            AuthenticationResult result = null;
            try
            {
                result = await pca.AcquireTokenAsync(new string[] { Globals.clientId },
                    string.Empty, UiOptions.ForceLogin, null, null, Globals.authority,
                    Globals.editProfilePolicy);
                UsernameLabel.Content = result.User.Name;
                GetTodoList();
            }
            catch (MsalException ex)
            {
                if (ex.ErrorCode != "authentication_canceled")
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }
                
                return;
            }
        }

        private void SignOut(object sender, RoutedEventArgs e)
        {
            // Clear any remnants of the user's session.
            pca.UserTokenCache.Clear(Globals.clientId);

            // This is a helper method that clears browser cookies in the browser control that MSAL uses, it is not part of MSAL.
            ClearCookies();

            // Update the UI to show the user as signed out.
            //TaskList.ItemsSource = string.Empty;
            SignInButton.Visibility = Visibility.Visible;
            //SignUpButton.Visibility = Visibility.Visible;
            EditProfileButton.Visibility = Visibility.Collapsed;
            SignOutButton.Visibility = Visibility.Collapsed;
            UsernameLabel.Content = "";
            return;
        }

        private async void GetTodoList()
        {
            AuthenticationResult result = null;
            try
            {
                // Here we want to check for a cached token, independent of whatever policy was used to acquire it.
                TokenCacheItem tci = pca.UserTokenCache.ReadItems(Globals.clientId).Where(i => i.Scope.Contains(Globals.clientId) && !string.IsNullOrEmpty(i.Policy)).FirstOrDefault();
                string existingPolicy = tci == null ? null : tci.Policy;

                // Use acquireTokenSilent to indicate that MSAL should throw an exception if a token cannot be acquired
                result = await pca.AcquireTokenSilentAsync(new string[] { Globals.clientId }, string.Empty, Globals.authority, existingPolicy, false);

            }
            // If a token could not be acquired silently, we'll catch the exception and show the user a message.
            catch (MsalException ex)
            {
                // There is no access token in the cache, so prompt the user to sign-in.
                if (ex.ErrorCode == "failed_to_acquire_token_silently")
                {
                    MessageBox.Show("Please sign up or sign in first");
                    SignInButton.Visibility = Visibility.Visible;
                    //SignUpButton.Visibility = Visibility.Visible;
                    EditProfileButton.Visibility = Visibility.Collapsed;
                    SignOutButton.Visibility = Visibility.Collapsed;
                    UsernameLabel.Content = string.Empty;
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }
                    MessageBox.Show(message);
                }

                return;
            }

            // Once the token has been returned by MSAL, add it to the http authorization header, before making the call to access the To Do list service.
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

            // Call the To Do list service.
            //HttpResponseMessage response = await httpClient.GetAsync(Globals.taskServiceUrl + "/api/tasks");

            //if (response.IsSuccessStatusCode)
            //{
            //    // Read the response and databind to the GridView to display To Do items.
            //    string s = await response.Content.ReadAsStringAsync();
            //    JavaScriptSerializer serializer = new JavaScriptSerializer();
            //    List<Models.Task> taskArray = serializer.Deserialize<List<Models.Task>>(s);

            //    TaskList.ItemsSource = taskArray.Select(t => new { t.Text });
            //}
            //else
            //{
            //    MessageBox.Show("An error occurred : " + response.ReasonPhrase);
            //}



            return;
        }

        private async void EmplSearch(object sender, RoutedEventArgs e)
        {

            var usersPaginator = client.Admin.GetListOfUsers(EmplSearchBar.Text);
            var usersList = await usersPaginator.Next();

            if (string.IsNullOrEmpty(EmplSearchBar.Text))
            {
                MessageBox.Show("Please enter a valid Employee Name");
                return;
            }

            AuthenticationResult result = null;
            try
            {
                TokenCacheItem tci = pca.UserTokenCache.ReadItems(Globals.clientId).Where(i => i.Scope.Contains(Globals.clientId) && !string.IsNullOrEmpty(i.Policy)).FirstOrDefault();
                string existingPolicy = tci == null ? null : tci.Policy;

                result = await pca.AcquireTokenSilentAsync(new string[] { Globals.clientId }, string.Empty, Globals.authority, existingPolicy, false);
            }
            catch (MsalException ex)
            {
                // There is no access token in the cache, so prompt the user to sign-in.
                if (ex.ErrorCode == "failed_to_acquire_token_silently")
                {
                    MessageBox.Show("Please sign up or sign in first");
                    SignInButton.Visibility = Visibility.Visible;
                    //SignUpButton.Visibility = Visibility.Visible;
                    EditProfileButton.Visibility = Visibility.Collapsed;
                    SignOutButton.Visibility = Visibility.Collapsed;
                    UsernameLabel.Content = string.Empty;
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }

                return;
            }

            EmplListBox.Items.Clear();

            //foreach (UserDTO userDTO in emplList)
            foreach(HsaServiceDtos.UserDto userDTO in usersList)
            {
                if (userDTO.GivenName.ToLower().Contains(EmplSearchBar.Text.ToLower())/* || userDTO.Surname.ToLower().Contains(EmplSearchBar.Text.ToLower())*/)
                {
                    //EmplSearchBar.Text = emplList[0].GivenName + "" + emplList[0].Surname;
                    //TaskList.ItemsSource = emplList.Select(t => new { t.GivenName } + " " + new { t.Surname });

                    //EmplListText.Content = emplList[0].GivenName + "" + emplList[0].Surname + "\n";
                    //EmplListText.Content = emplList[1].GivenName + "" + emplList[1].Surname + "\n";
                    //EmplListText.Content = emplList[2].GivenName + "" + emplList[2].Surname + "\n";

                    List<HsaServiceDtos.UserDto> searchResults = new List<HsaServiceDtos.UserDto>();
                    searchResults.Add(userDTO);

                    EmplListBox.Items.Add(new UserDTO ("",userDTO.GivenName,userDTO.Surname,userDTO.IsEmployee, userDTO.IsActiveUser));
                    EmplListBox.Tag = userDTO;

                    //myObj = userDTO;
                }
            }
            return;

        }

        
        // This function clears cookies from the browser control used by MSAL.
        private void ClearCookies()
        {
            const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
        }

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        private void EmplListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //textBox.Text = EmplListBox.SelectedItem.ToString();

            //UserDTO myobj = (UserDTO)e.Items.Tag;

            
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var usersPaginator = client.Admin.GetListOfUsers();
            var usersList = await usersPaginator.Next();

            var updatedUser = usersList.FirstOrDefault(u => u.GivenName.Contains(textBox.Content.ToString()));
            if (radioButton.IsChecked == true)
            {
                updatedUser.IsActiveUser = true;

            }
            else
            {
                updatedUser.IsActiveUser = false;
            }

            if (radioButton2.IsChecked == true)
            {
                updatedUser.IsEmployee = true;
            }
            else
            {
                updatedUser.IsEmployee = false;
            }

            var status = await client.Admin.UpdateUser(updatedUser.UserGuid, updatedUser);
        }
    }
}
