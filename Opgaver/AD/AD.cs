using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

using Terminal.Gui;

namespace Opgaver.AD
{
    public class ADProgram
    {
        public static void Run()
        {
            NewConnectionToServer().Bind(); //check connection

            Application.Run<MainMenu>();

            Application.Shutdown();
        }

        private static LdapConnection NewConnectionToServer() => 
            new LdapConnection(Enviroment.Get()["server"])
            {
                Credential = new NetworkCredential($"{Enviroment.Get()["username"]}@{Enviroment.Get()["domain"]}", Enviroment.Get()["password"]),
                AuthType = AuthType.Negotiate
            };

        public static DataTable GetAllGroups()
        {
            DataTable groups = new DataTable();

            groups.Columns.Add("Name", typeof(string));
            groups.Columns.Add("Description", typeof(string));

            using (var connection = NewConnectionToServer())
            {
                var searchRequest = new SearchRequest(
                "DC=mags,DC=local",
                "(objectClass=group)",
                SearchScope.Subtree, 
                "cn", 
                "description" 
                  );

                try
                {
                    var response = (SearchResponse)connection.SendRequest(searchRequest);

                    foreach (SearchResultEntry gruppe in response.Entries)
                    {
                        groups.Rows.Add(
                                gruppe.Attributes["cn"]?[0]?.ToString() ?? "N/A", // Name
                                gruppe.Attributes["description"]?[0]?.ToString() ?? "N/A" // Description
                            );
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($@"Der skete en fejl ved hentning af grupper: {ex.Message}");
                }
            }

            return groups;
        }


        public static DataTable GetAllUsers()
        {
            DataTable users = new DataTable();

            users.Columns.Add("Name", typeof(string));
            users.Columns.Add("Username", typeof(string));
            users.Columns.Add("Email", typeof(string));
            users.Columns.Add("Department", typeof(string));
            users.Columns.Add("Title", typeof(string));
            users.Columns.Add("DistinguishedName", typeof(string));



            using (var connection = NewConnectionToServer())
            {
                var searchRequest = new SearchRequest(
                    "DC=mags,DC=local",
                    "(objectClass=user)",
                    SearchScope.Subtree,
                    "cn",
                    "samAccountName",
                    "mail",
                    "department",
                    "title",
                    "distinguishedName"
                );

                try
                {
                    var response = (SearchResponse)connection.SendRequest(searchRequest);

                    foreach (SearchResultEntry entry in response.Entries)
                    {
                        users.Rows.Add(
                            entry.Attributes["cn"]?[0]?.ToString() ?? "N/A", // Name
                            entry.Attributes["samAccountName"]?[0]?.ToString() ?? "N/A", // Username
                            entry.Attributes["mail"]?[0]?.ToString() ?? "N/A", // Email
                            entry.Attributes["department"]?[0]?.ToString(), // Department
                            entry.Attributes["title"]?[0]?.ToString() ?? "N/A", // Title
                            entry.Attributes["distinguishedName"]?[0]?.ToString() ?? "N/A" // DistinguishedName
                            );
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fejl ved hentning af brugere: {ex.Message}");
                }
            }

            return users;
        }


        public class Enviroment
        {
            private static Dictionary<string, string>? env = null;
            public static Dictionary<string, string> Get()
            {
                if (env != null)
                {
                    return env;
                }
                string[] rawEnviroment = File.ReadAllLines(@"C:\Users\benj3\Documents\!personal\Mercantec\GF2\gf2-benj381a\Opgaver\AD\.env");

                Dictionary<string, string> enviroment = new();

                foreach (string rawItem in rawEnviroment)
                {
                    string[] item = rawItem.Split("=");

                    enviroment.Add(item[0], item[1]);
                }

                return enviroment;
            }

        }


        public class MainMenu : Window
        {
            DataTable users;
            DataTable groups;

            public MainMenu()
            {
                users = GetAllUsers();
                groups = GetAllGroups();

                DataColumn checkedinColumn = new DataColumn();
                checkedinColumn.ColumnName = "Checked in";
                checkedinColumn.DataType = typeof(bool);
                checkedinColumn.DefaultValue = false;
                checkedinColumn.ReadOnly = false;
                users.Columns.Add(checkedinColumn);

                Title = "AD (Ctrl + Q to quit)";

                ColorScheme = new ColorScheme()
                {
                    Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
                    Focus  = new Terminal.Gui.Attribute(Color.Black, Color.White),

                    HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black),
                    HotFocus  = new Terminal.Gui.Attribute(Color.Black, Color.White),
                };

                TableView usersView = new TableView(users) { Width = Dim.Fill(), Height = Dim.Fill() };
                TableView groupsView = new TableView(groups) { Width = Dim.Fill(), Height = Dim.Fill() };
                View checkinView = new View() { Width = Dim.Fill(), Height = Dim.Fill() };
                View checkoutView = new View() { Width = Dim.Fill(), Height = Dim.Fill() };

                #region Checkin

                TextField checkinTextField = new TextField("")
                {
                    Width = Dim.Fill(),
                };

                LineView checkinLine = new LineView(Terminal.Gui.Graphs.Orientation.Horizontal)
                {
                    Y = Pos.Bottom(checkinTextField),
                    Width = Dim.Fill(),
                    Height = 1,
                };

                ListView checkinList = new ListView(users.AsEnumerable().Where(row => row.Field<bool>("Checked in") == false).Select(row => row.Field<string>("Name")).ToList())
                {
                    Y = Pos.Bottom(checkinLine),
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };


                checkinTextField.TextChanged += (txt) =>
                {
                    UpdateList(checkinList, checkinTextField, false);
                };

                checkinList.OpenSelectedItem += (args) =>
                {
                    string user = users.AsEnumerable()
                            .Where(row => row.Field<bool>("Checked in") == false)
                            .Select(row => row.Field<string>("Name"))
                            .Where(val => (val ?? "").ToLower().StartsWith((string)checkinTextField.Text.ToLower()))
                            .ToList()
                            [args.Item]
                            ?? "";

                    int choice = MessageBox.Query(
                        "Checkin",
                        user,
                        "Checkin",
                        "Cancel"
                        );



                    if (choice == 0) // checkin
                    {
                        users.AsEnumerable()
                            .Where(row => row.Field<string>("Name") == user)
                            .ToList()[0]
                            .SetField<bool>("Checked in", true);

                        UpdateList(checkinList, checkinTextField, false);
                    }
                };


                checkinView.Add(checkinTextField, checkinLine, checkinList);

                #endregion Checkin

                #region Checkout

                TextField checkoutTextField = new TextField("")
                {
                    Width = Dim.Fill(),
                };

                LineView checkoutLine = new LineView(Terminal.Gui.Graphs.Orientation.Horizontal)
                {
                    Y = Pos.Bottom(checkoutTextField),
                    Width = Dim.Fill(),
                    Height = 1,
                };

                ListView checkoutList = new ListView(users.AsEnumerable().Where(row => row.Field<bool>("Checked in") == true).Select(row => row.Field<string>("Name")).ToList())
                {
                    Y = Pos.Bottom(checkoutLine),
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };


                checkoutTextField.TextChanged += (txt) =>
                {
                    UpdateList(checkoutList, checkoutTextField, true);
                };

                checkoutList.OpenSelectedItem += (args) =>
                {
                    string user = users.AsEnumerable()
                            .Where(row => row.Field<bool>("Checked in") == true)
                            .Select(row => row.Field<string>("Name"))
                            .Where(val => (val ?? "").ToLower().StartsWith((string)checkoutTextField.Text.ToLower()))
                            .ToList()
                            [args.Item]
                            ?? "";

                    int choice = MessageBox.Query(
                        "Checkout",
                        user,
                        "Checkout",
                        "Cancel"
                        );



                    if (choice == 0) // checkout
                    {
                        users.AsEnumerable()
                            .Where(row => row.Field<string>("Name") == user)
                            .ToList()[0]
                            .SetField<bool>("Checked in", false);

                        UpdateList(checkoutList, checkoutTextField, true);
                    }
                };

                checkoutView.Add(checkoutTextField, checkoutLine, checkoutList);

                #endregion Checkout

                TabView view = new TabView()
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };

                view.AddTab(new TabView.Tab() { Text = "Users", View = usersView, }, true);
                view.AddTab(new TabView.Tab() { Text = "Groups", View = groupsView, }, false);
                view.AddTab(new TabView.Tab() { Text = "Checkin", View = checkinView, }, false);
                view.AddTab(new TabView.Tab() { Text = "Checkout", View = checkoutView, }, false);


                view.SelectedTabChanged += (obj, args) =>
                {
                    switch ((string)args.NewTab.Text)
                    {
                        case "Checkin":
                            UpdateList(checkinList, checkinTextField, false);
                            break;
                        case "Checkout":
                            UpdateList(checkoutList, checkoutTextField, true);
                            break;
                        default:
                            break;
                    }
                };

                Add(view);
            }


            private void UpdateList(ListView list, TextField textField, bool checkedin)
            {
                list.SetSource(users.AsEnumerable()
                    .Where(row => row.Field<bool>("Checked in") == checkedin)
                    .Select(row => row.Field<string>("Name"))
                    .Where(val => (val ?? "").ToLower().StartsWith((string)textField.Text.ToLower()))
                    .ToList()
                    );
            }
        }
    }
}