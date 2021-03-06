﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Collaborator
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private MainWindow mainWindow = null;
        internal List<User> users;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        Server server = null;
        Client client = null;
        User user = null;
        public Dashboard()
        {
            InitializeComponent();

            Init();

        }
        
        private void Init()
        {
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();

        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ContactList.ItemsSource = users;

            server = new Server(ChatMessageScroll);
            server.StartServer();

            client = new Client();
            client.StartClient();

        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            User.Instance.Init();
            users = User.Instance.ContactList;

            // Checking whether the IP of current user has changed or not!
            if (!User.Instance.CheckIP())
            {
                User.Instance.UpdateIP();
            }
        }
        

        public string FullName { get; set; }
        public MainWindow MainWindowInstance
        {
            set{ mainWindow = value; }
        }

        private void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            user = ContactList.SelectedItem as User;
            HeaderTextBlock.Text = user.Name;
            ChatMessage.ItemsSource = user.messages;
            ChatMessageScroll.ScrollToBottom();


        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            User.Instance.Reset();
            mainWindow.Show();
            client.StopClient();
            server.StopServer();
            this.Close();
        }

        private void TopPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CloseIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseIcon.Foreground = Brushes.Red;
        }

        private void CloseIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseIcon.Foreground = Brushes.White;
        }

        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox MessageTextBox = sender as TextBox;
            if(MessageTextBox.Text.Length > 0 && !SendButton.IsEnabled)
            {
                SendButton.IsEnabled = true;
            }
            else if (MessageTextBox.Text.Length == 0 && SendButton.IsEnabled)
            {
                SendButton.IsEnabled = false;
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

            if (user.Alive && user.Client != null)
            {
                client.SendMessage(MessageTextBox.Text, user.Client);
                user.messages.Add(new Message() { Text = MessageTextBox.Text, Align = "Right", DateTime=DateTime.Now.ToString("hh:mm tt ddd"), Color="LightBlue"});
                
            }
            else
            {
                user.messages.Add(new Message() { Text = MessageTextBox.Text, Align = "Right", DateTime = DateTime.Now.ToString("hh:mm tt ddd"), Color = "LightBlue" });
                //user.messages.Add(new Message() { Text = "Message failed! "+ user.Name + " is Offline...", Align = "Right", DateTime = DateTime.Now.ToString("hh:mm tt ddd"), Color = "LightBlue" });
            }
            client.StoreMessage(MessageTextBox.Text, user.Id.ToString());
            MessageTextBox.Text = String.Empty;
            ChatMessageScroll.ScrollToBottom();

        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(this);
            profile.Show();
            this.Hide();
        }
    }
}
