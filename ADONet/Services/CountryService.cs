 using ADONet.Constants;
using ADONet.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONet.Services
{
    public static class CountryService
    {
        public static void GetAllCountries()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
            {
                connection.Open();
                SqlCommand countCommand = new SqlCommand(" SELECT COUNT(*) FROM Countries WHERE IsDeleted=0 ", connection);
                int countryCount = Convert.ToInt32(countCommand.ExecuteScalar());
                if (countryCount == 0)
                    Messages.NotFoundMessage("Any", "Country");

                var command = new SqlCommand("SELECT * FROM Countries WHERE IsDeleted=0", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = Convert.ToString(reader["Name"]);
                        decimal area = Convert.ToDecimal(reader["Area"]);

                        Messages.PrintMessage("Name", name);
                        Messages.PrintMessage("Area", area.ToString());
                        Console.WriteLine();
                    }
                }
            }
        }
        public static void AddCountry()
        {
            Messages.InputMessage("Country Name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {

                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", connection);
                    selectCommand.Parameters.AddWithValue("@name", name.ToLower());
                    try
                    {
                        int id = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (id > 0)
                        {
                            Messages.AlreadyExistMessage("Country ", name);
                            return;
                        }
                        Messages.InputMessage("Country Area");
                        string areaInput = Console.ReadLine();
                        decimal area;
                        bool isSuccessed = decimal.TryParse(areaInput, out area);
                        if (isSuccessed)
                        {


                            var command = new SqlCommand("INSERT INTO Countries (Name,Area) VALUES(@name,@area)", connection);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@area", area);

                            var affectedRows = command.ExecuteNonQuery();
                            if (affectedRows > 0)
                            {
                                Messages.SuccesseMessage($"Country", name, CrudOperationType.Add);
                            }
                            else
                            {
                                Messages.ErrorOcuredMessage();
                            }

                        }
                        else
                        {
                            Messages.InvalidInputMessage(" Area");
                        }

                    }
                    catch (Exception)
                    {

                        Messages.ErrorOcuredMessage();
                    }
                }


            }
            else
            {
                Messages.InvalidInputMessage("Country Name");
            }
        }

        public static void UpdateCountry()
        {
            GetAllCountries();
            Messages.InputMessage("Country name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);
                    int id = Convert.ToInt32(command.ExecuteScalar());
                    if (id > 0)
                    {
                    NameWantToChangeSection: Messages.PrintWantToChangeMessage("Name");
                        var choiceForName = Console.ReadLine();
                        char choice;
                        bool isSuccessed = char.TryParse(choiceForName, out choice);
                        if (isSuccessed && choice.IsValidChoice())
                        {
                            string newName = string.Empty;
                            if (choice.Equals('y'))
                            {
                                Messages.InputMessage("new name");
                                newName = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(newName))
                                {
                                    var alreadyExistsCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name AND Id!=@id", connection);
                                    alreadyExistsCommand.Parameters.AddWithValue("@name", newName);
                                    alreadyExistsCommand.Parameters.AddWithValue("@id", id);
                                    int existId = Convert.ToInt32(alreadyExistsCommand.ExecuteScalar());
                                    if (existId > 0)
                                    {
                                        Messages.AlreadyExistMessage("Country", newName);
                                        goto NameWantToChangeSection;
                                    }


                                }
                                else
                                {
                                    Messages.InvalidInputMessage("new name");
                                    goto NameWantToChangeSection;
                                }
                            }

                        AreaWantToChangeSection: Messages.PrintWantToChangeMessage("Area");
                            var choiceForArea = Console.ReadLine();
                            isSuccessed = char.TryParse(choiceForArea, out choice);
                            decimal newArea = default;

                            if (isSuccessed && choice.IsValidChoice())
                            {
                                if (choice.Equals('y'))
                                {
                                    Messages.InputMessage("New area");
                                    string newAreaInput = Console.ReadLine();
                                    isSuccessed = decimal.TryParse(newAreaInput, out newArea);
                                    if (!isSuccessed)
                                    {
                                        Messages.InvalidInputMessage("area");
                                        goto AreaWantToChangeSection;
                                    }
                                }

                            }
                            else
                            {
                                Messages.InvalidInputMessage("Choice");
                                goto AreaWantToChangeSection;
                            };

                            var updateCommand = new SqlCommand(" UPDATE Countries SET ", connection);
                            if (newName != string.Empty || newArea != default)
                            {
                                bool isRequiredComma = false;
                                if (newName != string.Empty)
                                {
                                    isRequiredComma = true;
                                    updateCommand.CommandText = updateCommand.CommandText + " Name=@name";
                                    updateCommand.Parameters.AddWithValue("@name", newName);
                                }
                                if (newArea != default)
                                {
                                    updateCommand.CommandText = updateCommand.CommandText + $" {(isRequiredComma ? " , " : "")}  Area=@area";
                                    updateCommand.Parameters.AddWithValue("@Area", newArea);
                                }

                                updateCommand.CommandText = updateCommand.CommandText + "  WHERE Id=@id";
                                updateCommand.Parameters.AddWithValue("@Id", id);
                                int affectRows = Convert.ToInt32(updateCommand.ExecuteNonQuery());
                                if (affectRows > 0)
                                {
                                    Messages.SuccesseMessage("Country", newName, CrudOperationType.Update);
                                }
                                else
                                    Messages.ErrorOcuredMessage();
                            }
                        }
                        else
                        {
                            Messages.InvalidInputMessage("Choice");
                        }
                    }
                    else
                    {
                        Messages.NotFoundMessage("Country ", name);
                    }
                }
            }
            else
            {
                Messages.InvalidInputMessage("Country name");
            }

        }

        public static void DeleteCountry()
        {
            GetAllCountries();
            Messages.InputMessage(" Country name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(" SELECT * FROM Countries WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);
                    try
                    {
                        int id = Convert.ToInt32(command.ExecuteScalar());
                        if (id > 0)
                        {
                            //SqlCommand deleteCommmand = new SqlCommand(" DELETE FROM Countries WHERE Id=@id ", connection); HardDelete
                            SqlCommand deleteCommmand = new SqlCommand("  UPDATE  Countries SET IsDeleted=1 WHERE Id=@id ", connection);
                            deleteCommmand.Parameters.AddWithValue("@id", id);
                            int affectedRows = deleteCommmand.ExecuteNonQuery();
                            if (affectedRows > 0)
                                Messages.SuccesseMessage("Country", name, CrudOperationType.Delete);
                            else
                                Messages.ErrorOcuredMessage();
                        }
                        else
                            Messages.NotFoundMessage("Country", name);
                    }
                    catch (Exception)
                    {

                        Messages.ErrorOcuredMessage();
                    }
                }
            }
            else
            {
                Messages.InvalidInputMessage("name");
            }

        }
  
        public static void DetailsOfCountry()
        {
            GetAllCountries();
            Messages.InputMessage("Name");
            string name= Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT * FROM Countries WHERE Name=@name",connection);
                    command.Parameters.AddWithValue("@name",name);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                Messages.PrintMessage("Id",  Convert.ToString(reader["Id"]));
                                Messages.PrintMessage("Name", Convert.ToString(reader["Name"]));
                                Messages.PrintMessage("Area", Convert.ToString(reader["Area"]));
                            }
                        }
                    }
                    catch (Exception)
                    {

                        Messages.ErrorOcuredMessage();
                    }
                }
            }
            else
            {
                Messages.InvalidInputMessage("name");
            }
        }
    }
}
