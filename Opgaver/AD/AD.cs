using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Opgaver.AD
{
    public class ADProgram
    {
        public static void Run()
        {
            LdapConnection conn = NewConnectionToServer();
            conn.Bind();
            foreach (ADGroup group in GetAllGroups())
            {
                Console.WriteLine(group.Name);
            }
        }

        private static LdapConnection NewConnectionToServer() => 
            new LdapConnection(Enviroment.Get()["server"])
            {
                Credential = new NetworkCredential($"{Enviroment.Get()["username"]}@{Enviroment.Get()["domain"]}", Enviroment.Get()["password"]),
                AuthType = AuthType.Negotiate
            };

        public static List<ADGroup> GetAllGroups()
        {
            // Opret en tom liste til at gemme alle AD grupper
            var groups = new List<ADGroup>();

            // Opret forbindelse til Active Directory
            using (var connection = NewConnectionToServer())
            {
                // Definer søgningen:
                // - Hvor skal vi søge: i "mags.local" domænet
                // - Hvad søger vi efter: alle objekter af typen "group"
                // - Hvilke informationer vil vi have: 
                // - navn (cn) og beskrivelse
                var searchRequest = new SearchRequest(
                "DC=mags,DC=local", // Søg i dette domæne
                "(objectClass=group)", // Find alle grupper
                SearchScope.Subtree, // Søg i hele domænet
                "cn", // Gruppens navn
                  "description" // Gruppens beskrivelse
                  );

                try
                {
                    // Udfør søgningen
                    var response = (SearchResponse)connection.SendRequest(searchRequest);

                    // For hver gruppe vi finder
                    foreach (SearchResultEntry gruppe in response.Entries)
                    {
                        // Opret et nyt ADGroup objekt med informationerne
                        var nyGruppe = new ADGroup
                        {
                            // Hvis værdien ikke findes, brug "N/A" som standard
                            Name = gruppe.Attributes["cn"]?[0]?.ToString() ?? "N/A",
                            Description = gruppe.Attributes["description"]?[0]?.ToString() ?? "N/A"
                        };

                        // Tilføj gruppen til vores liste
                        groups.Add(nyGruppe);
                    }
                }
                catch (Exception ex)
                {
                    // Hvis noget går galt, fortæl hvad der skete
                    throw new Exception($@"Der skete en fejl ved hentning af grupper:
	     {ex.Message}");
                }
            }

            // Send alle de fundne grupper tilbage
            return groups;
        }


        public class ADGroup
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public static List<ADUser> GetAllUsers()
        {
            var users = new List<ADUser>();

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
                        var user = new ADUser
                        {
                            Name = entry.Attributes["cn"]?[0]?.ToString() ?? "N/A",
                            Username = entry.Attributes["samAccountName"]?[0]?.ToString()
                            ?? "N/A",
                            Email = entry.Attributes["mail"]?[0]?.ToString() ?? "N/A",
                            Department = entry.Attributes["department"]?[0]?.ToString()
                            ?? "N/A",
                            Title = entry.Attributes["title"]?[0]?.ToString() ?? "N/A",
                            DistinguishedName = entry.Attributes
                            ["distinguishedName"]?[0]?.ToString() ?? "N/A"
                        };

                        users.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fejl ved hentning af brugere: {ex.Message}");
                }
            }

            return users;
        }
        public class ADUser
        {
            public string Name { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Department { get; set; }
            public string Title { get; set; }
            public string DistinguishedName { get; set; }
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
    }
}
