﻿using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace Collaborator
{
    class Login
    {
        private string username;
        private string password;
        public Login()
        {

        }
        public Login(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public bool Perform()
        {
            DBConnection.Instance.Connect();
            string query = "SELECT id, username, name, photo_path, ip FROM USER WHERE USERNAME='" + username + "' AND PASSWORD='" + password + "';";
            try
            {
                using (MySqlCommand mySqlCommand = new MySqlCommand(query, DBConnection.Instance.Connection))
                {
                    MySqlDataReader dataReader = mySqlCommand.ExecuteReader();
                    if (dataReader.Read())
                    {
                        User.Instance.Init(dataReader);
                        User.Instance.Save();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                DBConnection.Instance.Connection = null;
                MessageBox.Show(e.Message, this.ToString() + " Perform() Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
